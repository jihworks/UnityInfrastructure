// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

#if INFRASTRUCTURE_USE_NEWTONSOFT_JSON

using Jih.Unity.Infrastructure.Deterministics;
using Jih.Unity.Infrastructure.HexaGrid;
using Newtonsoft.Json;
using System;

namespace Jih.Unity.Infrastructure.Json
{
    internal class JsonSaveF64Converter : JsonConverter<F64>
    {
        public static readonly JsonSaveF64Converter Instance = new();

        public override void WriteJson(JsonWriter writer, F64 value, JsonSerializer serializer)
        {
            // Write integer raw value to ensure losslessness.
            writer.WriteValue(value.RawValue);
        }

        public override F64 ReadJson(JsonReader reader, Type objectType, F64 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType is JsonToken.Null)
            {
                return F64.Zero;
            }

            long rawValue = Convert.ToInt64(reader.Value);
            return F64.FromRaw(rawValue);
        }
    }

    internal class JsonSaveVector2F64Converter : JsonConverter<Vector2F64>
    {
        public static readonly JsonSaveVector2F64Converter Instance = new();

        public override void WriteJson(JsonWriter writer, Vector2F64 value, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            writer.WriteValue(value.X.RawValue);
            writer.WriteValue(value.Y.RawValue);
            writer.WriteEndArray();
        }

        public override Vector2F64 ReadJson(JsonReader reader, Type objectType, Vector2F64 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType is JsonToken.Null)
            {
                return Vector2F64.Zero;
            }

            if (reader.TokenType is not JsonToken.StartArray)
            {
                return Vector2F64.Zero;
            }
            if (!reader.Read())
            {
                return Vector2F64.Zero;
            }

            long xRaw = Convert.ToInt64(reader.Value);
            if (!reader.Read())
            {
                return Vector2F64.Zero;
            }

            long yRaw = Convert.ToInt64(reader.Value);
            if (!reader.Read())
            {
                return Vector2F64.Zero;
            }

            if (reader.TokenType is not JsonToken.EndArray)
            {
                return Vector2F64.Zero;
            }

            return new Vector2F64(F64.FromRaw(xRaw), F64.FromRaw(yRaw));
        }
    }

    internal class JsonSaveVector3F64Converter : JsonConverter<Vector3F64>
    {
        public static readonly JsonSaveVector3F64Converter Instance = new();

        public override void WriteJson(JsonWriter writer, Vector3F64 value, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            writer.WriteValue(value.X.RawValue);
            writer.WriteValue(value.Y.RawValue);
            writer.WriteValue(value.Z.RawValue);
            writer.WriteEndArray();
        }

        public override Vector3F64 ReadJson(JsonReader reader, Type objectType, Vector3F64 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType is JsonToken.Null)
            {
                return Vector3F64.Zero;
            }

            if (reader.TokenType is not JsonToken.StartArray)
            {
                return Vector3F64.Zero;
            }
            if (!reader.Read())
            {
                return Vector3F64.Zero;
            }

            long xRaw = Convert.ToInt64(reader.Value);
            if (!reader.Read())
            {
                return Vector3F64.Zero;
            }

            long yRaw = Convert.ToInt64(reader.Value);
            if (!reader.Read())
            {
                return Vector3F64.Zero;
            }

            long zRaw = Convert.ToInt64(reader.Value);
            if (!reader.Read())
            {
                return Vector3F64.Zero;
            }

            if (reader.TokenType is not JsonToken.EndArray)
            {
                return Vector3F64.Zero;
            }

            return new Vector3F64(F64.FromRaw(xRaw), F64.FromRaw(yRaw), F64.FromRaw(zRaw));
        }
    }

    internal class JsonSaveHexaCoordConverter : JsonConverter<HexaCoord>
    {
        public static readonly JsonSaveHexaCoordConverter Instance = new();

        public override void WriteJson(JsonWriter writer, HexaCoord value, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            writer.WriteValue(value.A);
            writer.WriteValue(value.B);
            writer.WriteValue(value.C);
            writer.WriteEndArray();
        }

        public override HexaCoord ReadJson(JsonReader reader, Type objectType, HexaCoord existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType is JsonToken.Null)
            {
                return default;
            }

            if (reader.TokenType is not JsonToken.StartArray)
            {
                return default;
            }
            if (!reader.Read())
            {
                return default;
            }

            int x = Convert.ToInt32(reader.Value);
            if (!reader.Read())
            {
                return default;
            }

            int y = Convert.ToInt32(reader.Value);
            if (!reader.Read())
            {
                return default;
            }

            int z = Convert.ToInt32(reader.Value);
            if (!reader.Read())
            {
                return default;
            }

            if (reader.TokenType is not JsonToken.EndArray)
            {
                return default;
            }

            return new HexaCoord(x, y, z);
        }
    }

    internal class JsonSaveHexaCoordF64Converter : JsonConverter<HexaCoordF64>
    {
        public static readonly JsonSaveHexaCoordF64Converter Instance = new();

        public override void WriteJson(JsonWriter writer, HexaCoordF64 value, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            writer.WriteValue(value.A.RawValue);
            writer.WriteValue(value.B.RawValue);
            writer.WriteValue(value.C.RawValue);
            writer.WriteEndArray();
        }

        public override HexaCoordF64 ReadJson(JsonReader reader, Type objectType, HexaCoordF64 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType is JsonToken.Null)
            {
                return default;
            }

            if (reader.TokenType is not JsonToken.StartArray)
            {
                return default;
            }
            if (!reader.Read())
            {
                return default;
            }

            long xRaw = Convert.ToInt64(reader.Value);
            if (!reader.Read())
            {
                return default;
            }

            long yRaw = Convert.ToInt64(reader.Value);
            if (!reader.Read())
            {
                return default;
            }

            long zRaw = Convert.ToInt64(reader.Value);
            if (!reader.Read())
            {
                return default;
            }

            if (reader.TokenType is not JsonToken.EndArray)
            {
                return default;
            }

            return new HexaCoordF64(F64.FromRaw(xRaw), F64.FromRaw(yRaw), F64.FromRaw(zRaw));
        }
    }
}

#endif
