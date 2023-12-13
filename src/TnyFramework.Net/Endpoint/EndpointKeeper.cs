// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using TnyFramework.Common.Event;
using TnyFramework.Net.Base;
using TnyFramework.Net.Endpoint.Event;
using TnyFramework.Net.Message;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Endpoint
{

    public static class EndpointKeeper
    {
        internal static IEventBus<EndpointKeeperAddEndpoint> AddEndpointEventBus { get; } = EventBuses.Create<EndpointKeeperAddEndpoint>();

        internal static IEventBus<EndpointKeeperRemoveEndpoint> RemoveEndpointEventBus { get; } = EventBuses.Create<EndpointKeeperRemoveEndpoint>();

        /// <summary>
        /// 激活事件总线, 可监听到所有 EndpointKeeper 的事件
        /// </summary>
        public static IEventBox<EndpointKeeperAddEndpoint> AddEndpointEventBox => AddEndpointEventBus;

        /// <summary>
        /// 断线事件总线, 可监听到所有 EndpointKeeper 的事件
        /// </summary>
        public static IEventBox<EndpointKeeperRemoveEndpoint> RemoveEndpointEventBox => RemoveEndpointEventBus;
    }

    public abstract class EndpointKeeper<TEndpoint> : IEndpointKeeper<TEndpoint>, INetEndpointKeeper
        where TEndpoint : IEndpoint
    {
        private readonly ConcurrentDictionary<object, TEndpoint> endpointMap = new ConcurrentDictionary<object, TEndpoint>();

        private readonly IEventBus<EndpointKeeperAddEndpoint> addEndpointEvent;

        private readonly IEventBus<EndpointKeeperRemoveEndpoint> removeEndpointEvent;

        public IEventBox<EndpointKeeperAddEndpoint> AddEndpointEvent => addEndpointEvent;

        public IEventBox<EndpointKeeperRemoveEndpoint> RemoveEndpointEvent => removeEndpointEvent;

        public IContactType ContactType { get; }

        public string ContactGroup => ContactType.Group;

        public int Size => endpointMap.Count;

        public EndpointKeeper(IContactType contactType)
        {
            ContactType = contactType;
            addEndpointEvent = EndpointKeeper.AddEndpointEventBus.ForkChild();
            removeEndpointEvent = EndpointKeeper.RemoveEndpointEventBus.ForkChild();
        }

        public virtual void Start()
        {
        }

        public TEndpoint? GetEndpoint(long identify)
        {
            return endpointMap.GetValueOrDefault(identify);
        }

        IEndpoint? IEndpointKeeper.GetEndpoint(long identify)
        {
            return endpointMap.GetValueOrDefault(identify);
        }

        IList<IEndpoint> IEndpointKeeper.GetAllEndpoints()
        {
            return endpointMap.Values.Select(e => e as IEndpoint).ToList();
        }

        public IList<TEndpoint> GetAllEndpoints()
        {
            return ImmutableList.CreateRange(endpointMap.Values);
        }

        public void Send2User(long identify, MessageContent content)
        {
            if (endpointMap.TryGetValue(identify, out var endpoint))
            {
                endpoint.Send(content);
            }
        }

        public void Send2Users(IEnumerable<long> identifies, MessageContent content)
        {
            foreach (var identify in identifies)
            {
                Send2User(identify, content);
            }
        }

        public void Send2AllOnline(MessageContent content)
        {
            foreach (var endpoint in endpointMap.Values)
            {
                endpoint.Send(content);
            }
        }

        private TEndpoint? DoClose(long identify)
        {
            var endpoint = GetEndpoint(identify);
            if (endpoint != null)
            {
                endpoint.Close();
            }
            return endpoint;
        }

        private TEndpoint? DoOffline(long identify)
        {
            var endpoint = GetEndpoint(identify);
            if (endpoint != null)
            {
                endpoint.Offline();
            }
            return endpoint;
        }

        TEndpoint? IEndpointKeeper<TEndpoint>.Close(long identify) => DoClose(identify);

        public IEndpoint? Close(long identify) => DoClose(identify);

        TEndpoint? IEndpointKeeper<TEndpoint>.Offline(long identify) => DoOffline(identify);

        public IEndpoint? Offline(long identify) => DoOffline(identify);

        public void OfflineAll()
        {
            foreach (var pair in endpointMap)
            {
                pair.Value.Offline();
            }
        }

        public void CloseAll()
        {
            foreach (var pair in endpointMap)
            {
                pair.Value.Close();
            }
        }

        public int OnlineSize => endpointMap.Values.Count(endpoint => endpoint.IsOnline());

        public abstract IEndpoint Online(ICertificate certificate, INetTunnel tunnel);

        public bool IsOnline(long identify)
        {
            var endpoint = GetEndpoint(identify);
            return endpoint != null && endpoint.IsOnline();
        }

        protected TEndpoint? FindEndpoint(long identify)
        {
            return endpointMap.GetValueOrDefault(identify);
        }

        protected bool RemoveEndpoint(long identify, IEndpoint removeOne)
        {
            if (!endpointMap.TryGetValue(identify, out var endpoint) || !ReferenceEquals(endpoint, removeOne))
                return false;
            if (!endpointMap.TryRemove(identify, out endpoint))
                return false;
            removeEndpointEvent.Notify(this, endpoint);
            return true;
        }

        protected TEndpoint? ReplaceEndpoint(long identify, IEndpoint newOne)
        {
            var endpoint = (TEndpoint) newOne;
            var current = default(TEndpoint);
            endpointMap.AddOrUpdate(identify, endpoint, (_, exist) => {
                if (ReferenceEquals(endpoint, exist))
                    return endpoint;
                exist.Close();
                removeEndpointEvent.Notify(this, exist);
                current = exist;
                return endpoint;
            });
            addEndpointEvent.Notify(this, newOne);
            return current;
        }

        protected virtual void OnEndpointOnline(TEndpoint endpoint)
        {

        }

        protected virtual void OnEndpointOffline(TEndpoint endpoint)
        {

        }

        protected virtual void OnEndpointClose(TEndpoint endpoint)
        {

        }

        public void NotifyEndpointOnline(IEndpoint endpoint)
        {
            if (!Equals(endpoint.ContactType, ContactType))
            {
                return;
            }
            OnEndpointOnline((TEndpoint) endpoint);
        }

        public void NotifyEndpointOffline(IEndpoint endpoint)
        {
            if (!Equals(endpoint.ContactType, ContactType))
            {
                return;
            }
            OnEndpointOffline((TEndpoint) endpoint);
        }

        public void NotifyEndpointClose(IEndpoint endpoint)
        {
            if (!Equals(endpoint.ContactType, ContactType))
            {
                return;
            }
            var closeOne = (TEndpoint) endpoint;
            if (RemoveEndpoint(closeOne.Identify, closeOne))
            {
                OnEndpointClose(closeOne);
            }
        }
    }

}
