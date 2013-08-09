using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;

namespace Service.Client
{
    public class CustomerServiceProxy : ServiceProxy<IProductService>, IProductService
    {
        public IEnumerable<Product> GetProducts()
        {
            return Invoke(s => GetProducts());
        }

        public Product GetProduct(Guid id)
        {
            return Invoke(s => GetProduct(id));
        }

        public void AddProduct(Product product)
        {
            Invoke(s => AddProduct(product));
        }

        public void UpdateProduct(Product product)
        {
            Invoke(s => UpdateProduct(product));
        }

        public IEnumerable<ProductMovement> GetProductMovements(Guid productId)
        {
            return Invoke(s => GetProductMovements(productId));
        }

        public void AddMovement(ProductMovement movement)
        {
            Invoke(s => AddMovement(movement));
        }
    }
}
