using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.Business.Managers;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;

namespace Shop.Business.Services
{
    public class InvoiceService : IInvoiceService
    {
        public InvoiceService(InvoiceManager manager)
        {
            Manager = manager;
        }

        private InvoiceManager Manager { get; set; }

        public void AddInvoice(Invoice invoice, IEnumerable<InvoiceItem> items, decimal payment)
        {
            Manager.AddInvoice(invoice, items, payment);
        }

        public void AddReceipt(Invoice invoice, IEnumerable<InvoiceItem> items)
        {
            Manager.AddReceipt(invoice, items);
        }


        public IEnumerable<InvoiceItem> GetInvoiceItems(Guid invoiceId)
        {
            return Manager.GetInvoiceItems(invoiceId);
        }
    }
}
