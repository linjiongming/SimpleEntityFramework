using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleEntityFramework.Infrastracture
{
    public class DbProviderMapping
    {
        private static readonly Dictionary<string, Lazy<DbProviderFactory>> _factories = new Dictionary<string, Lazy<DbProviderFactory>>()
        {
            ["Sql"] = new Lazy<DbProviderFactory>(() => System.Data.SqlClient.SqlClientFactory.Instance, true),
            ["OleDb"] = new Lazy<DbProviderFactory>(() => System.Data.OleDb.OleDbFactory.Instance, true),
            ["MySql"] = new Lazy<DbProviderFactory>(() => new MySql.Data.MySqlClient.MySqlClientFactory(), true),
            ["Oracle"] = new Lazy<DbProviderFactory>(() => new Oracle.ManagedDataAccess.Client.OracleClientFactory(), true),
            ["SQLite"] = new Lazy<DbProviderFactory>(() => new System.Data.SQLite.SQLiteFactory(), true),
        };

        private static string _description;
        public static string Description => _description ?? (_description = $"\r\n\tDatabase Providers:{string.Concat(_factories.Select(x => $"\r\n\t\t{x.Key}:\t{x.Value.Value.GetType().Namespace}"))}");

        public static DbProviderFactory GetFactory(string key)
        {
            key = _factories.Keys.FirstOrDefault(x => x.Equals(key, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(key))
            {
                return _factories[key].Value;
            }
            return null;
        }
    }
}
