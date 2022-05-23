using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TnyFramework.Codec;
using TnyFramework.Namespace.Sharding;

namespace TnyFramework.Namespace
{

    /// <summary>
    /// NamespaceExplorer 管理器接口
    /// </summary>
    public interface INamespaceExplorer
    {
        /// <summary>
        /// 获取指定 path 节点
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="type">值类型</param>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <returns>返回获取的节点 Task, 如果没有会Task返回值为 null</returns>
        Task<NameNode<TValue>> Get<TValue>(string path, ObjectMimeType<TValue> type);

        /// <summary>
        /// 匹配路径查找指定节点列表
        /// </summary>
        /// <param name="path">匹配路径</param>
        /// <param name="type">值MineType</param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns>返回获取的节点List的 Task, 如果没有会Task返回值为 空List</returns>
        Task<IList<NameNode<TValue>>> FindAll<TValue>(string path, ObjectMimeType<TValue> type);

        /// <summary>
        /// 匹配路径查找指定节点列表
        /// </summary>
        /// <param name="from">起始 (包括)</param>
        /// <param name="to">结束 (不包括)</param>
        /// <param name="type">值MineType</param>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <returns>返回获取的节点List的 Task, 如果没有会Task返回值为 空List</returns>
        Task<List<NameNode<TValue>>> FindAll<TValue>(string from, string to, ObjectMimeType<TValue> type);

        /// <summary>
        /// 创建 hash 节点存储器
        /// </summary>
        /// <param name="rootPath">路径</param>
        /// <param name="options">选项</param>
        /// <returns>返回一致性 hash 环</returns>
        INodeHashing<TNode> NodeHashing<TNode>(string rootPath, HashingOptions<TNode> options) where TNode : IShardingNode;

        /// <summary>
        /// 创建 hash 节点存储器
        /// </summary>
        /// <param name="rootPath">路径</param>
        /// <param name="factory">节点存储器工厂</param>
        /// <param name="options">选项</param>
        /// <returns>返回一致性 hash 环</returns>
        INodeHashing<TNode> NodeHashing<TNode>(string rootPath, INodeHashingFactory factory, HashingOptions<TNode> options)
            where TNode : IShardingNode;

        /// <summary>
        /// 创建 hash 节点存储器
        /// </summary>
        /// <param name="rootPath">路径</param>
        /// <param name="maxSlotSize">最大槽数</param>
        /// <param name="keyHasher">key哈希计算器</param>
        /// <param name="nodeHasher">节点哈希计算器</param>
        /// <returns>返回 hash 节点存储器</returns>
        INodeHashing<TNode> NodeHashing<TNode>(string rootPath, long maxSlotSize, IHasher<string> keyHasher, IHasher<PartitionSlot<TNode>> nodeHasher)
            where TNode : IShardingNode;

        /// <summary>
        /// 创建 hash 节点存储器
        /// </summary>
        /// <param name="rootPath">路径</param>
        /// <param name="maxSlotSize">最大槽数</param>
        /// <param name="keyHasher">key哈希计算器</param>
        /// <param name="nodeHasher">节点哈希计算器</param>
        /// <param name="factory">节点存储器工厂</param>
        /// <returns>返回 hash 节点存储器</returns>
        INodeHashing<TNode> NodeHashing<TNode>(string rootPath, long maxSlotSize, IHasher<string> keyHasher, IHasher<PartitionSlot<TNode>> nodeHasher,
            INodeHashingFactory factory) where TNode : IShardingNode;

        /// <summary>
        /// 创建 hash 节点存储器
        /// </summary>
        /// <param name="rootPath">路径</param>
        /// <param name="maxSlotSize">最大槽数</param>
        /// <param name="keyHasher">key哈希计算器</param>
        /// <param name="nodeHasher">节点哈希计算器</param>
        /// <param name="custom">选项自定义</param>
        /// <returns>返回 hash 节点存储器</returns>
        INodeHashing<TNode> NodeHashing<TNode>(string rootPath, long maxSlotSize, IHasher<string> keyHasher, IHasher<PartitionSlot<TNode>> nodeHasher,
            Action<HashingOptions<TNode>> custom) where TNode : IShardingNode;

        /// <summary>
        /// 创建 hash 节点存储器
        /// </summary>
        /// <param name="rootPath">路径</param>
        /// <param name="maxSlotSize">最大槽数</param>
        /// <param name="keyHasher">key哈希计算器</param>
        /// <param name="nodeHasher">节点哈希计算器</param>
        /// <param name="factory">节点存储器工厂</param>
        /// <param name="custom">选项自定义</param>
        /// <returns>返回 hash 节点存储器</returns>
        INodeHashing<TNode> NodeHashing<TNode>(string rootPath, long maxSlotSize, IHasher<string> keyHasher, IHasher<PartitionSlot<TNode>> nodeHasher,
            INodeHashingFactory factory, Action<HashingOptions<TNode>> custom) where TNode : IShardingNode;

