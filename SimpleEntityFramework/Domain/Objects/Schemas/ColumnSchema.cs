using SimpleEntityFramework.Domain.Roles.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleEntityFramework.Domain.Objects.Schemas
{
    public class ColumnSchema : IColumnSchema
    {
        public string Name { get; set; }
        public Type DataType { get; set; }
        public bool IsNullable { get; set; }
        public bool PrimaryKey { get; set; }
        public bool IsIdentity { get; set; }
        public int Length { get; set; }

        private string _typeName;
        public string TypeName => _typeName ?? (_typeName = TypeNameOrAlias(DataType) + (IsNullable && DataType.IsValueType ? "?" : ""));

        private string _propertyName;
        public string PropertyName => _propertyName ?? (_propertyName = GetPropertyName(Name));

        public static Dictionary<Type, string> TypeAlias = new Dictionary<Type, string>
        {
            { typeof(bool), "bool" },
            { typeof(byte), "byte" },
            { typeof(byte[]), "byte[]" },
            { typeof(char), "char" },
            { typeof(decimal), "decimal" },
            { typeof(double), "double" },
            { typeof(float), "float" },
            { typeof(int), "int" },
            { typeof(long), "long" },
            { typeof(object), "object" },
            { typeof(sbyte), "sbyte" },
            { typeof(short), "short" },
            { typeof(string), "string" },
            { typeof(uint), "uint" },
            { typeof(ulong), "ulong" },
            { typeof(void), "object" }
        };

        public static string TypeNameOrAlias(Type type)
        {
            // Lookup alias for type
            if (TypeAlias.TryGetValue(type, out string alias))
                return alias;

            // Default to CLR type name
            return type.Name;
        }


        public static readonly string[] ColumnPrefixes = new[] { "c_" };

        public static string GetPropertyName(string columnName)
        {
            var propertyName = columnName;
            var prefix = ColumnPrefixes.FirstOrDefault(x => propertyName.StartsWith(x, StringComparison.CurrentCultureIgnoreCase));
            if (!string.IsNullOrWhiteSpace(prefix) && propertyName.StartsWith(prefix, StringComparison.CurrentCultureIgnoreCase))
            {
                propertyName = propertyName.Substring(prefix.Length);
            }
            return string.Concat(propertyName.Split(new[] { '_', ' ', '-' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Substring(0, 1).ToUpper() + x.Substring(1)));
        }
    }
}
