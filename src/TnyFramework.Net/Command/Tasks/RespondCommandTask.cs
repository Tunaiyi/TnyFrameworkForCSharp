// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Threading.Tasks;
using TnyFramework.Net.Dispatcher;
using TnyFramework.Net.Message;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Command.Tasks
{

    public class RespondCommandTask : ICommandTask, ICommand
    {
        private readonly IMessage message;

        private readonly TaskResponseSource source;

        public string Name => "RespondCommandTask";

        public RespondCommandTask(IMessage message, TaskResponseSource source)
        {
            this.message = message;
            this.source = source;
        }

        public ICommand Command => this;

        public Task Execute()
        {
            source.SetResult(message);
            return Task.CompletedTask;
        }

        public bool IsDone() => source.Task.IsCompleted;
    }

}
