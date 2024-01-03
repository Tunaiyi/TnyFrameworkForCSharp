// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using dotnet_etcd;
using Etcdserverpb;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Mvccpb;
using TnyFramework.Codec;
using TnyFramework.Namespace.Exceptions;
using CollectionExtensions = TnyFramework.Common.Extensions.CollectionExtensions;

namespace TnyFramework.Namespace.Etcd
{

    public class EtcdObject
    {
        protected readonly EtcdAccessor accessor;

        protected readonly ObjectCodecAdapter objectCodecAdapter;

        internal EtcdAccessor Accessor => accessor;

        public EtcdObject(EtcdAccessor accessor, ObjectCodecAdapter objectCodecAdapter)
        {
            this.accessor = accessor;
            this.objectCodecAdapter = objectCodecAdapter;
        }

        public ByteString Encode<TValue>(TValue value, ObjectMimeType<TValue> type)
        {
            if (value == null)
            {
                return ByteString.Empty;
            }
            var codec = objectCodecAdapter.Codec(type);
            try
            {
                var data = codec.Encode(value);
                return ByteString.CopyFrom(data);
            } catch (IOException e)
            {
                throw new NamespaceNodeCodecException($"encode value {value} exception", e);
            }
        }

        public NameNode<TValue>? DecodeKeyValue<TValue>(RepeatedField<KeyValue> pairs, ObjectMimeType<TValue> type)
        {
            if (CollectionExtensions.IsNullOrEmpty(pairs))
            {
                return null;
            }
            var pair = pairs[0];
            return Decode(pair, type);
        }

        public List<NameNode<TValue>> DecodeAllKeyValues<TValue>(IEnumerable<KeyValue> pairs, ObjectMimeType<TValue> type)
        {
            return pairs.Select(kv => Decode(kv, type)).ToList();
        }

        public Task<TxnResponse> InTxn(Action<TxnRequest> requestInit)
        {
            var request = new TxnRequest();
            requestInit.Invoke(request);
            return accessor.TransactionAsync(request);
        }

        public NameNode<TValue> Decode<TValue>(KeyValue kv, ObjectMimeType<TValue> type)
        {
            return Decode(kv.Value, kv, kv.CreateRevision, kv.Version, type);
        }

        public NameNode<TValue> Decode<TValue>(ByteString kvValue, KeyValue kv, long createRevision, long version, ObjectMimeType<TValue> type)
        {
            var path = kv.Key.ToStringUtf8();
            var codec = objectCodecAdapter.Codec(type);
            try
            {
                var value = codec.Decode(kvValue.ToByteArray());
                var delete = kv.Version == 0;
                return new NameNode<TValue>(path, createRevision, value!, version, kv.ModRevision, delete);
            } catch (IOException e)
            {
                throw new NamespaceNodeCodecException($"decode value {path} exception", e);
            }

        }

        internal static ByteString Key(string key)
        {
            return ByteString.CopyFromUtf8(key);
        }

        internal static ByteString EndKey(string? key)
        {
            var rangeEnd = EtcdClient.GetRangeEnd(key);
            return EtcdClient.GetStringByteForRangeRequests(rangeEnd);
        }

        internal static Compare VersionCompare(ByteString path, long version, Compare.Types.CompareResult result)
        {
            return new Compare {
                Key = path,
                Version = version,
                Target = Compare.Types.CompareTarget.Version,
                Result = result
            };
        }

        internal static Compare RevisionCompare(ByteString path, long revision, Compare.Types.CompareResult result)
        {
            return new Compare {
                Key = path,
                ModRevision = revision,
                Target = Compare.Types.CompareTarget.Mod,
                Result = result
            };
        }

        internal static RequestOp GetOperation(ByteString path)
        {
            return new RequestOp {
                RequestRange = new RangeRequest {
                    Key = path,
                }
            };
        }

        internal static RequestOp PutOperation(ByteString path, ByteString value, ILessee? lessee = null)
        {

            return new RequestOp {
                RequestPut = new PutRequest {
                    Key = path,
                    Value = value,
                    Lease = lessee?.Id ?? 0
                }
            };
        }

        internal static RequestOp DeleteOperation(ByteString path)
        {
            return new RequestOp {
                RequestDeleteRange = new DeleteRangeRequest {
                    Key = path,
                    PrevKv = true
                }
            };
        }
    }

}
