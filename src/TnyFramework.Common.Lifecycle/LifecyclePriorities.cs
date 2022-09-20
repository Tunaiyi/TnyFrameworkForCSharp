using System.Collections.Concurrent;
using TnyFramework.Common.Exceptions;

namespace TnyFramework.Common.Lifecycle
{

    public static class LifecyclePriorities
    {
        private static readonly ConcurrentDictionary<int, ILifecyclePriority> PRIORITIES = new ConcurrentDictionary<int, ILifecyclePriority>();

        static LifecyclePriorities()
        {
            foreach (LifecycleLevel level in System.Enum.GetValues(typeof(LifecycleLevel)))
            {
                PRIORITIES.TryAdd(level.Order, level);
            }
        }

        public static ILifecyclePriority Of(int priority)
        {
            return PRIORITIES.GetOrAdd(priority, p => new CustomLifecyclePriority(p));
        }

        public static ILifecyclePriority Highest()
        {
            return Of(int.MaxValue);
        }

        public static ILifecyclePriority Lowest()
        {
            return Of(0);
        }

        public static ILifecyclePriority Lower(ILifecyclePriority priority, int order = 1)
        {
            if (order <= 0)
            {
                throw new IllegalArgumentException($"order {order} must >= 0");
            }
            var priorityValue = priority.Order - order;
            if (priorityValue < 0)
            {
                throw new IllegalArgumentException($"{priorityValue} is lowest");
            }
            return Of(priorityValue);
        }

        public static ILifecyclePriority Higher(ILifecyclePriority priority, int order = 1)
        {
            if (order <= 0)
            {
                throw new IllegalArgumentException($"order {order} must >= 0");
            }
            var priorityValue = priority.Order + order;
            if (priorityValue < int.MaxValue)
            {
                throw new IllegalArgumentException($"order {priorityValue} must <= {int.MaxValue}");
            }
            return Of(priorityValue);
        }
    }

    internal class CustomLifecyclePriority : ILifecyclePriority
    {
        public CustomLifecyclePriority(int order)
        {
            Order = order;
        }

        public int Order { get; }
    }

}
