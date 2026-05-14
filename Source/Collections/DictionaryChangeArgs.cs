// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

namespace Jih.Unity.Infrastructure.Collections
{
    public readonly struct DictionaryChangeArgs<TKey, TValue>
    {
        public readonly DictionaryChangeType Type;
        /// <summary>
        /// Only valid when <see cref="Type"/> is <b>not</b> <see cref="DictionaryChangeType.Clear"/>.
        /// </summary>
        public readonly TKey Key;
        /// <summary>
        /// Only valid when <see cref="Type"/> is <b>not</b> <see cref="DictionaryChangeType.Clear"/>.
        /// </summary>
        public readonly TValue Value;
        /// <summary>
        /// Only valid when <see cref="Type"/> is <see cref="DictionaryChangeType.Update"/>.
        /// </summary>
        public readonly TValue OldValue;

        public DictionaryChangeArgs(DictionaryChangeType type, TKey key, TValue value, TValue oldValue = default!)
        {
            Type = type;
            Key = key;
            Value = value;
            OldValue = oldValue;
        }
    }

    public enum DictionaryChangeType
    {
        Add,
        Remove,
        Update,
        Clear,
    }
}
