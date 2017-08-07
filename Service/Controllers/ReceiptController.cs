using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;

namespace Core.Server.Controllers
{
    [Route("api/[controller]")]
    public class ReceiptController : Controller
    {
        public ReceiptController(ILoggerFactory loggerFactory, IReceiptService receiptService)
        {
            Logger = loggerFactory.CreateLogger("CustomerController");
            ReceiptService = receiptService;
        }

        private readonly ILogger Logger;
        private readonly IReceiptService ReceiptService;

        [HttpPost]
        public void AddReceipt([FromBody] InvoiceTransaction transaction)
        {
            ReceiptService.AddReceipt(transaction);
        }
    }
}
