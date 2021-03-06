﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using DapperExtensions;
using Shop.Business.Database;
using Shop.Contracts.Entities;

namespace Shop.Business.Managers
{
    public class ProductManager
    {
        public IEnumerable<Product> GetProducts()
        {
            return Extensions.SelectAll<Product>().OrderBy(o => o.Description);
        }

        public Product GetProduct(Guid id)
        {
            return Extensions.SelectById<Product>(id);
        }

        public void AddProduct(Product product)
        {
            product.Insert();
        }

        public void UpdateProduct(Product product)
        {
            product.Update();
        }

        public IEnumerable<ProductMovement> GetProductMovements(Guid productId)
        {
            using (var connection = new ConnectionScope())
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

                return connection.Connection.Query<ProductMovement>(sql, new { ProductId = productId });
            }
        }

        public void AddMovement(ProductMovement movement)
        {
            using (var transaction = new TransactionScope())
            using (var connection = new ConnectionScope())
            {
                var product = GetProduct(movement.ProductId);
                if (product == null)
                    throw new Exception("Product not found");

                if (product.QuantityOnHand + movement.Quantity < 0)
                    throw new Exception("Quantity on hand cannot become negative.");

                movement.Insert();
                product.QuantityOnHand += movement.Quantity;
                product.Update();

                transaction.Complete();
            }
        }
    }
}
