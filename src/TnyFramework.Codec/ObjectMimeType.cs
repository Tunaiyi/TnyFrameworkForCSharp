using System;

namespace TnyFramework.Codec
{

    public class ObjectMimeType
    {
        public static ObjectMimeType<T> Of<T>()
        {
            return new ObjectMimeType<T>(null);
        }

        public static ObjectMimeType<T> Of<T>(IMimeType mimeType)
        {
            return new ObjectMimeType<T>(mimeType);
        }

        public IMimeType MimeType { get; }

        public Type ObjectType { get; }

        public ObjectMimeType(IMimeType mimeType, Type objectType)
        {
            MimeType = mimeType;
            ObjectType = objectType;
        }
    }

    public class ObjectMimeType<TObject> : ObjectMimeType
    {
        public bool HasMimeType() => MimeType != null;

        internal ObjectMimeType(IMimeType mimeType) : base(mimeType, typeof(TObject))
        {
        }

        /// <summary>
        /// 以当前MineType创建一个 type 的 ObjectMineType
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>返回新的MineType</returns>
        public ObjectMimeType<T> With<T>()
        {
            return new ObjectMimeType<T>(MimeType);
        }
    }

}
