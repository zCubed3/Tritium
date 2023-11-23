using System;
using System.Collections.Generic;
using System.Text;

namespace Tritium
{
    public static class IEnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action) 
        {
            foreach (T iterator in enumerable)
                action(iterator);
        }
    }
}
