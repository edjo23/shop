using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using PointOfSale.RT.Messages;
using PointOfSale.RT.Models;
using PointOfSale.RT.Services;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;

namespace PointOfSale.RT.ViewModels
{
    public class LoanViewModel : Screen, IEnabled
    {
        public LoanViewModel(IEventAggregator eventAggregator, ScreenCoordinator screenCoordinator, ImageService imageService, IApplicationService applicationService, ICustomerService customerService)
        {
            EventAggregator = eventAggregator;
            ScreenCoordinator = screenCoordinator;
            ImageService = imageService;
            ApplicationService = applicationService;
            CustomerService = customerService;
            DisplayName = "LOAN";
            LoanItems = new BindableCollection<TransactionItemViewModel>();
        }

        private readonly IEventAggregator EventAggregator;

        private readonly ScreenCoordinator ScreenCoordinator;

        private readonly ImageService ImageService;

        private readonly IApplicationService ApplicationService;

        private readonly ICustomerService CustomerService;

        public Customer Customer { get; set; }

        public BindableCollection<TransactionItemViewModel> LoanItems { get; set; }

        public decimal Total
        {
            get
            {
                return LoanItems.Sum(o => o.Total); ;
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

        private bool _IsEnabled = true;

        public bool IsEnabled
        {
            get
            {
                return _IsEnabled;
            }
            set
            {
                if (value != _IsEnabled)
                {
                    _IsEnabled = value;

                    NotifyOfPropertyChange(() => IsEnabled);
                }
            }
        }

        public void Load()
        {
            var denominations = ApplicationService.GetDenominations();

            LoanItems.AddRange(denominations.Select(o => new TransactionItemViewModel { Description = o.Description, BasePrice = o.Value, ImageSource = ImageService.GetImage(o.Description) }));
        }

        public void AddItem(TransactionItemViewModel item)
        {
            UpdateQuantity(item, 1);
        }

        public void RemoveItem(TransactionItemViewModel item)
        {
            UpdateQuantity(item, -1);
        }

        protected void UpdateQuantity(TransactionItemViewModel item, int value)
        {
            var canComplete = CanCompleteLoan;

            item.Quantity += value;

            NotifyOfPropertyChange(() => CanCompleteLoan);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => TotalText);

            if (!canComplete && CanCompleteLoan)
                EventAggregator.Publish(new TransactionStarted { Source = this });

            if (canComplete && !CanCompleteLoan)
                EventAggregator.Publish(new TransactionStopped { Source = this });
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
                    Type = CustomerTransactionType.Loan,
                    Amount = Total
                };

                CustomerService.AddTransaction(transaction);

                Customer.Balance = CustomerService.GetCustomer(Customer.Id).Balance;
            };

            processvm.CompleteAction = () =>
            {
                var message = IoC.Get<MessageBoxViewModel>();
                message.Content = new CustomerTransactionInfo { Customer = Customer, NewBalance = Customer.Balance };
                message.DismissAction = () => ScreenCoordinator.NavigateToHome();
                message.DismissTimeout = 2500;

                ScreenCoordinator.NavigateToScreen(message);
            };

            ScreenCoordinator.NavigateToScreen(processvm);
        }
    }

}
