// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace TnyFramework.Net.Session
{

    public enum MessageHandleStrategy
    {
        ///
        /// 处理
        ///
        Handle = 1,

        ///
        /// 忽略
        ///
        Ignore = 2,

        ///
        /// 拦截抛出异常
        ///
        Throw = 3,
    }

    public static class MessageHandleStrategyExtensions
    {
        public static bool IsHandleable(this MessageHandleStrategy value)
        {
            return value == MessageHandleStrategy.Handle;
        }

        public static bool IsThrowable(this MessageHandleStrategy value)
        {
            return value == MessageHandleStrategy.Throw;
        }
    }

}
