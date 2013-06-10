using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;
using Shop.Management.Messages;

namespace Shop.Management.ViewModels
{
    public class CustomerAccountPaymentViewModel : Screen
    {
        public CustomerAccountPaymentViewModel(IEventAggregator eventAggregator, ICustomerService customerService)
        {
            EventAggregator = eventAggregator;
            CustomerService = customerService;

            DisplayName = "New Payment Receipt";
        }

        public IEventAggregator EventAggregator { get; set; }

        public ICustomerService CustomerService { get; set; }

        public Customer Customer { get; set; }

        #region Name Property

        private string _Name;

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if (value != _Name)
                {
                    _Name = value;
                    NotifyOfPropertyChange(() => Name);
                }
            }
        }

        #endregion

        #region Amount Property

        private decimal _Amount;

        public decimal Amount
        {
            get
            {
                return _Amount;
            }
            set
            {
                if (value != _Amount)
                {
                    _Amount = value;
                    NotifyOfPropertyChange(() => Amount);
                }
            }
        }

        #endregion

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Name = Customer.Name;
        }

        public void Save()
        {            
            var payment = new CustomerTransaction
            {
                Id = Guid.NewGuid(),
                CustomerId = Customer.Id,
                DateTime = DateTimeOffset.Now,
                Type = CustomerTransactionType.Payment,
                Amount = Amount * -1
            };

            CustomerService.AddTransaction(payment);

            Close();
        }

        public void Cancel()
        {
            Close();
        }

        public void Close()
        {
            EventAggregator.Publish(new DeactivateItem { Item = this });
        }
    }
}
