// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

namespace Jih.Unity.Infrastructure.Collections
{
    public readonly struct ListChangeArgs<T>
    {
        public readonly ListChangeType Type;
        public readonly int Index;
        /// <summary>
        /// Only valid when <see cref="Type"/> is <b>not</b> <see cref="ListChangeType.Clear"/>.
        /// </summary>
        public readonly T Value;
        /// <summary>
        /// Only valid when <see cref="Type"/> is <see cref="ListChangeType.Replace"/>.
        /// </summary>
        public readonly T OldValue;

        public ListChangeArgs(ListChangeType type, int index, T value, T oldValue = default!)
        {
            Type = type;
            Index = index;
            Value = value;
            OldValue = oldValue;
        }
    }

    public enum ListChangeType
    {
        Add,
        Remove,
        Replace,
        Clear,
    }
}
