// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Jih.Unity.Infrastructure.Collections
{
    public class RefList<T> : IList<T>, IReadOnlyList<T>, IList
    {
        public int Count => _count;

        public int Capacity
        {
            get => _items.Length;
            set
            {
                if (value < _count)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                
                if (value == _items.Length)
                {
                    return;
                }

                if (value <= 0)
                {
                    _items = Array.Empty<T>();
                    return;
                }

                T[] newItems = new T[value];
                if (_count > 0)
                {
                    Array.Copy(_items, 0, newItems, 0, _count);
                }
                _items = newItems;
            }
        }

        int _version;
        object? _syncRoot;

        T[] _items;
        int _count;

        public RefList()
        {
            _items = Array.Empty<T>();
        }

        public RefList(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }
            _items = capacity == 0 ? Array.Empty<T>() : new T[capacity];
        }

        public ref T this[int index]
        {
            get
            {
                if ((uint)index >= (uint)_count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                return ref _items[index];
            }
        }

        public void Add(T item)
        {
            if (_count >= _items.Length)
            {
                EnsureCapacity(_count + 1);
            }
            _items[_count++] = item;
            _version++;
        }

        public void AddRange(IEnumerable<T> collection)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (collection is ICollection<T> c)
            {
                int count = c.Count;
                if (count > 0)
                {
                    EnsureCapacity(_count + count);
                    c.CopyTo(_items, _count);
                    _count += count;
                    _version++;
                }
            }
            else
            {
                using IEnumerator<T> en = collection.GetEnumerator();
                while (en.MoveNext())
                {
                    Add(en.Current);
                }
            }
        }

        public void Insert(int index, T item)
        {
            if ((uint)index > (uint)_count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (_count >= _items.Length)
            {
                EnsureCapacity(_count + 1);
            }
            if (index < _count)
            {
                Array.Copy(_items, index, _items, index + 1, _count - index);
            }
            _items[index] = item;
            _count++;
            _version++;
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            if ((uint)index >= (uint)_count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            _count--;
            if (index < _count)
            {
                Array.Copy(_items, index + 1, _items, index, _count - index);
            }
            _items[_count] = default!;
            _version++;
        }

        public int RemoveAll(Predicate<T> match)
        {
            if (match is null)
            {
                throw new ArgumentNullException(nameof(match));
            }

            int freeIndex = 0;

            while (freeIndex < _count && !match(_items[freeIndex]))
            {
                freeIndex++;
            }

            if (_count <= freeIndex)
            {
                return 0;
            }

            int current = freeIndex + 1;
            while (current < _count)
            {
                if (!match(_items[current]))
                {
                    _items[freeIndex++] = _items[current];
                }
                current++;
            }

            Array.Clear(_items, freeIndex, _count - freeIndex);

            int result = _count - freeIndex;
            _count = freeIndex;
            _version++;

            return result;
        }

        public void Clear()
        {
            if (_count > 0)
            {
                Array.Clear(_items, 0, _count);
                _count = 0;
            }
            _version++;
        }

        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public int IndexOf(T item)
        {
            return Array.IndexOf(_items, item, 0, _count);
        }

        public ref T Find(Predicate<T> match)
        {
            if (match is null)
            {
                throw new ArgumentNullException(nameof(match));
            }

            for (int i = 0; i < _count; i++)
            {
                if (match(_items[i]))
                {
                    return ref _items[i];
                }
            }

            throw new KeyNotFoundException("Cannot find matched element.");
        }

        [return: MaybeNull]
        public T FindOrDefault(Predicate<T> match)
        {
            if (match is null)
            {
                throw new ArgumentNullException(nameof(match));
            }

            for (int i = 0; i < _count; i++)
            {
                if (match(_items[i]))
                {
                    return _items[i];
                }
            }

            return default!;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Array.Copy(_items, 0, array, arrayIndex, _count);
        }

        public Span<T> AsSpan()
        {
            return new Span<T>(_items, 0, _count);
        }
        public ReadOnlySpan<T> AsReadOnlySpan()
        {
            return new ReadOnlySpan<T>(_items, 0, _count);
        }

        public void EnsureCapacity(int min)
        {
            if (_items.Length >= min)
            {
                return;
            }

            int newCapacity = _items.Length <= 0 ? DefaultCapacity : _items.Length * 2;
            if ((uint)newCapacity > int.MaxValue)
            {
                newCapacity = int.MaxValue;
            }
            if (newCapacity < min)
            {
                newCapacity = min;
            }
            Capacity = newCapacity;
        }

        public void TrimExcess()
        {
            if (Capacity == Count)
            {
                return;
            }
            Capacity = Count;
        }

        public void Sort()
        {
            Sort(Comparer<T>.Default);
        }
        public void Sort(IComparer<T> comparer)
        {
            Sort(0, Count, comparer);
        }
        /// <remarks>
        /// <b>NOT</b> garbage-free.
        /// </remarks>
        public void Sort(Comparison<T> comparison)
        {
            Sort(0, Count, Comparer<T>.Create(comparison));
        }
        public void Sort(int index, int count, IComparer<T> comparer)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
            if (_count - index < count)
            {
                throw new ArgumentException("Given index or count is out of range.");
            }

            if (count > 1)
            {
                Array.Sort(_items, index, count, comparer);
            }
            _version++;
        }

        public void Reverse()
        {
            Reverse(0, Count);
        }
        public void Reverse(int index, int count)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
            if (_count - index < count)
            {
                throw new ArgumentException("Given index or count is out of range.");
            }

            if (count > 1)
            {
                Array.Reverse(_items, index, count);
            }
            _version++;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(this);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        T IList<T>.this[int index]
        {
            get => this[index];
            set => this[index] = value;
        }

        bool ICollection<T>.IsReadOnly => false;

        T IReadOnlyList<T>.this[int index] => this[index];

        bool IList.IsReadOnly => false;
        bool IList.IsFixedSize => false;
        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot
        {
            get
            {
                if (_syncRoot is null)
                {
                    Interlocked.CompareExchange(ref _syncRoot, new object(), null);
                }
                return _syncRoot;
            }
        }

        object IList.this[int index]
        {
            get => this[index]!;
            set
            {
                try
                {
                    this[index] = (T)value;
                }
                catch (InvalidCastException)
                {
                    throw new ArgumentException(nameof(value));
                }
            }
        }

        int IList.Add(object? value)
        {
            try
            {
                Add((T)value!);
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException(nameof(value));
            }
            return Count - 1;
        }

        void IList.Insert(int index, object? value)
        {
            try
            {
                Insert(index, (T)value!);
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException(nameof(value));
            }
        }

        bool IList.Contains(object? value)
        {
            if (IsCompatibleObject(value))
            {
                return Contains((T)value!);
            }
            return false;
        }

        int IList.IndexOf(object? value)
        {
            if (IsCompatibleObject(value))
            {
                return IndexOf((T)value!);
            }
            return -1;
        }

        void IList.Remove(object? value)
        {
            if (IsCompatibleObject(value))
            {
                Remove((T)value!);
            }
        }

        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            if (array is not null && array.Rank != 1)
            {
                throw new ArgumentException(nameof(array));
            }
            Array.Copy(_items, 0, array, arrayIndex, _count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool IsCompatibleObject(object? value)
        {
            return (value is T) || (value is null && default(T) is null);
        }

        public struct Enumerator : IEnumerator<T>, IEnumerator
        {
            public readonly T Current => _current;

            readonly object IEnumerator.Current
            {
                get
                {
                    if (_index <= 0 || _list._count < _index)
                    {
                        throw new InvalidOperationException();
                    }
                    return Current!;
                }
            }

            readonly RefList<T> _list;
            readonly int _version;

            int _index;
            T _current;

            internal Enumerator(RefList<T> list)
            {
                _list = list;
                _version = list._version;

                _index = 0;
                _current = default!;
            }

            public bool MoveNext()
            {
                RefList<T> localList = _list;
                if (_version == localList._version && ((uint)_index < (uint)localList._count))
                {
                    _current = localList._items[_index];
                    _index++;
                    return true;
                }
                return MoveNextRare();
            }

            bool MoveNextRare()
            {
                if (_version != _list._version)
                {
                    throw new InvalidOperationException("Cannot change collection when enumerating.");
                }
                _index = _list._count + 1;
                _current = default!;
                return false;
            }

            public void Reset()
            {
                if (_version != _list._version)
                {
                    throw new InvalidOperationException("Cannot change collection when enumerating.");
                }
                _index = 0;
                _current = default!;
            }

            public readonly void Dispose()
            {
            }
        }

        const int DefaultCapacity = 4;
    }
}
