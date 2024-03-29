// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_etcd;
using Etcdserverpb;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Microsoft.Extensions.Logging;
using Mvccpb;
using TnyFramework.Codec;
using TnyFramework.Common.Extensions;
using TnyFramework.Common.Logger;
using TnyFramework.Namespace.Exceptions;
using TnyFramework.Namespace.Sharding;

namespace TnyFramework.Namespace.Etcd;

public class EtcdNamespaceExplorer : EtcdObject, INamespaceExplorer
{
    private static readonly ILogger LOGGER = LogFactory.Logger<EtcdNamespaceExplorer>();

    private readonly ConcurrentDictionary<string, EtcdLessee> leasers = new ConcurrentDictionary<string, EtcdLessee>();

    private readonly IList watchers = ArrayList.Synchronized(new List<EtcdNameNodeWatcher>());

    private bool shutdown;

    public EtcdNamespaceExplorer(EtcdAccessor accessor, ObjectCodecAdapter objectCodecAdapter) : base(accessor, objectCodecAdapter)
    {
    }

    public async Task<NameNode<TValue>?> Get<TValue>(string path, ObjectMimeType<TValue> type)
    {
        var response = await accessor.GetAsync(path);
        return response.Count == 0 ? null : DecodeKeyValue(response.Kvs, type);
    }

    public async Task<IList<NameNode<TValue>>> FindAll<TValue>(string path, ObjectMimeType<TValue> type)
    {
        var response = await accessor.GetRangeAsync(path);
        return response.Count == 0 ? Array.Empty<NameNode<TValue>>() : DecodeAllKeyValues(response.Kvs, type);
    }

    public async Task<IList<NameNode<TValue>>> FindAll<TValue>(string from, string to, ObjectMimeType<TValue> type)
    {
        var rangeEnd = EtcdClient.GetRangeEnd(to);
        var request = new RangeRequest {
            Key = Key(from),
            RangeEnd = EndKey(rangeEnd)
        };
        var response = await accessor.GetAsync(request);
        return response.Count == 0 ? Array.Empty<NameNode<TValue>>() : DecodeAllKeyValues(response.Kvs, type);
    }

    public INodeHashing<TNode>? NodeHashing<TNode>(string rootPath, HashingOptions<TNode> options) where TNode : IShardingNode
    {
        return NodeHashing(rootPath, null, options);
    }

    public INodeHashing<TNode>? NodeHashing<TNode>(string rootPath, INodeHashingFactory? factory, HashingOptions<TNode> options)
        where TNode : IShardingNode
    {
        if (rootPath.IsBlank())
        {
            throw new NamespaceHashingException($"rootPath {rootPath} is blank");
        }
        if (factory == null)
        {
            factory = EtcdNodeHashingMultimapFactory.Default;
        }
        return factory.Create(rootPath, options, this, this.objectCodecAdapter);
    }

    public INodeHashing<TNode>? NodeHashing<TNode>(string rootPath, long maxSlotSize, IHasher<string> keyHasher,
        IHasher<IPartitionSlot<TNode>> nodeHasher) where TNode : IShardingNode
    {
        return NodeHashing(rootPath, maxSlotSize, keyHasher, nodeHasher, null, null);
    }

    public INodeHashing<TNode>? NodeHashing<TNode>(string rootPath, long maxSlotSize, IHasher<string> keyHasher,
        IHasher<IPartitionSlot<TNode>> nodeHasher, INodeHashingFactory factory) where TNode : IShardingNode
    {
        return NodeHashing(rootPath, maxSlotSize, keyHasher, nodeHasher, factory, null);
    }

    public INodeHashing<TNode>? NodeHashing<TNode>(string rootPath, long maxSlotSize, IHasher<string> keyHasher,
        IHasher<IPartitionSlot<TNode>> nodeHasher, Action<HashingOptions<TNode>> custom) where TNode : IShardingNode
    {
        return NodeHashing(rootPath, maxSlotSize, keyHasher, nodeHasher, null, custom);
    }

