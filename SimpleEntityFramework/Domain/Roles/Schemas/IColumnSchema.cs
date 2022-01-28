using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleEntityFramework.Domain.Roles.Schemas
{
    public interface IColumnSchema
    {
        string Name { get; set; }
        string PropertyName { get; }
        Type DataType { get; set; }
        bool IsNullable { get; set; }
        bool PrimaryKey { get; set; }
        bool IsIdentity { get; set; }
        int Length { get; set; }
        string TypeName { get; }
    }
}
