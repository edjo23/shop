using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Shop.Contracts.Entities;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace PointOfSale.RT.ViewModels
{

    public class HomeOption
    {
        public enum OptionEnum
        {
            Cash,
            Account
        }

        public string Text { get; set; }

        public string Help { get; set; }

        public OptionEnum Option { get; set; }
    }

    public class HomeItemViewModel : PropertyChangedBase
    {
        public virtual Brush TileColorBrush
        {
            get
            {
                return new SolidColorBrush(Colors.White);
            }
        }
    }

    public class AccountHomeItemViewModel : HomeItemViewModel
    {
        public Customer Customer { get; set; }

        public override Brush TileColorBrush
        {
            get
            {
                var alpha = (byte)(((String.IsNullOrEmpty(Customer.Name) ? 0 : Convert.ToInt32(Customer.Name.ToCharArray()[0])) % 5) * 10);

                alpha = 255;
                return new SolidColorBrush(Color.FromArgb(255, alpha, alpha, alpha));
            }
        }

        public string BalanceText
        {
            get
            {
                return String.Format("{0:C}{1}", Math.Abs(Customer.Balance), Customer.Balance < 0 ? " CR" : "");
            }
        }      
    }

    public class CashHomeItemViewModel : AccountHomeItemViewModel
    {
    }    
}