    public INodeHashing<TNode>? NodeHashing<TNode>(string rootPath, long maxSlotSize, IHasher<string> keyHasher,
        IHasher<IPartitionSlot<TNode>> nodeHasher, INodeHashingFactory? factory, Action<HashingOptions<TNode>>? custom)
        where TNode : IShardingNode
    {
        var options = HashingOptions(maxSlotSize, keyHasher, nodeHasher);
        custom?.Invoke(options);
        return NodeHashing(rootPath, factory, options);
    }

    public HashingOptions<TNode> HashingOptions<TNode>(long maxSlotSize, IHasher<string> keyHasher, IHasher<IPartitionSlot<TNode>> nodeHasher)
        where TNode : IShardingNode
    {
        return new HashingOptions<TNode> {
            MaxSlots = maxSlotSize,
            KeyHasher = keyHasher,
            NodeHasher = nodeHasher
        };
    }

    public IHashingSubscriber<TValue> HashingSubscriber<TValue>(string parentPath, long maxSlotSize, ObjectMimeType<TValue> mineType)
    {
        if (parentPath.IsBlank())
        {
            throw new NamespaceHashingException($"rootPath {parentPath} is blank");
        }
        return new EtcdHashingSubscriber<TValue>(parentPath, maxSlotSize, mineType, this);
    }

    public IHashingPublisher<TKey, TValue> HashingPublisher<TKey, TValue>(string parentPath, long maxSlotSize, IHasher<TValue> valueHasher,
        ObjectMimeType<TValue> mineType)
    {
        if (parentPath.IsBlank())
        {
            throw new NamespaceHashingException($"rootPath {parentPath} is blank");
        }
        return new EtcdHashingPublisher<TKey, TValue>(parentPath, maxSlotSize, valueHasher, mineType, this);
    }

    public INameNodesWatcher<TValue> NodeWatcher<TValue>(string path, ObjectMimeType<TValue> type)
    {
        return InitWatcher(new EtcdNameNodesWatcher<TValue>(path, false, accessor, type, objectCodecAdapter));
    }

    public INameNodesWatcher<TValue> AllNodeWatcher<TValue>(string path, ObjectMimeType<TValue> type)
    {
        return InitWatcher(new EtcdNameNodesWatcher<TValue>(path, true, accessor, type, objectCodecAdapter));
    }

    public INameNodesWatcher<TValue> AllNodeWatcher<TValue>(string from, string to, ObjectMimeType<TValue> type)
    {
        return InitWatcher(new EtcdNameNodesWatcher<TValue>(from, to, true, accessor, type, objectCodecAdapter));
    }

    private INameNodesWatcher<TValue> InitWatcher<TValue>(INameNodesWatcher<TValue> watcher)
    {
        watcher.WatchEvent.Add(w => watchers.Add(w));
        watcher.CompleteEvent.Add(w => watchers.Remove(w));
        return watcher;
    }

    public async Task<ILessee> Lease(string name, long ttl)
    {
        if (leasers.TryGetValue(name, out var lessee))
        {
            return lessee;
        }
        lessee = new EtcdLessee(name, accessor, ttl);
        if (!leasers.TryAdd(lessee.Name, lessee))
        {
            return leasers[lessee.Name];
        }
        lessee.CompletedEvent.Add(HandLessee);
        lessee.ErrorEvent.Add(HandLessee);
        await lessee.Lease();
        return lessee;
    }

    public Task<ILessee> Lease(string name)
    {
        return Lease(name, NamespaceConstants.DEFAULT_TTL);
    }

    public Task<NameNode<TValue>> GetOrAdd<TValue>(string path, ObjectMimeType<TValue> type, TValue value)
    {
        return DoAdd(path, type, value, true);
    }

    public Task<NameNode<TValue>> GetOrAdd<TValue>(string path, ObjectMimeType<TValue> type, TValue value, ILessee? lessee)
    {
        return DoAdd(path, type, value, true, lessee);
    }

    public Task<NameNode<TValue>> Add<TValue>(string path, ObjectMimeType<TValue> type, TValue value)
    {
        return DoAdd(path, type, value, false);
    }

    public Task<NameNode<TValue>> Add<TValue>(string path, ObjectMimeType<TValue> type, TValue value, ILessee? lessee)
    {
        return DoAdd(path, type, value, false, lessee);
    }

    public Task<NameNode<TValue>> Save<TValue>(string path, ObjectMimeType<TValue> type, TValue value)
    {
        return DoSave(path, type, value);
    }

