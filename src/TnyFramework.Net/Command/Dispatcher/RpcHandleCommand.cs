// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Threading.Tasks;

namespace TnyFramework.Net.Command.Dispatcher
{

    public abstract class RpcHandleCommand : ICommand
    {

        protected readonly IRpcProviderContext rpcContext;

        private Exception cause;

        private bool Done { get; set; }

        protected RpcHandleCommand(IRpcProviderContext rpcContext)
        {
            this.rpcContext = rpcContext;
        }

        public async Task Execute()
        {
            try
            {
                await OnRun();
                Done = true;
            } catch (Exception e)
            {
                cause = e;
                OnException(e);
            } finally
            {
                OnDone(cause);
            }
        }

        protected abstract Task OnRun();

        protected abstract void OnDone(Exception cause);

        protected abstract void OnException(Exception cause);

        public bool IsDone()
        {
            return Done;
        }

        public abstract string Name { get; }
    }

}