        /// <summary>
        /// Hashing 选项 Builder
        /// </summary>
        /// <param name="maxSlotSize">最大槽数</param>
        /// <param name="keyHasher">key哈希计算器</param>
        /// <param name="nodeHasher">节点哈希计算器</param>
        /// <returns>选项 Build</returns>
        HashingOptions<TNode> HashingOptions<TNode>(long maxSlotSize, IHasher<string> keyHasher, IHasher<PartitionSlot<TNode>> nodeHasher)
            where TNode : IShardingNode;

        /// <summary>
        /// 创建 Hashing 节点订阅器
        /// </summary>
        /// <param name="parentPath">父路径</param>
        /// <param name="maxSlotSize">最大槽位</param>
        /// <param name="mineType">媒体类型</param>
        /// <returns>返回订阅器</returns>
        IHashingSubscriber<TValue> HashingSubscriber<TValue>(
            string parentPath, long maxSlotSize, ObjectMimeType<TValue> mineType);

        /// <summary>
        /// 创建 Hashing 节点发布器
        /// </summary>
        /// <param name="parentPath">父路径</param>
        /// <param name="maxSlotSize">最大槽位</param>
        /// <param name="valueHasher">value hash 计算器</param>
        /// <param name="mineType">媒体类型</param>
        /// <returns>返回发布器</returns>
        IHashingPublisher<TKey, TValue> HashingPublisher<TKey, TValue>(
            string parentPath, long maxSlotSize, IHasher<TValue> valueHasher, ObjectMimeType<TValue> mineType);

        /// <summary>
        /// 创建节点监控器
        /// </summary>
        /// <param name="path">监听路径</param>
        /// <param name="type">值MineType</param>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <returns>返回节点的监控器</returns>
        INameNodesWatcher<TValue> NodeWatcher<TValue>(string path, ObjectMimeType<TValue> type);

        /// <summary>
        /// 创建匹配节点监控器
        /// </summary>
        /// <param name="path">监听路径</param>
        /// <param name="type">值MineType</param>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <returns>返回匹配节点监控器</returns>
        INameNodesWatcher<TValue> AllNodeWatcher<TValue>(string path, ObjectMimeType<TValue> type);

        /// <summary>
        /// 创建匹配节点监控器
        /// </summary>
        /// <param name="from">起始 (包括)</param>
        /// <param name="to">结束 (不包括)</param>
        /// <param name="type">值MineType</param>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <returns>返回匹配节点监控器</returns>
        INameNodesWatcher<TValue> AllNodeWatcher<TValue>(string from, string to, ObjectMimeType<TValue> type);

        /// <summary>
        /// 创建租约
        /// </summary>
        /// <param name="name">名字</param>
        /// <param name="ttl">租约超时事件</param>
        /// <returns>返回结果</returns>
        Task<ILessee> Lease(string name, long ttl);

        /// <summary>
        /// 创建租约
        /// </summary>
        /// <param name="name">名字</param>
        /// <returns>返回结果</returns>
        Task<ILessee> Lease(string name);

        /// <summary>
        /// 获取指定 path 的节点, 如果存在则获取, 不存在则创建.
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="type">值类型Class</param>
        /// <param name="value">值</param>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <returns>返回获取的节点 Task, 如果存在则返回该节点, 如果不存在则返回创建的节点</returns>
        Task<NameNode<TValue>> GetOrAdd<TValue>(string path, ObjectMimeType<TValue> type, TValue value);

        /// <summary>
        /// 获取指定 path 的节点, 如果存在则获取, 不存在则创建.
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="type">值MineType</param>
        /// <param name="value">值</param>
        /// <param name="lessee">租客</param>
        /// <typeparam name="TValue">返回获取的节点 Task, 如果存在则返回该节点, 如果不存在则返回创建的节点</typeparam>
        /// <returns></returns>
        Task<NameNode<TValue>> GetOrAdd<TValue>(string path, ObjectMimeType<TValue> type, TValue value, ILessee lessee);

        /// <summary>
        /// 向指定 path 插入指定节点, 如果已存在则不插入.
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="type">值MineType</param>
        /// <param name="value">值</param>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <returns>返回插入的节点 Task, 如果没有插入则会Task返回值为 null</returns>
        Task<NameNode<TValue>> Add<TValue>(string path, ObjectMimeType<TValue> type, TValue value);