    public Task<NameNode<TValue>> Save<TValue>(string path, ObjectMimeType<TValue> type, TValue value, ILessee? lessee)
    {
        return DoSave(path, type, value, lessee);
    }

    public Task<NameNode<TValue>> Update<TValue>(string path, ObjectMimeType<TValue> type, TValue value)
    {
        return DoUpdate(path, type, value, null, null, default, null!, 0L, RangeBorder.Unlimited, 0L, RangeBorder.Unlimited);
    }

    public Task<NameNode<TValue>> Update<TValue>(string path, ObjectMimeType<TValue> type, TValue value, ILessee? lessee)
    {
        return DoUpdate(path, type, value, lessee, null, default, null!, 0L, RangeBorder.Unlimited, 0L, RangeBorder.Unlimited);
    }

    public Task<NameNode<TValue>> UpdateIf<TValue>(string path, ObjectMimeType<TValue> type, TValue expect, TValue value)
    {
        return DoUpdate(path, type, value, null, null, expect, null!, 0L, RangeBorder.Unlimited, 0L, RangeBorder.Unlimited);
    }

    public Task<NameNode<TValue>> UpdateIf<TValue>(string path, ObjectMimeType<TValue> type, TValue expect, TValue value, ILessee? lessee)
    {
        return DoUpdate(path, type, value, lessee, null, expect, null!, 0L, RangeBorder.Unlimited, 0L, RangeBorder.Unlimited);
    }

    public Task<NameNode<TValue>> UpdateIfVersion<TValue>(string path, long version, ObjectMimeType<TValue> type, TValue value)
    {
        return DoUpdate(path, type, value, null, null, default, VersionCompare, version, RangeBorder.Close, version, RangeBorder.Close);
    }

    public Task<NameNode<TValue>> UpdateIfVersion<TValue>(string path, long version, ObjectMimeType<TValue> type, TValue value, ILessee? lessee)
    {
        return DoUpdate(path, type, value, lessee, null, default, VersionCompare, version, RangeBorder.Close, version, RangeBorder.Close);
    }

    public Task<NameNode<TValue>> UpdateIfVersion<TValue>(string path, long minVersion, RangeBorder minBorder, long maxVersion,
        RangeBorder maxBorder,
        ObjectMimeType<TValue> type, TValue value)
    {
        return DoUpdate(path, type, value, null, null, default, VersionCompare, minVersion, minBorder, maxVersion, maxBorder);
    }

    public Task<NameNode<TValue>> UpdateIfVersion<TValue>(string path, long minVersion, RangeBorder minBorder, long maxVersion,
        RangeBorder maxBorder,
        ObjectMimeType<TValue> type, TValue value,
        ILessee? lessee)
    {
        return DoUpdate(path, type, value, lessee, null, default, VersionCompare, minVersion, minBorder, maxVersion, maxBorder);
    }

    public Task<NameNode<TValue>> UpdateIfRevision<TValue>(string path, long revision, ObjectMimeType<TValue> type, TValue value)
    {
        return DoUpdate(path, type, value, null, null, default, RevisionCompare, revision, RangeBorder.Close, revision, RangeBorder.Close);
    }

    public Task<NameNode<TValue>> UpdateIfRevision<TValue>(string path, long revision, ObjectMimeType<TValue> type, TValue value, ILessee? lessee)
    {
        return DoUpdate(path, type, value, lessee, null, default, RevisionCompare, revision, RangeBorder.Close, revision, RangeBorder.Close);
    }

    public Task<NameNode<TValue>> UpdateIfRevision<TValue>(string path, long minRevision, RangeBorder minBorder, long maxRevision,
        RangeBorder maxBorder, ObjectMimeType<TValue> type,
        TValue value)
    {
        return DoUpdate(path, type, value, null, null, default, RevisionCompare, minRevision, minBorder, maxRevision, maxBorder);
    }

    public Task<NameNode<TValue>> UpdateIfRevision<TValue>(string path, long minRevision, RangeBorder minBorder, long maxRevision,
        RangeBorder maxBorder, ObjectMimeType<TValue> type,
        TValue value, ILessee? lessee)
    {
        return DoUpdate(path, type, value, lessee, null, default, RevisionCompare, minRevision, minBorder, maxRevision, maxBorder);
    }

