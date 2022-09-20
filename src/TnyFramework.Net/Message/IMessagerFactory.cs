// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Net.Base;

namespace TnyFramework.Net.Message
{

    public interface IMessagerFactory
    {
        /// <summary>
        /// 创建 Messager
        /// </summary>
        /// <param name="type">消息者类型</param>
        /// <param name="messagerId">消息者id</param>
        /// <typeparam name="TM">返回创建的 messager</typeparam>
        /// <returns></returns>
        IMessager CreateMessager(IMessagerType type, long messagerId);

        /// <summary>
        /// 创建 Messager
        /// </summary>
        /// <param name="messager">转发消息者</param>
        /// <typeparam name="TM">返回创建的 messager</typeparam>
        /// <returns></returns>
        IMessager CreateMessager(ForwardMessager messager);
    }

}
