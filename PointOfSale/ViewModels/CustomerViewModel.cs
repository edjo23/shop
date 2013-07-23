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
            Logger = log4net.LogManager.GetLogger(GetType());
        }

        private ScreenCoordinator ScreenCoordinator;

        private readonly log4net.ILog Logger;

        #region IsLoading Property

        public bool _IsLoading = false;

        public bool IsLoading
        {
            get
            {
                return _IsLoading;
            }
            set
            {
                if (value != _IsLoading)
                {
                    _IsLoading = value;

                    NotifyOfPropertyChange(() => IsLoading);
                    NotifyOfPropertyChange(() => LoadingVisibility);
                    NotifyOfPropertyChange(() => ContentVisibility);
                }
            }
        }

        public Visibility LoadingVisibility
        {
            get
            {
                return IsLoading ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public Visibility ContentVisibility
        {
            get
            {
                return IsLoading ? Visibility.Hidden : Visibility.Visible;
            }
        }

        #endregion

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

            IsLoading = true;

            var saleViewModel = IoC.Get<SaleViewModel>();
            saleViewModel.Customer = Customer;

            Items.Add(saleViewModel);

            PayViewModel payViewModel = null;
            LoanViewModel loanViewModel = null;

            if (!IsCashAccount)
            {
                payViewModel = IoC.Get<PayViewModel>();
                payViewModel.Customer = Customer;

                loanViewModel = IoC.Get<LoanViewModel>();
                loanViewModel.Customer = Customer;

                Items.Add(payViewModel);
                Items.Add(loanViewModel);
            }

            Task.Factory.StartNew(() =>
                {
                    saleViewModel.Load();

                    if (payViewModel != null)
                        payViewModel.Load();

                    if (loanViewModel != null)
                        loanViewModel.Load();
                })
            .ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        ScreenCoordinator.HandleFault(task.Exception);
                    }
                    else
                    {
                        Execute.OnUIThread(() => IsLoading = false);
                        ActivateItem(Items.First());
                    }
                });
        }

        public void GoHome()
        {
            ScreenCoordinator.NavigateToHome();
        }      
    }
}
