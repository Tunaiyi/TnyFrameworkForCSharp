// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;

namespace TnyFramework.Common.Scanner
{

    public class TypeSelectedHandler : ITypeSelectedHandler
    {
        private readonly Action<ICollection<Type>> handler;

        public TypeSelectedHandler(Action<ICollection<Type>> handler)
        {
            this.handler = handler;
        }

        public void Handle(ICollection<Type> types)
        {
            handler(types);
        }
    }

    public interface ITypeSelectedHandler
    {
        void Handle(ICollection<Type> types);
    }

}
