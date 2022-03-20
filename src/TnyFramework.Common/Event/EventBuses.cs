using System;
namespace TnyFramework.Common.Event
{
    /// <summary>
    /// 事件总线静态类
    /// </summary>
    public static class EventBuses
    {
        public static IEventBus<THandler> Create<THandler>()
            where THandler : Delegate
        {
            return new EventBus<THandler>();
        }


        public static IEventBus<T> Event<T>(this object invoker, ref IEventBus<T> eventBus) where T : Delegate
        {
            if (eventBus != null)
            {
                return eventBus;
            }
            lock (invoker)
            {
                {
                    eventBus = new EventBus<T>();
                }
            }
            return eventBus;
        }


        public static IEventBus<T> ForkEvent<T>(this object invoker, IEventBus<T> parentBus, ref IEventBus<T> eventBus)
            where T : Delegate
        {
            if (eventBus != null)
            {
                return eventBus;
            }
            lock (invoker)
            {
                {
                    eventBus = parentBus.ForkChild();
                }
            }
            return eventBus;
        }


        public static IEventBus<THandler> Create<THandler>(out IEventBus<THandler> eventBus)
            where THandler : Delegate
        {
            var bus = new EventBus<THandler>();
            eventBus = bus;
            return bus;
        }
    }
}