    public Task<NameNode<TValue>> UpdateById<TValue>(string path, long id, ObjectMimeType<TValue> type, TValue value)
    {
        return DoUpdate(path, type, value, null, id, default, null!, 0L, RangeBorder.Unlimited, 0L, RangeBorder.Unlimited);
    }

    public Task<NameNode<TValue>> UpdateById<TValue>(string path, long id, ObjectMimeType<TValue> type, TValue value, ILessee? lessee)
    {
        return DoUpdate(path, type, value, lessee, id, default, null!, 0L, RangeBorder.Unlimited, 0L, RangeBorder.Unlimited);
    }

    public Task<NameNode<TValue>> UpdateByIdAndIf<TValue>(string path, long id, ObjectMimeType<TValue> type, TValue expect, TValue value)
    {
        return DoUpdate(path, type, value, null, id, expect, null!, 0L, RangeBorder.Unlimited, 0L, RangeBorder.Unlimited);
    }

    public Task<NameNode<TValue>> UpdateByIdAndIf<TValue>(string path, long id, ObjectMimeType<TValue> type, TValue expect, TValue value,
        ILessee? lessee)
    {
        return DoUpdate(path, type, value, lessee, id, expect, null!, 0L, RangeBorder.Unlimited, 0L, RangeBorder.Unlimited);
    }

    public Task<NameNode<TValue>> UpdateByIdAndIfVersion<TValue>(string path, long id, long version, ObjectMimeType<TValue> type, TValue value)
    {
        return DoUpdate(path, type, value, null, id, default, VersionCompare, version, RangeBorder.Close, version, RangeBorder.Close);
    }

    public Task<NameNode<TValue>> UpdateByIdAndIfVersion<TValue>(string path, long id, long version, ObjectMimeType<TValue> type, TValue value,
        ILessee? lessee)
    {
        return DoUpdate(path, type, value, lessee, id, default, VersionCompare, version, RangeBorder.Close, version, RangeBorder.Close);
    }

    public Task<NameNode<TValue>> UpdateByIdAndIfVersion<TValue>(string path, long id,
        long minVersion, RangeBorder minBorder, long maxVersion, RangeBorder maxBorder, ObjectMimeType<TValue> type, TValue value)
    {
        return DoUpdate(path, type, value, null, id, default, VersionCompare, minVersion, minBorder, maxVersion, maxBorder);
    }

    public Task<NameNode<TValue>> UpdateByIdAndIfVersion<TValue>(string path, long id,
        long minVersion, RangeBorder minBorder, long maxVersion, RangeBorder maxBorder, ObjectMimeType<TValue> type, TValue value,
        ILessee? lessee)
    {
        return DoUpdate(path, type, value, lessee, id, default, VersionCompare, minVersion, minBorder, maxVersion, maxBorder);
    }

    public Task<NameNode<TValue>> UpdateByIdAndIfRevision<TValue>(string path, long id,
        long revision, ObjectMimeType<TValue> type, TValue value)
    {
        return DoUpdate(path, type, value, null, id, default, RevisionCompare, revision, RangeBorder.Close, revision, RangeBorder.Close);
    }

    public Task<NameNode<TValue>> UpdateByIdAndIfRevision<TValue>(string path, long id,
        long revision, ObjectMimeType<TValue> type, TValue value, ILessee? lessee)
    {
        return DoUpdate(path, type, value, lessee, id, default, RevisionCompare, revision, RangeBorder.Close, revision, RangeBorder.Close);
    }

    public Task<NameNode<TValue>> UpdateByIdAndIfRevision<TValue>(string path, long id,
        long minRevision, RangeBorder minBorder, long maxRevision, RangeBorder maxBorder, ObjectMimeType<TValue> type, TValue value)
    {
        return DoUpdate(path, type, value, null, id, default, RevisionCompare, minRevision, minBorder, maxRevision, maxBorder);
    }

    public Task<NameNode<TValue>> UpdateByIdAndIfRevision<TValue>(string path, long id,
        long minRevision, RangeBorder minBorder, long maxRevision, RangeBorder maxBorder, ObjectMimeType<TValue> type, TValue value,
        ILessee? lessee)
    {
        return DoUpdate(path, type, value, lessee, id, default, RevisionCompare, minRevision, minBorder, maxRevision, maxBorder);
    }

