using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleEntityFramework.Domain.Roles.Templates
{
    public interface IClassTemplate : ITemplate
    {
        string Name { get; }
        IProjectTemplate Project { get; }
    }
}
