// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Common.Exceptions;
using TnyFramework.Common.Result;
using TnyFramework.Net.Common;

namespace TnyFramework.Net.DotNetty.Exception
{

    public class NetCodecException : ResultCodeException
    {
        public static NetCodecException CauseEncodeFailed(System.Exception cause, string message)
        {
            return new NetCodecException(NetResultCode.ENCODE_FAILED, cause, message);
        }

        public static NetCodecException CauseEncodeFailed(string message)
        {
            return new NetCodecException(NetResultCode.ENCODE_FAILED, message);
        }

        public static NetCodecException CauseEncodeError(System.Exception cause, string message)
        {
            return new NetCodecException(NetResultCode.ENCODE_ERROR, cause, message);
        }

        public static NetCodecException CauseEncodeError(string message)
        {
            return new NetCodecException(NetResultCode.ENCODE_ERROR, message);
        }

        public static NetCodecException CauseDecodeError(System.Exception cause, string message)
        {
            return new NetCodecException(NetResultCode.DECODE_ERROR, cause, message);
        }

        public static NetCodecException CauseDecodeError(string message)
        {
            return new NetCodecException(NetResultCode.DECODE_ERROR, message);
        }

        public static NetCodecException CauseDecodeFailed(System.Exception cause, string message)
        {
            return new NetCodecException(NetResultCode.DECODE_FAILED, cause, message);
        }

        public static NetCodecException CauseDecodeFailed(string message)
        {
            return new NetCodecException(NetResultCode.DECODE_FAILED, message);
        }

        public static NetCodecException CauseTimeout(string message)
        {
            return new NetCodecException(NetResultCode.PACKET_TIMEOUT, message);
        }

        public static NetCodecException CauseVerify(string message)
        {
            return new NetCodecException(NetResultCode.PACKET_VERIFY_FAILED, message);
        }

        public NetCodecException(IResultCode code, string message = "") : base(code, message)
        {
        }

        public NetCodecException(IResultCode code, System.Exception innerException, string message) : base(code,
            innerException, message)
        {
        }
    }

}
