using SimpleEntityFramework.Domain.Objects.Templates;
using System;
using System.Collections.Generic;

namespace SimpleEntityFramework.Domain.Roles.Templates
{
    public interface IProjectTemplate : ITemplate
    {
        Guid ID { get; }
        string Name { get; }
        List<string> RefDlls { get; }
        List<ITemplate> CompileItems { get; }
        List<IProjectTemplate> RefProjects { get; }
    }
}
