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
        public string OnlyTable { get; set; }
        public List<string> TableNames { get; }
        public List<IEntitySchema> Entities { get; }
        public List<IProjectTemplate> Projects { get; }

        public SefBuilder()
        {
            TableNames = new List<string>();
            Entities = new List<IEntitySchema>();
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

        private void LoadSchemas()
        {
            using (var adapter = ProviderFactory.CreateDataAdapter())
            using (var conn = ProviderFactory.CreateConnection())
            {
                conn.ConnectionString = ConnectionString;
                conn.Open();
                TableNames.AddRange(conn.GetSchema("Tables").AsEnumerable().Select(dr => dr.Field<string>(2)).ToArray());
                adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                foreach (var tableName in TableNames)
                {
                    var escapTableName = EscapeTableName(tableName);
                    Logger.Info($"The schema of table {escapTableName} is loading...");
                    var entity = new EntitySchema(tableName);
                    var schemaTable = new DataTable();
                    var command = ProviderFactory.CreateCommand();
                    command.Connection = conn;
                    command.CommandText = $"SELECT * FROM {escapTableName}";
                    adapter.SelectCommand = command;
                    adapter.FillSchema(schemaTable, SchemaType.Source);
                    foreach (DataColumn col in schemaTable.Columns)
                    {
                        var prop = new PropertySchema
                        {
                            Name = col.ColumnName,
                            DataType = col.DataType,
                            IsIdentity = col.AutoIncrement,
                            IsNullable = col.AllowDBNull,
                            Length = col.MaxLength,
                            PrimaryKey = schemaTable.PrimaryKey != null && schemaTable.PrimaryKey.Length > 0 ? schemaTable.PrimaryKey.Contains(col) : (col.AutoIncrement || col.Unique),
                        };
                        entity.Properties.Add(prop);
                    }
                    if (!entity.Properties.Any(x => x.PrimaryKey))
                    {
                        Logger.Error($"Cannot find any key in table {escapTableName}.");
                        continue;
                    }
                    if (entity.Properties.Any(x => !Regex.Match(x.Name, @"\w[\w,\d,_]*").Success))
                    {
                        Logger.Error($"Invalid column name in table {escapTableName}.");
                        continue;
                    }
                    Entities.Add(entity);
                }
            }
        }

        private EntitySchema GetSingleEntity()
        {
            if (!string.IsNullOrWhiteSpace(OnlyTable))
            {
                using (var adapter = ProviderFactory.CreateDataAdapter())
                using (var conn = ProviderFactory.CreateConnection())
                {
                    conn.ConnectionString = ConnectionString;
                    conn.Open();
                    adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                    var escapTableName = EscapeTableName(OnlyTable);
                    Logger.Info($"The schema of table {escapTableName} is loading...");
                    var entity = new EntitySchema(OnlyTable);
                    var schemaTable = new DataTable();
                    var command = ProviderFactory.CreateCommand();
                    command.Connection = conn;
                    command.CommandText = $"SELECT * FROM {escapTableName}";
                    adapter.SelectCommand = command;
                    adapter.FillSchema(schemaTable, SchemaType.Source);
                    foreach (DataColumn col in schemaTable.Columns)
                    {
                        var prop = new PropertySchema
                        {
                            Name = col.ColumnName,
                            DataType = col.DataType,
                            IsIdentity = col.AutoIncrement,
                            IsNullable = col.AllowDBNull,
                            Length = col.MaxLength,
                            PrimaryKey = schemaTable.PrimaryKey != null && schemaTable.PrimaryKey.Length > 0 ? schemaTable.PrimaryKey.Contains(col) : (col.AutoIncrement || col.Unique),
                        };
                        entity.Properties.Add(prop);
                    }
                    return entity;
                }
            }
            return null;
        }

        private string EscapeTableName(string tableName)
        {
            if (ProviderFactory.GetType().Name.Contains("MySql"))
            {
                return $"`{tableName}`";
            }
            else
            {
                return $"[{tableName}]";
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

            if (string.IsNullOrWhiteSpace(OnlyTable))
                BuildAll();
            else
                BuildSingle();
        }

        private void BuildSingle()
        {
            var template = new SingleEntityTemplate(this, GetSingleEntity());
            var content = template.FileContent;
            Logger.Info(content);
            Clipboard.SetText(content);
            Logger.Info("All codes have been copied to clipboard.");
            Console.ReadLine();
        }

        private void BuildAll()
        {
            LoadSchemas();
            LoadProjects();
            Projects.ForEach(x => x.Generate());
            Logger.Info("All codes have been generated successfully.");
            System.Diagnostics.Process.Start("explorer", OutputFolder);
        }
    }
}
