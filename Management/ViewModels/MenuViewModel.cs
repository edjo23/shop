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

            Items = new BindableCollection<IMenuItem>() 
            { 
                new MenuItemViewModel<SummaryViewModel>("Overview"),
                new MenuItemViewModel<ProductsViewModel>("Products"),
                new MenuItemViewModel<CustomersViewModel>("Customers")
            };
        }

        public IEventAggregator EventAggregator { get; set; }

        public BindableCollection<IMenuItem> Items { get; set; }

        public void Select(IMenuItem menuItem)
        {
            Items.Action(o => o.IsSelected = (o == menuItem));

            EventAggregator.Publish(menuItem);
        }        
    }
}
