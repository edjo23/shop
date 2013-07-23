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
    public class LoanViewModel : Screen
    {
        public LoanViewModel(IEventAggregator eventAggregator, ScreenCoordinator screenCoordinator, ICustomerService customerService)
        {
            EventAggregator = eventAggregator;
            ScreenCoordinator = screenCoordinator;
            CustomerService = customerService;
            DisplayName = "LOAN";
            LoanItems = new BindableCollection<LoanItemViewModel>();
        }

        private readonly IEventAggregator EventAggregator;

        private readonly ScreenCoordinator ScreenCoordinator;

        private readonly ICustomerService CustomerService;

        public Customer Customer { get; set; }

        public BindableCollection<LoanItemViewModel> LoanItems { get; set; }

        public decimal Total
        {
            get
            {
                return LoanItems.Sum(o => o.Total);;
            }
        }

        public string TotalText
        {
            get
            {
                return Total.ToString("C");
            }
        }

        public bool CanCompleteLoan
        {
            get
            {
                return LoanItems.Any(o => o.Quantity > 0);
            }
        }

        public void Load()
        {
            LoanItems.Add(new LoanItemViewModel { Description = "$100", Price = 100.0m });
            LoanItems.Add(new LoanItemViewModel { Description = "$50", Price = 50.0m });
            LoanItems.Add(new LoanItemViewModel { Description = "$20", Price = 20.0m });
            LoanItems.Add(new LoanItemViewModel { Description = "$10", Price = 10.0m });
            LoanItems.Add(new LoanItemViewModel { Description = "$5", Price = 5.0m });
            LoanItems.Add(new LoanItemViewModel { Description = "$2", Price = 2.0m });
            LoanItems.Add(new LoanItemViewModel { Description = "$1", Price = 1.0m });
            LoanItems.Add(new LoanItemViewModel { Description = "50c", Price = 0.50m });
            LoanItems.Add(new LoanItemViewModel { Description = "20c", Price = 0.20m });
            LoanItems.Add(new LoanItemViewModel { Description = "10c", Price = 0.10m });
            LoanItems.Add(new LoanItemViewModel { Description = "5c", Price = 0.05m });
        }

        public void AddItem(LoanItemViewModel item)
        {
            UpdateQuantity(item, 1);
        }

        public void RemoveItem(LoanItemViewModel item)
        {
            UpdateQuantity(item, -1);
        }

        protected void UpdateQuantity(LoanItemViewModel item, int value)
        {
            item.Quantity += value;

            NotifyOfPropertyChange(() => CanCompleteLoan);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => TotalText);
        }

        public void CompleteLoan()
        {
            var processvm = IoC.Get<ProcessViewModel>();
            processvm.Content = "Processing...";

            processvm.ProcessAction = () =>
            {
                var transaction = new CustomerTransaction
                {
                    Id = Guid.NewGuid(),
                    CustomerId = Customer.Id,
                    DateTime = DateTimeOffset.Now,
                    Type = CustomerTransactionType.CashAdvance,
                    Amount = Total
                };

                CustomerService.AddTransaction(transaction);

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
