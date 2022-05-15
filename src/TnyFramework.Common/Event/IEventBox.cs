using System;
using System.Collections.Generic;

namespace TnyFramework.Common.Event
{

    public interface IEventBox<in THandler> : IEventBus
        where THandler : Delegate
    {
        /// <summary>
        /// 添加监听
        /// </summary>
        /// <param name="handler"></param>
        void Add(THandler handler);

        /// <summary>
        /// 添加监听
        /// </summary>
        /// <param name="handler"></param>
        void Add(IEnumerable<THandler> handler);

        /// <summary>
        /// 添加监听
        /// </summary>
        /// <param name="handler"></param>
        void Add(params THandler[] handler);

        /// <summary>
        /// 移除监听
        /// </summary>
        /// <param name="handler"></param>
        void Remove(THandler handler);

        /// <summary>
        /// 移除监听
        /// </summary>
        /// <param name="handler"></param>
        void Remove(IEnumerable<THandler> handler);

        /// <summary>
        /// 移除监听
        /// </summary>
        /// <param name="handler"></param>
        void Remove(params THandler[] handler);

        /// <summary>
        /// 清理所有
        /// </summary>
        void Clear();

        /// <summary>
        /// 获取父事件盒
        /// </summary>
        IEventBox<THandler> Parent { get; }
    }

}
