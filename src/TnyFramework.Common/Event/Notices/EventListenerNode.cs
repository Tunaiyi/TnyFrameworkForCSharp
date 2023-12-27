// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using Microsoft.Extensions.ObjectPool;

namespace TnyFramework.Common.Event.Notices
{

    public class EventListenerNode<TListener> : IResettable
    {
        private static readonly ObjectPool<EventListenerNode<TListener>> OBJECT_POOL =
            new DefaultObjectPool<EventListenerNode<TListener>>(new DefaultPooledObjectPolicy<EventListenerNode<TListener>>());

        private TListener? listener;

        public static EventListenerNode<TListener> Create(TListener listener)
        {
            var node = OBJECT_POOL.Get();
            node.listener = listener;
            return node;
        }

        public EventListenerNode()
        {
        }

        /// <summary>
        /// 上一个节点
        /// </summary>
        public EventListenerNode<TListener>? Previous { get; private set; }

        /// <summary>
        /// 下一个节点
        /// </summary>
        public EventListenerNode<TListener>? Next { get; private set; }

        /// <summary>
        /// 是否有上一个节点
        /// </summary>
        public bool HasPrevious => Previous != null;

        /// <summary>
        /// 是否有下一个节点
        /// </summary>
        public bool HasNext => Next != null;

        /// <summary>
        /// 是否为最后的节点
        /// </summary>
        public bool IsLast => Next == null;

        /// <summary>
        /// 是否为首节点
        /// </summary>
        public bool IsFirst => Previous == null;

        /// <summary>
        /// 是否标记为延迟出队
        /// </summary>
        public bool IsDelayDequeue { get; private set; }

        public TListener? Listener => listener;

        public EventListenerNode<TListener> FirstNode {
            get {
                if (IsFirst)
                {
                    return this;
                }
                return Previous!.FirstNode;
            }
        }

        public EventListenerNode<TListener> LastNode {
            get {
                if (IsLast)
                {
                    return this;
                }
                return Next!.LastNode;
            }
        }

        internal bool Locked { get; set; }

        public EventListenerNode<TListener> Find(TListener listener1)
        {
            if (ReferenceEquals(listener, listener1))
            {
                return this;
            }
            return !IsLast ? Next!.Find(listener1) : null!;
        }

        /// <summary>
        /// 将新节点入队
        /// </summary>
        /// <param name="newNode">新节点</param>
        /// <returns>原样返回新节点</returns>
        public EventListenerNode<TListener> Enqueue(EventListenerNode<TListener> newNode)
        {
            if (newNode == this)
            {
                throw new ArgumentException("newNode:不能添加自己!");
            }

            if (newNode.Previous != null || newNode.Next != null)
            {
                throw new ArgumentException("newNode:请添加全新节点!");
            }

            if (IsLast)
            {
                newNode.Previous = this;
                Next = newNode;
            } else
            {
                Next!.Enqueue(newNode);
            }
            return newNode;
        }

        /// <summary>
        /// 将此节点出队
        /// </summary>
        /// <returns>当前节点出队后，原链的首节点（如果是唯一节点将返回null）</returns>
        public bool Dequeue(out EventListenerNode<TListener> first)
        {
            if (IsFirst && IsLast)
            {
                first = null!;
                return false;
            }
            if (Locked)
            {
                IsDelayDequeue = true;
                first = null!;
                return false;
            }
            EventListenerNode<TListener>? newFirst;
            if (IsFirst)
            {
                newFirst = Next;
                Next!.Previous = null;
                Next = null;
            } else if (IsLast)
            {
                newFirst = FirstNode;
                Previous!.Next = null;
                Previous = null;
            } else
            {
                newFirst = FirstNode;
                Previous!.Next = Next;
                Next!.Previous = Previous;
                Previous = null;
                Next = null;
            }
            first = newFirst!;
            return true;
        }

        /// <summary>
        /// 延迟出队
        /// </summary>
        public void DelayDequeue()
        {
            IsDelayDequeue = true;
        }

        public void Clear()
        {
            if (IsFirst)
            {
                DoClear();
            } else
            {
                FirstNode.DoClear();
            }
        }

        private void DoClear()
        {
            if (HasNext)
            {
                Next!.DoClear();
            }
            Next = null;
            Previous = null;
            Dispose();
        }

        public void Dispose()
        {
            OBJECT_POOL.Return(this);
        }

        public bool TryReset()
        {
            Next = null;
            Previous = null;
            listener = default!;
            return true;
        }
    }

}
