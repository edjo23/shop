using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Contracts.Entities
{
    public class InvoiceTransaction
    {
        public Invoice Invoice { get; set; }

        public IEnumerable<InvoiceItem> Items { get; set; }

        public decimal? Payment { get; set; }
    }
}
