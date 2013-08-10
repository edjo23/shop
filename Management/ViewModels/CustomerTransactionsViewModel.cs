using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;
using Shop.Management.Messages;
using Shop.Management.Models;

namespace Shop.Management.ViewModels
{
    public class CustomerTransactionsViewModel : Screen
    {
        public CustomerTransactionsViewModel(IEventAggregator eventAggregator, ICustomerService customerService)
        {
            EventAggregator = eventAggregator;
            CustomerService = customerService;

            DisplayName = "Transactions";
            Items = new BindableCollection<CustomerTransactionModel>();
        }

        public IEventAggregator EventAggregator { get; set; }

        public ICustomerService CustomerService { get; set; }

        public Customer Customer { get; set; }

        public BindableCollection<CustomerTransactionModel> Items { get; set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            DisplayName = String.Format("{0} {1}", Customer.Name, DisplayName);

            Task.Factory.StartNew(() => Load());
        }

        private void Load()
        {
            var transactions = CustomerService.GetTransactions(Customer.Id, null, null);

            Items.Clear();
            Items.AddRange(transactions.OrderByDescending(o => o.DateTime).Select(o => new CustomerTransactionModel(o, Customer)));
        }

        public void Close()
        {
            EventAggregator.Publish(new DeactivateItem { Item = this });
        }
    }
}
