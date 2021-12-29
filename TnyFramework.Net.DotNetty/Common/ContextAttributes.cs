using TnyFramework.Common.Attribute;
namespace TnyFramework.Net.DotNetty.Common
{
    public class ContextAttributes
    {
        private volatile IAttributes attributes;

        public IAttributes Attributes {
            get {
                if (attributes != null)
                {
                    return attributes;
                }
                lock (this)
                {
                    if (attributes != null)
                    {
                        return attributes;
                    }
                    return attributes = new DefaultAttributes();
                }
            }
        }
    }
}
