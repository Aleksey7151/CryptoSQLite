using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace CryptoSQLite
{
    internal class PredicateToSql
    {
        private readonly StringBuilder _builder = new StringBuilder();

        private PropertyInfo[] _compatibleProperties;

        private string _tableName;

        private List<object> _values;

        public string WhereToSqlCmd(LambdaExpression whereExpression, string tableName, PropertyInfo[] compatibleProperties, out object[] values)
        {
            if (whereExpression == null)
                throw new ArgumentNullException(nameof(whereExpression), "Predicate can't be null");

            if (compatibleProperties == null)
                throw new ArgumentNullException(nameof(compatibleProperties));

            _tableName = tableName;
            _compatibleProperties = compatibleProperties;

            _values = new List<object>();

            _builder.Clear();

            _builder.Append($"SELECT * FROM {tableName} WHERE ");

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
                var prop = _compatibleProperties.FirstOrDefault(p => p.Name == memberExp.Member.Name);
                if(prop == null)
                    throw new ArgumentException($"Table {_tableName} doesn't contain column with name {memberExp.Member.Name}.");

                _memberAccessLastType = prop.PropertyType;

                // Check forbidden types, they can't be used in Predicate to find items, because they are stored in database in BLOB view.
                if(OrmUtils.ForbiddenTypesInFindRequests.Contains(prop.PropertyType))
                    throw new CryptoSQLiteException("Properties with types 'UInt64', 'Int64', 'DateTime' can't be used in Predicates for finding elements.");

                if(prop.IsEncrypted())
                    throw new CryptoSQLiteException($"You can't use Encrypted columns for finding elements in database. Column {prop.ColumnName()} is Encrypted.");

                _builder.Append(prop.ColumnName());  // set real column name

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
