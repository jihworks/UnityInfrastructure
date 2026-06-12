// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;
using System.Collections.Generic;

namespace Jih.Unity.Infrastructure.Collections
{
    public interface IStructArray<T>
    {
        int Length { get; }

        T this[int index] { get; set; }

        void Sort();
        void Sort(IComparer<T> comparer);
        void Sort(Comparison<T> comparer);

        void Reverse();
    }

    public struct StructArray4<T> : IStructArray<T>, IEquatable<StructArray4<T>>
    {
        public readonly int Length => MaxLength;

        public T E0, E1, E2, E3;

        public T this[int index]
        {
            readonly get => index switch
            {
                0 => E0,
                1 => E1,
                2 => E2,
                3 => E3,
                _ => throw new ArgumentOutOfRangeException(nameof(index)),
            };
            set
            {
                switch (index)
                {
                    case 0: E0 = value; break;
                    case 1: E1 = value; break;
                    case 2: E2 = value; break;
                    case 3: E3 = value; break;
                    default: throw new ArgumentOutOfRangeException(nameof(index));
                }
            }
        }

        public void Sort()
        {
            Sort(Comparer<T>.Default);
        }
        public void Sort(IComparer<T> comparer)
        {
            if (comparer is null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            for (int i = 1; i < Length; i++)
            {
                T key = this[i];
                int j = i - 1;

                while (j >= 0 && comparer.Compare(this[j], key) > 0)
                {
                    this[j + 1] = this[j];
                    j--;
                }
                this[j + 1] = key;
            }
        }
        public void Sort(Comparison<T> comparer)
        {
            if (comparer is null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            for (int i = 1; i < Length; i++)
            {
                T key = this[i];
                int j = i - 1;

                while (j >= 0 && comparer(this[j], key) > 0)
                {
                    this[j + 1] = this[j];
                    j--;
                }
                this[j + 1] = key;
            }
        }

        public void Reverse()
        {
            int i = 0;
            int j = Length - 1;
            while (i < j)
            {
                (this[j], this[i]) = (this[i], this[j]);
                i++;
                j--;
            }
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is StructArray4<T> array && Equals(array);
        }
        public readonly bool Equals(StructArray4<T> other)
        {
            return EqualityComparer<T>.Default.Equals(E0, other.E0) &&
                   EqualityComparer<T>.Default.Equals(E1, other.E1) &&
                   EqualityComparer<T>.Default.Equals(E2, other.E2) &&
                   EqualityComparer<T>.Default.Equals(E3, other.E3);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(E0, E1, E2, E3);
        }

        public static bool operator ==(StructArray4<T> left, StructArray4<T> right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(StructArray4<T> left, StructArray4<T> right)
        {
            return !(left == right);
        }

        public const int MaxLength = 4;
    }

    public struct StructArray8<T> : IStructArray<T>, IEquatable<StructArray8<T>>
    {
        public readonly int Length => MaxLength;

        public T E0, E1, E2, E3, E4, E5, E6, E7;

        public T this[int index]
        {
            readonly get => index switch
            {
                0 => E0,
                1 => E1,
                2 => E2,
                3 => E3,
                4 => E4,
                5 => E5,
                6 => E6,
                7 => E7,
                _ => throw new ArgumentOutOfRangeException(nameof(index)),
            };
            set
            {
                switch (index)
                {
                    case 0: E0 = value; break;
                    case 1: E1 = value; break;
                    case 2: E2 = value; break;
                    case 3: E3 = value; break;
                    case 4: E4 = value; break;
                    case 5: E5 = value; break;
                    case 6: E6 = value; break;
                    case 7: E7 = value; break;
                    default: throw new ArgumentOutOfRangeException(nameof(index));
                }
            }
        }

        public void Sort()
        {
            Sort(Comparer<T>.Default);
        }
        public void Sort(IComparer<T> comparer)
        {
            if (comparer is null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            for (int i = 1; i < Length; i++)
            {
                T key = this[i];
                int j = i - 1;

                while (j >= 0 && comparer.Compare(this[j], key) > 0)
                {
                    this[j + 1] = this[j];
                    j--;
                }
                this[j + 1] = key;
            }
        }
        public void Sort(Comparison<T> comparer)
        {
            if (comparer is null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            for (int i = 1; i < Length; i++)
            {
                T key = this[i];
                int j = i - 1;

                while (j >= 0 && comparer(this[j], key) > 0)
                {
                    this[j + 1] = this[j];
                    j--;
                }
                this[j + 1] = key;
            }
        }

        public void Reverse()
        {
            int i = 0;
            int j = Length - 1;
            while (i < j)
            {
                (this[j], this[i]) = (this[i], this[j]);
                i++;
                j--;
            }
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is StructArray8<T> array && Equals(array);
        }
        public readonly bool Equals(StructArray8<T> other)
        {
            return EqualityComparer<T>.Default.Equals(E0, other.E0) &&
                   EqualityComparer<T>.Default.Equals(E1, other.E1) &&
                   EqualityComparer<T>.Default.Equals(E2, other.E2) &&
                   EqualityComparer<T>.Default.Equals(E3, other.E3) &&
                   EqualityComparer<T>.Default.Equals(E4, other.E4) &&
                   EqualityComparer<T>.Default.Equals(E5, other.E5) &&
                   EqualityComparer<T>.Default.Equals(E6, other.E6) &&
                   EqualityComparer<T>.Default.Equals(E7, other.E7);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(E0, E1, E2, E3, E4, E5, E6, E7);
        }

        public static bool operator ==(StructArray8<T> left, StructArray8<T> right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(StructArray8<T> left, StructArray8<T> right)
        {
            return !(left == right);
        }

        public const int MaxLength = 8;
    }

    public struct StructArray16<T> : IStructArray<T>, IEquatable<StructArray16<T>>
    {
        public readonly int Length => MaxLength;

        public T E0, E1, E2, E3, E4, E5, E6, E7, E8, E9, E10, E11, E12, E13, E14, E15;

        public T this[int index]
        {
            readonly get => index switch
            {
                0 => E0,
                1 => E1,
                2 => E2,
                3 => E3,
                4 => E4,
                5 => E5,
                6 => E6,
                7 => E7,
                8 => E8,
                9 => E9,
                10 => E10,
                11 => E11,
                12 => E12,
                13 => E13,
                14 => E14,
                15 => E15,
                _ => throw new ArgumentOutOfRangeException(nameof(index)),
            };
            set
            {
                switch (index)
                {
                    case 0: E0 = value; break;
                    case 1: E1 = value; break;
                    case 2: E2 = value; break;
                    case 3: E3 = value; break;
                    case 4: E4 = value; break;
                    case 5: E5 = value; break;
                    case 6: E6 = value; break;
                    case 7: E7 = value; break;
                    case 8: E8 = value; break;
                    case 9: E9 = value; break;
                    case 10: E10 = value; break;
                    case 11: E11 = value; break;
                    case 12: E12 = value; break;
                    case 13: E13 = value; break;
                    case 14: E14 = value; break;
                    case 15: E15 = value; break;
                    default: throw new ArgumentOutOfRangeException(nameof(index));
                }
            }
        }

        public void Sort()
        {
            Sort(Comparer<T>.Default);
        }
        public void Sort(IComparer<T> comparer)
        {
            if (comparer is null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            for (int i = 1; i < Length; i++)
            {
                T key = this[i];
                int j = i - 1;

                while (j >= 0 && comparer.Compare(this[j], key) > 0)
                {
                    this[j + 1] = this[j];
                    j--;
                }
                this[j + 1] = key;
            }
        }
        public void Sort(Comparison<T> comparer)
        {
            if (comparer is null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            for (int i = 1; i < Length; i++)
            {
                T key = this[i];
                int j = i - 1;

                while (j >= 0 && comparer(this[j], key) > 0)
                {
                    this[j + 1] = this[j];
                    j--;
                }
                this[j + 1] = key;
            }
        }

        public void Reverse()
        {
            int i = 0;
            int j = Length - 1;
            while (i < j)
            {
                (this[j], this[i]) = (this[i], this[j]);
                i++;
                j--;
            }
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is StructArray16<T> array && Equals(array);
        }
        public readonly bool Equals(StructArray16<T> other)
        {
            return EqualityComparer<T>.Default.Equals(E0, other.E0) &&
                   EqualityComparer<T>.Default.Equals(E1, other.E1) &&
                   EqualityComparer<T>.Default.Equals(E2, other.E2) &&
                   EqualityComparer<T>.Default.Equals(E3, other.E3) &&
                   EqualityComparer<T>.Default.Equals(E4, other.E4) &&
                   EqualityComparer<T>.Default.Equals(E5, other.E5) &&
                   EqualityComparer<T>.Default.Equals(E6, other.E6) &&
                   EqualityComparer<T>.Default.Equals(E7, other.E7) &&
                   EqualityComparer<T>.Default.Equals(E8, other.E8) &&
                   EqualityComparer<T>.Default.Equals(E9, other.E9) &&
                   EqualityComparer<T>.Default.Equals(E10, other.E10) &&
                   EqualityComparer<T>.Default.Equals(E11, other.E11) &&
                   EqualityComparer<T>.Default.Equals(E12, other.E12) &&
                   EqualityComparer<T>.Default.Equals(E13, other.E13) &&
                   EqualityComparer<T>.Default.Equals(E14, other.E14) &&
                   EqualityComparer<T>.Default.Equals(E15, other.E15);
        }

        public readonly override int GetHashCode()
        {
            HashCode hash = new();
            hash.Add(E0);
            hash.Add(E1);
            hash.Add(E2);
            hash.Add(E3);
            hash.Add(E4);
            hash.Add(E5);
            hash.Add(E6);
            hash.Add(E7);
            hash.Add(E8);
            hash.Add(E9);
            hash.Add(E10);
            hash.Add(E11);
            hash.Add(E12);
            hash.Add(E13);
            hash.Add(E14);
            hash.Add(E15);
            return hash.ToHashCode();
        }

        public static bool operator ==(StructArray16<T> left, StructArray16<T> right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(StructArray16<T> left, StructArray16<T> right)
        {
            return !(left == right);
        }

        public const int MaxLength = 16;
    }

    public struct StructArray32<T> : IStructArray<T>, IEquatable<StructArray32<T>>
    {
        public readonly int Length => MaxLength;

        public T E0, E1, E2, E3, E4, E5, E6, E7, E8, E9, E10, E11, E12, E13, E14, E15,
                 E16, E17, E18, E19, E20, E21, E22, E23, E24, E25, E26, E27, E28, E29, E30, E31;

        public T this[int index]
        {
            readonly get => index switch
            {
                0 => E0,
                1 => E1,
                2 => E2,
                3 => E3,
                4 => E4,
                5 => E5,
                6 => E6,
                7 => E7,
                8 => E8,
                9 => E9,
                10 => E10,
                11 => E11,
                12 => E12,
                13 => E13,
                14 => E14,
                15 => E15,
                16 => E16,
                17 => E17,
                18 => E18,
                19 => E19,
                20 => E20,
                21 => E21,
                22 => E22,
                23 => E23,
                24 => E24,
                25 => E25,
                26 => E26,
                27 => E27,
                28 => E28,
                29 => E29,
                30 => E30,
                31 => E31,
                _ => throw new ArgumentOutOfRangeException(nameof(index)),
            };
            set
            {
                switch (index)
                {
                    case 0: E0 = value; break;
                    case 1: E1 = value; break;
                    case 2: E2 = value; break;
                    case 3: E3 = value; break;
                    case 4: E4 = value; break;
                    case 5: E5 = value; break;
                    case 6: E6 = value; break;
                    case 7: E7 = value; break;
                    case 8: E8 = value; break;
                    case 9: E9 = value; break;
                    case 10: E10 = value; break;
                    case 11: E11 = value; break;
                    case 12: E12 = value; break;
                    case 13: E13 = value; break;
                    case 14: E14 = value; break;
                    case 15: E15 = value; break;
                    case 16: E16 = value; break;
                    case 17: E17 = value; break;
                    case 18: E18 = value; break;
                    case 19: E19 = value; break;
                    case 20: E20 = value; break;
                    case 21: E21 = value; break;
                    case 22: E22 = value; break;
                    case 23: E23 = value; break;
                    case 24: E24 = value; break;
                    case 25: E25 = value; break;
                    case 26: E26 = value; break;
                    case 27: E27 = value; break;
                    case 28: E28 = value; break;
                    case 29: E29 = value; break;
                    case 30: E30 = value; break;
                    case 31: E31 = value; break;
                    default: throw new ArgumentOutOfRangeException(nameof(index));
                }
            }
        }

        public void Sort()
        {
            Sort(Comparer<T>.Default);
        }
        public void Sort(IComparer<T> comparer)
        {
            if (comparer is null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            for (int i = 1; i < Length; i++)
            {
                T key = this[i];
                int j = i - 1;

                while (j >= 0 && comparer.Compare(this[j], key) > 0)
                {
                    this[j + 1] = this[j];
                    j--;
                }
                this[j + 1] = key;
            }
        }
        public void Sort(Comparison<T> comparer)
        {
            if (comparer is null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            for (int i = 1; i < Length; i++)
            {
                T key = this[i];
                int j = i - 1;

                while (j >= 0 && comparer(this[j], key) > 0)
                {
                    this[j + 1] = this[j];
                    j--;
                }
                this[j + 1] = key;
            }
        }

        public void Reverse()
        {
            int i = 0;
            int j = Length - 1;
            while (i < j)
            {
                (this[j], this[i]) = (this[i], this[j]);
                i++;
                j--;
            }
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is StructArray32<T> array && Equals(array);
        }
        public readonly bool Equals(StructArray32<T> other)
        {
            return EqualityComparer<T>.Default.Equals(E0, other.E0) &&
                   EqualityComparer<T>.Default.Equals(E1, other.E1) &&
                   EqualityComparer<T>.Default.Equals(E2, other.E2) &&
                   EqualityComparer<T>.Default.Equals(E3, other.E3) &&
                   EqualityComparer<T>.Default.Equals(E4, other.E4) &&
                   EqualityComparer<T>.Default.Equals(E5, other.E5) &&
                   EqualityComparer<T>.Default.Equals(E6, other.E6) &&
                   EqualityComparer<T>.Default.Equals(E7, other.E7) &&
                   EqualityComparer<T>.Default.Equals(E8, other.E8) &&
                   EqualityComparer<T>.Default.Equals(E9, other.E9) &&
                   EqualityComparer<T>.Default.Equals(E10, other.E10) &&
                   EqualityComparer<T>.Default.Equals(E11, other.E11) &&
                   EqualityComparer<T>.Default.Equals(E12, other.E12) &&
                   EqualityComparer<T>.Default.Equals(E13, other.E13) &&
                   EqualityComparer<T>.Default.Equals(E14, other.E14) &&
                   EqualityComparer<T>.Default.Equals(E15, other.E15) &&
                   EqualityComparer<T>.Default.Equals(E16, other.E16) &&
                   EqualityComparer<T>.Default.Equals(E17, other.E17) &&
                   EqualityComparer<T>.Default.Equals(E18, other.E18) &&
                   EqualityComparer<T>.Default.Equals(E19, other.E19) &&
                   EqualityComparer<T>.Default.Equals(E20, other.E20) &&
                   EqualityComparer<T>.Default.Equals(E21, other.E21) &&
                   EqualityComparer<T>.Default.Equals(E22, other.E22) &&
                   EqualityComparer<T>.Default.Equals(E23, other.E23) &&
                   EqualityComparer<T>.Default.Equals(E24, other.E24) &&
                   EqualityComparer<T>.Default.Equals(E25, other.E25) &&
                   EqualityComparer<T>.Default.Equals(E26, other.E26) &&
                   EqualityComparer<T>.Default.Equals(E27, other.E27) &&
                   EqualityComparer<T>.Default.Equals(E28, other.E28) &&
                   EqualityComparer<T>.Default.Equals(E29, other.E29) &&
                   EqualityComparer<T>.Default.Equals(E30, other.E30) &&
                   EqualityComparer<T>.Default.Equals(E31, other.E31);
        }

        public readonly override int GetHashCode()
        {
            HashCode hash = new();
            hash.Add(E0);
            hash.Add(E1);
            hash.Add(E2);
            hash.Add(E3);
            hash.Add(E4);
            hash.Add(E5);
            hash.Add(E6);
            hash.Add(E7);
            hash.Add(E8);
            hash.Add(E9);
            hash.Add(E10);
            hash.Add(E11);
            hash.Add(E12);
            hash.Add(E13);
            hash.Add(E14);
            hash.Add(E15);
            hash.Add(E16);
            hash.Add(E17);
            hash.Add(E18);
            hash.Add(E19);
            hash.Add(E20);
            hash.Add(E21);
            hash.Add(E22);
            hash.Add(E23);
            hash.Add(E24);
            hash.Add(E25);
            hash.Add(E26);
            hash.Add(E27);
            hash.Add(E28);
            hash.Add(E29);
            hash.Add(E30);
            hash.Add(E31);
            return hash.ToHashCode();
        }

        public static bool operator ==(StructArray32<T> left, StructArray32<T> right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(StructArray32<T> left, StructArray32<T> right)
        {
            return !(left == right);
        }

        public const int MaxLength = 32;
    }
}
