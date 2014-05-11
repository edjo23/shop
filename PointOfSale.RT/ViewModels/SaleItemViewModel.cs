using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Shop.Contracts.Entities;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace PointOfSale.RT.ViewModels
{
    public class TransactionItemViewModel : PropertyChangedBase
    {
        public string Description { get; set; }

        public decimal BasePrice { get; set; }

        public decimal Discount { get; set; }

        public decimal Price
        {
            get
            {
                return Math.Round(BasePrice * (100 - Discount) / 100, 2, MidpointRounding.AwayFromZero);
            }
        }

        public string PriceText
        {
            get
            {
                return Price.ToString("C");
            }
        }

        private int _Quantity;

        public int Quantity
        {
            get
            {
                return _Quantity;
            }
            set
            {
                if (value != _Quantity)
                {
                    _Quantity = value;
                    NotifyOfPropertyChange(() => Quantity);
                    NotifyOfPropertyChange(() => Total);
                    NotifyOfPropertyChange(() => TotalText);
                    NotifyOfPropertyChange(() => QuantityVisibility);
                }
            }
        }

        public decimal Total
        {
            get
            {
                return Quantity * Price;
            }
        }

        public string TotalText
        {
            get
            {
                return Total.ToString("C");
            }
        }

        public Visibility QuantityVisibility
        {
            get
            {
                return Quantity > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public BitmapImage ImageSource { get; set; }
    }

    public class SaleItemViewModel : TransactionItemViewModel
    {
        public SaleItemViewModel()
        {
        }

        public SaleItemViewModel(Product product)
        {
            Product = product;
            Description = product.Description;
            BasePrice = product.Price;
        }

        public Product Product { get; set; }
    }
}
