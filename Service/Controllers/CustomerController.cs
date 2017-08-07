using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Server.Controllers
{
    [Route("api/[controller]")]
    public class CustomerController : Controller
    {
        public CustomerController(ILoggerFactory loggerFactory, ICustomerService customerService)
        {
            Logger = loggerFactory.CreateLogger("CustomerController");
            CustomerService = customerService;
        }

        private readonly ILogger Logger;
        private readonly ICustomerService CustomerService;

        [HttpGet]
        public IEnumerable<Customer> GetCustomers()
        {
            return CustomerService.GetCustomers();
        }

        [HttpGet("all")]
        public IEnumerable<Customer> GetAllCustomers()
        {
            return CustomerService.GetAllCustomers();
        }

        [HttpGet("{id:Guid}")]
        public Customer GetCustomer(Guid id)
        {
            return CustomerService.GetCustomer(id);
        }

        [HttpGet("number/{number}")]
        public Customer GetCustomerByNumber(string number)
        {
            return CustomerService.GetCustomerByNumber(number);
        }

        [HttpPost]
        public void AddCustomer([FromBody] Customer customer)
        {
            CustomerService.AddCustomer(customer);
        }

        [HttpPut]
        public void UpdateCustomer([FromBody] Customer customer)
        {
            CustomerService.UpdateCustomer(customer);
        }

        [HttpGet("transaction")]
        public IEnumerable<CustomerTransaction> GetTransactions(Guid? customerId, DateTimeOffset? fromDate, DateTimeOffset? toDate)
        {
            Logger.LogInformation("Executing GetTransactions: {0}", JsonConvert.SerializeObject(new { customerId, fromDate, toDate }));

            return CustomerService.GetTransactions(customerId, fromDate, toDate);
        }

        [HttpPost("transaction")]
        public void AddTransaction([FromBody] CustomerTransaction transaction)
        {
            Logger.LogInformation("Executing AddTransaction: {0}", JsonConvert.SerializeObject(transaction));

            CustomerService.AddTransaction(transaction);
        }

        [HttpPost("{customerId:Guid}/pin")]
        public IActionResult CheckPin(Guid customerId, [FromBody] string pin)
        {
            Logger.LogInformation("Executing CheckPin: {0}", JsonConvert.SerializeObject(new { customerId, pin }));

            var ok = CustomerService.CheckPin(customerId, pin);
            return ok ? Ok() as IActionResult : NoContent() as IActionResult;
        }

        [HttpPut("{customerId:Guid}/pin")]
        public void UpdatePin(Guid customerId, [FromBody] string pin)
        {
            Logger.LogInformation("Executing CheckPin: {0}", JsonConvert.SerializeObject(new { customerId, pin }));

            CustomerService.UpdatePin(customerId, pin);
        }
    }
}
