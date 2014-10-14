using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
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

        public Customer GetCustomerByNumber(string number)
        {
            using (var connectionScope = new ConnectionScope())
            {
                var where = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                where.Predicates.Add(Predicates.Field<Customer>(f => f.Number, Operator.Eq, number));

                return connectionScope.Connection.GetList<Customer>(where.Predicates.Any() ? where : null).FirstOrDefault();
            }
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

                return connectionScope.Connection.GetList<CustomerTransaction>(where.Predicates.Any() ? where : null).ToList();
            }
        }

        public bool CheckPin(Guid customerId, string pin)
        {
            var customer = GetCustomer(customerId);
            if (customer == null)
                throw new ArgumentOutOfRangeException("customerId");

            return (GetPinHash(customerId, pin) == customer.Pin);
        }

        public void UpdatePin(Guid customerId, string pin)
        {
            var customer = GetCustomer(customerId);
            if (customer == null)
                throw new ArgumentOutOfRangeException("customerId");

            customer.Pin = String.IsNullOrEmpty(pin) ? null : GetPinHash(customerId, pin);

            UpdateCustomer(customer);
        }

        public string GetPinHash(Guid customerId, string pin)
        {
            var buffer = ASCIIEncoding.Default.GetBytes(pin + ConfigurationManager.AppSettings["PinSalt"]).Concat(customerId.ToByteArray()).ToArray();
            var hash = new SHA256Managed().ComputeHash(buffer);           

            return ASCIIEncoding.UTF8.GetString(hash);
        }
    }
}