        /// <summary>
        /// 向指定 path 插入指定的租约节点, 如果已存在则不插入.
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="type">值MineType</param>
        /// <param name="value">值</param>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="lessee">租客</param>
        /// <returns>返回插入的节点 Task, 如果没有插入则会Task返回值为 null</returns>
        Task<NameNode<TValue>> Add<TValue>(string path, ObjectMimeType<TValue> type, TValue value, ILessee lessee);

        /// <summary>
        /// 向指定 path 保存指定的节点. 无论存在与否都会插入
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="type">值MineType</param>
        /// <param name="value">值</param>
        /// <typeparam name="TValue">返回保存的节点 Task</typeparam>
        Task<NameNode<TValue>> Save<TValue>(string path, ObjectMimeType<TValue> type, TValue value);

        /// <summary>
        /// 向指定 path 保存指定的节点. 无论存在与否都会插入
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="type">值MineType</param>
        /// <param name="value">值</param>
        /// <param name="lessee">租客</param>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <returns>返回保存的节点 Task</returns>
        Task<NameNode<TValue>> Save<TValue>(string path, ObjectMimeType<TValue> type, TValue value, ILessee lessee);

        /// <summary>
        /// 如果指定 path 节点存在, 更新该节点.
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="type">值MineType</param>
        /// <param name="value">值</param>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <returns>返回更新的节点 Task, 无更新则Task返回值为 null</returns>
        Task<NameNode<TValue>> Update<TValue>(string path, ObjectMimeType<TValue> type, TValue value);

        /// <summary>
        /// 如果指定 path 节点存在, 更新该租约节点.
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="type">值MineType</param>
        /// <param name="value">值</param>
        /// <param name="lessee">租客</param>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <returns>返回更新的节点 Task, 无更新则Task返回值为 null</returns>
        Task<NameNode<TValue>> Update<TValue>(string path, ObjectMimeType<TValue> type, TValue value, ILessee lessee);

        /// <summary>
        /// 如果指定 path 节点存在并且值等于expect值, 则更新该节点.
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="type">值MineType</param>
        /// <param name="expect">期待值</param>
        /// <param name="value">值</param>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <returns>返回更新的节点 Task, 无更新则Task返回值为 null</returns>
        Task<NameNode<TValue>> UpdateIf<TValue>(string path, ObjectMimeType<TValue> type, TValue expect, TValue value);

        /// <summary>
        /// 如果指定 path 节点存在并且值等于expect值, 则更新该租约节点.
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="type">值MineType</param>
        /// <param name="expect">期待值</param>
        /// <param name="value">值</param>
        /// <param name="lessee">租客</param>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <returns>返回更新的节点 Task</returns>
        Task<NameNode<TValue>> UpdateIf<TValue>(string path, ObjectMimeType<TValue> type, TValue expect, TValue value, ILessee lessee);

        /// <summary>
        /// 如果指定 path 节点存在并且节点版本等于指定version, 则更新该节点.
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="version">期望版本</param>
        /// <param name="type">值MineType</param>
        /// <param name="value">值</param>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <returns>返回更新的节点 Task</returns>
        Task<NameNode<TValue>> UpdateIf<TValue>(string path, long version, ObjectMimeType<TValue> type, TValue value);

        /// <summary>
        /// 如果指定 path 节点存在并且节点版本等于指定version, 则更新该租约节点.
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="version">期望版本</param>
        /// <param name="type">值MineType</param>
        /// <param name="value">值</param>
        /// <param name="lessee">租客</param>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <returns>返回更新的节点 Task</returns>
        Task<NameNode<TValue>> UpdateIf<TValue>(string path, long version, ObjectMimeType<TValue> type, TValue value, ILessee lessee);

        /// <summary>
        /// 如果指定 path 节点存在并且节点版本在指定区间内, 则更新该节点.
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="minVersion">最小值</param>
        /// <param name="minBorder">最小值边界</param>
        /// <param name="maxVersion">最大值</param>
        /// <param name="maxBorder">最大值边界</param>
        /// <param name="type">值MineType</param>
        /// <param name="value">值</param>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <returns>返回更新的节点 Task</returns>
        Task<NameNode<TValue>> UpdateIf<TValue>(string path, long minVersion, RangeBorder minBorder, long maxVersion, RangeBorder maxBorder,
            ObjectMimeType<TValue> type, TValue value);

