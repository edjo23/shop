using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Shop.Business.Database;
using Shop.Contracts.Entities;

namespace Shop.Business.Managers
{
    public class InvoiceManager
    {
        public InvoiceManager(ProductManager productManager, CustomerManager customerManager, IConnectionProvider connectionProvider)
        {
            ProductManager = productManager;
            CustomerManager = customerManager;
            ConnectionProvider = connectionProvider;
        }

        private readonly IConnectionProvider ConnectionProvider;

        private readonly ProductManager ProductManager;

        private readonly CustomerManager CustomerManager;

        public IEnumerable<Invoice> GetInvoices()
        {
            using (var connection = ConnectionProvider.CreateConnection())
            {
                return connection.DbConnection.GetAll<Invoice>(connection.DbTransaction);
            }
        }

        public void AddInvoice(Invoice invoice, IEnumerable<InvoiceItem> items, decimal payment)
        {
            using (var connection = ConnectionProvider.CreateConnection(true))
            {
                // Insert Invoice.
                connection.DbConnection.Insert(invoice, connection.DbTransaction);

                foreach (var item in items)
                {
                    item.InvoiceId = invoice.Id;
                    connection.DbConnection.Insert(item, connection.DbTransaction);
                }

                // Update Inventory.
                foreach (var item in items)
                {
                    var product = ProductManager.GetProduct(item.ProductId);
                    if (product == null)
                        throw new Exception("Product not found.");

                    // Stock Correction?
                    if (product.QuantityOnHand < item.Quantity)
                    {
                        ProductManager.AddMovement(new ProductMovement
                        {
                            Id = Guid.NewGuid(),
                            ProductId = item.ProductId,
                            DateTime = invoice.DateTime,
                            MovementType = ProductMovementType.Correction,
                            Quantity = item.Quantity - product.QuantityOnHand,
                            SourceId = invoice.Id,
                            SourceItemNumber = item.ItemNumber
                        });
                    }

                    // Movement.
                    ProductManager.AddMovement(new ProductMovement
                    {
                        Id = Guid.NewGuid(),
                        ProductId = item.ProductId,
                        DateTime = invoice.DateTime,
                        MovementType = ProductMovementType.Invoice,
                        Quantity = item.Quantity * -1,
                        SourceId = invoice.Id,
                        SourceItemNumber = item.ItemNumber
                    });
                }

                // Add Customer Transaction.
                CustomerManager.AddTransaction(new CustomerTransaction()
                {
                    Id = Guid.NewGuid(),
                    CustomerId = invoice.CustomerId,
                    DateTime = invoice.DateTime,
                    Type = CustomerTransactionType.Invoice,
                    Amount = items.Aggregate(0.0m, (total, item) => total += item.Quantity * Math.Round(item.Price * (100 - item.Discount) / 100, 2, MidpointRounding.AwayFromZero)),
                    SourceId = invoice.Id
                });

                // Payment?
                if (payment > 0)
                {
                    CustomerManager.AddTransaction(new CustomerTransaction()
                    {
                        Id = Guid.NewGuid(),
                        CustomerId = invoice.CustomerId,
                        DateTime = invoice.DateTime,
                        Type = CustomerTransactionType.Payment,
                        Amount = payment * -1,
                        SourceId = invoice.Id
                    });
                }

                connection.Commit();
            }

        }

        public void AddReceipt(Invoice invoice, IEnumerable<InvoiceItem> items)
        {
            using (var connection = ConnectionProvider.CreateConnection(true))
            {
                if (invoice.Id == Guid.Empty)
                    invoice.Id = Guid.NewGuid();                

                // Update Inventory.
                foreach (var item in items)
                {
                    var product = ProductManager.GetProduct(item.ProductId);
                    if (product == null)
                        throw new Exception("Product not found.");

                    // Stock Correction?
                    // TODO.

                    // Movement.
                    ProductManager.AddMovement(new ProductMovement
                    {
                        Id = Guid.NewGuid(),
                        ProductId = item.ProductId,
                        DateTime = invoice.DateTime,
                        MovementType = ProductMovementType.Receipt,
                        Quantity = item.Quantity,
                        SourceId = invoice.Id,
                        SourceItemNumber = item.ItemNumber
                    });
                }

                // Add Customer Transaction.
                CustomerManager.AddTransaction(new CustomerTransaction()
                {
                    Id = Guid.NewGuid(),
                    CustomerId = invoice.CustomerId,
                    DateTime = invoice.DateTime,
                    Type = CustomerTransactionType.Payment,
                    Amount = items.Aggregate(0.0m, (total, item) => total += item.Quantity * item.Price) * -1.0m,
                    SourceId = invoice.Id
                });

                connection.Commit();
            }
        }

        public IEnumerable<InvoiceItem> GetInvoiceItems(Guid invoiceId)
        {
            using (var connection = ConnectionProvider.CreateConnection())
            {
                return connection.DbConnection.Query<InvoiceItem>(@"select * from InvoiceItem where InvoiceId = @InvoiceId", new { InvoiceId = invoiceId }, connection.DbTransaction).ToList();
            }
        }

        public IEnumerable<dynamic> GetInvoiceItemHistory(DateTimeOffset startDateTime)
        {
            using (var connection = ConnectionProvider.CreateConnection())
            {
                var sql = @"
                    select 
	                    invoice.DateTime,
	                    customer.Name as CustomerName,
	                    product.Description as ProductDescription,
	                    item.Quantity,
	                    item.Price,
                        item.Discount
                    from
	                    dbo.Invoice invoice
		                    join dbo.Customer customer on customer.Id = invoice.CustomerId
		                    join dbo.InvoiceItem item on item.InvoiceId = invoice.Id
		                    join dbo.Product product on product.Id = item.ProductId
                    where
                        invoice.DateTime >= @StartDateTime
                    order by
                        invoice.DateTime desc,
                        item.ItemNumber";

                return connection.DbConnection.Query(sql, new { StartDateTime = startDateTime }, connection.DbTransaction);
            }
        }
    }
}
