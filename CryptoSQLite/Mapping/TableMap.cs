using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptoSQLite.Mapping
{
    internal class TableMap
    {
        /// <summary>
        /// Table name in database file
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Determines if table contains encrypted columns
        /// </summary>
        public bool HasEncryptedColumns { get; }

        /// <summary>
        /// Encryption key, that is used for data encryption only for this table
        /// </summary>
        public byte[] Key { get; set; }

        /// <summary>
        /// Dictionary [Key]ColumnName --> [Value]ColumnMap
        /// </summary>
        public IDictionary<string, ColumnMap> Columns { get; }

        public TableMap(string name, bool hasEncryptedColumns, IDictionary<string, ColumnMap> columns)
        {
            Name = name;
            HasEncryptedColumns = hasEncryptedColumns;
            Columns = columns;
        }
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
        public ColumnMap(string name, string propertyName, Type clrType, string sqlType, bool isPrimaryKey, bool isAutoIncrement, bool isEncrypted, bool isNotNull, object defaultValue, bool isForeignKey, ForeignKey foreignKey)
        {
            Name = name;
            PropertyName = propertyName;
            ClrType = clrType;
            SqlType = sqlType;
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

        /// <summary>
        /// Represents the name of corresponding Property
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// CLR data Type of Column
        /// </summary>
        public Type ClrType { get; }

        /// <summary>
        /// SQL data type of column
        /// </summary>
        public string SqlType { get; }

        /// <summary>
        /// Determines if column has PrimaryKey constraint
        /// </summary>
        public bool IsPrimaryKey { get; }

        /// <summary>
        /// Determines if column has AutoIncrement constarint
        /// </summary>
        public bool IsAutoIncremental { get; }

        /// <summary>
        /// Determines if column should be stored in database in encrypted View
        /// </summary>
        public bool IsEncrypted { get; }

        /// <summary>
        /// Determines if column has NOT NULL constrait
        /// </summary>
        public bool IsNotNull { get; }

        /// <summary>
        /// Contains default value for column
        /// </summary>
        public object DefaultValue { get; }

        /// <summary>
        /// Determines if column has ForeignKey Constarait
        /// </summary>
        public bool IsForeignKey { get; }

        /// <summary>
        /// ForeignKey constrait information for this column
        /// </summary>
        public ForeignKey ForeignKey { get; }
    }

    /// <summary>
    /// Interface for working with value getters and setters
    /// </summary>
    /// <typeparam name="TTable"></typeparam>
    internal interface IValues<in TTable>
    {
        /// <summary>
        /// Value Setter for corresponding property
        /// </summary>
        Action<TTable, object> ValueSetter { get; }

        /// <summary>
        /// Value Getter for corresponding property
        /// </summary>
        Func<TTable, object> ValueGetter { get; }
    }

    internal class ColumnMap<TTable> : ColumnMap, IValues<TTable>
    {
        public ColumnMap(string name, string propertyName, Type clrType, string sqlType, bool isPrimaryKey, bool isAutoIncrement, bool isEncrypted, bool isNotNull, object defaultValue, bool isForeignKey, ForeignKey foreignKey, Action<TTable, object> valueSetter, Func<TTable, object> valueGetter) 
            : base(name, propertyName, clrType, sqlType, isPrimaryKey, isAutoIncrement, isEncrypted, isNotNull, defaultValue, isForeignKey, foreignKey)
        {
            ValueSetter = valueSetter;
            ValueGetter = valueGetter;
        }
        
        /// <summary>
        /// Value Setter for corresponding property
        /// </summary>
        public Action<TTable, object> ValueSetter { get; }

        /// <summary>
        /// Value Getter for corresponding property
        /// </summary>
        public Func<TTable, object> ValueGetter { get; }
    }
}
