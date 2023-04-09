using System;
using System.Collections.Generic;

namespace Webinex.DynamicRoles.EntityFrameworkCore
{
    internal static class LinqExtensions
    {
        public static TValue Get<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            TKey key,
            TValue defaultValue = default)
        {
            dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
            return dictionary.TryGetValue(key, out var value)
                ? value
                : defaultValue;
        }
    }
}