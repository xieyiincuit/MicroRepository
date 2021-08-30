using System.Linq.Expressions;
using XieyiESLibrary.ExpressionsToQuery.Common;

namespace XieyiESLibrary.ExpressionsToQuery
{
    public class ConstantExpressionResolve : BaseResolve
    {
        public ConstantExpressionResolve(ExpressionParameter parameter) : base(parameter)
        {
            if (Expression is not ConstantExpression expression) return;
            var value = ExpressionTool.GetValue(expression.Value);
            Context.LastValue = value;
        }
    }
}