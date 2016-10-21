using System;
using System.Reflection;

namespace CryptoSQLite.Mapping
{
    internal class ColumnMap
    {
        public string Name { get; }

        public Type ColumnType { get; }

        public object DefaultValue { get; }

        public bool HasDefaultValue { get; }

        public PropertyInfo InfoAboutProperty { get; }

        public bool IsPrimaryKey { get; }

        public bool IsAutoIncrement { get; }

        public bool IsEncrypted { get; }

        public bool IsNotNull { get; }

        public ColumnMap(string name, Type columnType, bool hasDefaultValue, object defaultValue, PropertyInfo propertyInfo, bool isPrimaryKey,
            bool isAutoIncrement, bool isNotNull, bool isEncrypted)
        {
            Name = name;
            ColumnType = columnType;
            HasDefaultValue = hasDefaultValue;
            DefaultValue = defaultValue;
            InfoAboutProperty = propertyInfo;
            IsPrimaryKey = isPrimaryKey;
            IsAutoIncrement = isAutoIncrement;
            IsNotNull = isNotNull;
            IsEncrypted = isEncrypted;
        }
    }
}
