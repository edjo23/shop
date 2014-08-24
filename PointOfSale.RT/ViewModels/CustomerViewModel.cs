using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using PointOfSale.RT.Services;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;
using Windows.UI.Xaml;

namespace PointOfSale.RT.ViewModels
{
    public class CustomerViewModel : Conductor<Screen>.Collection.OneActive
    {
        public CustomerViewModel(ScreenCoordinator screenCoordinator, ImageService imageService, IApplicationService applicationService)
        {
            ScreenCoordinator = screenCoordinator;
            ImageService = imageService;
            ApplicationService = applicationService;

            Logger = log4net.LogManager.GetLogger(GetType());
        }

        private readonly ScreenCoordinator ScreenCoordinator;

        private readonly ImageService ImageService;

        private readonly IApplicationService ApplicationService;

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

        public Customer Customer { get; set; }        

        public bool IsCashAccount
        {
            get
            {
                return Customer != null && String.Equals(Customer.Name, "Cash", StringComparison.CurrentCultureIgnoreCase);
            }
        }

        public Visibility BalanceVisibility
        {
            get
            {
                return IsCashAccount ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public string NameText
        {
            get
            {
                return Customer.Name.ToLower();
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
            ReceiptViewModel receiptViewModel = null;
            AccountViewModel accountViewModel = null;

            if (!IsCashAccount)
            {
                payViewModel = IoC.Get<PayViewModel>();
                payViewModel.Customer = Customer;

                loanViewModel = IoC.Get<LoanViewModel>();
                loanViewModel.Customer = Customer;

                receiptViewModel = IoC.Get<ReceiptViewModel>();
                receiptViewModel.Customer = Customer;

                accountViewModel = IoC.Get<AccountViewModel>();
                accountViewModel.Customer = Customer;

                Items.Add(payViewModel);
                Items.Add(loanViewModel);
                Items.Add(receiptViewModel);
                Items.Add(accountViewModel);
            }

            Task.Factory.StartNew(() =>
            {
                // Load images from server.   
                var serverImages = ApplicationService.GetImageList();
                var cachedImages = ImageService.GetImageList();

                foreach (var image in serverImages)
                {
                    if (!cachedImages.Contains(image, StringComparer.CurrentCultureIgnoreCase))
                        ImageService.SetImage(image, ApplicationService.GetImage(image));
                }

                // Load view models.
                saleViewModel.Load();

                if (payViewModel != null)
                    payViewModel.Load();

                if (loanViewModel != null)
                    loanViewModel.Load();

                if (receiptViewModel != null)
                    receiptViewModel.Load();

                if (accountViewModel != null)
                    accountViewModel.Load();
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
            if (IsCashAccount)
                ScreenCoordinator.NavigateToHome();
            else
                ScreenCoordinator.NavigateToScreen(IoC.Get<AccountsViewModel>(), true);
        }
    }
}
