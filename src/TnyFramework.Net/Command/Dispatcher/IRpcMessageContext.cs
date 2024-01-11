// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Common.Attribute;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.Command.Dispatcher;

public interface IRpcMessageContext
{
    /// <summary>
    /// 获取消息
    /// </summary>
    ///
    /// <return>获取消息</return>
    IMessageSubject MessageSubject { get; }

    /// <summary>
    /// 附加属性
    /// </summary>
    ///
    /// <return>附加属性</return>
    IAttributes Attributes { get; }

    /// <summary>
    /// 是否有效
    /// </summary>
    ///
    /// <return>空</return>
    bool Valid { get; }
}
