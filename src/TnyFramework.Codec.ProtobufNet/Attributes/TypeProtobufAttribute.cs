using System;

namespace TnyFramework.Codec.ProtobufNet.Attributes
{

    public class TypeProtobufAttribute : Attribute
    {
        public TypeProtobufAttribute(int id)
        {
            Id = id;
        }

        public int Id { get; }
    }

}
