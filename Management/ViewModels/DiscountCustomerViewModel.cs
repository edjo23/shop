using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Shop.Contracts.Entities;

namespace Shop.Management.ViewModels
{
    public class DiscountCustomerViewModel : PropertyChangedBase
    {
        public Customer Customer { get; set; }

        public DiscountCustomer DiscountCustomer { get; set; }

        private bool _Selected = false;

        public bool Selected
        {
            get
            {
                return _Selected;
            }
            set
            {
                if (value != _Selected)
                {
                    _Selected = value;

                    NotifyOfPropertyChange(() => Selected);
                }
            }
        }
    }
}
