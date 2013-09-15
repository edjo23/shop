using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using PointOfSale.RT.Services;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace PointOfSale.RT.ViewModels
{
    public class AccountViewModel : Screen
    {
        public AccountViewModel(ScreenCoordinator screenCoordinator, ICustomerService customerService)
        {
            ScreenCoordinator = screenCoordinator;
            CustomerService = customerService;

            DisplayName = "ACCOUNT";

            MaximumDate = new DateTimeOffset(DateTimeOffset.Now.Year, DateTimeOffset.Now.Month, 1, 0, 0, 0, TimeSpan.Zero);
            MinimumDate = new DateTimeOffset(2013, 07, 1, 0, 0, 0, TimeSpan.Zero);
        }

        private readonly ScreenCoordinator ScreenCoordinator;

        private readonly ICustomerService CustomerService;

        public Customer Customer { get; set; }

        private AccountTransactionListViewModel _TransactionListView;

        public AccountTransactionListViewModel TransactionListView
        {
            get
            {
                return _TransactionListView;
            }
            set
            {
                if (value != _TransactionListView)
                {
                    _TransactionListView = value;

                    NotifyOfPropertyChange(() => TransactionListView);
                }
            }
        }

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

        public DateTimeOffset MaximumDate { get; set; }

        public DateTimeOffset MinimumDate { get; set; }

        public DateTimeOffset? _FromDate = null;

        public DateTimeOffset? FromDate 
        {
            get
            {
                return _FromDate;
            }
            set
            {
                if (value != _FromDate)
                {
                    _FromDate = value;
                    NotifyOfPropertyChange(() => FromDate);
                    NotifyOfPropertyChange(() => FromDateText);
                    NotifyOfPropertyChange(() => CanShowNext);
                    NotifyOfPropertyChange(() => CanShowPrevious);
                }
            }
        }

        public string FromDateText
        {
            get
            {
                return FromDate.HasValue ? String.Format("{0:MMMM, yyyy}", FromDate.Value).ToUpper() : "";
            }
        }

        public bool CanShowNext
        {
            get
            {
                return FromDate.HasValue && FromDate.Value.AddDays(1) < MaximumDate;
            }
        }

        public bool CanShowPrevious
        {
            get
            {
                return FromDate.HasValue && FromDate.Value.AddDays(-1) > MinimumDate; ;
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            if (!FromDate.HasValue)
            {
                FromDate = MaximumDate;
                LoadTransactionView();
            }
        }

        public void Load()
        {
        }

        public void ShowPrevious()
        {
            LoadOffset(-1);
        }

        public void ShowNext()
        {
            LoadOffset(1);
        }

        private void LoadOffset(int offset)
        {
            if (!FromDate.HasValue)
                return;

            TransactionListView = null;
            FromDate = FromDate.Value.AddMonths(offset);
            LoadTransactionView();
        }

        private void LoadTransactionView()
        {
            var view = IoC.Get<AccountTransactionListViewModel>();
            view.CustomerId = Customer.Id;
            view.FromDate = FromDate.Value;
            view.ToDate = FromDate.Value.AddMonths(1).AddSeconds(-1);
            view.Load();

            TransactionListView = view;
        }

        public void ShowDetail(AccountTransactionViewModel item)
        {
            var popup = IoC.Get<InvoicePopupViewModel>();
            popup.InvoiceId = item.Transaction.SourceId.GetValueOrDefault();

            ScreenCoordinator.ShowPopup(popup);
        }
    }
}
