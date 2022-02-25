using System;
namespace TnyFramework.Net.Rpc.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class MsgParamAttribute : Attribute
    {
        public MsgParamAttribute(string name, bool require = true)
        {
            Name = name;
        }


        public MsgParamAttribute(int index, bool require = true)
        {
            Index = index;
        }


        public MsgParamAttribute()
        {
        }


        /// <summary>
        /// key 值, 与 index 互斥
        /// </summary>
        public string Name { get; } = "";

        /// <summary>
        /// Index 值, 与 Key 互斥, 默认 -1, 按 MsgParam 顺序
        /// </summary>
        public int Index { get; } = -1;

        /// <summary>
        /// 是否是必须的, 默认为 true
        /// </summary>
        public bool Require { get; set; } = true;
    }
}
