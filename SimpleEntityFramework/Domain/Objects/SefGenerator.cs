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
    public class SefGenerator : ISefGenerator
    {
        public const string OutputFolderName = "Output";

        public string NamespaceRoot { get; set; }
        public string OutputFolder { get; set; }
        public Database Database { get; set; }
        public string DatabaseName { get; set; }
        public List<string> TableNames { get; set; }
        public List<IEntitySchema> Entities { get; set; }
        public List<IProjectTemplate> Projects { get; set; }

        public static SefGenerator Current { get; private set; }

        public SefGenerator(string namespaceRoot, Database database)
        {
            Current = this;
            NamespaceRoot = namespaceRoot;
            OutputFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, OutputFolderName);
            if (!Directory.Exists(OutputFolder))
            {
                Directory.CreateDirectory(OutputFolder);
            }

            Database = database;
            using (var conn = Database.OpenConnection())
            {
                var dt = conn.GetSchema("Tables");
                var rows = dt.Rows.Cast<DataRow>();
                DatabaseName = rows.FirstOrDefault()?.Field<string>(0);
                TableNames = rows.Select(dr => dr.Field<string>(2)).ToList();
            }

            Entities = new List<IEntitySchema>();

            Projects = new List<IProjectTemplate>();
        }

        public ISefGenerator LoadEntities()
        {
            using (var adapter = Database.CreateDataAdapter())
            using (var conn = Database.OpenConnection())
            {
                adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                foreach (var tableName in TableNames)
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
                    Entities.Add(entity);
                }
            }
            return this;
        }
        
        public ISefGenerator AddProjects(params IProjectTemplate[] projects)
        {
            foreach (var project in projects)
            {
                project.AddRefProjets(Projects.ToArray());
            }
            Projects.AddRange(projects);
            return this;
        }

        public void Generate()
        {
            Projects.ForEach(x => x.Generate());
            Logger.Info("All codes have been generated successfully.");
            System.Diagnostics.Process.Start("explorer.exe", OutputFolder);
        }
    }
}
