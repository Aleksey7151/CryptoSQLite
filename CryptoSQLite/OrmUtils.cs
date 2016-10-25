using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CryptoSQLite.CryptoProviders;
using CryptoSQLite.Mapping;
using SQLitePCL.pretty;

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

        public static string GetSqlType(this Type type)
        {
            if (type == typeof(int) || 
                type == typeof(short) || 
                type == typeof(long) ||
                type == typeof(uint) ||
                type == typeof(ushort) ||
                type == typeof(ulong) ||
                type == typeof(byte) ||
                type == typeof(bool) ||
                type == typeof(DateTime))
                return "integer";
            if (type == typeof(string))
                return "text";
            if (type == typeof(double) || type == typeof(float))
                return "real";

            throw new Exception($"Type {type} is not compatible with CryptoSQLite.");
        }

        public static string PrepareValueForSql(Type type, object value, bool isEncrypted, IEncryptor encryptor)
        {
            if (type == typeof(string))
            {
                var str = value as string;
                if(str == null) throw new ArgumentException("Argument is not compatible with it type", nameof(value));

                var forbidden = new[] { '\'', '\"' };

                if(str.IndexOfAny(forbidden, 0) >= 0)
                    throw new Exception("String can't contain symbols like: \' and \".");

                if (!isEncrypted) return $"\'{value}\'";

                var open = Encoding.UTF8.GetBytes(str);
                var encrypted = encryptor.Encrypt(open);
                var encryptedStr = encrypted.ToSqlString(); //Encoding.UTF8.GetString(encrypted, 0, encrypted.Length);
                var fromSql = encryptedStr.FromSqlString();
                return $"\'{encryptedStr}\'";
            }
            if (type == typeof(bool))
            {
                var byteVal = Convert.ToByte(value);
                return $"{byteVal}";
            }
            if (type == typeof(DateTime))
            {
                var ticks = ((DateTime)value).Ticks;
                if (!isEncrypted) return $"{ticks}";

                var openBytes = BitConverter.GetBytes(ticks);
                var encryptedBytes = encryptor.Encrypt(openBytes);
                var encryptedTicks = BitConverter.ToInt64(encryptedBytes, 0);
                return $"{encryptedTicks}";
            }
            if (type == typeof(short) ||
                type == typeof(int) ||
                type == typeof(long) ||
                type == typeof(ushort) ||
                type == typeof(uint) ||
                type == typeof(ulong))
            {
                if (!isEncrypted) return $"{value}";
                var openVal = (ulong) value;
                var openBytes = BitConverter.GetBytes(openVal);
                var encryptedBytes = encryptor.Encrypt(openBytes);
                var encryptedVal = BitConverter.ToInt64(encryptedBytes, 0);
                return $"{encryptedVal}";
            }
            if (type == typeof(byte))
            {
                return $"{value}";
            }

            throw new Exception($"Type {type} is not compatible with CryptoSQLite");
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

        public static byte[] FromSqlString(this string data)
        {
            if(data.Length %2!=0)
                throw new ArgumentException("Data length must be an even number");

            var toRet = new byte[data.Length/2];

            var builder = new StringBuilder(data);
            var j = 0;
            for (var i = 0; i < data.Length; i += 2)
            {
                var b1 = Convert.ToByte(builder[i]);
                var b2 = Convert.ToByte(builder[i+1]);
                toRet[j] = (byte)(b1*16 + b2);
                j++;
            }
            return toRet;
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

        public static ColumnMap CreateSoltColumn()
        {
            return new ColumnMap(ColumnMap.SoltColumnName, typeof(string), false, null, null, false, false, false, false);
        }


        public static SqlColumnInfo[] GetColumnsMappingWithSqlTypes<TTable>()
        {
            var properties = GetCompatibleProperties<TTable>().ToList();

            var columnsMapping = new SqlColumnInfo[properties.Count + 1];   // for solt column

            var i = 0;
            foreach (var property in properties)
            {
                columnsMapping[i].Name = property.GetColumnName();
                columnsMapping[i].SqlType = property.PropertyType.GetSqlType();
                i++;
            }

            columnsMapping[properties.Count].Name = ColumnMap.SoltColumnName;
            columnsMapping[properties.Count].SqlType = typeof(string).GetSqlType();

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

        
    }
}
