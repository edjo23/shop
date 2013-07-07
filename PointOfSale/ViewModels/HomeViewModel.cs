using Caliburn.Micro;
using Shop.PointOfSale.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;

namespace Shop.PointOfSale.ViewModels
{
    public class HomeViewModel : Screen
    {
        public HomeViewModel(ScreenCoordinator screenCoordinator, ICustomerService customerService)
        {
            ScreenCoordinator = screenCoordinator;
            CustomerService = customerService;

            Accounts = new BindableCollection<Customer>();
            Visitors = new BindableCollection<Customer>();
        }

        private readonly ScreenCoordinator ScreenCoordinator;

        private readonly ICustomerService CustomerService;

        public BindableCollection<Customer> Accounts { get; set; }

        public BindableCollection<Customer> Visitors { get; set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Accounts.AddRange(CustomerService.GetCustomers().Where(o => !String.Equals(o.Name, "Cash", StringComparison.InvariantCultureIgnoreCase)));
            Visitors.AddRange(CustomerService.GetCustomers().Where(o => String.Equals(o.Name, "Cash", StringComparison.InvariantCultureIgnoreCase)));
        }

        public void NewTransaction(Customer customer)
        {
            ScreenCoordinator.GoToCustomer(customer);
        }
    }
}
