using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace Shop.Management.ViewModels
{
    public class MenuViewModel : PropertyChangedBase
    {
        public MenuViewModel(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;

            SalesItems = new BindableCollection<IMenuItem>()
            {
                new MenuItemViewModel<SummaryViewModel>("Overview")
            };

            AdministrationItems = new BindableCollection<IMenuItem>()
            {
                new MenuItemViewModel<ProductsViewModel>("Products"),
                new MenuItemViewModel<CustomersViewModel>("Customers")
            };

            Items = new BindableCollection<IMenuItem>();
            Items.AddRange(SalesItems);
            Items.AddRange(AdministrationItems);
        }

        public IEventAggregator EventAggregator { get; set; }

        public BindableCollection<IMenuItem> Items { get; set; }

        public BindableCollection<IMenuItem> SalesItems { get; set; }

        public BindableCollection<IMenuItem> AdministrationItems { get; set; }

        public void Select(IMenuItem menuItem)
        {
            Items.Action(o => o.IsSelected = (o == menuItem));

            EventAggregator.Publish(menuItem);
        }        
    }
}
