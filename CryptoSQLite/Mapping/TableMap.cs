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

        public TableMap(string name, Type tableType, IEnumerable<PropertyInfo> columns, bool hasEncryptedColumns)
        {
            Name = name;
            TableType = tableType;
            Columns = columns;
            HasEncryptedColumns = hasEncryptedColumns;
        }

        public IEnumerable<PropertyInfo> Columns { get; }
    }
}
