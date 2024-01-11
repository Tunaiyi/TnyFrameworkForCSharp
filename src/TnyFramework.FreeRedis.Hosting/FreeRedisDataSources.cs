// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using FreeRedis;
using TnyFramework.Codec;
using TnyFramework.Common.Extensions;
using TnyFramework.FreeRedis.Hosting.Configurations;

namespace TnyFramework.FreeRedis.Hosting;

public class FreeRedisDataSources
{
    private readonly IDictionary<string, RedisClient> redisClients;

    public RedisClient DefaultClient { get; }

    public FreeRedisDataSources(FreeRedisProperties properties, ObjectCodecAdapter codecAdapter)
    {
        if (properties.Default != null)
        {
            DefaultClient = CreateClient(properties.Default, codecAdapter);
        } else
        {
            DefaultClient = null!;
        }
        redisClients = properties.DataSources
            .ToDictionary(pair => pair.Key, pair => CreateClient(pair.Value, codecAdapter))
            .ToImmutableDictionary();
    }

    private RedisClient CreateClient(FreeRedisSetting setting, ObjectCodecAdapter codecAdapter)
    {
        var builders = setting.Connections.Select(connection => connection.ToConnectionBuilder()).ToList();
        MimeType? mimeType = null;
        if (setting.Mime.IsNotBlank())
        {
            mimeType = MimeType.ForMimeType(setting.Mime);
        }
        var encoding = setting.Encoding.IsNotBlank() ? Encoding.GetEncoding(setting.Encoding) : Encoding.UTF8;
        var redisCodec = new FreeRedisCodecs(codecAdapter, mimeType, encoding);
        RedisClient client;
        if (builders.Count == 1)
        {
            client = new RedisClient(builders[0]);
        } else
        {
            client = new RedisClient(builders.ToArray());
        }
        client.Serialize = redisCodec.Serialize;
        client.Deserialize = redisCodec.Deserialize;
        client.DeserializeRaw = redisCodec.DeserializeRaw;
        return client;
    }

    public RedisClient Client(Type type)
    {
        var scheme = RedisObjectScheme.SchemeOf(type);
        if (scheme == null)
        {
            throw new NullReferenceException($"Could not find the scheme of {type}");
        }
        if (scheme.Source.IsBlank())
        {
            if (DefaultClient == null)
            {
                throw new NullReferenceException($"Could not find the default client");
            }
            return DefaultClient;
        }

        var client = redisClients.Get(scheme.Source);
        if (client == null)
        {
            throw new NullReferenceException($"Could not find the client {scheme.Source}");
        }
        return client;
    }

    public RedisClient Client<TValue>()
    {
        return Client(typeof(TValue));
    }
}
