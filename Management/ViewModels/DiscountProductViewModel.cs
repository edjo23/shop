using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Shop.Contracts.Entities;

namespace Shop.Management.ViewModels
{
    public class DiscountProductViewModel : PropertyChangedBase
    {
        public Product Product { get; set; }

        public DiscountProduct DiscountProduct { get; set; }

        #region Discount Property

        private decimal _Discount;

        public decimal Discount
        {
            get
            {
                return _Discount;
            }
            set
            {
                if (value != _Discount)
                {
                    _Discount = value;                    
                    NotifyOfPropertyChange(() => Discount);
                }
            }
        }

        #endregion
    }
}
