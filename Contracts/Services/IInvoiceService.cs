using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.Contracts.Entities;

namespace Shop.Contracts.Services
{
    public interface IInvoiceService
    {
        void AddInvoice(Invoice invoice, IEnumerable<InvoiceItem> items);
    }
}
