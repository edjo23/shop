using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using DapperExtensions;

namespace Shop.Business.Database
{
    public static class Extensions
    {
        public static IEnumerable<T> SelectAll<T>()
             where T : class
        {
            using (var connectionScope = new ConnectionScope())
            {
                return connectionScope.Connection.GetList<T>();
            }
        }

        public static T SelectById<T>(Guid id)
             where T : class
        {
            using (var connectionScope = new ConnectionScope())
            {
                return connectionScope.Connection.Get<T>(id);
            }
        }

        public static void Insert<T>(this T entity)
             where T : class
        {
            using (var transaction = new TransactionScope())
            using (var connection = new ConnectionScope())
            {
                connection.Connection.Insert(entity);
                transaction.Complete();
            }
        }

        public static void Insert<T>(this IEnumerable<T> entities)
             where T : class
        {
            using (var transaction = new TransactionScope())
            using (var connection = new ConnectionScope())
            {
                foreach(var entity in entities)
                    connection.Connection.Insert(entity);

                transaction.Complete();
            }
        }   

        public static void Update<T>(this T entity)
             where T : class
        {
            using (var transaction = new TransactionScope())
            using (var connection = new ConnectionScope())
            {
                connection.Connection.Update(entity);
                transaction.Complete();
            }
        }
    }
}
