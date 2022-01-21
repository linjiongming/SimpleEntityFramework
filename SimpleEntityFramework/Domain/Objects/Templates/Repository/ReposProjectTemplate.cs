using SimpleEntityFramework.Domain.Objects.Schemas;
using SimpleEntityFramework.Domain.Roles.Templates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleEntityFramework.Domain.Objects.Templates
{
    public class ReposProjectTemplate : ProjectTemplate
    {
        public const string ProjectName = "Repository";

        public ReposProjectTemplate() : base(ProjectName)
        {
            this.AddClasses(new BaseReposTemplate())
                .AddClasses(Generator.Entities.Select(x => new ReposTemplate(x)).ToArray())
                .AddRefDlls(DefaultRefDlls)
                .AddRefDlls(
                    "System.Configuration",
                    "System.Transactions");
        }
    }
}
