using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Resta.UriTemplates
{
    internal static class Extensions
    {
        public static int Count<T>(this IEnumerable<T> source)
        {
            var typedCollection = source as ICollection<string>;

            if (typedCollection != null)
            {
                return typedCollection.Count;
            }

            var collection = source as ICollection;

            return collection?.Count ?? Enumerable.Count(source);
        }
    }
}