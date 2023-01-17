// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace TnyFramework.Navmesh.Navmesh.Collection
{

    /// <summary>
    /// 
    ///    |y
    ///    |
    ///    |_______x
    ///   /
    ///  /z
    /// 
    /// </summary>
    public struct QuotaVector2 : IQuotaVector2

    {
        public int X { get; set; }

        public int Y { get; set; }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="size">x, y 的大小</param>
        /// <returns></returns>
        public QuotaVector2(int size)
        {
            X = size;
            Y = size;
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="x">x的大小</param>
        /// <param name="y">y的大小</param>
        /// <returns></returns>
        public QuotaVector2(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

}
