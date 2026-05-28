// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;

namespace Jih.Unity.Infrastructure.Collections
{
    public class RingBuffer<T> : IReadOnlyCollection<T>
    {
        public int Capacity { get; }
        public int Count { get; private set; } = 0;

        public bool IsFull => Count >= Capacity;
        public bool IsEmpty => Count <= 0;

        public T this[int index] { get => GetRefAt(index); set => GetRefAt(index) = value; }

        readonly T[] _buffer;
        int _head = 0, _tail = 0;

        public RingBuffer(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentException("Capacity must greater than 0.", nameof(capacity));
            }

            Capacity = capacity;
            _buffer = new T[capacity];
        }

        public void Enqueue(T item)
        {
            _buffer[_head] = item;
            _head = (_head + 1) % Capacity;

            if (Count < Capacity)
            {
                Count++;
            }
            else
            {
                _tail = (_tail + 1) % Capacity;
            }
        }

        public T Dequeue()
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException("Collection is empty.");
            }

            T item = _buffer[_tail];
            _buffer[_tail] = default!;

            _tail = (_tail + 1) % Capacity;
            Count--;

            return item;
        }

        public void Clear()
        {
            Array.Clear(_buffer, 0, Capacity);
            _head = 0;
            _tail = 0;
            Count = 0;
        }

        public ref T GetRefAt(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            int at = _tail + index;
            return ref _buffer[at % Capacity];
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

        public struct Enumerator : IEnumerator<T>
        {
            public readonly T Current
            {
                get
                {
                    int index = (_target._tail + _currentIndex) % _target.Capacity;
                    return _target._buffer[index];
                }
            }
            readonly object IEnumerator.Current => Current!;

            readonly RingBuffer<T> _target;

            int _currentIndex;
            int _count;

            internal Enumerator(RingBuffer<T> target)
            {
                _target = target;
                _currentIndex = -1;
                _count = 0;
            }

            public bool MoveNext()
            {
                if (_count < _target.Count)
                {
                    _currentIndex++;
                    _count++;
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                _currentIndex = -1;
                _count = 0;
            }

            public readonly void Dispose()
            {
            }
        }
    }
}
