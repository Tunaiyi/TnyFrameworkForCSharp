// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using Microsoft.Extensions.Logging;

namespace TnyFramework.Common.Event
{

    public abstract class ListenInvokerEvent<TListener, TParam, TEvent>
        : ListenBaseEvent<TListener, TListener, TParam, TEvent>
        where TEvent : IListenableEvent<TListener, TEvent>
    {
        protected Action<TListener, TParam> invoker;

        protected ListenInvokerEvent(Action<TListener, TParam> invoker)
        {
            this.invoker = invoker;
        }

        protected ListenInvokerEvent(TEvent parent, Action<TListener, TParam> invoker) : base(parent)
        {
            this.invoker = invoker;
        }

        protected override void OnDispose()
        {
            invoker = default!;
        }

        protected override void OnNotify(TListener listener, TParam param)
        {
            invoker(listener, param);
        }

        protected override TListener GetHandle(TListener target) => target;
    }

    public abstract class ListenActionEvent<TListener, THandler, TParam, TEvent>
        : ListenBaseEvent<TListener, THandler, TParam, TEvent>
        where TEvent : IListenableEvent<TListener, TEvent>
    {
        protected Func<TListener, THandler> handleFactory;

        protected ListenActionEvent(Func<TListener, THandler> invoker)
        {
            handleFactory = invoker;
        }

        protected ListenActionEvent(TEvent parent, Func<TListener, THandler> invoker) : base(parent)
        {
            handleFactory = invoker;
        }

        protected override void OnDispose()
        {
            handleFactory = default!;
        }

        protected override THandler GetHandle(TListener target) => handleFactory(target);
    }

    public abstract class ListenBaseEvent<TListener, THandle, TParam, TEvent>
        : BaseEvent<TListener, THandle, TEvent>
        where TEvent : IListenableEvent<TListener, TEvent>
    {
        protected ListenBaseEvent()
        {
        }

        protected ListenBaseEvent(TEvent parent) : base(parent)
        {
        }

        protected abstract void OnNotify(THandle handle, TParam param);

        /// <summary>
        /// trigger
        /// </summary>
        protected void DoNotify(TParam param)
        {
            var node = firstNode;
            while (node != null)
            {
                try
                {
                    node.Locked = true;
                    if (node.Target == null)
                    {
                        continue;
                    }
                    var handler = node.Handle;
                    if (handler == null)
                    {
                        handler = GetHandle(node.Target);
                        node.Handle = handler;
                    }
                    if (handler != null)
                    {
                        OnNotify(handler, param);
                    } else
                    {
                        LOGGER.LogWarning("{listener} trigger handler is null", node.Target);
                    }
                } catch (Exception e)
                {
                    LOGGER.LogError(e, "{listener} trigger exception", node.Target);
                } finally
                {
                    node.Locked = false;
                }
                var next = node.Next;
                if (node.IsDelayDequeue)
                {
                    RemoveNode(node);
                }
                node = next;
            }
        }
    }

}
