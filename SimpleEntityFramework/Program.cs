using SimpleEntityFramework.Domain.Objects;
using SimpleEntityFramework.Domain.Objects.Templates;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleEntityFramework
{
    class Program
    {
        static void Main(string[] args)
        {
            var nr = ConfigurationManager.AppSettings.Get("NamespaceRoot") ?? "My";
            var db = Database.GetDefault();
            var generator = new SefGenerator(nr, db);
            {
                generator
                    .LoadEntities()
                    .AddProjects(new FrameworkProjectTemplate())
                    .AddProjects(new EntityProjectTemplate())
                    .AddProjects(new ReposProjectTemplate())
                    .Generate();
            }
            Console.ReadLine();
        }
    }
}
