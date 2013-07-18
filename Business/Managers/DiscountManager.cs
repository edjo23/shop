using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DapperExtensions;
using Shop.Business.Database;
using Shop.Contracts.Entities;

namespace Shop.Business.Managers
{
    public class DiscountManager
    {
        public IEnumerable<Discount> GetDiscounts()
        {
            return Extensions.SelectAll<Discount>();
        }

        public DiscountModel GetDiscountModel(Guid id)
        {
            using (var connectionScope = new ConnectionScope())
            {
                var result = new DiscountModel();

                result.Discount = connectionScope.Connection.Get<Discount>(id);
                result.Products = connectionScope.Connection.GetList<DiscountProduct>(Predicates.Field<DiscountProduct>(f => f.DiscountId, Operator.Eq, id)).ToList();
                result.Customers = connectionScope.Connection.GetList<DiscountCustomer>(Predicates.Field<DiscountCustomer>(f => f.DiscountId, Operator.Eq, id)).ToList();

                return result;
            }
        }

        public DiscountModel InsertDiscountModel(DiscountModel entity)
        {
            // TODO - Make this more efficient.
            using (var connectionScope = new ConnectionScope())
            {
                connectionScope.Connection.Insert<Discount>(entity.Discount);

                foreach (var product in entity.Products)
                    product.DiscountId = entity.Discount.Id;
                foreach (var customer in entity.Customers)
                    customer.DiscountId = entity.Discount.Id;

                connectionScope.Connection.Insert<DiscountProduct>(entity.Products);
                connectionScope.Connection.Insert<DiscountCustomer>(entity.Customers);
            }

            return entity;
        }

        public DiscountModel UpdateDiscountModel(DiscountModel entity)
        {
            // TODO - Make this more efficient.
            using (var connectionScope = new ConnectionScope())
            {
                connectionScope.Connection.Delete<DiscountProduct>(Predicates.Field<DiscountProduct>(f => f.DiscountId, Operator.Eq, entity.Discount.Id));
                connectionScope.Connection.Delete<DiscountCustomer>(Predicates.Field<DiscountCustomer>(f => f.DiscountId, Operator.Eq, entity.Discount.Id));

                connectionScope.Connection.Update<Discount>(entity.Discount);

                foreach (var product in entity.Products)
                    product.DiscountId = entity.Discount.Id;
                foreach (var customer in entity.Customers)
                    customer.DiscountId = entity.Discount.Id;

                connectionScope.Connection.Insert<DiscountProduct>(entity.Products);
                connectionScope.Connection.Insert<DiscountCustomer>(entity.Customers);
            }

            return entity;
        }

        public IEnumerable<DiscountProduct> GetDiscountProductsByCustomerId(Guid customerId)
        {
            using (var connectionScope = new ConnectionScope())
            {
                var sql = @"
                    select 
                        * 
                    from 
                        [DiscountProduct] dp
                        join [DiscountCustomer] dc on dc.DiscountId = dp.DiscountId and dc.CustomerId = @CustomerId";

                return connectionScope.Connection.Query<DiscountProduct>(sql, new { CustomerId = customerId }).ToList();
            }
        }
    }
}
