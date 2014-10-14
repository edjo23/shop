using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;

namespace Shop.Service.Client
{
    public class CustomerServiceProxy : ServiceProxy<ICustomerService>, ICustomerService
    {
        public CustomerServiceProxy(IServiceClientFactory factory)
            : base(factory)
        {
        }

        public IEnumerable<Customer> GetCustomers()
        {
            return Invoke(s => s.GetCustomers());
        }

        public Customer GetCustomer(Guid id)
        {
            return Invoke(s => s.GetCustomer(id));
        }

        public Customer GetCustomerByNumber(string number)
        {
            return Invoke(s => s.GetCustomerByNumber(number));
        }

        public void AddCustomer(Customer product)
        {
            Invoke(s => s.AddCustomer(product));
        }

        public void UpdateCustomer(Customer product)
        {
            Invoke(s => s.UpdateCustomer(product));
        }

        public IEnumerable<CustomerTransaction> GetTransactions(Guid? customerId, DateTimeOffset? fromDate, DateTimeOffset? toDate)
        {
            return Invoke(s => s.GetTransactions(customerId, fromDate, toDate));
        }

        public void AddTransaction(CustomerTransaction transaction)
        {
            Invoke(s => s.AddTransaction(transaction));
        }

        public bool CheckPin(Guid customerId, string pin)
        {
            return Invoke(s => s.CheckPin(customerId, pin));
        }

        public void UpdatePin(Guid customerId, string pin)
        {
            Invoke(s => s.UpdatePin(customerId, pin));
        }
    }
}
