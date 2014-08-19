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
    public class CashHomeViewModel : Screen
    {
        public CashHomeViewModel(ScreenCoordinator screenCoordinator, IApplicationService applicationService)
        {
            ScreenCoordinator = screenCoordinator;
            ApplicationService = applicationService;

            Text = new BindableCollection<string>();
        }

        private readonly ScreenCoordinator ScreenCoordinator;

        private readonly IApplicationService ApplicationService;

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

        public Customer CashCustomer { get; set; }

        public BindableCollection<string> Text { get; set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            IsLoading = true;

            Task.Factory.StartNew(() =>
            {
                if (Windows.Storage.ApplicationData.Current.LocalSettings.Values["HostAddress"] != null)
                {
                    var text = ApplicationService.GetText("CashHome");

                    Execute.OnUIThread(() =>
                    {
                        Text.Clear();

                        if (text != null)
                            Text.AddRange(text);

                        IsLoading = false;
                    });
                }
            });
        }

        public void GoHome()
        {
            ScreenCoordinator.NavigateToHome();
        }

        public void NewTransaction()
        {
            if (CashCustomer != null)
                ScreenCoordinator.NavigateToCustomer(CashCustomer);
        }
    }
}
