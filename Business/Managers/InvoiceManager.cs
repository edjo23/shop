using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Business.Entities;
using DapperExtensions;

namespace Business.Managers
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
            using (var connectionScope = new ConnectionScope())
            {
                return connectionScope.Connection.GetList<Invoice>();
            }
        }

        public void ProcessInvoice(Invoice invoice, IEnumerable<InvoiceItem> items)
        {
            using (var transactionScope = new TransactionScope())
            using (var connectionScope = new ConnectionScope())
            {
                // Update Invoice.
                connectionScope.Connection.Insert(invoice);

                foreach (var item in items)
                {
                    connectionScope.Connection.Insert(item);
                }

                // Update Inventory.
                foreach (var item in items)
                {
                    var product = connectionScope.Connection.Get<Product>(item.ProductId);
                    if (product == null)
                        throw new Exception("Product not found.");

                    // Stock Correction.
                    if (product.QuantityOnHand < item.Quantity)
                    {
                        var correction = new ProductMovement
                        {
                            Id = Guid.NewGuid(),
                            ProductId = item.ProductId,
                            DateTime = invoice.DateTime,
                            MovementType = ProductMovementType.Correction,
                            Quantity = item.Quantity - product.QuantityOnHand,
                            SourceId = invoice.Id,
                            SourceItemNumber = item.ItemNumber
                        };

                        connectionScope.Connection.Insert<ProductMovement>(correction);

                        product.QuantityOnHand += correction.Quantity;

                        connectionScope.Connection.Update(product);
                    }

                    // Movement.
                    var movement = new ProductMovement
                    {
                        Id = Guid.NewGuid(),
                        ProductId = item.ProductId,
                        DateTime = invoice.DateTime,
                        MovementType = ProductMovementType.Invoice,
                        Quantity = item.Quantity * -1,
                        SourceId = invoice.Id,
                        SourceItemNumber = item.ItemNumber
                    };

                    ProductManager.AddMovement(movement);
                }

                // Update Customer.
                var amount = 0.0m;

                foreach (var item in items)
                {
                    amount += item.Price * item.Quantity;
                }

                var transaction = new CustomerTransaction()
                {
                    Id = Guid.NewGuid(),
                    CustomerId = invoice.CustomerId,
                    DateTime = invoice.DateTime,
                    Type = CustomerTransactionType.Invoice,
                    Amount = amount,
                    SourceId = invoice.Id
                };

                CustomerManager.AddTransaction(transaction);

                transactionScope.Complete();
            }

        }
    }
}
