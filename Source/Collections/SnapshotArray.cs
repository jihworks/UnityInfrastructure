// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Jih.Unity.Infrastructure.Collections
{
    public class SnapshotArray<T>
    {
        T[] _arr = Array.Empty<T>();
        public T[] InnerArray => Volatile.Read(ref _arr);
        public int Count => InnerArray.Length;

        readonly object _lock = new();

        public void Add(T item)
        {
            lock (_lock)
            {
                int count = _arr.Length;
                T[] newArr = new T[count + 1];
                Array.Copy(_arr, newArr, count);
                newArr[count] = item;
                _arr = newArr;
            }
        }

        public void Insert(int index, T item)
        {
            lock (_lock)
            {
                int count = Count;
                if (index < 0 || count < index)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                T[] newArr = new T[count + 1];
                Array.Copy(_arr, newArr, index);
                newArr[index] = item;
                Array.Copy(_arr, index, newArr, index + 1, count - index);
                _arr = newArr;
            }
        }

        public bool Remove(T item)
        {
            lock (_lock)
            {
                int index = IndexOf(item);
                if (index < 0)
                {
                    return false;
                }

                RemoveAt(index);
                return true;
            }
        }

        public void RemoveAt(int index)
        {
            lock (_lock)
            {
                int count = Count;
                CheckIndex(index, count);

                T[] newArr = new T[count - 1];
                Array.Copy(_arr, newArr, index);
                Array.Copy(_arr, index + 1, newArr, index, count - index - 1);
                _arr = newArr;
            }
        }

        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public int IndexOf(T item)
        {
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            T[] arr = InnerArray;
            int count = arr.Length;
            for (int i = 0; i < count; i++)
            {
                if (comparer.Equals(arr[i], item))
                {
                    return i;
                }
            }
            return -1;
        }

        public void Foreach(Func<T, bool> func)
        {
            T[] arr = InnerArray;
            int count = arr.Length;
            for (int i = 0; i < count; i++)
            {
                if (!func(arr[i]))
                {
                    return;
                }
            }
        }
        public void Foreach(Func<int, T, bool> func)
        {
            T[] arr = InnerArray;
            int count = arr.Length;
            for (int i = 0; i < count; i++)
            {
                if (!func(i, arr[i]))
                {
                    return;
                }
            }
        }
        public void Foreach(Action<int, T> action)
        {
            T[] arr = InnerArray;
            int count = arr.Length;
            for (int i = 0; i < count; i++)
            {
                action(i, arr[i]);
            }
        }
        public void Foreach(Action<T> action)
        {
            T[] arr = InnerArray;
            int count = arr.Length;
            for (int i = 0; i < count; i++)
            {
                action(arr[i]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void CheckIndex(int index, int count)
        {
            if (index < 0 || count <= index)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }
    }
}
