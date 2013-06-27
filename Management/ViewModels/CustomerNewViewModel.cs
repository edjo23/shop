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
    public class CustomerNewViewModel : Screen
    {
        public CustomerNewViewModel(IEventAggregator eventAggregator, ICustomerService customerService)
        {
            CustomerService = customerService;
            EventAggregator = eventAggregator;

            DisplayName = "New Customer";
        }

        public IEventAggregator EventAggregator { get; set; }

        public ICustomerService CustomerService { get; set; }

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

        #region Number Property

        private string _Number;

        public string Number
        {
            get
            {
                return _Number;
            }
            set
            {
                if (value != _Number)
                {
                    _Number = value;
                    NotifyOfPropertyChange(() => Number);
                }
            }
        }

        #endregion

        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        public void Save()
        {
            var customer = new Customer { Name = Name, Number = Number };

            CustomerService.AddCustomer(customer);

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
