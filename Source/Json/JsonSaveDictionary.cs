// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

#if INFRASTRUCTURE_USE_NEWTONSOFT_JSON

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Jih.Unity.Infrastructure.Json
{
    [JsonObject(MemberSerialization.OptIn)]
    public class JsonSaveDictionary<TKey, TValue> : Dictionary<TKey, TValue> where TKey : notnull
    {
        [JsonProperty("Keys")]
        readonly List<TKey> _keys = new();
        [JsonProperty("Values")]
        readonly List<TValue> _values = new();

        [OnSerializing]
        void OnSerializingMethod(StreamingContext context)
        {
            _keys.Clear();
            _values.Clear();

            _keys.Capacity = Count;
            _values.Capacity = Count;

            foreach (var pair in this)
            {
                _keys.Add(pair.Key);
                _values.Add(pair.Value);
            }
        }

        [OnSerialized]
        void OnSerializedMethod(StreamingContext context)
        {
            _keys.Clear();
            _values.Clear();

            _keys.TrimExcess();
            _values.TrimExcess();
        }

        [OnDeserialized]
        void OnDeserializedMethod(StreamingContext context)
        {
            Clear();

            EnsureCapacity(_keys.Count);

            for (int i = 0; i < _keys.Count; i++)
            {
                Add(_keys[i], _values[i]);
            }

            _keys.Clear();
            _values.Clear();

            _keys.TrimExcess();
            _values.TrimExcess();
        }
    }
}

#endif
