using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Caliburn.Micro;
using Shop.Contracts.Messages;
using Shop.Contracts.Services;

namespace Card.Service.Business
{
    public class CardReadService : ICardReadService, IHandle<CardInserted>, IHandle<InvalidCardInserted>
    {
        public CardReadService(ICardReader cardReader, IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
            CardReader = cardReader;

            EventAggregator.Subscribe(this);
        }

        private ICardReader CardReader;

        private IEventAggregator EventAggregator;

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
    }
}