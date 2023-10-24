// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;

namespace TnyFramework.Net.Message
{

    public interface IMessageHeaderContainer
    {
        /// <summary>
        /// 获取转发 header
        /// </summary>
        /// <param name="key">查找 key</param>
        /// <returns>Header</returns>
        MessageHeader? GetHeader(string key);

        /// <summary>
        /// 获取转发 header
        /// </summary>
        T? GetHeader<T>(string key) where T : MessageHeader;

        /// <summary>
        /// 获取转发 header
        /// </summary>
        /// <param name="key">查找 key</param>
        /// <param name="type">Header 类</param>
        /// <returns>Header</returns>
        MessageHeader? GetHeader(string key, Type type);

        /// <summary>
        /// 获取转发 header
        /// </summary>
        IList<T> GetHeaders<T>() where T : MessageHeader;

        /// <summary>
        /// 获取转发 header
        /// </summary>
        /// <param name="type">Header 类</param>
        /// <returns>Header列表</returns>
        IList<MessageHeader> GetHeaders(Type type);

        /// <summary>
        /// 获取转发 header
        /// </summary>
        T? GetHeader<T>(MessageHeaderKey<T> key) where T : MessageHeader<T>;

        /// <summary>
        /// 获取全部 Header
        /// </summary>
        /// <return>获取全部 Header</return>
        bool IsHasHeaders();

        /// <summary>
        /// 获取全部 Header
        /// </summary>
        /// <return>获取全部 Header</return>
        IList<MessageHeader> GetAllHeaders();

        /// <summary>
        /// 获取全部 Header
        /// </summary>
        /// <return>获取全部 Header</return>
        IDictionary<string, MessageHeader> GetAllHeaderMap();

        /// <summary>
        /// 是否是转发
        /// </summary>
        /// <return>是否是转发</return>
        bool IsForward();

        /// <summary>
        /// 获取转发头
        /// </summary>
        RpcForwardHeader? ForwardHeader { get; }

        /// <summary>
        /// 是否存在指定 key 的 Header
        /// </summary>
        /// <param name="key">键值</param>
        /// <return>存在返回 true</return>
        bool ExistHeader(string key);

        /// <summary>
        /// 是否存在指定 key 的 Header
        /// </summary>
        /// <param name="key">键值</param>
        /// <return>否则返回 false</return>
        bool ExistHeader<T>(string key) where T : MessageHeader<T>;

        /// <summary>
        /// 是否存在指定 key 的 Header
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns>存在返回 true, 否则返回 false</returns>
        bool ExistHeader(MessageHeaderKey key);

        /// <summary>
        /// 是否存在指定 key 的 Header
        /// </summary>
        /// <param name="key">键值</param>
        /// <return>否则返回 false</return>
        bool ExistHeader<T>(MessageHeaderKey<T> key) where T : MessageHeader<T>;

        /// <summary>
        /// 添加 header
        /// </summary>
        /// <param name="header">头部信息</param>
        /// <return>返回 context 自身</return>
        T? PutHeader<T>(MessageHeader<T> header) where T : MessageHeader<T>;

        /// <summary>
        /// 添加 header, 如果Header 不存在.
        /// </summary>
        /// <param name="header">头部信息</param>
        /// <return>返回 context 自身</return>
        T? PutHeaderIfAbsent<T>(MessageHeader<T> header) where T : MessageHeader<T>;

        /// <summary>
        /// 删除 header
        /// </summary>
        bool RemoveHeader<T>(string key);

        /// <summary>
        /// 删除 header
        /// </summary>
        bool RemoveHeader<T>(MessageHeaderKey<T> key) where T : MessageHeader<T>;

        /// <summary>
        /// 删除 header
        /// </summary>
        void RemoveHeaders(IEnumerable<string> keys);

        /// <summary>
        /// 删除所有 header
        /// </summary>
        void RemoveAllHeaders();
    }

}
