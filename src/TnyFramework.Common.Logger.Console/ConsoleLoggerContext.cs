// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace TnyFramework.Common.Logger.Console
{

    public class ConsoleLoggerContext : LoggerContext
    {
        private ConsoleLoggerContext()
        {
        }

        public static void Init()
        {
            LogFactory.DefaultFactory = CreateConsoleFactory();
        }

        private static ILoggerFactory CreateConsoleFactory()
        {
            return LoggerFactory.Create(builder => {
                builder.AddSimpleConsole(op => {
                    op.IncludeScopes = true;
                    op.ColorBehavior = LoggerColorBehavior.Disabled;
                    // op.ColorBehavior = LoggerColorBehavior.Disabled;
                    op.TimestampFormat = "[hh.mm.ss] ";
                });
            });
        }
    }

}
