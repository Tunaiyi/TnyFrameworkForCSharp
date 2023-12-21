// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace TnyFramework.Net.Application
{

    public class NetAppScope : AppScope<NetAppScope>
    {
        /// <summary>
        /// 上线
        /// </summary>
        public static readonly IAppScope ONLINE = Of(1, "online");

        /// <summary>
        /// 开发
        /// </summary>
        public static readonly IAppScope DEVELOP = Of(2, "develop");

        /// <summary>
        /// 测试
        /// </summary>
        public static readonly IAppScope TEST = Of(3, "test");
    }

}
