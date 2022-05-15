using System;

namespace TnyFramework.Common.Event
{

    public interface IEventBus
    {
    }

    public interface IEventBus<THandler> : IEventBox<THandler>
        where THandler : Delegate
    {
        THandler Notify { get; }

        IEventBus<THandler> ForkChild();
    }

}
