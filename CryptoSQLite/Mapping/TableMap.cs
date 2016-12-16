using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CryptoSQLite.Mapping
{
    internal class TableMap
    {
        public string Name { get; }

        public Type TableType { get; }

        public bool HasEncryptedColumns { get; }

        public IList<ForeignKey> ForeignKeys { get; }

        public TableMap(string name, Type tableType, IEnumerable<PropertyInfo> columns, bool hasEncryptedColumns, IList<ForeignKey> foreignKeys)
        {
            Name = name;
            TableType = tableType;
            Columns = columns;
            HasEncryptedColumns = hasEncryptedColumns;
            ForeignKeys = foreignKeys;
        }

        public IEnumerable<PropertyInfo> Columns { get; }
    }
}
