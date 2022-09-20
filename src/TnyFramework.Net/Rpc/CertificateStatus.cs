// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;

namespace TnyFramework.Net.Rpc
{

    [Flags]
    public enum CertificateStatus
    {
        /**
		 * 无效的
		 */
        Invalid = 0,

        /**
		 * 未认证
		 */
        Unauthenticated = 1 << 1,

        /**
		 * 已认证
		 */
        Authenticated = (2 << 1) | 1,

        /**
		 * 续约认证
		 */
        Renew = (3 << 1) | 1,
    }

    public static class CertificateStatusExtensions
    {
        public static bool IsAuthenticated(this CertificateStatus status) => ((int) status & 1) == 1;
    }

}
