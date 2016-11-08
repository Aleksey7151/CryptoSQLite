using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CryptoSQLite
{
    internal class SqlColumnInfo
    {
        public string Name { get; set; }
        public string SqlType { get; set; }
        public object SqlValue { get; set; }
    }

    internal static class OrmUtils
    {
        public static Type[] CompatibleTypes =
        {
            typeof(DateTime), typeof(bool), typeof(string), typeof(byte[]), typeof(byte),
            typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong),
            typeof(float), typeof(double)
        };

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
            if (property.IsEncrypted())
                return "BLOB"; // ll encrypted types has BLOB QSL type

            var type = property.PropertyType;

            if (type == typeof(int) || type == typeof(short) || type == typeof(byte) ||
                type == typeof(uint) || type == typeof(ushort) || type == typeof(bool))
                return "INTEGER";

            if (type == typeof(string))
                return "TEXT";

            if (type == typeof(double) || type == typeof(float))
                return "REAL";

            if (type == typeof(long) || type == typeof(ulong) || type == typeof(DateTime) || type == typeof(byte[]))
                return "BLOB";

            throw new Exception($"Type {type} is not compatible with CryptoSQLite.");
        }



        public static IEnumerable<PropertyInfo> GetCompatibleProperties<TTable>()
        {
            // Point of this method is to find those properties, that can be used as column in table.
            // Only properties, that have public getter and public setter can be used as column in table.

            var type = typeof(TTable);

            var compatibleProperties = type.GetRuntimeProperties().Where(pr => pr.CanRead &&
                                                                               pr.CanWrite &&
                                                                               pr.GetMethod != null &&
                                                                               pr.SetMethod != null &&
                                                                               !pr.GetMethod.IsStatic &&
                                                                               !pr.SetMethod.IsStatic &&
                                                                               !pr.IsIgnorable());

            return compatibleProperties;
        }


        public static bool IsDefaultValue(this PropertyInfo property, object value)
        {
            if (property.PropertyType.IsPointer && value == null)
                return true;

            try
            {
                var intVal = Convert.ToInt32(value);
                if (intVal == 0)
                    return true;
            }
            catch (Exception)
            {
                return false;
            }

            return false;
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

        public static object GetSqlViewFromClr(Type type, object value)
        {
            if (type == typeof(int)  || type == typeof(short)  || type == typeof(double)|| type == typeof(byte) || 
                type == typeof(uint) || type == typeof(ushort) || type == typeof(float) || type == typeof(byte[]))
                return value;

            if (type == typeof(string))
                return value;

            if (type == typeof(long))
                return BitConverter.GetBytes((long) value);
            
            if (type == typeof(ulong))
                return BitConverter.GetBytes((ulong) value);

            if (type == typeof(DateTime))
            {
                var date = (DateTime) value;
                var ticks = date.ToBinary();
                return BitConverter.GetBytes(ticks);
            }

            if(type == typeof(bool))
                return Convert.ToInt32(value);

            throw new CryptoSQLiteException($"Type {type} is not compatible with CryptoSQLite.");
        }

        public static void GetClrViewFromSql(PropertyInfo property, object item, object sqlValue)
        {
            var type = property.PropertyType;

            if (type == typeof(string))
                property.SetValue(item, sqlValue);
            else if (type == typeof(DateTime))
            {
                var bytes = (byte[])sqlValue;
                var ticks = BitConverter.ToInt64(bytes, 0);
                var date = DateTime.FromBinary(ticks);
                property.SetValue(item, date);
            }
            else if (type == typeof(short))
                property.SetValue(item, Convert.ToInt16(sqlValue));
            else if (type == typeof(ushort))
                property.SetValue(item, Convert.ToUInt16(sqlValue));
            else if (type == typeof(int))
                property.SetValue(item, Convert.ToInt32(sqlValue));
            else if (type == typeof(uint))
                property.SetValue(item, Convert.ToUInt32(sqlValue));
            else if (type == typeof(long))
            {
                var value = BitConverter.ToInt64((byte[])sqlValue, 0);
                property.SetValue(item, value);
            }
            else if (type == typeof(ulong))
            {
                var value = BitConverter.ToUInt64((byte[]) sqlValue, 0);
                property.SetValue(item, value);
            }
            else if (type == typeof(byte))
                property.SetValue(item, Convert.ToByte(sqlValue));
            else if (type == typeof(bool))
                property.SetValue(item, Convert.ToBoolean(sqlValue));
            else if (type == typeof(double))
                property.SetValue(item, Convert.ToDouble(sqlValue));
            else if (type == typeof(float))
                property.SetValue(item, Convert.ToSingle(sqlValue));
            else if (type == typeof(byte[]))
                property.SetValue(item, (byte[])sqlValue);
        }
    }
}
