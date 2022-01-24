using System;
using System.Linq;
using SimpleEntityFramework.Domain.Roles.Templates;

namespace SimpleEntityFramework.Domain.Objects.Templates
{
    public class BaseReposTemplate : ClassTemplate
    {
        public const string ClassName = "BaseRepository";

        public BaseReposTemplate(IProjectTemplate project) : base(project)
        {
        }

        public override string Name => ClassName;

        public override string FileContent => $@"{Profile}
{string.Join(Environment.NewLine, Project.RefProjects.Select(x => $"using {x.Namespace};" ))}
using SE.Entity;
using SE.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace {Namespace}
{{
    public class {Name}<TEntity> where TEntity : BaseEntity
    {{
        protected static readonly Database DB = Database.GetDefault();

        protected static readonly string TableName = DB.GetTableName(typeof(TEntity).Name);

        protected static readonly Dictionary<PropertyInfo, DataObjectFieldAttribute> PropMapping
            = typeof(TEntity).GetProperties().Where(x => x.GetCustomAttribute<DataObjectFieldAttribute>() != null)
            .ToDictionary(x => x, x => x.GetCustomAttribute<DataObjectFieldAttribute>());

        protected static readonly PropertyInfo[] KeyProps = PropMapping.Where(x => x.Value.PrimaryKey).Select(x => x.Key).ToArray();

        protected static readonly PropertyInfo[] OrdinaryProps = PropMapping.Where(x => !x.Value.PrimaryKey && !x.Value.IsIdentity).Select(x => x.Key).ToArray();

        protected static readonly PropertyInfo[] NonIncrementProps = PropMapping.Where(x => !x.Value.IsIdentity).Select(x => x.Key).ToArray();

        protected DbParameter[] CreateParameters(IEnumerable<PropertyInfo> props, TEntity entity)
        {{
            return props.Select(x => DB.CreateParameter(x.Name, x.GetValue(entity), DB.GetDbType(x.PropertyType), x.GetCustomAttribute<DataObjectFieldAttribute>().Length)).ToArray();
        }}

        protected DbParameter[] CreateKeyParameters(object[] keyValues)
        {{
            var parameters = new DbParameter[KeyProps.Length];
            for (int i = 0; i < KeyProps.Length; i++)
            {{
                var prop = KeyProps[i];
                var attr = prop.GetCustomAttribute<DataObjectFieldAttribute>();
                parameters[i] = DB.CreateParameter(prop.Name, keyValues[i], DB.GetDbType(prop.PropertyType), attr.Length);
            }}
            return parameters;
        }}

        public virtual bool Exists(params object[] keyValues)
        {{
            var sql = $""SELECT 1 FROM {{TableName}} WHERE {{string.Join("" AND "", KeyProps.Select(x => $""{{x.Name}} = @{{x.Name}}""))}}"";
            return DB.ExecuteScalar<int>(sql, CreateKeyParameters(keyValues)) == 1;
        }}

        public virtual bool Exists(Expression<Func<TEntity, bool>> expression)
        {{
            var visitor = new ExpressionVisitor<TEntity>(DB.GetProviderFactory(), expression);
            return DB.ExecuteScalar<int>($""SELECT 1 FROM {{TableName}} {{visitor.Where}}"", visitor.Parameters) == 1;
        }}

        public virtual int Count(Expression<Func<TEntity, bool>> expression)
        {{
            var visitor = new ExpressionVisitor<TEntity>(DB.GetProviderFactory(), expression);
            return DB.ExecuteScalar<int>($""SELECT COUNT(1) FROM {{TableName}} {{visitor.Where}}"", visitor.Parameters);
        }}

        public virtual TEntity Find(params object[] keyValues)
        {{
            var sql = $""SELECT * FROM {{TableName}} WHERE {{string.Join("" AND "", KeyProps.Select(x => $""{{x.Name}} = @{{x.Name}}""))}}"";
            using (var reader = DB.ExecuteReader(sql, CreateKeyParameters(keyValues)))
            {{
                return reader.MapTo<TEntity>();
            }}
        }}

        public virtual List<TEntity> Select(Expression<Func<TEntity, bool>> expression)
        {{
            var visitor = new ExpressionVisitor<TEntity>(DB.GetProviderFactory(), expression);
            using (var dt = DB.ExecuteDataTable(visitor.CommandText, visitor.Parameters))
            {{
                return dt.MapToList<TEntity>();
            }}
        }}

        public virtual int Insert(TEntity entity)
        {{
            var sql = $""INSERT INTO {{TableName}} ({{string.Join("", "", NonIncrementProps.Select(x => x.Name))}}) VALUES ({{string.Join("","", NonIncrementProps.Select(x => ""@"" + x.Name))}})"";
            return DB.ExecuteNonQuery(sql, CreateParameters(NonIncrementProps, entity));
        }}

        public virtual int Insert(params TEntity[] entities)
        {{
            var effected = 0;
            foreach (var entity in entities)
            {{
                effected += Insert(entity);
            }}
            return effected;
        }}

        public virtual int Update(TEntity entity)
        {{
            var sql = $""UPDATE {{TableName}} SET {{string.Join("", "", OrdinaryProps.Select(x => $""{{x.Name}} = @{{x.Name}}""))}} WHERE {{string.Join("" AND "", KeyProps.Select(x => $""{{x.Name}} = @{{x.Name}}""))}}"";
            return DB.ExecuteNonQuery(sql, CreateParameters(OrdinaryProps.Concat(KeyProps), entity));
        }}

        public virtual int Update(params TEntity[] entities)
        {{
            var effected = 0;
            foreach (var entity in entities)
            {{
                effected += Update(entity);
            }}
            return effected;
        }}

        public virtual int Remove(params object[] keyValues)
        {{
            var sql = $""DELETE FROM {{TableName}} WHERE {{string.Join("" AND "", KeyProps.Select(x => $""{{x.Name}} = @{{x.Name}}""))}}"";
            return DB.ExecuteNonQuery(sql, CreateKeyParameters(keyValues));
        }}

        public virtual int Delete(TEntity entity)
        {{
            var sql = $""DELETE FROM {{TableName}} WHERE {{string.Join("" AND "", KeyProps.Select(x => $""{{x.Name}} = @{{x.Name}}""))}}"";
            return DB.ExecuteNonQuery(sql, CreateParameters(KeyProps, entity));
        }}

        public virtual int Delete(params TEntity[] entities)
        {{
            var effected = 0;
            foreach (var entity in entities)
            {{
                effected += Delete(entity);
            }}
            return effected;
        }}

        public virtual int Delete(Expression<Func<TEntity, bool>> expression)
        {{
            var visitor = new ExpressionVisitor<TEntity>(DB.GetProviderFactory(), expression);
            var sql = $""DELETE FROM {{TableName}} {{visitor.Where}}"";
            return DB.ExecuteNonQuery(sql, visitor.Parameters);
        }}
    }}
}}";
    }
}
