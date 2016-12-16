namespace CryptoSQLite.Mapping
{
    internal class ForeignKey
    {
        public string ReferenceTable { get; }

        public string ReferenceColumn { get; }

        public string ForeignKeyColumnName { get; }

        public ForeignKey(string referenceTable, string referenceColumn, string foreignKeyColumnName)
        {
            ReferenceTable = referenceTable;
            ReferenceColumn = referenceColumn;
            ForeignKeyColumnName = foreignKeyColumnName;
        }

        public bool Equal(ForeignKey fk)
        {
            return ReferenceTable == fk.ReferenceTable && ReferenceColumn == fk.ReferenceColumn &&
                   ForeignKeyColumnName == fk.ForeignKeyColumnName;
        }
    }
}
