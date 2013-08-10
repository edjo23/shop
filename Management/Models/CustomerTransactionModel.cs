using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.Contracts.Entities;

namespace Shop.Management.Models
{
    public class CustomerTransactionModel
    {
        public CustomerTransactionModel(CustomerTransaction customerTransaction, Customer customer)
        {
            CustomerTransaction = customerTransaction;
            Customer = customer;
        }

        public CustomerTransaction CustomerTransaction { get; private set; }

        public Customer Customer { get; private set; }

        public string DateTimeText
        {
            get
            {
                return CustomerTransaction.DateTime.ToString();
            }
        }

        public string TypeText
        {
            get
            {
                switch (CustomerTransaction.Type)
                {
                    case CustomerTransactionType.Invoice:
                        return "Invoice";
                    case CustomerTransactionType.Payment:
                        return "Payment";
                    case CustomerTransactionType.Adjustment:
                        return "Adjustment";
                    case CustomerTransactionType.Loan:
                        return "Loan";
                }

                return CustomerTransaction.Type.ToString();
            }
        }

        public string AmountText
        {
            get
            {
                return CustomerTransaction.Amount.ToString("C");
            }
        }
    }
}
