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
                    NotifyOfPropertyChange(() => Customers);
                    NotifyOfPropertyChange(() => VisitorVisibility);
                    NotifyOfPropertyChange(() => AccountVisibility);
                }
            }
        }

        public IEnumerable<HomeItemViewModel> Customers
        {
            get
            {
                return new List<HomeItemViewModel>((SelectedItem ?? "").Equals("Visitor", StringComparison.CurrentCultureIgnoreCase) ? Visitors : Accounts);
            }
        }

        public Visibility VisitorVisibility
        {
            get
            {
                return (SelectedItem ?? "").Equals("Visitor", StringComparison.CurrentCultureIgnoreCase) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility AccountVisibility
        {
            get
            {
                return (SelectedItem ?? "").Equals("Account", StringComparison.CurrentCultureIgnoreCase) ? Visibility.Visible : Visibility.Collapsed;
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

                var customers = CustomerService.GetCustomers();                
                var termCustomers = customers.Where(o => !o.Name.IsMatch("Cash"));
                var cashCustomers = customers.Where(o => o.Name.IsMatch("Cash"));

                var previousGroup = "";
                foreach (var customer in termCustomers.OrderBy(o => o.Name))
                {
                    var group = String.IsNullOrEmpty(customer.Name) ? "" : customer.Name.Substring(0, 1).ToUpper();

                    if (group != previousGroup)
                    {
                        Accounts.Add(new GroupHomeItemViewModel { Group = group });
                        previousGroup = group;
                    }

                    Accounts.Add(new AccountHomeItemViewModel { Customer = customer });
                }

                Visitors.AddRange(cashCustomers.Select(o => new CashHomeItemViewModel { Customer = o }));
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

        public void NewTransaction(AccountHomeItemViewModel item)
        {
            ScreenCoordinator.NavigateToCustomer(item.Customer);
        }
    }

}
