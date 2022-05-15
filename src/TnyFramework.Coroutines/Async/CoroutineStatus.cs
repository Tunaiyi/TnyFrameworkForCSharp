using System;

namespace TnyFramework.Coroutines.Async
{

    [Flags]
    public enum CoroutineStatus
    {
        Shutdown = 0,

        Start = 1,

        Shutting = 2,
    }

}
