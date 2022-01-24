using SimpleEntityFramework.Domain.Objects.Schemas;
using SimpleEntityFramework.Domain.Roles;
using SimpleEntityFramework.Domain.Roles.Templates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleEntityFramework.Domain.Objects.Templates
{
    public class ReposProjectTemplate : ProjectTemplate
    {
        public const string ProjectName = "Repository";

        public override string Name => ProjectName;

        public ReposProjectTemplate(ISefBuilder builder) : base(builder)
        {
            RefDlls.AddRange(DefaultRefDlls);
            RefDlls.Add("System.Configuration");
            RefDlls.Add("System.Transactions");
            CompileItems.Add(new BaseReposTemplate(this));
            CompileItems.AddRange(Builder.Entities.Select(x => new ReposTemplate(this, x)).ToArray());
        }
    }
}
