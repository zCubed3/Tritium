using System;
using System.Collections.Generic;
using System.Text;

namespace Tritium
{
    public static class ListExtensions
    {
        public static T PickRandom<T>(this List<T> list)
        {
            int index = Random.Shared.Next(0, list.Count);
            return list[index];
        }
    }
}
