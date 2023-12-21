// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Common.Attribute;

namespace TnyFramework.Net.Application
{

    public interface INetAppContext
    {
        /// <summary>
        /// 应用名字
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 应用类型标识
        /// </summary>
        string AppType { get; }

        /// <summary>
        /// 作用域类型标识
        /// </summary>
        string ScopeType { get; }

        /// <summary>
        /// 本地
        /// </summary>
        string Locale { get; }

        /// <summary>
        /// 全局唯一 id
        /// 确保所有的服务器类型的 id 都不重复
        /// </summary>
        int ServerId { get; }

        /// <summary>
        /// 属性
        /// </summary>
        IAttributes Attributes { get; }
    }

}