        /// <summary>
        /// 如果指定 path 节点存在并且节点版本在指定区间内, 则更新该租约节点.
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="minVersion">最小值</param>
        /// <param name="minBorder">最小值边界</param>
        /// <param name="maxVersion">最大值</param>
        /// <param name="maxBorder">最大值边界</param>
        /// <param name="type">值MineType</param>
        /// <param name="value">值</param>
        /// <param name="lessee">租客</param>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <returns>返回更新的节点 Task</returns>
        Task<NameNode<TValue>> UpdateIf<TValue>(string path, long minVersion, RangeBorder minBorder, long maxVersion, RangeBorder maxBorder,
            ObjectMimeType<TValue> type, TValue value, ILessee lessee);

        /// <summary>
        /// 如果指定 path 节点存在,同时id等于指定id,且值等于expect值, 则更新该节点.
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="id">指定 id</param>
        /// <param name="type">值MineType</param>
        /// <param name="value">值</param>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <returns>返回更新的节点 Task, 无更新则Task返回值为 null</returns>
        Task<NameNode<TValue>> UpdateById<TValue>(string path, long id, ObjectMimeType<TValue> type, TValue value);

        /// <summary>
        /// 如果指定 path 节点存在, 且id等于指定id, 等于指定expect值, 则更新该租约节点.
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="id">指定 id</param>
        /// <param name="lessee">租客</param>
        /// <param name="type">值MineType</param>
        /// <param name="value">值</param>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <returns>返回更新的节点 Task</returns>
        Task<NameNode<TValue>> UpdateById<TValue>(string path, long id, ObjectMimeType<TValue> type, TValue value, ILessee lessee);

        /// <summary>
        /// 如果指定 path 节点存在, 且id等于指定id, 则更新该节点.
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="id">指定 id</param>
        /// <param name="expect">期待值</param>
        /// <param name="type">值MineType</param>
        /// <param name="value">值</param>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <returns>返回更新的节点 Task, 无更新则Task返回值为 null</returns>
        Task<NameNode<TValue>> UpdateByIdAndIf<TValue>(string path, long id, ObjectMimeType<TValue> type, TValue expect, TValue value);

        /// <summary>
        /// 如果指定 path 节点存在,同时id等于指定id,等于指定expect值, 则更新该租约节点.
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="id">指定 id</param>
        /// <param name="type">值MineType</param>
        /// <param name="expect">期待值</param>
        /// <param name="value">值</param>
        /// <param name="lessee">租客</param>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <returns>返回更新的节点 Task</returns>
        Task<NameNode<TValue>> UpdateByIdAndIf<TValue>(string path, long id, ObjectMimeType<TValue> type, TValue expect, TValue value,
            ILessee lessee);

        /// <summary>
        /// 如果指定 path 节点存在,同时id等于指定id,且版本等于指定version, 则更新该节点.
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="id">指定 id</param>
        /// <param name="version">期望版本</param>
        /// <param name="type">值MineType</param>
        /// <param name="value">值</param>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <returns>返回更新的节点 Task</returns>
        Task<NameNode<TValue>> UpdateByIdAndIf<TValue>(string path, long id, long version, ObjectMimeType<TValue> type, TValue value);

        /// <summary>
        /// 如果指定 path 节点存在,同时id等于指定id,且版本等于指定version, 则更新该租约节点.
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="id">指定 id</param>
        /// <param name="version">期望版本</param>
        /// <param name="type">值MineType</param>
        /// <param name="value">值</param>
        /// <param name="lessee">租客</param>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <returns>返回更新的节点 Task</returns>
        Task<NameNode<TValue>> UpdateByIdAndIf<TValue>(string path, long id, long version, ObjectMimeType<TValue> type, TValue value, ILessee lessee);

        /// <summary>
        /// 如果指定 path 节点存在,同时id等于指定id,并且节点版本在指定区间内, 则更新该节点.
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="id">指定 id</param>
        /// <param name="minVersion">最小值</param>
        /// <param name="minBorder">最小值边界</param>
        /// <param name="maxVersion">最大值</param>
        /// <param name="maxBorder">最大值边界</param>
        /// <param name="type">值MineType</param>
        /// <param name="value">值</param>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <returns>返回更新的节点 Task</returns>
        Task<NameNode<TValue>> UpdateByIdAndIf<TValue>(string path, long id, long minVersion, RangeBorder minBorder, long maxVersion,
            RangeBorder maxBorder, ObjectMimeType<TValue> type, TValue value);

