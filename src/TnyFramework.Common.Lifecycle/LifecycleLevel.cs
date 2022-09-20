// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Common.Enum;

namespace TnyFramework.Common.Lifecycle
{

    /// <summary>
    /// 初始化优先级
    ///    优先级越高 越先初始化 大 -> 小
    /// </summary>
    public class LifecycleLevel : BaseEnum<LifecycleLevel>, ILifecyclePriority
    {
        public static readonly LifecycleLevel SYSTEM_LEVEL_1 = Of(9001);

        public static readonly LifecycleLevel SYSTEM_LEVEL_2 = Of(9002);

        public static readonly LifecycleLevel SYSTEM_LEVEL_3 = Of(9003);

        public static readonly LifecycleLevel SYSTEM_LEVEL_4 = Of(9004);

        public static readonly LifecycleLevel SYSTEM_LEVEL_5 = Of(9005);

        public static readonly LifecycleLevel SYSTEM_LEVEL_6 = Of(9006);

        public static readonly LifecycleLevel SYSTEM_LEVEL_7 = Of(9007);

        public static readonly LifecycleLevel SYSTEM_LEVEL_8 = Of(9008);

        public static readonly LifecycleLevel SYSTEM_LEVEL_9 = Of(9009);

        public static readonly LifecycleLevel SYSTEM_LEVEL_10 = Of(9010);

        public static readonly LifecycleLevel SYSTEM_LEVEL_MAX = Of(int.MaxValue);

        public static readonly LifecycleLevel CUSTOM_LEVEL_1 = Of(1001);

        public static readonly LifecycleLevel CUSTOM_LEVEL_2 = Of(1002);

        public static readonly LifecycleLevel CUSTOM_LEVEL_3 = Of(1003);

        public static readonly LifecycleLevel CUSTOM_LEVEL_4 = Of(1004);

        public static readonly LifecycleLevel CUSTOM_LEVEL_5 = Of(1005);

        public static readonly LifecycleLevel CUSTOM_LEVEL_6 = Of(1006);

        public static readonly LifecycleLevel CUSTOM_LEVEL_7 = Of(1007);

        public static readonly LifecycleLevel CUSTOM_LEVEL_8 = Of(1008);

        public static readonly LifecycleLevel CUSTOM_LEVEL_9 = Of(1009);

        public static readonly LifecycleLevel CUSTOM_LEVEL_10 = Of(1010);

        public static readonly LifecycleLevel POST_SYSTEM_LEVEL_1 = Of(101);

        public static readonly LifecycleLevel POST_SYSTEM_LEVEL_2 = Of(102);

        public static readonly LifecycleLevel POST_SYSTEM_LEVEL_3 = Of(103);

        public static readonly LifecycleLevel POST_SYSTEM_LEVEL_4 = Of(104);

        public static readonly LifecycleLevel POST_SYSTEM_LEVEL_5 = Of(105);

        public static readonly LifecycleLevel POST_SYSTEM_LEVEL_6 = Of(106);

        public static readonly LifecycleLevel POST_SYSTEM_LEVEL_7 = Of(107);

        public static readonly LifecycleLevel POST_SYSTEM_LEVEL_8 = Of(108);

        public static readonly LifecycleLevel POST_SYSTEM_LEVEL_9 = Of(109);

        public static readonly LifecycleLevel POST_SYSTEM_LEVEL_10 = Of(110);

        public int Order { get; private set; }

        public LifecycleLevel()
        {
        }

        public LifecycleLevel(int order)
        {
            Order = order;
        }

        protected static LifecycleLevel Of(int id)
        {
            return E(id, new LifecycleLevel {
                Order = id
            });
        }
    }

}
