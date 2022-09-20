// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;

namespace TnyFramework.Net.DotNetty.Codec
{

    public abstract class NetPacketV1Codec
    {
        private readonly ILogger logger;

        protected readonly IMessageCodec messageCodec;

        protected ICodecVerifier codecVerifier = new NoopCodecVerifier();

        protected ICodecCrypto codecCrypto = new NoopCodecCrypto();

        protected NetPacketV1Codec(IMessageCodec messageCodec)
        {
            this.messageCodec = messageCodec;
            logger = LogFactory.Logger(GetType());
        }

        protected NetPacketV1Codec(IMessageCodec messageCodec, ICodecVerifier codecVerifier)
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
