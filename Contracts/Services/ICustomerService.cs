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
        Customer GetCustomer(Guid id);
        void AddCustomer(Customer product);
        void UpdateCustomer(Customer product);
        void AddTransaction(CustomerTransaction transaction);
    }
}
