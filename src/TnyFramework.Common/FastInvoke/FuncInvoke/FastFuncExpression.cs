using System.Collections.Generic;
using System.Linq.Expressions;

namespace TnyFramework.Common.FastInvoke.FuncInvoke
{

    public class FastFuncExpression
    {
        public ParameterExpression[] ParameterExpressions { get; }

        public Expression BodyExpression { get; }

        public FastFuncExpression(ParameterExpression invoker, IReadOnlyList<ParameterExpression> paramExpressions,
            Expression bodyExpression)
        {
            var hasInvoker = invoker != null ? 1 : 0;
            ParameterExpressions = new ParameterExpression[paramExpressions.Count + hasInvoker];
            if (hasInvoker == 1)
            {
                ParameterExpressions[0] = invoker;
            }
            for (var index = 0; index < paramExpressions.Count; index++)
            {
                ParameterExpressions[index + hasInvoker] = paramExpressions[index];
            }
            BodyExpression = bodyExpression;
        }
    }

}