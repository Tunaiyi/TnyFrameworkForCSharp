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
using TnyFramework.Common.Event.Notices;
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
        public void OnTest(TestObject source, int value);
    }

    public class TestObject
    {
        private readonly IListenEvent<ITestListener, TestObject, int> @event =
            Events.Create<ITestListener, TestObject, int>((l, p) => l.OnTest(p.source, p.arg1));

        public IEvent<ITestListener> Event => @event;

        public void Handle(int value)
        {
            @event.Notify(this, value);
        }
    }

    private class Print(string content) : ITestListener
    {
        public void OnTest(TestObject source, int value)
        {
            _LOGGER.LogInformation("Print {source} OnTest {value} : {content}", source, value, content);
        }
    }

    public class Remove(params ITestListener[] listener) : ITestListener
    {
        private readonly List<ITestListener> listener = listener.ToList();

        public List<ITestListener> Listener => listener;

        public void OnTest(TestObject source, int value)
        {
            if (value != 100)
            {
                _LOGGER.LogInformation("{source} OnTest {value} No Remove ", source, value);
                return;
            }
            foreach (var l in listener)
            {
                _LOGGER.LogInformation("{source} OnTest {value} Remove {listen}", source, value, l);
                source.Event.RemoveListener(l);
            }
        }
    }

    [Fact]
    public void Test1()
    {
        TestObject obj = new TestObject();
        var p1 = new Print("p1");
        var p2 = new Print("p2");
        var p3 = new Print("p3");
        var p4 = new Print("p4");
        var p5 = new Print("p5");
        var p6 = new Print("p6");
        var remove = new Remove(p4, p5);
        remove.Listener.Add(remove);
        obj.Event.AddListener(p1);
        obj.Event.AddListener(p2);
        obj.Event.AddListener(p3);
        obj.Event.AddListener(remove);
        obj.Event.AddListener(p5);
        obj.Event.AddListener(p6);

        obj.Handle(-1);
        _LOGGER.LogInformation("========================================================================");
        obj.Handle(100);
        _LOGGER.LogInformation("========================================================================");
        obj.Handle(100);
    }
}
