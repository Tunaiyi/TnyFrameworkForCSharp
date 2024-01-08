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
using TnyFramework.Common.EventBus;
using TnyFramework.Net.Application;
using TnyFramework.Net.Message;
using TnyFramework.Net.Session.Event;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Session
{

    public static class SessionKeeperEvents
    {
        internal static IEventBus<SessionKeeperAdd> AddSessionEvent { get; } = EventBuses.Create<SessionKeeperAdd>();

        internal static IEventBus<SessionKeeperRemove> RemoveSessionEvent { get; } = EventBuses.Create<SessionKeeperRemove>();

        /// <summary>
        /// 激活事件总线, 可监听到所有 SessionKeeper 的事件
        /// </summary>
        public static IEventWatch<SessionKeeperAdd> AddSessionEventWatch => AddSessionEvent;

        /// <summary>
        /// 断线事件总线, 可监听到所有 SessionKeeper 的事件
        /// </summary>
        public static IEventWatch<SessionKeeperRemove> RemoveSessionEventWatch => RemoveSessionEvent;
    }

    public abstract class SessionKeeper<TSession> : INetSessionKeeper
        where TSession : ISession
    {
        private readonly ConcurrentDictionary<object, TSession> sessionMap = new ConcurrentDictionary<object, TSession>();

        private readonly IEventBus<SessionKeeperAdd> addSessionEvent;

        private readonly IEventBus<SessionKeeperRemove> removeSessionEvent;

        public IEventWatch<SessionKeeperAdd> AddSessionEvent => addSessionEvent;

        public IEventWatch<SessionKeeperRemove> RemoveSessionEvent => removeSessionEvent;

        public IContactType ContactType { get; }

        public string ContactGroup => ContactType.Group;

        public int Size => sessionMap.Count;

        public SessionKeeper(IContactType contactType)
        {
            ContactType = contactType;
            addSessionEvent = SessionKeeperEvents.AddSessionEvent.ForkChild();
            removeSessionEvent = SessionKeeperEvents.RemoveSessionEvent.ForkChild();
        }

        public virtual void Start()
        {
        }

        public TSession? GetSession(long identify)
        {
            return sessionMap.GetValueOrDefault(identify);
        }

        ISession? ISessionKeeper.GetSession(long identify)
        {
            return sessionMap.GetValueOrDefault(identify);
        }

        IList<ISession> ISessionKeeper.GetAllSessions()
        {
            return sessionMap.Values.Select(e => e as ISession).ToList();
        }

        public IList<TSession> GetAllSessions()
        {
            return ImmutableList.CreateRange(sessionMap.Values);
        }

        public void Send2User(long identify, MessageContent content)
        {
            if (sessionMap.TryGetValue(identify, out var session))
            {
                session.Send(content);
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
            foreach (var session in sessionMap.Values)
            {
                session.Send(content);
            }
        }

        private TSession? DoClose(long identify)
        {
            var session = GetSession(identify);
            if (session != null)
            {
                session.Close();
            }
            return session;
        }

        private TSession? DoOffline(long identify)
        {
            var session = GetSession(identify);
            if (session != null)
            {
                session.Offline();
            }
            return session;
        }

        public ISession? Close(long identify) => DoClose(identify);

        public ISession? Offline(long identify) => DoOffline(identify);

        public void OfflineAll()
        {
            foreach (var pair in sessionMap)
            {
                pair.Value.Offline();
            }
        }

        public void CloseAll()
        {
            foreach (var pair in sessionMap)
            {
                pair.Value.Close();
            }
        }

        public int OnlineSize => sessionMap.Values.Count(session => session.IsOnline());

        public abstract ISession Online(ICertificate certificate, INetTunnel tunnel);

        public bool IsOnline(long identify)
        {
            var session = GetSession(identify);
            return session != null && session.IsOnline();
        }

        protected TSession? FindSession(long identify)
        {
            return sessionMap.GetValueOrDefault(identify);
        }

        protected bool RemoveSession(long identify, ISession removeOne)
        {
            if (!sessionMap.TryGetValue(identify, out var session) || !ReferenceEquals(session, removeOne))
                return false;
            if (!sessionMap.TryRemove(identify, out session))
                return false;
            removeSessionEvent.Notify(this, session);
            return true;
        }

        protected TSession? ReplaceSession(long identify, ISession newOne)
        {
            var session = (TSession) newOne;
            var current = default(TSession);
            sessionMap.AddOrUpdate(identify, session, (_, exist) => {
                if (ReferenceEquals(session, exist))
                    return session;
                exist.Close();
                removeSessionEvent.Notify(this, exist);
                current = exist;
                return session;
            });
            addSessionEvent.Notify(this, newOne);
            return current;
        }

        protected virtual void OnSessionOnline(TSession session)
        {

        }

        protected virtual void OnSessionOffline(TSession session)
        {

        }

        protected virtual void OnSessionClose(TSession session)
        {

        }

        public void NotifySessionOnline(ISession session)
        {
            if (!Equals(session.ContactType, ContactType))
            {
                return;
            }
            OnSessionOnline((TSession) session);
        }

        public void NotifySessionOffline(ISession session)
        {
            if (!Equals(session.ContactType, ContactType))
            {
                return;
            }
            OnSessionOffline((TSession) session);
        }

        public void NotifySessionClose(ISession session)
        {
            if (!Equals(session.ContactType, ContactType))
            {
                return;
            }
            var closeOne = (TSession) session;
            if (RemoveSession(closeOne.Identify, closeOne))
            {
                OnSessionClose(closeOne);
            }
        }
    }

}
