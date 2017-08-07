using Moq;
using Shop.Business.Database;
using Shop.Business.Managers;
using Shop.Business.Services;
using Shop.Contracts.Services;
using System;
using Microsoft.Extensions.Configuration;
using Xunit;
using Shop.Contracts.Entities;
using System.Linq;
using System.Collections.Generic;

namespace Core.UnitTest
{
    public class UnitTest
    {        
        [Fact]
        public void AddCustomer()
        {
            ICustomerService customerService = new CustomerService(new CustomerManager(GetConfiguration(), new ConnectionProvider(GetConfiguration())));

            var customers = customerService.GetCustomers();

            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                Number = (customers.Count() + 1).ToString(),
                Name = "Test Customer " + (customers.Count() + 1).ToString(),
                Balance = Decimal.Zero
            };

            // AddCustomer
            customerService.AddCustomer(customer);

            // GetCustomers
            var gets = customerService.GetCustomers();
            Assert.Equal(customers.Count() + 1, gets.Count());

            // GetCustomer
            var get = customerService.GetCustomer(customer.Id);
            Assert.Equal(customer.Id, get.Id);
            Assert.Equal(customer.Number, get.Number);
            Assert.Equal(customer.Name, get.Name);
            Assert.Equal(customer.Balance, get.Balance);

            // GetCustomerByNumber
            get = customerService.GetCustomerByNumber(customer.Number);
            Assert.Equal(customer.Id, get.Id);
            Assert.Equal(customer.Number, get.Number);
            Assert.Equal(customer.Name, get.Name);
            Assert.Equal(customer.Balance, get.Balance);

            // UpdateCustomer
            customer.Number += 1000;
            customerService.UpdateCustomer(customer);
            get = customerService.GetCustomer(customer.Id);
            Assert.Equal(customer.Id, get.Id);
            Assert.Equal(customer.Number, get.Number);
            Assert.Equal(customer.Name, get.Name);
            Assert.Equal(customer.Balance, get.Balance);
        }

        [Fact]
        public void AddProduct()
        {
            IProductService productService = new ProductService(new ProductManager(new ConnectionProvider(GetConfiguration())));

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

            // AddProduct
            productService.AddProduct(product);

            // GetProducts
            var gets = productService.GetProducts();
            Assert.Equal(products.Count() + 1, gets.Count());

            // GetProduct
            var get = productService.GetProduct(product.Id);
            Assert.Equal(product.Id, get.Id);
            Assert.Equal(product.Code, get.Code);
            Assert.Equal(product.Group, get.Group);
            Assert.Equal(product.Description, get.Description);
            Assert.Equal(product.Price, get.Price);
            Assert.Equal(product.QuantityOnHand, get.QuantityOnHand);

            // UpdateProduct
            product.Price = 1.99m;
            productService.UpdateProduct(product);
            get = productService.GetProduct(product.Id);
            Assert.Equal(product.Id, get.Id);
            Assert.Equal(product.Code, get.Code);
            Assert.Equal(product.Group, get.Group);
            Assert.Equal(product.Description, get.Description);
            Assert.Equal(product.Price, get.Price);
            Assert.Equal(product.QuantityOnHand, get.QuantityOnHand);
        }

        [Fact]
        public void AddStockReceipt()
        {
            IProductService productService = new ProductService(new ProductManager(new ConnectionProvider(GetConfiguration())));

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

            Assert.Equal(product.QuantityOnHand + movement.Quantity, productService.GetProduct(product.Id).QuantityOnHand);
        }

        [Fact]
        public void AddInvoice()
        {
            var connectionProvider = new ConnectionProvider(GetConfiguration());
            IProductService productService = new ProductService(new ProductManager(connectionProvider));
            ICustomerService customerService = new CustomerService(new CustomerManager(GetConfiguration(), connectionProvider));
            IInvoiceService invoiceService = new InvoiceService(new InvoiceManager(new ProductManager(connectionProvider), new CustomerManager(GetConfiguration(), connectionProvider), connectionProvider));

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

            invoiceService.AddInvoice(new InvoiceTransaction { Invoice = invoice, Items = items, Payment = 0.0m });

            // Check products balances.
            foreach (var product in products)
            {
                Assert.Equal(product.QuantityOnHand < 1 ? 0 : product.QuantityOnHand - 1, productService.GetProduct(product.Id).QuantityOnHand);
            }

            // Check customer balance.
            Assert.Equal(customer.Balance + products.Sum(o => o.Price), customerService.GetCustomer(customer.Id).Balance);
        }

        [Fact]
        public void AddInvoiceWithQuantities()
        {
            var connectionProvider = new ConnectionProvider(GetConfiguration());
            IProductService productService = new ProductService(new ProductManager(connectionProvider));
            ICustomerService customerService = new CustomerService(new CustomerManager(GetConfiguration(), connectionProvider));
            IInvoiceService invoiceService = new InvoiceService(new InvoiceManager(new ProductManager(connectionProvider), new CustomerManager(GetConfiguration(), connectionProvider), connectionProvider));

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

            invoiceService.AddInvoice(new InvoiceTransaction { Invoice = invoice, Items = items });

            // Check products balances.
            Assert.Equal(product.QuantityOnHand < 3 ? 0 : product.QuantityOnHand - 3, productService.GetProduct(product.Id).QuantityOnHand);

            // Check customer balance.
            Assert.Equal(customer.Balance + product.Price * 3, customerService.GetCustomer(customer.Id).Balance);
        }

        [Fact]
        public void AddPayment()
        {
            ICustomerService customerService = new CustomerService(new CustomerManager(GetConfiguration(), new ConnectionProvider(GetConfiguration())));

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
            Assert.Equal(0, customerService.GetCustomer(customer.Id).Balance);
        }

        [Fact]
        public void AddOverpayment()
        {
            ICustomerService customerService = new CustomerService(new CustomerManager(GetConfiguration(), new ConnectionProvider(GetConfiguration())));

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
            Assert.Equal(customer.Balance * -1, customerService.GetCustomer(customer.Id).Balance);
        }

        [Fact]
        public void AddAdjustment()
        {
            ICustomerService customerService = new CustomerService(new CustomerManager(GetConfiguration(), new ConnectionProvider(GetConfiguration())));

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
            Assert.Equal(customer.Balance - 10.0m, customerService.GetCustomer(customer.Id).Balance);
        }

        [Fact]
        public void AddStockPayment()
        {
            var connectionProvider = new ConnectionProvider(GetConfiguration());
            IProductService productService = new ProductService(new ProductManager(connectionProvider));
            ICustomerService customerService = new CustomerService(new CustomerManager(GetConfiguration(), connectionProvider));
            IReceiptService invoiceService = new InvoiceService(new InvoiceManager(new ProductManager(connectionProvider), new CustomerManager(GetConfiguration(), connectionProvider), connectionProvider));

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
                    Price = product.Cost,
                    Quantity = 1
                });

            invoiceService.AddReceipt(new InvoiceTransaction { Invoice = invoice, Items = items });

            // Check products balances.
            foreach (var product in products)
            {
                Assert.Equal(product.QuantityOnHand + 1, productService.GetProduct(product.Id).QuantityOnHand);
            }

            // Check customer balance.
            Assert.Equal(customer.Balance - products.Sum(o => o.Cost), customerService.GetCustomer(customer.Id).Balance);
        }

        private IConfiguration GetConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string> {
                { "DatabaseConnectionString", @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Shop;Integrated Security=True;Pooling=False" },
                { "PinSalt", "Ivory Tower" }
            });
            return configurationBuilder.Build();
        }
    }
}
