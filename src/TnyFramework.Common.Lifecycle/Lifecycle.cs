using System;
using System.Collections.Generic;
using TnyFramework.Common.Exceptions;
using TnyFramework.Common.Extensions;

namespace TnyFramework.Common.Lifecycle
{

    public abstract class Lifecycle : IComparable<Lifecycle>
    {
        private static readonly Dictionary<LifecycleStage, Dictionary<Type, Lifecycle>> LIFECYCLES_MAP;

        public abstract Type HandlerType { get; }

        public abstract int Order { get; }

        public abstract Lifecycle GetHead<T>() where T : Lifecycle;

        public abstract Lifecycle GetTail<T>() where T : Lifecycle;

        public abstract Lifecycle GetPrev<T>() where T : Lifecycle;

        public abstract Lifecycle GetNext<T>() where T : Lifecycle;

        static Lifecycle()
        {
            var map = new Dictionary<LifecycleStage, Dictionary<Type, Lifecycle>>();
            foreach (var value in LifecycleStage.GetValues())
            {
                map[value] = new Dictionary<Type, Lifecycle>();
            }
            LIFECYCLES_MAP = map;
        }

        internal static TLifecycle PutLifecycle<TLifecycle>(LifecycleStage<TLifecycle> stage, TLifecycle lifecycle)
            where TLifecycle : Lifecycle
        {
            var dictionary = LIFECYCLES_MAP[stage];
            var old = dictionary.PutIfAbsent(lifecycle.HandlerType, lifecycle);
            if (old != null)
            {
                throw new IllegalArgumentException($"{lifecycle.HandlerType} 已经存在 {old}, 无法添加 {lifecycle}");
            }
            return lifecycle;
        }

        internal static TLifecycle GetLifecycle<TLifecycle>(LifecycleStage<TLifecycle> stage, Type type)
            where TLifecycle : Lifecycle
        {
            var dictionary = LIFECYCLES_MAP[stage];
            return (TLifecycle) dictionary.Get(type);
        }

        internal static TLifecycle GetLifecycle<TLifecycle, THandler>(LifecycleStage<TLifecycle> stage)
            where TLifecycle : Lifecycle
            where THandler : ILifecycleHandler
        {
            var dictionary = LIFECYCLES_MAP[stage];
            return (TLifecycle) dictionary.Get(typeof(THandler));
        }

        public int CompareTo(Lifecycle other)
        {
            var value = other.Order - Order;
            return value == 0 ? string.Compare(HandlerType.Name, other.HandlerType.Name, StringComparison.Ordinal) : value;
        }
    }

    public abstract class Lifecycle<TLife, THandler> : Lifecycle
        where TLife : Lifecycle<TLife, THandler>, new()
        where THandler : ILifecycleHandler
    {
        private Type handlerType;

        private ILifecyclePriority Priority { get; set; }

        public TLife Next { get; private set; }

        public TLife Prev { get; private set; }

        public override Type HandlerType => handlerType;

        public override int Order => Priority.Order;

        public TLife HeadLifecycle {
            get {
                if (Prev == null)
                {
                    return this as TLife;
                }
                return Prev.HeadLifecycle;
            }
        }

        public TLife TailLifecycle {
            get {
                if (Next == null)
                {
                    return this as TLife;
                }
                return Next.TailLifecycle;
            }
        }

        public override Lifecycle GetHead<T>() => HeadLifecycle;

        public override Lifecycle GetTail<T>() => TailLifecycle;

        public override Lifecycle GetPrev<T>() => Prev;

        public override Lifecycle GetNext<T>() => Next;

        public TLife Append(TLife life)
        {
            if (Next != null)
            {
                throw new IllegalArgumentException($"{this} next is exist {Next}");
            }
            if (life.Order > Order)
            {
                throw new IllegalArgumentException($"{life} [{life.Order}] prior to {this} [{Order}]");
            }
            if (!(this is TLife item))
            {
                throw new IllegalArgumentException($"this {this} is not type of {nameof(TLife)}");
            }
            life.SetPrev(item);
            return item.Next = life;
        }

        public TLife Append<TOtherHandler>() where TOtherHandler : THandler
        {
            return Append(Value<TOtherHandler>());
        }

        private void SetPrev(TLife life)
        {
            if (Prev != null)
            {
                throw new IllegalArgumentException($"{this} prev is exist {Prev}");
            }
            Prev = life;
        }

        private bool Equals(Lifecycle<TLife, THandler> other)
        {
            return HandlerType == other.HandlerType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Lifecycle<TLife, THandler>) obj);
        }

        public override int GetHashCode()
        {
            return HandlerType != null ? HandlerType.GetHashCode() : 0;
        }

        public static TLife Value<TSubHandler>()
            where TSubHandler : THandler
        {
            return Value<TSubHandler>(LifecycleLevel.CUSTOM_LEVEL_5);
        }

        public static TLife Value(Type handlerType)
        {
            return Value(LifecycleLevel.CUSTOM_LEVEL_5, handlerType);
        }

        public static TLife Value<TSubHandler>(ILifecyclePriority lifeCycleLevel)
            where TSubHandler : THandler
        {
            return Value(lifeCycleLevel, typeof(TSubHandler));
        }

        public static TLife Value(ILifecyclePriority lifeCycleLevel, Type handleType)
        {
            var stage = Stage();
            var lifecycle = GetLifecycle(Stage(), handleType);
            if (lifecycle != null)
                return lifecycle;
            lifecycle = new TLife {
                Priority = lifeCycleLevel,
                handlerType = handleType
            };
            return PutLifecycle(stage, lifecycle);
        }

        private static LifecycleStage<TLife> Stage()
        {
            return LifecycleStage.ForLifecycleType<TLife>();
        }
    }

}
