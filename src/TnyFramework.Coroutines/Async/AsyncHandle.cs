// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Threading.Tasks;

namespace TnyFramework.Coroutines.Async
{

    /// <summary>
    /// 协程 Action 委托
    /// </summary>
    public delegate Task AsyncHandle();

    /// <summary>
    /// 协程 Func 委托
    /// </summary>
    /// <typeparam name="T">返回类型</typeparam>
    public delegate Task<T> AsyncHandle<T>();

}
