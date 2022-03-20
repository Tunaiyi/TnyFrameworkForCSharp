using System;
using TnyFramework.DI.Units;
using TnyFramework.Net.Endpoint;
namespace TnyFramework.Net.DotNetty.Configuration.Endpoint
{
    public interface IEndpointSpec
    {
        /// <summary>
        /// 配置 SessionFactory
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IEndpointSpec SessionFactoryConfigure(Action<UnitCollectionSpec<ISessionFactory, IEndpointUnitContext>> action);


        /// <summary>
        /// 配置SessionKeeperFactory
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IEndpointSpec SessionKeeperFactoryConfigure(Action<IUnitCollectionSpec<ISessionKeeperFactory, IEndpointUnitContext>> action);


        /// <summary>
        /// 配置SessionKeeperFactory
        /// </summary>
        /// <returns></returns>
        IEndpointSpec SessionKeeperFactory<TUserId>();


        /// <summary>
        /// 配置SessionKeeperFactory
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IEndpointSpec SessionKeeperFactory<TUserId>(string name);


        /// <summary>
        /// 配置 默认Session
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IEndpointSpec DefaultSessionConfigure(Action<ISessionKeeperSettingSpec> action);


        /// <summary>
        /// 配置 自定义Session
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IEndpointSpec CustomSessionConfigure(Action<ISessionKeeperSettingSpec> action);


        /// <summary>
        /// 配置 自定义Session
        /// </summary>
        /// <param name="name"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        IEndpointSpec CustomSessionConfigure(string name, Action<ISessionKeeperSettingSpec> action);
    }
}