    public async Task<bool> Remove(string path)
    {
        var response = await accessor.DeleteAsync(path);
        return response.Deleted > 0;
    }

    public async Task<long> RemoveAll(string path)
    {
        var response = await accessor.DeleteAsync(new DeleteRangeRequest {
            Key = Key(path),
            RangeEnd = EndKey(path)
        });
        return response.Deleted;
    }

    public Task<NameNode<TValue>> RemoveAndGet<TValue>(string path, ObjectMimeType<TValue> type)
    {
        return DoDelete(path, type, null, default, null!, 0L, RangeBorder.Unlimited, 0L, RangeBorder.Unlimited);
    }

    public async Task<List<NameNode<TValue>>> RemoveAllAndGet<TValue>(string path, ObjectMimeType<TValue> type)
    {
        var response = await accessor.DeleteAsync(new DeleteRangeRequest {
            Key = Key(path),
            RangeEnd = EndKey(path),
            PrevKv = true
        });
        return DecodeAllKeyValues(response.PrevKvs, type);
    }

    public Task<NameNode<TValue>> RemoveIf<TValue>(string path, ObjectMimeType<TValue> type, TValue expect)
    {
        return DoDelete(path, type, null, expect, null!, 0L, RangeBorder.Unlimited, 0L, RangeBorder.Unlimited);
    }

    public Task<NameNode<TValue>> RemoveIfVersion<TValue>(string path, long version, ObjectMimeType<TValue> type)
    {
        return DoDelete(path, type, null, default, VersionCompare, version, RangeBorder.Close, version, RangeBorder.Close);
    }

    public Task<NameNode<TValue>> RemoveIfVersion<TValue>(string path, long minVersion, RangeBorder minBorder, long maxVersion,
        RangeBorder maxBorder,
        ObjectMimeType<TValue> type)
    {
        return DoDelete(path, type, null, default, VersionCompare, minVersion, minBorder, maxVersion, maxBorder);
    }

    public Task<NameNode<TValue>> RemoveIfRevision<TValue>(string path, long revision, ObjectMimeType<TValue> type)
    {
        return DoDelete(path, type, null, default, RevisionCompare, revision, RangeBorder.Close, revision, RangeBorder.Close);
    }

    public Task<NameNode<TValue>> RemoveIfRevision<TValue>(string path, long minRevision, RangeBorder minBorder, long maxRevision,
        RangeBorder maxBorder, ObjectMimeType<TValue> type)
    {
        return DoDelete(path, type, null, default, RevisionCompare, minRevision, minBorder, maxRevision, maxBorder);
    }

    public Task<NameNode<TValue>> RemoveById<TValue>(string path, long id, ObjectMimeType<TValue> type)
    {
        return DoDelete(path, type, id, default, null!, 0L, RangeBorder.Unlimited, 0L, RangeBorder.Unlimited);
    }

    public Task<NameNode<TValue>> RemoveByIdAndIf<TValue>(string path, long id, ObjectMimeType<TValue> type, TValue expect)
    {
        return DoDelete(path, type, id, expect, null!, 0L, RangeBorder.Unlimited, 0L, RangeBorder.Unlimited);
    }

    public Task<NameNode<TValue>> RemoveByIdAndIfVersion<TValue>(string path, long id, long version, ObjectMimeType<TValue> type)
    {
        return DoDelete(path, type, id, default, VersionCompare, version, RangeBorder.Close, version, RangeBorder.Close);
    }

    public Task<NameNode<TValue>> RemoveByIdAndIfVersion<TValue>(string path, long id, long minVersion, RangeBorder minBorder, long maxVersion,
        RangeBorder maxBorder,
        ObjectMimeType<TValue> type)
    {
        return DoDelete(path, type, id, default, VersionCompare, minVersion, minBorder, maxVersion, maxBorder);
    }

    public Task<NameNode<TValue>> RemoveByIdAndIfRevision<TValue>(string path, long id, long revision, ObjectMimeType<TValue> type)
    {
        return DoDelete(path, type, id, default, RevisionCompare, revision, RangeBorder.Close, revision, RangeBorder.Close);
    }

