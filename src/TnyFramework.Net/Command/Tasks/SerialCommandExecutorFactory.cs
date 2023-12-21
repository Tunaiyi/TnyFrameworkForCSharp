// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace TnyFramework.Net.Command.Tasks
{

    public class SerialCommandExecutorFactory : ICommandExecutorFactory
    {
        private readonly ICommandTaskSchedulerFactory taskSchedulerFactory;

        public SerialCommandExecutorFactory()
        {
            taskSchedulerFactory = new CoroutineCommandTaskSchedulerFactory();
        }

        public SerialCommandExecutorFactory(ICommandTaskSchedulerFactory taskSchedulerFactory)
        {
            this.taskSchedulerFactory = taskSchedulerFactory;
        }

        public ICommandExecutor CreateCommandExecutor(CommandBox commandBox)
        {
            var taskScheduler = taskSchedulerFactory.CreateTaskScheduler(commandBox, "CoroutineCommandExecutor");
            return new SerialCommandExecutor(commandBox, taskScheduler);
        }
    }

}
