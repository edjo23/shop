using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.Contracts.Entities;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace PointOfSale.RT.ViewModels
{
    public class AccountTransactionViewModel 
    {
        public CustomerTransaction Transaction { get; set; }

        public int Index { get; set; }

        public string DateTimeText
        {
            get
            {
                return String.Format("{0:ddd dd/MM/yy hh:mm tt}", Transaction.DateTime).ToUpper();
            }
        }

        public string TypeText
        {
            get
            {
                switch (Transaction.Type)
                {
                    case CustomerTransactionType.Invoice:
                        return "PURCHASE";
                    case CustomerTransactionType.Payment:
                        return "PAYMENT";
                    case CustomerTransactionType.Loan:
                        return "LOAN";
                    case CustomerTransactionType.Adjustment:
                        return "ADJUSTMENT";
                }

                return Transaction.Type.ToString();
            }
        }

        public string AmountText
        {
            get
            {
                return String.Format("{0:C}", Math.Abs(Transaction.Amount));
            }
        }

        public string DebitText
        {
            get
            {
                return Transaction.Amount >= 0.0m ? AmountText : "";
            }
        }

        public string CreditText
        {
            get
            {
                return Transaction.Amount < 0.0m ? AmountText : "";
            }
        }

        public Brush Background
        {
            get
            {
                if (Index % 2 == 0)
                    return new SolidColorBrush(Color.FromArgb(10, 0, 0, 0));
                else
                    return new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            }
        }
    }
}
