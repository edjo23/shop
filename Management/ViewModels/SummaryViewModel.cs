using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;

namespace Shop.Management.ViewModels
{
    public class SummaryViewModel : Screen
    {
        public SummaryViewModel(ICustomerService customerService, IProductService productService)
        {
            CustomerService = customerService;
            ProductService = productService;
        }

        public ICustomerService CustomerService { get; set; }

        public IProductService ProductService { get; set; }

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
                    NotifyOfPropertyChange(() => CanRefresh);
                }
            }
        }

        public Visibility LoadingVisibility
        {
            get
            {
                return IsLoading ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public Visibility ContentVisibility
        {
            get
            {
                return IsLoading ? Visibility.Hidden : Visibility.Visible;
            }
        }

        public bool CanRefresh
        {
            get
            {
                return !IsLoading;
            }
        }

        public decimal TotalLoanAmount { get; set; }

        public decimal TotalCashPaymentAmount { get; set; }

        public decimal TotalCashAmount
        {
            get
            {
                return TotalCashPaymentAmount - TotalLoanAmount;
            }
        }

        public decimal TotalAccountsReceivableAmount { get; set; }

        public decimal TotalAccountsPayableAmount { get; set; }

        public decimal TotalInventoryAmount { get; set; }

        public decimal TotalAssetValue
        {
            get
            {
                return TotalCashAmount + TotalInventoryAmount + TotalAccountsReceivableAmount;
            }
        }

        public decimal TotalEquityAmount
        {
            get
            {
                return TotalAssetValue - TotalAccountsPayableAmount;
            }
        }

        public decimal TotalLiabilityValue
        {
            get
            {
                return TotalAccountsPayableAmount + TotalEquityAmount;
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Load();
        }

        private void Load()
        {
            IsLoading = true;

            Task.Factory.StartNew(() =>
            {

                var customers = CustomerService.GetAllCustomers();
                var transactions = CustomerService.GetTransactions(null, null, null).ToList();
                var products = ProductService.GetProducts();

                TotalLoanAmount = 0;
                TotalCashPaymentAmount = 0;
                TotalAccountsReceivableAmount = 0;
                TotalAccountsPayableAmount = 0;
                TotalInventoryAmount = 0;

                foreach (var customer in customers)
                {
                    if (customer.Balance >= 0)
                        TotalAccountsReceivableAmount += customer.Balance;
                    else
                        TotalAccountsPayableAmount += (customer.Balance * -1);
                }

                foreach (var transaction in transactions)
                {
                    if (transaction.Type == CustomerTransactionType.Payment)
                    {
                        if (transaction.SourceId == null) // Not from a stock receipt.
                            TotalCashPaymentAmount += transaction.Amount * -1;
                    }
                    else if (transaction.Type == CustomerTransactionType.Loan)
                    {
                        TotalLoanAmount += transaction.Amount;
                    }
                }

                foreach (var product in products)
                {
                    TotalInventoryAmount += (product.Cost * product.QuantityOnHand);
                }

                Execute.OnUIThread(() =>
                {
                    NotifyOfPropertyChange(() => TotalCashAmount);
                    NotifyOfPropertyChange(() => TotalInventoryAmount);
                    NotifyOfPropertyChange(() => TotalAccountsReceivableAmount);

                    NotifyOfPropertyChange(() => TotalAccountsPayableAmount);
                    NotifyOfPropertyChange(() => TotalEquityAmount);

                    NotifyOfPropertyChange(() => TotalAssetValue);
                    NotifyOfPropertyChange(() => TotalLiabilityValue);

                    IsLoading = false;
                });
            })
            .ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    foreach (var exception in t.Exception.InnerExceptions)
                    {
                        MessageBox.Show(exception.Message + Environment.NewLine + exception.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            });
        }

        public void Refresh()
        {
            if (CanRefresh)
                Load();
        }
    }
}
