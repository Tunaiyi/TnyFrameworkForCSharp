using TnyFramework.Common.Attribute;
namespace TnyFramework.Net.DotNetty.Message
{
    /// <summary>
    /// 消息接口
    /// </summary>
    public interface IMessage : IMessageHead, IMessageContent
    {
        /// <summary>
        /// 消息头
        /// </summary>
        IMessageHead Head { get; }

        /// <summary>
        /// 附加属性
        /// </summary>
        IAttributes Attribute { get; }
    }
}
