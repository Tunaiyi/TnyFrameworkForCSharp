// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace TnyFramework.Net.Transport
{

    public abstract class RequestContext : MessageContext
    {
        private const long DEFAULT_FUTURE_TIMEOUT = 10000L;

        /// <summary>
        /// 设置消息 body
        /// </summary>
        /// <param name="body"> 消息body</param>
        /// <returns>返回当前 context</returns>
        public RequestContext WithRequestBody(object body)
        {
            WithBody(body);
            return this;
        }

        /// <summary>
        /// 设置响应等待者
        /// </summary>
        /// <returns>返回当前 context</returns>
        public RequestContext WillRespondAwaiter()
        {
            WillRespondAwaiter(DEFAULT_FUTURE_TIMEOUT);
            return this;
        }

        /// <summary>
        /// 设置响应等待者
        /// </summary>
        /// <param name="timeout">超时</param>
        /// <returns>返回当前 context</returns>
        public abstract RequestContext WillRespondAwaiter(long timeout);
    }

}
