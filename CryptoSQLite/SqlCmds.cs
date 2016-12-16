using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CryptoSQLite.Mapping;

namespace CryptoSQLite
{
    internal static class SqlCmds
    {
        public static string CmdCreateTable(TableMap table, string soltColumn = null)
        {
            var cmd = $"CREATE TABLE IF NOT EXISTS {table.Name}(";

            var cols = table.Columns.Select(col => col.CmdMapColumn()).ToList();

            if (soltColumn != null)
                cols.Add($"{soltColumn} BLOB");

            var columns = string.Join(",\n", cols);

            cmd += columns + ")";

            return cmd;
        }


        public static string CmdInsertOrReplace(string tableName, IEnumerable<string> columns)
        {
            var enumerable = columns as string[] ?? columns.ToArray();

            var cols = string.Join(",", enumerable.Select(x => x));

            var vals = string.Join(",", enumerable.Select(x => "?"));

            var cmd = $"INSERT OR REPLACE INTO {tableName} ({cols}) VALUES ({vals})";

            return cmd;
        }


        public static string CmdInsert(string tableName, IEnumerable<string> columns)
        {
            var enumerable = columns as string[] ?? columns.ToArray();

            var cols = string.Join(",", enumerable.Select(x => x));

            var vals = string.Join(",", enumerable.Select(x => "?"));

            var cmd = $"INSERT INTO {tableName} ({cols}) VALUES ({vals})";

            return cmd;
        }

        public static string CmdSelect(string tableName, string columnName)
        {
            var cmd = $"SELECT * FROM {tableName} WHERE {columnName} = (?)";

            return cmd;
        }

        public static string CmdSelectAllTable(string tableName)
        {
            return $"SELECT * FROM {tableName}";
        }

        public static string CmdFindInTable(string tableName, string columnName, object lower, object upper)
        {
            if (lower != null && upper != null)
            {
                return $"SELECT * FROM {tableName} WHERE {columnName} BETWEEN (?) AND (?)";
            }
            if (lower != null)
            {
                return $"SELECT * FROM {tableName} WHERE {columnName} >= (?)";
            }
            if (upper != null)
            {
                return $"SELECT * FROM {tableName} WHERE {columnName} <= (?)";
            }
            return $"SELECT * FROM {tableName}";    // all table
        }

        public static string CmdFindNullInTable(string tableName, string columnName)
        {
            return $"SELECT * FROM {tableName} WHERE {columnName} IS NULL";
        }

        public static string CmdDeleteRow(string tableName, string columnName)
        {
            var cmd = $"DELETE FROM {tableName} WHERE {columnName} = (?)";

            return cmd;
        }

        /// <summary>
        /// Maps the PropertyInfo to SQL column, that uses in Table creation
        /// </summary>
        /// <param name="column">ColumnAttribute map</param>
        /// <returns>string with column map</returns>
        public static string CmdMapColumn(this PropertyInfo column)
        {
            string clmnMap = $"{column.ColumnName()} {column.GetSqlType()}";

            if (column.IsPrimaryKey() && column.IsAutoIncremental())
                clmnMap += " PRIMARY KEY AUTOINCREMENT";
            else if(column.IsPrimaryKey() && !column.IsAutoIncremental())
                clmnMap += " PRIMARY KEY NOT NULL";
            else if (column.IsNotNull())
            {
                clmnMap += " NOT NULL";
                var defaultValue = column.DefaultValue();
                if (defaultValue != null)
                {
                    clmnMap += $" DEFAULT \"{defaultValue}\"";
                }
            }

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
