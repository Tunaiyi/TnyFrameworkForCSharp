using System;

namespace TnyFramework.Common.Exceptions
{

    public static class Assets
    {
        public static void CheckArguments(bool predicate, string message, params object[] paramList)
    {
        if (!predicate)
        {
            throw new IllegalArgumentException(string.Format(message, paramList));
        }
    }

        public static void CheckArguments(Func<bool> predicate, string message, params object[] paramList)
    {
        if (!predicate())
        {
            throw new IllegalArgumentException(string.Format(message, paramList));
        }
    }

        public static void CheckNotNull(object value, string message, params object[] paramList)
    {
        if (value == null)
        {
            throw new NullReferenceException(string.Format(message, paramList));
        }
    }
    }

}