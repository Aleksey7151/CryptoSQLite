using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptoSQLite.Mapping
{
    internal class TableMap
    {
        public string Name { get; }

        public Type TableType { get; }

        public TableMap(string name, Type tableType)
        {
            Name = name;
            TableType = tableType;
        }

        public List<ColumnMap> Columns { get; set; }

        public IEnumerable<string> GetColumnsName()
        {
            return Columns.Select(col => $"\"{col.Name}\"");
        }
    }
}
