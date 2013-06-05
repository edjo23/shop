using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Business.Services;
using Caliburn.Micro;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;

namespace Shop.Management.ViewModels
{
    public class CustomersViewModel : Screen
    {
        public CustomersViewModel(ICustomerService customerService)
        {
            CustomerService = customerService;
            DisplayName = "Customers";

            Customers = new BindableCollection<Customer>();
        }

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

        private Product _SelectedCustomer = null;

        public Product SelectedCustomer
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
                }
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

        public void Refresh()
        {
            if (CanRefresh)
                Load(true);
        }
    }
}
