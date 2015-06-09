using System;
using System.Collections.Generic;
using System.Linq;

namespace FuwaTea.Lib.Collections
{
    public static class ExtensionMethods
    {
        public static OneToManyDictionary<TKey, TValue> ToOneToManyDictionary<TKey, TValue, TCollection>(
            this IEnumerable<KeyValuePair<TKey, TCollection>> dictionary) where TCollection : IEnumerable<TValue>
        {
            return new OneToManyDictionary<TKey, TValue>(dictionary.ToDictionary(pair => pair.Key,
                                                                                 pair => new HashSet<TValue>(pair.Value)));
        }

        public static OneToManyDictionary<TKey, TValue> ToOneToManyDictionary<TKey, TValue, TCollection>(
            this IEnumerable<KeyValuePair<TKey, TCollection>> dictionary,
            Func<TCollection, IEnumerable<TValue>> collectionAdapter)
        {
            return new OneToManyDictionary<TKey, TValue>(dictionary.ToDictionary(pair => pair.Key,
                                                                                 pair => new HashSet<TValue>(collectionAdapter(pair.Value))));
        }

        /// <summary>
        /// Converts a list of relations into a <see cref="OneToManyDictionary{TKey,TValue}"/>
        /// </summary>
        /// <remarks>Duplicate Item1 values are allowed, as the collection is "flattened"</remarks>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="keyValueCollection"></param>
        /// <returns></returns>
        public static OneToManyDictionary<TKey, TValue> ToOneToManyDictionary<TKey, TValue>(
            this ICollection<Tuple<TKey, TValue>> keyValueCollection)
        {
            return new OneToManyDictionary<TKey, TValue>(
                (from t in keyValueCollection.Select(tu => tu.Item1).Distinct()
                 select new KeyValuePair<TKey, IEnumerable<TValue>>(t, keyValueCollection.Where(tu => tu.Item1.Equals(t))
                                                                                         .Select(tu => tu.Item2)))
                .ToDictionary(pair => pair.Key, pair => new HashSet<TValue>(pair.Value)));
        }

        /// <summary>
        /// Converts a list of relations into a <see cref="OneToManyDictionary{TKey,TValue}"/>
        /// </summary>
        /// <remarks>Duplicate Item1 values are allowed, as the collection is "flattened"</remarks>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="keyValueCollection"></param>
        /// <returns></returns>
        public static OneToManyDictionary<TKey, TValue> ToOneToManyDictionary<TKey, TValue>(
            this ICollection<Tuple<TKey, IEnumerable<TValue>>> keyValueCollection)
        {
            return new OneToManyDictionary<TKey, TValue>(
                (from t in keyValueCollection.Select(tu => tu.Item1).Distinct()
                 select new KeyValuePair<TKey, IEnumerable<TValue>>(t, keyValueCollection.Where(tu => tu.Item1.Equals(t))
                                                                                         .SelectMany(tu => tu.Item2)))
                .ToDictionary(pair => pair.Key, pair => new HashSet<TValue>(pair.Value)));
        }
    }
}
