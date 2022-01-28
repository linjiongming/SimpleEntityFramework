using SimpleEntityFramework.Domain.Roles.Templates;
using System;
using System.IO;

namespace SimpleEntityFramework.Domain.Objects.Templates
{
    public class AssemblyInfoTemplate : BaseTemplate
    {
        private readonly IProjectTemplate _project;

        public AssemblyInfoTemplate(IProjectTemplate project) : base(project.Builder)
        {
            _project = project;
        }

        public override string Namespace => $"{Builder.NamespaceRoot}.{_project.Name}";

        public override string FolderPath => Path.Combine(Builder.OutputFolder, Namespace, "Columns");

        public override string FileName => "AssemblyInfo.cs";

        public override string FileContent => $@"
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
[assembly: AssemblyTitle(""{Namespace}"")]
[assembly: AssemblyDescription("""")]
[assembly: AssemblyConfiguration("""")]
[assembly: AssemblyCompany("""")]
[assembly: AssemblyProduct(""{Namespace}"")]
[assembly: AssemblyCopyright(""Copyright ©  {DateTime.Today.Year}"")]
[assembly: AssemblyTrademark("""")]
[assembly: AssemblyCulture("""")]
[assembly: ComVisible(false)]
[assembly: Guid(""{_project.ID.ToString().ToLower()}"")]
[assembly: AssemblyVersion(""1.0.0.0"")]
[assembly: AssemblyFileVersion(""1.0.0.0"")]";

        public override void Generate()
        {
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }
            base.Generate();
        }
    }
}
