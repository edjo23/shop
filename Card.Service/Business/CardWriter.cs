using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Caliburn.Micro;
using log4net;
using NdefLibrary.Ndef;
using PCSC;
using PCSC.Iso7816;

namespace Card.Service.Business
{    
    public interface ICardWriter
    {
        string Write();
    }

    public class CardWriter : ICardWriter
    {
        public CardWriter(ILog log)
        {
            Log = log;

            Log.Debug("CardWriter constructed");

            Connect();

        }

        private readonly ILog Log;

        public SCardContext CardContext { get; set; }

        public void Connect()
        {
            Log.Debug("Connecting card reader");

            try
            {
                CardContext = new SCardContext();
                CardContext.Establish(SCardScope.System);
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

        public string Write()
        {
            Log.Debug("Card write");

            try
            {
                using (var reader = new IsoReader(CardContext, GetReader(), SCardShareMode.Shared, SCardProtocol.Any, false))
                {
                    var status = GetReaderStatus(reader);

                    Log.Debug(String.Format("Card State: {0}", status.CardState));

                    if (!status.CardState.HasFlag(SCardState.Present))
                        return null;

                    var id = GetCardId(reader);
                    var cardName = GetCardName(status.Atr);
                    var cardType = GetInt16(cardName);

                    var isMifare = cardType == CardReader.Mifare1KCard || cardType == CardReader.Mifare4KCard;
                    var isMifareUltralight = cardType == CardReader.MifareUltralightCard;

                    Log.Debug(String.Format("Card Id: {0}", BitConverter.ToString(id)));

                    var cardString = BitConverter.ToString(cardName.Concat(id).ToArray()).Replace("-", "");

                    if (isMifareUltralight)
                    {
                        var msg = new NdefMessage { new NdefUriRecord { Uri = CardReader.CardUri + "/#/" + cardString } };
                        var data = msg.ToByteArray();
                        var buffer = new List<byte>(new byte[] { 0x03, (byte)data.Length }.Concat(data));

                        WriteAllCardBytes(reader, buffer.ToArray(), isMifareUltralight ? 4 : 16);
                    }

                    return BitConverter.ToString(cardName.Concat(id).ToArray()).Replace("-", "");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return null;
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

        private void WriteAllCardBytes(IsoReader isoReader, byte[] bytes, int packetSize)
        {
            var bytesToWrite = new List<byte>(bytes);

            //while (bytesToWrite.Count < 38 * 4)
            //    bytesToWrite.Add(0x00);

            while (bytesToWrite.Count % packetSize != 0)
                bytesToWrite.Add(0x00);

            for (int i = 0; i < bytesToWrite.Count / packetSize; i++)
            {
                var updateBinaryCmd = new CommandApdu(IsoCase.Case3Short, SCardProtocol.Any)
                {
                    CLA = 0xFF,
                    Instruction = InstructionCode.UpdateBinary,
                    P1 = 0x00,
                    P2 = (byte)((16 / packetSize) + i),
                    Data = bytesToWrite.Skip(i * packetSize).Take(packetSize).ToArray()
                };

                var response = isoReader.Transmit(updateBinaryCmd);

                Console.WriteLine("UpdateBinary: {0},{1}", response.SW1, response.SW2);
            }
        }

    }
}