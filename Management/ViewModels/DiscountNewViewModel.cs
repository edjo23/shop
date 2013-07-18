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
    public class DiscountNewViewModel : Screen
    {
        public DiscountNewViewModel(IEventAggregator eventAggregator, IDiscountService discountService, IProductService productService, ICustomerService customerService)
        {
            EventAggregator = eventAggregator;
            DiscountService = discountService;
            ProductService = productService;
            CustomerService = customerService;

            DisplayName = "New Discount";
            DiscountProducts = new BindableCollection<DiscountProductViewModel>();
            DiscountCustomers = new BindableCollection<DiscountCustomerViewModel>();
        }

        private readonly IEventAggregator EventAggregator;

        private readonly IDiscountService DiscountService;

        private readonly IProductService ProductService;

        private readonly ICustomerService CustomerService;

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

            foreach (var product in ProductService.GetProducts())
            {
                DiscountProducts.Add(new DiscountProductViewModel { Product = product, DiscountProduct = new DiscountProduct { ProductId = product.Id }, Discount = 0.0m });
            }

            foreach (var customer in CustomerService.GetCustomers())
            {
                DiscountCustomers.Add(new DiscountCustomerViewModel { Customer = customer, DiscountCustomer = new DiscountCustomer { CustomerId = customer.Id }, Selected = false });
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
            var model = new DiscountModel
            {
                Discount = new Discount { Description = Description },
                Products = new List<DiscountProduct>(DiscountProducts.Where(o => o.Discount > 0.0m).Select(o => o.DiscountProduct)),
                Customers = new List<DiscountCustomer>(DiscountCustomers.Where(o => o.Selected).Select(o => o.DiscountCustomer))
            };

            DiscountService.InsertDiscountModel(model);

            Close();
        }
    }
}
