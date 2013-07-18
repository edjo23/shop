using System;
using System.Collections.Generic;
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
    public class DiscountsViewModel : Screen
    {
        public DiscountsViewModel(IDiscountService discountService, IEventAggregator eventAggregator)
        {
            DiscountService = discountService;
            EventAggregator = eventAggregator;

            EventAggregator = IoC.Get<IEventAggregator>();

            DisplayName = "Discounts";
            Discounts = new BindableCollection<Discount>();
        }

        public IDiscountService DiscountService { get; set; }

        public IEventAggregator EventAggregator { get; set; }

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
                    NotifyOfPropertyChange(() => CanNew);
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

        public bool CanNew
        {
            get
            {
                return !IsLoading;
            }
        }

        public BindableCollection<Discount> Discounts { get; set; }

        private Discount _SelectedDiscount = null;

        public Discount SelectedDiscount
        {
            get
            {
                return _SelectedDiscount;
            }
            set
            {
                if (value != _SelectedDiscount)
                {
                    _SelectedDiscount = value;

                    NotifyOfPropertyChange(() => SelectedDiscount);
                    NotifyOfPropertyChange(() => ItemCommandVisibility);
                }
            }
        }

        public Visibility ItemCommandVisibility
        {
            get
            {
                return SelectedDiscount == null ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Load(false);
        }

        protected void Load(bool refresh)
        {
            if (!refresh && Discounts.Count > 0)
            {
                IsLoading = false;
                return;
            }

            IsLoading = true;

            Task.Factory.StartNew(() =>
            {
                Discounts.Clear();
                Discounts.AddRange(DiscountService.GetDiscounts());
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

        public void ShowDiscountNew()
        {
            EventAggregator.Publish(new ActivateItem<DiscountNewViewModel>());
        }

        public void ShowDiscountEdit()
        {
            if (SelectedDiscount != null)
                EventAggregator.Publish(new ActivateItem<DiscountEditViewModel>(o => o.Discount = SelectedDiscount));
        }
    }
}
