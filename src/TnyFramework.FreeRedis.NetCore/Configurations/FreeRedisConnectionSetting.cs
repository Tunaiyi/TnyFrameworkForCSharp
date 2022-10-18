// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using FreeRedis;

namespace TnyFramework.FreeRedis.NetCore.Configurations
{

    public class FreeRedisConnectionSetting : FreeRedisParamSetting
    {
        public string Host { get; set; }

        public bool? Ssl { get; set; }

        internal ConnectionStringBuilder ToConnectionBuilder()
        {
            return ToConnectionBuilder(this, this);
        }

        internal ConnectionStringBuilder ToConnectionBuilder(FreeRedisParamSetting setting)
        {
            return ToConnectionBuilder(this, setting);
        }

        internal ConnectionStringBuilder ToConnectionBuilder(FreeRedisConnectionSetting urlSetting, FreeRedisParamSetting setting)
        {
            var builder = new ConnectionStringBuilder();
            if (urlSetting.Host != null)
            {
                builder.Host = urlSetting.Host;
            }
            if (urlSetting.Ssl != null)
            {
                builder.Ssl = urlSetting.Ssl.Value;
            }
            UpdateConnectionBuilder(builder, setting);
            UpdateConnectionBuilder(builder, urlSetting);
            return builder;
        }

        private static void UpdateConnectionBuilder(ConnectionStringBuilder builder, FreeRedisParamSetting setting)
        {
            if (!setting.Update)
            {
                return;
            }
            if (!setting.Update)
            {
                return;
            }
            if (setting.Protocol != null)
            {
                builder.Protocol = setting.Protocol.Value;
            }
            if (setting.User != null)
            {
                builder.User = setting.User;
            }
            if (setting.Password != null)
            {
                builder.Password = setting.Password;
            }
            if (setting.Database != null)
            {
                builder.Database = setting.Database.Value;
            }
            if (setting.Prefix != null)
            {
                builder.Prefix = setting.Prefix;
            }
            if (setting.ClientName != null)
            {
                builder.ClientName = setting.ClientName;
            }
            if (setting.Encoding != null)
            {
                builder.Encoding = System.Text.Encoding.GetEncoding(setting.Encoding);
            }
            if (setting.IdleTimeout != null)
            {
                builder.IdleTimeout = TimeSpan.FromMilliseconds(setting.IdleTimeout.Value);
            }
            if (setting.ConnectTimeout != null)
            {
                builder.ConnectTimeout = TimeSpan.FromMilliseconds(setting.ConnectTimeout.Value);
            }
            if (setting.ReceiveTimeout != null)
            {
                builder.ReceiveTimeout = TimeSpan.FromMilliseconds(setting.ReceiveTimeout.Value);
            }
            if (setting.SendTimeout != null)
            {
                builder.SendTimeout = TimeSpan.FromMilliseconds(setting.SendTimeout.Value);
            }
            if (setting.MaxPoolSize != null)
            {
                builder.MaxPoolSize = setting.MaxPoolSize.Value;
            }
            if (setting.MinPoolSize != null)
            {
                builder.MinPoolSize = setting.MinPoolSize.Value;
            }
            if (setting.Retry != null)
            {
                builder.Retry = setting.Retry.Value;
            }
        }
    }

}
