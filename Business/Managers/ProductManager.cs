using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Business.Entities;
using DapperExtensions;

namespace Business.Managers
{
    public class ProductManager
    {
        public IEnumerable<Product> GetProducts()
        {
            using (var connectionScope = new ConnectionScope())
            {
                return connectionScope.Connection.GetList<Product>();
            }
        }

        public Product GetProduct(Guid id)
        {
            using (var connectionScope = new ConnectionScope())
            {
                return connectionScope.Connection.Get<Product>(id);
            }
        }

        public void AddProduct(Product product, IDbConnection useConnection = null)
        {
            using (var transactionScope = new TransactionScope())
            using (var connectionScope = new ConnectionScope())
            {
                connectionScope.Connection.Insert(product);
                transactionScope.Complete();
            }
        }

        public void AddMovement(ProductMovement movement)
        {
            using (var transaction = new TransactionScope())
            using (var connection = new ConnectionScope())
            {
                connection.Connection.Insert(movement);

                var product = connection.Connection.Get<Product>(movement.ProductId);
                if (product == null)
                    throw new Exception("Product not found");

                product.QuantityOnHand += movement.Quantity;
                connection.Connection.Update(product);

                transaction.Complete();
            }
        }
    }
}
