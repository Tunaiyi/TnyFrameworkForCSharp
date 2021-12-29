using System.Threading;
using DotNetty.Buffers;
using DotNetty.Common.Utilities;
using TnyFramework.Net.DotNetty.Common;
namespace TnyFramework.Net.DotNetty.Message
{
    public abstract class BaseMessageBody<T> : IOctetMessageBody<T>
    {
        private const int UNRELEASED = 0;
        private const int RELEASED = 1;

        private int released = UNRELEASED;

        private T body;


        public BaseMessageBody(T body, bool relay)
        {
            this.body = body;
            Relay = relay;
        }


        public bool Relay { get; }

        public T Body => body;

        object IOctetMessageBody.Body => body;


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
