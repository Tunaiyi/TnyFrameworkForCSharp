namespace TnyFramework.Common.Extensions
{

    public static class StringExtensions
    {
        public static bool IsBlank(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool IsNotBlank(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }
    }

}
