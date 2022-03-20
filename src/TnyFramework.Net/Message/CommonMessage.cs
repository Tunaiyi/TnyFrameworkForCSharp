namespace TnyFramework.Net.Message
{
    /// <summary>
    /// 通用消息对象
    /// </summary>
    public class CommonMessage : AbstractNetMessage
    {
        public CommonMessage(INetMessageHead head) : base(head)
        {
        }


        public CommonMessage(INetMessageHead head, object body) : base(head, body)
        {
        }
    }
}
