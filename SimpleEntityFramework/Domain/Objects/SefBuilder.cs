using SimpleEntityFramework.Domain.Objects.Schemas;
using SimpleEntityFramework.Domain.Objects.Templates;
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
using System.Text.RegularExpressions;

namespace SimpleEntityFramework.Domain.Objects
{
    public class SefBuilder : ISefBuilder
    {
        public string ConnectionString { get; set; }
        public DbProviderFactory ProviderFactory { get; set; }
        public string NamespaceRoot { get; set; }
        public string OutputFolder { get; set; }
        public List<string> Tables { get; }
        public List<IEntitySchema> Entities { get; }
        public List<IProjectTemplate> Projects { get; }

        public SefBuilder()
        {
            Tables = new List<string>();
            Entities = new List<IEntitySchema>();
            Projects = new List<IProjectTemplate>();
        }

        private void Prepare()
        {
            CheckInput();

            LoadSchemas();

            LoadProjects();
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
                Tables.AddRange(conn.GetSchema("Tables").AsEnumerable().Select(dr => dr.Field<string>(2)).ToArray());
                adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                foreach (var tableName in Tables)
                {
                    var name = GetTableName(tableName);
                    Logger.Info($"The schema of table {name} is loading...");
                    var entity = new EntitySchema(tableName);
                    var table = new DataTable();
                    var command = ProviderFactory.CreateCommand();
                    command.Connection = conn;
                    command.CommandText = $"SELECT * FROM {name}";
                    adapter.SelectCommand = command;
                    adapter.FillSchema(table, SchemaType.Source);
                    foreach (DataColumn col in table.Columns)
                    {
                        var prop = new PropertySchema
                        {
                            Name = col.ColumnName,
                            DataType = col.DataType,
                            IsIdentity = col.AutoIncrement,
                            IsNullable = col.AllowDBNull,
                            Length = col.MaxLength,
                            PrimaryKey = table.PrimaryKey != null && table.PrimaryKey.Length > 0 ? table.PrimaryKey.Contains(col) : (col.AutoIncrement || col.Unique),
                        };
                        entity.Properties.Add(prop);
                    }
                    if (!entity.Properties.Any(x => x.PrimaryKey))
                    {
                        Logger.Error($"Cannot find any key in table {name}.");
                        continue;
                    }
                    if (entity.Properties.Any(x => !Regex.Match(x.Name, @"\w[\w,\d,_]*").Success))
                    {
                        Logger.Error($"Invalid column name in table {name}.");
                        continue;
                    }
                    Entities.Add(entity);
                }
            }
        }

        private string GetTableName(string tableName)
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
            Prepare();
            Projects.ForEach(x => x.Generate());
            Logger.Info("All codes have been generated successfully.");
            System.Diagnostics.Process.Start("explorer", OutputFolder);
        }
    }
}
