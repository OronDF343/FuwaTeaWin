using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace FuwaTea.Lib.Collections
{
    [Serializable]
    public sealed class ObservableHashSet<T> : ISet<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly HashSet<T> _hashSet;

        public ObservableHashSet()
        {
            _hashSet = new HashSet<T>();
        }

        public ObservableHashSet(IEqualityComparer<T> comparer)
        {
            _hashSet = new HashSet<T>(comparer);
        }

        public ObservableHashSet(IEnumerable<T> collection)
        {
            _hashSet = new HashSet<T>(collection);
        }

        public ObservableHashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer)
        {
            _hashSet = new HashSet<T>(collection, comparer);
        }

        public ObservableHashSet(ObservableHashSet<T> collection)
        {
            _hashSet = new HashSet<T>(collection);
            CollectionChanged = collection.CollectionChanged;
            PropertyChanged = collection.PropertyChanged;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _hashSet.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        public bool Add(T item)
        {
            CheckLock();
            if (!_hashSet.Add(item)) return false;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            OnPropertyChanged(nameof(Count));
            return true;
        }

        public void UnionWith(IEnumerable<T> other)
        {
            CheckLock();
            _hashSet.UnionWith(other);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            OnPropertyChanged(nameof(Count));
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            CheckLock();
            _hashSet.IntersectWith(other);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            OnPropertyChanged(nameof(Count));
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            CheckLock();
            _hashSet.ExceptWith(other);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            OnPropertyChanged(nameof(Count));
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            CheckLock();
            _hashSet.SymmetricExceptWith(other);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            OnPropertyChanged(nameof(Count));
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return _hashSet.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return _hashSet.IsSupersetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return _hashSet.IsProperSupersetOf(other);
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return _hashSet.IsProperSubsetOf(other);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            return _hashSet.Overlaps(other);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return _hashSet.SetEquals(other);
        }

        public void Clear()
        {
            CheckLock();
            _hashSet.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            OnPropertyChanged(nameof(Count));
        }

        public bool Contains(T item)
        {
            return _hashSet.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _hashSet.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Has the correct CollectionChanged event. Good for when the event handler is slow but the <see cref="ObservableHashSet{T}"/> contains few items.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool RemoveSlow(T item)
        {
            CheckLock();
            var index = _hashSet.ToList().IndexOf(item);
            if (index < 0) return false;
            _hashSet.Remove(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
            OnPropertyChanged(nameof(Count));
            return true;
        }

        public bool Remove(T item)
        {
            CheckLock();
            if (!_hashSet.Remove(item)) return false;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            OnPropertyChanged(nameof(Count));
            return true;
        }

        public int Count => _hashSet.Count;
        public bool IsReadOnly => false;
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        private readonly object _lock = new object();
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            Monitor.Enter(_lock);
            CollectionChanged?.Invoke(this, e);
            Monitor.Exit(_lock);
        }

        private void CheckLock()
        {
            if (Monitor.IsEntered(_lock))
                throw new InvalidOperationException("Can't make changes to this collection from within a CollectionChanged event handler!");
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
