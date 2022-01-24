using SimpleEntityFramework.Domain.Roles;
using SimpleEntityFramework.Domain.Roles.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleEntityFramework.Domain.Objects.Templates
{
    public abstract class ClassTemplate : BaseTemplate, IClassTemplate
    {
        public ClassTemplate(IProjectTemplate project)
        {
            Project = project;
        }

        public IProjectTemplate Project { get; }

        public override ISefBuilder Builder { get => Project.Builder; set => Project.Builder = value; }

        public abstract string Name { get; }

        public override string Namespace => $"{Builder.NamespaceRoot}.{Project.Name}";

        public override string FileName => $"{Name}.cs";

        public abstract override string FileContent { get; }
    }
}
