using Shop.Contracts.Entities;
using Shop.Service.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Core.IntegrationTest
{
    public class TestUrlProvider : IUrlProvider
    {
        public string GetBaseUrl()
        {
            return "http://localhost:5000/api";
        }
    }

    public class IntegrationTest
    {
        [Fact]
        public void Customer()
        {
            var customerService = new CustomerClient(new TestUrlProvider());

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

            // Pin
            customerService.UpdatePin(customer.Id, "1234");
            var pinOK = customerService.CheckPin(customer.Id, "1234");
            Assert.True(pinOK);
            var pinNotOk = customerService.CheckPin(customer.Id, "4321");
            Assert.False(pinNotOk);
        }

        [Fact]
        public void Product()
        {
            var productService = new ProductClient(new TestUrlProvider());

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
        public void StockReceipt()
        {
            var productService = new ProductClient(new TestUrlProvider());

            var product = productService.GetProducts().First();
            var movementCnt = productService.GetProductMovements(product.Id).Count();

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

            var actual = productService.GetProductMovements(product.Id);
            Assert.Equal(movementCnt + 1, actual.Count());
            Assert.All(actual, o => Assert.Equal(product.Id, o.ProductId));
        }

        [Fact]
        public void Invoice()
        {
            var productService = new ProductClient(new TestUrlProvider());
            var customerService = new CustomerClient(new TestUrlProvider());
            var invoiceService = new InvoiceClient(new TestUrlProvider());

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

            invoiceService.AddInvoice(new InvoiceTransaction { Invoice = invoice, Items = items, Payment = 0 });

            // Check products balances.
            foreach (var product in products)
            {
                Assert.Equal(product.QuantityOnHand < 1 ? 0 : product.QuantityOnHand - 1, productService.GetProduct(product.Id).QuantityOnHand);
            }

            // Check customer balance.
            Assert.Equal(customer.Balance + products.Sum(o => o.Price), customerService.GetCustomer(customer.Id).Balance);
        }

        [Fact]
        public void InvoiceWithQuantities()
        {
            var productService = new ProductClient(new TestUrlProvider());
            var customerService = new CustomerClient(new TestUrlProvider());
            var invoiceService = new InvoiceClient(new TestUrlProvider());

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

            invoiceService.AddInvoice(new InvoiceTransaction { Invoice = invoice, Items = items, Payment = 0 });

            // Check products balances.
            Assert.Equal(product.QuantityOnHand < 3 ? 0 : product.QuantityOnHand - 3, productService.GetProduct(product.Id).QuantityOnHand);

            // Check customer balance.
            Assert.Equal(customer.Balance + product.Price * 3, customerService.GetCustomer(customer.Id).Balance);
        }

        [Fact]
        public void StockPayment()
        {
            var productService = new ProductClient(new TestUrlProvider());
            var customerService = new CustomerClient(new TestUrlProvider());
            var receiptService = new ReceiptClient(new TestUrlProvider());

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

            receiptService.AddReceipt(new InvoiceTransaction { Invoice = invoice, Items = items });

            // Check products balances.
            foreach (var product in products)
            {
                Assert.Equal(product.QuantityOnHand + 1, productService.GetProduct(product.Id).QuantityOnHand);
            }

            // Check customer balance.
            Assert.Equal(customer.Balance - products.Sum(o => o.Cost), customerService.GetCustomer(customer.Id).Balance);
        }

        [Fact]
        public void CashPayment()
        {
            var customerService = new CustomerClient(new TestUrlProvider());

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
        public void CashOverpayment()
        {
            var customerService = new CustomerClient(new TestUrlProvider());

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
        public void CustomerTransactionAdjustment()
        {
            var customerService = new CustomerClient(new TestUrlProvider());

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
        public void Application()
        {
            var applicationService = new ApplicationClient(new TestUrlProvider());            
            var denominations = applicationService.GetDenominations();
            Assert.NotEmpty(denominations);
            var imageList = applicationService.GetImageList();
            Assert.Empty(imageList);
            var image = applicationService.GetImage("test");
            Assert.Empty(image);
            var textList = applicationService.GetText("test");
            Assert.Empty(textList);
        }

        [Fact]
        public void Discount()
        {
            var discountService = new DiscountClient(new TestUrlProvider());
            var productService = new ProductClient(new TestUrlProvider());
            var customerService = new CustomerClient(new TestUrlProvider());

            var productId = Guid.NewGuid();           
            var customerId = Guid.NewGuid();           

            productService.AddProduct(new Product
            {
                Id = productId,
                Code = productId.ToString(),
                Group = "Test",
                Description = productId.ToString(),
                Price = 100.0m
            });

            customerService.AddCustomer(new Customer
            {
                Id = customerId,
                Number = DateTime.Now.ToFileTime().ToString(),
                Name = customerId.ToString()
            });

            var discountId = Guid.NewGuid();

            var discount = new Discount
            {
                Id = discountId,
                Description = discountId.ToString()
            };

            var discountModel = new DiscountModel()
            {
                Discount = discount,
                Products = new List<DiscountProduct> { new DiscountProduct { Id = Guid.NewGuid(), ProductId = productId, Discount = 0.1m } },
                Customers = new List<DiscountCustomer> { new DiscountCustomer { Id = Guid.NewGuid(), CustomerId = customerId } }
            };

            // InsertDiscountModel
            var insertedDiscountModel = discountService.InsertDiscountModel(discountModel);
            DiscountDeepEquals(discountModel, insertedDiscountModel);

            // GetDiscounts
            var discounts = discountService.GetDiscounts();
            Assert.NotNull(discounts.FirstOrDefault(o => o.Id == discountId));

            // GetDiscount
            var actualDiscount = discountService.GetDiscountModel(discountId);
            DiscountDeepEquals(insertedDiscountModel, actualDiscount);

            // Update
            actualDiscount.Products.First().Discount = 0.2m;
            var updateDiscountModel = discountService.UpdateDiscountModel(actualDiscount);
            DiscountDeepEquals(actualDiscount, updateDiscountModel);

            // GetDiscountProductsByCustomerId
            var productsByCustomer = discountService.GetDiscountProductsByCustomerId(customerId);
            Assert.NotNull(productsByCustomer);
            Assert.Equal(1, productsByCustomer.Count());
            Assert.Equal(productId, productsByCustomer.First().ProductId);
            Assert.Equal(0.2m, productsByCustomer.First().Discount);
        }

        private void DiscountDeepEquals (DiscountModel expected, DiscountModel actual)
        {
            if (expected == null)
            {
                Assert.Null(actual);
                return;
            }

            if (expected.Discount == null)
                Assert.Null(actual.Discount);
            else
                Assert.NotNull(actual.Discount);

            Assert.Equal(expected.Discount.Id, actual.Discount.Id);
            Assert.Equal(expected.Discount.Description, actual.Discount.Description);

            if (expected.Products == null)
                Assert.Null(actual.Customers);
            else
                Assert.NotNull(actual.Discount);

            Assert.Equal(expected.Products.Count(), actual.Products.Count());
            for (int i = 0; i < expected.Products.Count(); i++)
            {
                var expectedItem = expected.Products.ToArray()[i];
                var actualItem = actual.Products.ToArray()[i];
                Assert.Equal(expectedItem.Id, actualItem.Id);
                Assert.Equal(expectedItem.ProductId, actualItem.ProductId);
                Assert.Equal(expectedItem.Discount, actualItem.Discount);
            }

            if (expected.Customers == null)
                Assert.Null(actual.Customers);
            else
                Assert.NotNull(actual.Discount);

            Assert.Equal(expected.Customers.Count(), actual.Customers.Count());
            for (int i = 0; i < expected.Customers.Count(); i++)
            {
                var expectedItem = expected.Customers.ToArray()[i];
                var actualItem = actual.Customers.ToArray()[i];
                Assert.Equal(expectedItem.Id, actualItem.Id);
                Assert.Equal(expectedItem.CustomerId, actualItem.CustomerId);
            }
        }
    }
}
