using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;
using Shop.PointOfSale.Models;
using Shop.PointOfSale.Services;

namespace Shop.PointOfSale.ViewModels
{
    public class PayViewModel : Screen
    {
        public PayViewModel(IEventAggregator eventAggregator, ScreenCoordinator screenCoordinator, ICustomerService customerService)
        {
            EventAggregator = eventAggregator;
            ScreenCoordinator = screenCoordinator;
            CustomerService = customerService;
            DisplayName = "PAYMENT";
            PaymentItems = new BindableCollection<PayItemViewModel>();
        }

        private readonly IEventAggregator EventAggregator;

        private readonly ScreenCoordinator ScreenCoordinator;

        private readonly ICustomerService CustomerService;

        public Customer Customer { get; set; }

        public BindableCollection<PayItemViewModel> PaymentItems { get; set; }

        public decimal Total
        {
            get
            {
                return PaymentItems.Sum(o => o.Total);;
            }
        }

        public string TotalText
        {
            get
            {
                return Total.ToString("C");
            }
        }

        public bool CanCompletePayment
        {
            get
            {
                return PaymentItems.Any(o => o.Quantity > 0);
            }
        }

        public void Load()
        {
            PaymentItems.Add(new PayItemViewModel { Description = "$20", Price = 20.0m });
            PaymentItems.Add(new PayItemViewModel { Description = "$10", Price = 10.0m });
            PaymentItems.Add(new PayItemViewModel { Description = "$5", Price = 5.0m });
            PaymentItems.Add(new PayItemViewModel { Description = "$1", Price = 1.0m });
            PaymentItems.Add(new PayItemViewModel { Description = "50c", Price = 0.50m });
            PaymentItems.Add(new PayItemViewModel { Description = "20c", Price = 0.20m });
            PaymentItems.Add(new PayItemViewModel { Description = "10c", Price = 0.10m });
            PaymentItems.Add(new PayItemViewModel { Description = "5c", Price = 0.05m });
        }

        public void AddItem(PayItemViewModel item)
        {
            UpdateQuantity(item, 1);
        }

        public void RemoveItem(PayItemViewModel item)
        {
            UpdateQuantity(item, -1);
        }

        protected void UpdateQuantity(PayItemViewModel item, int value)
        {
            item.Quantity += value;

            NotifyOfPropertyChange(() => CanCompletePayment);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => TotalText);
        }

        public void CompletePayment()
        {
            var processvm = IoC.Get<ProcessViewModel>();
            processvm.Content = "Processing...";

            processvm.ProcessAction = () =>
            {
                var payment = new CustomerTransaction
                {
                    Id = Guid.NewGuid(),
                    CustomerId = Customer.Id,
                    DateTime = DateTimeOffset.Now,
                    Type = CustomerTransactionType.Payment,
                    Amount = Total * -1
                };

                CustomerService.AddTransaction(payment);

                Customer.Balance = CustomerService.GetCustomer(Customer.Id).Balance;
            };

            processvm.CompleteAction = () =>
            {
                var message = IoC.Get<MessageBoxViewModel>();
                message.Content = new CustomerTransactionInfo { NewBalance = Customer.Balance };
                message.DismissAction = () => ScreenCoordinator.NavigateToHome();
                message.DismissTimeout = 2500;

                ScreenCoordinator.NavigateToScreen(message);
            };

            ScreenCoordinator.NavigateToScreen(processvm);
        }
    }
}
