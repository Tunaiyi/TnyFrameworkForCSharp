// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Common.Extensions;

namespace TnyFramework.Net.Base
{

    public interface IServerBootstrapSetting : INetBootstrapSetting
    {
        /// <summary>
        /// 绑定域名
        /// </summary>
        string Host { get; }

        /// <summary>
        /// 绑定端口
        /// </summary>
        int Port { get; }

        /// <summary>
        /// 协议
        /// </summary>
        string Scheme { get; }
    }

    public static class ServerBootstrapSettingExtensions
    {
        public static string ServiceName(this IServerBootstrapSetting self)
        {
            return self.Name.IsBlank() ? self.ServeName : self.Name;
        }
    }

}
