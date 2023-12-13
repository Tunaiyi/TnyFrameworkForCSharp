// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;
using TnyFramework.Coroutines.Async;
using TnyFramework.Net.Base;
using TnyFramework.Net.Command;
using TnyFramework.Net.Common;
using TnyFramework.Net.Exceptions;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Endpoint
{

    public class SessionKeeper
    {
        internal static readonly ILogger LOGGER = LogFactory.Logger<SessionKeeper>();
    }

    public class SessionKeeper<TUserId> : EndpointKeeper<TUserId, ISession<TUserId>>, ISessionKeeper<TUserId>
    {
        /* 离线session */
        private readonly ConcurrentDictionary<ISession, bool> offlineSessionQueue = new ConcurrentDictionary<ISession, bool>();

        private readonly ISessionFactory factory;

        private readonly ISessionKeeperSetting setting;

        private readonly object[] locks;

        private ICoroutine? coroutine;

        private int start;

        private ILogger Logger => SessionKeeper.LOGGER;

        public SessionKeeper(IContactType contactType, ISessionFactory factory, ISessionKeeperSetting setting) : base(contactType)
        {
            this.factory = factory;
            this.setting = setting;
            coroutine = null!;
            locks = new object[1024];
            for (var i = locks.Length - 1; i >= 0; i--)
            {
                locks[i] = new object();
            }
        }

        public override void Start()
        {
            if (Interlocked.CompareExchange(ref start, 1, 0) != 0)
                return;
            if (coroutine != null)
                return;
            coroutine = DefaultCoroutineFactory.Default.Create("SessionKeeperClearInvalidedSession");
            coroutine.AsyncExec(async () => {
                try
                {
                    ClearInvalidedSession();
                    await Task.Delay(TimeSpan.FromMilliseconds(setting.ClearSessionInterval));
                } catch (Exception e)
                {
                    Logger.LogError(e, "");
                }
            });
        }

        public override IEndpoint Online(ICertificate certificate, INetTunnel tunnel)
        {
            if (!certificate.IsAuthenticated())
            {
                throw new AuthFailedException(NetResultCode.AUTH_FAIL_ERROR, null, $"cert {certificate} is unauthentic");
            }
            if (!Equals(ContactType, certificate.ContactType))
            {
                throw new AuthFailedException(NetResultCode.AUTH_FAIL_ERROR, null,
                    $"cert {certificate} userType is {certificate.UserGroup}, not {ContactType}");
            }
            var uid = certificate.GetUserId();
            if (uid == null)
            {
                throw new NullReferenceException("uid is null");
            }
            var index = Math.Abs(uid.GetHashCode()) % locks.Length;
            lock (locks[index])
            {
                return certificate.IsAuthenticated() ? NewSession(certificate, tunnel) : DoAcceptTunnel(certificate, tunnel);
            }
        }

        private IEndpoint DoAcceptTunnel(ICertificate certificate, INetTunnel newTunnel)
        {
            var userId = certificate.GetUserId();
            if (userId == null)
            {
                Logger.LogWarning("新session userId is null");
                throw new AuthFailedException(NetResultCode.SESSION_LOSS_ERROR);
            }
            var existSession = FindEndpoint(userId);
            if (existSession == null)
            {
                // 旧 session 失效
                Logger.LogWarning("旧session {User} 已经丢失", newTunnel.GetUserId());
                throw new AuthFailedException(NetResultCode.SESSION_LOSS_ERROR);
            }
            if (existSession.IsClosed())
            {
                // 旧 session 已经关闭(失效)
                Logger.LogWarning("旧session {Session} 已经关闭", existSession);
                throw new AuthFailedException(NetResultCode.SESSION_LOSS_ERROR);
            }

            var current = (INetSession<TUserId>) existSession;
            // existSession.offline(); // 将旧 session 的 Tunnel T 下线
            current.Online(certificate, newTunnel);
            return existSession;
        }

        private IEndpoint NewSession(ICertificate certificate, INetTunnel newTunnel)
        {

            var userId = certificate.GetUserId();
            if (userId == null)
            {
                Logger.LogWarning("新session userId is null");
                throw new AuthFailedException(NetResultCode.SESSION_LOSS_ERROR);
            }
            var oldSession = FindEndpoint(userId);
            if (oldSession != null)
            {
                // 如果旧 session 存在
                var oldCert = oldSession.Certificate;
                // 判断新授权是否比原有授权时间早, 如果是则无法登录
                if (!certificate.IsSameCertificate(oldCert) && certificate.IsOlderThan(oldCert))
                {
                    Logger.LogWarning("认证已过 {Cer}", certificate);
                    throw new AuthFailedException(NetResultCode.INVALID_CERTIFICATE_ERROR);
                }
            }
            var endpoint = newTunnel.GetEndpoint();
            var session = factory.Create(setting.SessionSetting, endpoint.Context, newTunnel.As<TUserId>().CertificateFactory);
            if (oldSession != null)
            {
                if (!oldSession.IsClosed())
                {
                    oldSession.Close();
                }
            }
            session.Online(certificate, newTunnel);
            ReplaceEndpoint(session.GetUserId(), session);
            return session;
        }

        protected override void OnEndpointClose(ISession<TUserId> endpoint)
        {
            offlineSessionQueue.TryRemove(endpoint, out _);
        }

        protected override void OnEndpointOnline(ISession<TUserId> endpoint)
        {
            offlineSessionQueue.TryRemove(endpoint, out _);
        }

        protected override void OnEndpointOffline(ISession<TUserId> endpoint)
        {
            if (endpoint.IsAuthenticated() && endpoint.IsOffline())
            {
                offlineSessionQueue.TryAdd(endpoint, true);
            }
        }

        private void ClearInvalidedSession()
        {
            var logger = SessionKeeper.LOGGER;
            var now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            foreach (var session in offlineSessionQueue.Keys)
            {
                try
                {
                    ISession? closeSession = null;
                    if (session.IsClosed())
                    {
                        logger.LogInformation("移除已关闭的 OfflineSession userId : {User}", session.GetUserId());
                        closeSession = session;
                    } else if (session.IsOnline() && session.OfflineTime + setting.OfflineCloseDelay < now)
                    {
                        logger.LogInformation("移除下线超时的 OfflineSession userId : {User}", session.GetUserId());
                        session.Close();
                        if (session.IsClosed())
                        {
                            closeSession = session;
                        }
                    }
                    if (closeSession == null)
                        continue;
                    RemoveEndpoint(closeSession.GetUserId(), closeSession);
                    offlineSessionQueue.TryRemove(closeSession, out _);
                } catch (Exception e)
                {
                    logger.LogError(e, "clear {User} invalided session exception", session.GetUserId());
                }
            }
            // TODO LRU 算法移除多余的 session
            // var maxSize = setting.OfflineMaxSize;
            // if (maxSize <= 0)
            //     return;
            //
            // var size = offlineSessionQueue.Count - maxSize;
            // for (var i = 0; i < size; i++)
            // {
            //     Session<UID> session = this.offlineSessionQueue.poll();
            //     if (session == null)
            //     {
            //         continue;
            //     }
            //     try
            //     {
            //         LOG.info("关闭第{}个 超过{}数量的OfflineSession {}", i, size, session.getUserId());
            //         session.close();
            //     } catch (Throwable e)
            //     {
            //         LOG.error("clear {} overmuch offline session exception", session.getUserId(), e);
            //     }
            // }
        }
    }

}
