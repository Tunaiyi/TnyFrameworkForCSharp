using System;
using System.Threading.Tasks;
using TnyFramework.Net.Message;
namespace TnyFramework.Net.Transport
{
    public class TaskResponseSource : TaskCompletionSource<IMessage>
    {
        public long Timeout { get; }


        public bool IsTimeout(long now)
        {
            return now > Timeout;
        }


        public TaskResponseSource(long timeout)
        {
            Timeout = DateTimeOffset.Now.ToUnixTimeMilliseconds() + timeout;
        }
    }
}
