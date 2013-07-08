using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

        public bool IsCashAccount
        {
            get
            {
                return Customer != null && String.Equals(Customer.Name, "Cash", StringComparison.InvariantCultureIgnoreCase);
            }
        }

        public Visibility BalanceVisibility
        {
            get
            {
                return IsCashAccount ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public string BalanceText
        {
            get
            {
                return Customer.Balance.ToString("C");
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            var saleViewModel = IoC.Get<SaleViewModel>();
            saleViewModel.Customer = Customer;

            Items.Add(saleViewModel);

            if (!IsCashAccount)
            {
                var payViewModel = IoC.Get<PayViewModel>();
                payViewModel.Customer = Customer;

                Items.Add(payViewModel);
            }

            ActivateItem(Items.First());
        }

        public void GoHome()
        {
            ScreenCoordinator.GoToHome();
        }
    }
}
