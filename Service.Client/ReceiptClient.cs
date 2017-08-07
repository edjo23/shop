using Shop.Contracts.Entities;
using Shop.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Service.Client
{
    public class ReceiptClient : HttpClient, IReceiptService
    {
        public ReceiptClient(IUrlProvider urlProvider)
        {
            BaseUrl = urlProvider.GetBaseUrl();
        }

        private readonly string BaseUrl = "http://localhost:5000/api";

        public void AddReceipt(InvoiceTransaction transaction)
        {
            Post($"{BaseUrl}/receipt", transaction);
        }
    }
}
