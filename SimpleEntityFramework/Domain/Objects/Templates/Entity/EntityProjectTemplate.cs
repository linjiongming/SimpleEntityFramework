using SimpleEntityFramework.Domain.Objects.Schemas;
using SimpleEntityFramework.Domain.Roles;
using SimpleEntityFramework.Domain.Roles.Templates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleEntityFramework.Domain.Objects.Templates
{
    public class EntityProjectTemplate : ProjectTemplate
    {
        public const string ProjectName = "Entity";

        public override string Name => ProjectName;

        public EntityProjectTemplate(ISefBuilder gear) : base(gear)
        {
            RefDlls.AddRange(DefaultRefDlls);
            CompileItems.Add(new EntityInterfaceTemplate(this));
            CompileItems.Add(new BaseEntityTemplate(this));
            CompileItems.AddRange(Builder.Entities.Select(x => new EntityTemplate(this, x)).ToArray());
        }
    }
}
