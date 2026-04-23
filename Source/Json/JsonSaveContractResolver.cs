// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

#if INFRASTRUCTURE_USE_NEWTONSOFT_JSON

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Jih.Unity.Infrastructure.Json
{
    class JsonSaveContractResolver : DefaultContractResolver
    {
        // Value is dummy.
        static readonly ConcurrentDictionary<Type, bool> _checkedTypes = new();

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            static void CheckConstructors(Type reportType, IReadOnlyList<ConstructorInfo> constructors)
            {
                bool anyFound = false;
                foreach (var constructor in constructors)
                {
                    if (constructor.GetParameters().Length > 0)
                    {
                        continue;
                    }
                    if (constructor.GetCustomAttribute<JsonConstructorAttribute>() is null)
                    {
                        continue;
                    }

                    anyFound = true;
                }

                if (!anyFound)
                {
                    throw new JsonSaveException($"Type '{reportType.FullName}' must have explicit default-constructor with JsonConstructor attribute.", reportType);
                }
            }

            static void CheckType(Type checkType)
            {
                JsonObjectAttribute? objAttr = checkType.GetCustomAttribute<JsonObjectAttribute>()
                    ?? throw new JsonSaveException($"JSON save type '{checkType.FullName}' must marked with JsonObjectAttribute.", checkType);
                
                if (objAttr.MemberSerialization is not MemberSerialization.OptIn)
                {
                    throw new JsonSaveException($"JSON save type '{checkType.FullName}' must marked as MemberSerialization.OptIn.", checkType);
                }

                PropertyInfo[] properties = checkType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                foreach (var property in properties)
                {
                    JsonPropertyAttribute? propAttr = property.GetCustomAttribute<JsonPropertyAttribute>();
                    if (propAttr is null)
                    {
                        continue;
                    }

                    if (!property.CanRead)
                    {
                        throw new JsonSaveException($"JSON save Property '{property.Name}' in type '{checkType.FullName}' missing getter.", checkType, property);
                    }
                    if (!property.CanWrite)
                    {
                        throw new JsonSaveException($"JSON save Property '{property.Name}' in type '{checkType.FullName}' missing setter.", checkType, property);
                    }
                }
            }

            // Skip if already checked type.
            if (!_checkedTypes.TryAdd(type, true))
            {
                goto SKIP;
            }

            if (type.IsValueType)
            {
                CheckType(type);
            }
            else
            {
                ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                CheckConstructors(type, constructors);

                Type? currentType = type;
                while (currentType is not null && currentType != typeof(object))
                {
                    CheckType(currentType);

                    currentType = currentType.BaseType;
                }
            }

        SKIP:
            return base.CreateProperties(type, memberSerialization);
        }
    }
}

#endif
