// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeRedis;

namespace TnyFramework.FreeRedis.NetCore.Client
{

    public static class FreeRedisExtensions
    {
        
        // public static async  Task<ZMember<byte[]>> BZPopMinAsync(this RedisClient self, string key, int timeoutSeconds) => (await BZPopAsync(self, false, new[] { key }, timeoutSeconds))?.value;
        // public static Task<KeyValue<ZMember>> BZPopMinAsync(this RedisClient self,string[] keys, int timeoutSeconds) => await BZPopAsync(self, false, keys, timeoutSeconds);
        // public static async  Task<ZMember> BZPopMaxAsync(this RedisClient self,string key, int timeoutSeconds) => (await BZPopAsync(self, true, new[] { key }, timeoutSeconds))?.value;
        // public static Task<KeyValue<ZMember>> BZPopMaxAsync(this RedisClient self,string[] keys, int timeoutSeconds) => BZPopAsync(self, true, keys, timeoutSeconds);
        //
        // private static async Task<KeyValue<ZMember>> BZPopAsync(this RedisClient self, bool ismax, string[] keys, int timeoutSeconds)
        // {
        //     
        //     var value = await self.CallAsync((ismax ? "BZPOPMAX" : "BZPOPMIN").InputKey(keys, timeoutSeconds)); 
        //     var a = (object[])value;
        //     a == null || a.Length == 0 ? null : 
        //         new KeyValue<ZMember>(
        //             (byte[])a[0].ConvertTo<string>(),
        //             new ZMember(a[1].ConvertTo<byte[]>(), 
        //                 a[2].ConvertTo<decimal>()
        //                     .ConvertTo<decimal>())))
        // }
        //
        // public static Task<ZMember> ZPopMinAsync(this RedisClient self, string key) => ZPopAsync(false, key);
        // public static Task<ZMember[]> ZPopMinAsync(this RedisClient self, string key, int count) => ZPopAsync(false, key, count);
        // public static Task<ZMember> ZPopMaxAsync(this RedisClient self, string key) => ZPopAsync(true, key);
        // public static Task<ZMember[]> ZPopMaxAsync(this RedisClient self, string key, int count) => ZPopAsync(true, key, count);
        // private static Task<ZMember> ZPopAsync(this RedisClient self, bool ismax, string key) => CallAsync((ismax ? "ZPOPMAX" : "ZPOPMIN").InputKey(key), rt => rt
        //     .ThrowOrValue((a, _) => a == null || a.Length == 0 ? null : new ZMember(a[0].ConvertTo<string>(), a[1].ConvertTo<decimal>())));
        // private static Task<ZMember[]> ZPopAsync(this RedisClient self, bool ismax, string key, int count) => CallAsync((ismax ? "ZPOPMAX" : "ZPOPMIN").InputKey(key, count), rt => rt
        //     .ThrowOrValue((a, _) => a == null || a.Length == 0 ? new ZMember[0] : a.MapToHash<decimal>(rt.Encoding).Select(b => new ZMember(b.Key, b.Value)).ToArray()));
        //
        // public static Task<string[]> ZRangeAsync(this RedisClient self, string key, decimal start, decimal stop) => CallAsync("ZRANGE".InputKey(key, start, stop), rt => rt.ThrowOrValue<string[]>());
        // public static Task<ZMember[]> ZRangeWithScoresAsync(this RedisClient self, string key, decimal start, decimal stop) => CallAsync("ZRANGE"
        //     .InputKey(key, start, stop)
        //     .Input("WITHSCORES"), rt => rt
        //     .ThrowOrValue((a, _) => a == null || a.Length == 0 ? new ZMember[0] : a.MapToHash<decimal>(rt.Encoding).Select(b => new ZMember(b.Key, b.Value)).ToArray()));
        //
        // public static Task<string[]> ZRangeByLexAsync(this RedisClient self, string key, string min, string max, int offset = 0, int count = 0) => CallAsync("ZRANGEBYLEX"
        //     .InputKey(key, min, max)
        //     .InputIf(offset > 0 || count > 0, "LIMIT", offset, count), rt => rt.ThrowOrValue<string[]>());
        // public static Task<string[]> ZRangeByScoreAsync(this RedisClient self, string key, decimal min, decimal max, int offset = 0, int count = 0) => CallAsync("ZRANGEBYSCORE"
        //     .InputKey(key, min, max)
        //     .InputIf(offset > 0 || count > 0, "LIMIT", offset, count), rt => rt.ThrowOrValue<string[]>());
        // public static Task<string[]> ZRangeByScoreAsync(this RedisClient self, string key, string min, string max, int offset = 0, int count = 0) => CallAsync("ZRANGEBYSCORE"
        //     .InputKey(key, min, max)
        //     .InputIf(offset > 0 || count > 0, "LIMIT", offset, count), rt => rt.ThrowOrValue<string[]>());
        // public static Task<ZMember[]> ZRangeByScoreWithScoresAsync(this RedisClient self, string key, decimal min, decimal max, int offset = 0, int count = 0) => CallAsync("ZRANGEBYSCORE"
        //     .InputKey(key, min, max)
        //     .Input("WITHSCORES")
        //     .InputIf(offset > 0 || count > 0, "LIMIT", offset, count), rt => rt
        //     .ThrowOrValue((a, _) => a == null || a.Length == 0 ? new ZMember[0] : a.MapToHash<decimal>(rt.Encoding).Select(b => new ZMember(b.Key, b.Value)).ToArray()));
        // public static Task<ZMember[]> ZRangeByScoreWithScoresAsync(this RedisClient self, string key, string min, string max, int offset = 0, int count = 0) => CallAsync("ZRANGEBYSCORE"
        //     .InputKey(key, min, max)
        //     .Input("WITHSCORES")
        //     .InputIf(offset > 0 || count > 0, "LIMIT", offset, count), rt => rt
        //     .ThrowOrValue((a, _) => a == null || a.Length == 0 ? new ZMember[0] : a.MapToHash<decimal>(rt.Encoding).Select(b => new ZMember(b.Key, b.Value)).ToArray()));
        //
        //
        // public static Task<string[]> ZRevRangeAsync(this RedisClient self, string key, decimal start, decimal stop) => CallAsync("ZREVRANGE".InputKey(key, start, stop), rt => rt.ThrowOrValue<string[]>());
        // public static Task<ZMember[]> ZRevRangeWithScoresAsync(this RedisClient self, string key, decimal start, decimal stop) => CallAsync("ZREVRANGE"
        //     .InputKey(key, start, stop)
        //     .Input("WITHSCORES"), rt => rt
        //     .ThrowOrValue((a, _) => a == null || a.Length == 0 ? new ZMember[0] : a.MapToHash<decimal>(rt.Encoding).Select(b => new ZMember(b.Key, b.Value)).ToArray()));
        // public static Task<string[]> ZRevRangeByLexAsync(this RedisClient self, string key, decimal max, decimal min, int offset = 0, int count = 0) => CallAsync("ZREVRANGEBYLEX"
        //     .InputKey(key, max, min)
        //     .InputIf(offset > 0 || count > 0, "LIMIT", offset, count), rt => rt.ThrowOrValue<string[]>());
        // public static Task<string[]> ZRevRangeByLexAsync(this RedisClient self, string key, string max, string min, int offset = 0, int count = 0) => CallAsync("ZREVRANGEBYLEX"
        //     .InputKey(key, max, min)
        //     .InputIf(offset > 0 || count > 0, "LIMIT", offset, count), rt => rt.ThrowOrValue<string[]>());
        // public static Task<string[]> ZRevRangeByScoreAsync(this RedisClient self, string key, decimal max, decimal min, int offset = 0, int count = 0) => CallAsync("ZREVRANGEBYSCORE"
        //     .InputKey(key, max, min)
        //     .InputIf(offset > 0 || count > 0, "LIMIT", offset, count), rt => rt.ThrowOrValue<string[]>());
        // public static Task<string[]> ZRevRangeByScoreAsync(this RedisClient self, string key, string max, string min, int offset = 0, int count = 0) => CallAsync("ZREVRANGEBYSCORE"
        //     .InputKey(key, max, min)
        //     .InputIf(offset > 0 || count > 0, "LIMIT", offset, count), rt => rt.ThrowOrValue<string[]>());
        // public static Task<ZMember[]> ZRevRangeByScoreWithScoresAsync(this RedisClient self, string key, decimal max, decimal min, int offset = 0, int count = 0) => CallAsync("ZREVRANGEBYSCORE"
        //     .InputKey(key, max, min)
        //     .Input("WITHSCORES")
        //     .InputIf(offset > 0 || count > 0, "LIMIT", offset, count), rt => rt
        //     .ThrowOrValue((a, _) => a == null || a.Length == 0 ? new ZMember[0] : a.MapToHash<decimal>(rt.Encoding).Select(b => new ZMember(b.Key, b.Value)).ToArray()));
        // public static Task<ZMember[]> ZRevRangeByScoreWithScoresAsync(this RedisClient self, string key, string max, string min, int offset = 0, int count = 0) => CallAsync("ZREVRANGEBYSCORE"
        //     .InputKey(key, max, min)
        //     .Input("WITHSCORES")
        //     .InputIf(offset > 0 || count > 0, "LIMIT", offset, count), rt => rt
        //     .ThrowOrValue((a, _) => a == null || a.Length == 0 ? new ZMember[0] : a.MapToHash<decimal>(rt.Encoding).Select(b => new ZMember(b.Key, b.Value)).ToArray()));
    }

}
