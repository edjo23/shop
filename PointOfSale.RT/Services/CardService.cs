using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Shop.Contracts.Entities;
using Shop.Contracts.Messages;
using Shop.Contracts.Services;

namespace PointOfSale.RT.Services
{
    public class CardReadHandler : ICardHandler
    {
        public CardReadHandler(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
        }

        private readonly IEventAggregator EventAggregator;

        public void HandleKeepAlive()
        {
            System.Diagnostics.Debug.WriteLine("Keep Alive - {0}", DateTime.Now);
        }

        public void HandleCardInserted(CardInserted message)
        {
            EventAggregator.Publish(message, Execute.OnUIThread);
        }

        public void HandleInvalidCardInserted(InvalidCardInserted message)
        {
        }
    }

    public class CardService
    {
        public CardService(ICardReadService cardReadService, ICardWriteService cardWriteService, ICustomerService customerService)
        {
            CardReadService = cardReadService;
            CardWriteService = cardWriteService;
            CustomerService = customerService;

            Connect();
        }

        private readonly ICardReadService CardReadService;

        private readonly ICardWriteService CardWriteService;

        private readonly ICustomerService CustomerService;

        private void Connect()
        {
            var cardReaderHost = (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["CardReaderHostAddress"];

            if (!String.IsNullOrEmpty(cardReaderHost))
            {
                Task.Factory.StartNew(() =>
                {
                    CardReadService.Connect();
                });
            }
        }

        public void Write(Customer customer)
        {
            var card = CardWriteService.Write();

            if (!String.IsNullOrEmpty(card))
            {
                var current = CustomerService.GetCustomer(customer.Id);
                customer.Number = card;

                CustomerService.UpdateCustomer(customer);
            }
        }
    }
}
