using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;
using Shop.Service.Client;
using Windows.UI.Xaml;

namespace PointOfSale.RT.ViewModels
{
    public class AccountTransactionListViewModel : PropertyChangedBase
    {
        public AccountTransactionListViewModel(ICustomerService customerService)
        {
            CustomerService = customerService;

            Transactions = new BindableCollection<AccountTransactionViewModel>();
        }

        private readonly ICustomerService CustomerService;

        public Guid CustomerId { get; set; }

        public DateTimeOffset FromDate { get; set; }

        public DateTimeOffset ToDate { get; set; }

        private bool _IsLoading = false;

        public bool IsLoading
        {
            get
            {
                return _IsLoading;
            }
            set
            {
                if (value != _IsLoading)
                {
                    _IsLoading = value;

                    NotifyOfPropertyChange(() => IsLoading);
                    NotifyOfPropertyChange(() => LoadingVisibility);
                    NotifyOfPropertyChange(() => ContentVisibility);
                    NotifyOfPropertyChange(() => ListVisibility);
                    NotifyOfPropertyChange(() => EmptyListVisibility);
                }
            }
        }

        public Visibility LoadingVisibility
        {
            get
            {
                return IsLoading ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility ContentVisibility
        {
            get
            {
                return !IsLoading ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility ListVisibility
        {
            get
            {
                return !IsLoading && Transactions.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility EmptyListVisibility
        {
            get
            {
                return !IsLoading && Transactions.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public BindableCollection<AccountTransactionViewModel> Transactions { get; set; }

        public void Load()
        {
            IsLoading = true;

            Task.Run(() =>
                {
                    var transactions = CustomerService.GetTransactions(CustomerId, FromDate, ToDate);

                    Transactions.AddRange(transactions.OrderByDescending(o => o.DateTime).Select((o, i) => new AccountTransactionViewModel { Transaction = o, Index = i, CanViewDetail = o.Type == CustomerTransactionType.Invoice }));

                    //Task.Delay(2000).Wait();

                    Execute.OnUIThread(() => IsLoading = false);
                });
        }
    }
}
