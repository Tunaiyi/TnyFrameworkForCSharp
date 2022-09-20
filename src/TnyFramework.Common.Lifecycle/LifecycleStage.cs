using System;
using System.Collections.Concurrent;
using TnyFramework.Common.Enum;

namespace TnyFramework.Common.Lifecycle
{

    /// <summary>
    /// 生命周期阶段
    /// </summary>
    public class LifecycleStage : BaseEnum<LifecycleStage>
    {
        /// <summary>
        /// 准备启动
        /// </summary>
        public static readonly LifecycleStage<PrepareStarter> PREPARE_START = LifecycleStage<PrepareStarter>.Of(1);

        /// <summary>
        /// 启动结束
        /// </summary>
        public static readonly LifecycleStage<PostStarter> POST_START = LifecycleStage<PostStarter>.Of(2);

        /// <summary>
        /// 启动结束
        /// </summary>
        public static readonly LifecycleStage<PostCloser> CLOSED = LifecycleStage<PostCloser>.Of(3);
        

        private static readonly ConcurrentDictionary<Type, LifecycleStage> TYPE_STAGE_MAP = new ConcurrentDictionary<Type, LifecycleStage>();
        
        public new static void LoadAll() => LoadAll(typeof(LifecycleStage));
        
        public Type LifecycleType { get; protected set; }

        protected override void OnCheck()
        {
            if (TYPE_STAGE_MAP.TryAdd(LifecycleType, this))
                return;
            var value = TYPE_STAGE_MAP[LifecycleType];
            if (!ReferenceEquals(value, this))
            {
                throw new ArgumentException($"{value} 与 {this} 存在相同的 LifecycleType {LifecycleType}");
            }
        }

        public static LifecycleStage<TLifecycle> ForLifecycleType<TLifecycle>()
            where TLifecycle : Lifecycle
        {
            var type = typeof(TLifecycle);
            if (!TYPE_STAGE_MAP.TryGetValue(type, out var obj))
                throw new ArgumentException($"LifecycleType {type} 枚举不存在");
            return (LifecycleStage<TLifecycle>) obj;
        }

        public static LifecycleStage ForLifecycleType(Type type)
        {
            if (!TYPE_STAGE_MAP.TryGetValue(type, out var obj))
                throw new ArgumentException($"LifecycleType {type} 枚举不存在");
            return obj;
        }
    }

    public class LifecycleStage<TLifecycle> : LifecycleStage
        where TLifecycle : Lifecycle
    {
        internal static LifecycleStage<TLifecycle> Of(int id)
        {
            return E(id, new LifecycleStage<TLifecycle> {
                LifecycleType = typeof(TLifecycle)
            });
        }
    }

}
