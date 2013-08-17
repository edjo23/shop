using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Shop.Contracts.Entities;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace PointOfSale.RT.ViewModels
{
    public class HomeItemViewModel : PropertyChangedBase
    {
    }

    public class GroupHomeItemViewModel : HomeItemViewModel
    {
        public string Group { get; set; }
    }

    public class AccountHomeItemViewModel : HomeItemViewModel
    {
        public Customer Customer { get; set; }

        public string BalanceText
        {
            get
            {
                return String.Format("{0:C}{1}", Math.Abs(Customer.Balance), Customer.Balance < 0 ? " CR" : "");
            }
        }
       
        public Brush BalanceColorBrush
        {
            get
            {
                return Customer.Balance > 0 ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Gray);
            }
        }
    }

    public class CashHomeItemViewModel : AccountHomeItemViewModel
    {
    }    
}
