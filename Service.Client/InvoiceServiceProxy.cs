using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;

namespace Shop.Service.Client
{
    public class InvoiceServiceProxy : ServiceProxy<IInvoiceService>, IInvoiceService
    {
        public InvoiceServiceProxy(IServiceClientFactory factory)
            : base(factory)
        {
        }

        public void AddInvoice(Invoice invoice, IEnumerable<InvoiceItem> items, decimal payment)
        {
            Invoke(s => s.AddInvoice(invoice, items, payment));
        }

        public IEnumerable<InvoiceItem> GetInvoiceItems(Guid invoiceId)
        {
            return Invoke(s => s.GetInvoiceItems(invoiceId));
        }
    }
}
