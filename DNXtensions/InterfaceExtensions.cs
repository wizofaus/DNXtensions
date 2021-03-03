#nullable enable
using System.Collections.Generic;

namespace DNXtensions
{
    public static class InterfaceExtensions
    {
        public static void AddRange<T>(this IList<T> destination, IEnumerable<T> source)
        {
            if (destination is List<T> concreteList)
                concreteList.AddRange(source);
            else
                foreach (var element in source)
                    destination.Add(element);
        }
    }
}
