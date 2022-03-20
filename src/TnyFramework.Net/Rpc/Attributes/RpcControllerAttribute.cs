using System;
using TnyFramework.Net.Message;
namespace TnyFramework.Net.Rpc.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class RpcControllerAttribute : Attribute
    {
        public RpcControllerAttribute(params MessageMode[] messageModes)
        {
            MessageModes = messageModes;
        }


        public RpcControllerAttribute()
        {
            var values = Enum.GetValues(typeof(MessageMode));
            MessageModes = new MessageMode[values.Length];
            var index = 0;
            foreach (var value in values)
            {
                MessageModes[index] = (MessageMode)value;
                index++;
            }
        }


        public MessageMode[] MessageModes { get; set; }
        
        
    }
}
