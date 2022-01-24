﻿using SimpleEntityFramework.Domain.Roles.Schemas;
using SimpleEntityFramework.Domain.Roles.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleEntityFramework.Domain.Roles
{
    public interface ISefBuilder
    {
        string NamespaceRoot { get; set; }
        string OutputFolder { get; set; }
        Database Database { get; set; }
        List<string> Tables { get; }
        List<IEntitySchema> Entities { get; }
        List<IProjectTemplate> Projects { get; }
        void Build();
    }
}