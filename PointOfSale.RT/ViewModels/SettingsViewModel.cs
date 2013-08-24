using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace PointOfSale.RT.ViewModels
{
    public class SettingsChanged
    {
    }

    public class SettingsViewModel : Screen
    {
        public SettingsViewModel(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
        }

        public IEventAggregator EventAggregator { get; set; }

        private string _Host;

        public string Host
        {
            get
            {
                return _Host;
            }
            set
            {
                if (value != _Host)
                {
                    _Host = value;

                    NotifyOfPropertyChange(() => Host);
                }
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            // Get app setting.
            var host = Windows.Storage.ApplicationData.Current.LocalSettings.Values["HostAddress"];
            if (host != null)
                Host = (string)host;
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            // Set app setting.
            if (Host != (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["HostAddress"])
            {
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["HostAddress"] = Host;

                EventAggregator.Publish(new SettingsChanged());
            }
        }
    }
}
