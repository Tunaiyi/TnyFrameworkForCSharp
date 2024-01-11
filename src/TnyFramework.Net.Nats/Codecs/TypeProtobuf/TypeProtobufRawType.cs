// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;

namespace TnyFramework.Net.Nats.Codecs.TypeProtobuf
{

    [Flags]
    internal enum ProtobufRawType
    {
        /// <summary>
        /// null
        /// </summary>
        Null = 0,

        /// <summary>
        /// byte
        /// </summary>
        Byte = 1,

        /// <summary>
        /// short
        /// </summary>
        Short = 2,

        /// <summary>
        /// int
        /// </summary>
        Int = 3,

        /// <summary>
        /// long
        /// </summary>
        Long = 4,

        /// <summary>
        /// float
        /// </summary>
        Float = 5,

        /// <summary>
        /// double
        /// </summary>
        Double = 6,

        /// <summary>
        /// bool
        /// </summary>
        Bool = 7,

        /// <summary>
        /// string
        /// </summary>
        String = 8,

        /// <summary>
        /// complex
        /// </summary>
        Complex = 9,
    }

}
