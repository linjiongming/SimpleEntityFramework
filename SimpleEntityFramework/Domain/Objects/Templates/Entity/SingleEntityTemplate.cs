using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleEntityFramework.Domain.Roles;
using SimpleEntityFramework.Domain.Roles.Schemas;

namespace SimpleEntityFramework.Domain.Objects.Templates.Entity
{
    public class SingleEntityTemplate : BaseTemplate
    {
        public SingleEntityTemplate(ISefBuilder builder, IEntitySchema entity) : base(builder)
        {
            Entity = entity;
        }

        public IEntitySchema Entity { get; set; }

        public override string Namespace => $"{Builder.NamespaceRoot}.Entity";

        public override string FileName => $"{Entity.Name}.cs";

        public override string FileContent => $@"{Profile}
using System;
using System.ComponentModel;

namespace {Namespace}
{{
    [DataObject]
    public partial class {Entity.Name} : {BaseEntityTemplate.ClassName}{(Entity.PrimaryKeys.Count == 1 ? $"<{Entity.PrimaryKeys[0].TypeName}>" : string.Empty)}
    {{        
        {string.Join("\r\n\r\n        ", Entity.Properties.Select(x =>
        $@"[DataObjectField(primaryKey: {x.PrimaryKey.ToString().ToLower()}, isIdentity: {x.IsIdentity.ToString().ToLower()}, isNullable: {x.IsNullable.ToString().ToLower()}, length: {x.Length})]
        public {x.TypeName} {x.Name} {{ get; set; }}"))}
    }}
}}";
    }
}
