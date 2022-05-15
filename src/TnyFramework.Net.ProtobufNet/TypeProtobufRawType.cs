using System;

namespace TnyFramework.Net.ProtobufNet
{

    [Flags]
    internal enum ProtobufRawType
    {
        /// <summary>
        /// null
        /// </summary>
        Null = 0,

        /// <summary>
        /// byte
        /// </summary>
        Byte = 1,

        /// <summary>
        /// short
        /// </summary>
        Short = 2,

        /// <summary>
        /// int
        /// </summary>
        Int = 3,

        /// <summary>
        /// long
        /// </summary>
        Long = 4,

        /// <summary>
        /// float
        /// </summary>
        Float = 5,

        /// <summary>
        /// double
        /// </summary>
        Double = 6,

        /// <summary>
        /// bool
        /// </summary>
        Bool = 7,

        /// <summary>
        /// string
        /// </summary>
        String = 8,

        /// <summary>
        /// complex
        /// </summary>
        Complex = 9,
    }

}
