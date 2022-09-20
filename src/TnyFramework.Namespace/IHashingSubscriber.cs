// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

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
