using System.Collections.Generic;
using System.Threading.Tasks;
using TnyFramework.Codec;
using TnyFramework.Common.Event;
using TnyFramework.Namespace.Listener;
using TnyFramework.Namespace.Sharding;

namespace TnyFramework.Namespace
{

    public interface IHashingSubscriber<TValue>
    {
        string Path { get; }

        ObjectMimeType<TValue> MineType { get; }

        Task Subscribe<TRange>(IList<TRange> ranges) where TRange : ShardingRange;

        Task SubscribeAll();

        Task Unsubscribe();

        void ClearListener();

        void Close();

        IEventBox<OnNodeLoad<TValue>> LoadEvent { get; }

        IEventBox<OnNodeCreate<TValue>> CreateEvent { get; }

        IEventBox<OnNodeUpdate<TValue>> UpdateEvent { get; }

        IEventBox<OnNodeDelete<TValue>> DeleteEvent { get; }
    }

}
