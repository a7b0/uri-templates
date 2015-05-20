namespace Resta.UriTemplates
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

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

            if (collection != null)
            {
                return collection.Count;
            }

            var size = 0;

            foreach (var item in source)
            {
                size++;
            }

            return size;
        }
    }
}