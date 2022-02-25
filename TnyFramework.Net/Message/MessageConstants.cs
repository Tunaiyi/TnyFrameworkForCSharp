namespace TnyFramework.Net.Message
{
    public static class MessageConstants
    {
        // long MESSAGE_MASK_BIT_SIZE = 1;
        public const long SENDER_MESSAGE_ID_MASK = 1;
        
        public static readonly ulong MESSAGE_ID_MASK = ulong.MaxValue >> 63;
        
        public const long EMPTY_MESSAGE_ID = 0;
        
    }
}
