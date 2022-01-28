using SimpleEntityFramework.Domain.Roles.Schemas;
using SimpleEntityFramework.Infrastracture;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SimpleEntityFramework.Domain.Objects.Schemas
{
    public class SchemaBuilder : ISchemaBuilder, IDisposable
    {
        private readonly DbProviderFactory _providerFactory;
        private readonly EscapeSymbol _escapeSymbol;
        private readonly DbDataAdapter _dataAdapter;
        private readonly DbConnection _connection;

        public SchemaBuilder(DbProviderFactory providerFactory, string connectionString)
        {
            _providerFactory = providerFactory;
            _escapeSymbol = EscapeSymbol.FromProviderFactory(_providerFactory);
            _dataAdapter = providerFactory.CreateDataAdapter();
            {
                _dataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            }
            _connection = providerFactory.CreateConnection();
            {
                _connection.ConnectionString = connectionString;
                _connection.Open();
            }
        }

        ~SchemaBuilder()
        {
            Dispose(false);
        }

        public IEnumerable<TableSchema> GetAllTableSchemas()
        {
            var dataTable = _connection.GetSchema("Tables");
            foreach (DataRow row in dataTable.Rows)
            {
                var tableName = row.Field<string>(2);
                var tableSchema = GetTableSchema(tableName);
                if (!tableSchema.Columns.Any(x => x.PrimaryKey))
                {
                    Logger.Error($"Cannot find any key in table {tableName}.");
                    continue;
                }
                if (tableSchema.Columns.Any(x => !Regex.Match(x.Name, @"\w[\w,\d,_]*").Success))
                {
                    Logger.Error($"Invalid column name in table {tableName}.");
                    continue;
                }
                yield return tableSchema;
            }
        }

        public TableSchema GetTableSchema(string tableName)
        {
            var escapeTableName = _escapeSymbol.Escape(tableName);
            Logger.Info($"The schema of table {escapeTableName} is loading...");
            var tableSchema = new TableSchema(tableName);
            var dataTable = new DataTable();
            var command = _providerFactory.CreateCommand();
            command.Connection = _connection;
            command.CommandText = $"SELECT * FROM {escapeTableName}";
            _dataAdapter.SelectCommand = command;
            _dataAdapter.FillSchema(dataTable, SchemaType.Source);
            foreach (DataColumn col in dataTable.Columns)
            {
                tableSchema.Columns.Add(new ColumnSchema
                {
                    Name = col.ColumnName,
                    DataType = col.DataType,
                    IsIdentity = col.AutoIncrement,
                    IsNullable = col.AllowDBNull,
                    Length = col.MaxLength,
                    PrimaryKey = dataTable.PrimaryKey != null && dataTable.PrimaryKey.Length > 0 ? dataTable.PrimaryKey.Contains(col) : (col.AutoIncrement || col.Unique),
                });
            }
            return tableSchema;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dataAdapter?.Dispose();
                _connection?.Dispose();
            }
            // extern disposing
        }

        public class EscapeSymbol
        {
            public string Left { get; set; }
            public string Right { get; set; }
            public EscapeSymbol(string left, string right = null)
            {
                Left = left;
                Right = right ?? left;
            }
            public static EscapeSymbol FromProviderFactory(DbProviderFactory providerFactory)
            {
                if (providerFactory.GetType().Name.Contains("MySql"))
                {
                    return new EscapeSymbol("`");
                }
                else
                {
                    return new EscapeSymbol("[", "]");
                }
            }
            public string Escape(string schemaName)
            {
                return Left + schemaName.TrimStart(Left.ToCharArray()).TrimEnd(Right.ToCharArray()) + Right;
            }
        }
    }
}
