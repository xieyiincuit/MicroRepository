using System.Linq.Expressions;
using XieyiESLibrary.ExpressionsToQuery.Common;

namespace XieyiESLibrary.ExpressionsToQuery
{
    public class LambdaExpressionResolve : BaseResolve
    {
        public LambdaExpressionResolve(ExpressionParameter parameter) : base(parameter)
        {
            var lambda = Expression as LambdaExpression;
            var expression = lambda.Body;
            Expression = expression;
            Start();
        }
    }
}