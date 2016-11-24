﻿using System;

namespace CryptoSQLite
{
    /// <summary>
    /// This attribute used to specify table and its name, that can contain encrypted colunms
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CryptoTableAttribute : Attribute
    {
        /// <summary>
        /// Contains crypto table name, that will be used in database file 
        /// </summary>
        public string TableName { get; }
        
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="tableName">Table name</param>
        public CryptoTableAttribute(string tableName)
        {
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
        /// <summary>
        /// Contain column name in table
        /// </summary>
        public string ColumnName { get; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="columnName"> column name</param>
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
        /// <summary>
        /// Default value
        /// </summary>
        public object DefaultValue { get; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="defaultValue">Default value</param>
        public HasDefaultValueAttribute(object defaultValue)
        {
            DefaultValue = defaultValue;
        }
    }
}
