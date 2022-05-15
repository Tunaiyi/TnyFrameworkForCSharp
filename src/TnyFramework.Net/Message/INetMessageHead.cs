namespace TnyFramework.Net.Message
{

    public interface INetMessageHead : IMessageHead
    {
        void AllotMessageId(long id);
    }

}
