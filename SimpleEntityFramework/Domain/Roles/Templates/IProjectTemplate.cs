using SimpleEntityFramework.Domain.Objects.Templates;
using System;
using System.Collections.Generic;

namespace SimpleEntityFramework.Domain.Roles.Templates
{
    public interface IProjectTemplate : ITemplate
    {
        Guid ID { get; }
        string Name { get; set; }
        List<string> RefDlls { get; set; }
        List<ITemplate> CompileItems { get; set; }
        List<IProjectTemplate> RefProjects { get; set; }
        IProjectTemplate AddRefDlls(params string[] referenceItems);
        IProjectTemplate AddClasses(params IClassTemplate[] compileItems);
        IProjectTemplate AddRefProjets(params IProjectTemplate[] projects);
    }
}
