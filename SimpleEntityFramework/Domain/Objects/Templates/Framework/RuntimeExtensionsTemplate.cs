using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleEntityFramework.Domain.Roles.Templates;

namespace SimpleEntityFramework.Domain.Objects.Templates.Framework
{
    public class RuntimeExtensionsTemplate : ClassTemplate
    {
        public const string ClassName = "RuntimeExtensions";

        public RuntimeExtensionsTemplate(IProjectTemplate project) : base(project)
        {
        }

        public override string Name => ClassName;

        public override string FileContent => $@"{Profile}
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace {Builder.NamespaceRoot}
{{
    public static class {Builder.NamespaceRoot}_{Project.Name}_{Name}
    {{
        public static T GetField<T>(this DataRow dr, string field)
        {{
            if (dr.Table.Columns.Contains(field) && !dr.IsNull(field))
            {{
                return dr.Field<T>(field);
            }}
            return default(T);
        }}

        public static int? ToInt(this object obj)
        {{
            if (obj != null && obj != DBNull.Value)
            {{
                if (obj is int equal)
                    return equal;
                if (int.TryParse(obj?.ToString(), out var parse))
                    return parse;
            }}
            return null;
        }}

        public static int ToInt(this object obj, int def)
        {{
            return obj.ToInt() ?? def;
        }}

        public static long? ToLong(this object obj)
        {{
            if (obj != null && obj != DBNull.Value)
            {{
                if (obj is long equal)
                    return equal;
                if (long.TryParse(obj?.ToString(), out var parse))
                    return parse;
            }}
            return null;
        }}

        public static long ToLong(this object obj, long def)
        {{
            return obj.ToLong() ?? def;
        }}

        public static short? ToShort(this object obj)
        {{
            if (obj != null && obj != DBNull.Value)
            {{
                if (obj is short equal)
                    return equal;
                if (short.TryParse(obj?.ToString(), out var parse))
                    return parse;
            }}
            return null;
        }}

        public static short ToShort(this object obj, short def)
        {{
            return obj.ToShort() ?? def;
        }}

        public static byte? ToByte(this object obj)
        {{
            if (obj != null && obj != DBNull.Value)
            {{
                if (obj is byte equal)
                    return equal;
                if (byte.TryParse(obj?.ToString(), out var parse))
                    return parse;
            }}
            return null;
        }}

        public static short ToByte(this object obj, short def)
        {{
            return obj.ToByte() ?? def;
        }}

        public static double? ToDouble(this object obj)
        {{
            if (obj != null && obj != DBNull.Value)
            {{
                if (obj is double equal)
                    return equal;
                if (double.TryParse(obj?.ToString(), out var parse))
                    return parse;
            }}
            return null;
        }}

        public static double ToDouble(this object obj, double def)
        {{
            return obj.ToDouble() ?? def;
        }}

        public static float? ToFloat(this object obj)
        {{
            if (obj != null && obj != DBNull.Value)
            {{
                if (obj is float equal)
                    return equal;
                if (float.TryParse(obj?.ToString(), out var parse))
                    return parse;
            }}
            return null;
        }}

        public static float ToFloat(this object obj, float def)
        {{
            return obj.ToFloat() ?? def;
        }}

        public static DateTime? ToDateTime(this object obj)
        {{
            if (obj != null && obj != DBNull.Value)
            {{
                if (obj is DateTime equal)
                    return equal;
                if (DateTime.TryParse(obj?.ToString(), out var parse))
                    return parse;
            }}
            return null;
        }}

        public static DateTime ToDateTimeOrDefault(this object obj)
        {{
            if (obj != null && obj != DBNull.Value)
            {{
                if (obj is DateTime equal)
                    return equal;
                if (DateTime.TryParse(obj?.ToString(), out var parse))
                    return parse;
            }}
            return default(DateTime);
        }}

        public static TimeSpan ToTimeSpan(this string time)
        {{
            int h = 0, m = 0, s = 0;
            var arr = time.Split(':');
            if (arr.Length > 1)
            {{
                h = arr[0].ToInt(0);
                m = arr[1].ToInt(0);
                if (arr.Length > 2)
                {{
                    s = arr[2].ToInt(0);
                }}
            }}
            return new TimeSpan(h, m, s);
        }}

        public static int IfZero(this int num, int val)
        {{
            return num == 0 ? val : num;
        }}

        public static bool IsEmpty(this string str)
        {{
            return string.IsNullOrWhiteSpace(str);
        }}

        public static string IfEmpty(this string str, string def)
        {{
            return string.IsNullOrWhiteSpace(str) ? def : str;
        }}

        public static bool IsNotEmpty(this string str)
        {{
            return !string.IsNullOrWhiteSpace(str);
        }}

        public static string AddSuffixBeforeExt(this string filename, string suffix)
        {{
            var index = filename.LastIndexOf('.');
            return filename.Insert(index, suffix);
        }}

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {{
            foreach (var item in source)
            {{
                action(item);
            }}
        }}

        public static IEnumerable<T> FirstHalf<T>(this IEnumerable<T> source)
        {{
            return source.Take(source.Count() / 2);
        }}

        public static IEnumerable<T> SecondHalf<T>(this IEnumerable<T> source)
        {{
            return source.Skip(source.Count() / 2);
        }}

        public static IEnumerable<IEnumerable<T>> ToBatches<T>(this IEnumerable<T> source, int batchSize)
        {{
            var cursor = batchSize - 1;
            var total = source.Count();
            for (int i = 0; i < total; i++)
            {{
                if (i == cursor)
                {{
                    yield return source.Skip(cursor + 1 - batchSize).Take(batchSize);
                    cursor += batchSize;
                }}
                if (i == total - 1)
                {{
                    yield return source.Skip(cursor + 1 - batchSize);
                    break;
                }}
            }}
        }}

        public static string JoinStrings<T>(this IEnumerable<T> source, string separator)
        {{
            if (source == null) return null;
            return string.Join(separator, source);
        }}

        public static string Break(this string str, int size, string breaker = ""\r\n"")
        {{
            return str.ToBatches(size).Select(x => new string(x.ToArray())).JoinStrings(breaker);
        }}

        public static DateTime Reset(this DateTime date, int? year = null, int? month = null, int? day = null, int? hour = null, int? minute = null, int? second = null)
        {{
            if (null == year)   /**/    year = date.Year;
            if (null == month)  /**/    month = date.Month;
            if (null == day)    /**/    day = date.Day;
            if (null == hour)   /**/    hour = date.Hour;
            if (null == minute) /**/    minute = date.Minute;
            if (null == second) /**/    second = date.Second;
            return new DateTime(year.Value, month.Value, day.Value, hour.Value, minute.Value, second.Value);
        }}

        public static int GetMonthNum(this DateTime date)
        {{
            return date.Year * 100 + date.Month;
        }}

        public static string ToUniversalJson(this DateTime dateTime)
        {{
            return dateTime.ToUniversalTime().ToString(""yyyy-MM-ddTHH:mm:ss.fffZ"");
        }}

        public static IEnumerable<T> Find<T>(this IContainer container)
        {{
            foreach (var component in container.Components)
            {{
                if (component is T result)
                {{
                    yield return result;
                }}
            }}
        }}

        public static T First<T>(this IContainer container) where T : class
        {{
            foreach (var component in container.Components)
            {{
                if (component is T result)
                {{
                    return result;
                }}
            }}
            return null;
        }}

        public static readonly string[] TrueStrings = new string[] {{ ""true"", ""1"", ""yes"", ""y"" }};
        public static bool GetBoolValue(this NameValueCollection settings, string key, bool @default = false)
        {{
            if (settings != null && !string.IsNullOrWhiteSpace(key) && settings.AllKeys.Contains(key))
            {{
                return TrueStrings.Contains(settings[key], StringComparer.CurrentCultureIgnoreCase);
            }}
            return @default;
        }}

        public static TEnum GetEnumValue<TEnum>(this NameValueCollection settings, string key, TEnum @default = default(TEnum)) where TEnum : struct
        {{
            if (settings != null && !string.IsNullOrWhiteSpace(key) && settings.AllKeys.Contains(key))
            {{
                if (Enum.TryParse(settings[key], out TEnum result))
                    return result;
            }}
            return @default;
        }}

        public static string[] GetArray(this NameValueCollection settings, string key, params string[] separators)
        {{
            if (settings != null && !string.IsNullOrWhiteSpace(key) && settings.AllKeys.Contains(key))
            {{
                return settings[key].Split(new[] {{ "","", ""，"" }}.Union(separators).ToArray(), StringSplitOptions.RemoveEmptyEntries).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            }}
            return null;
        }}

        public static T[] GetArray<T>(this NameValueCollection settings, string key, params string[] separators) where T : IConvertible
        {{
            if (settings != null && !string.IsNullOrWhiteSpace(key) && settings.AllKeys.Contains(key))
            {{
                return settings[key].Split(new[] {{ "","", ""，"" }}.Union(separators).ToArray(), StringSplitOptions.RemoveEmptyEntries)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => (T)Convert.ChangeType(x, Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T))).ToArray();
            }}
            return null;
        }}

    }}
}}";
    }
}
