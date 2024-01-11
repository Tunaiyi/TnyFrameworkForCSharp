// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using dotnet_etcd;
using Etcdserverpb;
using Grpc.Core;
using TnyFramework.Common.Extensions;
using V3Lockpb;

namespace TnyFramework.Namespace.Etcd;

public class EtcdAccessor
{
    private readonly EtcdConfig config;

    private EtcdClient Client { get; }

    private EtcdAuthCredential? credential;

    private Encoding KeyEncoding { get; }

    public EtcdAccessor(EtcdConfig config)
    {
        this.config = config;
        Client = new EtcdClient(config.Endpoints);
        KeyEncoding = config.Encoding.IsNotBlank() ? Encoding.GetEncoding(config.Encoding) : Encoding.UTF8;
    }

    internal async Task Init()
    {
        credential = new EtcdAuthCredential(Client, config);
        if (credential.NeedAuthenticate)
        {
            await credential.ApplyToken();
        }
    }

    private Metadata? AuthHeader(Metadata? headers = null)
    {
        var authCredential = credential;
        if (authCredential == null || !authCredential.NeedAuthenticate)
        {
            return headers;
        }
        if (headers == null)
        {
            headers = new Metadata();
        }
        var token = authCredential.ApplyToken().Result;
        if (token != null)
        {
            headers.Add(token);
        }
        return headers;
    }

    private async Task<Metadata?> AuthHeaderAsync(Metadata? headers = null)
    {
        var authCredential = credential;
        if (authCredential == null || !authCredential.NeedAuthenticate)
        {
            return headers;
        }
        if (headers == null)
        {
            headers = new Metadata();
        }
        var token = await authCredential.ApplyToken();
        if (token != null)
        {
            headers.Add(token);
        }
        return headers;
    }

    public AuthenticateResponse Authenticate(AuthenticateRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.Authenticate(request, headers, deadline, cancellationToken);
    }

