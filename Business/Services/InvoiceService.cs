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
    public class InvoiceService : IInvoiceService, IReceiptService
    {
        public InvoiceService(InvoiceManager manager)
        {
            Manager = manager;
        }

        private InvoiceManager Manager { get; set; }

        public void AddInvoice(InvoiceTransaction transaction)
        {
            Manager.AddInvoice(transaction.Invoice, transaction.Items, transaction.Payment.GetValueOrDefault());
        }

        public void AddReceipt(InvoiceTransaction transaction)
        {
            Manager.AddReceipt(transaction.Invoice, transaction.Items);
        }


        public IEnumerable<InvoiceItem> GetInvoiceItems(Guid invoiceId)
        {
            return Manager.GetInvoiceItems(invoiceId);
        }
    }
}
