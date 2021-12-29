using System;
using DotNetty.Transport.Channels;
namespace TnyFramework.Net.DotNetty.Bootstrap
{
    public interface IChannelMaker
    {
        void InitChannel(object channel);
    }
    
    public interface IChannelMaker<in TC> : IChannelMaker
        where TC:IChannel 
    {
        void InitChannel(TC channel);
        
    }

    public abstract class AbstractChannelMaker<TC> : IChannelMaker<TC>
        where TC:IChannel 
    {
        public abstract void InitChannel(TC channel);
        
        public void InitChannel(object channel)
        {
            InitChannel((TC)channel);
        }
    }

}
