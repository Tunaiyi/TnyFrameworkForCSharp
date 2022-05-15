using System;

namespace TnyFramework.Net.Attributes
{

    [AttributeUsage(AttributeTargets.Parameter)]
    public class RpcParamAttribute : Attribute
    {
        public RpcParamAttribute(int index = -1)
        {
            Index = index;
        }

        /// <summary>
        /// Index 值, 与 Key 互斥, 默认 -1, 按 MsgParam 顺序
        /// </summary>
        public int Index { get; } = -1;
    }

}
