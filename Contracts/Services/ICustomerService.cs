using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Shop.Contracts.Entities;

namespace Shop.Contracts.Services
{
    [ServiceContract]
    public interface ICustomerService
    {
        [OperationContract]
        IEnumerable<Customer> GetCustomers();

        [OperationContract]
        Customer GetCustomer(Guid id);

        [OperationContract]
        Customer GetCustomerByNumber(string number);

        [OperationContract]
        void AddCustomer(Customer product);

        [OperationContract]
        void UpdateCustomer(Customer product);

        [OperationContract]
        IEnumerable<CustomerTransaction> GetTransactions(Guid? customerId, DateTimeOffset? fromDate, DateTimeOffset? toDate);

        [OperationContract]
        void AddTransaction(CustomerTransaction transaction);

        [OperationContract]
        bool CheckPin(Guid customerId, string pin);

        [OperationContract]
        void UpdatePin(Guid customerId, string pin);
    }
}
