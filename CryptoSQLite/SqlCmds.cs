using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoSQLite.Mapping;

namespace CryptoSQLite
{
    internal static class SqlCmds
    {
        public static string CmdCreateTable(this TableMap table)
        {
            var cmd = $"CREATE TABLE IF NOT EXISTS \"{table.Name}\"(";

            var cols = table.Columns.Select(c => c.CmdMapColumn());

            var columns = string.Join(",\n", cols);

            cmd += columns + ")";

            return cmd;
        }



        /// <summary>
        /// Maps the column to SQL command, that uses in Table creation
        /// </summary>
        /// <param name="column">ColumnAttribute map</param>
        /// <returns>string with column map</returns>
        public static string CmdMapColumn(this ColumnMap column)
        {
            string clmnMap = $"\"{column.Name}\" {OrmUtils.GetSqlType(column.ColumnType)}";

            if (column.IsPrimaryKey)
                clmnMap += " PRIMARY KEY";
            if (column.IsAutoIncrement)
                clmnMap += " AUTOINCREMENT";
            if (column.IsNotNull)
                clmnMap += " NOT NULL";
            if (column.HasDefaultValue)
                clmnMap += $" DEFAULT \"{column.DefaultValue}\"";

            return clmnMap;
        }
        public static string CmdDeleteTable(string tableName)
        {
            return $"DROP TABLE IF EXISTS \"{tableName}\"";
        }
    }
}
