using Shop.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.Contracts.Entities;

namespace Shop.Service.Client
{
    public class ProductClient : HttpClient, IProductService
    {
        public ProductClient(IUrlProvider urlProvider)
        {
            BaseUrl = urlProvider.GetBaseUrl();
        }

        private readonly string BaseUrl = "http://localhost:5000/api";

        public IEnumerable<Product> GetProducts()
        {
            return Get<List<Product>>($"{BaseUrl}/product");
        }

        public Product GetProduct(Guid id)
        {
            return Get<Product>($"{BaseUrl}/product/{id.ToString()}");
        }

        public void AddProduct(Product product)
        {
            Post($"{BaseUrl}/product", product);
        }

        public void UpdateProduct(Product product)
        {
            Put($"{BaseUrl}/product", product);
        }

        public void AddMovement(ProductMovement movement)
        {
            Post($"{BaseUrl}/product/{movement.ProductId.ToString()}/movement", movement);
        }

        public IEnumerable<ProductMovement> GetProductMovements(Guid productId)
        {
            return Get<IEnumerable<ProductMovement>>($"{BaseUrl}/product/{productId.ToString()}/movement");
        }
    }
}
