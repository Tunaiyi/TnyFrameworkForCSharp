// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;

namespace TnyFramework.Net.Command.Dispatcher
{

    public interface IRpcInvocationContext : IRpcContext
    {
        /// <summary>
        /// 是否为空
        /// </summary>
        ///
        /// <return>空</return>
        bool IsEmpty();

        /// <summary>
        /// @return 准备
        /// </summary>
        bool Prepare(string operationName);

        /// <summary>
        /// @return 操作名
        /// </summary>
        string OperationName { get; }

        /// <summary>
        /// 失败并响应
        /// </summary>
        /// <param name="error">错误原因</param>
        /// <return>是否完成成功</return>
        bool Complete(Exception error);

        /// <summary>
        /// 成功并响应
        /// </summary>
        /// <return>是否完成成功</return>
        bool Complete();

        /// <summary>
        /// 获取错误原因
        /// </summary>
        /// <return>获取错误原因</return>
        Exception Cause { get; }

        /// <summary>
        /// 获取错误原因
        /// </summary>
        /// <return>是否错误(异常)</return>
        bool IsError();
    }

}
