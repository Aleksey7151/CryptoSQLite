using System.Collections.Generic;
using System.Linq;
using CryptoSQLite.Mapping;

namespace CryptoSQLite
{
    internal static class SqlCmds
    {
        public static string CmdCreateTable(TableMap table, FullTextSearchFlags fullTextSearchFlags = FullTextSearchFlags.None)
        {
            var fts3 = (fullTextSearchFlags & FullTextSearchFlags.FTS3) != 0;
            var fts4 = (fullTextSearchFlags & FullTextSearchFlags.FTS4) != 0;

            var virtualTable = (fts3 || fts4) ? "VIRTUAL " : "";

            var usingFts = fts4 ? " USING FTS4" : fts3 ? " USING FTS3" : "";

            var columns = table.Columns.Values.ToList();

            var cmd = $"CREATE {virtualTable}TABLE IF NOT EXISTS {table.Name}{usingFts}\n(\n";

            var mappedColumns = columns.Select(col => col.MapPropertyToColumn()).ToList();

            if (table.HasEncryptedColumns)
                mappedColumns.Add($"{CryptoSQLiteConnection.SoltColumnName} BLOB");

            var joinedColumns = string.Join(",\n", mappedColumns);

            var foreignKeys = table.Columns.ForeignKeys();
            
            var mappedForeignKeys = string.Join(",\n", foreignKeys.Select(fk => $"FOREIGN KEY({fk.ForeignKeyColumnName}) REFERENCES {fk.ReferencedTableName}({fk.ReferencedColumnName})"));

            if (mappedForeignKeys.Length > 0)
                joinedColumns += ",\n" + mappedForeignKeys;
            
            cmd += joinedColumns + "\n)";

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

        public static string CmdSelect(string tableName, string columnName, object columnValue)
        {
            var value = columnValue == null ? "IS NULL" : "= (?)";

            var cmd = $"SELECT * FROM {tableName} WHERE {columnName} {value}";

            return cmd;
        }

        public static string CmdSelectTop(string tableName)
        {
            var cmd = $"SELECT * FROM {tableName} LIMIT (?)";

            return cmd;
        }

        public static string CmdSelectAllTable(string tableName)
        {
            return $"SELECT * FROM {tableName}";
        }

        public static string CmdDeleteRow(string tableName, string columnName, object columnValue)
        {
            var value = columnValue == null ? "IS NULL" : "= (?)";

            var cmd = $"DELETE FROM {tableName} WHERE {columnName} {value}";

            return cmd;
        }

        public static string CmdCount(string tableName, string columnName = null, bool isDistinct = false)
        {
            var column = columnName ?? "*";

            var distinct = isDistinct ? "DISTINCT " : "";

            var cmd = $"SELECT COUNT({distinct}{column}) FROM {tableName}";

            return cmd;
        }

        /// <summary>
        /// Maps the PropertyInfo to SQL column, that uses in Table creation
        /// </summary>
        /// <param name="column">ColumnAttribute map</param>
        /// <returns>string with column map</returns>
        public static string MapPropertyToColumn(this ColumnMap column)
        {
            string clmnMap = $"{column.Name} {column.SqlType}";

            //TODO you need think here a lot

            if (column.IsPrimaryKey && column.IsAutoIncremental)
            {
                clmnMap += " PRIMARY KEY AUTOINCREMENT";
            }
            else if (column.IsPrimaryKey)
            {
                clmnMap += " PRIMARY KEY NOT NULL";
            }
            else if (column.IsNotNull)
            {
                clmnMap += " NOT NULL";
                var defaultValue = column.DefaultValue;
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

        public static string CmdClearTable(string tableName)
        {
            return $"DELETE FROM {tableName}";  // SQLite doesn't have TRUNCATE TABLE command.
        }

        public static string CmdGetTableInfo(string tableName)
        {
            return $"PRAGMA TABLE_INFO(\"{tableName}\")";
        }
    }
}
