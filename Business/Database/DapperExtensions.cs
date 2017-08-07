using Dapper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Shop.Business.Database
{
    // Dotnetcore does not support ambient transaction. To support transactions they have to be explicitly created and passed around, which is a PITA.
    public static class DapperExtensions
    {
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IList<PropertyInfo>> TypeProperties = new ConcurrentDictionary<RuntimeTypeHandle, IList<PropertyInfo>>();

        public static IEnumerable<T> GetAll<T>(this IDbConnection dbConnection, IDbTransaction transaction)
        {
            var type = typeof(T);
            var allProperties = GetProperties(type);

            var tableName = type.Name;
            var sbColumnList = new StringBuilder(null);

            for (var i = 0; i < allProperties.Count; i++)
            {
                (i == 0 ? sbColumnList : sbColumnList.Append(", ")).Append($"[{allProperties[i].Name}]");
            }

            var cmd = $"select {sbColumnList.ToString()} from {tableName}";
            return dbConnection.Query<T>(cmd, transaction: transaction);
        }

        public static T Get<T>(this IDbConnection dbConnection, dynamic id, IDbTransaction transaction)
        {
            var type = typeof(T);
            var allProperties = GetProperties(type);

            var tableName = type.Name;
            var sbColumnList = new StringBuilder(null);

            for (var i = 0; i < allProperties.Count; i++)
            {
                (i == 0 ? sbColumnList : sbColumnList.Append(", ")).Append($"[{allProperties[i].Name}]");
            }

            var cmd = $"select {sbColumnList.ToString()} from {tableName} where Id = @Id";
            return dbConnection.QueryFirst<T>(cmd, new { Id = id }, transaction);
        }

        public static void Insert<T>(this IDbConnection dbConnection, T entity, IDbTransaction transaction)
        {
            var type = typeof(T);
            var allProperties = GetProperties(type);

            var tableName = type.Name;
            var sbColumnList = new StringBuilder(null);
            var sbParameterList = new StringBuilder(null);

            for (var i = 0; i < allProperties.Count; i++)
            {
                (i == 0 ? sbColumnList : sbColumnList.Append(", ")).Append($"[{allProperties[i].Name}]");
                (i == 0 ? sbParameterList : sbParameterList.Append(", ")).Append($"@{allProperties[i].Name}");
            }

            var cmd = $"insert into {tableName} ({sbColumnList.ToString()}) values ({sbParameterList.ToString()})";
            dbConnection.Execute(cmd, entity, transaction);
        }

        public static T Update<T>(this IDbConnection dbConnection, T entity, IDbTransaction transaction)
        {
            var type = typeof(T);
            var allProperties = GetProperties(type);

            var tableName = type.Name;
            var sbColumnList = new StringBuilder(null);
            var sbParameterList = new StringBuilder(null);

            for (var i = 0; i < allProperties.Count; i++)
            {
                (i == 0 ? sbColumnList : sbColumnList.Append(", ")).Append($"[{allProperties[i].Name}]");
                (i == 0 ? sbParameterList : sbParameterList.Append(", ")).Append($"[{allProperties[i].Name}] = @{allProperties[i].Name}");
            }

            var cmd = $"update {tableName} set {sbParameterList.ToString()} where Id = @Id";
            dbConnection.Execute(cmd, entity, transaction);

            cmd = $"select {sbColumnList.ToString()} from {tableName} where Id = @Id";
            return dbConnection.QueryFirst<T>(cmd, entity, transaction);
        }

        public static bool Delete<T>(this IDbConnection dbConnection, T entity, IDbTransaction transaction)
        {
            var type = typeof(T);
            var tableName = type.Name;

            var cmd = $"delete {tableName} where Id = @Id";
            return dbConnection.Execute(cmd, entity, transaction) > 0;
        }

        private static IList<PropertyInfo> GetProperties(Type type)
        {
            if (TypeProperties.ContainsKey(type.TypeHandle))
                return TypeProperties[type.TypeHandle];

            var properties = type.GetProperties().Where(o => o.CanWrite).ToList();
            TypeProperties[type.TypeHandle] = properties;

            return properties;
        }
    }
}
