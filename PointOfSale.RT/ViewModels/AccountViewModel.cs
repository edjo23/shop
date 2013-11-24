using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using PointOfSale.RT.Messages;
using PointOfSale.RT.Services;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;
using Windows.UI;
using Windows.UI.Xaml;
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

        private bool _HasPin;

        public bool HasPin
        {
            get 
            { 
                return _HasPin; 
            }
            set 
            {
                if (value != _HasPin)
                {
                    _HasPin = value;

                    NotifyOfPropertyChange(() => HasPin);
                    NotifyOfPropertyChange(() => CanAddPin);
                    NotifyOfPropertyChange(() => CanChangePin);
                    NotifyOfPropertyChange(() => CanRemovePin);
                    NotifyOfPropertyChange(() => AddPinVisibility);
                    NotifyOfPropertyChange(() => ChangePinVisibility);
                    NotifyOfPropertyChange(() => RemovePinVisibility);
                }
            }
        }

        public bool CanAddPin
        {
            get { return !PinUpdating && !HasPin; }
        }

        public bool CanChangePin
        {
            get { return !PinUpdating && HasPin; }
        }

        public bool CanRemovePin
        {
            get { return !PinUpdating && HasPin; }
        }

        public Visibility AddPinVisibility
        {
            get { return CanAddPin ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility ChangePinVisibility
        {
            get { return CanChangePin ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility RemovePinVisibility
        {
            get { return CanRemovePin ? Visibility.Visible : Visibility.Collapsed; }
        }

        private bool _PinUpdating;

        public bool PinUpdating
        {
            get
            {
                return _PinUpdating;
            }
            set
            {
                if (value != _PinUpdating)
                {
                    _PinUpdating = value;

                    NotifyOfPropertyChange(() => PinUpdating);
                    NotifyOfPropertyChange(() => PinUpdatingVisibility);
                    NotifyOfPropertyChange(() => CanAddPin);
                    NotifyOfPropertyChange(() => CanChangePin);
                    NotifyOfPropertyChange(() => CanRemovePin);
                    NotifyOfPropertyChange(() => AddPinVisibility);
                    NotifyOfPropertyChange(() => ChangePinVisibility);
                    NotifyOfPropertyChange(() => RemovePinVisibility);
                }
            }
        }

        private string _PinUpdatingText = "UPDATING PIN...";

        public string PinUpdatingText
        {
            get
            {
                return _PinUpdatingText;
            }
            set
            {
                if (value != _PinUpdatingText)
                {
                    _PinUpdatingText = value;

                    NotifyOfPropertyChange(() => PinUpdatingText);
                }
            }
        }

        public Visibility PinUpdatingVisibility
        {
            get { return PinUpdating ? Visibility.Visible : Visibility.Collapsed; }
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
            HasPin = !String.IsNullOrEmpty(Customer.Pin);
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

        public void AddPin()
        {
            var popup = IoC.Get<PinPopupViewModel>();
            popup.OnPinEntered = pin => UpdatePin(pin);

            ScreenCoordinator.ShowPopup(popup, 500, 500);
        }

        public void ChangePin()
        {
            var popup = IoC.Get<PinPopupViewModel>();
            popup.OnPinEntered = pin => UpdatePin(pin);

            ScreenCoordinator.ShowPopup(popup, 500, 500);
        }

        public void RemovePin()
        {
            PinUpdating = true;
            UpdatePin(null);
        }

        private Task UpdatePin(string pin)
        {
            PinUpdating = true;

            return Task.Factory
                .StartNew(() =>
                {
                    Execute.OnUIThread(() => { }); // TODO Status Test.
                    CustomerService.UpdatePin(Customer.Id, pin);
                })
                .ContinueWith(t => Execute.OnUIThread(() =>
                {
                    if (t.Exception == null)
                    {
                        HasPin = !String.IsNullOrEmpty(pin);
                        PinUpdating = false;
                    }
                    else
                    {
                        PinUpdatingText = "PIN UPDATE FAILED";
                    }
                }));
        }
    }
}
