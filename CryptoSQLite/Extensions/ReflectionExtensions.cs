using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using CryptoSQLite.Mapping;

namespace CryptoSQLite.Extensions
{
    internal static class ReflectionExtensions
    {
        public static bool IsTypeCompatible(this Type type)
        {
            return OrmUtils.CompatibleIntegerTypes.Contains(type) || OrmUtils.CompatibleTextTypes.Contains(type) || 
                   OrmUtils.CompatibleBlobTypes.Contains(type) || OrmUtils.CompatibleRealTypes.Contains(type);
        }

        public static bool IsEncrypted(this PropertyInfo property)
        {
            var attributes = property.GetCustomAttributes<EncryptedAttribute>();
            return attributes.Any();
        }

        public static bool IsPrimaryKey(this PropertyInfo property)
        {
            var attributes = property.GetCustomAttributes<PrimaryKeyAttribute>();
            return attributes.Any();
        }

        public static bool IsAutoIncremental(this PropertyInfo property)
        {
            var attributes = property.GetCustomAttributes<AutoIncrementalAttribute>();
            return attributes.Any();
        }

        public static bool IsIgnorable(this PropertyInfo property)
        {
            var attributes = property.GetCustomAttributes<IgnoredAttribute>();
            return attributes.Any();
        }

        public static bool IsNotNull(this PropertyInfo property)
        {
            var attributes = property.GetCustomAttributes<NotNullAttribute>();
            return attributes.Any();
        }

        public static object DefaultValue(this PropertyInfo property)
        {
            var attribute = property.GetCustomAttribute<NotNullAttribute>();
            return attribute?.DefaultValue;
        }

        public static string ColumnName(this PropertyInfo property)
        {
            var attrs = property.GetCustomAttribute<ColumnAttribute>();
            return attrs == null ? property.Name : attrs.ColumnName;
        }

        public static ForeignKeyAttribute ForeignKey(this PropertyInfo property)
        {
            var attribute = property.GetCustomAttribute<ForeignKeyAttribute>();
            return attribute;
        }
        /*
        public static bool IsForeignKey(this PropertyInfo property)
        {
            var attribute = property.GetCustomAttributes<ForeignKeyAttribute>();
            return attribute.Any();
        }
        */
        /// <summary>
        /// Gets table name using CryptoTable Attribute
        /// </summary>
        /// <param name="table">Type of object</param>
        /// <returns>Name of crypto table</returns>
        public static string TableName(this Type table)
        {
            var tableAttribute = table.GetTypeInfo().GetCustomAttribute<CryptoTableAttribute>();

            if (tableAttribute == null)
                throw new CryptoSQLiteException($"Table {table} doesn't have Custom Attribute: {nameof(CryptoTableAttribute)}.");

            if (string.IsNullOrEmpty(tableAttribute.TableName))
                throw new CryptoSQLiteException("Table name can't be null or empty.");

            return tableAttribute.TableName;
        }

        /// <summary>
        /// Gets SQL type that corresponds to property type
        /// </summary>
        /// <param name="property">Property info</param>
        /// <returns>SQL type</returns>
        public static string SqlType(this PropertyInfo property)
        {
            if (property.IsEncrypted())
                return "BLOB"; // all encrypted types has BLOB QSL type

            var type = property.PropertyType;

            if (OrmUtils.CompatibleIntegerTypes.Contains(type))
                return "INTEGER";

            if (OrmUtils.CompatibleTextTypes.Contains(type))
                return "TEXT";      // by default SQLite can store 1.000.000.000 bytes

            if (OrmUtils.CompatibleRealTypes.Contains(type))
                return "REAL";

            if (OrmUtils.CompatibleBlobTypes.Contains(type))
                return "BLOB";      // by default SQLite can store 1.000.000.000 bytes

            throw new Exception($"Type {type} is not compatible with CryptoSQLite.");
        }

        /// <summary>
        /// Returns all compatible properties that can be used as a columns in SQL database file
        /// </summary>
        /// <param name="tableType"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> CompatibleProperties(this Type tableType)
        {
            // Point of this method is to find those properties, that can be used as columns in table.
            // Only properties, that have public getter and public setter can be used as column in table.

            var compatibleProperties = tableType.GetRuntimeProperties().Where(pr =>
                                                                               pr.PropertyType.IsTypeCompatible() &&
                                                                               pr.CanRead &&
                                                                               pr.CanWrite &&
                                                                               pr.GetMethod != null &&
                                                                               pr.SetMethod != null &&
                                                                               !pr.GetMethod.IsStatic &&
                                                                               !pr.SetMethod.IsStatic &&
                                                                               !pr.IsIgnorable());

            return compatibleProperties;
        }

