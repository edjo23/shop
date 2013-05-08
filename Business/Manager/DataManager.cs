using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Entities;
using Dapper;
using DapperExtensions;

namespace Business.Manager
{
    public class DataManager
    {
        public IDbConnection NewConnection()
        {
            return new SqlConnection(@"Data Source=LOCALHOST\SQLEXPRESS;Initial Catalog=Shop;Integrated Security=True;Pooling=False");
        }

        public void AddProduct(IDbConnection connection, Product product)
        {
            connection.Insert<Product>(product);
        }

        public void UpdateProduct(IDbConnection connection, Product product)
        {
            connection.Update<Product>(product);
        }

        public void AddMovement(IDbConnection connection, ProductMovement movement)
        {
            connection.Insert<ProductMovement>(movement);
        }

        public Product GetProduct(IDbConnection connection, Guid id)
        {
            return connection.Get<Product>(id);
        }

        public void AddCustomer(IDbConnection connection, Customer customer)
        {
            connection.Insert<Customer>(customer);
        }

        public void AddInvoice(IDbConnection connection, Invoice invoice)
        {
            connection.Insert<Invoice>(invoice);
        }

        public void AddInvoiceItem(IDbConnection connection, InvoiceItem item)
        {
            connection.Insert<InvoiceItem>(item);
        }
    }
}
