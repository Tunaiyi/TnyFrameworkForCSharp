#region

using System;

#endregion

namespace TnyFramework.Net.TypeProtobuf
{

    public interface ITypeProtobufObjectFactory
    {
        T Create<T>(int id) where T : new();

        bool Id<T>(out int id) where T : new();

        bool Id(Type type, out int id);

        ITypeProtobufObjectFactory Register(Type type);

        ITypeProtobufObjectFactory Register<T>() where T : new();

        ITypeProtobufObjectFactory Register<T>(int id) where T : new();
    }

}
