// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Threading.Tasks;

namespace TnyFramework.Net.Command.Dispatcher
{

    public interface ICommand
    {
        /// <summary>
        /// 执行
        /// </summary>
        Task Execute();

        /// <summary>
        /// 是否成功完成
        /// </summary>
        /// <returns></returns>
        bool IsDone();

        /// <summary>
        /// 命令名字
        /// </summary>
        string Name { get; }
    }

    public abstract class BaseCommand : ICommand
    {
        private bool done;

        protected BaseCommand()
        {
            Name = GetType().Name;
        }

        protected BaseCommand(string name)
        {
            Name = name;
        }

        public string Name { get; }

        /// <summary>
        /// 执行
        /// </summary>
        public async Task Execute()
        {
            try
            {
                await Action();
            } finally
            {
                done = true;
            }
        }

        protected abstract Task Action();

        /// <summary>
        /// 是否成功完成
        /// </summary>
        /// <returns></returns>
        public bool IsDone() => done;
    }

}
