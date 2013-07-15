using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;
using Shop.Management.Messages;

namespace Shop.Management.ViewModels
{
    public class CustomersViewModel : Screen
    {
        public CustomersViewModel(IEventAggregator eventAggregator, ICustomerService customerService)
        {
            CustomerService = customerService;
            DisplayName = "Customers";

            EventAggregator = eventAggregator;
            Customers = new BindableCollection<Customer>();
        }

        public IEventAggregator EventAggregator { get; set; }

        public ICustomerService CustomerService { get; set; }

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

        public bool CanRefresh
        {
            get
            {
                return !IsLoading;
            }
        }

        public BindableCollection<Customer> Customers { get; set; }

        private Customer _SelectedCustomer = null;

        public Customer SelectedCustomer
        {
            get
            {
                return _SelectedCustomer;
            }
            set
            {
                if (value != _SelectedCustomer)
                {
                    _SelectedCustomer = value;

                    NotifyOfPropertyChange(() => SelectedCustomer);
                    NotifyOfPropertyChange(() => ItemCommandVisibility);
                }
            }
        }

        public Visibility ItemCommandVisibility
        {
            get
            {
                return SelectedCustomer == null ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Load(false);
        }

        protected void Load(bool refresh)
        {
            if (!refresh && Customers.Count > 0)
            {
                IsLoading = false;
                return;
            }

            IsLoading = true;

            Task.Factory.StartNew(() =>
            {
                Customers.Clear();
                Customers.AddRange(CustomerService.GetCustomers());
            }).ContinueWith(t =>
            {
                Execute.OnUIThread(() =>
                {
                    IsLoading = false;
                });
            });
        }

        public void RefreshView()
        {
            if (CanRefresh)
                Load(true);
        }

        public void ShowCustomerNew()
        {
            EventAggregator.Publish(new ActivateItem<CustomerNewViewModel>());
        }

        public void ShowInvoiceNew()
        {
            if (SelectedCustomer != null)
                EventAggregator.Publish(new ActivateItem<CustomerInvoiceViewModel>(o => o.Customer = SelectedCustomer));
        }

        public void ShowCustomerEdit()
        {
            if (SelectedCustomer != null)
                EventAggregator.Publish(new ActivateItem<CustomerEditViewModel>(o => o.Customer = SelectedCustomer));
        }

        public void ShowPaymentReceipt()
        {
            if (SelectedCustomer != null)
                EventAggregator.Publish(new ActivateItem<CustomerAccountPaymentViewModel>(o => o.Customer = SelectedCustomer));
        }

        public void ShowStockAdjustment()
        {
            if (SelectedCustomer != null)
                EventAggregator.Publish(new ActivateItem<CustomerAccountAdjustmentViewModel>(o => o.Customer = SelectedCustomer));
        }
    }
}
