using Microsoft.Extensions.CommandLineUtils;
using SimpleEntityFramework.Domain.Objects;
using SimpleEntityFramework.Infrastracture;
using System;
using System.Collections.Generic;
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
            var o = cla.Option("-o | --output <OutputFolder>", "Output Directory (Default \"[BaseDirectory]\\Output\")", CommandOptionType.SingleValue);
            var c = cla.Option("-c | --connection <ConnectionString>", "Connection String", CommandOptionType.SingleValue);
            var p = cla.Option("-p | --provider <ProviderName>", $"Database Provider (Default \"Sql\"){DbProviderMapping.Description}", CommandOptionType.SingleValue);

            cla.OnExecute(() =>
            {
                var builder = new SefBuilder();
                {
                    builder.NamespaceRoot = r.Value();
                    builder.OutputFolder = o.Value();
                    builder.ConnectionString = c.Value();
                    builder.ProviderFactory = DbProviderMapping.GetFactory(p.Value());
                    builder.Build();
                }
                return 0;
            });

            cla.Execute(args);
        }
    }
}
