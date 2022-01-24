using Microsoft.Extensions.CommandLineUtils;
using SimpleEntityFramework.Domain.Objects;
using SimpleEntityFramework.Domain.Objects.Templates;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleEntityFramework
{
    class Program
    {
        static void Main(string[] args)
        {
            var cla = new CommandLineApplication();
            {
                cla.Name = "Simple Entity Framework Command Line Interface";
                cla.Description = "Automatically generate CRUD code similar to EF.";
                cla.HelpOption("-? | -h | --help");
            }

            var r = cla.Option("-r | --root <NamespaceRoot>", "Namespace Root (Default \"My\")", CommandOptionType.SingleValue);
            var c = cla.Option("-c | --conn <ConnectionString>", "Connection String", CommandOptionType.SingleValue);
            var p = cla.Option("-p | --provider <ProviderName>", "Database Provider (Default \"System.Data.SqlClient\")", CommandOptionType.SingleValue);
            var o = cla.Option("-o | --output <OutputFolder>", "Output Directory (Default \"[BaseDirectory]\\Output\")", CommandOptionType.SingleValue);

            cla.OnExecute(() =>
            {
                var builder = new SefBuilder();
                {
                    builder.NamespaceRoot = r.Value() ?? "My";
                    builder.Database = new Database()
                    {
                        ProviderName = p.Value() ?? "System.Data.SqlClient",
                        ConnectionString = c.Value()
                    };
                    builder.OutputFolder = o.Value() ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Output");
                    builder.Build();
                }
                return 0;
            });

            cla.Execute(args);
        }
    }
}
