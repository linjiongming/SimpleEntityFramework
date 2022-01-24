using SimpleEntityFramework.Domain.Objects.Templates;
using SimpleEntityFramework.Domain.Roles.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleEntityFramework.Domain.Objects.Templates.Framework
{
    public class EntityMapperTemplate : ClassTemplate
    {
        public const string ClassName = "EntityMapper";

        public EntityMapperTemplate(IProjectTemplate project) : base(project)
        {
        }

        public override string Name => ClassName;

        public override string FileContent => $@"{Profile}
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

public static class EntityMapper
{{
    public static T MapTo<T>(this IDataReader reader)
    {{
        var entity = Activator.CreateInstance<T>();
        reader.FillEntity(entity);
        return entity;
    }}

    public static T MapTo<T>(this DataTable dt)
    {{
        var entity = Activator.CreateInstance<T>();
        dt.FillEntity(entity);
        return entity;
    }}

    public static T MapTo<T>(this object obj)
    {{
        var entity = Activator.CreateInstance<T>();
        FillEntity(obj, entity);
        return entity;
    }}

    public static List<T> MapToList<T>(this IDataReader reader)
    {{
        var list = new List<T>();
        reader.FillList(list);
        return list;
    }}

    public static List<T> MapToList<T>(this DataTable dt)
    {{
        var list = new List<T>();
        dt.FillList(list);
        return list;
    }}

    public static List<TTarget> MapToList<TSource, TTarget>(this IEnumerable<TSource> sources)
    {{
        var list = new List<TTarget>();
        sources.FillList(list);
        return list;
    }}

    public static void FillEntity<T>(this IDataReader reader, T entity)
    {{
        if (entity == null)
        {{
            entity = Activator.CreateInstance<T>();
        }}
        var mapping = reader.GetColumnMapping<T>();
        var isRead = true;
        if (reader.IsClosed)
        {{
            isRead = reader.Read();
        }}
        foreach (var key in mapping.Keys)
        {{
            if (!reader.IsDBNull(key))
            {{
                var prop = mapping[key];
                var value = Convert.ChangeType(reader[key], Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                prop.SetValue(entity, value);
            }}
        }}
    }}

    public static void FillEntity<T>(this DataTable dt, T entity)
    {{
        if (dt == null || dt.Rows.Count <= 0) return;
        if (entity == null)
        {{
            entity = Activator.CreateInstance<T>();
        }}
        var mapping = dt.GetColumnMapping<T>();
        if (mapping != null && mapping.Count > 0)
        {{
            var row = dt.Rows[0];
            foreach (var key in mapping.Keys)
            {{
                if (!row.IsNull(key))
                {{
                    var prop = mapping[key];
                    var value = Convert.ChangeType(row[key], Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                    prop.SetValue(entity, value);
                }}
            }}
        }}
    }}

    public static void FillEntity<TSource, TTarget>(this TSource source, TTarget target) 
    {{
        if (source == null) return;
        if (target == null)
        {{
            target = Activator.CreateInstance<TTarget>();
        }}
        var mapping = GetTypeMapping<TSource, TTarget>();
        if (mapping != null && mapping.Count > 0)
        {{
            foreach (var srcProp in mapping.Keys)
            {{
                var srcValue = srcProp.GetValue(source);
                if (srcValue != null)
                {{
                    var tgtProp = mapping[srcProp];
                    var tgtValue = Convert.ChangeType(srcValue, Nullable.GetUnderlyingType(tgtProp.PropertyType) ?? tgtProp.PropertyType);
                    tgtProp.SetValue(target, tgtValue);
                }}
            }}
        }}
    }}

    public static void FillList<T>(this IDataReader reader, List<T> list)
    {{
        if (list == null)
        {{
            list = new List<T>();
        }}
        var mapping = reader.GetColumnMapping<T>();
        if (mapping != null && mapping.Count > 0)
        {{
            var isRead = true;
            if (reader.IsClosed)
            {{
                isRead = reader.Read();
            }}
            do
            {{
                var entity = Activator.CreateInstance<T>();
                foreach (var key in mapping.Keys)
                {{
                    if (!reader.IsDBNull(key))
                    {{
                        var prop = mapping[key];
                        var value = Convert.ChangeType(reader[key], Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                        prop.SetValue(entity, value);
                    }}
                }}
                list.Add(entity);
            }} while (reader.Read());
        }}
    }}

    public static void FillList<T>(this DataTable dt, List<T> list)
    {{
        if (list == null)
        {{
            list = new List<T>();
        }}
        var mapping = dt.GetColumnMapping<T>();
        if (mapping != null && mapping.Count > 0)
        {{
            foreach (DataRow row in dt.Rows)
            {{
                var entity = Activator.CreateInstance<T>();
                foreach (var key in mapping.Keys)
                {{
                    if (!row.IsNull(key))
                    {{
                        var prop = mapping[key];
                        var value = Convert.ChangeType(row[key], Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                        prop.SetValue(entity, value);
                    }}
                }}
                list.Add(entity);
            }}
        }}
    }}

    public static void FillList<TSource, TTarget>(this IEnumerable<TSource> sources, List<TTarget> list)
    {{
        if (sources == null || sources.Count() <= 0) return;
        if (list == null)
        {{
            list = new List<TTarget>();
        }}
        var mapping = GetTypeMapping<TSource, TTarget>();
        if (mapping != null && mapping.Count > 0)
        {{
            foreach (var source in sources)
            {{
                var target = Activator.CreateInstance<TTarget>();
                foreach (var srcProp in mapping.Keys)
                {{
                    var srcValue = srcProp.GetValue(source);
                    if (srcValue != null)
                    {{
                        var tgtProp = mapping[srcProp];
                        var tgtValue = Convert.ChangeType(srcValue, Nullable.GetUnderlyingType(tgtProp.PropertyType) ?? tgtProp.PropertyType);
                        tgtProp.SetValue(target, tgtValue);
                    }}
                }}
                list.Add(target);
            }}
        }}
    }}

    public static Dictionary<string, PropertyInfo> GetColumnMapping<T>(this DataTable dt)
    {{
        if (dt == null) return null;
        var type = typeof(T);
        var props = type.GetProperties();
        var mapping = new Dictionary<string, PropertyInfo>();
        foreach (DataColumn col in dt.Columns)
        {{
            var prop = props.FirstOrDefault(x => x.Name.Equals(col.ColumnName, StringComparison.CurrentCultureIgnoreCase));
            if (prop != null)
            {{
                mapping[col.ColumnName] = prop;
            }}
        }}
        return mapping;
    }}

    public static Dictionary<int, PropertyInfo> GetColumnMapping<T>(this IDataReader reader)
    {{
        if (reader == null) return null;
        var cols = reader.GetSchemaTable().AsEnumerable().Select(dr => dr[""ColumnName""].ToString()).ToList();
        var type = typeof(T);
        var props = type.GetProperties();
        var mapping = new Dictionary<int, PropertyInfo>();
        for (int i = 0; i < cols.Count; i++)
        {{
            var prop = props.FirstOrDefault(x => x.Name.Equals(cols[i], StringComparison.CurrentCultureIgnoreCase));
            if (prop != null)
            {{
                mapping[i] = prop;
            }}
        }}
        return mapping;
    }}

    public static Dictionary<PropertyInfo, PropertyInfo> GetTypeMapping<TSource, TTarget>()
    {{
        var mapping = new Dictionary<PropertyInfo, PropertyInfo>();
        foreach (var srcProp in typeof(TSource).GetProperties())
        {{
            if (mapping.ContainsKey(srcProp)) continue;
            foreach (var tgtProp in typeof(TTarget).GetProperties())
            {{
                if (mapping.ContainsValue(tgtProp)) continue;
                if (srcProp.Name.Equals(tgtProp.Name))
                {{
                    mapping.Add(srcProp, tgtProp);
                }}
            }}
        }}
        return mapping;
    }}

}}";
    }
}
