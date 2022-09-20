// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;

namespace TnyFramework.Namespace.Listener
{

    public delegate void OnComplete(INameNodesWatcher source);

    public delegate void OnWatch(INameNodesWatcher source);

    public delegate void OnError(INameNodesWatcher source, Exception exception);

    public delegate void OnNodeLoad<TValue>(INameNodesWatcher<TValue> source, List<NameNode<TValue>> create);

    public delegate void OnNodeCreate<TValue>(INameNodesWatcher<TValue> source, NameNode<TValue> create);

    public delegate void OnNodeUpdate<TValue>(INameNodesWatcher<TValue> source, NameNode<TValue> create);

    public delegate void OnNodeDelete<TValue>(INameNodesWatcher<TValue> source, NameNode<TValue> create);

}
