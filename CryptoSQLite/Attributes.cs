using System;

namespace CryptoSQLite
{
    /// <summary>
    /// This attribute used to specify table and its name, that can contain encrypted colunms
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CryptoTableAttribute : Attribute
    {
        public string TableName { get; }
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="tableName">Table name</param>
        public CryptoTableAttribute(string tableName)
        {
            if(tableName == null)
                throw new ArgumentException("Table name can't be null.");

            if(string.IsNullOrEmpty(tableName))
                throw new ArgumentException("Table name can't be empty.");

            TableName = tableName;
        }
    }

    /// <summary>
    /// This attribute used to specify column, that will be encrypted before writting into SQL database file.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EncryptedAttribute : Attribute
    {

    }

    /// <summary>
    /// This attribute used to specify column name in table.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        public string ColumnName { get; }

        public ColumnAttribute(string columnName)
        {
            if (columnName == null)
                throw new ArgumentException("Column name can't be null.");

            if (string.IsNullOrEmpty(columnName))
                throw new ArgumentException("Column name can't be empty.");

            ColumnName = columnName;
        }
    }

    /// <summary>
    /// This attribute used to specify column that contains Primary Keys for SQL table
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryKeyAttribute : Attribute
    {
        
    }

    /// <summary>
    /// This attribute used to specify column, that contain auto increment values (incremented by SQL)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoIncrementalAttribute : Attribute
    {
        
    }

    /*
    /// <summary>
    /// This attribute used to specify length of column that contains chars.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MaxLengthAttribute : Attribute
    {
        public int Length { get; }

        public MaxLengthAttribute(int maxLength)
        {
            Length = maxLength;
        }
    }
    */

    /// <summary>
    /// This attribute used to indicate property that won't be added in table in database file
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoredAttribute : Attribute
    {
        
    }

    /// <summary>
    /// This attribute used to indicate, that column can't be NULLABLE 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NotNullAttribute : Attribute
    {
        
    }

    /// <summary>
    /// This attribute used to indicate, that column has default value, that must be written to database 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class HasDefaultValueAttribute : Attribute
    {
        public object DefaultValue { get; }

        public HasDefaultValueAttribute(object defaultValue)
        {
            DefaultValue = defaultValue;
        }
    }
}
