#region License
//     This file is part of FuwaTeaWin.
// 
//     FuwaTeaWin is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     FuwaTeaWin is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with FuwaTeaWin.  If not, see <http://www.gnu.org/licenses/>.
#endregion

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
