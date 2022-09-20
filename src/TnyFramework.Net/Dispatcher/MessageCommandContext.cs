// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using TnyFramework.Common.Result;
using TnyFramework.Net.Command;

namespace TnyFramework.Net.Dispatcher
{

    public class MessageCommandContext : IMessageCommandContext
    {
        public MessageCommandContext(MethodControllerHolder controller)
        {
            Controller = controller;
        }

        public MethodControllerHolder Controller { get; }

        public string Name => Controller.Name;

        public object Result { get; private set; }

        public Exception Cause { get; private set; }

        public bool Done { get; private set; }

        public void Complete(object result)
        {
            if (Done)
            {
                return;
            }
            if (result is Exception cause)
            {
                Cause = cause;
                Done = true;
            } else
            {
                Result = result;
                Done = true;
            }
        }

        public void Intercept(IRpcResult result)
        {

            Complete(result);
        }

        public void Intercept(IResultCode code)
        {
            Complete(code);
        }

        public void Intercept(IResultCode code, object body)
        {
            Complete(RpcResults.Result(code, body));
        }
    }

}
