using SimpleEntityFramework.Domain.Roles;
using SimpleEntityFramework.Domain.Roles.Templates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleEntityFramework.Domain.Objects.Templates
{
    public abstract class ProjectTemplate : BaseTemplate, IProjectTemplate
    {
        public static readonly string[] DefaultRefDlls = new string[]
        {
            "Microsoft.CSharp",
            "System",
            "System.ComponentModel.DataAnnotations",
            "System.Core",
            "System.Data",
            "System.Data.DataSetExtensions",
            "System.Net.Http",
            "System.Xml",
            "System.Xml.Linq",
        };

        private readonly AssemblyInfoTemplate _assemblyInfo;

        public ProjectTemplate(ISefBuilder builder) : base(builder)
        {
            ID = Guid.NewGuid();
            _assemblyInfo = new AssemblyInfoTemplate(this);
        }

        public Guid ID { get; }

        public abstract string Name { get; }

        public List<string> RefDlls { get; } = new List<string>();

        public List<ITemplate> CompileItems { get; } = new List<ITemplate>();

        public List<IProjectTemplate> RefProjects { get; } = new List<IProjectTemplate>();

        public override string Namespace => $"{Builder.NamespaceRoot}.{Name}";

        public override string FileName => $"{Namespace}.csproj";

        public override string FileContent => $@"<?xml version=""1.0"" encoding=""utf-8""?>
<Project ToolsVersion=""15.0"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <Import Project=""$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props"" Condition=""Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')"" />
  <PropertyGroup>
    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
    <ProjectGuid>{{{ID.ToString().ToUpper()}}}</ProjectGuid>
    <OutputType>Library</OutputType>
    <AppDesignerFolder>Columns</AppDesignerFolder>
    <RootNamespace>{Namespace}</RootNamespace>
    <AssemblyName>{Namespace}</AssemblyName>
    <TargetFrameworkVersion>v4.5</TargetFrameworkVersion>
    <FileAlignment>512</FileAlignment>
    <Deterministic>true</Deterministic>
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' "">
    <DebugSymbols>true</DebugSymbols>
    <DebugType>full</DebugType>
    <Optimize>false</Optimize>
    <OutputPath>bin\Debug\</OutputPath>
    <DefineConstants>DEBUG;TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' "">
    <DebugType>pdbonly</DebugType>
    <Optimize>true</Optimize>
    <OutputPath>bin\Release\</OutputPath>
    <DefineConstants>TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <ItemGroup>{string.Concat(RefDlls.Select(x => $@"
    <Reference Include=""{x}"" />"))}
  </ItemGroup>
  <ItemGroup>{string.Concat(CompileItems.Select(x => $@"
    <Compile Include=""{x.FilePath.Remove(0, FolderPath.Length).TrimStart('\\')}"" />"))}
  </ItemGroup>
  <ItemGroup>{string.Concat(RefProjects.Select(x => $@"
    <ProjectReference Include=""..\{x.Namespace}\{x.FileName}"">
      <Project>{{{x.ID.ToString().ToUpper()}}}</Project>
      <Name>{x.Namespace}</Name>
    </ProjectReference>"))}
  </ItemGroup>
  <Import Project=""$(MSBuildToolsPath)\Microsoft.CSharp.targets"" />
</Project>";

        public override void Generate()
        {
            Sort();
            base.Generate();
            _assemblyInfo.Generate();
            CompileItems.ForEach(x => x.Generate());
        }

        private void Sort()
        {
            RefDlls.Sort();
            CompileItems.Sort((left, right) => string.Compare(left.FilePath, right.FilePath));
            RefProjects.Sort((left, right) => string.Compare(left.Name, right.Name));
        }
    }
}
