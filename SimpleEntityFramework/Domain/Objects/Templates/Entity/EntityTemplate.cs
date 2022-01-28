using SimpleEntityFramework.Domain.Objects.Schemas;
using SimpleEntityFramework.Domain.Roles.Schemas;
using SimpleEntityFramework.Domain.Roles.Templates;
using System;
using System.Linq;

namespace SimpleEntityFramework.Domain.Objects.Templates
{
    public class EntityTemplate : ClassTemplate
    {
        public override string Name => Table.EntityName;

        public EntityTemplate(IProjectTemplate project, ITableSchema table) : base(project)
        {
            Table = table;
        }

        public ITableSchema Table { get; set; }

        public override string FileContent => $@"{Profile}
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace {Namespace}
{{
    [Table(""{Table.Name}"")]
    public partial class {Table.EntityName} : {BaseEntityTemplate.ClassName}{(Table.PrimaryKeys.Count == 1 ? $"<{Table.PrimaryKeys[0].TypeName}>" : string.Empty)}
    {{        
        {string.Join("\r\n\r\n        ", Table.Columns.Select(col =>
        $@"[Column(""{col.Name}"")]
        [DataObjectField(primaryKey: {col.PrimaryKey.ToString().ToLower()}, isIdentity: {col.IsIdentity.ToString().ToLower()}, isNullable: {col.IsNullable.ToString().ToLower()}, length: {col.Length})]
        public {col.TypeName} {col.PropertyName} {{ get; set; }}"))}
    }}
}}";

    }
}
