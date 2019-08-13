using System.Collections.Generic;
using System.Linq;
using CryptoSQLite.Mapping;

namespace CryptoSQLite
{
    internal static class SqlCommands
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
            var enumerable = columns.ToArray();

            var cols = string.Join(", ", enumerable.Select(x => x));
            var values = string.Join(", ", enumerable.Select(x => "?"));
            var cmd = $"INSERT OR REPLACE INTO {tableName} ({cols}) VALUES ({values})";

            return cmd;
        }


        public static string CmdInsert(string tableName, IEnumerable<string> columns)
        {
            var enumerable = columns.ToArray();

            var cols = string.Join(", ", enumerable.Select(x => x));
            var values = string.Join(", ", enumerable.Select(x => "?"));
            var cmd = $"INSERT INTO {tableName} ({cols}) VALUES ({values})";

            return cmd;
        }

        public static string CmdUpdate(string tableName, IEnumerable<string> columns, string predicate)
        {
            var enumerable = columns.Select(c => $"{c} = (?)");
            var cols = string.Join(", ", enumerable);
            var cmd = $"UPDATE {tableName} SET {cols} WHERE {predicate}";

            return cmd;
        }

        public static string CmdSelect(string tableName, string columnName, object columnValue)
        {
            var value = columnValue == null ? "IS NULL" : "= (?)";
            var cmd = $"SELECT * FROM {tableName} WHERE {columnName} {value}";

            return cmd;
        }

        public static string CmdSelectForPredicate(TableMap tableMap, string predicate, string orderByColumnName, SortOrder orderType, int? limitNumber)
        {
            var cmd = $"SELECT * FROM {tableMap.Name} WHERE {predicate} \n";

            if (!string.IsNullOrEmpty(orderByColumnName))
            {
                if (!tableMap.Columns.Keys.Contains(orderByColumnName))
                    throw new CryptoSQLiteException($"Table '{tableMap.Name}' doesn't contain column '{orderByColumnName}'.");

                var type = orderType == SortOrder.Desc ? "DESC" : "ASC";
                var order = $"ORDER BY {orderByColumnName} {type} \n";

                cmd += order;
            }

            if (limitNumber != null)
            {
                if(limitNumber <= 0)
                    throw new CryptoSQLiteException("Limit number can't be less or equal to 0.");
                cmd += $" LIMIT {limitNumber}";
            }

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

        public static string CmdCountForPredicate(string tableName)
        {
            var cmd = $"SELECT COUNT(*) FROM {tableName} WHERE ";

            return cmd;
        }

        /// <summary>
        /// Maps the PropertyInfo to SQL column, that uses in Table creation
        /// </summary>
        /// <param name="column">ColumnAttribute map</param>
        /// <returns>string with column map</returns>
        public static string MapPropertyToColumn(this ColumnMap column)
        {
            string columnMap = $"{column.Name} {column.SqlType}";

            //TODO you need think here a lot

            if (column.IsPrimaryKey && column.IsAutoIncremental)
            {
                columnMap += " PRIMARY KEY AUTOINCREMENT";
            }
            else if (column.IsPrimaryKey)
            {
                columnMap += " PRIMARY KEY NOT NULL";
            }
            else if (column.IsNotNull)
            {
                columnMap += " NOT NULL";
                var defaultValue = column.DefaultValue;
                if (defaultValue != null)
                {
                    columnMap += $" DEFAULT \"{defaultValue}\"";
                }
            }

            return columnMap;
        }
        public static string CmdDeleteTable(string tableName)
        {
            return $"DROP TABLE IF EXISTS \"{tableName}\"";
        }

        public static string CmdDeleteForPredicate(string tableName)
        {
            return $"DELETE FROM {tableName} WHERE ";
        }

        public static string CmdClearTable(string tableName)
        {
            return $"DELETE FROM {tableName}";  // SQLite doesn't have TRUNCATE TABLE command.
        }

        public static string CmdGetTableInfo(string tableName)
        {
            return $"PRAGMA TABLE_INFO(\"{tableName}\")";
        }

        public static string CmdMax(string tableName, string columnName)
        {
            return $"SELECT MAX({columnName}) FROM {tableName}";
        }

        public static string CmdMaxForPredicate(string tableName, string columnName)
        {
            return $"SELECT MAX({columnName}) FROM {tableName} WHERE ";
        }

        public static string CmdMin(string tableName, string columnName)
        {
            return $"SELECT MIN({columnName}) FROM {tableName}";
        }

        public static string CmdMinForPredicate(string tableName, string columnName)
        {
            return $"SELECT MIN({columnName}) FROM {tableName} WHERE ";
        }

        public static string CmdSum(string tableName, string columnName)
        {
            return $"SELECT SUM({columnName}) FROM {tableName}";
        }

        public static string CmdSumForPredicate(string tableName, string columnName)
        {
            return $"SELECT SUM({columnName}) FROM {tableName} WHERE ";
        }

        public static string CmdAvg(string tableName, string columnName)
        {
            return $"SELECT AVG({columnName}) FROM {tableName}";
        }

        public static string CmdAvgForPredicate(string tableName, string columnName)
        {
            return $"SELECT AVG({columnName}) FROM {tableName} WHERE ";
        }

        public static string CmdJoinTwoTables(TableMap table1, TableMap table2, string onJoin, string wherePredicate)
        {
            var columns1 = table1.Columns.Keys.Select(n => $"{table1.Name}.{n}").ToList();
            var columns2 = table2.Columns.Keys.Select(n => $"{table2.Name}.{n}").ToList();

            if (table1.HasEncryptedColumns)
                columns1.Add($"{table1.Name}.{CryptoSQLiteConnection.SoltColumnName}");

            if (table2.HasEncryptedColumns)
                columns2.Add($"{table2.Name}.{CryptoSQLiteConnection.SoltColumnName}");

            var selectedColumns = string.Join(", ", columns1) + ", " + string.Join(", ", columns2);

            var where = !string.IsNullOrEmpty(wherePredicate) ? $" WHERE {wherePredicate}" : "";

            string cmd = $"SELECT {selectedColumns} FROM {table1.Name} INNER JOIN {table2.Name} ON {onJoin}{where}";

            return cmd;
        }

        public static string CmdLeftJoinTwoTables(TableMap tableLeft, TableMap tableRight, string joiningCondition, string wherePredicate)
        {
            var columns1 = tableLeft.Columns.Keys.Select(n => $"{tableLeft.Name}.{n}").ToList();
            var columns2 = tableRight.Columns.Keys.Select(n => $"{tableRight.Name}.{n}").ToList();

            if (tableLeft.HasEncryptedColumns)
                columns1.Add($"{tableLeft.Name}.{CryptoSQLiteConnection.SoltColumnName}");

            if (tableRight.HasEncryptedColumns)
                columns2.Add($"{tableRight.Name}.{CryptoSQLiteConnection.SoltColumnName}");

            var selectedColumns = string.Join(", ", columns1) + ", " + string.Join(", ", columns2);

            var where = !string.IsNullOrEmpty(wherePredicate) ? $" WHERE {wherePredicate}" : "";

            string cmd = $"SELECT {selectedColumns} FROM {tableLeft.Name} LEFT JOIN {tableRight.Name} ON {joiningCondition}{where}";

            return cmd;
        }

        public static string CmdJoinThreeTables(TableMap table1, TableMap table2, TableMap table3, string onJoin12, string onJoin13, string wherePredicate)
        {
            var columns1 = table1.Columns.Keys.Select(n => $"{table1.Name}.{n}").ToList();
            var columns2 = table2.Columns.Keys.Select(n => $"{table2.Name}.{n}").ToList();
            var columns3 = table3.Columns.Keys.Select(n => $"{table3.Name}.{n}").ToList();

            if (table1.HasEncryptedColumns)
                columns1.Add($"{table1.Name}.{CryptoSQLiteConnection.SoltColumnName}");

            if (table2.HasEncryptedColumns)
                columns2.Add($"{table2.Name}.{CryptoSQLiteConnection.SoltColumnName}");

            if (table3.HasEncryptedColumns)
                columns3.Add($"{table3.Name}.{CryptoSQLiteConnection.SoltColumnName}");

            var selectedColumns = string.Join(", ", columns1) + ", " + string.Join(", ", columns2) + ", " + string.Join(", ", columns3);

            var where = !string.IsNullOrEmpty(wherePredicate) ? $" WHERE {wherePredicate}" : "";

            string cmd = $"SELECT {selectedColumns} FROM {table1.Name} INNER JOIN {table2.Name} ON {onJoin12} INNER JOIN {table3.Name} ON {onJoin13}{where}";

            return cmd;
        }

        public static string CmdJoinFourTables(TableMap table1, TableMap table2, TableMap table3, TableMap table4, string onJoin12, string onJoin13, string onJoin14, string wherePredicate)
        {
            var columns1 = table1.Columns.Keys.Select(n => $"{table1.Name}.{n}").ToList();
            var columns2 = table2.Columns.Keys.Select(n => $"{table2.Name}.{n}").ToList();
            var columns3 = table3.Columns.Keys.Select(n => $"{table3.Name}.{n}").ToList();
            var columns4 = table4.Columns.Keys.Select(n => $"{table4.Name}.{n}").ToList();

            if (table1.HasEncryptedColumns)
                columns1.Add($"{table1.Name}.{CryptoSQLiteConnection.SoltColumnName}");

            if (table2.HasEncryptedColumns)
                columns2.Add($"{table2.Name}.{CryptoSQLiteConnection.SoltColumnName}");

            if (table3.HasEncryptedColumns)
                columns3.Add($"{table3.Name}.{CryptoSQLiteConnection.SoltColumnName}");

            if (table4.HasEncryptedColumns)
                columns4.Add($"{table4.Name}.{CryptoSQLiteConnection.SoltColumnName}");

            var selectedColumns = string.Join(", ", columns1) + ", " + string.Join(", ", columns2) + ", " + string.Join(", ", columns3) + ", " + string.Join(", ", columns4);

            var where = !string.IsNullOrEmpty(wherePredicate) ? $" WHERE {wherePredicate}" : "";

            string cmd = $"SELECT {selectedColumns} FROM {table1.Name} INNER JOIN {table2.Name} ON {onJoin12} INNER JOIN {table3.Name} ON {onJoin13} INNER JOIN {table4.Name} ON {onJoin14}{where}";

            return cmd;
        }
    }
}
