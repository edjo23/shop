using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace PointOfSale.RT.ViewModels
{
    public class PayItemViewModel : PropertyChangedBase
    {
        public string Description { get; set; }

        public decimal Price { get; set; }

        public BitmapImage ImageSource
        {
            get
            {
                //var fileInfo = new FileInfo(String.Format(@"Images\{0}.png", Description));
                //if (fileInfo.Exists)
                //    return new BitmapImage(new Uri(fileInfo.FullName));
                //else
                    return null;
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
                return Quantity * Price;
            }
        }

        public Visibility QuantityVisibility
        {
            get
            {
                return Quantity > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }

}
