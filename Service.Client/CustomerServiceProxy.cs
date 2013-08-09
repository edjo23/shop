using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;

namespace Service.Client
{
    public class CustomerServiceProxy : ServiceProxy<ICustomerService>, ICustomerService
    {
        public IEnumerable<Customer> GetCustomers()
        {
            return Invoke(s => s.GetCustomers());
        }

        public Customer GetCustomer(Guid id)
        {
            return Invoke(s => s.GetCustomer(id));
        }

        public void AddCustomer(Customer product)
        {
            Invoke(s => s.AddCustomer(product));
        }

        public void UpdateCustomer(Customer product)
        {
            Invoke(s => s.UpdateCustomer(product));
        }

        public void AddTransaction(CustomerTransaction transaction)
        {
            Invoke(s => s.AddTransaction(transaction));
        }
    }
}
