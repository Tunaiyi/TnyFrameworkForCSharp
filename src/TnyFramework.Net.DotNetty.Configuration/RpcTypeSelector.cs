using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using TnyFramework.Common.Scanner;
using TnyFramework.Net.Attributes;
using TnyFramework.Net.Rpc.Attributes;

namespace TnyFramework.Net.DotNetty.Configuration
{

    public class RpcTypeSelector : TypeSelectorDefinition
    {
        internal static IList<Type> Controllers { get; private set; }

        internal static IList<Type> RemoteService { get; private set; }

        public RpcTypeSelector()
        {
            Selector(selector => selector
                .AddFilter(AttributeTypeFilter.OfInclude<RpcControllerAttribute>())
                .WithHandler(OnLoadController)
            );

            Selector(selector => selector
                .AddFilter(AttributeTypeFilter.OfInclude<RpcRemoteServiceAttribute>())
                .AddFilter(TypeFilter.Include(type => type.IsInterface))
                .WithHandler(OnLoadService)
            );
        }

        private static void OnLoadController(ICollection<Type> types)
        {
            Controllers = types.ToImmutableList();
        }

        private static void OnLoadService(ICollection<Type> types)
        {
            RemoteService = types.ToImmutableList();
        }
    }

}