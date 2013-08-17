using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;

namespace Service.Client
{
    public class ProductServiceProxy : ServiceProxy<IProductService>, IProductService
    {
        public ProductServiceProxy()
        {
            EndPointAddress = "http://localhost:60233/Services/ProductService.svc";
        }
        public IEnumerable<Product> GetProducts()
        {
            return Invoke(s => s.GetProducts());
        }

        public Product GetProduct(Guid id)
        {
            return Invoke(s => s.GetProduct(id));
        }

        public void AddProduct(Product product)
        {
            Invoke(s => s.AddProduct(product));
        }

        public void UpdateProduct(Product product)
        {
            Invoke(s => s.UpdateProduct(product));
        }

        public IEnumerable<ProductMovement> GetProductMovements(Guid productId)
        {
            return Invoke(s => s.GetProductMovements(productId));
        }

        public void AddMovement(ProductMovement movement)
        {
            Invoke(s => s.AddMovement(movement));
        }
    }
}
