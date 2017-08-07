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
    public class CustomerInvoiceViewModel : Screen
    {
        public CustomerInvoiceViewModel(IEventAggregator eventAggregator, ICustomerService customerService, IProductService productService, IInvoiceService invoiceService)
        {
            EventAggregator = eventAggregator;
            CustomerService = customerService;
            ProductService = productService;
            InvoiceService = invoiceService;

            DisplayName = "New Invoice";
            Items = new BindableCollection<CustomerInvoiceItemViewModel>();
        }

        public IEventAggregator EventAggregator { get; set; }

        public ICustomerService CustomerService { get; set; }

        public IProductService ProductService { get; set; }

        public IInvoiceService InvoiceService { get; set; }

        public BindableCollection<CustomerInvoiceItemViewModel> Items { get; set; }

        public Customer Customer { get; set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Items.AddRange(ProductService.GetProducts().Select(o => new CustomerInvoiceItemViewModel { Product = o }));
        }

        public void Save()
        {
            var invoice = new Invoice
            {
                DateTime = DateTimeOffset.Now,
                CustomerId = Customer.Id
            };

            var invoiceItems = new List<InvoiceItem>(Items
                .Where(o => o.Quantity > 0)
                .Select((o,i) => new InvoiceItem
                {
                    ItemNumber = i + 1,
                    ProductId = o.Product.Id,
                    Price = o.Product.Price,
                    Quantity = o.Quantity
                }));

            InvoiceService.AddInvoice(new InvoiceTransaction { Invoice = invoice, Items = invoiceItems, Payment = 0 });

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
