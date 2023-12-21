// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;

namespace TnyFramework.Common.Exceptions
{

    public static class Assets
    {
        public static void CheckArguments(bool predicate, string message, params object[] paramList)
        {
            if (!predicate)
            {
                throw new IllegalArgumentException(string.Format(message, paramList));
            }
        }

        public static void CheckArguments(Func<bool> predicate, string message, params object[] paramList)
        {
            if (!predicate())
            {
                throw new IllegalArgumentException(string.Format(message, paramList));
            }
        }

        public static void CheckNotNull(object value, string message, params object[] paramList)
        {
            if (value == null)
            {
                throw new NullReferenceException(string.Format(message, paramList));
            }
        }
    }

}
