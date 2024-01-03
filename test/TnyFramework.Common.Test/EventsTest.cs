// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Event;
using TnyFramework.Common.Logger;
using Xunit;
using Xunit.Abstractions;

namespace TnyFramework.Common.Test;

public class EventsTest
{
    private static ILogger _LOGGER = null!;

    public EventsTest(ITestOutputHelper output)
    {
        XUnitLoggerFactory.InitFactory(output);
        _LOGGER = LogFactory.Logger<EventsTest>();
    }

    public interface ITestListener
    {
        public void OnTest(TestObject source, MoveEventArgs args);
    }

    public readonly struct MoveEventArgs(int value, int x, int y)
    {
        public int Value { get; } = value;

        public int X { get; } = x;

        public int Y { get; } = y;
    }

    public class TestObject
    {
        private static readonly IListenEvent<ITestListener, TestObject, MoveEventArgs> EVENT =
            Events.Create<ITestListener, TestObject, MoveEventArgs>( listener => listener.OnTest);

        public IEventWatch<ITestListener> Event => EVENT;

        public void Handle(int value, int x, int y)
        {

            // 移动
            // 伤害
            EVENT.Notify(this, new MoveEventArgs(value, x, y));
        }
    }

    private class Print(string content) : ITestListener
    {
        public void OnTest(TestObject source, MoveEventArgs args)
        {
            _LOGGER.LogInformation("Print {source} OnTest {x} {y} : {content}", source, args.X, args.Y, content);
        }
    }

    public class Remove(params ITestListener[] listener) : ITestListener
    {
        private readonly List<ITestListener> listener = listener.ToList();

        public List<ITestListener> Listener => listener;

        public void OnTest(TestObject source, MoveEventArgs args)
        {
            if (args.Value != 100)
            {
                _LOGGER.LogInformation("{source} OnTest {value} No Remove ", source, args.Value);
                return;
            }
            foreach (var l in listener)
            {
                _LOGGER.LogInformation("{source} OnTest {value} Remove {listen}", source, args.Value, l);
                source.Event.Remove(l);
            }
        }
    }

    [Fact]
    public void Test1()
    {
        var obj = new TestObject();
        var p1 = new Print("p1");
        var p2 = new Print("p2");
        var p3 = new Print("p3");
        var p4 = new Print("p4");
        var p5 = new Print("p5");
        var p6 = new Print("p6");

        var remove = new Remove(p4, p5);
        remove.Listener.Add(remove);
        obj.Event.Add(p1);
        obj.Event.Add(p2);
        obj.Event.Add(p3);
        obj.Event.Add(remove);
        obj.Event.Add(p5);
        obj.Event.Add(p6);

        obj.Handle(-1, 100, 200);
        _LOGGER.LogInformation("========================================================================");
        obj.Handle(100, 100, 200);
        _LOGGER.LogInformation("========================================================================");
        obj.Handle(100, 100, 200);
    }
}
