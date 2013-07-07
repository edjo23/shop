using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Shop.Contracts.Services;
using Shop.PointOfSale.Services;

namespace Shop.PointOfSale.ViewModels
{
    public class SaleViewModel : Screen
    {
        public SaleViewModel(ScreenCoordinator screenCoordinator, IProductService productService)
        {
            ScreenCoordinator = screenCoordinator;
            ProductService = productService;

            DisplayName = "New Purchase";
        }

        private readonly ScreenCoordinator ScreenCoordinator;

        private readonly IProductService ProductService;

        public BindableCollection<SaleItemViewModel> Products { get; set; }

        public decimal TotalQuantity
        {
            get
            {
                return Products.Sum(o => o.Quantity); ;
            }
        }

        public decimal Total
        {
            get
            {
                return Products.Sum(o => o.Total);
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Products = new BindableCollection<SaleItemViewModel>(ProductService.GetProducts().Select((o, i) => new SaleItemViewModel { Product = o }));
        }

        public void AddToCart(SaleItemViewModel item)
        {
            item.Quantity += 1;

            NotifyOfPropertyChange(() => TotalQuantity);
            NotifyOfPropertyChange(() => Total);
        }

        public void RemoveFromCart(SaleItemViewModel item)
        {
            item.Quantity -= 1;

            NotifyOfPropertyChange(() => TotalQuantity);
            NotifyOfPropertyChange(() => Total);
        }

        public void Checkout()
        {
            ScreenCoordinator.GoToHome();
        }
    }
}
