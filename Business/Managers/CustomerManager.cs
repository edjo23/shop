using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using DapperExtensions;
using Shop.Business.Database;
using Shop.Contracts.Entities;

namespace Shop.Business.Managers
{
    public class CustomerManager
    {
        public IEnumerable<Customer> GetCustomers()
        {
            return Extensions.SelectAll<Customer>().OrderBy(o => o.Name);
        }

        public Customer GetCustomer(Guid id)
        {
            return Extensions.SelectById<Customer>(id);
        }

        public void AddCustomer(Customer customer)
        {
            if (customer.Id == Guid.Empty)
            {
                customer.Id = Guid.NewGuid();
            }

            customer.Insert();
        }

        public void UpdateCustomer(Customer customer)
        {
            customer.Update();
        }

        public void AddTransaction(CustomerTransaction customerTransaction)
        {
            using (var transaction = new TransactionScope())
            using (var connection = new ConnectionScope())
            {
                var customer = GetCustomer(customerTransaction.CustomerId);
                if (customer == null)
                    throw new Exception("Customer not found");

                customerTransaction.Insert();
                customer.Balance += customerTransaction.Amount;
                customer.Update();

                transaction.Complete();
            }
        }

        public IEnumerable<CustomerTransaction> GetTransactions(Guid? customerId, DateTimeOffset? fromDate, DateTimeOffset? toDate)
        {
            using (var connectionScope = new ConnectionScope())
            {
                var where = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                if (customerId.HasValue)
                    where.Predicates.Add(Predicates.Field<CustomerTransaction>(f => f.CustomerId, Operator.Eq, customerId.Value));
                if (fromDate.HasValue)
                    where.Predicates.Add(Predicates.Field<CustomerTransaction>(f => f.DateTime, Operator.Ge, fromDate.Value));
                if (toDate.HasValue)
                    where.Predicates.Add(Predicates.Field<CustomerTransaction>(f => f.DateTime, Operator.Le, toDate.Value));

                return connectionScope.Connection.GetList<CustomerTransaction>(where).ToList();
            }
        }
    }
}
