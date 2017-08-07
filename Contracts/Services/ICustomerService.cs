using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.Contracts.Entities;

namespace Shop.Contracts.Services
{
    public interface ICustomerService
    {
        IEnumerable<Customer> GetCustomers();

        IEnumerable<Customer> GetAllCustomers();

        Customer GetCustomer(Guid id);

        Customer GetCustomerByNumber(string number);

        void AddCustomer(Customer customer);

        void UpdateCustomer(Customer customer);

        IEnumerable<CustomerTransaction> GetTransactions(Guid? customerId, DateTimeOffset? fromDate, DateTimeOffset? toDate);

        void AddTransaction(CustomerTransaction transaction);

        bool CheckPin(Guid customerId, string pin);

        void UpdatePin(Guid customerId, string pin);
    }
}
