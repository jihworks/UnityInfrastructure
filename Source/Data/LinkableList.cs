// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using Jih.Unity.Infrastructure.Collections;
using System.Collections;
using System.Collections.Generic;

namespace Jih.Unity.Infrastructure.Data
{
    public class LinkableList<T> : BaseThreadCriticalObject, IList<T>, IReadOnlyList<T>, IReadOnlyLinkableList<T>
    {
        public LinkableEvent<ListChangeArgs<T>> OnChanged { get; } = new();

        public T this[int index]
        {
            get
            {
                CheckThread();
                return _innerList[index];
            }
            set
            {
                CheckThread();

                T oldValue = _innerList[index];
                if (_comparer.Equals(oldValue, value))
                {
                    return;
                }
                _innerList[index] = value;
                OnChanged.OnNext(new ListChangeArgs<T>(ListChangeType.Replace, index, value, oldValue));
            }
        }

        public int Count
        {
            get
            {
                CheckThread();
                return _innerList.Count;
            }
        }

        public int Capacity
        {
            get
            {
                CheckThread();
                return _innerList.Capacity;
            }
            set
            {
                CheckThread();
                _innerList.Capacity = value;
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                CheckThread();
                return ((ICollection<T>)_innerList).IsReadOnly;
            }
        }

        readonly EqualityComparer<T> _comparer = EqualityComparer<T>.Default;

        readonly List<T> _innerList;

        public LinkableList()
        {
            _innerList = new List<T>();
        }
        public LinkableList(int capacity)
        {
            _innerList = new List<T>(capacity);
        }

        public void Add(T item)
        {
            CheckThread();

            int index = _innerList.Count;
            _innerList.Add(item);
            OnChanged.OnNext(new ListChangeArgs<T>(ListChangeType.Add, index, item));
        }

        public void Insert(int index, T item)
        {
            CheckThread();

            _innerList.Insert(index, item);
            OnChanged.OnNext(new ListChangeArgs<T>(ListChangeType.Add, index, item));
        }

        public bool Remove(T item)
        {
            CheckThread();

            int index = _innerList.IndexOf(item);
            if (index < 0)
            {
                return false;
            }

            _innerList.RemoveAt(index);
            OnChanged.OnNext(new ListChangeArgs<T>(ListChangeType.Remove, index, item));
            return true;
        }

        public void RemoveAt(int index)
        {
            CheckThread();

            T value = _innerList[index];
            _innerList.RemoveAt(index);
            OnChanged.OnNext(new ListChangeArgs<T>(ListChangeType.Remove, index, value));
        }

        public void Clear()
        {
            CheckThread();

            _innerList.Clear();
            OnChanged.OnNext(new ListChangeArgs<T>(ListChangeType.Clear, -1, default!));
        }

        public int IndexOf(T item)
        {
            CheckThread();
            return _innerList.IndexOf(item);
        }

        public bool Contains(T item)
        {
            CheckThread();
            return _innerList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            CheckThread();
            _innerList.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            CheckThread();
            return _innerList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            CheckThread();
            return _innerList.GetEnumerator();
        }
    }
}
