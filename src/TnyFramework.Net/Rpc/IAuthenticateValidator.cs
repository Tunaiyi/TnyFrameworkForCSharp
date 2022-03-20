using System.Collections.Generic;
using System.Collections.Immutable;
using TnyFramework.Net.Command;
using TnyFramework.Net.Message;
using TnyFramework.Net.Transport;
namespace TnyFramework.Net.Rpc
{
    public interface IAuthenticateValidator
    {
        ICertificate Validate(ITunnel tunnel, IMessage message, ICertificateFactory certificateFactory);

        IList<int> AuthProtocolLimit { get; }
    }

    public interface IAuthenticateValidator<TUserId> : IAuthenticateValidator
    {
        ICertificate<TUserId> Validate(ITunnel<TUserId> tunnel, IMessage message, ICertificateFactory<TUserId> factory);
    }


    public abstract class AuthenticateValidator<TUserId> : IAuthenticateValidator<TUserId>
    {
        protected AuthenticateValidator()
        {
            AuthProtocolLimit = ImmutableList.Create<int>();
        }


        protected AuthenticateValidator(IList<int> authProtocolLimit)
        {
            AuthProtocolLimit = authProtocolLimit == null ? ImmutableList.Create<int>() : ImmutableList.CreateRange(authProtocolLimit);

        }


        public abstract ICertificate<TUserId> Validate(ITunnel<TUserId> tunnel, IMessage message, ICertificateFactory<TUserId> factory);


        public ICertificate Validate(ITunnel tunnel, IMessage message, ICertificateFactory certificateFactory)
        {
            return Validate((ITunnel<TUserId>)tunnel, message, (ICertificateFactory<TUserId>)certificateFactory);
        }


        public IList<int> AuthProtocolLimit { get; }
    }
}
