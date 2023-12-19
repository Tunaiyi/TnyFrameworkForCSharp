using System;
using System.Threading;
using System.Threading.Tasks;

namespace TnyFramework.Common.Extensions
{

    public static class SafeTask
    {
        public static async Task<bool> Delay(TimeSpan delay, CancellationToken cancellationToken)
    {

        try
        {
            await Task.Delay(delay, cancellationToken);
            return true;
        } catch (TaskCanceledException)
        {
            return false;
        }
    }

        public static async Task<bool> Delay(int millisecondsDelay, CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(millisecondsDelay, cancellationToken);
            return true;
        } catch (TaskCanceledException)
        {
            return false;
        }

    }
    }

}