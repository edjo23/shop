using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;
using Shop.Management.Messages;

namespace Shop.Management.ViewModels
{
    public class ProductMovementsViewModel : Screen
    {
        public ProductMovementsViewModel(IEventAggregator eventAggregator, IProductService productService)
        {
            EventAggregator = eventAggregator;
            ProductService = productService;

            DisplayName = "Movements";
            Movements = new BindableCollection<ProductMovementViewModel>();
        }

        public IEventAggregator EventAggregator { get; set; }

        public IProductService ProductService { get; set; }

        public Product Product { get; set; }

        public BindableCollection<ProductMovementViewModel> Movements { get; set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            DisplayName = Product.Description + " " + DisplayName;

            Load();
        }

        protected void Load()
        {
            Movements.Clear();
            Movements.AddRange(ProductService.GetProductMovements(Product.Id).Select(o => new ProductMovementViewModel(o)));
        }

        public void Cancel()
        {
            Close();
        }

        public void Close()
        {
            EventAggregator.Publish(new DeactivateItem { Item = this });
        }
    }
}
