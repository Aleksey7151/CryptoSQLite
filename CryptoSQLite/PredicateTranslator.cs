﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using CryptoSQLite.Extensions;
using CryptoSQLite.Mapping;

namespace CryptoSQLite
{
    internal class PredicateTranslator
    {
        private readonly StringBuilder _builder = new StringBuilder();

        private ColumnMap[] _mappedColumns;

        private string _tableName;

        private readonly List<object> _values = new List<object>();

        public string DeleteToSqlCmd(LambdaExpression deleteExpression, string tableName, ICollection<ColumnMap> mappedColumns, out object[] values)
        {
            _tableName = tableName;

            _mappedColumns = mappedColumns.ToArray();

            _values.Clear();

            _builder.Clear();

            _builder.Append($"DELETE FROM {tableName} WHERE ");

            TranslateExpression(deleteExpression);

            _builder.Replace("= NULL", "IS NULL");

            _builder.Replace("<> NULL", "IS NOT NULL");

            values = _values.ToArray();

            return _builder.ToString();
        }

        public string WhereToSqlCmd(LambdaExpression whereExpression, string tableName, ICollection<ColumnMap> mappedColumns, out object[] values, string[] selectedPropertyNames = null)
        {
            _tableName = tableName;
            _mappedColumns = mappedColumns.ToArray();

            _values.Clear();

            _builder.Clear();

            if (selectedPropertyNames != null && selectedPropertyNames.Length > 0)      // if selected columns defined, then take only them
            {
                IList<string> columnNames = new List<string>();
                var hasEncrypted = false;
                foreach (var propertyName in selectedPropertyNames)
                {
                    if(string.IsNullOrEmpty(propertyName))
                        throw new CryptoSQLiteException("Property Name for 'Select' can't be Null or Empty.");

                    var clmn = mappedColumns.FirstOrDefault(cp => cp.PropertyName == propertyName); // if wrong property name passed

                    if (clmn == null)
                        throw new CryptoSQLiteException($"Table '{tableName}' doesn't contain property with name: '{propertyName}'.");

                    if (clmn.IsEncrypted)       // we must read SoltColumn from database only if onle of selected properties has Encrypted attribute
                        hasEncrypted = true;

                    columnNames.Add(clmn.Name);
                }

                if(hasEncrypted)
                    columnNames.Add(CryptoSQLiteConnection.SoltColumnName);

                var joinedColumnNames = string.Join(", ", columnNames);

                _builder.Append($"SELECT {joinedColumnNames} FROM {tableName} WHERE ");
            }

            else
                _builder.Append($"SELECT * FROM {tableName} WHERE ");   // take all columns

            TranslateExpression(whereExpression);

            _builder.Replace("= NULL", "IS NULL");
            _builder.Replace("<> NULL", "IS NOT NULL");

            values = _values.ToArray();

            return _builder.ToString();
        }

        private Expression TranslateUnaryExpression(UnaryExpression unaryExp)
        {
            if (unaryExp.NodeType == ExpressionType.Convert)
            {
                TranslateExpression(unaryExp.Operand);
            }
            else if (unaryExp.NodeType == ExpressionType.Not)
            {
                _builder.Append(" NOT ");
                TranslateExpression(unaryExp.Operand);
            }
            else
                throw new NotSupportedException($"Operator {unaryExp.NodeType} not supported.");

            return unaryExp;

        }


        private Expression TranslateConstantExpression(ConstantExpression constExp)
        {
            _builder.Append(constExp.Value == null ? "NULL" : "(?)");
            if (constExp.Value != null)
            {
                if(OrmUtils.TypesForOnlyNullFindRequests.Contains(_memberAccessLastType))
                    throw new CryptoSQLiteException("Properties with types 'UInt64?', 'Int64?', 'DateTime?' or 'Byte[]' can be used only in Equal To NULL (==null) or Not Equal To NULL (!=null) Predicate statements.");
                
                // Add only NOT NULL values, because NULL values written as IS NULL or IS NOT NULL in SQL request.
                _values.Add(constExp.Value);
            }
            return constExp;
        }

