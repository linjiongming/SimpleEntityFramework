using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleEntityFramework.Domain.Roles.Schemas
{
    public interface ITableSchema
    {
        string Name { get; set; }
        string EntityName { get; }
        List<IColumnSchema> Columns { get; set; }
        List<IColumnSchema> PrimaryKeys { get; }
    }
}
