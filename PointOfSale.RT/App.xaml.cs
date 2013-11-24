using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Caliburn.Micro;
using PointOfSale.RT.Services;
using PointOfSale.RT.ViewModels;
using PointOfSale.RT.Views;
using Shop.Contracts.Services;
using Shop.Service.Client;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// TODO
// Logging
// Portable Libraries

namespace PointOfSale.RT
{
    sealed partial class App
    {
        private WinRTContainer Container;

        public App()
        {
            InitializeComponent();
        }

        protected override void Configure()
        {
            Container = new WinRTContainer();
            Container.RegisterWinRTServices();

            Container.PerRequest<ShellViewModel>();
            Container.PerRequest<HomeViewModel>();
            Container.PerRequest<CustomerViewModel>();
            Container.PerRequest<PinEntryViewModel>();
            Container.PerRequest<SaleViewModel>();
            Container.PerRequest<PayViewModel>();
            Container.PerRequest<LoanViewModel>();
            Container.PerRequest<AccountViewModel>();
            Container.PerRequest<AccountTransactionListViewModel>();
            Container.PerRequest<ProcessViewModel>();
            Container.PerRequest<MessageBoxViewModel>();
            Container.PerRequest<SettingsViewModel>();
            Container.PerRequest<InvoicePopupViewModel>();
            Container.PerRequest<PinPopupViewModel>();

            Container.Singleton<IServiceClientFactory, WindowsStoreServiceClientFactory>();

            Container.PerRequest<IApplicationService, ApplicationServiceProxy>();
            Container.PerRequest<ICustomerService, CustomerServiceProxy>();
            Container.PerRequest<IProductService, ProductServiceProxy>();
            Container.PerRequest<IInvoiceService, InvoiceServiceProxy>();
            Container.PerRequest<IDiscountService, DiscountServiceProxy>();

            Container.PerRequest<ScreenCoordinator>();
            Container.PerRequest<ImageService>();

            var settings = Container.RegisterSettingsService();
            settings.RegisterCommand<SettingsViewModel>("Options");
        }

        protected override object GetInstance(Type service, string key)
        {
            var instance = Container.GetInstance(service, key);
            if (instance != null)
                return instance;

            throw new Exception("Could not locate any instances.");
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return Container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            Container.BuildUp(instance);
        }

        protected override void PrepareViewFirst(Frame rootFrame)
        {
            Container.RegisterNavigationService(rootFrame);
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            DisplayRootView<ShellView>();
        }
    }
}
