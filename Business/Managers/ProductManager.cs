using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Shop.Business.Database;
using Shop.Contracts.Entities;
using System.Data;

namespace Shop.Business.Managers
{
    public class ProductManager
    {
        public ProductManager(IConnectionProvider connectionProvider)
        {
            ConnectionProvider = connectionProvider;
        }

        private readonly IConnectionProvider ConnectionProvider;

        public IEnumerable<Product> GetProducts()
        {
            using (var connection = ConnectionProvider.CreateConnection())
            {
                return connection.DbConnection.GetAll<Product>(connection.DbTransaction).OrderBy(o => o.Description);
            }
        }

        public Product GetProduct(Guid id)
        {
            using (var connection = ConnectionProvider.CreateConnection())
            {
                return connection.DbConnection.Get<Product>(id, connection.DbTransaction);
            }
        }

        public void AddProduct(Product product)
        {
            using (var connection = ConnectionProvider.CreateConnection(true))
            {
                connection.DbConnection.Insert(product, connection.DbTransaction);
                connection.Commit();
            }
        }

        public void UpdateProduct(Product product)
        {
            using (var connection = ConnectionProvider.CreateConnection(true))
            {
                connection.DbConnection.Update(product, connection.DbTransaction);
                connection.Commit();
            }
        }

        public IEnumerable<ProductMovement> GetProductMovements(Guid productId)
        {
            using (var connection = ConnectionProvider.CreateConnection())
            {
                var sql = @"
                    select
	                    Id,
	                    ProductId,
	                    MovementType,
	                    Quantity,
	                    DateTime,
	                    SourceId,
	                    SourceItemNumber
                    from
	                    dbo.ProductMovement
                    where
	                    ProductId = @ProductId
                    order by                
                        DateTime desc,
                        MovementType";

                return connection.DbConnection.Query<ProductMovement>(sql, new { ProductId = productId }, connection.DbTransaction);
            }
        }

        public void AddMovement(ProductMovement movement)
        {
            using (var connection = ConnectionProvider.CreateConnection(true))
            {
                var product = GetProduct(movement.ProductId);
                if (product == null)
                    throw new Exception("Product not found");

                if (product.QuantityOnHand + movement.Quantity < 0)
                    throw new Exception("Quantity on hand cannot become negative.");

                connection.DbConnection.Insert(movement, connection.DbTransaction);
                product.QuantityOnHand += movement.Quantity;
                connection.DbConnection.Update(product, connection.DbTransaction);

                connection.Commit();
            }
        }
    }
}
