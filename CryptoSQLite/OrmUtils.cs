using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CryptoSQLite.Mapping;

namespace CryptoSQLite
{
    internal static class OrmUtils
    {
        public static bool IsEncrypted(this PropertyInfo property)
        {
            var attributes = property.GetCustomAttributes<EncryptedAttribute>();
            return attributes.Any();
        }

        public static bool IsColumn(this PropertyInfo property)
        {
            var attributes = property.GetCustomAttributes<ColumnAttribute>();
            return attributes.Any();
        }

        public static bool IsPrimaryKey(this PropertyInfo property)
        {
            var attributes = property.GetCustomAttributes<PrimaryKeyAttribute>();
            return attributes.Any();
        }

        public static bool IsAutoIncremental(this PropertyInfo property)
        {
            var attributes = property.GetCustomAttributes<AutoIncrementalAttribute>();
            return attributes.Any();
        }

        public static bool IsIgnorable(this PropertyInfo property)
        {
            var attributes = property.GetCustomAttributes<IgnoreAttribute>();
            return attributes.Any();
        }

        public static bool IsNotNullable(this PropertyInfo property)
        {
            var attributes = property.GetCustomAttributes<NotNullAttribute>();
            return attributes.Any();
        }

        public static bool HasDefaultValue(this PropertyInfo property)
        {
            var attributes = property.GetCustomAttributes<HasDefaultValueAttribute>();
            return attributes.Any();
        }

        public static object GetDefaultValue(this PropertyInfo property)
        {
            var attributes = property.GetCustomAttributes<HasDefaultValueAttribute>().ToArray();
            return attributes.Length == 0 ? null : attributes[0].DefaultValue;
        }

        public static string GetColumnName(this PropertyInfo property)
        {
            var attrs = property.GetCustomAttributes<ColumnAttribute>().ToArray();

            return attrs.Length == 0 ? property.Name : attrs[0].Name;
        }

        public static string GetSqlType(Type type)
        {
            if (type == typeof(int) || 
                type == typeof(short) || 
                type == typeof(long) ||
                type == typeof(uint) ||
                type == typeof(ushort) ||
                type == typeof(ulong) ||
                type == typeof(byte) ||
                type == typeof(bool))
                return "integer";
            if (type == typeof(string))
                return "text";

            throw new Exception("CryptoSQLite can work only with next types: integers (1-8 bytes) add strings");
        }


        public static IEnumerable<PropertyInfo> GetCompatibleProperties<TTable>()
        {
            // Point of this method is to find those properties, that can be used as column in table.
            // Only properties, that have public getter and public setter can be used as column in table.

            var type = typeof(TTable);

            var compatibleProperties = type.GetRuntimeProperties().Where(pr => pr.CanRead &&
                                                                               pr.CanWrite &&
                                                                               pr.GetMethod!=null &&
                                                                               pr.SetMethod!=null &&
                                                                               !pr.GetMethod.IsStatic &&
                                                                               !pr.SetMethod.IsStatic &&
                                                                               !pr.IsIgnorable());

            return compatibleProperties;
        }


        public static ColumnMap CreateColumnMap(this PropertyInfo property)
        {
            return new ColumnMap(property.GetColumnName(),
                                 property.PropertyType,
                                 property.HasDefaultValue(), 
                                 property.GetDefaultValue(),
                                 property,
                                 property.IsPrimaryKey(),
                                 property.IsAutoIncremental(),
                                 property.IsNotNullable(),
                                 property.IsEncrypted());
        }
    }
}
