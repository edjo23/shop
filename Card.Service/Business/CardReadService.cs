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
        public const int KeepAliveInterval = 1000 * 60 * 1;

        public CardReadService(ICardReader cardReader, IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
            CardReader = cardReader;

            EventAggregator.Subscribe(this);
        }

        private readonly ICardReader CardReader;

        private readonly IEventAggregator EventAggregator;

        private ICardHandler ClientCallback;

        private Timer KeepAliveTimer;

        public void Connect()
        {
            ClientCallback = OperationContext.Current.GetCallbackChannel<ICardHandler>();

            //KeepAliveTimer = new Timer(s => 
            //{
            //    if (ClientCallback != null)
            //        ClientCallback.HandleKeepAlive();
            //    else
            //        KeepAliveTimer.Change(Timeout.Infinite, KeepAliveInterval);
            //},
            //null, KeepAliveInterval, KeepAliveInterval);
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