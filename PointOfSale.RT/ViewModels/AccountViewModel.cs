using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace PointOfSale.RT.ViewModels
{
    public class AccountViewModel : Screen
    {
        public AccountViewModel(ICustomerService customerService)
        {
            CustomerService = customerService;

            DisplayName = "ACCOUNT";

            Transactions = new BindableCollection<AccountTransactionViewModel>();
        }

        private readonly ICustomerService CustomerService;

        public Customer Customer { get; set; }

        public BindableCollection<AccountTransactionViewModel> Transactions { get; set; }

        public string BalanceText
        {
            get
            {
                return String.Format("{0:C}{1}", Math.Abs(Customer.Balance), Customer.Balance < 0 ? " CR" : "");
            }
        }

        public Brush BalanceColorBrush
        {
            get
            {
                return Customer.Balance > 0 ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Gray);
            }
        }

        public void Load()
        {
            var transactions = CustomerService.GetTransactions(Customer.Id, DateTime.Today.AddMonths(-1), null);

            Transactions.AddRange(transactions.OrderByDescending(o => o.DateTime).Select((o, i) => new AccountTransactionViewModel { Transaction = o, Index = i }));
        }
    }
}
