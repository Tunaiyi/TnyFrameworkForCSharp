// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;

namespace TnyFramework.Namespace.Listener;

/// <summary>
/// 开始租约事件
/// </summary>
/// <param name="source">事件源</param>
public delegate void LesseeOnLease(ILessee source);

/// <summary>
/// 恢复租约续约事件
/// </summary>
/// <param name="source">事件源</param>
public delegate void LesseeOnResume(ILessee source);

/// <summary>
/// 租约续约事件
/// </summary>
/// <param name="source">事件源</param>
public delegate void LesseeOnRenew(ILessee source);

/// <summary>
/// 租约取消事件
/// </summary>
/// <param name="source">事件源</param>
public delegate void LesseeOnCompleted(ILessee source);

/// <summary>
/// 租约异常事件
/// </summary>
/// <param name="source">事件源</param>
public delegate void LesseeOnError(ILessee source, Exception cause);
