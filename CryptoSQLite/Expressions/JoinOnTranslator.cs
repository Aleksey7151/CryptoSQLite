using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CryptoSQLite.Mapping;

namespace CryptoSQLite.Expressions
{
    internal class JoinOnTranslator
    {
        private readonly List<string> _joinConditions = new List<string>();
        private TableMap _table1;   // Left table
        private TableMap _table2;   // Right table

        public string Translate(LambdaExpression predicate, TableMap table1, TableMap table2)
        {
            _table1 = table1;
            _table2 = table2;

            _joinConditions.Clear();

            TranslateExpression(predicate);

            if(_joinConditions.Count != 2)
                throw new CryptoSQLiteException("Join tables rule must contain only one binary operator: '=='.");

            var array = _joinConditions.ToArray();

            if(array[0] == array[1])
                throw new CryptoSQLiteException("Join tables rule must contain different tables.");

            return array[0] + " = " + array[1];
        }

        private Expression TranslateExpression(Expression expression)
        {
            if (expression == null)
                return null;

            switch (expression.NodeType)
            {
                case ExpressionType.Convert:
                    return TranslateUnaryExpression((UnaryExpression)expression);

                case ExpressionType.MemberAccess:
                    return TranslateMemberAccess((MemberExpression)expression);

                case ExpressionType.Equal:                      //  ==
                    return TranslateBinaryExpression((BinaryExpression)expression);

                case ExpressionType.Lambda:
                    return VisitLambda((LambdaExpression)expression);

                default:
                    throw new CryptoSQLiteException($"Not supported Expression type {expression.NodeType}.");
            }
        }

        private Expression TranslateUnaryExpression(UnaryExpression unaryExp)
        {
            if (unaryExp.NodeType == ExpressionType.Convert || unaryExp.NodeType == ExpressionType.Not)
            {
                TranslateExpression(unaryExp.Operand);
            }
            else
            {
                throw new CryptoSQLiteException($"Operator {unaryExp.NodeType} not supported.");
            }

            return unaryExp;
        }

        private Expression VisitLambda(LambdaExpression lambda)
        {
            var body = TranslateExpression(lambda.Body);

            return body != lambda.Body ? Expression.Lambda(lambda.Type, body, lambda.Parameters) : lambda;
        }

        private Expression TranslateMemberAccess(MemberExpression memberExp)
        {
            if (memberExp.Expression == null || memberExp.Expression.NodeType != ExpressionType.Parameter)
                throw new CryptoSQLiteException($"Member {memberExp.Member.Name} is not supported.");

            var tableType = memberExp.Expression.Type;
            var table = _table1.Type == tableType ? _table1 : _table2;
                
            //Get real column name:
            var column = table.Columns.Values.FirstOrDefault(col => col.PropertyName == memberExp.Member.Name);
            if (column == null)
                throw new ArgumentException($"Table {table.Name} doesn't contain column with name {memberExp.Member.Name}.");

            if(column.IsEncrypted)
                throw new CryptoSQLiteException("Columns that are used in joining expressions can't be Encrypted.");

            _joinConditions.Add($"{table.Name}.{column.Name}");
                
            return memberExp;
        }

        private Expression TranslateBinaryExpression(BinaryExpression binaryExp)
        {
            
            TranslateExpression(binaryExp.Left);

            switch (binaryExp.NodeType)
            {
                case ExpressionType.Equal:
                    break;

                default:
                    throw new CryptoSQLiteException("The relationship between the two tables for joining them must be based on Equal (T1.Id == T2.Id) expression.");
            }

            TranslateExpression(binaryExp.Right);

            return binaryExp;
        }
    }
}
