using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.Business.Managers;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;

namespace Shop.Business.Services
{
    public class ProductService : IProductService
    {
        public ProductService(ProductManager manager)
        {
            Manager = manager;
        }

        public ProductManager Manager { get; set; }

        public IEnumerable<Product> GetProducts()
        {
            return Manager.GetProducts();
        }

        public Product GetProduct(Guid id)
        {
            return Manager.GetProduct(id);
        }

        public void AddProduct(Product product)
        {
            Manager.AddProduct(product);
        }

        public void UpdateProduct(Product product)
        {
            Manager.UpdateProduct(product);
        }

        public IEnumerable<ProductMovement> GetProductMovements(Guid productId)
        {
            return Manager.GetProductMovements(productId);
        }

        public void AddMovement(ProductMovement movement)
        {
            Manager.AddMovement(movement);
        }
    }
}
