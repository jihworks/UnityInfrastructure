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
    public class LinkableDictionary<TKey, TValue> : BaseThreadCriticalObject, IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, IReadOnlyLinkableDictionary<TKey, TValue> where TKey : notnull
    {
        public LinkableEvent<DictionaryChangeArgs<TKey, TValue>> OnChanged { get; } = new();

        public ICollection<TKey> Keys
        {
            get
            {
                CheckThread();
                return _innerDict.Keys;
            }
        }
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

        public ICollection<TValue> Values
        {
            get
            {
                CheckThread();
                return _innerDict.Values;
            }
        }
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

        public int Count
        {
            get
            {
                CheckThread();
                return _innerDict.Count;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                CheckThread();
                return _innerDict[key];
            }

            set
            {
                CheckThread();
                if (_innerDict.TryGetValue(key, out TValue oldValue))
                {
                    // Update.
                    _innerDict[key] = value;
                    OnChanged.OnNext(new DictionaryChangeArgs<TKey, TValue>(DictionaryChangeType.Update, key, value, oldValue));
                }
                else
                {
                    // Add.
                    _innerDict[key] = value;
                    OnChanged.OnNext(new DictionaryChangeArgs<TKey, TValue>(DictionaryChangeType.Add, key, value));
                }
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get
            {
                CheckThread();
                return ((ICollection<KeyValuePair<TKey, TValue>>)_innerDict).IsReadOnly;
            }
        }

        readonly Dictionary<TKey, TValue> _innerDict;

        public LinkableDictionary()
        {
            _innerDict = new Dictionary<TKey, TValue>();
        }
        public LinkableDictionary(int capacity)
        {
            _innerDict = new Dictionary<TKey, TValue>(capacity);
        }

        public void Add(TKey key, TValue value)
        {
            CheckThread();

            _innerDict.Add(key, value);
            OnChanged.OnNext(new DictionaryChangeArgs<TKey, TValue>(DictionaryChangeType.Add, key, value));
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public bool Remove(TKey key)
        {
            CheckThread();

            if (_innerDict.TryGetValue(key, out TValue removedValue))
            {
                _innerDict.Remove(key);
                OnChanged.OnNext(new DictionaryChangeArgs<TKey, TValue>(DictionaryChangeType.Remove, key, removedValue));
                return true;
            }
            return false;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public void Clear()
        {
            CheckThread();

            _innerDict.Clear();
            OnChanged.OnNext(new DictionaryChangeArgs<TKey, TValue>(DictionaryChangeType.Clear, default!, default!));
        }

        public bool ContainsKey(TKey key)
        {
            CheckThread();
            return _innerDict.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            CheckThread();
            return _innerDict.TryGetValue(key, out value);
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            CheckThread();
            return ((IDictionary<TKey, TValue>)_innerDict).Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            CheckThread();
            ((IDictionary<TKey, TValue>)_innerDict).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            CheckThread();
            return _innerDict.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            CheckThread();
            return _innerDict.GetEnumerator();
        }
    }
}
