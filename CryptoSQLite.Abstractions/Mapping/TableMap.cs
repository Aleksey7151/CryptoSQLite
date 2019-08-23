using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptoSQLite.Mapping
{
    internal class TableMap
    {
        public TableMap(string name, Type tableType, bool hasEncryptedColumns, IDictionary<string, ColumnMap> columns)
        {
            Name = name;
            Type = tableType;
            HasEncryptedColumns = hasEncryptedColumns;
            Columns = columns;
        }

        public string Name { get; }

        public Type Type { get; }

        public bool HasEncryptedColumns { get; }

        public byte[] Key { get; set; }

        public IDictionary<string, ColumnMap> Columns { get; }
    }

    internal static class TableMapExtensions
    {
        public static IList<ForeignKey> ForeignKeys(this IDictionary<string, ColumnMap> columns)
        {
            return columns.Values.Where(cm => cm.IsForeignKey).Select(cm => cm.ForeignKey).ToList();
        }
    }

    internal class ColumnMap
    {
        public ColumnMap(
            string name,
            string propertyName,
            Type clrType,
            string sqlType,
            int columnNumber,
            bool isPrimaryKey,
            bool isAutoIncrement,
            bool isEncrypted,
            bool isNotNull,
            object defaultValue,
            bool isForeignKey,
            ForeignKey foreignKey)
        {
            Name = name;
            PropertyName = propertyName;
            ClrType = clrType;
            SqlType = sqlType;
            ColumnNumber = columnNumber;
            IsPrimaryKey = isPrimaryKey;
            IsAutoIncremental = isAutoIncrement;
            IsEncrypted = isEncrypted;
            IsNotNull = isNotNull;
            DefaultValue = defaultValue;
            IsForeignKey = isForeignKey;
            ForeignKey = foreignKey;
        }

        /// <summary>
        /// Name of column !! IN DATABASE FILE !!. (Name of property can be different)
        /// </summary>
        public string Name { get; }

        public string PropertyName { get; }

        public Type ClrType { get; }

        public string SqlType { get; }

        public int ColumnNumber { get; }

        public bool IsPrimaryKey { get; }

        public bool IsAutoIncremental { get; }

        public bool IsEncrypted { get; }

        public bool IsNotNull { get; }

        public object DefaultValue { get; }

        public bool IsForeignKey { get; }

        public ForeignKey ForeignKey { get; }
    }

    internal interface IValues<in TTable>
    {
        Action<TTable, object> ValueSetter { get; }

        Func<TTable, object> ValueGetter { get; }
    }

    internal class ColumnMap<TTable> : ColumnMap, IValues<TTable>
    {
        public ColumnMap(
            string name,
            string propertyName,
            Type clrType,
            string sqlType,
            int columnNumber,
            bool isPrimaryKey,
            bool isAutoIncrement,
            bool isEncrypted,
            bool isNotNull,
            object defaultValue,
            bool isForeignKey,
            ForeignKey foreignKey, 
            Action<TTable, object> valueSetter,
            Func<TTable, object> valueGetter) 
            : base(
                name,
                propertyName, 
                clrType,
                sqlType,
                columnNumber,
                isPrimaryKey,
                isAutoIncrement,
                isEncrypted,
                isNotNull,
                defaultValue,
                isForeignKey,
                foreignKey)
        {
            ValueSetter = valueSetter;
            ValueGetter = valueGetter;
        }
        
        public Action<TTable, object> ValueSetter { get; }

        public Func<TTable, object> ValueGetter { get; }
    }
}
