namespace TnyFramework.Common.Attribute
{

    public class AttributesContext
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
