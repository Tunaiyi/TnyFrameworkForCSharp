// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;

namespace TnyFramework.DI.Attributes
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class ComponentAttribute : Attribute
    {
        public string Name { get; }

        public DIMode Mode { get; }

        public bool Lazy { get; }

        public ComponentAttribute(DIMode mode) : this()
        {
        }

        public ComponentAttribute(string name = "", bool lazy = false, DIMode mode = DIMode.Singleton)
        {
            Name = name;
            Lazy = lazy;
            Mode = mode;
        }

        public bool Named()
        {
            return !string.IsNullOrEmpty(Name);
        }
    }

}
