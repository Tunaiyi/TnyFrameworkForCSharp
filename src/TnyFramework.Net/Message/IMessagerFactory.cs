// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Net.Application;

namespace TnyFramework.Net.Message
{

    public interface IContactFactory
    {
        /// <summary>
        /// 创建 Contact
        /// </summary>
        /// <param name="type">消息者类型</param>
        /// <param name="contactId">消息者id</param>
        /// <typeparam name="TM">返回创建的 contact</typeparam>
        /// <returns></returns>
        IContact CreateContact(IContactType type, long contactId);

        /// <summary>
        /// 创建 Contact
        /// </summary>
        /// <param name="contact">转发消息者</param>
        /// <typeparam name="TM">返回创建的 contact</typeparam>
        /// <returns></returns>
        IContact CreateContact(ForwardContact contact);
    }

}
