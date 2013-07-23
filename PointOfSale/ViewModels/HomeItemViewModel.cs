using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Caliburn.Micro;
using Shop.Contracts.Entities;

namespace Shop.PointOfSale.ViewModels
{
    public class HomeItemViewModel : PropertyChangedBase
    {
        public Customer Customer { get; set; }        

        public string Group
        {
            get
            {
                return String.IsNullOrEmpty(Customer.Name) ? "" : Customer.Name.Substring(0, 1).ToUpper();
            }
        }

        public string BalanceText
        {
            get
            {
                return Math.Abs(Customer.Balance).ToString("C");
            }
        }

        public Brush BalanceColorBrush
        {
            get
            {
                return Customer.Balance > 0 ? Brushes.Firebrick : Brushes.Gray;
            }
        }

        private bool _IsFirstInGroup = false;

        public bool IsFirstInGroup
        {
            get
            {
                return _IsFirstInGroup;
            }
            set
            {
                if (value != _IsFirstInGroup)
                {
                    _IsFirstInGroup = value;
                    NotifyOfPropertyChange(() => IsFirstInGroup);
                }
            }
        }
    }
}
