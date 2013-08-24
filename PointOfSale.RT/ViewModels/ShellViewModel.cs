using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Service.Client;
using Shop.Contracts.Services;

namespace PointOfSale.RT.ViewModels
{
    public class ShellViewModel : Conductor<Screen>.Collection.OneActive, IHandle<SettingsChanged>, IHandle<Screen>
    {
        public ShellViewModel(IEventAggregator eventAggregator, IServiceClientConfiguration serviceClientConfiguration)
        {
            EventAggregator = eventAggregator;
            ServiceClientConfiguration = serviceClientConfiguration;
            Logger = log4net.LogManager.GetLogger(GetType());

            DisplayName = "The Shop";

            EventAggregator.Subscribe(this);
        }

        private readonly IEventAggregator EventAggregator;

        private readonly IServiceClientConfiguration ServiceClientConfiguration;

        private readonly log4net.ILog Logger;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            ConfigureServiceClients();

            Task.Factory.StartNew(() => Handle(IoC.Get<HomeViewModel>()));
        }

        public void Handle(SettingsChanged message)
        {
            ConfigureServiceClients();

            Task.Factory.StartNew(() => Handle(IoC.Get<HomeViewModel>()));
        }

        private void ConfigureServiceClients()
        {
            ServiceClientConfiguration.EndpointAddressFormatString = "http://{0}/Services/{1}.svc";
            ServiceClientConfiguration.Host = (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["HostAddress"];
        }

        public void Handle(Screen message)
        {
            Logger.Debug(String.Format("Showing screen [Type: {0}]", message.GetType()));

            if (ActiveItem != null)
            {
                Logger.Debug(String.Format("Deactivating screen [Type: {0}]", ActiveItem.GetType()));
                DeactivateItem(ActiveItem, true);
            }

            Logger.Debug(String.Format("Activating screen [Type: {0}]", message.GetType()));

            ActivateItem(message);

            Logger.Debug(String.Format("Active screen is [Type: {0}]", ActiveItem.GetType()));
        }
    }
}
