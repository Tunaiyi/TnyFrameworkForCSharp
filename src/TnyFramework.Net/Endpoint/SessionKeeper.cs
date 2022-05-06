using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;
using TnyFramework.Coroutines.Async;
using TnyFramework.Net.Command;
using TnyFramework.Net.Common;
using TnyFramework.Net.Exceptions;
using TnyFramework.Net.Rpc;
using TnyFramework.Net.Transport;
namespace TnyFramework.Net.Endpoint
{
    public class SessionKeeper
    {
        internal static readonly ILogger LOGGER = LogFactory.Logger<SessionKeeper>();
    }

    public class SessionKeeper<TUserId> : EndpointKeeper<TUserId, ISession<TUserId>>, ISessionKeeper<TUserId>
    {
        public static readonly ILogger LOGGER = LogFactory.Logger<SessionKeeper>();

        /* 离线session */
        private readonly ConcurrentDictionary<ISession, bool> offlineSessionQueue = new ConcurrentDictionary<ISession, bool>();

        private readonly ISessionFactory factory;

        private readonly ISessionKeeperSetting setting;

        private readonly object[] locks;

        private ICoroutine coroutine;

        private int start = 0;


        public SessionKeeper(string userType, ISessionFactory factory, ISessionKeeperSetting setting) : base(userType)
        {
            this.factory = factory;
            this.setting = setting;
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
            coroutine.Exec(async () => {
                try
                {
                    ClearInvalidedSession();
                    await Task.Delay(TimeSpan.FromMilliseconds(setting.ClearSessionInterval));
                } catch (Exception e)
                {
                    LOGGER.LogError(e, "");
                }
            });
        }


        public override IEndpoint Online(ICertificate certificate, INetTunnel tunnel)
        {
            if (!certificate.IsAuthenticated())
            {
                throw new ValidatorFailException(NetResultCode.VALIDATOR_FAIL_ERROR, $"cert {certificate} is unauthentic");
            }
            if (!Equals(UserType, certificate.UserType))
            {
                throw new ValidatorFailException(NetResultCode.VALIDATOR_FAIL_ERROR,
                    $"cert {certificate} userType is {certificate.UserType}, not {UserType}");
            }
            var uid = certificate.GetUserId();
            var index = Math.Abs(uid.GetHashCode()) % locks.Length;
            lock (locks[index])
            {
                return certificate.IsAuthenticated() ? NewSession(certificate, tunnel) : DoAcceptTunnel(certificate, tunnel);
            }
        }


        private IEndpoint DoAcceptTunnel(ICertificate certificate, INetTunnel newTunnel)
        {
            var existSession = FindEndpoint(certificate.GetUserId());
            if (existSession == null)
            {
                // 旧 session 失效
                SessionKeeper.LOGGER.LogWarning("旧session {User} 已经丢失", newTunnel.GetUserId());
                throw new ValidatorFailException(NetResultCode.SESSION_LOSS_ERROR);
            }
            if (existSession.IsClosed())
            {
                // 旧 session 已经关闭(失效)
                SessionKeeper.LOGGER.LogWarning("旧session {Session} 已经关闭", existSession);
                throw new ValidatorFailException(NetResultCode.SESSION_LOSS_ERROR);
            }

            var current = (INetSession<TUserId>)existSession;
            // existSession.offline(); // 将旧 session 的 Tunnel T 下线
            current.Online(certificate, newTunnel);
            return existSession;
        }


        private IEndpoint NewSession(ICertificate certificate, INetTunnel newTunnel)
        {

            var oldSession = FindEndpoint(certificate.GetUserId());
            if (oldSession != null)
            {
                // 如果旧 session 存在
                var oldCert = oldSession.Certificate;
                // 判断新授权是否比原有授权时间早, 如果是则无法登录
                if (!certificate.IsSameCertificate(oldCert) && certificate.IsOlderThan(oldCert))
                {
                    SessionKeeper.LOGGER.LogWarning("认证已过 {Cer}", certificate);
                    throw new ValidatorFailException(NetResultCode.INVALID_CERTIFICATE_ERROR);
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
                    ISession closeSession = null;
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
