// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Threading;

namespace TnyFramework.Net.Message
{

    public abstract class BaseMessageBody<T> : IOctetMessageBody<T>
    {
        private const int UNRELEASED = 0;
        private const int RELEASED = 1;

        private int released = UNRELEASED;

        private T? body;

        public BaseMessageBody(T body, bool relay)
        {
            this.body = body;
            Relay = relay;
        }

        public bool Relay { get; }

        public T? Body => body;

        object? IOctetMessageBody.Body => body;

        public void Release()
        {
            var current = released;
            if (current == RELEASED)
                return;
            if (Interlocked.CompareExchange(ref released, RELEASED, UNRELEASED) != UNRELEASED)
                return;
            var buffer = body;
            if (buffer == null)
                return;
            body = default;
            DoRelease(buffer);
        }

        protected abstract void DoRelease(T body);
    }

}
