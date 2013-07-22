using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.PointOfSale.Models
{
    public class CustomerTransactionInfo
    {
        public decimal NewBalance { get; set; }

        public string NewBalanceText
        {
            get
            {
                return String.Format("Your balance is now {0:C}", NewBalance);
            }
        }
    }
}
