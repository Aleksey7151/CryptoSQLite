using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CryptoSQLite.Extensions;

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
        public static Type[] CompatibleTextTypes = { typeof(string) };

        public static Type[] CompatibleRealTypes =
        {
            typeof(double), typeof(float), typeof(double?), typeof(float?)
        };

        public static Type[] CompatibleIntegerTypes =
        {
            typeof(int), typeof(short), typeof(byte), typeof(uint), typeof(ushort), typeof(bool), typeof(int?),
            typeof(short?), typeof(byte?), typeof(uint?), typeof(ushort?), typeof(bool?)
        };
        
        public static Type[] CompatibleBlobTypes =
        {
            typeof(long), typeof(ulong), typeof(DateTime), typeof(byte[]),
            typeof(long?), typeof(ulong?), typeof(DateTime?)
        };

        public static Type[] ForbiddenTypesInFindRequests =
        {
            typeof(long), typeof(ulong), typeof(DateTime)
        };

        public static bool IsForbiddenInFindRequests(this Type type)
        {
            return ForbiddenTypesInFindRequests.Contains(type);
        }

        // In Find requests this types can be used only in equal to null or not equal to null Predicates,
        // because this types stored in database in BLOB type.
        public static Type[] TypesForOnlyNullFindRequests =
        {
            typeof(long?), typeof(ulong?), typeof(DateTime?), typeof(byte[])
        };

        

        public static SqlColumnInfo[] GetColumnsMappingWithSqlTypes(this IList<PropertyInfo> properties)
        {
            var columnsMapping = new SqlColumnInfo[properties.Count];

            var i = 0;
            foreach (var property in properties)
            {
                columnsMapping[i] = new SqlColumnInfo {Name = property.ColumnName(), SqlType = property.SqlType()};
                i++;
            }
            return columnsMapping;
        }

        public static bool IsTablesEqual(IEnumerable<SqlColumnInfo> tab1, IEnumerable<SqlColumnInfo> tab2)
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
