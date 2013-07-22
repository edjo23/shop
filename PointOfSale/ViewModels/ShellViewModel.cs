using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.PointOfSale.Models;

namespace Shop.PointOfSale.ViewModels
{
    public class ShellViewModel : Conductor<Screen>.Collection.OneActive, IHandle<Screen>
    {
        public ShellViewModel(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
            Logger = log4net.LogManager.GetLogger(GetType());

            DisplayName = "The Shop";

            EventAggregator.Subscribe(this);
        }

        private readonly IEventAggregator EventAggregator;

        private readonly log4net.ILog Logger;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Task.Factory.StartNew(() => Handle(IoC.Get<HomeViewModel>()));
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
