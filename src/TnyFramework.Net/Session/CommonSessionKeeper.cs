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
using TnyFramework.Net.Application;
using TnyFramework.Net.Common;
using TnyFramework.Net.Exceptions;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Session;

public class CommonSessionKeeper : SessionKeeper<ISession>
{
    private static readonly ILogger LOGGER = LogFactory.Logger<CommonSessionKeeper>();

    /* 离线session */
    private readonly ConcurrentDictionary<ISession, bool> offlineSessionQueue = new ConcurrentDictionary<ISession, bool>();

    private readonly ISessionKeeperSetting setting;

    private readonly object[] locks;

    private ICoroutine? coroutine;

    private int start;

    private ILogger Logger => LOGGER;

    public CommonSessionKeeper(IContactType contactType, ISessionKeeperSetting setting) : base(contactType)
    {
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

    public override ISession Online(ICertificate certificate, INetTunnel tunnel)
    {
        if (!certificate.IsAuthenticated())
        {
            throw new AuthFailedException(NetResultCode.AUTH_FAIL_ERROR, null, $"cert {certificate} is unauthentic");
        }
        if (!Equals(ContactType, certificate.ContactType))
        {
            throw new AuthFailedException(NetResultCode.AUTH_FAIL_ERROR, null,
                $"cert {certificate} userType is {certificate.ContactGroup}, not {ContactType}");
        }
        var identify = certificate.Identify;
        var index = Math.Abs(identify.GetHashCode()) % locks.Length;
        lock (locks[index])
        {
            return certificate.IsAuthenticated() ? DoAuth(certificate, tunnel) : DoReAuth(certificate, tunnel);
        }
    }

    private ISession DoReAuth(ICertificate certificate, INetTunnel newTunnel)
    {
        var identify = certificate.Identify;
        var existSession = FindSession(identify);
        if (existSession == null)
        {
            // 旧 session 失效
            Logger.LogWarning("旧session {User} 已经丢失", newTunnel.Identify);
            throw new AuthFailedException(NetResultCode.SESSION_LOSS_ERROR);
        }
        if (existSession.IsClosed())
        {
            // 旧 session 已经关闭(失效)
            Logger.LogWarning("旧session {Session} 已经关闭", existSession);
            throw new AuthFailedException(NetResultCode.SESSION_LOSS_ERROR);
        }

        var current = (INetSession) existSession;
        // existSession.offline(); // 将旧 session 的 Tunnel T 下线
        current.Online(certificate, newTunnel);
        return existSession;
    }

    private ISession DoAuth(ICertificate certificate, INetTunnel newTunnel)
    {

        var identify = certificate.Identify;
        var oldSession = FindSession(identify);
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
        var session = newTunnel.NetSession;
        if (oldSession != null)
        {
            if (!oldSession.IsClosed())
            {
                oldSession.Close();
            }
        }
        session.Online(certificate, newTunnel);
        ReplaceSession(session.Identify, session);
        return session;
    }

    protected override void OnSessionClose(ISession session)
    {
        offlineSessionQueue.TryRemove(session, out _);
    }

    protected override void OnSessionOnline(ISession session)
    {
        offlineSessionQueue.TryRemove(session, out _);
    }

    protected override void OnSessionOffline(ISession session)
    {
        if (session.IsAuthenticated() && session.IsOffline())
        {
            offlineSessionQueue.TryAdd(session, true);
        }
    }

    private void ClearInvalidedSession()
    {
        var logger = LOGGER;
        var now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        foreach (var session in offlineSessionQueue.Keys)
        {
            try
            {
                ISession? closeSession = null;
                if (session.IsClosed())
                {
                    logger.LogInformation("移除已关闭的 OfflineSession identify : {User}", session.Certificate);
                    closeSession = session;
                } else if (session.IsOnline() && session.OfflineTime + setting.OfflineCloseDelay < now)
                {
                    logger.LogInformation("移除下线超时的 OfflineSession identify : {User}", session.Certificate);
                    session.Close();
                    if (session.IsClosed())
                    {
                        closeSession = session;
                    }
                }
                if (closeSession == null)
                    continue;
                RemoveSession(closeSession.Identify, closeSession);
                offlineSessionQueue.TryRemove(closeSession, out _);
            } catch (Exception e)
            {
                logger.LogError(e, "clear {User} invalided session exception", session.Certificate);
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
