using System;
using System.Collections;
using System.Collections.Generic;

namespace FuwaTea.Lib.Collections
{
    public class SortComparer<T> : IComparer<T>, IComparer
    {
        private readonly Comparison<T> _pred;
        public SortComparer(Comparison<T> pred)
        {
            _pred = pred;
        }

        public int Compare(T x, T y)
        {
            return _pred(x, y);
        }

        public int Compare(object x, object y)
        {
            return Compare((T)x, (T)y);
        }
    }
}
