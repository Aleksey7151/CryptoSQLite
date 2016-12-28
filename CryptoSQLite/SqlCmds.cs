using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CryptoSQLite
{
    internal static class SqlCmds
    {
        public static string CmdCreateTable(Type table, FullTextSearchFlags fullTextSearchFlags = FullTextSearchFlags.None)
        {
            var fts3 = (fullTextSearchFlags & FullTextSearchFlags.FTS3) != 0;
            var fts4 = (fullTextSearchFlags & FullTextSearchFlags.FTS4) != 0;

            var virtualTable = (fts3 || fts4) ? "VIRTUAL " : "";

            var usingFts = fts4 ? " USING FTS4" : fts3 ? " USING FTS3" : ""; 

            var columns = table.GetColumns().ToArray();

            var cmd = $"CREATE {virtualTable}TABLE IF NOT EXISTS {table.TableName()}{usingFts}\n(\n";

            var mappedColumns = columns.Select(col => col.MapPropertyToColumn()).ToList();

            if (columns.Any(p => p.IsEncrypted()))
                mappedColumns.Add($"{CryptoSQLiteConnection.SoltColumnName} BLOB");

            var joinedColumns = string.Join(",\n", mappedColumns);

            
            var mappedForeignKeys = string.Join(",\n", columns.ForeignKeys(table).Select(fk => $"FOREIGN KEY({fk.ForeignKeyColumnName}) REFERENCES {fk.ReferencedTableName}({fk.ReferencedColumnName})"));

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
        public static string MapPropertyToColumn(this PropertyInfo column)
        {
            string clmnMap = $"{column.ColumnName()} {column.SqlType()}";

            //TODO you need think here a lot

            if (column.IsPrimaryKey() && column.IsAutoIncremental())
            {
                clmnMap += " PRIMARY KEY AUTOINCREMENT";
            }
            else if (column.IsPrimaryKey())
            {
                clmnMap += " PRIMARY KEY NOT NULL";
            }
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
