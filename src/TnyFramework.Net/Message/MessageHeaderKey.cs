// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;

namespace TnyFramework.Net.Message
{

    public abstract class MessageHeaderKey
    {
        public string Key { get; }

        public Type HeaderType { get; }

        protected MessageHeaderKey(string key, Type headerType)
        {
            Key = key;
            HeaderType = headerType;
        }

        private bool Equals(MessageHeaderKey other)
        {
            return Key == other.Key && HeaderType == other.HeaderType;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((MessageHeaderKey) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Key.GetHashCode() * 397) ^ HeaderType.GetHashCode();
            }
        }
    }

    /// <summary>
    /// 消息头信息 Key
    /// </summary>
    /// <typeparam name="TH"></typeparam>
    public class MessageHeaderKey<TH> : MessageHeaderKey where TH : MessageHeader
    {
        public static MessageHeaderKey<TH> Of(string key)
        {
            return new MessageHeaderKey<TH>(key);
        }

        private MessageHeaderKey(string key) : base(key, typeof(TH))
        {
        }
    }

}
