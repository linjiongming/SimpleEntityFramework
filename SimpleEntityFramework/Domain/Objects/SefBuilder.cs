using SimpleEntityFramework.Domain.Objects.Schemas;
using SimpleEntityFramework.Domain.Objects.Templates;
using SimpleEntityFramework.Domain.Roles;
using SimpleEntityFramework.Domain.Roles.Schemas;
using SimpleEntityFramework.Domain.Roles.Templates;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SimpleEntityFramework.Domain.Objects
{
    public class SefBuilder : ISefBuilder
    {
        private List<string> _tables;
        private List<IEntitySchema> _entities;
        private List<IProjectTemplate> _projects;

        public string NamespaceRoot { get; set; }
        public string OutputFolder { get; set; }
        public Database Database { get; set; }

        public List<string> Tables => _tables;
        public List<IEntitySchema> Entities => _entities;
        public List<IProjectTemplate> Projects => _projects;

        public SefBuilder()
        {
            NamespaceRoot = "My";
            Database = new Database() { ProviderName = "System.Data.SqlClient" };
            OutputFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Output");
        }

        private void Prepare()
        {
            if (!Directory.Exists(OutputFolder))
            {
                Directory.CreateDirectory(OutputFolder);
            }

            if (string.IsNullOrWhiteSpace(Database.ConnectionString))
            {
                Database = Database.GetDefault();
            }

            _entities = new List<IEntitySchema>();
            using (var adapter = Database.CreateDataAdapter())
            using (var conn = Database.OpenConnection())
            {
                _tables = conn.GetSchema("Tables").AsEnumerable().Select(dr => dr.Field<string>(2)).ToList();
                adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                foreach (var tableName in Tables)
                {
                    var name = Database.GetTableName(tableName);
                    Logger.Info($"The schema of table {name} is loading...");
                    var entity = new EntitySchema(tableName);
                    var table = new DataTable();
                    var command = Database.GetSqlStringCommand($"SELECT * FROM {name}", conn);
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
                    _entities.Add(entity);
                }
            }

            _projects = new List<IProjectTemplate>();
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
                _projects.Add(frameworkProject);
                _projects.Add(entityProject);
                _projects.Add(reposProject);
            }
        }

        public void Build()
        {
            Prepare();
            Projects.ForEach(x => x.Generate());
            Logger.Info("All codes have been generated successfully.");
            System.Diagnostics.Process.Start("explorer.exe", OutputFolder);
        }
    }
}
