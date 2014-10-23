using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Caliburn.Micro;
using log4net;
using Shop.Contracts.Messages;
using Shop.Contracts.Services;

namespace Card.Service.Business
{
    public class CardReadService : ICardReadService, IHandle<CardInserted>, IHandle<InvalidCardInserted>, IHandle<KeepAlive>
    {
        public CardReadService(ICardReader cardReader, IEventAggregator eventAggregator, ILog log)
        {
            CardReader = cardReader;
            EventAggregator = eventAggregator;
            Log = log;

            EventAggregator.Subscribe(this);
        }

        private readonly ICardReader CardReader;

        private readonly IEventAggregator EventAggregator;

        private readonly ILog Log;

        private ICardHandler ClientCallback;

        public void Connect()
        {
            ClientCallback = OperationContext.Current.GetCallbackChannel<ICardHandler>();            
        }

        public void Handle(CardInserted message)
        {
            if (ClientCallback != null)
                ClientCallback.HandleCardInserted(message);
        }

        public void Handle(InvalidCardInserted message)
        {
            if (ClientCallback != null)
                ClientCallback.HandleInvalidCardInserted(message);
        }

        public void Handle(KeepAlive message)
        {
            Log.Debug("Keep alive received");

            if (ClientCallback != null)
                ClientCallback.HandleKeepAlive();
        }
    }
}