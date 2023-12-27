// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using Microsoft.Extensions.Logging;

namespace TnyFramework.Common.Event.Notices;

public abstract class ListenBaseEvent<TListener, TCaller, TEvent> : BaseEvent<TListener, TEvent>
    where TEvent : ListenBaseEvent<TListener, TCaller, TEvent>
{
    protected readonly TCaller caller;

    protected ListenBaseEvent(TCaller caller)
    {
        this.caller = caller;
    }

    protected ListenBaseEvent(TEvent parent)
        : base(parent)
    {
        caller = parent.caller;
    }

    /// <summary>
    /// trigger
    /// </summary>
    protected void DoTrigger<TParam>(Action<TCaller, TListener, TParam> invoker, TParam param)
    {
        var node = firstNode;
        while (node != null)
        {
            try
            {
                node.Locked = true;
                if (node.Listener != null)
                {
                    invoker(caller, node.Listener, param);
                }
            } catch (Exception e)
            {
                LOGGER.LogError(e, "{listener} trigger exception", node.Listener);
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
