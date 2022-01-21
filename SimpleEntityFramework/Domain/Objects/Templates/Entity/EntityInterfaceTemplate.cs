using System;

namespace SimpleEntityFramework.Domain.Objects.Templates
{
    public class EntityInterfaceTemplate : ClassTemplate
    {
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
