using SimpleEntityFramework.Domain.Roles.Schemas;
using SimpleEntityFramework.Domain.Roles.Templates;
using System;
using System.Linq;

namespace SimpleEntityFramework.Domain.Objects.Templates
{
    public class ReposTemplate : ClassTemplate
    {
        public override string Name => $"{Entity.Name}Repository";
        
        public ReposTemplate(IProjectTemplate project, IEntitySchema entity) : base(project)
        {
            Entity = entity;
        }

        public IEntitySchema Entity { get; }

        public override string FileContent => $@"{Profile}
{string.Join(Environment.NewLine, Project.RefProjects.Select(x => $"using {x.Namespace};"))}
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;

namespace {Namespace}
{{
    public partial class {Name} : {BaseReposTemplate.ClassName}<{Entity.Name}>
    {{
        
    }}
}}";

    }
}
