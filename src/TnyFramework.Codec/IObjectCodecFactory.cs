using System;
using System.Collections.Generic;

namespace TnyFramework.Codec
{

    /// <summary>
    /// 对象编解码器工厂
    /// </summary>
    public interface IObjectCodecFactory
    {
        IReadOnlyList<IMimeType> MediaTypes { get; }

        IObjectCodec<T> CreateCodec<T>();

        IObjectCodec CreateCodec(Type type);
    }

}
