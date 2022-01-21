using SimpleEntityFramework.Domain.Roles.Schemas;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleEntityFramework.Domain.Objects.Schemas
{
    public class EntitySchema : IEntitySchema
    {
        public EntitySchema(string name)
        {
            Name = name;
            Properties = new List<IPropertySchema>();
        }

        public string Name { get; set; }

        public List<IPropertySchema> Properties { get; set; }

        public List<IPropertySchema> PrimaryKeys => Properties.FindAll(x => x.PrimaryKey);


        //public static string[] TablePrefixes = new[] { "t_", "tb_", "tbl_", "tbl" };

        //public static string GetEntityName(string tableName)
        //{
        //    var entityName = tableName;
        //    foreach (var prefix in TablePrefixes)
        //    {
        //        if (entityName.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
        //        {
        //            entityName = entityName.Substring(prefix.Length);
        //            break;
        //        }
        //    }
        //    return SnakeCaseToPascalCase(entityName);
        //}

        //private static string SnakeCaseToPascalCase(string snakeCase)
        //{
        //    return string.Concat(snakeCase.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Substring(0, 1).ToUpper() + x.Substring(1)));
        //}

    }
}
