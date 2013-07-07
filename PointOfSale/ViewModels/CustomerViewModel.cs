using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;
using Shop.PointOfSale.Models;
using Shop.PointOfSale.Services;

namespace Shop.PointOfSale.ViewModels
{
    public class CustomerViewModel : Conductor<Screen>.Collection.OneActive
    {
        public CustomerViewModel(ScreenCoordinator screenCoordinator)
        {
            ScreenCoordinator = screenCoordinator;
        }

        private ScreenCoordinator ScreenCoordinator;

        public Customer Customer { get; set; }
        
        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            Items.Add(IoC.Get<SaleViewModel>());

            if (!String.Equals(Customer.Name, "Cash", StringComparison.InvariantCultureIgnoreCase))
                Items.Add(IoC.Get<PayViewModel>());

            ActivateItem(Items.First());
        }

        public void GoHome()
        {
            ScreenCoordinator.GoToHome();
        }
    }
}
