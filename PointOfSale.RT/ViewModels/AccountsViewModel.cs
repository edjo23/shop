using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using PointOfSale.RT.Services;
using Shop.Contracts.Services;
using Windows.UI.Xaml;

namespace PointOfSale.RT.ViewModels
{
    public class AccountsViewModel : Screen
    {
        public AccountsViewModel(IEventAggregator eventAggregator, ScreenCoordinator screenCoordinator, ICustomerService customerService)
        {
            EventAggregator = eventAggregator;
            ScreenCoordinator = screenCoordinator;
            CustomerService = customerService;

            Logger = log4net.LogManager.GetLogger(GetType());
            Accounts = new BindableCollection<HomeItemViewModel>();
        }

        private readonly IEventAggregator EventAggregator;

        private readonly ScreenCoordinator ScreenCoordinator;
        
        private readonly ICustomerService CustomerService;

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
                return IsLoading ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility ContentVisibility
        {
            get
            {
                return IsLoading ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        #endregion

        public BindableCollection<HomeItemViewModel> Accounts { get; set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            
            IsLoading = true;

            Task.Factory.StartNew(() =>
            {
                if (Windows.Storage.ApplicationData.Current.LocalSettings.Values["HostAddress"] != null)
                {
                    var customers = CustomerService.GetCustomers();

                    Accounts.AddRange(customers.Where(o => !o.Name.IsMatch("Cash")).OrderBy(o => o.Name).Select(o => new AccountHomeItemViewModel { Customer = o }));
                }
            })            
            .ContinueWith(task =>
            {               
                if (task.IsFaulted)
                {
                    ScreenCoordinator.HandleFault(task.Exception);
                }
                else
                {
                    Execute.OnUIThread(() =>
                    {
                        IsLoading = false;

                        Task.Delay(10000).ContinueWith(t =>
                            {
                                if (IsActive)
                                    Execute.OnUIThread(() => GoHome());
                            });
                    });
                }
            });
        }

        public void NewTransaction(AccountHomeItemViewModel item)
        {
            if (String.IsNullOrEmpty(item.Customer.Pin))
                ScreenCoordinator.NavigateToCustomer(item.Customer);
            else
                ScreenCoordinator.NavigateToPinEntry(item.Customer);
        }

        public void GoHome()
        {
            ScreenCoordinator.NavigateToHome();
        }
    }
}
