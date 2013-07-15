using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Business.Database
{
    public class ConnectionScope : IDisposable
    {
        [ThreadStatic]
        private static ConnectionScope Current = null;

        private IDbConnection _Connection;

        public IDbConnection Connection 
        {
            get
            {
                return Current._Connection;
            }
            set
            {
                Current._Connection = value;
            }
        }

        public ConnectionScope()
        {
            if (Current == null)
            {
                Current = this;

                var config = ConfigurationManager.ConnectionStrings["Shop"];

                Connection = new SqlConnection(config != null ? config.ConnectionString : @"Data Source=LOCALHOST\SQLEXPRESS;Initial Catalog=Shop;Integrated Security=True;Pooling=False");
                Connection.Open();
            }
        }

        public void Dispose()
        {
            if (Current == this)
            {
                Connection.Close();
                Connection = null;

                Current = null;
            }
        }
    }
}
