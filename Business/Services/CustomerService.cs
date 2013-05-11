using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.Business.Managers;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;

namespace Business.Services
{
    public class CustomerService : ICustomerService
    {
        public CustomerService(CustomerManager manager)
        {
            Manager = manager;
        }

        private CustomerManager Manager { get; set; }

        public IEnumerable<Customer> GetCustomers()
        {
            return Manager.GetCustomers();
        }

        public Customer GetCustomer(Guid id)
        {
            return Manager.GetCustomer(id);
        }

        public void AddCustomer(Customer customer)
        {
            Manager.AddCustomer(customer);
        }

        public void UpdateCustomer(Customer customer)
        {
            Manager.UpdateCustomer(customer);
        }

        public void AddTransaction(CustomerTransaction transaction)
        {
            Manager.AddTransaction(transaction);
        }
    }
}
