using Caliburn.Micro;
using Shop.PointOfSale.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;
using System.Windows;

namespace Shop.PointOfSale.ViewModels
{
    public class HomeViewModel : Screen
    {
        public HomeViewModel(IEventAggregator eventAggregator, ScreenCoordinator screenCoordinator, ICustomerService customerService)
        {
            EventAggregator = eventAggregator;
            ScreenCoordinator = screenCoordinator;
            CustomerService = customerService;

            Logger = log4net.LogManager.GetLogger(GetType());
            Items = new BindableCollection<string>();
            Accounts = new BindableCollection<HomeItemViewModel>();
            Visitors = new BindableCollection<HomeItemViewModel>();
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

        public BindableCollection<string> Items { get; set; }

        public BindableCollection<HomeItemViewModel> Accounts { get; set; }

        public BindableCollection<HomeItemViewModel> Visitors { get; set; }

        private string _SelectedItem;

        public string SelectedItem
        {
            get
            {
                return _SelectedItem;
            }
            set
            {
                if (value != _SelectedItem)
                {
                    _SelectedItem = value;

                    NotifyOfPropertyChange(() => SelectedItem);
                    NotifyOfPropertyChange(() => VisitorVisibility);
                    NotifyOfPropertyChange(() => AccountVisibility);
                }
            }
        }

        public Visibility VisitorVisibility
        {
            get
            {
                return (SelectedItem ?? "").Equals("Visitor", StringComparison.InvariantCultureIgnoreCase) ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public Visibility AccountVisibility
        {
            get
            {
                return (SelectedItem ?? "").Equals("Account", StringComparison.InvariantCultureIgnoreCase) ? Visibility.Visible : Visibility.Hidden;
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            IsLoading = true;

            Task.Factory.StartNew(() =>
                {
                    Items.Add("ACCOUNT");
                    Items.Add("VISITOR");

                    var customers = CustomerService.GetCustomers().Where(o => !o.Name.IsMatch("Cash"));

                    var previousGroup = "";
                    foreach (var customer in customers)
                    {
                        var item = new HomeItemViewModel { Customer = customer };

                        if (item.Group != previousGroup)
                        {
                            item.IsFirstInGroup = true;
                            previousGroup = item.Group;
                        }

                        Accounts.Add(item);
                    }

                    Visitors.AddRange(CustomerService.GetCustomers().Where(o => o.Name.IsMatch("Cash")).Select(o => new HomeItemViewModel { Customer = o }));                    
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
                                SelectedItem = Items.First();
                            });
                    }
                });
        }        

        public void NewTransaction(HomeItemViewModel item)
        {
            ScreenCoordinator.NavigateToCustomer(item.Customer);
        }
    }
}
