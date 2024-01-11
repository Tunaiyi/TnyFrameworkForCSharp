// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Common.Result;
using TnyFramework.Net.Rpc;

namespace TnyFramework.Net.Command.Dispatcher;

public interface IRpcInvokeContext
{
    string Name { get; }

    /**
     * 设置CommandResult,并中断执行
     *
     * @param result 运行结果
     */
    void Intercept(IRpcResult result);

    /**
     * 设置结果码,并中断执行
     *
     * @param code 结果码
     */
    void Intercept(IResultCode code);

    /**
     * 设置结果码,消息体,并中断执行
     */
    void Intercept(IResultCode code, object body);
}
