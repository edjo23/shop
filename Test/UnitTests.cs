using System;
using System.Collections.Generic;
using System.Linq;
using Business.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shop.Business.Managers;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;

namespace Test
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void AddCustomer()
        {
            ICustomerService customerService = new CustomerService(new CustomerManager());

            var customers = customerService.GetCustomers();

            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                Number = (customers.Count() + 1).ToString(),
                Name = "Test Customer " + (customers.Count() + 1).ToString(),
                Balance = Decimal.Zero
            };

            customerService.AddCustomer(customer);

            Assert.AreEqual<int>(customers.Count() + 1, customerService.GetCustomers().Count());
        }

        [TestMethod]
        public void AddProduct()
        {
            IProductService productService = new ProductService(new ProductManager());

            var products = productService.GetProducts();

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Code = (products.Count() + 1).ToString(),
                Group = "Test",
                Description = "Test Product " + (products.Count() + 1).ToString(),
                Price = 0.99m,
                QuantityOnHand = 0
            };

            productService.AddProduct(product);

            Assert.AreEqual<int>(products.Count() + 1, productService.GetProducts().Count());
        }

        [TestMethod]
        public void AddStockReceipt()
        {
            IProductService productService = new ProductService(new ProductManager());

            var product = productService.GetProducts().First();

            var movement = new ProductMovement
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                DateTime = DateTimeOffset.Now,
                MovementType = ProductMovementType.Receipt, // Stock Receipt
                Quantity = 10
            };

            productService.AddMovement(movement);

            Assert.AreEqual<int>(product.QuantityOnHand + movement.Quantity, productService.GetProduct(product.Id).QuantityOnHand);
        }

        [TestMethod]
        public void AddInvoice()
        {
            IProductService productService = new ProductService(new ProductManager());
            ICustomerService customerService = new CustomerService(new CustomerManager());
            IInvoiceService invoiceService =  new InvoiceService(new InvoiceManager(new ProductManager(), new CustomerManager()));

            var products = productService.GetProducts();
            var customer = customerService.GetCustomers().First();

            var invoice = new Invoice
            {
                Id = Guid.NewGuid(),
                DateTime = DateTimeOffset.Now,
                CustomerId = customer.Id
            };

            var items = products.Select((product, i) =>
                new InvoiceItem
                {
                    Id = Guid.NewGuid(),
                    InvoiceId = invoice.Id,
                    ItemNumber = i + 1,
                    ProductId = product.Id,
                    Price = product.Price,
                    Quantity = 1
                });
            
            invoiceService.AddInvoice(invoice, items);

            // Check products balances.
            foreach (var product in products)
            {
                Assert.AreEqual<int>(product.QuantityOnHand < 1 ? 0 : product.QuantityOnHand - 1, productService.GetProduct(product.Id).QuantityOnHand);
            }
            
            // Check customer balance.
            Assert.AreEqual<decimal>(customer.Balance + products.Sum(o => o.Price), customerService.GetCustomer(customer.Id).Balance);
        }

        [TestMethod]
        public void AddInvoiceWithQuantities()
        {
            IProductService productService = new ProductService(new ProductManager());
            ICustomerService customerService = new CustomerService(new CustomerManager());
            IInvoiceService invoiceService = new InvoiceService(new InvoiceManager(new ProductManager(), new CustomerManager()));

            var customer = customerService.GetCustomers().First();
            var product = productService.GetProducts().First();

            var invoice = new Invoice
            {
                Id = Guid.NewGuid(),
                DateTime = DateTimeOffset.Now,
                CustomerId = customer.Id
            };

            var items = new List<InvoiceItem>
            {
                new InvoiceItem
                {
                    Id = Guid.NewGuid(),
                    InvoiceId = invoice.Id,
                    ItemNumber = 1,
                    ProductId = product.Id,
                    Price = product.Price,
                    Quantity = 3
                }
            };

            invoiceService.AddInvoice(invoice, items);

            // Check products balances.
            Assert.AreEqual<int>(product.QuantityOnHand < 3 ? 0 : product.QuantityOnHand - 3, productService.GetProduct(product.Id).QuantityOnHand);

            // Check customer balance.
            Assert.AreEqual<decimal>(customer.Balance + product.Price * 3, customerService.GetCustomer(customer.Id).Balance);
        }

        [TestMethod]
        public void AddPayment()
        {
            ICustomerService customerService = new CustomerService(new CustomerManager());

            var customer = customerService.GetCustomers().First();

            var transaction = new CustomerTransaction
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.Id,
                DateTime = DateTimeOffset.Now,
                Type = CustomerTransactionType.Payment,
                Amount = customer.Balance * -1
            };

            customerService.AddTransaction(transaction);

            // Check customer balance.
            Assert.AreEqual<decimal>(0, customerService.GetCustomer(customer.Id).Balance);
        }

        [TestMethod]
        public void AddOverpayment()
        {
            ICustomerService customerService = new CustomerService(new CustomerManager());

            var customer = customerService.GetCustomers().First();

            var transaction = new CustomerTransaction
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.Id,
                DateTime = DateTimeOffset.Now,
                Type = CustomerTransactionType.Payment,
                Amount = customer.Balance * -2
            };

            customerService.AddTransaction(transaction);

            // Check customer balance.
            Assert.AreEqual<decimal>(customer.Balance * -1, customerService.GetCustomer(customer.Id).Balance);
        }

        [TestMethod]
        public void AddAdjustment()
        {
            ICustomerService customerService = new CustomerService(new CustomerManager());

            var customer = customerService.GetCustomers().First();

            var transaction = new CustomerTransaction
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.Id,
                DateTime = DateTimeOffset.Now,
                Type = CustomerTransactionType.Adjustment,
                Amount = -10.0m
            };

            customerService.AddTransaction(transaction);

            // Check customer balance.
            Assert.AreEqual<decimal>(customer.Balance - 10.0m, customerService.GetCustomer(customer.Id).Balance);
        }
    }
}
