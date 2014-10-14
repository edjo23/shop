using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using PointOfSale.RT.Services;
using Shop.Contracts.Entities;
using Shop.Contracts.Messages;
using Shop.Contracts.Services;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace PointOfSale.RT.ViewModels
{
    public class HomeViewModel : Screen, IHandle<CardInserted>
    {
        public HomeViewModel(IEventAggregator eventAggregator, ScreenCoordinator screenCoordinator, CardService cardService, ICustomerService customerService)
        {
            EventAggregator = eventAggregator;
            ScreenCoordinator = screenCoordinator;
            CardService = cardService;
            CustomerService = customerService;

            Options = new BindableCollection<HomeOption>();
            Logger = log4net.LogManager.GetLogger(GetType());

            EventAggregator.Subscribe(this);
        }

        private readonly IEventAggregator EventAggregator;

        private readonly ScreenCoordinator ScreenCoordinator;

        private readonly CardService CardService;
        
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

        public BindableCollection<HomeOption> Options { get; set; }

        public Customer CashCustomer { get; set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            IsLoading = true;

            Task.Factory.StartNew(() =>
            {
                if (Windows.Storage.ApplicationData.Current.LocalSettings.Values["HostAddress"] != null)
                {
                    var customers = CustomerService.GetCustomers();
                    var cash = customers.FirstOrDefault(o => o.Name.IsMatch("Cash"));

                    Execute.OnUIThread(() =>
                    {
                        CashCustomer = cash;

                        if (cash != null)
                        {
                            Options.Add(new HomeOption
                            {
                                Option = HomeOption.OptionEnum.Cash,
                                Text = "price list + cash",
                                Help = "Touch here to see the price list and to pay with cash."
                            });
                        }

                        Options.Add(new HomeOption
                        {
                            Option = HomeOption.OptionEnum.Account,
                            Text = "account",
                            Help = "Touch here to select an account."
                        });

                        IsLoading = false;
                    });
                }
            });
        }

        public void SelectItem(HomeOption item)
        {
            if (item.Option == HomeOption.OptionEnum.Account)
            {
                ScreenCoordinator.NavigateToScreen(IoC.Get<AccountsViewModel>(), true);
            }
            else if (item.Option == HomeOption.OptionEnum.Cash)
            {
                ScreenCoordinator.NavigateToCashHome(CashCustomer);                       
            }
        }

        public void Handle(CardInserted message)
        {
            if (IsActive)
            {
                Task.Factory.StartNew(() =>
                {
                    var customer = CustomerService.GetCustomerByNumber(message.CardId);
                    if (customer != null)
                        Execute.OnUIThread(() => ScreenCoordinator.NavigateToCustomer(customer, true));
                });
            }
        }
    }
}
