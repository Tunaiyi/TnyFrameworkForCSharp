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

    public abstract class NetGuideSpec<TGuide, TContext, TImplContext, TSpec>
        : UnitSpec<TGuide, TContext>, INetGuideSpec<TGuide, TContext, TSpec>
        where TGuide : INetServer
        where TContext : INetGuideUnitContext
        where TImplContext : NetGuideUnitContext<TContext>, TContext
        where TSpec : INetGuideSpec<TGuide, TContext, TSpec>
    {
        protected readonly TImplContext context;

        protected TGuide? Guide { get; set; }

        protected IServiceCollection UnitContainer { get; }

        protected NetGuideSpec(IServiceCollection unitContainer, TImplContext context)
        {
            UnitContainer = unitContainer;
            this.context = context;
        }

        protected abstract TSpec Self();

        public TSpec MessageConfigure(Action<IUnitSpec<IMessageFactory, TContext>> action)
        {
            action(context.MessageFactorySpec);
            return Self();
        }

        public TSpec ContactConfigure(Action<IUnitSpec<IContactFactory, TContext>> action)
        {
            action(context.ContactFactorySpec);
            return Self();
        }

        public TSpec RpcMonitorConfigure(Action<UnitSpec<RpcMonitor, TContext>> action)
        {
            action(context.RpcMonitorSpec);
            return Self();
        }

        public override TGuide Load(TContext context, IServiceCollection services)
        {
            if (Guide != null)
                return Guide;
            return Guide = base.Load(context, UnitContainer);
        }

        public TGuide BuildGuide()
        {
            return Load(context, UnitContainer);
        }
    }

}
