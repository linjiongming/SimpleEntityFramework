using SimpleEntityFramework.Domain.Objects.Schemas;
using SimpleEntityFramework.Domain.Roles.Schemas;
using System;
using System.Linq;

namespace SimpleEntityFramework.Domain.Objects.Templates
{
    public class EntityTemplate : ClassTemplate
    {
        public override string Name => Entity.Name;

        public EntityTemplate(IEntitySchema entity)
        {
            Entity = entity;
        }
        
        public IEntitySchema Entity { get; set; }
        
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
