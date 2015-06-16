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
