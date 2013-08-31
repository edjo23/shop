using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.Contracts.Entities;
using Windows.UI.Xaml;

namespace PointOfSale.RT.Models
{
    public class CustomerTransactionInfo
    {
        public Customer Customer { get; set; }

        public decimal NewBalance { get; set; }

        public bool IsCash()
        {
            return Customer.Name.Equals("Cash", StringComparison.CurrentCultureIgnoreCase);
        }

        public string DisplayName
        {
            get
            {
                return IsCash() ? "" : Customer.Name;
            }
        }

        public string NewBalanceText
        {
            get
            {
                var balanceText = String.Format("{0:C}{1}", Math.Abs(NewBalance), NewBalance < 0 ? " CR" : "");

                return String.Format("Your balance is now {0:C}", balanceText);
            }
        }

        public Visibility NewBalanceVisibility
        {
            get
            {
                return IsCash() ? Visibility.Collapsed : Visibility.Visible;
            }
        }
    }
}
