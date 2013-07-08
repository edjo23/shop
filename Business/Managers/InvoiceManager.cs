using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using DapperExtensions;
using Shop.Business.Database;
using Shop.Contracts.Entities;

namespace Shop.Business.Managers
{
    public class InvoiceManager
    {
        public InvoiceManager(ProductManager productManager, CustomerManager customerManager)
        {
            ProductManager = productManager;
            CustomerManager = customerManager;
        }

        private ProductManager ProductManager { get; set; }

        private CustomerManager CustomerManager { get; set; }

        public IEnumerable<Invoice> GetInvoices()
        {
            return Extensions.SelectAll<Invoice>();
        }

        public void AddInvoice(Invoice invoice, IEnumerable<InvoiceItem> items, decimal payment)
        {

            using (var transaction = new TransactionScope())
            using (var connection = new ConnectionScope())
            {
                // Insert Invoice.
                invoice.Insert();

                foreach (var item in items)
                    item.InvoiceId = invoice.Id;

                items.Insert();

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
                    Amount = items.Aggregate(0.0m, (total, item) => total += item.Price * item.Quantity),
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
                        Amount = items.Aggregate(0.0m, (total, item) => total += item.Price * item.Quantity) * -1,
                        SourceId = invoice.Id
                    });
                }

                transaction.Complete();
            }

        }

        public IEnumerable<dynamic> GetInvoiceItemHistory(DateTimeOffset startDateTime)
        {
            using (var connection = new ConnectionScope())
            {
                var sql = @"
                    select 
	                    invoice.DateTime,
	                    customer.Name as CustomerName,
	                    product.Description as ProductDescription,
	                    item.Quantity,
	                    item.Price
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

                return connection.Connection.Query(sql, new { StartDateTime = startDateTime });
            }
        }
    }
}
