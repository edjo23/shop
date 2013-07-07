using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using Shop.Contracts.Entities;

namespace Shop.PointOfSale.ViewModels
{
    public class SaleItemViewModel : PropertyChangedBase
    {
        public Product Product { get; set; }

        public BitmapImage ImageSource
        {
            get
            {
                return new BitmapImage(new Uri(String.Format("pack://siteoforigin:,,,/{0}.jpg", Product.Code)));
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
                    NotifyOfPropertyChange(() => QuantityVisibility);
                }
            }
        }

        public decimal Total
        {
            get
            {
                return Quantity * Product.Price;
            }
        }

        public Visibility QuantityVisibility
        {
            get
            {
                return Quantity > 0 ? Visibility.Visible : Visibility.Hidden;
            }
        }        
    }
}
