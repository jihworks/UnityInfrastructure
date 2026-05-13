// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using Jih.Unity.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Jih.Unity.Infrastructure.Runtime
{
    /// <remarks>
    /// Duplication check is based on a HashSet with <see cref="ReferenceEqualityComparer{T}"/>.<br/>
    /// Therefore, there is no effect even though <typeparamref name="T"/> overrides <see cref="object.GetHashCode"/> or <see cref="object.Equals(object?)"/>.
    /// </remarks>
    public abstract class BaseObjectPool<T> where T : class
    {
        protected const int DefaultInitialCollectionCapacity = 8;
        protected const bool DefaultIsThreadSafe = false;

        readonly Stack<T> _pool;
        readonly HashSet<T> _poolMap;

        readonly object? _lock;
        public bool IsThreadSafe => _lock is not null;

        /// <summary>
        /// Objects count in the pool.
        /// </summary>
        public int Count => _pool.Count;

        /// <param name="initialCollectionCapacity">Initial internal management collections capacity.</param>
        /// <param name="isThreadSafe">Whether thread-safe or not. If <c>true</c>, will use <c>lock</c> to achieve thread-safe. Pass <c>false</c> to gain performance if thread-safe is not essential.</param>
        protected BaseObjectPool(int initialCollectionCapacity, bool isThreadSafe)
        {
            if (isThreadSafe)
            {
                _lock = new object();
            }

            _pool = new Stack<T>(initialCollectionCapacity);
            _poolMap = new HashSet<T>(initialCollectionCapacity, ReferenceEqualityComparer<T>.Instance);
        }

        /// <summary>
        /// Takes an object from the pool. If the pool is empty, a new object will be created.
        /// </summary>
        public T Get()
        {
            T item;

            if (_lock is not null)
            {
                lock (_lock)
                {
                    item = Get_Impl();
                }
            }
            else
            {
                item = Get_Impl();
            }

            return item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        T Get_Impl()
        {
            T item;

            if (_pool.Count > 0)
            {
                item = _pool.Pop();
                _poolMap.Remove(item);
            }
            else
            {
                item = Create();
            }

            // Initialize_Internal the object.
            Activate(item);

            return item;
        }

        /// <summary>
        /// Returns an object to the pool. If the object is already in the pool, it will be ignored.
        /// </summary>
        public void Release(T item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (_lock is not null)
            {
                lock (_lock)
                {
                    Release_Impl(item);
                }
            }
            else
            {
                Release_Impl(item);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Release_Impl(T item)
        {
            // Atomic check & add to prevent duplicates in poolMap
            if (!_poolMap.Add(item))
            {
                // Already released item.
                return;
            }

            try
            {
                // Call Deactivate before adding to pool.
                Deactivate(item);
                // Add to pool if Deactivate succeeded.
                _pool.Push(item);
            }
            catch
            {
                // If Deactivate throws an exception, remove the item from poolMap.
                _poolMap.Remove(item);
                throw; // Throw the exception to the caller.
            }
        }

        /// <summary>
        /// Returns multiple objects to the pool. If any object is already in the pool, it will be ignored.
        /// </summary>
        public void ReleaseMany(IEnumerable<T> items)
        {
            if (items is null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (_lock is not null)
            {
                lock (_lock)
                {
                    foreach (var item in items)
                    {
                        if (item is not null)
                        {
                            Release_Impl(item);
                        }
                    }
                }
            }
            else
            {
                foreach (var item in items)
                {
                    if (item is not null)
                    {
                        Release_Impl(item);
                    }
                }
            }
        }

        /// <summary>
        /// Removes all objects from the pool. The objects will be deactivated.
        /// </summary>
        public void Clear()
        {
            if (_lock is not null)
            {
                lock (_lock)
                {
                    Clear_Impl();
                }
            }
            else
            {
                Clear_Impl();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Clear_Impl()
        {
            while (_pool.Count > 0)
            {
                T item = _pool.Pop();
                _poolMap.Remove(item);

                Deactivate(item);
                Destroy(item);
            }
        }

        /// <summary>
        /// Resizes the pool. If the pool size is larger than count, the excess layers will be removed.
        /// </summary>
        /// <param name="count">Target pool size in count.</param>
        public void Trim(int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            if (_lock is not null)
            {
                lock (_lock)
                {
                    Trim_Impl(count);
                }
            }
            else
            {
                Trim_Impl(count);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Trim_Impl(int count)
        {
            while (_pool.Count > count)
            {
                T item = _pool.Pop();
                _poolMap.Remove(item);

                Deactivate(item);
                Destroy(item);
            }
        }

        /// <summary>
        /// Adds objects to the pool until the pool size reaches the specified count.
        /// </summary>
        /// <param name="count">Target size in count.</param>
        public void EnsureCapacity(int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            if (_lock is not null)
            {
                lock (_lock)
                {
                    EnsureCapacity_Impl(count);
                }
            }
            else
            {
                EnsureCapacity_Impl(count);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void EnsureCapacity_Impl(int count)
        {
            for (int i = _pool.Count; i < count; i++)
            {
                T item = Create();
                _pool.Push(item);
                _poolMap.Add(item);
            }
        }

        /// <summary>
        /// Called when an object is created by the pool. The object will be added to the pool after this method returns.
        /// </summary>
        /// <returns>The object must be in deactivated state.</returns>
        protected abstract T Create();
        /// <summary>
        /// Called when an object is taken from the pool. The object will be removed from the pool before this method returns.
        /// </summary>
        protected abstract void Activate(T obj);
        /// <summary>
        /// Called when an object is returned to the pool. The object will be added to the pool after this method returns.
        /// </summary>
        protected abstract void Deactivate(T obj);
        /// <summary>
        /// Called when an object is removed from the pool forever because of clear or trim the pool.
        /// </summary>
        /// <remarks>
        /// The object already called deactivate.
        /// </remarks>
        protected abstract void Destroy(T obj);
    }
}
