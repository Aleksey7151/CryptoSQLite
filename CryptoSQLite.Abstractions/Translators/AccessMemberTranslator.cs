using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CryptoSQLite.Mapping;

namespace CryptoSQLite.Expressions
{
    internal class AccessMemberTranslator
    {
        private static string _columnName;
        private static bool _isEncrypted;
        private static string _propertyName;

        public static string GetColumnName<TTable>(
            Expression<Func<TTable, object>> accessExpression,
            string tableName, 
            ICollection<ColumnMap> mappedColumns,
            out bool isEncrypted,
            out string propertyName)
        {
            TranslateExpression(accessExpression, tableName, mappedColumns);
            isEncrypted = _isEncrypted;
            propertyName = _propertyName;

            return _columnName;
        }

        private static Expression TranslateExpression(
            Expression expression,
            string tableName,
            ICollection<ColumnMap> mappedColumns)
        {
            if (expression == null)
                return null;

            switch (expression.NodeType)
            {                                                   
                case ExpressionType.Convert:
                    return TranslateUnaryExpression((UnaryExpression)expression, tableName, mappedColumns);

                case ExpressionType.MemberAccess:
                    return TranslateMemberAccess((MemberExpression)expression, tableName, mappedColumns);

                case ExpressionType.Lambda:
                    return VisitLambda((LambdaExpression)expression, tableName, mappedColumns);

                default:
                    throw new CryptoSQLiteException($"Not supported Expression type {expression.NodeType}.");
            }
        }

        private static Expression TranslateUnaryExpression(
            UnaryExpression unaryExp,
            string tableName,
            ICollection<ColumnMap> mappedColumns)
        {
            if (unaryExp.NodeType == ExpressionType.Convert)
            {
                TranslateExpression(unaryExp.Operand, tableName, mappedColumns);
            }
            else if (unaryExp.NodeType == ExpressionType.Not)
            {
                TranslateExpression(unaryExp.Operand, tableName, mappedColumns);
            }
            else
            {
                throw new CryptoSQLiteException($"Operator {unaryExp.NodeType} not supported.");
            }

            return unaryExp;
        }

        private static Expression VisitLambda(LambdaExpression lambda, string tableName, ICollection<ColumnMap> mappedColumns)
        {
            var body = TranslateExpression(lambda.Body, tableName, mappedColumns);

            return body != lambda.Body ? Expression.Lambda(lambda.Type, body, lambda.Parameters) : lambda;
        }

        private static Expression TranslateMemberAccess(
            MemberExpression memberExp,
            string tableName,
            IEnumerable<ColumnMap> mappedColumns)
        {
            if (memberExp.Expression == null || memberExp.Expression.NodeType != ExpressionType.Parameter)
                throw new CryptoSQLiteException($"Member {memberExp.Member.Name} is not supported.");
            //Get real column name:
            var column = mappedColumns.FirstOrDefault(col => col.PropertyName == memberExp.Member.Name);
            if (column == null)
                throw new ArgumentException($"Table {tableName} doesn't contain column with name {memberExp.Member.Name}.");

            _columnName = column.Name;  // sets name of column
            _isEncrypted = column.IsEncrypted;
            _propertyName = column.PropertyName;

            return memberExp;
        }
    }
}

