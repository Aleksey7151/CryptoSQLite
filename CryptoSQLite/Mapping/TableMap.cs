using System;
using System.Collections.Generic;
using System.Reflection;

namespace CryptoSQLite.Mapping
{
    internal class TableMap<TTable>
    {
        public TableMap(string name, bool hasEncryptedColumns, IList<ColumnMap<TTable>> columns)
        {
            Name = name;
            HasEncryptedColumns = hasEncryptedColumns;
            Columns = columns;
        }

        /// <summary>
        /// Table name in database file
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Determines if table contains encrypted columns
        /// </summary>
        public bool HasEncryptedColumns { get; }
        
        /// <summary>
        /// List of columns in Table
        /// </summary>
        public IList<ColumnMap<TTable>> Columns { get; }
    }

    internal class ColumnMap<TTable>
    {
        public ColumnMap(string name, bool isEncrypted, bool isNotNull, object defaultValue, bool isForeignKey, PropertyInfo navigationProperty, Action<TTable, object> valueSetter, Func<TTable, object> valueGetter)
        {
            Name = name;
            IsEncrypted = isEncrypted;
            IsNotNull = isNotNull;
            DefaultValue = defaultValue;
            IsForeignKey = isForeignKey;
            NavigationProperty = navigationProperty;
            ValueSetter = valueSetter;
            ValueGetter = valueGetter;
        }
        /// <summary>
        /// Name of column !! IN DATABASE FILE !!. (Name of property can be different)
        /// </summary>
        public string Name { get; }

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
        /// Name for navigation property, that navigates to referenced table
        /// </summary>
        public PropertyInfo NavigationProperty { get; }

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
