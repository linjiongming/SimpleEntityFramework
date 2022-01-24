using System;
using SimpleEntityFramework.Domain.Roles.Templates;

namespace SimpleEntityFramework.Domain.Objects.Templates
{
    public class EntityInterfaceTemplate : ClassTemplate
    {
        public EntityInterfaceTemplate(IProjectTemplate project) : base(project)
        {
        }

        public override string Name => "IEntity";

        public override string FileContent => $@"{Profile}
using System;

namespace {Namespace}
{{
    public interface {Name}
    {{
    }}
}}";
    }
}
