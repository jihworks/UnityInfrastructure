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
                throw new FormatException($"Failed to read {nameof(F64)}.");
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
            if (reader.TokenType is not JsonToken.StartArray)
            {
                goto FAILED;
            }
            if (!reader.Read())
            {
                goto FAILED;
            }

            long xRaw = Convert.ToInt64(reader.Value);
            if (!reader.Read())
            {
                goto FAILED;
            }

            long yRaw = Convert.ToInt64(reader.Value);
            if (!reader.Read())
            {
                goto FAILED;
            }

            if (reader.TokenType is not JsonToken.EndArray)
            {
                goto FAILED;
            }

            return new Vector2F64(F64.FromRaw(xRaw), F64.FromRaw(yRaw));

        FAILED:
            throw new FormatException($"Failed to read {nameof(Vector2F64)}.");
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
            if (reader.TokenType is not JsonToken.StartArray)
            {
                goto FAILED;
            }
            if (!reader.Read())
            {
                goto FAILED;
            }

            long xRaw = Convert.ToInt64(reader.Value);
            if (!reader.Read())
            {
                goto FAILED;
            }

            long yRaw = Convert.ToInt64(reader.Value);
            if (!reader.Read())
            {
                goto FAILED;
            }

            long zRaw = Convert.ToInt64(reader.Value);
            if (!reader.Read())
            {
                goto FAILED;
            }

            if (reader.TokenType is not JsonToken.EndArray)
            {
                goto FAILED;
            }

            return new Vector3F64(F64.FromRaw(xRaw), F64.FromRaw(yRaw), F64.FromRaw(zRaw));

        FAILED:
            throw new FormatException($"Failed to read {nameof(Vector3F64)}.");
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
            if (reader.TokenType is not JsonToken.StartArray)
            {
                goto FAILED;
            }
            if (!reader.Read())
            {
                goto FAILED;
            }

            int a = Convert.ToInt32(reader.Value);
            if (!reader.Read())
            {
                goto FAILED;
            }

            int b = Convert.ToInt32(reader.Value);
            if (!reader.Read())
            {
                goto FAILED;
            }

            int c = Convert.ToInt32(reader.Value);
            if (!reader.Read())
            {
                goto FAILED;
            }

            if (reader.TokenType is not JsonToken.EndArray)
            {
                goto FAILED;
            }

            return new HexaCoord(a, b, c);

        FAILED:
            throw new FormatException($"Failed to read {nameof(HexaCoord)}.");
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
            if (reader.TokenType is not JsonToken.StartArray)
            {
                goto FAILED;
            }
            if (!reader.Read())
            {
                goto FAILED;
            }

            long aRaw = Convert.ToInt64(reader.Value);
            if (!reader.Read())
            {
                goto FAILED;
            }

            long bRaw = Convert.ToInt64(reader.Value);
            if (!reader.Read())
            {
                goto FAILED;
            }

            long cRaw = Convert.ToInt64(reader.Value);
            if (!reader.Read())
            {
                goto FAILED;
            }

            if (reader.TokenType is not JsonToken.EndArray)
            {
                goto FAILED;
            }

            return new HexaCoordF64(F64.FromRaw(aRaw), F64.FromRaw(bRaw), F64.FromRaw(cRaw));

        FAILED:
            throw new FormatException($"Failed to read {nameof(HexaCoordF64)}.");
        }
    }
}

#endif
