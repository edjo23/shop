using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shop.Business.Managers;
using Shop.Business.Services;
using Shop.Contracts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    [TestClass]
    public class SampleData
    {
        [TestMethod]
        public void AddCustomers()
        {
            var service = new CustomerService(new CustomerManager());

            for (int i = 101; i <= 150; i++)
            {
                var id = String.Format("{0:00}", i);
                var customer = new Customer { Id = Guid.Empty, Name = "Customer " + id, Number = id };
                service.AddCustomer(customer);
            }
        }
    }
}
