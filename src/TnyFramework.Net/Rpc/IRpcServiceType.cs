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
using TnyFramework.Net.Application;

namespace TnyFramework.Net.Rpc
{

    public interface IRpcServiceType : IContactType
    {
        /// <summary>
        /// 用户组
        /// </summary>
        string Service { get; }
    }

    public class RpcServiceType : ContactType, IRpcServiceType
    {
        private static readonly ConcurrentDictionary<string, RpcServiceType> SERVICE_MAP = new();

        protected static readonly List<RpcServiceType> SERVICES = new();

        /// <summary>
        /// 服务名
        /// </summary>
        public string Service => Group;

        protected override void OnCheck()
        {
            base.OnCheck();
            if (!SERVICE_MAP.TryAdd(Service, this) && !ReferenceEquals(SERVICE_MAP[Service], this))
            {
                throw new ArgumentException($"{SERVICE_MAP[Service]} 与 {this} 存在相同的 Service {Service}");
            }
            SERVICES.Add(this);
        }

        public new static RpcServiceType ForId(int id)
        {
            var value = GetById(id, false);
            if (value is RpcServiceType type)
            {
                return type;
            }
            throw new ArgumentException($"{typeof(RpcServiceType)} 枚举ID不存在 -> {id}");
        }

        public new static RpcServiceType ForName(string name)
        {
            var value = GetByName(name, false);
            if (value is RpcServiceType type)
            {
                return type;
            }
            throw new ArgumentException($"{typeof(RpcServiceType)} 枚举Name不存在 -> {name}");
        }

        public static RpcServiceType ForService(string service)
        {
            if (!SERVICE_MAP.TryGetValue(service, out var obj))
                throw new ArgumentException($"枚举Service不存在 -> {service}");
            return obj;
        }

        public static implicit operator int(RpcServiceType type) => type.Id;

        public static explicit operator RpcServiceType(int type) => ForId(type);
    }

    public abstract class RpcServiceType<T> : RpcServiceType where T : RpcServiceType<T>, new()
    {
        protected static T Of(int id, string service, Action<T>? builder = null)
        {
            return E(id, new T {
                Group = service,
            }, builder);
        }

        public new static void LoadAll() => LoadAll(typeof(T));

        public new static IReadOnlyCollection<RpcServiceType> GetValues()
        {
            LoadAll();
            return SERVICES;
        }
    }

}
