// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;

namespace TnyFramework.Net.Attributes
{

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class AfterPluginAttribute : PluginAttribute
    {
        /// <summary>
        /// 执行后插件
        /// </summary>
        /// <param name="pluginType">插件类型, pluginType 必须 实现 ICommandPlugin</param>
        /// <param name="attribute">参数</param>
        public AfterPluginAttribute(Type pluginType, string attribute = default) : base(pluginType, attribute)
        {
        }

        public override bool IsBefore()
        {
            return false;
        }

        public override bool IsAfter()
        {
            return true;
        }
    }

}
