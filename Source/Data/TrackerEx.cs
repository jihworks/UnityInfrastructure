// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;

namespace Jih.Unity.Infrastructure.Data
{
    public static class TrackerEx
    {
        public static IObservable<(TItem Item, TEvent EventData)> TrackItems<TItem, TEvent>(
            this LinkableList<TItem> source,
            Func<TItem, IObservable<TEvent>> eventSelector)
        {
            return new LinkableListTracker<TItem, TEvent>(source, eventSelector);
        }

        public static IObservable<(TKey Key, TEvent EventData)> TrackItems<TKey, TValue, TEvent>(
            this LinkableDictionary<TKey, TValue> source,
            Func<TValue, IObservable<TEvent>> eventSelector) where TKey : notnull
        {
            return new LinkableDictionaryTracker<TKey, TValue, TEvent>(source, eventSelector);
        }
    }
}
