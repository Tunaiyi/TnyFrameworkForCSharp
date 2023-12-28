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

    public class EventHandleNode<TTarget, THandle> : IResettable
    {
        private static readonly ObjectPool<EventHandleNode<TTarget, THandle>> OBJECT_POOL =
            new DefaultObjectPool<EventHandleNode<TTarget, THandle>>(new DefaultPooledObjectPolicy<EventHandleNode<TTarget, THandle>>());

        private TTarget? target;
        private THandle? handle;

        public static EventHandleNode<TTarget, THandle> Create(TTarget target)
        {
            var node = OBJECT_POOL.Get();
            node.target = target;
            return node;
        }

        public EventHandleNode()
        {
        }

        /// <summary>
        /// 上一个节点
        /// </summary>
        public EventHandleNode<TTarget, THandle>? Previous { get; private set; }

        /// <summary>
        /// 下一个节点
        /// </summary>
        public EventHandleNode<TTarget, THandle>? Next { get; private set; }

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

        public THandle? Handle {
            get => handle;
            set => handle = value;
        }

        public TTarget? Target => target;

        public EventHandleNode<TTarget, THandle> FirstNode {
            get {
                if (IsFirst)
                {
                    return this;
                }
                return Previous!.FirstNode;
            }
        }

        public EventHandleNode<TTarget, THandle> LastNode {
            get {
                if (IsLast)
                {
                    return this;
                }
                return Next!.LastNode;
            }
        }

        internal bool Locked { get; set; }

        public EventHandleNode<TTarget, THandle> Find(TTarget listener1)
        {
            if (ReferenceEquals(target, listener1))
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
        public EventHandleNode<TTarget, THandle> Enqueue(EventHandleNode<TTarget, THandle> newNode)
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
        public bool Dequeue(out EventHandleNode<TTarget, THandle> first)
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
            EventHandleNode<TTarget, THandle>? newFirst;
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
            target = default!;
            handle = default!;
            return true;
        }
    }

}
