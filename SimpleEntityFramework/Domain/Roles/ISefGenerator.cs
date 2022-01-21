using SimpleEntityFramework.Domain.Roles.Schemas;
using SimpleEntityFramework.Domain.Roles.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleEntityFramework.Domain.Roles
{
    public interface ISefGenerator
    {
        string NamespaceRoot { get; set; }
        string OutputFolder { get; set; }
        Database Database { get; set; }
        string DatabaseName { get; set; }
        List<string> TableNames { get; set; }
        List<IEntitySchema> Entities { get; set; }
        List<IProjectTemplate> Projects { get; set; }
        ISefGenerator LoadEntities();
        ISefGenerator AddProjects(params IProjectTemplate[] projects);
        void Generate();
    }
}
