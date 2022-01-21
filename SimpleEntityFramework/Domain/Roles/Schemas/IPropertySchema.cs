using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleEntityFramework.Domain.Roles.Schemas
{
    public interface IPropertySchema
    {
        string Name { get; set; }
        Type DataType { get; set; }
        bool IsNullable { get; set; }
        bool PrimaryKey { get; set; }
        bool IsIdentity { get; set; }
        int Length { get; set; }
        string TypeName { get; }
    }
}
