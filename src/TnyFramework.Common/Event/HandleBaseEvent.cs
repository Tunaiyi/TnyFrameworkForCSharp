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

    public abstract class HandleBaseEvent<THandler, TEvent> : BaseEvent<THandler, TEvent>
        where TEvent : IHandleableEvent<THandler, TEvent>
        where THandler : Delegate
    {
        protected HandleBaseEvent()
        {
        }

        protected HandleBaseEvent(TEvent parent)
            : base(parent)
        {
        }

        /// <summary>
        /// trigger
        /// </summary>
        protected void DoNotify<TParam>(Action<THandler, TParam> invoker, TParam param)
        {
            var node = firstNode;
            while (node != null)
            {
                try
                {
                    node.Locked = true;
                    if (node.Target != null)
                    {
                        invoker(node.Target, param);
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
