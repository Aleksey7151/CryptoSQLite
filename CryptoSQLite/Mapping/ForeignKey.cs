using System;

namespace CryptoSQLite.Mapping
{
    internal class ForeignKey
    {
        public ForeignKey(
            string referencedTableName,
            string referencedColumnName,
            string foreignKeyPropertyName,
            string foreignKeyColumnName,
            string navigationPropertyName,
            Type referencedTable,
            bool isAutoResolve)
        {
            ReferencedTableName = referencedTableName;
            ReferencedColumnName = referencedColumnName;
            ForeignKeyPropertyName = foreignKeyPropertyName;
            ForeignKeyColumnName = foreignKeyColumnName;
            NavigationPropertyName = navigationPropertyName;
            TypeOfReferencedTable = referencedTable;
            IsAutoResolve = isAutoResolve;
        }

        public Type TypeOfReferencedTable { get; }

        public string ReferencedTableName { get; }

        public string ReferencedColumnName { get; }

        public string ForeignKeyPropertyName { get; }

        public string ForeignKeyColumnName { get; }

        public string NavigationPropertyName { get; }

        public bool IsAutoResolve { get; }
    }
}
