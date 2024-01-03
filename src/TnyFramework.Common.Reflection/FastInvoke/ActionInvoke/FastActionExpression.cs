// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Collections.Generic;
using System.Linq.Expressions;

namespace TnyFramework.Common.Reflection.FastInvoke.ActionInvoke
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
