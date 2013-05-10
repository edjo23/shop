using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Business.Entities;
using DapperExtensions;

namespace Business.Managers
{
    public class CustomerManager
    {
        public IEnumerable<Customer> GetCustomers()
        {
            using (var connectionScope = new ConnectionScope())
            {
                return connectionScope.Connection.GetList<Customer>();
            }
        }

        public Customer GetCustomer(Guid id)
        {
            using (var connectionScope = new ConnectionScope())
            {
                return connectionScope.Connection.Get<Customer>(id);
            }
        }

        public void AddCustomer(Customer customer)
        {
            using (var transactionScope = new TransactionScope())
            using (var connectionScope = new ConnectionScope())
            {
                connectionScope.Connection.Insert(customer);
                transactionScope.Complete();
            }
        }

        public void UpdateCustomer(Customer customer)
        {
            using (var transactionScope = new TransactionScope())
            using (var connectionScope = new ConnectionScope())
            {
                connectionScope.Connection.Update(customer);
                transactionScope.Complete();
            }
        }

        public void AddTransaction(CustomerTransaction customerTransaction)
        {
            using (var transactionScope = new TransactionScope())
            using (var connectionScope = new ConnectionScope())
            {
                connectionScope.Connection.Insert(customerTransaction);

                var customer = GetCustomer(customerTransaction.CustomerId);
                if (customer == null)
                    throw new Exception("Customer not found");

                customer.Balance += customerTransaction.Amount;
                UpdateCustomer(customer);

                transactionScope.Complete();
            }
        }        
    }
}
