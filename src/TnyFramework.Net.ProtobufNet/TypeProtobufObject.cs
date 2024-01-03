// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using TnyFramework.Common.Exceptions;
using TnyFramework.Common.Extensions;
using TnyFramework.Common.Reflection.FastInvoke.FuncInvoke;

namespace TnyFramework.Net.ProtobufNet
{

    internal class TypeProtobufObject
    {
        public int Id { get; }

        public Type Type { get; }

        private readonly Func<object>? creator;

        public TypeProtobufObject(int id, Type type, Func<object>? creator = null)
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
                this.creator = () => invoker.Invoke(null!);
            } else
            {
                this.creator = creator;
            }
        }

        public object? Create()
        {
            return creator?.Invoke();
        }

        private bool Equals(TypeProtobufObject other)
        {
            return Id == other.Id && Type == other.Type;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TypeProtobufObject) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Id * 397) ^ (Type.IsNotNull() ? Type.GetHashCode() : 0);
            }
        }
    }

}
