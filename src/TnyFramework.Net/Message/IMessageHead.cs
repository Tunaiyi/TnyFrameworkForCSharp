using System;
using System.Collections.Generic;

namespace TnyFramework.Net.Message
{

    /// <summary>
    /// 消息头
    /// </summary>
    public interface IMessageHead : IMessageSchema
    {
        /// <summary>
        /// 请求Id
        /// </summary>
        long Id { get; }

        /// <summary>
        /// 消息响应码
        /// </summary>
        int Code { get; }

        /// <summary>
        /// 请求时间
        /// </summary>
        long Time { get; }

        /// <summary>
        /// 获取转发 header
        /// </summary>
        /// <param name="key">查找 key</param>
        /// <typeparam name="T">Header 类</typeparam>
        /// <returns>Header</returns>
        T GetHeader<T>(string key) where T : MessageHeader<T>;

        /// <summary>
        /// 获取转发 header
        /// </summary>
        /// <param name="key">查找 key</param>
        /// <param name="type">Header 类</param>
        /// <returns>Header</returns>
        MessageHeader GetHeader(string key, Type type);

        /// <summary>
        /// 获取转发 header
        /// </summary>
        /// <param name="key">查找 key</param>
        /// <returns>Header</returns>
        MessageHeader GetHeader(string key);

        /// <summary>
        /// 获取转发 header
        /// </summary>
        /// <typeparam name="T">Header 类</typeparam>
        /// <returns>Header列表</returns>
        IList<T> GetHeaders<T>() where T : MessageHeader<T>;

        /// <summary>
        /// 获取转发 header
        /// </summary>
        /// <param name="type">Header 类</param>
        /// <returns>Header列表</returns>
        IList<MessageHeader> GetHeaders(Type type);

        /// <summary>
        /// 获取转发 header
        /// </summary>
        /// <param name="key">查找 key</param>
        /// <typeparam name="T">Header 类</typeparam>
        /// <returns>Header</returns>
        T GetHeader<T>(MessageHeaderKey<T> key) where T : MessageHeader<T>;

        /// <summary>
        /// 是否有 Header 存在
        /// </summary>
        bool IsHasHeaders { get; }

        /// <summary>
        /// 是否是转发
        /// </summary>
        /// <returns>是转发返回 True,否则返回 false</returns>
        bool IsForward();

        /// <summary>
        /// 获取转发头
        /// </summary>
        RpcForwardHeader ForwardHeader { get; }

        /// <summary>
        /// 获取全部 Header
        /// </summary>
        /// <returns>Header 列表</returns>
        IList<MessageHeader> GetAllHeaders();

        /// <summary>
        /// 获取全部 Header
        /// </summary>
        /// <returns>全部 Header</returns>
        IDictionary<string, MessageHeader> GetAllHeadersMap();

        /// <summary>
        /// 是否存在指定 key 的 Header
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns>存在返回 true, 否则返回 false</returns>
        bool ExistHeader(string key);

        /// <summary>
        /// 是否存在指定 key 的 Header
        /// </summary>
        /// <param name="key">键值</param>
        /// <typeparam name="T">是否是指定类</typeparam>
        /// <returns>存在返回 true, 否则返回 false</returns>
        bool ExistHeader<T>(string key) where T : MessageHeader<T>;

        /// <summary>
        /// 是否存在指定 key 的 Header
        /// </summary>
        /// <param name="key">键值</param>
        /// <typeparam name="T">是否是指定类</typeparam>
        /// <returns>存在返回 true, 否则返回 false</returns>
        bool ExistHeader(MessageHeaderKey key);
    }

}
