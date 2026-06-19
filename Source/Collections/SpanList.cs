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
    public ref struct SpanList<T>
    {
        public int Count { readonly get; private set; }

        public readonly int Capacity => _innerSpan.Length;

        public readonly ref T this[int index]
        {
            get
            {
                CheckIndex(index);
                return ref _innerSpan[index];
            }
        }

        readonly Span<T> _innerSpan;

        public SpanList(Span<T> arr)
        {
            Count = 0;
            _innerSpan = arr;
        }

        public readonly Span<T> GetInnerSpan()
        {
            return _innerSpan;
        }

        public readonly Span<T> ToSpan()
        {
            return _innerSpan[..Count];
        }

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
                _innerSpan[i + 1] = _innerSpan[i];
            }
            _innerSpan[index] = item;
            Count++;
        }

        public void Add(T item)
        {
            if (Count >= Capacity)
            {
                throw new InvalidOperationException($"Cannot {nameof(Add)}. Because capacity reached max.");
            }
            _innerSpan[Count++] = item;
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
                _innerSpan[i] = _innerSpan[i + 1];
            }
            _innerSpan[--Count] = default!;
        }

        public void Clear()
        {
            _innerSpan[..Count].Clear();
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
                if (comparer.Equals(_innerSpan[i], item))
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

            _innerSpan[..Count].CopyTo(array.AsSpan(arrayIndex));
        }

        public readonly void Sort()
        {
            Sort(Comparer<T>.Default);
        }
        public readonly void Sort(IComparer<T> comparer)
        {
            if (comparer is null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            for (int i = 1; i < Count; i++)
            {
                T key = _innerSpan[i];
                int j = i - 1;

                while (j >= 0 && comparer.Compare(_innerSpan[j], key) > 0)
                {
                    _innerSpan[j + 1] = _innerSpan[j];
                    j--;
                }
                _innerSpan[j + 1] = key;
            }
        }
        public readonly void Sort(Comparison<T> comparer)
        {
            if (comparer is null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            for (int i = 1; i < Count; i++)
            {
                T key = _innerSpan[i];
                int j = i - 1;

                while (j >= 0 && comparer(_innerSpan[j], key) > 0)
                {
                    _innerSpan[j + 1] = _innerSpan[j];
                    j--;
                }
                _innerSpan[j + 1] = key;
            }
        }

        public readonly void Reverse()
        {
            _innerSpan[..Count].Reverse();
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
