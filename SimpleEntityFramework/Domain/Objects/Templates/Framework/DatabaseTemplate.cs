using SimpleEntityFramework.Domain.Roles.Templates;

namespace SimpleEntityFramework.Domain.Objects.Templates.Framework
{
    public class DatabaseTemplate : ClassTemplate
    {
        public const string ClassName = "Database";

        public DatabaseTemplate(IProjectTemplate project) : base(project)
        {
        }

        public override string Name => ClassName;
        
        public override string FileContent => $@"{Profile}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Transactions;

namespace {Namespace}
{{
    [SuppressMessage(""Security"", ""CA2100"")]
    public class Database
    {{
        public string ProviderName {{ get; set; }}
        public string ConnectionString {{ get; set; }}
        public Database() {{ }}
        public Database(string name)
        {{
            var settings = ConfigurationManager.ConnectionStrings[name];
            ProviderName = settings.ProviderName;
            ConnectionString = settings.ConnectionString;
        }}
        public static Database GetDefault()
        {{
            var defaultName = ConfigurationManager.AppSettings.Get(""DefaultConnection"");
            if (!string.IsNullOrWhiteSpace(defaultName))
            {{
                return new Database(defaultName);
            }}
            var settings = ConfigurationManager.ConnectionStrings[ConfigurationManager.ConnectionStrings.Count - 1];
            return new Database
            {{
                ProviderName = settings.ProviderName,
                ConnectionString = settings.ConnectionString
            }};
        }}
        public DbProviderFactory GetProviderFactory()
        {{
            return DbProviderFactories.GetFactory(ProviderName);
        }}
        public DbDataAdapter CreateDataAdapter()
        {{
            return GetProviderFactory().CreateDataAdapter();
        }}
        public string GetTableName(string tableName)
        {{
            tableName = tableName.TrimStart('[').TrimEnd(']').Trim('`');
            if (ProviderName.Contains(""MySql""))
            {{
                return $""`{{tableName}}`"";
            }}
            else
            {{
                return $""[{{tableName}}]"";
            }}
        }}
        public DbConnection CreateConnection()
        {{
            var factory = GetProviderFactory();
            var conn = factory.CreateConnection();
            conn.ConnectionString = ConnectionString;
            return conn;
        }}
        public DbConnection OpenConnection()
        {{
            var conn = CreateConnection();
            conn.Open();
            return conn;
        }}
        public DbCommand CreateCommand()
        {{
            var factory = GetProviderFactory();
            var dbCmd = factory.CreateCommand();
            return dbCmd;
        }}
        public DbParameter CreateParameter(string name, object value, DbType dbType, int size = 0, ParameterDirection direction = ParameterDirection.Input)
        {{
            var factory = GetProviderFactory();
            var para = factory.CreateParameter();
            para.Direction = direction;
            para.ParameterName = name;
            para.Value = value ?? DBNull.Value;
            para.DbType = dbType;
            para.Size = size;
            return para;
        }}
        public DbCommand GetSqlStringCommand(string sql, DbConnection conn = null)
        {{
            var dbCmd = CreateCommand();
            dbCmd.CommandType = CommandType.Text;
            dbCmd.CommandText = sql;
            dbCmd.Connection = conn;
            return dbCmd;
        }}
        public DbCommand GetSqlStringCommand(string sql, params DbParameter[] parameters)
        {{
            var dbCmd = CreateCommand();
            dbCmd.CommandText = sql;
            if (parameters != null && parameters.Length > 0)
            {{
                dbCmd.Parameters.AddRange(parameters);
            }}
            return dbCmd;
        }}
        public DbCommand GetStoredProcCommand(string procedureName, DbConnection conn = null)
        {{
            var dbCmd = CreateCommand();
            dbCmd.CommandType = CommandType.StoredProcedure;
            dbCmd.CommandText = procedureName;
            dbCmd.Connection = conn;
            return dbCmd;
        }}
        public DbParameter AddParameter(DbCommand dbCmd, string name, object value, DbType dbType, int size, ParameterDirection direction)
        {{
            var para = CreateParameter(name, value, dbType, size, direction);
            dbCmd.Parameters.Add(para);
            return para;
        }}
        public void AddInParameter(DbCommand dbCmd, string name, object value, DbType dbType, int size = 0)
        {{
            AddParameter(dbCmd, name, value, dbType, size, ParameterDirection.Input);
        }}
        public DbParameter AddOutParameter(DbCommand dbCmd, string name, DbType dbType, int size = 0)
        {{
            return AddParameter(dbCmd, name, null, dbType, size, ParameterDirection.Output);
        }}
        public int ExecuteNonQuery(DbCommand dbCmd, DbTransaction tran = null)
        {{
            using (new CommandWrapper(this, dbCmd, tran))
            {{
                return dbCmd.ExecuteNonQuery();
            }}
        }}
        public int ExecuteNonQuery(string sql, params DbParameter[] parameters)
        {{
            var dbCmd = GetSqlStringCommand(sql, parameters);
            return ExecuteNonQuery(dbCmd);
        }}
        public DbDataReader ExecuteReader(DbCommand dbCmd)
        {{
            using (new CommandWrapper(this, dbCmd))
            {{
                return dbCmd.ExecuteReader();
            }}
        }}
        public DbDataReader ExecuteReader(string sql, params DbParameter[] parameters)
        {{
            var dbCmd = GetSqlStringCommand(sql, parameters);
            return ExecuteReader(dbCmd);
        }}
        public DbDataReader ExecuteReader(DbCommand dbCmd, CommandBehavior behavior)
        {{
            using (new CommandWrapper(this, dbCmd))
            {{
                return dbCmd.ExecuteReader(behavior);
            }}
        }}
        public object ExecuteScalar(DbCommand dbCmd)
        {{
            using (new CommandWrapper(this, dbCmd))
            {{
                return dbCmd.ExecuteScalar();
            }}
        }}
        public object ExecuteScalar(string sql, params DbParameter[] parameters)
        {{
            var dbCmd = GetSqlStringCommand(sql, parameters);
            return ExecuteScalar(dbCmd);
        }}
        public T ExecuteScalar<T>(DbCommand dbCmd)
        {{
            var scalar = ExecuteScalar(dbCmd);
            if (scalar != null && scalar != DBNull.Value)
            {{
                return scalar is T equal ? equal : (T)Convert.ChangeType(scalar, Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T));
            }}
            return default(T);
        }}
        public T ExecuteScalar<T>(string sql, params DbParameter[] parameters)
        {{
            var dbCmd = GetSqlStringCommand(sql, parameters);
            return ExecuteScalar<T>(dbCmd);
        }}
        public DataSet ExecuteDataSet(DbCommand dbCmd)
        {{
            using (new CommandWrapper(this, dbCmd))
            {{
                var factory = GetProviderFactory();
                var dbAdapter = factory.CreateDataAdapter();
                var ds = new DataSet();
                dbAdapter.SelectCommand = dbCmd;
                dbAdapter.Fill(ds);
                return ds;
            }}
        }}
        public DataSet ExecuteDataSet(string sql, params DbParameter[] parameters)
        {{
            var dbCmd = GetSqlStringCommand(sql, parameters);
            return ExecuteDataSet(dbCmd);
        }}
        public DataTable ExecuteDataTable(DbCommand dbCmd, int tableIndex = 0)
        {{
            var ds = ExecuteDataSet(dbCmd);
            if (ds != null && ds.Tables != null && ds.Tables.Count > tableIndex)
            {{
                return ds.Tables[tableIndex];
            }}
            return null;
        }}
        public DataTable ExecuteDataTable(string sql, params DbParameter[] parameters)
        {{
            var dbCmd = GetSqlStringCommand(sql, parameters);
            return ExecuteDataTable(dbCmd);
        }}
        public List<T> ExecuteList<T>(DbCommand dbCmd, int tableIndex = 0)
        {{
            var dt = ExecuteDataTable(dbCmd, tableIndex);
            var type = typeof(T);
            if (type.IsValueType)
            {{
                return dt.AsEnumerable().Where(dr =>
                    dr[0] != null && dr[0] != DBNull.Value).Select(dr =>
                    dr[0] is T equal ? equal : (T)Convert.ChangeType(dr[0], Nullable.GetUnderlyingType(type) ?? type)).ToList();
            }}
            else
            {{
                return dt.MapToList<T>();
            }}
        }}
        public List<T> ExecuteList<T>(string sql, params DbParameter[] parameters)
        {{
            var dbCmd = GetSqlStringCommand(sql, parameters);
            return ExecuteList<T>(dbCmd);
        }}
        public DataTable ExecutePagedTable(DbCommand dbCmd, out int total, string orderBy, int pageNum = 1, int pageSize = 10)
        {{
            dbCmd.CommandText = $@""
SELECT *
     , ROW_NUMBER() OVER (ORDER BY {{orderBy}}) rn
  INTO #__TEMP
  FROM (
      {{dbCmd.CommandText}}
  ) __TEMP;

SELECT @__total = COUNT(1)
  FROM #__TEMP;

SELECT *
  FROM #__TEMP
 WHERE rn BETWEEN @__begin AND @__end;
"";
            AddInParameter(dbCmd, ""__begin"", (pageNum - 1) * pageSize + 1, DbType.Int32);
            AddInParameter(dbCmd, ""__end"", pageNum * pageSize, DbType.Int32);
            var paraTotal = AddOutParameter(dbCmd, ""__total"", DbType.Int32);
            var table = ExecuteDataTable(dbCmd);
            total = paraTotal.Value is int ttl ? ttl : 0;
            return table;
        }}
        public class CommandWrapper : IDisposable
        {{

            private readonly DbCommand _dbCmd;
            private readonly bool _needClose;
            public CommandWrapper(Database db, DbCommand dbCmd, DbTransaction tran = null)
            {{
                _dbCmd = dbCmd;
                if (tran != null)
                {{
                    _dbCmd.Transaction = tran;
                    _dbCmd.Connection = tran.Connection;
                }}
                else if (_dbCmd.Connection == null)
                {{
                    if (Transaction.Current != null)
                    {{
                        _dbCmd.Connection = TransactionScopeConnections.GetTransactionConnection(db);
                    }}
                    else
                    {{
                        _dbCmd.Connection = db.CreateConnection();
                        _needClose = true;
                    }}
                }}
                if (_dbCmd.Connection.State == ConnectionState.Closed)
                {{
                    _dbCmd.Connection.Open();
                }}
            }}
            protected virtual void Dispose(bool disposing)
            {{
                if (disposing)
                {{
                    if (_needClose)
                    {{
                        _dbCmd.Connection.Dispose();
                    }}
                    // ...
                }}
                // ...
            }}
            public void Dispose()
            {{
                Dispose(true);
                GC.SuppressFinalize(this);
            }}
        }}
        public class TransactionScopeConnections
        {{
            private static readonly Dictionary<string, DbConnection> _transactionConncetionPool = new Dictionary<string, DbConnection>();
            public static DbConnection GetTransactionConnection(Database db)
            {{
                if (Transaction.Current == null) return null;
                var id = Transaction.Current.TransactionInformation.LocalIdentifier;
                lock (_transactionConncetionPool)
                {{
                    if (!_transactionConncetionPool.ContainsKey(id) || _transactionConncetionPool[id] == null)
                    {{
                        _transactionConncetionPool[id] = db.CreateConnection();
                        Transaction.Current.TransactionCompleted += Current_TransactionCompleted;
                    }}
                    return _transactionConncetionPool[id];
                }}
            }}
            private static void Current_TransactionCompleted(object sender, TransactionEventArgs e)
            {{
                var id = e.Transaction.TransactionInformation.LocalIdentifier;
                lock (_transactionConncetionPool)
                {{
                    if (_transactionConncetionPool.ContainsKey(id))
                    {{
                        _transactionConncetionPool[id]?.Dispose();
                        _transactionConncetionPool.Remove(id);
                    }}
                }}
            }}
        }}
    }}
}}";
    }
}
