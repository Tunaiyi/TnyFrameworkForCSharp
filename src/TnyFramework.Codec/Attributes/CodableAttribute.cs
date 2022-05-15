using System;

namespace TnyFramework.Codec.Attributes
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class CodableAttribute : Attribute
    {
        /// <summary>
        /// 协议 id
        /// </summary>
        public string MimeType { get; }

        public CodableAttribute(string mimeType)
        {
            MimeType = mimeType;
        }
    }

}
