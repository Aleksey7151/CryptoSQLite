using System;
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

        public string WhereToSqlCmd(LambdaExpression whereExpression, string tableName, PropertyInfo[] compatibleProperties)
        {
            if (whereExpression == null)
                throw new ArgumentNullException(nameof(whereExpression));

            if (compatibleProperties == null)
                throw new ArgumentNullException(nameof(compatibleProperties));

            _compatibleProperties = compatibleProperties;

            _builder.Clear();

            _builder.Append($"SELECT * FROM {tableName} WHERE ");

            TranslateExpression(whereExpression);

            _builder.Replace("= NULL", "IS NULL");
            _builder.Replace("<> NULL", "IS NOT NULL");

            return _builder.ToString();
        }

        private Expression TranslateUnaryExpression(UnaryExpression unaryExp)
        {
            if (unaryExp.NodeType != ExpressionType.Not)
                throw new NotSupportedException($"Operator {unaryExp.NodeType} not supported.");

            _builder.Append(" NOT ");

            TranslateExpression(unaryExp.Operand);

            return unaryExp;

        }

        private Expression TranslateConstantExpression(ConstantExpression constExp)
        {
            _builder.Append(constExp.Value == null ? "NULL" : "(?)");

            return constExp;
        }


        private Expression TranslateMemberAccess(MemberExpression memberExp)
        {
            if (memberExp.Expression != null && memberExp.Expression.NodeType == ExpressionType.Parameter)
            {
                //Get real column name:
                var prop = _compatibleProperties.FirstOrDefault(p => p.Name == memberExp.Member.Name);
                if(prop == null)
                    throw new ArgumentException("Table column names are incorect");

                _builder.Append(prop.GetColumnName());  // set real column name

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
