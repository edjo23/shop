using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
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

        public void AddInvoice(Invoice invoice, IEnumerable<InvoiceItem> items)
        {
            using (var transaction = new TransactionScope())
            using (var connection = new ConnectionScope())
            {
                // Insert Invoice.
                invoice.Insert();
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

                transaction.Complete();
            }

        }
    }
}
