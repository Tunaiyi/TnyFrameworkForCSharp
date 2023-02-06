// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Threading;
using TnyFramework.Common.Attribute;

namespace TnyFramework.Net.Command.Dispatcher
{

    public class RpcContexts
    {
        private static readonly AsyncLocal<IRpcProviderContext> LOCAL_CONTEXT = new AsyncLocal<IRpcProviderContext>();

        private static readonly IRpcProviderContext EMPTY = new RpcProviderInvocationContext(null, null, EmptyAttributes.GetEmpty());

        public static IRpcContext Current {
            get {
                var info = LOCAL_CONTEXT.Value;
                if (info != null)
                    return info;
                info = EMPTY;
                LOCAL_CONTEXT.Value = info;
                return info;
            }
        }

        internal static void SetCurrent(IRpcProviderContext context)
        {
            var info = LOCAL_CONTEXT.Value;
            if (info == null || info.IsEmpty())
            {
                LOCAL_CONTEXT.Value = context;
            }
        }

        internal static void Clear()
        {
            var info = LOCAL_CONTEXT.Value;
            if (info != null && !info.IsEmpty())
            {
                LOCAL_CONTEXT.Value = EMPTY;
            }
        }
    }

}
