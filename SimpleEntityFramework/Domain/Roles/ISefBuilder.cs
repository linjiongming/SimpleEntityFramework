using SimpleEntityFramework.Domain.Roles.Schemas;
using SimpleEntityFramework.Domain.Roles.Templates;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleEntityFramework.Domain.Roles
{
    public interface ISefBuilder
    {
        string ConnectionString { get; set; }
        DbProviderFactory ProviderFactory { get; set; }
        string NamespaceRoot { get; set; }
        string OutputFolder { get; set; }
        List<ITableSchema> TableSchemas { get; }
        List<IProjectTemplate> Projects { get; }
        void Build();
    }
}
