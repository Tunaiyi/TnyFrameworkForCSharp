// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using TnyFramework.Common.Event;
using TnyFramework.Net.Base;
using TnyFramework.Net.Command;
using TnyFramework.Net.Endpoint.Event;
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

    public abstract class EndpointKeeper<TUserId, TEndpoint> : IEndpointKeeper<TUserId, TEndpoint>, INetEndpointKeeper
        where TEndpoint : IEndpoint<TUserId>
    {
        private readonly ConcurrentDictionary<object, TEndpoint> endpointMap = new ConcurrentDictionary<object, TEndpoint>();

        private readonly IEventBus<EndpointKeeperAddEndpoint> addEndpointEvent;

        private readonly IEventBus<EndpointKeeperRemoveEndpoint> removeEndpointEvent;

        public IEventBox<EndpointKeeperAddEndpoint> AddEndpointEvent => addEndpointEvent;

        public IEventBox<EndpointKeeperRemoveEndpoint> RemoveEndpointEvent => removeEndpointEvent;

        public IMessagerType MessagerType { get; }

        public string UserGroup => MessagerType.Group;

        public int Size => endpointMap.Count;

        public EndpointKeeper(IMessagerType messagerType)
        {
            MessagerType = messagerType;
            addEndpointEvent = EndpointKeeper.AddEndpointEventBus.ForkChild();
            removeEndpointEvent = EndpointKeeper.RemoveEndpointEventBus.ForkChild();
        }

        public virtual void Start()
        {
        }

        IEndpoint? IEndpointKeeper.GetEndpoint(object? userId)
        {
            if (userId != null)
                return GetEndpoint((TUserId) userId);
            return null;
        }

        public TEndpoint? GetEndpoint(TUserId userId)
        {
            if (userId == null)
            {
                return default;
            }
            return endpointMap.TryGetValue(userId, out var endpoint) ? endpoint : default;
        }

        IList<IEndpoint> IEndpointKeeper.GetAllEndpoints()
        {
            return endpointMap.Values.Select(e => e as IEndpoint).ToList();
        }

        public IList<TEndpoint> GetAllEndpoints()
        {
            return ImmutableList.CreateRange(endpointMap.Values);
        }

        public void Send2User(object userId, MessageContent content)
        {
            if (endpointMap.TryGetValue(userId, out var endpoint))
            {
                endpoint.Send(content);
            }
        }

        public void Send2Users(IEnumerable<object> userIds, MessageContent content)
        {
            foreach (var userId in userIds)
            {
                Send2User(userId, content);
            }
        }

        public void Send2AllOnline(MessageContent content)
        {
            throw new NotImplementedException();
        }

        public TEndpoint? Close(TUserId userId)
        {
            var endpoint = GetEndpoint(userId);
            if (endpoint != null)
            {
                endpoint.Close();
            }
            return endpoint;
        }

        public IEndpoint? Close(object userId)
        {
            return Close((TUserId) userId);
        }

        public TEndpoint? Offline(TUserId userId)
        {
            var endpoint = GetEndpoint(userId);
            if (endpoint != null)
            {
                endpoint.Offline();
            }
            return endpoint;
        }

        public IEndpoint? Offline(object userId)
        {
            return Offline((TUserId) userId);
        }

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

        public bool IsOnline(TUserId userId)
        {
            var endpoint = GetEndpoint(userId);
            return endpoint != null && endpoint.IsOnline();
        }

        public bool IsOnline(object userId)
        {
            return IsOnline((TUserId) userId);
        }

        protected TEndpoint? FindEndpoint(object uid)
        {
            return endpointMap.TryGetValue(uid, out var endpoint) ? endpoint : default;
        }

        protected bool RemoveEndpoint(object uid, IEndpoint removeOne)
        {
            if (!endpointMap.TryGetValue(uid, out var endpoint) || !ReferenceEquals(endpoint, removeOne))
                return false;
            if (!endpointMap.TryRemove(uid, out endpoint))
                return false;
            removeEndpointEvent.Notify(this, endpoint);
            return true;
        }

        protected TEndpoint? ReplaceEndpoint(object uid, IEndpoint newOne)
        {
            var endpoint = (TEndpoint) newOne;
            var current = default(TEndpoint);
            endpointMap.AddOrUpdate(uid, endpoint, (_, exist) => {
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
            if (!Equals(endpoint.MessagerType, MessagerType))
            {
                return;
            }
            OnEndpointOnline((TEndpoint) endpoint);
        }

        public void NotifyEndpointOffline(IEndpoint endpoint)
        {
            if (!Equals(endpoint.MessagerType, MessagerType))
            {
                return;
            }
            OnEndpointOffline((TEndpoint) endpoint);
        }

        public void NotifyEndpointClose(IEndpoint endpoint)
        {
            if (!Equals(endpoint.MessagerType, MessagerType))
            {
                return;
            }
            var closeOne = (TEndpoint) endpoint;
            if (closeOne.UserId != null && RemoveEndpoint(closeOne.UserId, closeOne))
            {
                OnEndpointClose(closeOne);
            }
        }
    }

}
