using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;
using Shop.PointOfSale.Messages;
using Shop.PointOfSale.Services;

namespace Shop.PointOfSale.ViewModels
{
    public class SaleViewModel : Screen
    {
        public SaleViewModel(IEventAggregator eventAggregator, ScreenCoordinator screenCoordinator, ICustomerService customerService, IProductService productService, IInvoiceService invoiceService)        
        {
            EventAggregator = eventAggregator;
            ScreenCoordinator = screenCoordinator;
            CustomerService = customerService;
            ProductService = productService;
            InvoiceService = invoiceService;

            DisplayName = "New Purchase";
            Products = new BindableCollection<SaleItemViewModel>();
        }

        private readonly IEventAggregator EventAggregator;

        private readonly ScreenCoordinator ScreenCoordinator;

        private readonly ICustomerService CustomerService;

        private readonly IProductService ProductService;

        private readonly IInvoiceService InvoiceService;

        public Customer Customer { get; set; }

        public bool IsCashAccount
        {
            get
            {
                return Customer != null && String.Equals(Customer.Name, "Cash", StringComparison.InvariantCultureIgnoreCase);
            }
        }

        public BindableCollection<SaleItemViewModel> Products { get; set; }

        public decimal TotalQuantity
        {
            get
            {
                return Products.Sum(o => o.Quantity); ;
            }
        }

        public decimal Total
        {
            get
            {
                return Products.Sum(o => o.Total);
            }
        }

        public string TotalText
        {
            get
            {
                return Total.ToString("C");
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            EventAggregator.Publish(new ShowDialog { Screen = IoC.Get<LoadingViewModel>() });

            Task.Factory.StartNew(() =>
            {
                Products.AddRange(ProductService.GetProducts().Select(o => new SaleItemViewModel { Product = o }));

                EventAggregator.Publish(new HideDialog { });
            });
        }

        public void AddToCart(SaleItemViewModel item)
        {
            item.Quantity += 1;

            NotifyOfPropertyChange(() => TotalQuantity);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => TotalText);
        }

        public void RemoveFromCart(SaleItemViewModel item)
        {
            item.Quantity -= 1;

            NotifyOfPropertyChange(() => TotalQuantity);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => TotalText);
        }

        public void Checkout()
        {
            var messageBox = IoC.Get<MessageBoxViewModel>();
            messageBox.DisplayName = "";
            messageBox.Content = "";

            EventAggregator.Publish(new ShowDialog { Screen = messageBox });

            Task.Factory.StartNew(() =>
                {
                    var invoice = new Invoice
                    {
                        DateTime = DateTimeOffset.Now,
                        CustomerId = Customer.Id
                    };

                    var invoiceItems = new List<InvoiceItem>(Products
                        .Where(o => o.Quantity > 0)
                        .Select((o, i) => new InvoiceItem
                        {
                            ItemNumber = i + 1,
                            ProductId = o.Product.Id,
                            Price = o.Product.Price,
                            Quantity = o.Quantity
                        }));

                    InvoiceService.AddInvoice(invoice, invoiceItems, IsCashAccount ? Total : 0.0m);

                    Customer.Balance = CustomerService.GetCustomer(Customer.Id).Balance;
                })
            .ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        Execute.OnUIThread(() =>
                            {
                                messageBox.Background = System.Windows.Media.Brushes.Firebrick;
                                messageBox.DisplayName = "Sorry, an error has occurred :(";
                                messageBox.Content = "The purchase was not be processed";
                            });
                    }
                    else
                    {
                        Execute.OnUIThread(() =>
                            {
                                messageBox.DisplayName = "Thank you :)";
                                messageBox.Content = String.Format("Your new balance is now {0:C}", Customer.Balance);
                            });

                        System.Threading.Thread.Sleep(5000);

                        Execute.OnUIThread(() =>
                            {
                                ScreenCoordinator.GoToHome();
                            });
                    }
                });
        }
    }
}
