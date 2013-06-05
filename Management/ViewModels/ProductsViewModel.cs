using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;
using Shop.Management.Messages;

namespace Shop.Management.ViewModels
{
    public class ProductsViewModel : Screen
    {
        public ProductsViewModel(IProductService productService, IEventAggregator eventAggregator)
        {
            ProductService = productService;
            EventAggregator = eventAggregator;

            EventAggregator = IoC.Get<IEventAggregator>();

            DisplayName = "Products";
            Products = new BindableCollection<Product>();
        }

        public IProductService ProductService { get; set; }

        public IEventAggregator EventAggregator { get; set; }

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
                    NotifyOfPropertyChange(() => CanRefresh);
                    NotifyOfPropertyChange(() => CanNew);
                }
            }
        }

        public Visibility LoadingVisibility
        {
            get
            {
                return IsLoading ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public bool CanRefresh
        {
            get
            {
                return !IsLoading;
            }
        }

        public bool CanNew
        {
            get
            {
                return !IsLoading;
            }
        }

        public BindableCollection<Product> Products { get; set; }

        private Product _SelectedProduct = null;

        public Product SelectedProduct
        {
            get
            {
                return _SelectedProduct;
            }
            set
            {
                if (value != _SelectedProduct)
                {
                    _SelectedProduct = value;

                    NotifyOfPropertyChange(() => SelectedProduct);
                    NotifyOfPropertyChange(() => ItemCommandVisibility);
                }
            }
        }

        public Visibility ItemCommandVisibility
        {
            get
            {
                return SelectedProduct == null ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Load(false); 
        }

        protected void Load(bool refresh)
        {
            if (!refresh && Products.Count > 0)
            {
                IsLoading = false;
                return;
            }

            IsLoading = true;

            Task.Factory.StartNew(() =>
            {
                Products.Clear();
                //System.Threading.Thread.Sleep(2000);
                Products.AddRange(ProductService.GetProducts());
            }).ContinueWith(t =>
            {
                Execute.OnUIThread(() =>
                {
                    IsLoading = false;
                });
            });
        }

        public void Refresh()
        {
            if (CanRefresh)
                Load(true);
        }

        public void New()
        {
            EventAggregator.Publish(new ActivateItem<ProductNewViewModel>());
        }

        public void Edit(Product product)
        {
            EventAggregator.Publish(new ActivateItem<ProductEditViewModel>());
        }

        public void Receipt(Product product)
        {
            EventAggregator.Publish(new ActivateItem<ProductReceiptViewModel>(o => o.Product = product));
        }
    }
}
