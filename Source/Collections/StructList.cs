// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Jih.Unity.Infrastructure.Collections
{
    public struct StructList4<T>
    {
        public int Count { get; private set; }

        public readonly int Capacity => _innerArray.Length;

        public T this[int index]
        {
            readonly get
            {
                CheckIndex(index);
                return _innerArray[index];
            }
            set
            {
                CheckIndex(index);
                _innerArray[index] = value;
            }
        }

        StructArray4<T> _innerArray;

        public void Insert(int index, T item)
        {
            if (index < 0 || Count < index)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            if (Count >= Capacity)
            {
                throw new InvalidOperationException($"Cannot {nameof(Insert)}. Because capacity reached max.");
            }

            for (int i = Count - 1; i >= index; i--)
            {
                _innerArray[i + 1] = _innerArray[i];
            }
            _innerArray[index] = item;
            Count++;
        }

        public void Add(T item)
        {
            if (Count >= Capacity)
            {
                throw new InvalidOperationException($"Cannot {nameof(Add)}. Because capacity reached max.");
            }
            _innerArray[Count++] = item;
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index < 0)
            {
                return false;
            }

            RemoveAt(index);
            return true;
        }

        public void RemoveAt(int index)
        {
            CheckIndex(index);

            for (int i = index; i < Count - 1; i++)
            {
                _innerArray[i] = _innerArray[i + 1];
            }
            _innerArray[--Count] = default!;
        }

        public void Clear()
        {
            _innerArray = default;
            Count = 0;
        }

        public readonly bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public readonly int IndexOf(T item)
        {
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;

            for (int i = 0; i < Count; i++)
            {
                if (comparer.Equals(_innerArray[i], item))
                {
                    return i;
                }
            }
            return -1;
        }

        public readonly void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            }
            if (array.Length - arrayIndex < Count)
            {
                throw new ArgumentException("Destination array is too short.");
            }

            for (int i = 0; i < Count; i++)
            {
                array[i + arrayIndex] = _innerArray[i];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly void CheckIndex(int index)
        {
            if (index < 0 || Count <= index)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }
    }

    public struct StructList8<T>
    {
        public int Count { get; private set; }

        public readonly int Capacity => _innerArray.Length;

        public T this[int index]
        {
            readonly get
            {
                CheckIndex(index);
                return _innerArray[index];
            }
            set
            {
                CheckIndex(index);
                _innerArray[index] = value;
            }
        }

        StructArray8<T> _innerArray;

        public void Insert(int index, T item)
        {
            if (index < 0 || Count < index)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            if (Count >= Capacity)
            {
                throw new InvalidOperationException($"Cannot {nameof(Insert)}. Because capacity reached max.");
            }

            for (int i = Count - 1; i >= index; i--)
            {
                _innerArray[i + 1] = _innerArray[i];
            }
            _innerArray[index] = item;
            Count++;
        }

        public void Add(T item)
        {
            if (Count >= Capacity)
            {
                throw new InvalidOperationException($"Cannot {nameof(Add)}. Because capacity reached max.");
            }
            _innerArray[Count++] = item;
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index < 0)
            {
                return false;
            }

            RemoveAt(index);
            return true;
        }

        public void RemoveAt(int index)
        {
            CheckIndex(index);

            for (int i = index; i < Count - 1; i++)
            {
                _innerArray[i] = _innerArray[i + 1];
            }
            _innerArray[--Count] = default!;
        }

        public void Clear()
        {
            _innerArray = default;
            Count = 0;
        }

        public readonly bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public readonly int IndexOf(T item)
        {
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;

            for (int i = 0; i < Count; i++)
            {
                if (comparer.Equals(_innerArray[i], item))
                {
                    return i;
                }
            }
            return -1;
        }

        public readonly void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            }
            if (array.Length - arrayIndex < Count)
            {
                throw new ArgumentException("Destination array is too short.");
            }

            for (int i = 0; i < Count; i++)
            {
                array[i + arrayIndex] = _innerArray[i];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly void CheckIndex(int index)
        {
            if (index < 0 || Count <= index)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }
    }

    public struct StructList16<T>
    {
        public int Count { get; private set; }

        public readonly int Capacity => _innerArray.Length;

        public T this[int index]
        {
            readonly get
            {
                CheckIndex(index);
                return _innerArray[index];
            }
            set
            {
                CheckIndex(index);
                _innerArray[index] = value;
            }
        }

        StructArray16<T> _innerArray;

        public void Insert(int index, T item)
        {
            if (index < 0 || Count < index)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            if (Count >= Capacity)
            {
                throw new InvalidOperationException($"Cannot {nameof(Insert)}. Because capacity reached max.");
            }

            for (int i = Count - 1; i >= index; i--)
            {
                _innerArray[i + 1] = _innerArray[i];
            }
            _innerArray[index] = item;
            Count++;
        }

        public void Add(T item)
        {
            if (Count >= Capacity)
            {
                throw new InvalidOperationException($"Cannot {nameof(Add)}. Because capacity reached max.");
            }
            _innerArray[Count++] = item;
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index < 0)
            {
                return false;
            }

            RemoveAt(index);
            return true;
        }

        public void RemoveAt(int index)
        {
            CheckIndex(index);

            for (int i = index; i < Count - 1; i++)
            {
                _innerArray[i] = _innerArray[i + 1];
            }
            _innerArray[--Count] = default!;
        }

        public void Clear()
        {
            _innerArray = default;
            Count = 0;
        }

        public readonly bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public readonly int IndexOf(T item)
        {
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;

            for (int i = 0; i < Count; i++)
            {
                if (comparer.Equals(_innerArray[i], item))
                {
                    return i;
                }
            }
            return -1;
        }

        public readonly void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            }
            if (array.Length - arrayIndex < Count)
            {
                throw new ArgumentException("Destination array is too short.");
            }

            for (int i = 0; i < Count; i++)
            {
                array[i + arrayIndex] = _innerArray[i];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly void CheckIndex(int index)
        {
            if (index < 0 || Count <= index)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }
    }

    public struct StructList32<T>
    {
        public int Count { get; private set; }

        public readonly int Capacity => _innerArray.Length;

        public T this[int index]
        {
            readonly get
            {
                CheckIndex(index);
                return _innerArray[index];
            }
            set
            {
                CheckIndex(index);
                _innerArray[index] = value;
            }
        }

        StructArray32<T> _innerArray;

        public void Insert(int index, T item)
        {
            if (index < 0 || Count < index)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            if (Count >= Capacity)
            {
                throw new InvalidOperationException($"Cannot {nameof(Insert)}. Because capacity reached max.");
            }

            for (int i = Count - 1; i >= index; i--)
            {
                _innerArray[i + 1] = _innerArray[i];
            }
            _innerArray[index] = item;
            Count++;
        }

        public void Add(T item)
        {
            if (Count >= Capacity)
            {
                throw new InvalidOperationException($"Cannot {nameof(Add)}. Because capacity reached max.");
            }
            _innerArray[Count++] = item;
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index < 0)
            {
                return false;
            }

            RemoveAt(index);
            return true;
        }

        public void RemoveAt(int index)
        {
            CheckIndex(index);

            for (int i = index; i < Count - 1; i++)
            {
                _innerArray[i] = _innerArray[i + 1];
            }
            _innerArray[--Count] = default!;
        }

        public void Clear()
        {
            _innerArray = default;
            Count = 0;
        }

        public readonly bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public readonly int IndexOf(T item)
        {
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;

            for (int i = 0; i < Count; i++)
            {
                if (comparer.Equals(_innerArray[i], item))
                {
                    return i;
                }
            }
            return -1;
        }

        public readonly void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            }
            if (array.Length - arrayIndex < Count)
            {
                throw new ArgumentException("Destination array is too short.");
            }

            for (int i = 0; i < Count; i++)
            {
                array[i + arrayIndex] = _innerArray[i];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly void CheckIndex(int index)
        {
            if (index < 0 || Count <= index)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }
    }
}
