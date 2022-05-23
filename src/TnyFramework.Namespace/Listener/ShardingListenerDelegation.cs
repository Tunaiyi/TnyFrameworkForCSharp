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
