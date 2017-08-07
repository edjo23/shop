using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Shop.Business.Database
{
    public interface IConnectionProvider
    {
        IConnection CreateConnection(bool beginTransaction = false);
    }

    public class ConnectionProvider : IConnectionProvider
{
        public ConnectionProvider(IConfiguration configuration)
        {
            ConnectionString = configuration.GetSection("DatabaseConnectionString").Value;
        }

        public string ConnectionString { get; private set; }

        private List<Connection> DbConnectionInstances = new List<Connection>();

        public IConnection CreateConnection(bool beginTransaction = false)
        {
            var connection = GetConnection() ?? new SqlConnection(ConnectionString);
            var transaction = GetTransaction();
            var canCommit = false;

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            if (beginTransaction && transaction == null)
            {
                transaction = connection.BeginTransaction();
                canCommit = true;
            }            

            var instance = new Connection(this, connection, transaction, canCommit);
            DbConnectionInstances.Add(instance);
            return instance;
        }

        public void Close(Connection instance)
        {
            DbConnectionInstances.Remove(instance);

            if (GetConnection() == null)
                instance.DbConnection.Dispose();
        }

        private IDbConnection GetConnection()
        {
            return DbConnectionInstances.Count > 0 ? DbConnectionInstances[0].DbConnection : null;
        }
        private IDbTransaction GetTransaction()
        {
            var owner = DbConnectionInstances.FirstOrDefault(o => o.DbTransaction != null);
            return owner != null ? owner.DbTransaction : null;
        }
    }

    public interface IConnection : IDisposable
    {
        IDbConnection DbConnection { get; }
        IDbTransaction DbTransaction { get; }
        void Commit();
    }

    public class Connection : IConnection
    {
        public Connection(ConnectionProvider parent, IDbConnection dbConnection, IDbTransaction dbTransaction, bool canCommit)
        {
            Parent = parent;
            DbConnection = dbConnection;
            DbTransaction = dbTransaction;
            CanCommit = canCommit;
        }

        private readonly ConnectionProvider Parent = null;
        private readonly bool CanCommit = false;

        public IDbConnection DbConnection { get; private set; }
        public IDbTransaction DbTransaction { get; private set; }

        public void Commit()
        {
            if (DbTransaction != null)
            {
                if (CanCommit)
                    DbTransaction.Commit();
            }
            else
            {
                throw new Exception("There is no transaction to commit");
            }
        }

        public void Dispose()
        {
            Parent.Close(this);
        }        
    }
}
