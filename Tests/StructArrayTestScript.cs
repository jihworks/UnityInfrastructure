// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using Jih.Unity.Infrastructure.Collections;
using NUnit.Framework;
using System;

public class StructArrayTestScript
{
    [Test]
    public void ArraySortTest()
    {
        ArraySortTest_Impl<StructArray4<int>>();
        ArraySortTest_Impl<StructArray8<int>>();
        ArraySortTest_Impl<StructArray16<int>>();
        ArraySortTest_Impl<StructArray32<int>>();
    }

    static void ArraySortTest_Impl<TArray>() where TArray : struct, IStructArray<int>
    {
        Random random = new();
        for (int r = 0; r < 1000; r++)
        {
            TArray array = default;
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = random.Next();
            }

            array.Sort();

            for (int i = 0; i < array.Length - 1; i++)
            {
                int v0 = array[i];
                int v1 = array[i + 1];
                Assert.IsTrue(v1 >= v0);
            }
        }
    }

    [Test]
    public void ListSortTest()
    {
        ListSortTest_Impl<StructList4<int>>();
        ListSortTest_Impl<StructList8<int>>();
        ListSortTest_Impl<StructList16<int>>();
        ListSortTest_Impl<StructList32<int>>();
    }

    static void ListSortTest_Impl<TList>() where TList : struct, IStructList<int>
    {
        Random random = new();
        for (int r = 0; r < 1000; r++)
        {
            TList list = default;
            for (int i = 0; i < list.Capacity; i++)
            {
                list.Add(random.Next());
            }

            list.Sort();

            for (int i = 0; i < list.Capacity - 1; i++)
            {
                int v0 = list[i];
                int v1 = list[i + 1];
                Assert.IsTrue(v1 >= v0);
            }
        }
    }
}