        private Type _memberAccessLastType;
        private Expression TranslateMemberAccess(MemberExpression memberExp)
        {
            if (memberExp.Expression != null && memberExp.Expression.NodeType == ExpressionType.Parameter)
            {
                //Get real column name:
                var column = _mappedColumns.FirstOrDefault(col => col.PropertyName == memberExp.Member.Name);
                if(column == null)
                    throw new ArgumentException($"Table {_tableName} doesn't contain column with name {memberExp.Member.Name}.");

                _memberAccessLastType = column.ClrType;

                // Check forbidden types, they can't be used in Predicate to find items, because they are stored in database in BLOB view.
                if(column.ClrType.IsForbiddenInFindRequests())
                    throw new CryptoSQLiteException("Properties with types 'UInt64', 'Int64', 'DateTime' can't be used in Predicates for finding elements.");

                if(column.IsEncrypted)
                    throw new CryptoSQLiteException($"You can't use Encrypted columns for finding elements in database. Column '{column.Name}' is Encrypted.");

                _builder.Append(column.Name);  // choose real column name

                return memberExp;
            }
            throw new NotSupportedException($"Member {memberExp.Member.Name} is not supported.");
        }

        protected Expression TranslateBinaryExpression(BinaryExpression binaryExp)
        {
            _builder.Append("(");

            TranslateExpression(binaryExp.Left);

            switch (binaryExp.NodeType)
            {
                case ExpressionType.Equal:
                    _builder.Append(" = ");
                    break;

                case ExpressionType.NotEqual:
                    _builder.Append(" <> ");
                    break;

                case ExpressionType.LessThan:
                    _builder.Append(" < ");
                    break;

                case ExpressionType.LessThanOrEqual:
                    _builder.Append(" <= ");
                    break;

                case ExpressionType.GreaterThan:
                    _builder.Append(" > ");
                    break;

                case ExpressionType.GreaterThanOrEqual:
                    _builder.Append(" >= ");
                    break;

                case ExpressionType.AndAlso:
                    _builder.Append(" AND ");
                    break;

                case ExpressionType.OrElse:
                    _builder.Append(" OR ");
                    break;

                default:
                    throw new NotSupportedException("");

            }

            TranslateExpression(binaryExp.Right);

            _builder.Append(")");

            return binaryExp;
        }

        protected virtual Expression VisitLambda(LambdaExpression lambda)
        {

            Expression body = TranslateExpression(lambda.Body);

            return body != lambda.Body ? Expression.Lambda(lambda.Type, body, lambda.Parameters) : lambda;
        }

        private Expression TranslateExpression(Expression expression)
        {
            if (expression == null)
                return null;

            switch (expression.NodeType)
            {                                                   //  SQL Commands:
                case ExpressionType.Not:                        //  NOT
                case ExpressionType.Convert:
                    return TranslateUnaryExpression((UnaryExpression)expression);

                case ExpressionType.Equal:                      //  ==
                case ExpressionType.NotEqual:                   //  <>
                case ExpressionType.LessThan:                   //  <
                case ExpressionType.LessThanOrEqual:            //  <=
                case ExpressionType.GreaterThan:                //  >
                case ExpressionType.GreaterThanOrEqual:         //  >=
                case ExpressionType.AndAlso:                    //  AND
                case ExpressionType.OrElse:                     //  OR
                    return TranslateBinaryExpression((BinaryExpression)expression);

                case ExpressionType.MemberAccess:
                    return TranslateMemberAccess((MemberExpression)expression);

                case ExpressionType.Constant:
                    return TranslateConstantExpression((ConstantExpression)expression);

                case ExpressionType.Lambda:
                    return VisitLambda((LambdaExpression)expression);

                default:
                    throw new NotSupportedException($"Not supported Expression type {expression.NodeType}.");
            }
        }
    }
}