    public Task<NameNode<TValue>> RemoveByIdAndIfRevision<TValue>(string path, long id, long minRevision, RangeBorder minBorder, long maxRevision,
        RangeBorder maxBorder,
        ObjectMimeType<TValue> type)
    {
        return DoDelete(path, type, id, default, RevisionCompare, minRevision, minBorder, maxRevision, maxBorder);
    }

    public async Task Shutdown()
    {
        if (shutdown)
            return;
        shutdown = true;
        var leasersTasks = leasers.Values.Select(leaser => leaser.Shutdown()).ToList();
        foreach (var value in watchers)
        {
            if (value is EtcdNameNodeWatcher watcher)
            {
                leasersTasks.Add(watcher.Unwatch());
            }
        }
        foreach (var task in leasersTasks)
        {
            try
            {
                await task;
            } catch (Exception e)
            {
                LOGGER.LogError(e, "");
            }
        }
    }

    private async Task<NameNode<TValue>> DoAdd<TValue>(string path, ObjectMimeType<TValue> type, TValue value, bool loadIfExist,
        ILessee? lessee = null)
    {
        var response = await InTxn(txn => {
            var key = Key(path);
            var valueBytes = Encode(value, type);
            txn.Compare.Add(VersionCompare(key, 0, Compare.Types.CompareResult.Equal));
            txn.Success.Add(new[] {
                PutOperation(key, valueBytes, lessee),
                GetOperation(key)
            });
            if (loadIfExist)
            {
                txn.Failure.Add(GetOperation(key));
            }
        });
        return DecodeGetRangeFirstKeyValue(type, response);
    }

    private async Task<NameNode<TValue>> DoSave<TValue>(string path, ObjectMimeType<TValue> type, TValue value, ILessee? lessee = null)
    {
        var key = Key(path);
        var valueBytes = Encode(value, type);
        var response = await InTxn(txn => {
            txn.Success.Add(new[] {
                PutOperation(key, valueBytes, lessee),
                GetOperation(key)
            });
        });
        return DecodeGetRangeFirstKeyValue(type, response);
    }

    private async Task<NameNode<TValue>> DoUpdate<TValue>(
        string path, ObjectMimeType<TValue> type, TValue value, ILessee? lessee, long? id, TValue? expect,
        Func<ByteString, long, Compare.Types.CompareResult, Compare> verCompare,
        long minVersion, RangeBorder minBorder, long maxVersion, RangeBorder maxBorder)
    {
        var response = await InTxn(txn => {
            var key = Key(path);
            var valueBytes = Encode(value, type);
            var expectBytes = expect == null ? null : Encode(expect, type);
            var getOp = GetOperation(key);
            var putOp = PutOperation(key, valueBytes, lessee);
            CreateCompareList(txn.Compare, id, expectBytes, key, verCompare, minVersion, minBorder, maxVersion, maxBorder);
            txn.Success.Add(putOp);
            txn.Success.Add(getOp);
        });
        return DecodeGetRangeFirstKeyValue(type, response);
    }

    private async Task<NameNode<TValue>> DoDelete<TValue>(string path, ObjectMimeType<TValue> type, long? id, TValue? expect,
        Func<ByteString, long, Compare.Types.CompareResult, Compare> verCompare,
        long minVersion, RangeBorder minBorder, long maxVersion, RangeBorder maxBorder)
    {
        var response = await InTxn(txn => {
            var key = Key(path);
            var expectBytes = expect == null ? null : Encode(expect, type);
            CreateCompareList(txn.Compare, id, expectBytes, key, verCompare, minVersion, minBorder, maxVersion, maxBorder);
            txn.Success.Add(DeleteOperation(key));
        });
        return DecodeDeletePrevFirstKeyValue(type, response);
    }

