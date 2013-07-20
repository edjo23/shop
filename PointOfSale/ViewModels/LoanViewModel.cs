using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;
using Shop.PointOfSale.Messages;
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

        protected override void OnInitialize()
        {
            base.OnInitialize();

            LoanItems.Add(new LoanItemViewModel { Description = "$10", Amount = 10.0m });
            LoanItems.Add(new LoanItemViewModel { Description = "$1", Amount = 1.0m });
            LoanItems.Add(new LoanItemViewModel { Description = "10c", Amount = 0.10m });
            LoanItems.Add(new LoanItemViewModel { Description = "1c", Amount = 0.01m });
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
            var messageBox = IoC.Get<MessageBoxViewModel>();
            messageBox.DisplayName = "";
            messageBox.Content = "";

            EventAggregator.Publish(new ShowDialog { Screen = messageBox });

            Task.Factory.StartNew(() =>
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
            })
            .ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Execute.OnUIThread(() =>
                    {
                        messageBox.Background = System.Windows.Media.Brushes.Firebrick;
                        messageBox.DisplayName = "Sorry, an error has occurred :(";
                        messageBox.Content = "The loan was not processed";
                    });
                }
                else
                {
                    Execute.OnUIThread(() =>
                    {
                        messageBox.DisplayName = "Thank you";
                        messageBox.Content = String.Format("Your balance is now {0:C}", Customer.Balance);
                    });

                    System.Threading.Thread.Sleep(5000);

                    Execute.OnUIThread(() =>
                    {
                        ScreenCoordinator.GoToHome();
                    });
                }
            });
        }
    }
}
