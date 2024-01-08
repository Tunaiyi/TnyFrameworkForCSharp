// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using TnyFramework.DI.Units;
using TnyFramework.Net.Session;

namespace TnyFramework.Net.Hosting.Session
{

    public interface ISessionSpec
    {
        /// <summary>
        /// 配置 SessionFactory
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        ISessionSpec SessionFactoryConfigure(Action<UnitCollectionSpec<ISessionFactory, ISessionUnitContext>> action);

        /// <summary>
        /// 配置SessionKeeperFactory
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        ISessionSpec SessionKeeperFactoryConfigure(Action<IUnitCollectionSpec<ISessionKeeperFactory, ISessionUnitContext>> action);

        /// <summary>
        /// 配置SessionKeeperFactory
        /// </summary>
        /// <returns></returns>
        ISessionSpec SessionKeeperFactory();

        /// <summary>
        /// 配置SessionKeeperFactory
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        ISessionSpec SessionKeeperFactory(string name);

        /// <summary>
        /// 配置 默认Session
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        ISessionSpec DefaultSessionConfigure(Action<ISessionKeeperSettingSpec> action);

        /// <summary>
        /// 配置 自定义Session
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        ISessionSpec CustomSessionConfigure(Action<ISessionKeeperSettingSpec> action);

        /// <summary>
        /// 配置 自定义Session
        /// </summary>
        /// <param name="name"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        ISessionSpec CustomSessionConfigure(string name, Action<ISessionKeeperSettingSpec> action);
    }

}
