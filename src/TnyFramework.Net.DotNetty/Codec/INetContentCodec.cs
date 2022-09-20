// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using DotNetty.Buffers;

namespace TnyFramework.Net.DotNetty.Codec
{

    public interface INetContentCodec
    {
        /// <summary>
        /// 编码
        /// </summary>
        /// <param name="body">消息体</param>
        /// <param name="buffer">写入字节</param>
        void Encode(object body, IByteBuffer buffer);

        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="buffer">读取字节</param>
        /// <returns>解析的消息体</returns>
        object Decode(IByteBuffer buffer);
    }

    public interface INetContentCodec<T> : INetContentCodec
    {
        /// <summary>
        /// 编码
        /// </summary>
        /// <param name="body">消息体</param>
        /// <param name="buffer">写入字节</param>
        void Encode(T body, IByteBuffer buffer);

        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="buffer">读取字节</param>
        /// <returns>解析的消息体</returns>
        new T Decode(IByteBuffer buffer);
    }

}
