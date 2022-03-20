using TnyFramework.Common.Attribute;
using TnyFramework.Net.Command;
using TnyFramework.Net.Rpc;
namespace TnyFramework.Net.Transport
{
    public abstract class Communicator<TUserId> : AttributesContext, ICommunicator<TUserId>
    {
        public abstract ICertificate<TUserId> Certificate { get; }

        public TUserId UserId => Certificate.UserId;


        public object GetUserId()
        {
            return UserId;
        }


        public string UserType => Certificate.UserType;


        public ICertificate GetCertificate()
        {
            return Certificate;
        }


        public bool IsAuthenticated()
        {
            return Certificate.IsAuthenticated();
        }
    }
}
