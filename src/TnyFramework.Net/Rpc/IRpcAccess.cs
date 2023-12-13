// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Net.Endpoint;

namespace TnyFramework.Net.Rpc
{

    public interface IRpcAccess
    {
        /// <summary>
        /// 访问点 id
        /// </summary>
        long AccessId { get; }

        /// <summary>
        /// 是否已上线
        /// </summary>
        /// <returns>连接返回true 否则返回false</returns>
        bool IsActive();

        /// <summary>
        /// endpoint
        /// </summary>
        IEndpoint Endpoint { get; }
    }

}
