using SimpleEntityFramework.Domain.Objects.Schemas;
using SimpleEntityFramework.Domain.Objects.Templates.Framework;
using SimpleEntityFramework.Domain.Roles;
using SimpleEntityFramework.Domain.Roles.Templates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleEntityFramework.Domain.Objects.Templates
{
    public class FrameworkProjectTemplate : ProjectTemplate
    {
        public const string ProjectName = "Framework";

        public override string Name => ProjectName;

        public FrameworkProjectTemplate(ISefBuilder builder) : base(builder)
        {
            RefDlls.AddRange(DefaultRefDlls);
            RefDlls.Add("System.Configuration");
            RefDlls.Add("System.Transactions");
            RefDlls.Add("System.Web.Extensions");
            CompileItems.Add(new DatabaseTemplate(this));
            CompileItems.Add(new EntityMapperTemplate(this));
            CompileItems.Add(new ExpressionVisitorTemplate(this));
            CompileItems.Add(new HttpHelperTemplate(this));
            CompileItems.Add(new JsonHelperTemplate(this));
            CompileItems.Add(new LoggerTemplate(this));
            CompileItems.Add(new RuntimeExtensionsTemplate(this));
        }
    }
}
