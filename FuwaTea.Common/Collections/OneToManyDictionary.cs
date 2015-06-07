using System;
using System.Collections.Generic;
using System.Linq;

namespace FuwaTea.Common.Collections
{
    public class OneToManyDictionary<TKey, TValue> : Dictionary<TKey, HashSet<TValue>>
    {
        public OneToManyDictionary() { }
        public OneToManyDictionary(IDictionary<TKey, HashSet<TValue>> dictionary)
            : base(dictionary) { }
        public OneToManyDictionary(IDictionary<TKey, HashSet<TValue>> dictionary, IEqualityComparer<TKey> comparer)
            : base(dictionary, comparer) { }

        public void Update(TKey key, TValue value)
        {
            if (!ContainsKey(key)) Add(key, new HashSet<TValue> { value });
            else this[key].Add(value);
        }

        public void Update(TKey key, params TValue[] values)
        {
            if (!ContainsKey(key)) Add(key, new HashSet<TValue>(values));
            else this[key].UnionWith(values);
        }

        public void Update(TKey key, IEnumerable<TValue> values)
        {
            if (!ContainsKey(key)) Add(key, new HashSet<TValue>(values));
            else this[key].UnionWith(values);
        }

        public IEnumerable<TKey> FindKeys(TValue value)
        {
            return Keys.Where(key => this[key].Contains(value));
        }

        public IEnumerable<TKey> FindKeys(Func<TValue, bool> valueSelector)
        {
            return Keys.Where(key => this[key].Any(valueSelector));
        }
    }
}
