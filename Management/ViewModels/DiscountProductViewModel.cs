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
        public DiscountProductViewModel()
        {
            DiscountProduct = new DiscountProduct();
        }

        public Product Product { get; set; }

        public DiscountProduct DiscountProduct { get; set; }

        #region Discount Property

        public decimal Discount
        {
            get
            {
                return DiscountProduct.Discount;
            }
            set
            {
                if (value != DiscountProduct.Discount)
                {
                    DiscountProduct.Discount = value;
                    NotifyOfPropertyChange(() => Discount);
                }
            }
        }

        #endregion
    }
}
