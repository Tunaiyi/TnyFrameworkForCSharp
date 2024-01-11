// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace TnyFramework.Net.Message;

public interface IMessageSubject : IMessageHeaderContainer, IMessageSchema
{
    /// <summary>
    /// 是否存在消息
    /// </summary>
    bool ExistBody { get; }

    /// <summary>
    /// 获取消息体
    /// </summary>
    object? Body { get; }

    // /// <summary>
    // /// 获取真实原始响应消息 id
    // /// </summary>
    // long OriginalToMessage { get; }

    /// <summary>
    /// 获取消息体
    /// </summary>
    /// <typeparam name="T">消息体类型</typeparam>
    /// <returns></returns>
    T? BodyAs<T>();
}
