using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;

namespace TnyFramework.Net.DotNetty.Codec
{
    public abstract class DatagramPackV1Codec
    {
        private readonly ILogger logger;

        protected readonly IMessageCodec messageCodec;

        protected ICodecVerifier codecVerifier = new NoopCodecVerifier();

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


        public ICodecVerifier CodecVerifier {
            get => codecVerifier;
            set => codecVerifier = value;
        }

        public ICodecCrypto CodecCrypto {
            get => codecCrypto;
            set => codecCrypto = value;
        }
    }
}
