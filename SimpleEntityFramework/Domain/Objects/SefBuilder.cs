using SimpleEntityFramework.Domain.Objects.Schemas;
using SimpleEntityFramework.Domain.Objects.Templates;
using SimpleEntityFramework.Domain.Objects.Templates.Entity;
using SimpleEntityFramework.Domain.Roles;
using SimpleEntityFramework.Domain.Roles.Schemas;
using SimpleEntityFramework.Domain.Roles.Templates;
using SimpleEntityFramework.Infrastracture;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace SimpleEntityFramework.Domain.Objects
{
    public class SefBuilder : ISefBuilder
    {
        public string ConnectionString { get; set; }
        public DbProviderFactory ProviderFactory { get; set; }
        public string NamespaceRoot { get; set; }
        public string OutputFolder { get; set; }
        public string SingleTable { get; set; }
        public List<ITableSchema> TableSchemas { get; }
        public List<IProjectTemplate> Projects { get; }

        public SefBuilder()
        {
            TableSchemas = new List<ITableSchema>();
            Projects = new List<IProjectTemplate>();
        }

        private void CheckInput()
        {
            if (string.IsNullOrWhiteSpace(ConnectionString))
            {
                throw new ArgumentNullException(nameof(ConnectionString));
            }
            if (ProviderFactory == null)
            {
                ProviderFactory = System.Data.SqlClient.SqlClientFactory.Instance;
            }
            if (string.IsNullOrWhiteSpace(NamespaceRoot))
            {
                NamespaceRoot = "My";
            }
            if (string.IsNullOrWhiteSpace(OutputFolder))
            {
                OutputFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output");
            }
            if (!Directory.Exists(OutputFolder))
            {
                Directory.CreateDirectory(OutputFolder);
            }
        }

        private void LoadProjects()
        {
            var frameworkProject = new FrameworkProjectTemplate(this);
            var entityProject = new EntityProjectTemplate(this);
            {
                entityProject.RefProjects.Add(frameworkProject);
            }
            var reposProject = new ReposProjectTemplate(this);
            {
                reposProject.RefProjects.Add(frameworkProject);
                reposProject.RefProjects.Add(entityProject);
            }
            Projects.Add(frameworkProject);
            Projects.Add(entityProject);
            Projects.Add(reposProject);
        }

        public void Build()
        {
            CheckInput();

            using (var schemaBuilder = new SchemaBuilder(ProviderFactory, ConnectionString))
            {
                if (!string.IsNullOrWhiteSpace(SingleTable))
                {
                    var tableSchema = schemaBuilder.GetTableSchema(SingleTable);
                    var template = new SingleEntityTemplate(this, tableSchema);
                    var content = template.FileContent;
                    Logger.Info(content);
                    Clipboard.SetText(content);
                    Logger.Info("All codes have been copied to clipboard.");
                    Console.ReadLine();
                }
                else
                {
                    TableSchemas.AddRange(schemaBuilder.GetAllTableSchemas());
                    LoadProjects();
                    Projects.ForEach(x => x.Generate());
                    Logger.Info("All codes have been generated successfully.");
                    System.Diagnostics.Process.Start("explorer", OutputFolder);
                }
            }
        }
    }
}