        public static PropertyInfo NavigationProperty(this Type tableType, string propertyName)
        {
            var navigationTableProperty = tableType.GetRuntimeProperties().FirstOrDefault(pr =>
                                                                               pr.Name == propertyName &&
                                                                               pr.CanRead &&
                                                                               pr.CanWrite &&
                                                                               pr.GetMethod != null &&
                                                                               pr.SetMethod != null &&
                                                                               !pr.GetMethod.IsStatic &&
                                                                               !pr.SetMethod.IsStatic);

            return navigationTableProperty;
        }

        /// <summary>
        /// Creates Value Getter for property using Expressions.
        /// </summary>
        /// <typeparam name="TTable">Type of table for which value getter will be created</typeparam>
        /// <param name="property">property for which value getter will be created</param>
        /// <returns>Value getter</returns>
        public static Func<TTable, object> ValueGetter<TTable>(this PropertyInfo property) where TTable : class 
        {
            if (typeof(TTable) != property.DeclaringType)
                throw new ArgumentException(nameof(property));

            var instance = Expression.Parameter(typeof(TTable), "instance");
            var getPropertyExpr = Expression.Property(instance, property.Name);
            var convertPropertyToObject = Expression.Convert(getPropertyExpr, typeof(object));
            var getter = Expression.Lambda<Func<TTable, object>>(convertPropertyToObject, instance).Compile();
            return getter;
        }

        /// <summary>
        /// Creates Value Setter for property using Expressions 
        /// </summary>
        /// <typeparam name="TTable">Type of table for which value setter will be created</typeparam>
        /// <param name="property">property for which value setter will be created</param>
        /// <returns>Value setter</returns>
        public static Action<TTable, object> ValueSetter<TTable>(this PropertyInfo property) where TTable : class 
        {
            if (typeof(TTable) != property.DeclaringType)
                throw new ArgumentException(nameof(property));

            var instanceParameter = Expression.Parameter(typeof(TTable), "instance");
            var valueParameter = Expression.Parameter(typeof(object), "value");

            var convertValueToPropertyTypeExpress = Expression.Convert(valueParameter, property.PropertyType);
            var assignExpr = Expression.Assign(Expression.Property(instanceParameter, property), convertValueToPropertyTypeExpress);

            //var assignExpr = Expression.Assign(Expression.Property(Expression.Convert(instanceParameter, typeof(TTable)), propertyInfo), Expression.Convert(valueParameter, propertyType));
            var lambda = Expression.Lambda<Action<TTable, object>>(assignExpr, instanceParameter, valueParameter);

            return lambda.Compile();
        }

        /// <summary>
        /// Returns all information about foreign key constraint
        /// </summary>
        /// <typeparam name="TTable">Type of table</typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        public static ForeignKey ForeignKeyInfo<TTable>(this PropertyInfo property)
        {
            if (property.PropertyType != typeof(int) && property.PropertyType != typeof(uint) &&
                property.PropertyType != typeof(short) && property.PropertyType != typeof(ushort))
                throw new CryptoSQLiteException("ForeignKey attribute can be applied only to 'Int32', 'UInt32', 'Int16', 'UInt16' properties, or to property, Type of which has CryptoTable attribute.");

            var table = typeof(TTable);
            var navigationPropertyNameToReferencedTable = property.ForeignKey().NavigationPropertyName;

            if (string.IsNullOrEmpty(navigationPropertyNameToReferencedTable))
                throw new CryptoSQLiteException($"Foreign Key Attribute in property '{property.Name}' can't have empty name.");

            var navigationProperty = table.NavigationProperty(navigationPropertyNameToReferencedTable);
            if (navigationProperty == null)
                throw new CryptoSQLiteException($"Can't find Navigation Property for '{property.Name}'. Check if ForeignKey Attribute has correct Name.");

            var referencedTable = navigationProperty.PropertyType;

            var referencedTableName = referencedTable.TableName();

            var primaryKeyInReferencedTable = referencedTable.GetRuntimeProperties().FirstOrDefault(p => p.IsPrimaryKey());  // check if table has PrimaryKey Attribute
            if (primaryKeyInReferencedTable == null)
                throw new CryptoSQLiteException($"Table {referencedTableName} doesn't contain property with PrimaryKey Attribute.");

            var primaryKeyColumnNameInReferencedTable = primaryKeyInReferencedTable.ColumnName();

            return new ForeignKey(referencedTableName, primaryKeyColumnNameInReferencedTable, property.Name, property.ColumnName(), navigationProperty.Name, referencedTable);
        }
    }
}