        /// <summary>
        /// 如果指定 path 节点存在,同时id等于指定id,并且节点版本在指定区间内, 则更新该租约节点.
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="id">指定 id</param>
        /// <param name="minVersion">最小值</param>
        /// <param name="minBorder">最小值边界</param>
        /// <param name="maxVersion">最大值</param>
        /// <param name="maxBorder">最大值边界</param>
        /// <param name="type">值MineType</param>
        /// <param name="value">值</param>
        /// <param name="lessee">租客</param>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <returns>返回更新的节点 Task</returns>
        Task<NameNode<TValue>> UpdateByIdAndIf<TValue>(string path, long id, long minVersion, RangeBorder minBorder, long maxVersion,
            RangeBorder maxBorder, ObjectMimeType<TValue> type, TValue value, ILessee lessee);

        /// <summary>
        /// 删除指定 path 节点
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>返回删除节点 Task</returns>
        Task<bool> Remove(string path);

        /// <summary>
        /// 删除匹配 path 的所有节点
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>返回删除节点 Task</returns>
        Task<long> RemoveAll(string path);

        /// <summary>
        /// 删除指定 path 节点
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="type">值MineType</param>
        /// <returns>返回删除节点 Task</returns>
        Task<NameNode<TValue>> RemoveAndGet<TValue>(string path, ObjectMimeType<TValue> type);

        /// <summary>
        /// 删除匹配 path 的所有节点
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="type">值MineType</param>
        /// <returns>返回删除节点 Task</returns>
        Task<List<NameNode<TValue>>> RemoveAllAndGet<TValue>(string path, ObjectMimeType<TValue> type);

        /// <summary>
        /// 删除指定 path 节点, 且值等于expect值
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="type">值MineType</param>
        /// <param name="expect">期待值</param>
        /// <returns>返回删除节点 Task</returns>
        Task<NameNode<TValue>> RemoveIf<TValue>(string path, ObjectMimeType<TValue> type, TValue expect);

        /// <summary>
        /// 删除指定 path 节点, 且版本等于version值
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="version">期望版本</param>
        /// <param name="type">值MineType</param>
        /// <returns>返回删除节点 Task</returns>
        Task<NameNode<TValue>> RemoveIf<TValue>(string path, long version, ObjectMimeType<TValue> type);

        /// <summary>
        /// 删除指定 path 节点, 并且节点版本在指定区间内.
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="minVersion">最小值</param>
        /// <param name="minBorder">最小值边界</param> 
        /// <param name="maxVersion">最大值</param>
        /// <param name="maxBorder">最大值边界</param>
        /// <param name="type">值MineType</param>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <returns>返回更新的节点 Task</returns>
        Task<NameNode<TValue>> RemoveIf<TValue>(string path, long minVersion, RangeBorder minBorder, long maxVersion, RangeBorder maxBorder,
            ObjectMimeType<TValue> type);

        /// <summary>
        /// 删除指定 path 节点, 且id等于指定id
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="id">指定 id</param>
        /// <param name="type">值MineType</param>
        /// <returns>返回删除节点 Task</returns>
        Task<NameNode<TValue>> RemoveById<TValue>(string path, long id, ObjectMimeType<TValue> type);

        /// <summary>
        /// 删除指定 path 节点, 且id等于指定id, 且值等于expect值
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="id">指定 id</param>
        /// <param name="expect">期待值</param>
        /// <param name="type">值MineType</param>
        /// <returns>返回删除节点 Task</returns>
        Task<NameNode<TValue>> RemoveByIdAndIf<TValue>(string path, long id, ObjectMimeType<TValue> type, TValue expect);

        /// <summary>
        /// 删除指定 path 节点, 且id等于指定id, 且版本等于version值
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="id">指定 id</param>
        /// <param name="version">期望版本</param>
        /// <param name="type">值MineType</param>
        /// <returns>返回删除节点 Task</returns>
        Task<NameNode<TValue>> RemoveByIdAndIf<TValue>(string path, long id, long version, ObjectMimeType<TValue> type);

        /// <summary>
        /// 删除指定 path 节点, 同时id等于指定id,并且节点版本在指定区间内.
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="id">指定 id</param>
        /// <param name="minVersion">最小值</param>
        /// <param name="minBorder">最小值边界</param> 
        /// <param name="maxVersion">最大值</param>
        /// <param name="maxBorder">最大值边界</param>
        /// <param name="type">值MineType</param>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <returns>返回更新的节点 Task</returns>
        Task<NameNode<TValue>> RemoveByIdAndIf<TValue>(string path, long id, long minVersion, RangeBorder minBorder, long maxVersion,
            RangeBorder maxBorder, ObjectMimeType<TValue> type);

        Task Shutdown();
    }

}
