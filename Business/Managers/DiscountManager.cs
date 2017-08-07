using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Shop.Business.Database;
using Shop.Contracts.Entities;

namespace Shop.Business.Managers
{
    public class DiscountManager
    {
        public DiscountManager(IConnectionProvider connectionProvider)
        {
            ConnectionProvider = connectionProvider;
        }

        private readonly IConnectionProvider ConnectionProvider;

        public IEnumerable<Discount> GetDiscounts()
        {
            using (var connection = ConnectionProvider.CreateConnection())
            {
                return connection.DbConnection.GetAll<Discount>(connection.DbTransaction);
            }
        }

        public DiscountModel GetDiscountModel(Guid id)
        {
            using (var connection = ConnectionProvider.CreateConnection())
            {
                var result = new DiscountModel()
                {
                    Discount = connection.DbConnection.Get<Discount>(id, connection.DbTransaction),
                    Products = connection.DbConnection.Query<DiscountProduct>(@"select * from DiscountProduct where DiscountId = @DiscountId", new { DiscountId = id }, connection.DbTransaction),
                    Customers = connection.DbConnection.Query<DiscountCustomer>(@"select * from DiscountCustomer where DiscountId = @DiscountId", new { DiscountId = id }, connection.DbTransaction)
                };
                return result;
            }
        }

        public DiscountModel InsertDiscountModel(DiscountModel entity)
        {
            // TODO - Make this more efficient.
            using (var connection = ConnectionProvider.CreateConnection(true))
            {
                connection.DbConnection.Insert<Discount>(entity.Discount, connection.DbTransaction);

                foreach (var product in entity.Products)
                    product.DiscountId = entity.Discount.Id;
                foreach (var customer in entity.Customers)
                    customer.DiscountId = entity.Discount.Id;

                foreach (var product in entity.Products)
                    connection.DbConnection.Insert(product, connection.DbTransaction);
                foreach (var customer in entity.Customers)
                    connection.DbConnection.Insert(customer, connection.DbTransaction);

                connection.Commit();
            }

            return entity;
        }

        public DiscountModel UpdateDiscountModel(DiscountModel entity)
        {
            // TODO - Make this more efficient.
            using (var connection = ConnectionProvider.CreateConnection(true))
            {
                connection.DbConnection.Execute(@"delete from DiscountProduct where DiscountId = @DiscountId", new { DiscountId = entity.Discount.Id }, connection.DbTransaction);
                connection.DbConnection.Execute(@"delete from DiscountCustomer where DiscountId = @DiscountId", new { DiscountId = entity.Discount.Id }, connection.DbTransaction);

                connection.DbConnection.Update<Discount>(entity.Discount, connection.DbTransaction);

                foreach (var product in entity.Products)
                    product.DiscountId = entity.Discount.Id;
                foreach (var customer in entity.Customers)
                    customer.DiscountId = entity.Discount.Id;

                foreach (var product in entity.Products)
                    connection.DbConnection.Insert(product, connection.DbTransaction);
                foreach (var customer in entity.Customers)
                    connection.DbConnection.Insert(customer, connection.DbTransaction);

                connection.Commit();
            }

            return entity;
        }

        public IEnumerable<DiscountProduct> GetDiscountProductsByCustomerId(Guid customerId)
        {
            using (var connection = ConnectionProvider.CreateConnection())
            {
                var sql = @"
                    select 
                        * 
                    from 
                        [DiscountProduct] dp
                        join [DiscountCustomer] dc on dc.DiscountId = dp.DiscountId and dc.CustomerId = @CustomerId";

                return connection.DbConnection.Query<DiscountProduct>(sql, new { CustomerId = customerId }, connection.DbTransaction).ToList();
            }
        }
    }
}
