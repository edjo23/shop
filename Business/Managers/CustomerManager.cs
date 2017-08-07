using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using Shop.Business.Database;
using Shop.Contracts.Entities;
using Microsoft.Extensions.Configuration;

namespace Shop.Business.Managers
{
    public class CustomerManager
    {
        public CustomerManager(IConfiguration configuration, IConnectionProvider connectionProvider)
        {
            ConnectionProvider = connectionProvider;
            PinSalt = configuration.GetSection("PinSalt").Value;
        }

        private readonly IConnectionProvider ConnectionProvider;

        public string PinSalt { get; set; }

        public IEnumerable<Customer> GetCustomers()
        {
            using (var connection = ConnectionProvider.CreateConnection())
            {
                return connection.DbConnection.GetAll<Customer>(connection.DbTransaction).OrderBy(o => o.Name);
            }
        }

        public Customer GetCustomer(Guid id)
        {
            using (var connection = ConnectionProvider.CreateConnection())
            {
                return connection.DbConnection.Get<Customer>(id, connection.DbTransaction);
            }
        }

        public Customer GetCustomerByNumber(string number)
        {
            using (var connection = ConnectionProvider.CreateConnection())
            {
                return connection.DbConnection.QueryFirst<Customer>(@"select * from Customer where Number = @Number", new { Number = number }, connection.DbTransaction);
            }
        }

        public void AddCustomer(Customer customer)
        {
            if (customer.Id == Guid.Empty)
            {
                customer.Id = Guid.NewGuid();
            }

            using (var connection = ConnectionProvider.CreateConnection(true))
            {
                connection.DbConnection.Insert(customer, connection.DbTransaction);
                connection.Commit();
            }
        }

        public void UpdateCustomer(Customer customer)
        {
            using (var connection = ConnectionProvider.CreateConnection(true))
            {
                connection.DbConnection.Update(customer, connection.DbTransaction);
                connection.Commit();
            }
        }

        public void AddTransaction(CustomerTransaction customerTransaction)
        {
            using (var connection = ConnectionProvider.CreateConnection(true))
            {
                var customer = connection.DbConnection.Get<Customer>(customerTransaction.CustomerId, connection.DbTransaction);
                if (customer == null)
                    throw new Exception("Customer not found");

                connection.DbConnection.Insert(customerTransaction, connection.DbTransaction);
                customer.Balance += customerTransaction.Amount;
                connection.DbConnection.Update(customer, connection.DbTransaction);

                connection.Commit();
            }
        }

        public IEnumerable<CustomerTransaction> GetTransactions(Guid? customerId, DateTimeOffset? fromDate, DateTimeOffset? toDate)
        {
            using (var connection = ConnectionProvider.CreateConnection())
            {
                var where = "where";

                if (customerId.HasValue)
                    where += " and CustomerId = @CustomerId";
                if (fromDate.HasValue)
                    where += " and DateTime >= @FromDate";
                if (toDate.HasValue)
                    where += " and DateTime <= @ToDate";

                where = where == "where" ? "" : where.Replace("where and", "where");

                return connection.DbConnection.Query<CustomerTransaction>($"select * from CustomerTransaction {where}", new { CustomerId = customerId.GetValueOrDefault(), FromDate = fromDate.GetValueOrDefault(), ToDate = toDate.GetValueOrDefault() }, connection.DbTransaction).ToList();
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
            var buffer = Encoding.ASCII.GetBytes(pin + PinSalt).Concat(customerId.ToByteArray()).ToArray();
            var hash = SHA256.Create().ComputeHash(buffer);           

            return Encoding.UTF8.GetString(hash);
        }
    }
}
