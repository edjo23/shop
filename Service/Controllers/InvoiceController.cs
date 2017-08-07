using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Server.Controllers
{
    [Route("api/[controller]")]
    public class InvoiceController : Controller
    {
        public InvoiceController(ILoggerFactory loggerFactory, IInvoiceService invoiceService)
        {
            Logger = loggerFactory.CreateLogger("CustomerController");
            InvoiceService = invoiceService;
        }

        private readonly ILogger Logger;
        private readonly IInvoiceService InvoiceService;

        [HttpPost]
        public void AddInvoice([FromBody] InvoiceTransaction transaction)
        {
            InvoiceService.AddInvoice(transaction);
        }

        [HttpGet("{invoiceId:Guid}/item")]
        public IEnumerable<InvoiceItem> GetInvoiceItems(Guid invoiceId)
        {
            return InvoiceService.GetInvoiceItems(invoiceId);
        }

    }
}
