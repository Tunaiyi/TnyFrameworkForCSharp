using TnyFramework.Common.Attribute;
using TnyFramework.Net.Base;
using TnyFramework.Net.Command;

namespace TnyFramework.Net.Transport
{

    public abstract class Communicator<TUserId> : AttributesContext, ICommunicator<TUserId>
    {
        public abstract ICertificate<TUserId> Certificate { get; }

        public TUserId UserId => Certificate.UserId;

        public long MessagerId => Certificate.MessagerId;

        public string UserGroup => Certificate.UserGroup;

        public IMessagerType MessagerType => Certificate.MessagerType;

        public object GetUserId()
        {
            return UserId;
        }

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
