using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Shop.Management.Messages;

namespace Shop.Management.ViewModels
{
    public class ShellViewModel : Caliburn.Micro.Conductor<Screen>.Collection.OneActive, IHandle<ActivateItem>, IHandle<DeactivateItem>
    {
        public ShellViewModel(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
            DisplayName = "Shop";

            EventAggregator.Subscribe(this);
        }

        public IEventAggregator EventAggregator { get; set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            ActivateItem(IoC.Get<MainViewModel>());
        }

        public void Handle(ActivateItem message)
        {
            ActivateItem(message.CreateItem());
        }

        public void Handle(DeactivateItem message)
        {
            DeactivateItem(message.Item, true);
        }
    }
}
