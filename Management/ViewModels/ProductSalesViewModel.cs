using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;

namespace Shop.Management.ViewModels
{
    public class ProductSalesViewModel : Screen
    {
        public ProductSalesViewModel(IInvoiceService invoiceService)
        {
            InvoiceService = invoiceService;

            Items = new BindableCollection<dynamic>();
        }

        public IInvoiceService InvoiceService { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public BindableCollection<dynamic> Items { get; set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Load();
        }

        protected void Load()
        {
            Items.Clear();
            Items.AddRange(InvoiceService.GetInvoiceItemHistory(DateTime.Today));
        }

        public void RefreshView()
        {
            Load();
        }
    }
}
