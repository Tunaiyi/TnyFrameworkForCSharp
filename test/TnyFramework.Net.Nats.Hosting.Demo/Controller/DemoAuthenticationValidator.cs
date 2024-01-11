using TnyFramework.Net.Application;
using TnyFramework.Net.Command.Auth;
using TnyFramework.Net.Exceptions;
using TnyFramework.Net.Message;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Nats.Hosting.Demo.Controller;

public class DemoAuthenticationValidator : AuthenticationValidator
{
    public override ICertificate Validate(ITunnel tunnel, IMessage message)
    {
        var value = message.Body;
        if (!(value is MessageParamList paramList))
            throw new AuthFailedException("登录失败");
        var id = (long) paramList[0]!;
        var userId = (long) paramList[1]!;
        return Certificates.CreateAuthenticated(id, userId, userId, NetContactType.DEFAULT_USER, userId);
    }
}