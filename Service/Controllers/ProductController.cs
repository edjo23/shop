using Microsoft.AspNetCore.Mvc;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Server.Controllers
{
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        public ProductController(IProductService productService)
        {
            ProductService = productService;
        }

        private readonly IProductService ProductService;

        [HttpGet]
        public IEnumerable<Product> GetProducts()
        {
            return ProductService.GetProducts();
        }

        [HttpGet("{id:Guid}")]
        public Product GetProduct(Guid id)
        {
            return ProductService.GetProduct(id);
        }

        [HttpPost]
        public IActionResult AddCustomer([FromBody] Product product)
        {
            ProductService.AddProduct(product);

            return Ok();
        }

        [HttpPut]
        public IActionResult UpdateProduct([FromBody] Product product)
        {
            ProductService.UpdateProduct(product);

            return Ok();
        }

        [HttpGet("{productId:Guid}/movement")]
        public IEnumerable<ProductMovement> GetProductMovements(Guid productId)
        {
            return ProductService.GetProductMovements(productId);
        }

        [HttpPost("{productId:Guid}/movement")]
        public IActionResult AddMovement(Guid productId, [FromBody] ProductMovement movement)
        {
            if (movement == null || movement.ProductId != productId)
                return BadRequest();

            ProductService.AddMovement(movement);

            return Ok();
        }
    }
}
