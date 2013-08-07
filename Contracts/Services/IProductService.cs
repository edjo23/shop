using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Shop.Contracts.Entities;

namespace Shop.Contracts.Services
{
    [ServiceContract]
    public interface IProductService
    {
        [OperationContract]
        IEnumerable<Product> GetProducts();

        [OperationContract]
        Product GetProduct(Guid id);

        [OperationContract]
        void AddProduct(Product product);

        [OperationContract]
        void UpdateProduct(Product product);

        [OperationContract]
        IEnumerable<ProductMovement> GetProductMovements(Guid productId);

        [OperationContract]
        void AddMovement(ProductMovement movement);
    }
}
