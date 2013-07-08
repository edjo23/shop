using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.PointOfSale.Messages;

namespace Shop.PointOfSale.ViewModels
{
    public class ShellViewModel : Conductor<Screen>.Collection.OneActive, IHandle<Screen>, IHandle<ShowDialog>, IHandle<HideDialog>
    {
        public ShellViewModel(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;

            DisplayName = "The Shop";

            EventAggregator.Subscribe(this);
        }

        private readonly IEventAggregator EventAggregator;

        private Screen _DialogItem;

        public Screen DialogItem 
        {
            get
            {
                return _DialogItem;
            }
            set
            {
                if (value != _DialogItem)
                {
                    _DialogItem = value;
                    NotifyOfPropertyChange(() => DialogItem);
                }
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            ActivateItem(IoC.Get<HomeViewModel>());
        }
        
        public void Handle(Screen message)
        {
            ActivateItem(message);
        }

        public void Handle(ShowDialog message)
        {
            DialogItem = message.Screen;
        }

        public void Handle(HideDialog message)
        {
            DialogItem = null;
        }
    }
}
