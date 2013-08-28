using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointOfSale.RT.Models
{
    public class CustomerTransactionInfo
    {
        public decimal NewBalance { get; set; }

        public string NewBalanceText
        {
            get
            {
                var balanceText = String.Format("{0:C}{1}", Math.Abs(NewBalance), NewBalance < 0 ? " CR" : "");

                return String.Format("Your balance is now {0:C}", balanceText);
            }
        }
    }
}
