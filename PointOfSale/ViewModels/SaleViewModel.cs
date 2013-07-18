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
        public SaleViewModel(IEventAggregator eventAggregator, ScreenCoordinator screenCoordinator, ICustomerService customerService, IProductService productService, IInvoiceService invoiceService, IDiscountService discountService)        
        {
            EventAggregator = eventAggregator;
            ScreenCoordinator = screenCoordinator;
            CustomerService = customerService;
            ProductService = productService;
            InvoiceService = invoiceService;
            DiscountService = discountService;

            DisplayName = "Purchase";
            Products = new BindableCollection<SaleItemViewModel>();
        }

        private readonly IEventAggregator EventAggregator;

        private readonly ScreenCoordinator ScreenCoordinator;

        private readonly ICustomerService CustomerService;

        private readonly IProductService ProductService;

        private readonly IInvoiceService InvoiceService;

        private readonly IDiscountService DiscountService;

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

        public bool CanCheckout
        {
            get
            {
                return Products.Any(o => o.Quantity > 0);
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            EventAggregator.Publish(new ShowDialog { Screen = IoC.Get<LoadingViewModel>() });

            Task.Factory.StartNew(() =>
            {
                var products = ProductService.GetProducts();
                var discounts = DiscountService.GetDiscounts();
                var discountProducts = DiscountService.GetDiscountProductsByCustomerId(Customer.Id);

                Products.AddRange(products.Select(o => new SaleItemViewModel { Product = o, Discount = discountProducts.Where(p => p.ProductId == o.Id).Select(p => p.Discount).DefaultIfEmpty(0.0m).Max() }));

                EventAggregator.Publish(new HideDialog { });
            });
        }

        public void AddToCart(SaleItemViewModel item)
        {
            UpdateQuantity(item, 1);
        }

        public void RemoveFromCart(SaleItemViewModel item)
        {
            UpdateQuantity(item, -1);
        }

        protected void UpdateQuantity(SaleItemViewModel item, int value)
        {
            item.Quantity += value;

            NotifyOfPropertyChange(() => CanCheckout);
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
                            Quantity = o.Quantity,
                            Price = o.Product.Price,
                            Discount = o.Discount
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
                                messageBox.Content = "The purchase was not processed";
                            });
                    }
                    else
                    {
                        Execute.OnUIThread(() =>
                            {
                                messageBox.DisplayName = "Thank you";
                                messageBox.Content = IsCashAccount ? "" : String.Format("Your balance is now {0:C}", Customer.Balance);
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
