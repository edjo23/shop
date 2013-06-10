using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.Contracts.Entities;

namespace Shop.Contracts.Services
{
    public interface IProductService
    {
        IEnumerable<Product> GetProducts();
        Product GetProduct(Guid id);
        void AddProduct(Product product);
        void UpdateProduct(Product product);
        IEnumerable<ProductMovement> GetProductMovements(Guid productId);
        void AddMovement(ProductMovement movement);
    }
}
