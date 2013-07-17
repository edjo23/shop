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
    public class DiscountEditViewModel : Screen
    {
        public DiscountEditViewModel (IEventAggregator eventAggregator, IDiscountService discountService, IProductService productService, ICustomerService customerService)
        {
            EventAggregator = eventAggregator;
            DiscountService = discountService;
            ProductService = productService;
            CustomerService = customerService;

            DisplayName = "Edit Discount";
            DiscountProducts = new BindableCollection<DiscountProductViewModel>();
            DiscountCustomers = new BindableCollection<DiscountCustomerViewModel>();
        }

        private readonly IEventAggregator EventAggregator;

        private readonly IDiscountService DiscountService;

        private readonly IProductService ProductService;

        private readonly ICustomerService CustomerService;

        public Discount Discount { get; set; }

        public BindableCollection<DiscountProductViewModel> DiscountProducts { get; set; }

        public BindableCollection<DiscountCustomerViewModel> DiscountCustomers { get; set; }

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

        protected override void OnInitialize()
        {
            base.OnInitialize();

            // TODO - Make this async.

            Description = Discount.Description;

            var model = DiscountService.GetDiscountModel(Discount.Id);

            foreach (var product in ProductService.GetProducts())
            {
                var discountProduct = model.Products.Where(o => o.ProductId == product.Id).FirstOrDefault();
                DiscountProducts.Add(new DiscountProductViewModel { Product = product, DiscountProduct = discountProduct, Discount = (discountProduct != null ? discountProduct.Discount : 0.0m) });
            }

            foreach (var customer in CustomerService.GetCustomers())
            {
                var discountCustomer = model.Customers.Where(o => o.CustomerId == customer.Id).FirstOrDefault();
                DiscountCustomers.Add(new DiscountCustomerViewModel { Customer = customer, DiscountCustomer = discountCustomer, Selected = discountCustomer != null });
            }
        }

        public void Cancel()
        {
            Close();
        }

        public void Close()
        {
            EventAggregator.Publish(new DeactivateItem { Item = this });
        }

        public void Save()
        {
            var model = DiscountService.GetDiscountModel(Discount.Id);
            model.Discount.Description = Description;
            model.Customers = new List<DiscountCustomer>();
            model.Products = new List<DiscountProduct>();

            foreach (var dcvm in DiscountCustomers.Where(o => o.Selected))
            {
                var dc = dcvm.DiscountCustomer;
                if (dc == null)
                {
                    dc = new DiscountCustomer();
                    dc.DiscountId = Discount.Id;
                    dc.CustomerId = dcvm.Customer.Id;
                }

                ((List<DiscountCustomer>)model.Customers).Add(dc);
            }

            foreach (var dpvm in DiscountProducts.Where(o => o.Discount > 0))
            {
                var dp = dpvm.DiscountProduct;
                if (dp == null)
                {
                    dp = new DiscountProduct();
                    dp.DiscountId = Discount.Id;
                    dp.ProductId = dpvm.Product.Id;
                }

                dp.Discount = dpvm.Discount;

                ((List<DiscountProduct>)model.Products).Add(dp);
            }

            DiscountService.UpdateDiscountModel(model);

            Close();
        }
    }
}
