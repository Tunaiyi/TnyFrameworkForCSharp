using System;
using TnyFramework.Net.Command;
using TnyFramework.Net.Exceptions;
using TnyFramework.Net.Message;
using TnyFramework.Net.Rpc;
using TnyFramework.Net.Transport;
namespace TnyFramework.Net.Demo.Controller
{
    public class DemoAuthenticateValidator : AuthenticateValidator<long>
    {
        public override ICertificate<long> Validate(ITunnel<long> tunnel, IMessage message, ICertificateFactory<long> factory)
        {
            var value = message.Body;
            if (!(value is MessageParamList paramList))
                throw new ValidatorFailException("登录失败");
            var id = (long)paramList[0];
            var userId = (long)paramList[1];
            return factory.Authenticate(id, userId, Certificate.DEFAULT_USER_TYPE, DateTimeOffset.Now.ToUnixTimeMilliseconds());
            // if (value instanceof LoginDTO) {
            //     LoginDTO dto = as(value);
            //     return factory.certificate(dto.getCertId(), dto.getUserId(), Certificates.DEFAULT_USER_TYPE, Instant.now());
            // }
            // if (value instanceof LoginResultDTO) {
            //     LoginResultDTO dto = as(value);
            //     return factory.certificate(System.currentTimeMillis(), dto.getUserId(), Certificates.DEFAULT_USER_TYPE, Instant.now());
            // }
            // System.out.println(value);
        }
    }
}
