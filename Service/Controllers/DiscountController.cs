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
    public class DiscountController : Controller
    {
        public DiscountController(ILoggerFactory loggerFactory, IDiscountService discountService)
        {
            Logger = loggerFactory.CreateLogger("CustomerController");
            DiscountService = discountService;
        }

        private readonly ILogger Logger;
        private readonly IDiscountService DiscountService;

        [HttpGet]
        public IEnumerable<Discount> GetDiscounts()
        {
            return DiscountService.GetDiscounts();
        }

        [HttpGet("{id:Guid}/model")]
        public DiscountModel GetDiscountModel(Guid id)
        {
            return DiscountService.GetDiscountModel(id);
        }

        [HttpPost("{id:Guid}/model")]
        public DiscountModel InsertDiscountModel([FromBody] DiscountModel entity)
        {
            return DiscountService.InsertDiscountModel(entity);
        }

        [HttpPut("{id:Guid}/model")]
        public DiscountModel UpdateDiscountModel([FromBody] DiscountModel entity)
        {
            return DiscountService.UpdateDiscountModel(entity);
        }

        [HttpGet("customer/{customerId:Guid}/product")]
        public IEnumerable<DiscountProduct> GetDiscountProductsByCustomerId(Guid customerId)
        {
            return DiscountService.GetDiscountProductsByCustomerId(customerId);
        }

    }
}