    private static void CreateCompareList(ICollection<Compare> compares, long? id, ByteString? expect, ByteString key,
        Func<ByteString, long, Compare.Types.CompareResult, Compare> verCompare,
        long minVersion, RangeBorder minBorder, long maxVersion, RangeBorder maxBorder)
    {
        VerCompareList(compares, key, verCompare, minVersion, minBorder, maxVersion, maxBorder);
        if (id != null)
        {
            compares.Add(new Compare {
                Key = key,
                CreateRevision = id.Value,
                Target = Compare.Types.CompareTarget.Create,
                Result = Compare.Types.CompareResult.Equal
            });
        }
        if (expect != null)
        {
            compares.Add(new Compare {
                Key = key,
                Value = expect,
                Target = Compare.Types.CompareTarget.Value,
                Result = Compare.Types.CompareResult.Equal
            });
        }
        if (compares.IsNullOrEmpty())
        {
            compares.Add(new Compare {
                Key = key,
                Version = 0,
                Target = Compare.Types.CompareTarget.Version,
                Result = Compare.Types.CompareResult.Greater
            });
        }
    }

    private static void VerCompareList(ICollection<Compare> compares, ByteString key,
        Func<ByteString, long, Compare.Types.CompareResult, Compare> verCompare,
        long minVersion, RangeBorder minBorder, long maxVersion, RangeBorder maxBorder)
    {
        if (minBorder == RangeBorder.Unlimited && maxBorder == RangeBorder.Unlimited)
        {
            return;
        }
        if (minBorder != RangeBorder.Unlimited && maxBorder != RangeBorder.Unlimited && maxVersion < minVersion)
        {
            throw new NamespaceNodeException($"minVersion : {minVersion}, maxVersion : {maxVersion}, maxVersion must > or = minVersion");
        }

        if (minBorder == RangeBorder.Close && maxBorder == RangeBorder.Close && minVersion == maxVersion)
        {
            compares.Add(verCompare(key, minVersion, Compare.Types.CompareResult.Equal));
        } else
        {
            switch (minBorder)
            {
                case RangeBorder.Open:
                    compares.Add(verCompare(key, minVersion, Compare.Types.CompareResult.Greater));
                    break;
                case RangeBorder.Close:
                    compares.Add(verCompare(key, minVersion - 1, Compare.Types.CompareResult.Greater));
                    break;
                case RangeBorder.Unlimited:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(minBorder), minBorder, null);
            }
            switch (maxBorder)
            {
                case RangeBorder.Open:
                    compares.Add(verCompare(key, maxVersion, Compare.Types.CompareResult.Less));
                    break;
                case RangeBorder.Close:
                    compares.Add(verCompare(key, maxVersion + 1, Compare.Types.CompareResult.Less));
                    break;
                case RangeBorder.Unlimited:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(maxBorder), maxBorder, null);
            }
        }
    }

    public NameNode<TValue> DecodeGetRangeFirstKeyValue<TValue>(ObjectMimeType<TValue> type, TxnResponse txnResponse)
    {
        return DecodeFirstKeyValue(type, txnResponse, t => t.ResponseRange, r => r.Kvs);
    }

    public NameNode<TValue> DecodeDeletePrevFirstKeyValue<TValue>(ObjectMimeType<TValue> type, TxnResponse txnResponse)
    {
        return DecodeFirstKeyValue(type, txnResponse, t => t.ResponseDeleteRange, r => r.PrevKvs);
    }

    private NameNode<TValue> DecodeFirstKeyValue<TValue, TResponse>(ObjectMimeType<TValue> type, TxnResponse txnResponse,
        Func<ResponseOp, TResponse> txnToResponse, Func<TResponse, RepeatedField<KeyValue>> keyValueFunc)
    {
        return (from responseOp in txnResponse.Responses
                let response = txnToResponse(responseOp)
                where response != null
                select DecodeKeyValue(keyValueFunc(response), type))
            .FirstOrDefault()!;
    }

    private void HandLessee(ILessee source, Exception exception)
    {
        LOGGER.LogWarning(exception, "Lessee {id} exception", source.Id);
        HandLessee(source);
    }

    private void HandLessee(ILessee source)
    {
        if (!leasers.TryGetValue(source.Name, out var lessee))
        {
            return;
        }
        if (lessee.IsPause())
        {
            Resume(lessee, 3000);
            return;
        }
        if (lessee.IsShutdown())
        {
            leasers.TryRemove(source.Name, out _);
        }
    }

    private static void Resume(EtcdLessee lessee, int delay)
    {
        Task.Run(async () => {
            while (lessee.IsPause())
            {
                if (await lessee.Resume())
                {
                    return;
                }
                await Task.Delay(delay);
            }
        });
    }
}
