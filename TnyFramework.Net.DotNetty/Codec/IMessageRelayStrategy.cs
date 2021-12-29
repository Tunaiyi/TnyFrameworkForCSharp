using TnyFramework.Net.DotNetty.Message;
namespace TnyFramework.Net.DotNetty.Codec
{
    public interface IMessageRelayStrategy
    {
        bool IsRelay(IMessageHead head);
    }

    public class NeverRelayStrategy : IMessageRelayStrategy
    {
        private static readonly NeverRelayStrategy STRATEGY = new NeverRelayStrategy();

        public bool IsRelay(IMessageHead head) => false;

        public static NeverRelayStrategy Strategy => STRATEGY;
    }
}
