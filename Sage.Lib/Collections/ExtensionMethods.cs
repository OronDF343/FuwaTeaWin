﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Sage.Lib.Collections
{
    public static class ExtensionMethods
    {
        public static void SortBySequence<T, TId>(this List<T> source, IList<TId> order,
                                                  Func<T, TId> idSelector)
        {
            var lookup = source.ToDictionary<T, T, int>(t => t, t => order.IndexOf(idSelector(t)).WithCondition(i => i < 0, int.MaxValue));
            source.Sort((t1, t2) => lookup[t1].CompareTo(lookup[t2]));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T WithCondition<T>(this T value, Func<T, bool> condition, T valueIfTrue)
        {
            return condition(value) ? valueIfTrue : value;
        }

        public static void AddOrSet<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (dict.ContainsKey(key)) dict[key] = value;
            else dict.Add(key, value);
        }
    }
}
