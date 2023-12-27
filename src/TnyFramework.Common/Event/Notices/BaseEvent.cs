// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;

namespace TnyFramework.Common.Event.Notices;

public abstract class BaseEvent<TListener, TEvent> : IEvent<TListener>, IDisposable
    where TEvent : BaseEvent<TListener, TEvent>
{
    protected static readonly ILogger LOGGER = LogFactory.Logger<TEvent>();

    protected EventListenerNode<TListener>? firstNode;

    protected TEvent? parent;

    protected BaseEvent()
    {
    }

    protected BaseEvent(TEvent parent)
    {
        this.parent = parent;
    }

    public void AddListener(TListener listener)
    {
        var newNode = EventListenerNode<TListener>.Create(listener);
        if (firstNode == null)
        {
            firstNode = newNode;
        } else
        {
            firstNode.Enqueue(newNode);
            firstNode = newNode.FirstNode;
        }
    }

    public void RemoveListener(TListener listener)
    {
        if (HasHandler)
        {
            RemoveNode(firstNode?.Find(listener));
        }
    }

    public void RemoveAllListener()
    {
        if (!HasHandler)
        {
            return;
        }
        firstNode?.Clear();
        firstNode = null;
    }

    protected void RemoveNode(EventListenerNode<TListener>? node)
    {
        if (node == null)
        {
            return;
        }
        if (node.Dequeue(out var first))
        {
            firstNode = first;
            node.Dispose();
        } else // 如果当前节点正在分发中， 则标记为死亡， 不移除
        {
            node.DelayDequeue();
        }
    }

    private bool HasHandler => firstNode != null;

    public void Dispose()
    {
        RemoveAllListener();
        parent = null;
        OnDispose();
    }

    protected virtual void OnDispose()
    {
    }
}
