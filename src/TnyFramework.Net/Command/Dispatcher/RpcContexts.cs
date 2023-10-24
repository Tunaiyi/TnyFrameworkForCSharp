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

    public static class RpcContexts
    {
        private static readonly AsyncLocal<IRpcEnterContext> LOCAL_CONTEXT = new AsyncLocal<IRpcEnterContext>();

        private static readonly IRpcEnterContext EMPTY = new RpcEnterInvocationContext(null!, null!, false, EmptyAttributes.GetEmpty());

        public static IRpcContext Current {
            get {
                var info = LOCAL_CONTEXT.Value;
                if (info != null)
                    return info;
                info = EMPTY;
                info.Resume();
                LOCAL_CONTEXT.Value = info;
                return info;
            }
        }

        internal static void SetCurrent(IRpcEnterContext context)
        {
            var info = LOCAL_CONTEXT.Value;
            if (info is not {Valid: true})
                return;
            info.Suspend();
            LOCAL_CONTEXT.Value = context;
        }

        internal static void Clear()
        {
            var info = LOCAL_CONTEXT.Value;
            if (info is {Valid: false})
            {
                LOCAL_CONTEXT.Value = EMPTY;
            }
        }
    }

}