    public Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.AuthenticateAsync(request, headers, deadline, cancellationToken);
    }

    public AuthEnableResponse AuthEnable(AuthEnableRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.AuthEnable(request, AuthHeader(headers), deadline, cancellationToken);
    }

    public async Task<AuthEnableResponse> AuthEnableAsync(AuthEnableRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.AuthEnableAsync(request, headers, deadline, cancellationToken);
    }

    public AuthDisableResponse AuthDisable(AuthDisableRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.AuthDisable(request, AuthHeader(headers), deadline, cancellationToken);
    }

    public async Task<AuthDisableResponse> AuthDisableAsync(AuthDisableRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.AuthDisableAsync(request, headers, deadline, cancellationToken);
    }

    public AuthUserAddResponse UserAdd(AuthUserAddRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.UserAdd(request, AuthHeader(headers), deadline, cancellationToken);
    }

    public async Task<AuthUserAddResponse> UserAddAsync(AuthUserAddRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.UserAddAsync(request, headers, deadline, cancellationToken);
    }

    public AuthUserGetResponse UserGet(AuthUserGetRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.UserGet(request, AuthHeader(headers), deadline, cancellationToken);
    }

    public async Task<AuthUserGetResponse> UserGetAsync(AuthUserGetRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.UserGetAsync(request, headers, deadline, cancellationToken);
    }

    public AuthUserListResponse UserList(AuthUserListRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.UserList(request, AuthHeader(headers), deadline, cancellationToken);
    }

    public async Task<AuthUserListResponse> UserListAsync(AuthUserListRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.UserListAsync(request, headers, deadline, cancellationToken);
    }

    public AuthUserDeleteResponse UserDelete(AuthUserDeleteRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.UserDelete(request, AuthHeader(headers), deadline, cancellationToken);
    }

    public async Task<AuthUserDeleteResponse> UserDeleteAsync(AuthUserDeleteRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.UserDeleteAsync(request, headers, deadline, cancellationToken);
    }

    public AuthUserChangePasswordResponse UserChangePassword(AuthUserChangePasswordRequest request, Metadata? headers = null,
        DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.UserChangePassword(request, AuthHeader(headers), deadline, cancellationToken);
    }

    public async Task<AuthUserChangePasswordResponse> UserChangePasswordAsync(AuthUserChangePasswordRequest request, Metadata? headers = null,
        DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.UserChangePasswordAsync(request, headers, deadline, cancellationToken);
    }

    public AuthUserGrantRoleResponse UserGrantRole(AuthUserGrantRoleRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.UserGrantRole(request, AuthHeader(headers), deadline, cancellationToken);
    }

    public async Task<AuthUserGrantRoleResponse> UserGrantRoleAsync(AuthUserGrantRoleRequest request, Metadata? headers = null,
        DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.UserGrantRoleAsync(request, headers, deadline, cancellationToken);
    }

    public AuthUserRevokeRoleResponse UserRevokeRole(AuthUserRevokeRoleRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.UserRevokeRole(request, AuthHeader(headers), deadline, cancellationToken);
    }

    public async Task<AuthUserRevokeRoleResponse> UserRevokeRoleAsync(AuthUserRevokeRoleRequest request, Metadata? headers = null,
        DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.UserRevokeRoleAsync(request, headers, deadline, cancellationToken);
    }

    public AuthRoleAddResponse RoleAdd(AuthRoleAddRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.RoleAdd(request, AuthHeader(headers), deadline, cancellationToken);
    }

    public async Task<AuthRoleAddResponse> RoleAddAsync(AuthRoleAddRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.RoleAddAsync(request, headers, deadline, cancellationToken);
    }

    public AuthRoleGetResponse RoleGet(AuthRoleGetRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.RoleGet(request, AuthHeader(headers), deadline, cancellationToken);
    }

    public async Task<AuthRoleGetResponse> RoleGetASync(AuthRoleGetRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.RoleGetASync(request, headers, deadline, cancellationToken);
    }

    public AuthRoleListResponse RoleList(AuthRoleListRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.RoleList(request, AuthHeader(headers), deadline, cancellationToken);
    }

    public async Task<AuthRoleListResponse> RoleListAsync(AuthRoleListRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.RoleListAsync(request, headers, deadline, cancellationToken);
    }

    public AuthRoleDeleteResponse RoleDelete(AuthRoleDeleteRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.RoleDelete(request, AuthHeader(headers), deadline, cancellationToken);
    }

    public async Task<AuthRoleDeleteResponse> RoleDeleteAsync(AuthRoleDeleteRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.RoleDeleteAsync(request, headers, deadline, cancellationToken);
    }

    public AuthRoleGrantPermissionResponse RoleGrantPermission(AuthRoleGrantPermissionRequest request, Metadata? headers = null,
        DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.RoleGrantPermission(request, AuthHeader(headers), deadline, cancellationToken);
    }

    public async Task<AuthRoleGrantPermissionResponse> RoleGrantPermissionAsync(AuthRoleGrantPermissionRequest request, Metadata? headers = null,
        DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.RoleGrantPermissionAsync(request, headers, deadline, cancellationToken);
    }

    public AuthRoleRevokePermissionResponse RoleRevokePermission(AuthRoleRevokePermissionRequest request, Metadata? headers = null,
        DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.RoleRevokePermission(request, AuthHeader(headers), deadline, cancellationToken);
    }

    public async Task<AuthRoleRevokePermissionResponse> RoleRevokePermissionAsync(AuthRoleRevokePermissionRequest request,
        Metadata? headers = null,
        DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.RoleRevokePermissionAsync(request, headers, deadline, cancellationToken);
    }

    public MemberAddResponse MemberAdd(MemberAddRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.MemberAdd(request, AuthHeader(headers), deadline, cancellationToken);
    }

    public async Task<MemberAddResponse> MemberAddAsync(MemberAddRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.MemberAddAsync(request, headers, deadline, cancellationToken);
    }

    public MemberRemoveResponse MemberRemove(MemberRemoveRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.MemberRemove(request, AuthHeader(headers), deadline, cancellationToken);
    }

    public async Task<MemberRemoveResponse> MemberRemoveAsync(MemberRemoveRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.MemberRemoveAsync(request, headers, deadline, cancellationToken);
    }

    public MemberUpdateResponse MemberUpdate(MemberUpdateRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.MemberUpdate(request, AuthHeader(headers), deadline, cancellationToken);
    }

    public async Task<MemberUpdateResponse> MemberUpdateAsync(MemberUpdateRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.MemberUpdateAsync(request, headers, deadline, cancellationToken);
    }

    public MemberListResponse MemberList(MemberListRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.MemberList(request, AuthHeader(headers), deadline, cancellationToken);
    }

    public async Task<MemberListResponse> MemberListAsync(MemberListRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.MemberListAsync(request, headers, deadline, cancellationToken);
    }

    public void Dispose()
    {
        Client.Dispose();
    }

    public RangeResponse Get(RangeRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.Get(request, AuthHeader(headers), deadline, cancellationToken);
    }

    public RangeResponse Get(string key, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.Get(key, AuthHeader(headers), deadline, cancellationToken);
    }

    public async Task<RangeResponse> GetAsync(RangeRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.GetAsync(request, headers, deadline, cancellationToken);
    }

    public async Task<RangeResponse> GetAsync(string key, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.GetAsync(key, headers, deadline, cancellationToken);
    }

    public string GetVal(string key, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.GetVal(key, AuthHeader(headers), deadline, cancellationToken);
    }

    public async Task<string> GetValAsync(string key, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.GetValAsync(key, headers, deadline, cancellationToken);
    }

    public RangeResponse GetRange(string prefixKey, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.GetRange(prefixKey, AuthHeader(headers), deadline, cancellationToken);
    }

    public async Task<RangeResponse> GetRangeAsync(string prefixKey, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.GetRangeAsync(prefixKey, headers, deadline, cancellationToken);
    }

    public IDictionary<string, string> GetRangeVal(string prefixKey, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.GetRangeVal(prefixKey, AuthHeader(headers), deadline, cancellationToken);
    }

    public async Task<IDictionary<string, string>> GetRangeValAsync(string prefixKey, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.GetRangeValAsync(prefixKey, headers, deadline, cancellationToken);
    }

    public PutResponse Put(PutRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.Put(request, AuthHeader(headers), deadline, cancellationToken);
    }

    public PutResponse Put(string key, string val, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.Put(key, val, AuthHeader(headers), deadline, cancellationToken);
    }

    public async Task<PutResponse> PutAsync(PutRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.PutAsync(request, headers, deadline, cancellationToken);
    }

    public async Task<PutResponse> PutAsync(string key, string val, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.PutAsync(key, val, headers, deadline, cancellationToken);
    }

    public DeleteRangeResponse Delete(DeleteRangeRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.Delete(request, AuthHeader(headers), deadline, cancellationToken);
    }

    public DeleteRangeResponse Delete(string key, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.Delete(key, AuthHeader(headers), deadline, cancellationToken);
    }

    public async Task<DeleteRangeResponse> DeleteAsync(DeleteRangeRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.DeleteAsync(request, headers, deadline, cancellationToken);
    }

    public async Task<DeleteRangeResponse> DeleteAsync(string key, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.DeleteAsync(key, headers, deadline, cancellationToken);
    }

    public DeleteRangeResponse DeleteRange(string prefixKey, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.DeleteRange(prefixKey, AuthHeader(headers), deadline, cancellationToken);
    }

    public async Task<DeleteRangeResponse> DeleteRangeAsync(string prefixKey, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.DeleteRangeAsync(prefixKey, headers, deadline, cancellationToken);
    }

    public TxnResponse Transaction(TxnRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.Transaction(request, AuthHeader(headers), deadline, cancellationToken);
    }

    public async Task<TxnResponse> TransactionAsync(TxnRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.TransactionAsync(request, headers, deadline, cancellationToken);
    }

    public CompactionResponse Compact(CompactionRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.Compact(request, AuthHeader(headers), deadline, cancellationToken);
    }

    public async Task<CompactionResponse> CompactAsync(CompactionRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.CompactAsync(request, headers, deadline, cancellationToken);
    }

    public LeaseGrantResponse LeaseGrant(LeaseGrantRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.LeaseGrant(request, AuthHeader(headers), deadline, cancellationToken);
    }

    public async Task<LeaseGrantResponse> LeaseGrantAsync(LeaseGrantRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.LeaseGrantAsync(request, headers, deadline, cancellationToken);
    }

    public LeaseRevokeResponse LeaseRevoke(LeaseRevokeRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.LeaseRevoke(request, AuthHeader(headers), deadline, cancellationToken);
    }

    public async Task<LeaseRevokeResponse> LeaseRevokeAsync(LeaseRevokeRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.LeaseRevokeAsync(request, headers, deadline, cancellationToken);
    }

    public Task LeaseKeepAlive(long leaseId, CancellationToken cancellationToken)
    {
        return Client.LeaseKeepAlive(leaseId, cancellationToken);
    }

    public async Task LeaseKeepAlive(LeaseKeepAliveRequest request, Action<LeaseKeepAliveResponse> method, CancellationToken cancellationToken,
        Metadata? headers = null)
    {
        headers = await AuthHeaderAsync(headers);
        await Client.LeaseKeepAlive(request, method, cancellationToken, headers);
    }

    public async Task LeaseKeepAlive(LeaseKeepAliveRequest request, Action<LeaseKeepAliveResponse>[] methods, CancellationToken cancellationToken,
        Metadata? headers = null)
    {
        headers = await AuthHeaderAsync(headers);
        await Client.LeaseKeepAlive(request, methods, cancellationToken, headers);
    }

    public async Task LeaseKeepAlive(LeaseKeepAliveRequest[] requests, Action<LeaseKeepAliveResponse> method, CancellationToken cancellationToken,
        Metadata? headers = null)
    {
        headers = await AuthHeaderAsync(headers);
        await Client.LeaseKeepAlive(requests, method, cancellationToken, headers);
    }

    public async Task LeaseKeepAlive(LeaseKeepAliveRequest[] requests, Action<LeaseKeepAliveResponse>[] methods,
        CancellationToken cancellationToken,
        Metadata? headers = null,
        DateTime? deadline = null)
    {
        headers = await AuthHeaderAsync(headers);
        await Client.LeaseKeepAlive(requests, methods, cancellationToken, headers, deadline);
    }

    public LeaseTimeToLiveResponse LeaseTimeToLive(LeaseTimeToLiveRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.LeaseTimeToLive(request, AuthHeader(headers), deadline, cancellationToken);
    }

    public async Task<LeaseTimeToLiveResponse> LeaseTimeToLiveAsync(LeaseTimeToLiveRequest request, Metadata? headers = null,
        DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.LeaseTimeToLiveAsync(request, headers, deadline, cancellationToken);
    }

    public LockResponse Lock(string name, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.Lock(name, AuthHeader(headers), deadline, cancellationToken);
    }

    public LockResponse Lock(LockRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.Lock(request, AuthHeader(headers), deadline, cancellationToken);
    }

    public async Task<LockResponse> LockAsync(string name, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.LockAsync(name, headers, deadline, cancellationToken);
    }

    public async Task<LockResponse> LockAsync(LockRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.LockAsync(request, headers, deadline, cancellationToken);
    }

    public UnlockResponse Unlock(string key, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.Unlock(key, AuthHeader(headers), deadline, cancellationToken);
    }

    public UnlockResponse Unlock(UnlockRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.Unlock(request, AuthHeader(headers), deadline, cancellationToken);
    }

    public async Task<UnlockResponse> UnlockAsync(string key, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.UnlockAsync(key, headers, deadline, cancellationToken);
    }

    public async Task<UnlockResponse> UnlockAsync(UnlockRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.UnlockAsync(request, headers, deadline, cancellationToken);
    }

    public AlarmResponse Alarm(AlarmRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.Alarm(request, AuthHeader(headers), deadline, cancellationToken);
    }

    public async Task<AlarmResponse> AlarmAsync(AlarmRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.AlarmAsync(request, headers, deadline, cancellationToken);
    }

    public StatusResponse Status(StatusRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.Status(request, AuthHeader(headers), deadline, cancellationToken);
    }

    public async Task<StatusResponse> StatusASync(StatusRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.StatusASync(request, headers, deadline, cancellationToken);
    }

    public DefragmentResponse Defragment(DefragmentRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.Defragment(request, AuthHeader(headers), deadline, cancellationToken);
    }

    public async Task<DefragmentResponse> DefragmentAsync(DefragmentRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.DefragmentAsync(request, headers, deadline, cancellationToken);
    }

    public HashResponse Hash(HashRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.Hash(request, AuthHeader(headers), deadline, cancellationToken);
    }

    public async Task<HashResponse> HashAsync(HashRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.HashAsync(request, headers, deadline, cancellationToken);
    }

    public HashKVResponse HashKv(HashKVRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.HashKV(request, AuthHeader(headers), deadline, cancellationToken);
    }

    public async Task<HashKVResponse> HashKvAsync(HashKVRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.HashKVAsync(request, headers, deadline, cancellationToken);
    }

    public async Task Snapshot(SnapshotRequest request, Action<SnapshotResponse> method, CancellationToken cancellationToken,
        Metadata? headers = null,
        DateTime? deadline = null)
    {
        headers = await AuthHeaderAsync(headers);
        await Client.Snapshot(request, method, cancellationToken, headers, deadline);
    }

    public async Task Snapshot(SnapshotRequest request, Action<SnapshotResponse>[] methods, CancellationToken cancellationToken,
        Metadata? headers = null, DateTime? deadline = null)
    {
        headers = await AuthHeaderAsync(headers);
        await Client.Snapshot(request, methods, cancellationToken, headers, deadline);
    }

    public MoveLeaderResponse MoveLeader(MoveLeaderRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Client.MoveLeader(request, AuthHeader(headers), deadline, cancellationToken);
    }

    public async Task<MoveLeaderResponse> MoveLeaderAsync(MoveLeaderRequest request, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        return await Client.MoveLeaderAsync(request, headers, deadline, cancellationToken);
    }

    public async Task Watch(WatchRequest request, Action<WatchResponse> method, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        await Client.WatchAsync(request, method, headers, deadline, cancellationToken);
    }

    public async Task Watch(WatchRequest request, Action<WatchResponse>[] methods, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        await Client.WatchAsync(request, methods, headers, deadline, cancellationToken);
    }

    public async Task Watch(WatchRequest request, Action<WatchEvent[]> method, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        await Client.WatchAsync(request, method, headers, deadline, cancellationToken);
    }

    public async Task Watch(WatchRequest request, Action<WatchEvent[]>[] methods, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        await Client.WatchAsync(request, methods, headers, deadline, cancellationToken);
    }

    public async Task Watch(WatchRequest[] requests, Action<WatchResponse> method, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        await Client.WatchAsync(requests, method, headers, deadline, cancellationToken);
    }

    public async Task Watch(WatchRequest[] requests, Action<WatchResponse>[] methods, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        await Client.WatchAsync(requests, methods, headers, deadline, cancellationToken);
    }

    public async Task Watch(WatchRequest[] requests, Action<WatchEvent[]> method, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        await Client.WatchAsync(requests, method, headers, deadline, cancellationToken);
    }

    public async Task Watch(WatchRequest[] requests, Action<WatchEvent[]>[] methods, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        await Client.WatchAsync(requests, methods, headers, deadline, cancellationToken);
    }

    public void Watch(string key, Action<WatchResponse> method, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        Client.Watch(key, method, AuthHeader(headers), deadline, cancellationToken);
    }

    public void Watch(string key, Action<WatchResponse>[] methods, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        Client.Watch(key, methods, AuthHeader(headers), deadline, cancellationToken);
    }

    public void Watch(string key, Action<WatchEvent[]> method, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        Client.Watch(key, method, AuthHeader(headers), deadline, cancellationToken);
    }

    public void Watch(string key, Action<WatchEvent[]>[] methods, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        Client.Watch(key, methods, AuthHeader(headers), deadline, cancellationToken);
    }

    public void Watch(string[] keys, Action<WatchResponse> method, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        Client.Watch(keys, method, AuthHeader(headers), deadline, cancellationToken);
    }

    public void Watch(string[] keys, Action<WatchResponse>[] methods, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        Client.Watch(keys, methods, AuthHeader(headers), deadline, cancellationToken);
    }

    public void Watch(string[] keys, Action<WatchEvent[]> method, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        Client.Watch(keys, method, AuthHeader(headers), deadline, cancellationToken);
    }

    public void Watch(string[] keys, Action<WatchEvent[]>[] methods, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        Client.Watch(keys, methods, AuthHeader(headers), deadline, cancellationToken);
    }

    public async Task WatchRange(WatchRequest request, Action<WatchResponse> method, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        await Client.WatchAsync(request, method, headers, deadline, cancellationToken);
    }

    public async Task WatchRange(WatchRequest request, Action<WatchResponse>[] methods, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        await Client.WatchAsync(request, methods, headers, deadline, cancellationToken);
    }

    public async Task WatchRange(WatchRequest request, Action<WatchEvent[]> method, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        await Client.WatchAsync(request, method, headers, deadline, cancellationToken);
    }

    public async Task WatchRange(WatchRequest request, Action<WatchEvent[]>[] methods, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        await Client.WatchAsync(request, methods, headers, deadline, cancellationToken);
    }

    public async Task WatchRange(WatchRequest[] requests, Action<WatchResponse> method, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        await Client.WatchAsync(requests, method, headers, deadline, cancellationToken);
    }

    public async Task WatchRange(WatchRequest[] requests, Action<WatchResponse>[] methods, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        await Client.WatchAsync(requests, methods, headers, deadline, cancellationToken);
    }

    public async Task WatchRange(WatchRequest[] requests, Action<WatchEvent[]> method, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        await Client.WatchAsync(requests, method, headers, deadline, cancellationToken);
    }

    public async Task WatchRange(WatchRequest[] requests, Action<WatchEvent[]>[] methods, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        headers = await AuthHeaderAsync(headers);
        await Client.WatchAsync(requests, methods, headers, deadline, cancellationToken);
    }

    public void WatchRange(string path, Action<WatchResponse> method, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        Client.WatchAsync(path, method, AuthHeader(headers), deadline, cancellationToken);
    }

    public void WatchRange(string path, Action<WatchResponse>[] methods, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        Client.WatchAsync(path, methods, AuthHeader(headers), deadline, cancellationToken);
    }

    public void WatchRange(string path, Action<WatchEvent[]> method, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        Client.WatchAsync(path, method, AuthHeader(headers), deadline, cancellationToken);
    }

    public void WatchRange(string path, Action<WatchEvent[]>[] methods, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        Client.WatchAsync(path, methods, AuthHeader(headers), deadline, cancellationToken);
    }

    public void WatchRange(string[] paths, Action<WatchResponse> method, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        Client.WatchAsync(paths, method, AuthHeader(headers), deadline, cancellationToken);
    }

    public void WatchRange(string[] paths, Action<WatchResponse>[] methods, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        Client.WatchAsync(paths, methods, AuthHeader(headers), deadline, cancellationToken);
    }

    public void WatchRange(string[] paths, Action<WatchEvent[]> method, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        Client.WatchAsync(paths, method, AuthHeader(headers), deadline, cancellationToken);
    }

    public void WatchRange(string[] paths, Action<WatchEvent[]>[] methods, Metadata? headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        Client.WatchAsync(paths, methods, AuthHeader(headers), deadline, cancellationToken);
    }
}
