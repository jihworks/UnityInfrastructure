// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using Jih.Unity.Infrastructure.Deterministics;
using Jih.Unity.Infrastructure.HexaGrid;
using Jih.Unity.Infrastructure.Json;
using Newtonsoft.Json;
using NUnit.Framework;
using System;

public class JsonSaveLoadTestScript
{
    [Test]
    public void F64Test()
    {
        TestData testSource = new()
        {
            F64 = F64.FromDouble(0.123),
        };

        string json = JsonSave.SerializeObject(testSource, typeof(TestData).Namespace);

        TestData testResult = JsonSave.DeserializeObject<TestData>(json, typeof(TestData).Namespace)
            ?? throw new InvalidOperationException();

        Assert.AreEqual(testSource.F64, testResult.F64);
    }

    [Test]
    public void Vector2F64Test()
    {
        TestData testSource = new()
        {
            Vector2F64 = new Vector2F64(F64.FromDouble(0.123), F64.FromDouble(0.321)),
        };

        string json = JsonSave.SerializeObject(testSource, typeof(TestData).Namespace);

        TestData testResult = JsonSave.DeserializeObject<TestData>(json, typeof(TestData).Namespace)
            ?? throw new InvalidOperationException();

        Assert.AreEqual(testSource.Vector2F64, testResult.Vector2F64);
    }

    [Test]
    public void Vector3F64Test()
    {
        TestData testSource = new()
        {
            Vector3F64 = new Vector3F64(F64.FromDouble(0.123), F64.FromLong(0x7fff0000L), F64.FromInt(999)),
        };

        string json = JsonSave.SerializeObject(testSource, typeof(TestData).Namespace);

        TestData testResult = JsonSave.DeserializeObject<TestData>(json, typeof(TestData).Namespace)
            ?? throw new InvalidOperationException();

        Assert.AreEqual(testSource.Vector3F64, testResult.Vector3F64);
    }

    [Test]
    public void HexaCoordTest()
    {
        TestData testSource = new()
        {
            HexaCoord = new HexaCoord(1, 2, -3),
        };

        string json = JsonSave.SerializeObject(testSource, typeof(TestData).Namespace);

        TestData testResult = JsonSave.DeserializeObject<TestData>(json, typeof(TestData).Namespace)
            ?? throw new InvalidOperationException();

        Assert.AreEqual(testSource.HexaCoord, testResult.HexaCoord);
    }

    [Test]
    public void HexaCoordF64Test()
    {
        TestData testSource = new()
        {
            HexaCoordF64 = new HexaCoordF64(1, 2, -3),
        };

        string json = JsonSave.SerializeObject(testSource, typeof(TestData).Namespace);

        TestData testResult = JsonSave.DeserializeObject<TestData>(json, typeof(TestData).Namespace)
            ?? throw new InvalidOperationException();

        Assert.AreEqual(testSource.HexaCoordF64, testResult.HexaCoordF64);
    }

    [JsonObject(MemberSerialization.OptIn)]
    class TestData
    {
        [JsonProperty]
        public F64 F64 { get; set; }

        [JsonProperty]
        public Vector2F64 Vector2F64 { get; set; }

        [JsonProperty]
        public Vector3F64 Vector3F64 { get; set; }

        [JsonProperty]
        public HexaCoord HexaCoord { get; set; }

        [JsonProperty]
        public HexaCoordF64 HexaCoordF64 { get; set; }

        [JsonConstructor]
        public TestData()
        {
        }
    }
}
