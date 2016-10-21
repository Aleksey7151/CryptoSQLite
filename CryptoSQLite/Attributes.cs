using System;

namespace CryptoSQLite
{
    /// <summary>
    /// This attribute used to specify table, that can contain encrypted colunms
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CryptoTableAttribute : Attribute
    {
        public string Name { get; }

        public CryptoTableAttribute(string name)
        {
            if(name == null)
                throw new ArgumentException("Table name can't be null");

            if(string.IsNullOrEmpty(name))
                throw new ArgumentException("Table name can't be empty");

            Name = name;
        }
    }

    /// <summary>
    /// This attribute used to specify column, that will be encrypted before writting into SQL file.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EncryptedAttribute : Attribute
    {

    }

    /// <summary>
    /// This attribute used to specify column, that won't be encrypted before writting into SQL file.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        public string Name { get; }

        public ColumnAttribute(string name)
        {
            Name = name;
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

    /// <summary>
    /// This attribute used to indicate property that should be ignored by the table mapping
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreAttribute : Attribute
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
