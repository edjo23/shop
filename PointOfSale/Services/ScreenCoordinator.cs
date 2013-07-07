using Caliburn.Micro;
using Shop.Contracts.Entities;
using Shop.PointOfSale.Models;
using Shop.PointOfSale.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.PointOfSale.Services
{
    public class ScreenCoordinator
    {
        public ScreenCoordinator(IEventAggregator eventAggregator)
	    {
            EventAggregator = eventAggregator;
	    }

        private readonly IEventAggregator EventAggregator;

        public void GoToHome()
        {
            var screen = IoC.Get<HomeViewModel>();

            EventAggregator.Publish(screen);
        }

        public void GoToCustomer(Customer customer)
        {
            var screen = IoC.Get<CustomerViewModel>();
            screen.Customer = customer;

            EventAggregator.Publish(screen);
        }
    }
}
