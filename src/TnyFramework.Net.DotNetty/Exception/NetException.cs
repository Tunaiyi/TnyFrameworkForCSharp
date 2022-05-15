using TnyFramework.Common.Exceptions;

namespace TnyFramework.Net.DotNetty.Exception
{

    public class NetException : CommonException
    {
        public NetException(string message) : base(message)
        {
        }

        public NetException(System.Exception innerException, string message) : base(innerException, message)
        {
        }

        public NetException()
        {
        }
    }

}
