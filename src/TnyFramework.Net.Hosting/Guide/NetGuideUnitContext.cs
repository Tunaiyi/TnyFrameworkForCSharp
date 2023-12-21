// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using Microsoft.Extensions.DependencyInjection;
using TnyFramework.DI.Units;
using TnyFramework.Net.Application;
using TnyFramework.Net.Command.Dispatcher.Monitor;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.Hosting.Guide
{

    public abstract class NetGuideUnitContext<TGuideContext> : INetGuideUnitContext
        where TGuideContext : INetGuideUnitContext
    {
        protected IServiceCollection UnitContainer { get; }

        public INetUnitContext UnitContext { get; }

        public UnitSpec<IMessageFactory, TGuideContext> MessageFactorySpec { get; }

        public UnitSpec<IContactFactory, TGuideContext> ContactFactorySpec { get; }

        public UnitSpec<RpcMonitor, TGuideContext> RpcMonitorSpec { get; }

        public UnitSpec<INetworkContext, TGuideContext> NetworkContextSpec { get; }

        protected TGuideContext Self {
            get {
                if (this is TGuideContext context)
                {
                    return context;
                }
                throw new InvalidCastException($"{GetType()} cast {nameof(TGuideContext)}");
            }
        }

        protected NetGuideUnitContext(INetUnitContext unitContext, IServiceCollection unitContainer)
        {
            UnitContainer = unitContainer;

            UnitContext = unitContext;

            // MessageFactory
            MessageFactorySpec = UnitSpec.Unit<IMessageFactory, TGuideContext>()
                .Default<CommonMessageFactory>();

            // ContactFactory
            ContactFactorySpec = UnitSpec.Unit<IContactFactory, TGuideContext>()
                .Default<InnerContactFactory>();

            // RpcMonitor
            RpcMonitorSpec = UnitSpec.Unit<RpcMonitor, TGuideContext>()
                .Default(DefaultRpcMonitor);

            NetworkContextSpec = UnitSpec.Unit<INetworkContext, TGuideContext>()
                .Default(DefaultNetworkContext);
        }


        private static INetworkContext DefaultNetworkContext(TGuideContext context)
        {
            var unitContext = context.UnitContext;
            return new NetworkContext(
                unitContext.LoadMessageDispatcher(),
                unitContext.LoadCommandBoxFactory(),
                context.LoadMessageFactory(),
                context.LoadContactFactory(),
                context.LoadRpcMonitor());
        }

        private RpcMonitor DefaultRpcMonitor(TGuideContext context)
        {
            return new RpcMonitor();
        }

        public void SetName(string name)
        {
            MessageFactorySpec.WithNamePrefix(name);
            ContactFactorySpec.WithNamePrefix(name);
            OnSetName(name);
        }

        protected abstract void OnSetName(string name);

        public RpcMonitor LoadRpcMonitor()
        {
            return RpcMonitorSpec.Load(Self, UnitContainer);
        }

        public IMessageFactory LoadMessageFactory()
        {
            return MessageFactorySpec.Load(Self, UnitContainer);
        }

        public IContactFactory LoadContactFactory()
        {
            return ContactFactorySpec.Load(Self, UnitContainer);
        }

        public INetworkContext LoadNetworkContext()
        {
            return NetworkContextSpec.Load(Self, UnitContainer);
        }
    }

}
