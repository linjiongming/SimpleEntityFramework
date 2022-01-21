using SimpleEntityFramework.Domain.Roles.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleEntityFramework.Domain.Objects.Schemas
{
    public class PropertySchema: IPropertySchema
    {
        public string Name { get; set; }
        public Type DataType { get; set; }
        public bool IsNullable { get; set; }
        public bool PrimaryKey { get; set; }
        public bool IsIdentity { get; set; }
        public int Length { get; set; }
        public string TypeName => TypeNameOrAlias(DataType) + (IsNullable && DataType.IsValueType ? "?" : "");

        static Dictionary<Type, string> _typeAlias = new Dictionary<Type, string>
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

        static string TypeNameOrAlias(Type type)
        {
            // Lookup alias for type
            if (_typeAlias.TryGetValue(type, out string alias))
                return alias;

            // Default to CLR type name
            return type.Name;
        }
    }
}
