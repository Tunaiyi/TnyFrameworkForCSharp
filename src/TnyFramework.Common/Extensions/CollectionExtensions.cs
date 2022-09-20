using System.Collections.Generic;

namespace TnyFramework.Common.Extensions
{

    public static class CollectionExtensions
    {
        public static bool IsEmpty<T>(this ICollection<T> collection)
        {
            return collection == null || collection.Count == 0;
        }

        public static void AddRang<T>(this ICollection<T> collection, IEnumerable<T> valuse)
        {
            foreach (var value in valuse)
            {
                collection.Add(value);
            }
        }
    }

}
