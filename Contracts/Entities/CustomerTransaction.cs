using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Contracts.Entities
{
    public static class CustomerTransactionType
    {
        public const int Invoice = 1;
        public const int Payment = 2;
        public const int Adjustment = 3;
        public const int CashAdvance = 4;
    }

    public class CustomerTransaction
    {
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }

        public DateTimeOffset DateTime { get; set; }

        public int Type { get; set; }

        public decimal Amount { get; set; }

        public Guid? SourceId { get; set; }
    }
}
