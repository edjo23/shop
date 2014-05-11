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

        public decimal TotalBalanceAmount { get; set; }       

        public decimal TotalAccountInvoiceAmount { get; set; }

        public decimal TotalCashInvoiceAmount { get; set; }

        public decimal TotalInvoiceAmount
        {
            get
            {
                return TotalAccountInvoiceAmount + TotalCashInvoiceAmount;
            }
        }

        public decimal TotalLoanAmount { get; set; }

        public decimal TotalPaymentAmount { get; set; }

        public decimal TotalCashAmount
        {
            get
            {
                return TotalPaymentAmount + TotalLoanAmount;
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

                var customers = CustomerService.GetCustomers();
                var transactions = CustomerService.GetTransactions(null, null, null).ToList();
                var products = ProductService.GetProducts();

                TotalBalanceAmount = 0;
                TotalAccountInvoiceAmount = 0;
                TotalCashInvoiceAmount = 0;
                TotalLoanAmount = 0;
                TotalPaymentAmount = 0;
                TotalAccountsReceivableAmount = 0;
                TotalAccountsPayableAmount = 0;
                TotalInventoryAmount = 0;

                foreach (var customer in customers)
                {
                    TotalBalanceAmount += customer.Balance;
                    if (customer.Balance >= 0)
                        TotalAccountsReceivableAmount += customer.Balance;
                    else
                        TotalAccountsPayableAmount += (customer.Balance * -1);
                }

                foreach (var transaction in transactions)
                {
                    if (transaction.Type == CustomerTransactionType.Invoice)
                    {
                        var customer = customers.First(o => o.Id == transaction.CustomerId);
                        if (customer.Name == "Cash")
                            TotalCashInvoiceAmount += transaction.Amount;
                        else
                            TotalAccountInvoiceAmount += transaction.Amount;
                    }
                    else if (transaction.Type == CustomerTransactionType.Loan)
                    {
                        TotalLoanAmount -= transaction.Amount;
                    }
                    else if (transaction.Type == CustomerTransactionType.Payment)
                    {
                        TotalPaymentAmount += transaction.Amount * -1;
                    }
                }

                foreach (var product in products)
                {
                    if (product.Price == 1.0m)
                        TotalInventoryAmount += (0.75m * product.QuantityOnHand);
                    else
                        if (product.Price == 1.3m)
                            TotalInventoryAmount += (1.1m * product.QuantityOnHand);
                }

                Execute.OnUIThread(() =>
                {
                    //NotifyOfPropertyChange(() => TotalBalanceAmount);
                    //NotifyOfPropertyChange(() => TotalAccountInvoiceAmount);
                    //NotifyOfPropertyChange(() => TotalCashInvoiceAmount);
                    //NotifyOfPropertyChange(() => TotalInvoiceAmount);
                    //NotifyOfPropertyChange(() => TotalLoanAmount);
                    //NotifyOfPropertyChange(() => TotalPaymentAmount);

                    NotifyOfPropertyChange(() => TotalCashAmount);
                    NotifyOfPropertyChange(() => TotalInventoryAmount);
                    NotifyOfPropertyChange(() => TotalAccountsReceivableAmount);

                    NotifyOfPropertyChange(() => TotalAccountsPayableAmount);
                    NotifyOfPropertyChange(() => TotalEquityAmount);

                    NotifyOfPropertyChange(() => TotalAssetValue);
                    NotifyOfPropertyChange(() => TotalLiabilityValue);

                    IsLoading = false;
                });
            });
        }

        public void Refresh()
        {
            if (CanRefresh)
                Load();
        }
    }
}
