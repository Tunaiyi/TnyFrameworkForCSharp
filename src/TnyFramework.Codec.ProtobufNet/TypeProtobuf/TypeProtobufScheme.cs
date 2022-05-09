using System;
using TnyFramework.Common.Exception;
using TnyFramework.Common.Invoke;

namespace TnyFramework.Codec.ProtobufNet.TypeProtobuf
{

    public class TypeProtobufScheme
    {
        public int Id { get; }

        public Type Type { get; }

        private readonly Func<object> creator;


        public TypeProtobufScheme(int id, Type type, Func<object> creator = null)
        {
            Id = id;
            Type = type;
            this.creator = creator;
            if (creator == null)
            {

                var constructor = type.GetConstructor(Type.EmptyTypes);
                if (constructor == null)
                {
                    throw new IllegalArgumentException($"{type} 未实现默认构造方法");
                }
                var invoker = FastFuncFactory.Invoker(constructor) ??
                              throw new ArgumentNullException($"{type} 创建 FastFuncFactory.Invoker(constructor) 失败");
                this.creator = () => invoker.Invoke(null);
            } else
            {
                this.creator = creator;
            }
        }



        public object Create()
        {
            return creator();
        }


        private bool Equals(TypeProtobufScheme other)
        {
            return Id == other.Id && Type == other.Type;
        }


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TypeProtobufScheme) obj);
        }


        public override int GetHashCode()
        {
            unchecked
            {
                return (Id * 397) ^ (Type != null ? Type.GetHashCode() : 0);
            }
        }
    }

}
