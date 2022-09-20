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

namespace TnyFramework.Common.Assemblies.Attributes
{

    [AttributeUsage(AttributeTargets.Assembly)]
    public abstract class AssemblyAutoLoadAttributes : System.Attribute
    {
        public IList<string> LoadClasses { get; }

        protected AssemblyAutoLoadAttributes(string loadClass, params string[] loadClasses)
        {
            var classes = new List<string> {loadClass};
            classes.AddRange(loadClasses);
            LoadClasses = classes.ToImmutableList();
        }
    }

}
