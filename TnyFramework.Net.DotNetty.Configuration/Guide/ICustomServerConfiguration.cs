using System;
namespace TnyFramework.Net.DotNetty.Configuration.Guide
{
    public interface ICustomServerConfiguration
    {
        void Configure(NettyServerConfiguration configuration);
    }

    public class ActionCustomServerConfiguration : ICustomServerConfiguration
    {
        private readonly Action<NettyServerConfiguration> configurator;


        public ActionCustomServerConfiguration(Action<NettyServerConfiguration> configurator)
        {
            this.configurator = configurator;
        }


        public void Configure(NettyServerConfiguration configuration)
        {
            configurator.Invoke(configuration);
        }
    }
}
