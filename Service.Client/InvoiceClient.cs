using Shop.Contracts.Entities;
using Shop.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Service.Client
{
    public class InvoiceClient : HttpClient, IInvoiceService
    {
        public InvoiceClient(IUrlProvider urlProvider)
        {
            BaseUrl = urlProvider.GetBaseUrl();
        }

        private readonly string BaseUrl = "http://localhost:5000/api";

        public void AddInvoice(InvoiceTransaction transaction)
        {
            Post($"{BaseUrl}/invoice", transaction);
        }

        public IEnumerable<InvoiceItem> GetInvoiceItems(Guid invoiceId)
        {
            return Get<List<InvoiceItem>>($"{BaseUrl}/invoice/{invoiceId.ToString()}/item");
        }
    }
}
