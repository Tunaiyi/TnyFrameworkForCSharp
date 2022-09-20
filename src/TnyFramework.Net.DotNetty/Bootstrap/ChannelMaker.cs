// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using DotNetty.Transport.Channels;

namespace TnyFramework.Net.DotNetty.Bootstrap
{

    public interface IChannelMaker
    {
        void InitChannel(IChannel channel);
    }

    public interface IChannelMaker<in TC> : IChannelMaker
        where TC : IChannel
    {
        void InitChannel(TC channel);
    }

    public abstract class AbstractChannelMaker<TC> : IChannelMaker<TC>
        where TC : IChannel
    {
        public abstract void InitChannel(TC channel);

        public void InitChannel(IChannel channel)
        {
            InitChannel((TC) channel);
        }
    }

}
