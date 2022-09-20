// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Runtime.Serialization;

namespace TnyFramework.Common.Exceptions
{

    public class CommonException : ApplicationException
    {
        public CommonException()
        {
        }

        public CommonException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public CommonException(string message) : base(message)
        {
        }

        public CommonException(Exception innerException, string message) : base(message, innerException)
        {
        }
    }

}
