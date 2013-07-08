using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.Business.Managers;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;

namespace Business.Services
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

        public IEnumerable<dynamic> GetInvoiceItemHistory(DateTimeOffset startDateTime)
        {
            return Manager.GetInvoiceItemHistory(startDateTime);
        }
    }
}
