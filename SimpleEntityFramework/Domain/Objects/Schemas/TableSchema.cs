using SimpleEntityFramework.Domain.Roles.Schemas;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleEntityFramework.Domain.Objects.Schemas
{
    public class TableSchema : ITableSchema
    {
        public TableSchema(string name)
        {
            Name = name;
            Columns = new List<IColumnSchema>();
        }

        public string Name { get; set; }

        public List<IColumnSchema> Columns { get; set; }

        public List<IColumnSchema> PrimaryKeys => Columns.FindAll(x => x.PrimaryKey);

        private string _entityName;
        public string EntityName => _entityName ?? (_entityName = GetEntityName(Name));

        public static readonly string[] TablePrefixes = new[] { "t_", "tb_", "tbl_", "tbl" };

        public static string GetEntityName(string tableName)
        {
            var entityName = tableName;
            var prefix = TablePrefixes.FirstOrDefault(x => entityName.StartsWith(x, StringComparison.CurrentCultureIgnoreCase));
            if (!string.IsNullOrWhiteSpace(prefix) && entityName.StartsWith(prefix, StringComparison.CurrentCultureIgnoreCase))
            {
                entityName = entityName.Substring(prefix.Length);
            }
            return string.Concat(entityName.Split(new[] { '_', ' ', '-' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Substring(0, 1).ToUpper() + x.Substring(1)));
        }
    }
}
