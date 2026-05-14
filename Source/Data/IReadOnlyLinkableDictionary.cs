// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using Jih.Unity.Infrastructure.Collections;
using System.Collections.Generic;

namespace Jih.Unity.Infrastructure.Data
{
    public interface IReadOnlyLinkableDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue> where TKey : notnull
    {
        LinkableEvent<DictionaryChangeArgs<TKey, TValue>> OnChanged { get; }
    }
}
