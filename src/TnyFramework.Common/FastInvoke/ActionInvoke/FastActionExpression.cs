using System.Collections.Generic;
using System.Linq.Expressions;

namespace TnyFramework.Common.FastInvoke.ActionInvoke
{

    public class FastActionExpression
    {
        public ParameterExpression[] ParameterExpressions { get; }

        public Expression BodyExpression { get; }

        public FastActionExpression(ParameterExpression invoker, IReadOnlyList<ParameterExpression> paramExpressions, Expression bodyExpression)
        {
            ParameterExpressions = new ParameterExpression[paramExpressions.Count + 1];
            ParameterExpressions[0] = invoker;
            for (var index = 0; index < paramExpressions.Count; index++)
            {
                ParameterExpressions[index + 1] = paramExpressions[index];
            }
            BodyExpression = bodyExpression;
        }
    }

}
