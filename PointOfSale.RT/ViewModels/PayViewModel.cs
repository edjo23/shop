using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using PointOfSale.RT.Models;
using PointOfSale.RT.Services;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;

namespace PointOfSale.RT.ViewModels
{
    public class PayViewModel : Screen
    {
        public PayViewModel(IEventAggregator eventAggregator, ScreenCoordinator screenCoordinator, ImageService imageService, IApplicationService applicationService, ICustomerService customerService)
        {
            EventAggregator = eventAggregator;
            ScreenCoordinator = screenCoordinator;
            ImageService = imageService;
            ApplicationService = applicationService;
            CustomerService = customerService;
            DisplayName = "PAYMENT";
            PaymentItems = new BindableCollection<TransactionItemViewModel>();
        }

        private readonly IEventAggregator EventAggregator;

        private readonly ScreenCoordinator ScreenCoordinator;

        private readonly ImageService ImageService;

        private readonly IApplicationService ApplicationService;

        private readonly ICustomerService CustomerService;

        public Customer Customer { get; set; }

        public BindableCollection<TransactionItemViewModel> PaymentItems { get; set; }

        public decimal Total
        {
            get
            {
                return PaymentItems.Sum(o => o.Total); ;
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
            var denominations = ApplicationService.GetDenominations();

            PaymentItems.AddRange(denominations.Select(o => new TransactionItemViewModel { Description = o.Description, BasePrice = o.Value, ImageSource = ImageService.GetImage(o.Description) }));
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
