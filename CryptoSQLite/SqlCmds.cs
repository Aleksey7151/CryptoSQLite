using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CryptoSQLite.Mapping;

namespace CryptoSQLite
{
    internal static class SqlCmds
    {
        public static string CmdCreateTable(this TableMap table, string soltColumn = null)
        {
            var cmd = $"CREATE TABLE IF NOT EXISTS {table.Name}(";

            var cols = table.Columns.Select(col => col.CmdMapColumn()).ToList();

            if (soltColumn != null)
                cols.Add($"{soltColumn} text");

            var columns = string.Join(",\n", cols);

            cmd += columns + ")";

            return cmd;
        }

        public static string CmdInsertOrReplace(string tableName, IEnumerable<string> columns, IEnumerable<string> values)
        {
            var cols = string.Join(",", columns.Select(x => x));

            var vals = string.Join(",", values.Select(x => x));

            var cmd = $"INSERT OR REPLACE INTO {tableName} ({cols}) VALUES ({vals})";

            return cmd;
        }

        public static string CmdInsert(string tableName, IEnumerable<string> columns, IEnumerable<string> values)
        {
            var cols = string.Join(",", columns.Select(x => x));

            var vals = string.Join(",", values.Select(x => x));

            var cmd = $"INSERT INTO {tableName} ({cols}) VALUES ({vals})";

            return cmd;
        }

        public static string CmdSelect(string tableName, string columnName, object value, Type valueType)
        {
            var cmd = $"SELECT * FROM {tableName} WHERE {columnName} = {OrmUtils.GetSqlView(valueType, value)}";

            return cmd;
        }

        public static string CmdSelectAllTable(string tableName)
        {
            return $"SELECT * FROM {tableName}";
        }

        public static string CmdDeleteRow(string tableName, string columnName, object value, Type valueType)
        {
            var cmd = $"DELETE FROM {tableName} WHERE {columnName} = {OrmUtils.GetSqlView(valueType, value)}";

            return cmd;
        }

        /// <summary>
        /// Maps the PropertyInfo to SQL column, that uses in Table creation
        /// </summary>
        /// <param name="column">ColumnAttribute map</param>
        /// <returns>string with column map</returns>
        public static string CmdMapColumn(this PropertyInfo column)
        {
            string clmnMap = $"{column.GetColumnName()} {column.GetSqlType()}";

            if (column.IsPrimaryKey())
                clmnMap += " PRIMARY KEY";

            if (column.IsAutoIncremental())
                clmnMap += " AUTOINCREMENT";

            if (column.IsNotNullable())
                clmnMap += " NOT NULL";

            if (column.HasDefaultValue())
                clmnMap += $" DEFAULT \"{column.GetDefaultValue()}\"";

            return clmnMap;
        }
        public static string CmdDeleteTable(string tableName)
        {
            return $"DROP TABLE IF EXISTS \"{tableName}\"";
        }

        public static string CmdGetTableInfo(string tableName)
        {
            return $"PRAGMA TABLE_INFO(\"{tableName}\")";
        }
    }
}
