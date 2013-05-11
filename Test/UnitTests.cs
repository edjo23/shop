using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shop.Business.Managers;
using Shop.Contracts.Entities;

namespace Test
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void AddCustomer()
        {
            var customerManager = new CustomerManager();

            var customers = customerManager.GetCustomers();

            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                Number = (customers.Count() + 1).ToString(),
                Name = "Test Customer " + (customers.Count() + 1).ToString(),
                Balance = Decimal.Zero
            };

            customerManager.AddCustomer(customer);

            Assert.AreEqual<int>(customers.Count() + 1, customerManager.GetCustomers().Count());
        }

        [TestMethod]
        public void AddProduct()
        {
            var productManager = new ProductManager();

            var products = productManager.GetProducts();

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Code = (products.Count() + 1).ToString(),
                Group = "Test",
                Description = "Test Product " + (products.Count() + 1).ToString(),
                Price = 0.99m,
                QuantityOnHand = 0
            };

            productManager.AddProduct(product);

            Assert.AreEqual<int>(products.Count() + 1, productManager.GetProducts().Count());
        }

        [TestMethod]
        public void AddStockReceipt()
        {
            var productManager = new ProductManager();

            var product = productManager.GetProducts().First();

            var movement = new ProductMovement
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                DateTime = DateTimeOffset.Now,
                MovementType = ProductMovementType.Receipt, // Stock Receipt
                Quantity = 10
            };

            productManager.AddMovement(movement);

            Assert.AreEqual<int>(product.QuantityOnHand + movement.Quantity, productManager.GetProduct(product.Id).QuantityOnHand);
        }

        [TestMethod]
        public void AddInvoice()
        {
            var productManager = new ProductManager();
            var customerManager = new CustomerManager();
            var invoiceManager = new InvoiceManager(productManager, customerManager);

            var products = productManager.GetProducts();
            var customer = customerManager.GetCustomers().First();

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
            
            invoiceManager.AddInvoice(invoice, items);

            // Check products balances.
            foreach (var product in products)
            {
                Assert.AreEqual<int>(product.QuantityOnHand < 1 ? 0 : product.QuantityOnHand - 1, productManager.GetProduct(product.Id).QuantityOnHand);
            }
            
            // Check customer balance.
            Assert.AreEqual<decimal>(customer.Balance + products.Sum(o => o.Price), customerManager.GetCustomer(customer.Id).Balance);
        }

        [TestMethod]
        public void AddInvoiceWithQuantities()
        {
            var productManager = new ProductManager();
            var customerManager = new CustomerManager();
            var invoiceManager = new InvoiceManager(productManager, customerManager);

            var customer = customerManager.GetCustomers().First();
            var product = productManager.GetProducts().First();

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

            invoiceManager.AddInvoice(invoice, items);

            // Check products balances.
            Assert.AreEqual<int>(product.QuantityOnHand < 3 ? 0 : product.QuantityOnHand - 3, productManager.GetProduct(product.Id).QuantityOnHand);

            // Check customer balance.
            Assert.AreEqual<decimal>(customer.Balance + product.Price * 3, customerManager.GetCustomer(customer.Id).Balance);
        }

        [TestMethod]
        public void AddPayment()
        {
            var customerManager = new CustomerManager();

            var customer = customerManager.GetCustomers().First();

            var transaction = new CustomerTransaction
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.Id,
                DateTime = DateTimeOffset.Now,
                Type = CustomerTransactionType.Payment,
                Amount = customer.Balance * -1
            };

            customerManager.AddTransaction(transaction);

            // Check customer balance.
            Assert.AreEqual<decimal>(0, customerManager.GetCustomer(customer.Id).Balance);
        }

        [TestMethod]
        public void AddOverpayment()
        {
            var customerManager = new CustomerManager();

            var customer = customerManager.GetCustomers().First();

            var transaction = new CustomerTransaction
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.Id,
                DateTime = DateTimeOffset.Now,
                Type = CustomerTransactionType.Payment,
                Amount = customer.Balance * -2
            };

            customerManager.AddTransaction(transaction);

            // Check customer balance.
            Assert.AreEqual<decimal>(customer.Balance * -1, customerManager.GetCustomer(customer.Id).Balance);
        }

        [TestMethod]
        public void AddAdjustment()
        {
            var customerManager = new CustomerManager();

            var customer = customerManager.GetCustomers().First();

            var transaction = new CustomerTransaction
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.Id,
                DateTime = DateTimeOffset.Now,
                Type = CustomerTransactionType.Adjustment,
                Amount = -10.0m
            };

            customerManager.AddTransaction(transaction);

            // Check customer balance.
            Assert.AreEqual<decimal>(customer.Balance - 10.0m, customerManager.GetCustomer(customer.Id).Balance);
        }
    }
}
