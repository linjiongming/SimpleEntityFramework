using SimpleEntityFramework.Domain.Objects.Schemas;
using SimpleEntityFramework.Domain.Roles.Templates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleEntityFramework.Domain.Objects.Templates
{
    public class EntityProjectTemplate : ProjectTemplate
    {
        public const string ProjectName = "Entity";

        public EntityProjectTemplate() : base(ProjectName)
        {
            this.AddClasses(new EntityInterfaceTemplate(), new BaseEntityTemplate())
                .AddClasses(Generator.Entities.Select(x => new EntityTemplate(x)).ToArray())
                .AddRefDlls(DefaultRefDlls);
        }
    }
}
