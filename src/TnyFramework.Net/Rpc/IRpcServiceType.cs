using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TnyFramework.Common.Enum;
using TnyFramework.Net.Base;

namespace TnyFramework.Net.Rpc
{

    public interface IRpcServiceType : IMessagerType
    {
        /// <summary>
        /// 用户组
        /// </summary>
        string Service { get; }

        /// <summary>
        /// app类型
        /// </summary>
        AppType AppType { get; }
    }

    public class RpcServiceType : BaseEnum<RpcServiceType>, IRpcServiceType
    {
        private static readonly ConcurrentDictionary<string, RpcServiceType> SERVICE_MAP = new ConcurrentDictionary<string, RpcServiceType>();

        private static readonly ConcurrentDictionary<AppType, RpcServiceType> APP_TYPE_MAP = new ConcurrentDictionary<AppType, RpcServiceType>();

        /// <summary>
        /// 服务名
        /// </summary>
        public string Service { get; protected set; }

        public AppType AppType { get; protected set; }

        public string Group => Service;

        protected override void OnCheck()
        {
            if (!SERVICE_MAP.TryAdd(Service, this) && !ReferenceEquals(SERVICE_MAP[Service], this))
            {
                throw new ArgumentException($"{SERVICE_MAP[Service]} 与 {this} 存在相同的 Service {Service}");
            }
            if (!APP_TYPE_MAP.TryAdd(AppType, this) && !ReferenceEquals(APP_TYPE_MAP[AppType], this))
            {
                throw new ArgumentException($"{APP_TYPE_MAP[AppType]} 与 {this} 存在相同的 AppType {Service}");
            }
        }

        public new static RpcServiceType ForId(int id)
        {
            return BaseEnum<RpcServiceType>.ForId(id);
        }

        public new static RpcServiceType ForName(string name)
        {
            return BaseEnum<RpcServiceType>.ForName(name);
        }

        public static RpcServiceType ForService(string service)
        {
            if (!SERVICE_MAP.TryGetValue(service, out var obj))
                throw new ArgumentException($"枚举Service不存在 -> {service}");
            return obj;
        }

        public static RpcServiceType ForAppType(AppType appType)
        {
            if (!APP_TYPE_MAP.TryGetValue(appType, out var obj))
                throw new ArgumentException($"枚举AppType不存在 -> {appType}");
            return obj;
        }
    }

    public abstract class RpcServiceType<T> : RpcServiceType where T : RpcServiceType<T>, new()
    {
        protected static T Of(int id, AppType appType, string service, Action<T> builder = null)
        {
            return E(id, new T {
                Service = service,
                AppType = appType
            }, builder);
        }

        public new static void LoadAll() => LoadAll(typeof(T));

        public new static IReadOnlyCollection<RpcServiceType> GetValues()
        {
            LoadAll();
            return BaseEnum<RpcServiceType>.GetValues();
        }
    }

}
