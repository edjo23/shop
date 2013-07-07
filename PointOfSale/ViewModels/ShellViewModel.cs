using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.PointOfSale.ViewModels
{
    public class ShellViewModel : Conductor<Screen>.Collection.OneActive, IHandle<Screen>
    {
        public ShellViewModel(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;

            DisplayName = "The Shop";

            EventAggregator.Subscribe(this);
        }

        private readonly IEventAggregator EventAggregator;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            ActivateItem(IoC.Get<HomeViewModel>());
        }
        
        public void Handle(Screen message)
        {
            ActivateItem(message);
        }
    }
}
