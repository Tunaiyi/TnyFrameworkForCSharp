#region

using System;

#endregion

namespace TnyFramework.Net.TypeProtobuf.Attributes
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
