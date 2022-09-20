// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;

namespace TnyFramework.DI.Container
{

    public class ObjectServiceInstance<T> : IServiceInstance
    {
        private readonly T instance;

        public ObjectServiceInstance(T instance)
        {
            if (instance == null)
            {
                throw new NullReferenceException();
            }
            this.instance = instance;
        }

        public object Get(IServiceProvider provider)
        {
            return instance;
        }
    }

}
