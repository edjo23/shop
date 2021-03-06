﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using PointOfSale.RT.Messages;
using PointOfSale.RT.Models;
using PointOfSale.RT.Services;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;
using Windows.UI.Xaml;

namespace PointOfSale.RT.ViewModels
{
    public class SaleViewModel : Screen, IEnabled
    {
        public SaleViewModel(ScreenCoordinator screenCoordinator, ImageService imageService, ICustomerService customerService, IProductService productService, IInvoiceService invoiceService, IDiscountService discountService, IEventAggregator eventAggregator)
        {
            ScreenCoordinator = screenCoordinator;
            ImageService = imageService;
            CustomerService = customerService;
            ProductService = productService;
            InvoiceService = invoiceService;
            DiscountService = discountService;
            EventAggregator = eventAggregator;

            DisplayName = "PURCHASE";
            Products = new BindableCollection<SaleItemViewModel>();
        }

        private readonly ScreenCoordinator ScreenCoordinator;

        private readonly ImageService ImageService;

        private readonly ICustomerService CustomerService;

        private readonly IProductService ProductService;

        private readonly IInvoiceService InvoiceService;

        private readonly IDiscountService DiscountService;

        private readonly IEventAggregator EventAggregator;

        private bool _IsEnabled = true;

        public bool IsEnabled
        {
            get
            {
                return _IsEnabled;
            }
            set
            {
                if (value != _IsEnabled)
                {
                    _IsEnabled = value;

                    NotifyOfPropertyChange(() => IsEnabled);
                }
            }
        }

        private bool _IsLoading = false;

        public bool IsLoading
        {
            get
            {
                return _IsLoading;
            }
            set
            {
                if (value != _IsLoading)
                {
                    _IsLoading = value;

                    NotifyOfPropertyChange(() => IsLoading);
                    NotifyOfPropertyChange(() => LoadingVisibility);
                    NotifyOfPropertyChange(() => ContentVisibility);
                }
            }
        }

        public Visibility LoadingVisibility
        {
            get
            {
                return IsLoading ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility ContentVisibility
        {
            get
            {
                return IsLoading ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public Customer Customer { get; set; }

        public bool IsCashAccount
        {
            get
            {
                return Customer != null && String.Equals(Customer.Name, "Cash", StringComparison.CurrentCultureIgnoreCase);
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

        public void Load()
        {
            var products = ProductService.GetProducts();
            var discounts = DiscountService.GetDiscounts();
            var discountProducts = DiscountService.GetDiscountProductsByCustomerId(Customer.Id);

            Products.AddRange(products.Select(o => new SaleItemViewModel(o)
            { 
                Discount = discountProducts.Where(p => p.ProductId == o.Id).Select(p => p.Discount).DefaultIfEmpty(0.0m).Max(),
                ImageSource = ImageService.GetImage(o.Code)
            }));
        }

        public void AddItem(SaleItemViewModel item)
        {
            UpdateQuantity(item, 1);
        }

        public void RemoveItem(SaleItemViewModel item)
        {
            UpdateQuantity(item, -1);
        }

        protected void UpdateQuantity(SaleItemViewModel item, int value)
        {
            var totalQuantity = TotalQuantity;

            item.Quantity += value;

            NotifyOfPropertyChange(() => CanCheckout);
            NotifyOfPropertyChange(() => TotalQuantity);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => TotalText);

            if (totalQuantity == 0 && value > 0)
                EventAggregator.Publish(new TransactionStarted { Source = this });

            if (TotalQuantity == 0)
                EventAggregator.Publish(new TransactionStopped { Source = this });
        }

        public void Checkout()
        {
            var processvm = IoC.Get<ProcessViewModel>();
            processvm.Content = "Processing...";

            processvm.ProcessAction = () =>
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
            };

            processvm.CompleteAction = () =>
            {
                var message = IoC.Get<MessageBoxViewModel>();
                message.Content = new CustomerTransactionInfo { Customer = Customer, NewBalance = Customer.Balance };
                message.DismissAction = () => ScreenCoordinator.NavigateToHome();
                message.DismissTimeout = 2500;

                ScreenCoordinator.NavigateToScreen(message);
            };

            ScreenCoordinator.NavigateToScreen(processvm);
        }
    }

}
