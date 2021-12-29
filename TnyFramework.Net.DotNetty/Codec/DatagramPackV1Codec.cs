using DotNetty.Common.Utilities;
using Microsoft.Extensions.Logging;
using TnyFramework.Common;
using TnyFramework.Common.Logger;

namespace TnyFramework.Net.DotNetty.Codec
{
    public abstract class DatagramPackV1Codec
    {
        private readonly ILogger logger;

        protected readonly IMessageCodec messageCodec;

        protected readonly ICodecVerifier codecVerifier = new NoopCodecVerifier();

        protected ICodecCrypto codecCrypto = new NoopCodecCrypto();
        
        protected DatagramPackV1Codec(IMessageCodec messageCodec)
        {
            this.messageCodec = messageCodec;
            logger = LogFactory.Logger(GetType());
        }
        
        protected DatagramPackV1Codec(IMessageCodec messageCodec, ICodecVerifier codecVerifier)
        {
            this.messageCodec = messageCodec;
            this.codecVerifier = codecVerifier;
            logger = LogFactory.Logger(GetType());
        }
    }
}
