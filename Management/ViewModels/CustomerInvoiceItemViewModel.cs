using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Shop.Contracts.Entities;

namespace Shop.Management.ViewModels
{
    public class CustomerInvoiceItemViewModel : PropertyChangedBase
    {
        public Product Product { get; set; }

        private int _Quantity;

        public int Quantity
        {
            get
            {
                return _Quantity;
            }
            set
            {
                if (value != Quantity)
                {
                    _Quantity = value;

                    NotifyOfPropertyChange(() => Quantity);
                    NotifyOfPropertyChange(() => CanDecreaseQuantity);
                }
            }
        }

        public bool CanDecreaseQuantity
        {
            get
            {
                return Quantity > 0;
            }
        }

        public void IncreaseQuantity()
        {
            Quantity += 1;
        }

        public void DecreaseQuantity()
        {
            Quantity -= 1;
        }
    }
}
