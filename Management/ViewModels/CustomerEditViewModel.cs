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
    public class CustomerEditViewModel : Screen
    {
        public CustomerEditViewModel(IEventAggregator eventAggregator, ICustomerService customerService)
        {
            EventAggregator = eventAggregator;
            CustomerService = customerService;

            DisplayName = "Edit Customer";
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

            Name = Customer.Name;
            Number = Customer.Number;
        }

        public void Save()
        {
            var customer = CustomerService.GetCustomer(Customer.Id);
            customer.Name = Name;
            customer.Number = Number;

            CustomerService.UpdateCustomer(customer);

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
