using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;
using Shop.PointOfSale.Services;

namespace Shop.PointOfSale.ViewModels
{
    public class PayViewModel : Screen
    {
        public PayViewModel(ScreenCoordinator screenCoordinator, ICustomerService customerService)
        {
            ScreenCoordinator = screenCoordinator;
            CustomerService = customerService;
            DisplayName = "Pay Account";
            PaymentItems = new BindableCollection<PayItemViewModel>();
        }

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

        protected override void OnInitialize()
        {
            base.OnInitialize();

            PaymentItems.Add(new PayItemViewModel { Description = "$10", Amount = 10.0m });
            PaymentItems.Add(new PayItemViewModel { Description = "$1", Amount = 1.0m });
            PaymentItems.Add(new PayItemViewModel { Description = "10c", Amount = 0.10m });
            PaymentItems.Add(new PayItemViewModel { Description = "1c", Amount = 0.01m });
        }

        public void AddPayment(PayItemViewModel item)
        {
            item.Quantity += 1;

            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => TotalText);
        }

        public void RemovePayment(PayItemViewModel item)
        {
            item.Quantity -= 1;

            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => TotalText);
        }

        public void CompletePayment()
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

            ScreenCoordinator.GoToHome();
        }
    }
}
