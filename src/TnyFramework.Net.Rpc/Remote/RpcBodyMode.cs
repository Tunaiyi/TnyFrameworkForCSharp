// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using TnyFramework.Common.Enum;
using TnyFramework.Common.Extensions;
using TnyFramework.Common.Result;
using TnyFramework.Common.Scanner.Assemblies;
using TnyFramework.Net.Attributes;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.Rpc.Remote
{

    public class RpcBodyMode : BaseEnum<RpcBodyMode>
    {
        public static readonly RpcBodyMode VOID = Of(0, typeof(void));

        public static readonly RpcBodyMode MESSAGE = Of(1, typeof(IMessage));

        public static readonly RpcBodyMode MESSAGE_HEAD = Of(2, typeof(IMessageHead));

        public static readonly RpcBodyMode RESULT = Of(3, typeof(IRpcResult));

        public static readonly RpcBodyMode RESULT_CODE_ID = Of<RpcCodeAttribute>(4, typeof(int));

        public static readonly RpcBodyMode RESULT_CODE = Of(5, typeof(IResultCode));

        public static readonly RpcBodyMode BODY = Of<RpcBodyAttribute>(6);

        public Type? BodyAttribute { get; private set; }

        public IList<Type> BodyTypes { get; private set; } = null!;

        public static RpcBodyMode TypeOf(MethodInfo method, Type returnClass)
        {
            foreach (var rpcBodyType in GetValues())
            {
                if (rpcBodyType.IsNeedAttribute())
                {
                    var attribute = method.GetCustomAttribute(rpcBodyType.BodyAttribute!);
                    if (attribute == null)
                    {
                        continue;
                    }
                }
                if (rpcBodyType.IsCanReturn(returnClass))
                {
                    return rpcBodyType;
                }
            }
            return BODY;
        }

        private static RpcBodyMode Of(int id, params Type[] bodyTypes)
        {
            return E(id, new RpcBodyMode {
                BodyAttribute = null!,
                BodyTypes = bodyTypes.ToImmutableList()
            });
        }

        private static RpcBodyMode Of<T>(int id, params Type[] bodyTypes) where T : Attribute
        {
            return E(id, new RpcBodyMode {
                BodyAttribute = typeof(T),
                BodyTypes = bodyTypes.ToImmutableList()
            });
        }

        public bool IsNeedAttribute()
        {
            return BodyAttribute != null;
        }

        public bool IsCanReturn(Type type)
        {
            if (BodyTypes.IsNullOrEmpty())
            {
                return true;
            }
            foreach (var bodyType in BodyTypes)
            {
                if (bodyType.IsGenericType)
                {
                    if (type.IsAssignableFromGeneric(bodyType))
                        return true;
                } else
                {
                    if (bodyType.IsAssignableFrom(type))
                        return true;
                }
            }
            return false;
        }

        public static implicit operator int(RpcBodyMode type) => type.Id;

        public static explicit operator RpcBodyMode(int type) => ForId(type);
    }

}
