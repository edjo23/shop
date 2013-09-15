using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using PointOfSale.RT.Services;
using Shop.Contracts.Services;
using Windows.UI.Xaml;

namespace PointOfSale.RT.ViewModels
{
    public abstract class PopupViewModel : PropertyChangedBase
    {
        public abstract void Activate();
    }

    public class InvoicePopupViewModel : PopupViewModel
    {
        public InvoicePopupViewModel(IInvoiceService invoiceService, IProductService productService)
        {
            InvoiceService = invoiceService;
            ProductService = productService;

            Items = new BindableCollection<SaleItemViewModel>();
        }

        private readonly IInvoiceService InvoiceService;

        private readonly IProductService ProductService;

        public Guid InvoiceId { get; set; }

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
                return !IsLoading ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public BindableCollection<SaleItemViewModel> Items { get; set; }

        public string TotalText
        {
            get
            {
                return Items.Sum(o => o.Total).ToString("C");
            }
        }

        public override void Activate()
        {
            Load();
        }

        private void Load()
        {
            IsLoading = true;

            Task.Factory.StartNew(() =>
                {
                    var items = InvoiceService.GetInvoiceItems(InvoiceId);
                    var products = ProductService.GetProducts(); // TODO - make this efficient (maybe product description should be on invoice item).

                    Items.AddRange(items.Select(o => new SaleItemViewModel (products.FirstOrDefault(p => p.Id == o.ProductId)) {  Quantity = o.Quantity, Discount = o.Discount, BasePrice = o.Price }));

                    Execute.OnUIThread(() => NotifyOfPropertyChange(() => TotalText));

                    //Task.Delay(2000).Wait();
                }).ContinueWith(t => 
                    {                        
                        Execute.OnUIThread(() => IsLoading = false);
                    });
        }
    }
}
