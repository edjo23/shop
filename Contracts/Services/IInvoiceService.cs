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
        void AddInvoice(InvoiceTransaction transaction);

        IEnumerable<InvoiceItem> GetInvoiceItems(Guid invoiceId);
    }

    public interface IReceiptService
    {
        void AddReceipt(InvoiceTransaction transaction);
    }
}
