using Shop.Contracts.Services;
using System;
using System.Collections.Generic;
using Shop.Contracts.Entities;

namespace Shop.Service.Client
{
    public class CustomerClient : HttpClient, ICustomerService
    {
        public CustomerClient(IUrlProvider urlProvider)
        {
            BaseUrl = urlProvider.GetBaseUrl();
        }

        private readonly string BaseUrl = "http://localhost:5000/api";

        public IEnumerable<Customer> GetCustomers()
        {
            return Get<List<Customer>>($"{BaseUrl}/customer");
        }

        public IEnumerable<Customer> GetAllCustomers()
        {
            return Get<List<Customer>>($"{BaseUrl}/customer/all");
        }

        public Customer GetCustomer(Guid id)
        {
            return Get<Customer>($"{BaseUrl}/customer/{id.ToString()}");
        }

        public Customer GetCustomerByNumber(string number)
        {
            return Get<Customer>($"{BaseUrl}/customer/number/{number}");
        }

        public void AddCustomer(Customer customer)
        {
            Post($"{BaseUrl}/customer", customer);
        }

        public void UpdateCustomer(Customer customer)
        {
            Put($"{BaseUrl}/customer", customer);
        }

        public IEnumerable<CustomerTransaction> GetTransactions(Guid? customerId, DateTimeOffset? fromDate, DateTimeOffset? toDate)
        {
            var parameters = new Dictionary<string, string>();

            if (customerId.HasValue)
                parameters.Add("customerId", customerId.Value.ToString());
            if (fromDate.HasValue)
                parameters.Add("fromDate", fromDate.Value.ToString("yyy-MM-ddTHH:mm:ssZ"));
            if (toDate.HasValue)
                parameters.Add("toDate", toDate.Value.ToString("yyy-MM-ddTHH:mm:ssZ"));

            return Get<List<CustomerTransaction>>($"{BaseUrl}/customer/transaction", parameters);
        }

        public void AddTransaction(CustomerTransaction transaction)
        {           
            Post($"{BaseUrl}/customer/transaction", transaction);
        }

        public bool CheckPin(Guid customerId, string pin)
        {
            return Post($"{BaseUrl}/customer/{customerId.ToString()}/pin", pin) == System.Net.HttpStatusCode.OK;
        }

        public void UpdatePin(Guid customerId, string pin)
        {
            Put($"{BaseUrl}/customer/{customerId.ToString()}/pin", pin);
        }
    }
}
