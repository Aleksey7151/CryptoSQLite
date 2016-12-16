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
    /// Defining a FOREIGN KEY constraint.
    /// <para/>You can have a different name of a foreign key property name than the primary key of a dependent class
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ForeignKeyAttribute : Attribute
    {
        /// <summary>
        /// Column name that will be Foreing Key in table 
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Ctor.
        /// <para/>You can have a different name of a foreign key property name than the primary key of a dependent class
        /// </summary>
        /// <param name="name">Foreign key property name. Or name of key property to specify which navigation property it points to.</param>
        public ForeignKeyAttribute(string name)
        {
            Name = name;
        }
    }

    /// <summary>
    /// This attribute used to indicate property that won't be added in table in database file
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoredAttribute : Attribute
    {
        
    }

    /// <summary>
    /// This attribute used to indicate NOT NULL column. If Default value is set, this value will be used as a default value for column. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NotNullAttribute : Attribute
    {
        /// <summary>
        /// Default value that is used when property doesn't have value.
        /// </summary>
        public object DefaultValue { get; }

        /// <summary>
        /// Ctor. Default value will be NULL.
        /// </summary>
        public NotNullAttribute()
        {
            DefaultValue = null;
        }

        /// <summary>
        /// Ctor. The default value will be added to all new records, if no other value is specified
        /// </summary>
        /// <param name="defaultValue">Default value for the property that is used if property doesn't have value</param>
        public NotNullAttribute(object defaultValue)
        {
            DefaultValue = defaultValue;
        }
    }

    /// <summary>
    /// This attribute used to indicate, that column has default value, that must be written to database 
    /// <para/>This Attribute is deprecated. You should use NotNullAttribute instead.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    [Obsolete("This class is deprecated. You should use NotNull Attribute instead.", false)]
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
