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
    public class CustomerReceiptViewModel : Screen
    {
        public CustomerReceiptViewModel(IEventAggregator eventAggregator, ICustomerService customerService, IProductService productService, IReceiptService receiptService)
        {
            EventAggregator = eventAggregator;
            CustomerService = customerService;
            ProductService = productService;
            ReceiptService = receiptService;

            DisplayName = "New Stock Payment";
            Items = new BindableCollection<CustomerReceiptItemViewModel>();
        }

        public IEventAggregator EventAggregator { get; set; }

        public ICustomerService CustomerService { get; set; }

        public IProductService ProductService { get; set; }

        public IReceiptService ReceiptService { get; set; }

        public BindableCollection<CustomerReceiptItemViewModel> Items { get; set; }

        public Customer Customer { get; set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Items.AddRange(ProductService.GetProducts().Select(o => new CustomerReceiptItemViewModel { Product = o }));
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
                .Select((o, i) => new InvoiceItem
                {
                    ItemNumber = i + 1,
                    ProductId = o.Product.Id,
                    Price = o.Product.Cost,
                    Quantity = o.Quantity
                }));

            ReceiptService.AddReceipt(new InvoiceTransaction { Invoice = invoice, Items = invoiceItems });

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
