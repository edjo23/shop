using Caliburn.Micro;
using Shop.PointOfSale.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;
using Shop.PointOfSale.Messages;

namespace Shop.PointOfSale.ViewModels
{
    public class HomeViewModel : Screen
    {
        public HomeViewModel(IEventAggregator eventAggregator, ScreenCoordinator screenCoordinator, ICustomerService customerService)
        {
            EventAggregator = eventAggregator;
            ScreenCoordinator = screenCoordinator;
            CustomerService = customerService;

            Accounts = new BindableCollection<Customer>();
            Visitors = new BindableCollection<Customer>();
        }

        private readonly IEventAggregator EventAggregator;

        private readonly ScreenCoordinator ScreenCoordinator;

        private readonly ICustomerService CustomerService;

        public BindableCollection<Customer> Accounts { get; set; }

        public BindableCollection<Customer> Visitors { get; set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            EventAggregator.Publish(new ShowDialog { Screen = IoC.Get<LoadingViewModel>() });

            Task.Factory.StartNew(() =>
                {
                    Accounts.AddRange(CustomerService.GetCustomers().Where(o => !String.Equals(o.Name, "Cash", StringComparison.InvariantCultureIgnoreCase)));
                    Visitors.AddRange(CustomerService.GetCustomers().Where(o => String.Equals(o.Name, "Cash", StringComparison.InvariantCultureIgnoreCase)));

                    EventAggregator.Publish(new HideDialog { });
                });
        }

        public void NewTransaction(Customer customer)
        {
            ScreenCoordinator.GoToCustomer(customer);
        }
    }
}
