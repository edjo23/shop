using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;
using Shop.Management.Models;

namespace Shop.Management.ViewModels
{
    public class TodaysCustomerTransactionsViewModel : Screen
    {
        public TodaysCustomerTransactionsViewModel(ICustomerService customerService, IInvoiceService invoiceService)
        {
            CustomerService = customerService;
            InvoiceService = invoiceService;

            Items = new BindableCollection<CustomerTransactionModel>();
        }

        public ICustomerService CustomerService { get; set; }

        public IInvoiceService InvoiceService { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public BindableCollection<CustomerTransactionModel> Items { get; set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Load();
        }

        protected void Load()
        {
            Items.Clear();

            Task.Factory.StartNew(() =>
                {
                    var customers = CustomerService.GetAllCustomers();
                    var transactions = CustomerService.GetTransactions(null, DateTimeOffset.Now.Date, null);

                    Items.AddRange(transactions.OrderByDescending(o => o.DateTime).Select(o => new CustomerTransactionModel(o, customers.FirstOrDefault(c => c.Id == o.CustomerId))));
                });
        }

        public void RefreshView()
        {
            Load();
        }
    }
}
