namespace TnyFramework.Net.Message
{

    public interface IOctetMessageBody
    {
        bool Relay { get; }

        object Body { get; }

        void Release();
    }

    public interface IOctetMessageBody<out T> : IOctetMessageBody
    {
        new T Body { get; }
    }

}
