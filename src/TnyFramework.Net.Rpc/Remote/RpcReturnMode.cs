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
using System.Threading.Tasks;
using TnyFramework.Common.Assemblies;
using TnyFramework.Common.Enum;
using TnyFramework.Net.Command;
using TnyFramework.Net.Message;
using TnyFramework.Net.Rpc.Attributes;

namespace TnyFramework.Net.Rpc.Remote
{

    public class RpcReturnMode : BaseEnum<RpcReturnMode>
    {
        private static IEnumerable<T> Params<T>(params T[] values)
        {
            return values;
        }

        private static IEnumerable<Type> EmptyType => ImmutableList<Type>.Empty;

        private static Func<MethodInfo, Type> MethodReturnType { get; } =
            m => m.ReturnType;

        private static Func<MethodInfo, Type> TaskReturnType { get; } =
            m => m.ReturnType == typeof(Task) ? typeof(void) : m.ReturnType.GenericTypeArguments[0];

        private static Func<MethodInfo, Type> ResultReturnType { get; } =
            m => m.ReturnType == typeof(IRpcResult) ? typeof(void) : m.ReturnType.GenericTypeArguments[0];

        /// <summary>
        /// Task 类型
        /// </summary>
        public static readonly RpcReturnMode TASK = Of(1,
            Params(MessageMode.Request),
            Params(typeof(Task), typeof(Task<>)),
            TaskReturnType,
            RpcInvokeMode.Async);

        /// <summary>
        /// 结果
        /// </summary>
        public static readonly RpcReturnMode RESULT = Of(2,
            Params(MessageMode.Request),
            Params(typeof(IRpcResult), typeof(IRpcResult<>)),
            ResultReturnType,
            RpcInvokeMode.Sync);

        /// <summary>
        /// void 对象
        /// </summary>
        /// <returns></returns>
        public static readonly RpcReturnMode VOID = Of(3,
            Params(MessageMode.Request, MessageMode.Push),
            Params(typeof(void)),
            MethodReturnType,
            RpcInvokeMode.Async, RpcInvokeMode.Sync);

        /// <summary>
        /// 普通对象
        /// </summary>
        public static readonly RpcReturnMode OBJECT = Of(4,
            Params(MessageMode.Request),
            EmptyType,
            MethodReturnType,
            RpcInvokeMode.Sync);

        public RpcReturnMode()
        {
        }

        public RpcInvokeMode DefaultInvocation { get; private set; }

        public IList<RpcInvokeMode> Invocations { get; private set; }

        public IList<MessageMode> Modes { get; private set; }

        public IList<Type> ReturnTypes { get; private set; }

        public Func<MethodInfo, Type> BodyTypeFinder { get; private set; }

        private static RpcReturnMode Of(int id,
            IEnumerable<MessageMode> modes,
            IEnumerable<Type> returnClasses,
            Func<MethodInfo, Type> bodyTypeFinder,
            RpcInvokeMode defaultInvocation,
            params RpcInvokeMode[] invocations)
        {
            var invokeModes = new List<RpcInvokeMode> {defaultInvocation};
            invokeModes.AddRange(invocations);
            return E(id, new RpcReturnMode {
                DefaultInvocation = defaultInvocation,
                BodyTypeFinder = bodyTypeFinder,
                Invocations = invokeModes.ToImmutableList(),
                Modes = modes.ToImmutableList(),
                ReturnTypes = returnClasses.ToImmutableList()
            });
        }

        public static RpcReturnMode TypeOf(Type returnClass)
        {
            foreach (var mode in GetValues())
            {
                if (mode.IsCanReturn(returnClass))
                {
                    return mode;
                }
            }
            return OBJECT;
        }

        public bool IsCanInvokeBy(MessageMode mode)
        {
            return Modes.Contains(mode);
        }

        public RpcInvokeMode CheckInvocation(RpcInvokeMode invokeMode)
        {
            return invokeMode == RpcInvokeMode.Default ? DefaultInvocation : invokeMode;
        }

        public bool IsAsync()
        {
            return Invocations.Contains(RpcInvokeMode.Async);
        }

        private bool IsCanReturn(Type type)
        {
            foreach (var returnType in ReturnTypes)
            {
                if (returnType.IsGenericType)
                {
                    if (type.IsAssignableFromGeneric(returnType))
                    {
                        return true;
                    }
                } else
                {
                    if (returnType.IsAssignableFrom(type))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public Type FindBodyType(MethodInfo method)
        {
            return BodyTypeFinder(method);
        }
    }

}
