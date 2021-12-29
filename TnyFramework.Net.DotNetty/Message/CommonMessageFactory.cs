namespace TnyFramework.Net.DotNetty.Message
{
    /// <summary>
    /// CommonMessage 工厂
    /// </summary>
    public class CommonMessageFactory : IMessageFactory
    {
        public INetMessage Create(long id, IMessageContent subject)
        {
            return new CommonMessage(new CommonMessageHead(id, subject), subject.Body);
        }


        public INetMessage Create(INetMessageHead head, object body)
        {
            return new CommonMessage(head, body);
        }
    }
}
