using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace Shop.Management.ViewModels
{
    public class MainViewModel : Caliburn.Micro.Conductor<Screen>.Collection.OneActive, IHandle<IMenuItem>
    {
        public MainViewModel(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
            
            DisplayName = "Shop";
            MenuItem = IoC.Get<MenuViewModel>();

            EventAggregator.Subscribe(this);
        }

        public IEventAggregator EventAggregator { get; set; }

        public MenuViewModel MenuItem { get; set; }

        private IMenuItem _SelectedMenuItem;

        public IMenuItem SelectedMenuItem 
        {
            get
            {
                return _SelectedMenuItem;
            }
            set
            {
                if (value != _SelectedMenuItem)
                {
                    _SelectedMenuItem = value;
                    NotifyOfPropertyChange(() => SelectedMenuItem);
                }
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            MenuItem.Select(MenuItem.Items.First());
        }

        public void Handle(IMenuItem message)
        {
            SelectedMenuItem = message;
            ActivateItem(message.Instance);
        }
    }
}
