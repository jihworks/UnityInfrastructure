// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System.Collections;
using System.Collections.Generic;

namespace Jih.Unity.Infrastructure.Collections
{
    public class DirtyList<T> : IList<T>, IReadOnlyList<T>
    {
        bool _isDirty = false;
        public bool IsDirty
        {
            get => _isDirty;
            set
            {
                if (_isDirty == value)
                {
                    return;
                }
                _isDirty = value;
                OnIsDirtyChanged();
            }
        }

        public T this[int index]
        {
            get => _innerList[index];
            set
            {
                if (EqualityComparer<T>.Default.Equals(_innerList[index], value))
                {
                    return;
                }
                _innerList[index] = value;
                IsDirty = true;
            }
        }

        public int Count => _innerList.Count;

        public int Capacity { get => _innerList.Capacity; set => _innerList.Capacity = value; }

        bool ICollection<T>.IsReadOnly => ((ICollection<T>)_innerList).IsReadOnly;

        readonly List<T> _innerList;

        readonly IDirtyListOwner<T> _owner;

        public DirtyList(IDirtyListOwner<T> owner, int capacity)
        {
            _owner = owner;
            _innerList = new List<T>(capacity);

            _owner.OnConstructed(new Proxy(this));
        }

        public void Add(T item)
        {
            _innerList.Add(item);
            IsDirty = true;
        }

        public void AddRange(IEnumerable<T> items)
        {
            int count = _innerList.Count;
            _innerList.AddRange(items);
            IsDirty |= count != _innerList.Count;
        }

        public void Insert(int index, T item)
        {
            _innerList.Insert(index, item);
            IsDirty = true;
        }

        public bool Remove(T item)
        {
            bool result = _innerList.Remove(item);
            IsDirty |= result;
            return result;
        }

        public void RemoveAt(int index)
        {
            _innerList.RemoveAt(index);
            IsDirty = true;
        }

        public void Clear()
        {
            bool anyExisted = _innerList.Count > 0;
            _innerList.Clear();
            IsDirty |= anyExisted;
        }

        public bool Contains(T item)
        {
            return _innerList.Contains(item);
        }

        public int IndexOf(T item)
        {
            return _innerList.IndexOf(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _innerList.CopyTo(array, arrayIndex);
        }
        
        public List<T>.Enumerator GetEnumerator()
        {
            return _innerList.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _innerList.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _innerList.GetEnumerator();
        }

        protected virtual void OnIsDirtyChanged()
        {
            _owner.OnIsDirtyChanged(new Proxy(this));
        }

        public readonly struct Proxy
        {
            public readonly DirtyList<T> Target;

            public readonly bool IsDirty { get => Target.IsDirty; set => Target.IsDirty = value; }

            public readonly List<T> InnerList { get => Target._innerList; }

            internal Proxy(DirtyList<T> target)
            {
                Target = target;
            }
        }
    }

    public interface IDirtyListOwner<T>
    {
        void OnConstructed(DirtyList<T>.Proxy proxy);

        void OnIsDirtyChanged(DirtyList<T>.Proxy proxy);
    }
}
