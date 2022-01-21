using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleEntityFramework.Domain.Roles.Schemas
{
    public interface IEntitySchema
    {
        string Name { get; set; }
        List<IPropertySchema> Properties { get; set; }
        List<IPropertySchema> PrimaryKeys { get; }
    }
}
