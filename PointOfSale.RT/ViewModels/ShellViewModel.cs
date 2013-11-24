using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using PointOfSale.RT.Messages;
using PointOfSale.RT.Services;
using Shop.Contracts.Services;
using Shop.Service.Client;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace PointOfSale.RT.ViewModels
{
    public class ShellViewModel : Conductor<Screen>.Collection.OneActive, IHandle<SettingsChanged>, IHandle<Screen>, IHandle<ShowPopup>, IHandle<HidePopup>
    {
        public ShellViewModel(IEventAggregator eventAggregator, IServiceClientFactory serviceClientFactory)
        {
            EventAggregator = eventAggregator;
            ServiceClientFactory = serviceClientFactory as WindowsStoreServiceClientFactory;
            Logger = log4net.LogManager.GetLogger(GetType());

            DisplayName = "The Shop";

            EventAggregator.Subscribe(this);
        }

        private readonly IEventAggregator EventAggregator;

        private readonly WindowsStoreServiceClientFactory ServiceClientFactory;

        private readonly log4net.ILog Logger;

        public Visibility ContentVisibility
        {
            get
            {
                return ApplicationView.Value == ApplicationViewState.FullScreenLandscape ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private double _PopupWidth;

        public double PopupWidth
        {
            get
            {
                return _PopupWidth;
            }
            set
            {
                if (value != _PopupWidth)
                {
                    _PopupWidth = value;

                    NotifyOfPropertyChange(() => PopupWidth);
                }
            }
        }

        private double _PopupHeight;

        public double PopupHeight
        {
            get
            {
                return _PopupHeight;
            }
            set
            {
                if (value != _PopupHeight)
                {
                    _PopupHeight = value;

                    NotifyOfPropertyChange(() => PopupHeight);
                }
            }
        }

        private PopupViewModel _PopupItem;

        public PopupViewModel PopupItem
        {
            get
            {
                return _PopupItem;
            }
            set
            {
                if (value != _PopupItem)
                {
                    _PopupItem = value;

                    NotifyOfPropertyChange(() => PopupItem);
                }
            }
        }

        private bool _PopupIsOpen;

        public bool PopupIsOpen
        {
            get
            {
                return _PopupIsOpen;
            }
            set
            {
                if (value != _PopupIsOpen)
                {
                    _PopupIsOpen = value;

                    if (_PopupIsOpen == false)
                    {
                        PopupItem = null;
                    }                

                    NotifyOfPropertyChange(() => PopupIsOpen);
                }
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Window.Current.SizeChanged += Window_SizeChanged;

            ConfigureServiceClients();

            Task.Factory.StartNew(() => Handle(IoC.Get<HomeViewModel>()));
        }

        private void Window_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            NotifyOfPropertyChange(() => ContentVisibility);
        }        

        public void Handle(SettingsChanged message)
        {
            ConfigureServiceClients();

            Task.Factory.StartNew(() => Handle(IoC.Get<HomeViewModel>()));
        }

        private void ConfigureServiceClients()
        {
            ServiceClientFactory.EndpointAddressFormatString = "http://{0}/Services/{1}.svc";
            ServiceClientFactory.Host = (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["HostAddress"];
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

        public void Handle(ShowPopup message)
        {
            if (!PopupIsOpen)
            {
                PopupWidth = message.Width;
                PopupHeight = message.Height;
                PopupItem = message.Popup;
                PopupItem.Activate();
                PopupIsOpen = true;         
            }
        }

        public void Handle(HidePopup message)
        {
            PopupIsOpen = false;
        }
    }
}
