namespace TnyFramework.Net.DotNetty.Message
{
    public interface INetMessageHead : IMessageHead
    {
        void AllotMessageId(long id);
    }
}
