// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.Attributes
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class RpcControllerAttribute : Attribute
    {
        public RpcControllerAttribute(params MessageMode[] messageModes)
        {
            MessageModes = messageModes;
        }

        public RpcControllerAttribute()
        {
            var values = Enum.GetValues(typeof(MessageMode));
            MessageModes = new MessageMode[values.Length];
            var index = 0;
            foreach (var value in values)
            {
                MessageModes[index] = (MessageMode) value;
                index++;
            }
        }

        public MessageMode[] MessageModes { get; set; }
    }

}
