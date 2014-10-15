using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Caliburn.Micro;
using log4net;
using NdefLibrary.Ndef;
using PCSC;
using PCSC.Iso7816;
using Shop.Contracts.Messages;
using Shop.Contracts.Services;

namespace Card.Service.Business
{
    public class CardRemoved
    {
    }

    public class ReaderStatus
    {
        public string[] ReaderName { get; set; }

        public SCardState CardState { get; set; }

        public SCardProtocol Protocol { get; set; }

        public byte[] Atr { get; set; }
    }

    public interface ICardReader
    {
    }

    public class CardReader : ICardReader, IDisposable
    {
        public const ushort Mifare1KCard = 1;
        public const ushort Mifare4KCard = 2;
        public const ushort MifareUltralightCard = 3;
        public const string CardUri = "http://has.azurewebsites.net";

        public CardReader(ILog log, IEventAggregator eventAggregator)
        {
            Log = log;
            EventAggregator = eventAggregator;

            Log.Debug("CardReader constructed");

            Connect();
        }

        private readonly ILog Log;

        private readonly IEventAggregator EventAggregator;

        public SCardContext CardContext { get; set; }

        public SCardMonitor CardMonitor { get; set; }

        public void Dispose()
        {
            Disconnect();
        }

        public void Connect()
        {
            Log.Debug("Connecting card reader");

            try
            {
                CardContext = new SCardContext();
                CardContext.Establish(SCardScope.System);

                CardMonitor = new SCardMonitor(CardContext);
                CardMonitor.CardInserted += CardInserted;
                CardMonitor.CardRemoved += CardRemoved;

                CardMonitor.Start(GetReader());
            }
            catch (Exception)
            {
                Disconnect();
            }

            if (!CardContext.IsValid())
            {
                // TODO - Log.
                Disconnect();
            }

            Log.Info("Card reader connected");
        }

        public void Disconnect()
        {
            if (CardMonitor != null)
            {
                try
                {
                    CardMonitor.Cancel();
                    CardMonitor.CardInserted -= CardInserted;
                    CardMonitor.CardInserted -= CardRemoved;
                }
                catch (Exception)
                {
                }
                finally
                {
                    CardMonitor = null;
                }
            }

            if (CardContext != null)
            {
                try
                {
                    CardContext.Release();
                }
                catch (Exception)
                {
                }
                finally
                {
                    CardContext = null;
                }
            }
        }

        private string GetReader()
        {
            return CardContext.GetReaders().FirstOrDefault();
        }

        private ReaderStatus GetReaderStatus(IsoReader reader)
        {
            var readerName = new string[0];
            var cardState = SCardState.Unknown;
            var protocol = SCardProtocol.Unset;
            var atr = new byte[0];

            var error = reader.Reader.Status(out readerName, out cardState, out protocol, out atr);

            return new ReaderStatus { ReaderName = readerName, CardState = cardState, Protocol = protocol, Atr = atr };
        }

        private ushort GetInt16(byte[] buffer)
        {
            if (buffer.Length == 0)
                return 0;
            else if (buffer.Length == 1)
                return buffer.First();
            else
                return BitConverter.ToUInt16(buffer.Reverse().Take(2).ToArray(), 0);
        }

        public byte[] GetCardName(byte[] atr)
        {
            var result = new List<byte>();

            // Valid ATR?
            if (atr.Length >= 15 && atr[0] == 0x3B)
            {
                // Historical byte count.
                var cnt = atr[1] & 0x0F;

                // Card name is 
                result.AddRange(atr.Skip(3 + cnt - 5).Take(2));
            }

            return result.ToArray();
        }

        public byte[] GetCardId(IsoReader reader)
        {
            var command = new CommandApdu(IsoCase.Case2Short, SCardProtocol.Any)
            {
                CLA = 0xFF,
                Instruction = InstructionCode.GetData,
                P1 = 0x00,
                P2 = 0x00,
                Le = 0x00
            };

            var response = reader.Transmit(command);

            return response.GetData();
        }

        private byte[] GetAllCardBytes(IsoReader reader, int packetSize)
        {
            try
            {
                var firstDataBlock = 16 / packetSize;
                var readSize = 16;
                var bytesToRead = 0;
                var buffer = new List<byte>();

                while (true)
                {
                    var blockToRead = (byte)(firstDataBlock + (buffer.Count / packetSize));

                    var readBinaryCmd = new CommandApdu(IsoCase.Case2Short, SCardProtocol.Any)
                    {
                        CLA = 0xFF,
                        Instruction = InstructionCode.ReadBinary,
                        P1 = 0x00,
                        P2 = blockToRead,
                        Le = readSize
                    };

                    var response = reader.Transmit(readBinaryCmd);

                    var data = response.GetData();

                    if (buffer.Count == 0)
                        bytesToRead = data[1] + 1 + 1;

                    buffer.AddRange(data.Take(bytesToRead - buffer.Count < readSize ? bytesToRead - buffer.Count : readSize).ToArray());
                    if (buffer.Count >= bytesToRead)
                        break;
                }

                Log.Debug(String.Format("ReadBinary: {0}", BitConverter.ToString(buffer.ToArray())));
                Log.Debug(String.Format("Buffersize: Reported: {0}, Actual: {1}", bytesToRead, buffer.Count));

                return buffer.ToArray();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return new byte[0];
        }

        private bool IsShopCard(byte[] buffer)
        {
            // NDEF - byte 0 should be 0x03, byte 1 should be length of remaining bytes.
            if (buffer.Length < 2 || buffer[0] != 0x03 || buffer[1] != buffer.Length - 2)
                return false;

            try
            {
                var msg = NdefMessage.FromByteArray(buffer.Skip(2).ToArray());

                if (msg.Count > 0 && msg.First().CheckSpecializedType(false) == typeof(NdefUriRecord))
                {
                    var record = new NdefUriRecord(msg.First());

                    return record.Uri.StartsWith(CardUri);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return false;
        }

        private void CardInserted(object sender, CardStatusEventArgs e)
        {
            Log.Debug("Card inserted");

            try
            {
                using (var reader = new IsoReader(CardContext, GetReader(), SCardShareMode.Shared, SCardProtocol.Any, false))
                {
                    var id = GetCardId(reader);
                    var status = GetReaderStatus(reader);

                    var cardName = GetCardName(status.Atr);
                    var cardType = GetInt16(cardName);

                    var isMifare = cardType == Mifare1KCard || cardType == Mifare4KCard;
                    var isMifareUltralight = cardType == MifareUltralightCard;

                    //var bytes = GetAllCardBytes(reader, isMifareUltralight ? 4 : 16);
                    var isShopCard = true; // IsShopCard(bytes);

                    Log.Debug(String.Format("Card Id: {0}, Shop Card: {1}", BitConverter.ToString(id), isShopCard));

                    if (isShopCard)
                    {
                        var cardString = BitConverter.ToString(cardName.Concat(id).ToArray()).Replace("-", "");

                        EventAggregator.Publish(new CardInserted { CardId = cardString });
                    }
                    else
                    {
                        EventAggregator.Publish(new InvalidCardInserted { });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);

                EventAggregator.Publish(new InvalidCardInserted { });
            }
        }

        private void CardRemoved(object sender, CardStatusEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}