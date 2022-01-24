using SimpleEntityFramework.Domain.Roles.Templates;

namespace SimpleEntityFramework.Domain.Objects.Templates
{
    public class BaseEntityTemplate : ClassTemplate
    {
        public const string ClassName = "BaseEntity";

        public BaseEntityTemplate(IProjectTemplate project) : base(project)
        {
        }

        public override string Name => ClassName;
        
        public override string FileContent => $@"{Profile}
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace {Namespace}
{{
    public class {Name}: IEntity
    {{
        public virtual object[] Keys => GetType().GetProperties().Where(x => x.GetCustomAttribute<DataObjectFieldAttribute>().PrimaryKey).Select(x => x.GetValue(this)).ToArray();
    }}

    public class {Name}<TKey> : BaseEntity
    {{
        public virtual TKey Key => (TKey)Keys.FirstOrDefault();
    }}
}}";
    }
}
