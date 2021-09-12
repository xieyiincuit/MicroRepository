using System.Linq.Expressions;
using XieyiESLibrary.ExpressionsToQuery.Common;

namespace XieyiESLibrary.ExpressionsToQuery
{
    public class BaseResolve
    {
        public BaseResolve(ExpressionParameter parameter)
        {
            Expression = parameter.CurrentExpression;
            Context = parameter.Context;
            BaseParameter = parameter;
        }

        protected Expression Expression { get; set; }
        private Expression ExactExpression { get; set; }
        protected ExpressionContext Context { get; set; }
        protected bool? IsLeft { get; set; }
        private ExpressionParameter BaseParameter { get; }

        public BaseResolve Start()
        {
            var expression = Expression;
            var parameter = new ExpressionParameter
            {
                Context = Context,
                CurrentExpression = expression,
                BaseExpression = ExactExpression,
                BaseParameter = BaseParameter
            };
            return expression switch
            {
                LambdaExpression _ => new LambdaExpressionResolve(parameter),
                BinaryExpression _ => new BinaryExpressionResolve(parameter),
                MethodCallExpression _ => new MethodCallExpressionResolve(parameter),
                MemberExpression memberExpression when memberExpression.Expression.NodeType == ExpressionType.Constant
                    => new MemberConstExpressionResolve(parameter),
                ConstantExpression _ => new ConstantExpressionResolve(parameter),
                MemberExpression _ => new MemberExpressionResolve(parameter),
                _ => null
            };
        }
    }
}