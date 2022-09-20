// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace TnyFramework.Net.DotNetty.Codec
{

    /// <summary>
    /// 编码加解密器
    /// </summary>
    public interface ICodecCrypto
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="packager">包上下文</param>
        /// <param name="bytes">字节</param>
        /// <param name="offset">开始</param>
        /// <param name="length">长度</param>
        /// <returns>返回加密字节</returns>
        byte[] Encrypt(DataPackageContext packager, byte[] bytes, int offset, int length);

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="packager">包上下文</param>
        /// <param name="bytes">字节</param>
        /// <param name="offset">开始</param>
        /// <param name="length">长度</param>
        /// <returns>返回解密字节</returns>
        byte[] Decrypt(DataPackageContext packager, byte[] bytes, int offset, int length);
    }

    public class NoopCodecCrypto : ICodecCrypto
    {
        public byte[] Encrypt(DataPackageContext packager, byte[] bytes, int offset, int length)
        {
            return bytes;
        }

        public byte[] Decrypt(DataPackageContext packager, byte[] bytes, int offset, int length)
        {
            return bytes;
        }
    }

}
