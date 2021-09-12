using System.Linq.Expressions;

namespace XieyiESLibrary.ExpressionsToQuery.Common
{
    public class ExpressionParameter
    {
        public ExpressionContext Context { get; set; }
        public ExpressionParameter BaseParameter { get; set; }
        public Expression BaseExpression { get; set; }

        public Expression CurrentExpression { get; set; }
    }
}