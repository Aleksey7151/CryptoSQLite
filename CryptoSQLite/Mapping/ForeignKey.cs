using System;

namespace CryptoSQLite.Mapping
{
    internal class ForeignKey
    {
        public Type TypeOfReferencedTable { get; }

        public string ReferencedTableName { get; }

        public string ReferencedColumnName { get; }

        public string ForeignKeyPropertyName { get; }

        /// <summary>
        /// Creates class that contains all data for creating reference to another table using FOREIGN KEY constrait
        /// </summary>
        /// <param name="referencedTableName">Another table name in which <paramref name="foreignKeyPropertyName"/> is PRIMARY KEY</param>
        /// <param name="referencedColumnName">Column name in another table which is PRIMARY KEY column</param>
        /// <param name="foreignKeyPropertyName">Name of property that has KoreignKey Attribute</param>
        /// <param name="referencedTable">Type of referenced Table</param>
        public ForeignKey(string referencedTableName, string referencedColumnName, string foreignKeyPropertyName, Type referencedTable)
        {
            ReferencedTableName = referencedTableName;
            ReferencedColumnName = referencedColumnName;
            ForeignKeyPropertyName = foreignKeyPropertyName;
            TypeOfReferencedTable = referencedTable;
        }

        public bool Equal(ForeignKey fk)
        {
            return ReferencedTableName == fk.ReferencedTableName && ReferencedColumnName == fk.ReferencedColumnName &&
                   ForeignKeyPropertyName == fk.ForeignKeyPropertyName;
        }
    }
}
