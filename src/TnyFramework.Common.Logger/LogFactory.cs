// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using static System.Threading.Volatile;

namespace TnyFramework.Common.Logger
{

    public static class LogFactory
    {
        private static ILoggerFactory? _DEFAULT_FACTORY;

        public static ILoggerFactory DefaultFactory {
            get {
                var current = Read(ref _DEFAULT_FACTORY);
                if (current != null)
                    return current;
                current = CreateNoopFactory();
                var old = Interlocked.CompareExchange(ref _DEFAULT_FACTORY, current, null);
                return old ?? current;
            }
            set => Write(ref _DEFAULT_FACTORY, value);
        }

        private static ILoggerFactory CreateNoopFactory()
        {
            return LoggerFactory.Create(builder => { builder.AddProvider(new NoopLoggerProvider()); });
        }

        /// <summary>
        ///     Creates a new logger instance with the name of the specified type.
        /// </summary>
        /// <typeparam name="T">type where logger is used</typeparam>
        /// <returns>logger instance</returns>
        public static ILogger Logger<T>() => Logger(typeof(T));

        /// <summary>
        ///     Creates a new logger instance with the name of the specified type.
        /// </summary>
        /// <param name="type">type where logger is used</param>
        /// <returns>logger instance</returns>
        public static ILogger Logger(Type type) => Logger(type.FullName ?? "Default");

        /// <summary>
        ///     Creates a new logger instance with the specified name.
        /// </summary>
        /// <param name="name">logger name</param>
        /// <returns>logger instance</returns>
        public static ILogger Logger(string name) => DefaultFactory.CreateLogger(name);
    }

}
