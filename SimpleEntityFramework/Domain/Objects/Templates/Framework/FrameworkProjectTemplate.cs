using SimpleEntityFramework.Domain.Objects.Schemas;
using SimpleEntityFramework.Domain.Objects.Templates.Framework;
using SimpleEntityFramework.Domain.Roles.Templates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleEntityFramework.Domain.Objects.Templates
{
    public class FrameworkProjectTemplate : ProjectTemplate
    {
        public const string ProjectName = "Framework";

        public FrameworkProjectTemplate() : base(ProjectName)
        {
            this.AddClasses(
                    new DatabaseTemplate(),
                    new EntityMapperTemplate(),
                    new ExpressionVisitorTemplate(),
                    new HttpHelperTemplate(),
                    new JsonHelperTemplate(),
                    new LoggerTemplate(),
                    new RuntimeExtensionsTemplate())
                .AddRefDlls(DefaultRefDlls)
                .AddRefDlls(
                    "System.Configuration",
                    "System.Transactions",
                    "System.Web.Extensions");
        }
    }
}
