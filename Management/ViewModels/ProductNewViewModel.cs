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
    public class ProductNewViewModel : Screen
    {
        public ProductNewViewModel (IEventAggregator eventAggregator, IProductService productService)
        {
            EventAggregator = eventAggregator;
            ProductService = productService;

            DisplayName = "New Product";
        }

        public IEventAggregator EventAggregator { get; set; }

        public IProductService ProductService { get; set; }

        #region Code Property

        private string _Code;

        public string Code
        {
            get
            {
                return _Code;
            }
            set
            {
                if (value != _Code)
                {
                    _Code = value;
                    NotifyOfPropertyChange(() => Code);
                }
            }
        }

        #endregion

        #region Group Property

        private string _Group;

        public string Group
        {
            get
            {
                return _Group;
            }
            set
            {
                if (value != _Group)
                {
                    _Group = value;
                    NotifyOfPropertyChange(() => Group);
                }
            }
        }

        #endregion

        #region Description Property

        private string _Description;

        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                if (value != _Description)
                {
                    _Description = value;
                    NotifyOfPropertyChange(() => Description);
                }
            }
        }

        #endregion

        #region Price Property

        private decimal _Price;

        public decimal Price
        {
            get
            {
                return _Price;
            }
            set
            {
                if (value != _Price)
                {
                    _Price = value;
                    NotifyOfPropertyChange(() => Price);
                }
            }
        }

        #endregion

        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        public void Save()
        {
            var product = new Product();
            product.Id = Guid.NewGuid();
            product.Code = Code;
            product.Group = Group;
            product.Description = Description;
            product.Price = Price;

            ProductService.AddProduct(product);

            Close();
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
