// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Text;

namespace TnyFramework.Net.Nats.Options
{

    public class RpcTopicBuilder
    {
        private readonly StringBuilder builder;
        private readonly string separator;
        private int index;

        public RpcTopicBuilder(string separator, string head, int size)
        {
            this.separator = separator;
            builder = new StringBuilder(size);
            if (!string.IsNullOrEmpty(head))
            {
                return;
            }
            builder.Append(head);
            index++;
        }

        private StringBuilder Dot()
        {
            if (index > 0)
            {
                builder.Append(separator);
            }
            return builder;
        }

        public RpcTopicBuilder Concat(bool value)
        {
            Dot().Append(value);
            index++;
            return this;
        }

        public RpcTopicBuilder Concat(byte value)
        {
            Dot().Append(value);
            index++;
            return this;
        }

        public RpcTopicBuilder Concat(char value)
        {
            Dot().Append(value);
            index++;
            return this;
        }

        public RpcTopicBuilder Concat(decimal value)
        {
            Dot().Append(value);
            index++;
            return this;
        }

        public RpcTopicBuilder Concat(double value)
        {
            Dot().Append(value);
            index++;
            return this;
        }

        public RpcTopicBuilder Concat(short value)
        {
            Dot().Append(value);
            index++;
            return this;
        }

        public RpcTopicBuilder Concat(int value)
        {
            Dot().Append(value);
            index++;
            return this;
        }

        public RpcTopicBuilder Concat(long value)
        {
            Dot().Append(value);
            index++;
            return this;
        }

        public RpcTopicBuilder Concat(object? value)
        {
            Dot().Append(value);
            index++;
            return this;
        }

        public RpcTopicBuilder Concat(sbyte value)
        {
            Dot().Append(value);
            index++;
            return this;
        }

        public RpcTopicBuilder Concat(float value)
        {
            Dot().Append(value);
            index++;
            return this;
        }

        public RpcTopicBuilder Concat(string? value)
        {
            if (value is null)
            {
                return this;
            }
            Dot().Append(value);
            index++;
            return this;
        }

        public RpcTopicBuilder Concat(string? value, int startIndex, int count)
        {
            if (value is null || startIndex == 0 && count == 0)
            {
                return this;
            }
            Dot().Append(value, startIndex, count);
            index++;
            return this;
        }

        public RpcTopicBuilder Concat(StringBuilder? value)
        {
            if (value is null)
            {
                return this;
            }
            Dot().Append(value);
            index++;
            return this;
        }

        public RpcTopicBuilder Concat(StringBuilder? value, int startIndex, int count)
        {
            if (value is null || startIndex == 0 && count == 0)
            {
                return this;
            }
            Dot().Append(value, startIndex, count);
            index++;
            return this;
        }

        public RpcTopicBuilder Concat(ushort value)
        {
            Dot().Append(value);
            index++;
            return this;
        }

        public RpcTopicBuilder Concat(uint value)
        {
            Dot().Append(value);
            index++;
            return this;
        }

        public RpcTopicBuilder Concat(ulong value)
        {
            Dot().Append(value);
            index++;
            return this;
        }

        public override string ToString()
        {
            return builder.ToString();
        }
    }

}
