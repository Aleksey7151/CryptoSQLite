using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CryptoSQLite
{
    internal class SqlColumnInfo
    {
        public string Name { get; set; }
        public string SqlType { get; set; }
        public string SqlValue { get; set; }
    }

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
            var attributes = property.GetCustomAttributes<IgnoredAttribute>();
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
            return attrs.Length == 0 ? property.Name : attrs[0].ColumnName;
        }

        public static string GetSqlType(this PropertyInfo property)
        {
            var type = property.PropertyType;

            if (type == typeof(int)    || type == typeof(short) || type == typeof(uint) || 
                type == typeof(ushort) || type == typeof(byte)  || type == typeof(bool) )
                return "INTEGER";

            if (type == typeof(string) || type == typeof(ulong) || type == typeof(long) || type == typeof(DateTime))
                return "TEXT";

            if (type == typeof(double) || type == typeof(float))
                return "REAL";

            throw new Exception($"Type {type} is not compatible with CryptoSQLite.");
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

        public static string ToSqlString(this byte[] data)
        {
            var builder = new StringBuilder("");
            foreach (var t in data)
            {
                builder.Append($"{t:X02}");
            }
            return builder.ToString();
        }

        public static byte[] ToByteArrayFromSqlString(this string data)
        {
            if (data.Length % 2 != 0)
                throw new ArgumentException("Data length must be an even number");

            var toRet = new byte[data.Length / 2];

            var builder = new StringBuilder(data);
            var j = 0;
            for (var i = 0; i < data.Length; i += 2)
            {
                toRet[j] = byte.Parse($"{builder[i]}{builder[i + 1]}", NumberStyles.HexNumber);
                j++;
            }
            return toRet;
        }



        public static SqlColumnInfo[] GetColumnsMappingWithSqlTypes(IList<PropertyInfo> properties)
        {
            var columnsMapping = new SqlColumnInfo[properties.Count];

            var i = 0;
            foreach (var property in properties)
            {
                columnsMapping[i] = new SqlColumnInfo {Name = property.GetColumnName(), SqlType = property.GetSqlType()};
                i++;
            }
            return columnsMapping;
        }

        public static bool AreTablesEqual(IEnumerable<SqlColumnInfo> tab1, IEnumerable<SqlColumnInfo> tab2)
        {
            var table1 = tab1.ToArray();
            var table2 = tab2.ToArray();

            if (table1.Length != table2.Length)
                return false;

            foreach (var t in table1)
            {
                var finded = table2.Select(sqlInf => sqlInf.Name == t.Name && sqlInf.SqlType == t.SqlType);
                if (!finded.Any())
                    return false;
            }

            return true;
        }

        public static string GetSqlView(Type type, object value)
        {
            if (type == typeof(int) || type == typeof(short) || type == typeof(uint) || type == typeof(ushort) || 
                type == typeof(byte) || type == typeof(double) || type == typeof(float))
                return $"{value}";

            if (type == typeof(string) || type == typeof(ulong) || type == typeof(long))
            {
                var str = value.ToString();
                
                var forbidden = new[] { '\'', '\"' };
                if (str.IndexOfAny(forbidden, 0) >= 0)
                    throw new CryptoSQLiteException("Strings that will not be encrypted can't contain symbols like: \' and \". Strings, that will be encrypted can contain any symbols.");

                return $"\'{value}\'";
            }

            if (type == typeof(DateTime))
            {
                var date = (DateTime) value;
                var ticks = date.ToBinary();
                var data = BitConverter.GetBytes(ticks);
                var str = data.ToSqlString();
                return $"\'{str}\'";
            }

            if(type == typeof(bool))
                return $"{Convert.ToByte(value)}";

            throw new CryptoSQLiteException($"Type {type} is not compatible with CryptoSQLite.");
        }

        
    }
}
