//-----------------------------------------------------------------------
// <copyright file="RandomByteSourceAdapter.cs" company="Math.NET Project">
//    Copyright (c) 2002-2009, Christoph Rüegg.
//    All Right Reserved.
// </copyright>
// <author>
//    Christoph Rüegg, http://christoph.ruegg.name
// </author>
// <product>
//    Math.NET Iridium, part of the Math.NET Project.
//    http://mathnet.opensourcedotnet.info
// </product>
// <license type="opensource" name="LGPL" version="2 or later">
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU Lesser General Public License as published
//    by the Free Software Foundation; either version 2 of the License, or
//    any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
// </license>
//-----------------------------------------------------------------------

using System;

namespace MathNet.Numerics.RandomSources.ObjectModel
{
    /// <summary>
    /// Adapter to extend byte-only uniform random sources to a full <see cref="RandomSource"/> provider.
    /// </summary>
    public abstract class RandomByteSourceAdapter :
        RandomSource
    {
        /// <summary>
        /// Represents the multiplier that computes a double-precision floating point number greater than or equal to 0.0 
        ///   and less than 1.0 when it gets applied to a nonnegative 32-bit signed integer.
        /// </summary>
        const double Int32ToDoubleMultiplier = 1.0 / (Int32.MaxValue + 1.0);

        /// <summary>
        /// Represents the multiplier that computes a double-precision floating point number greater than or equal to 0.0 
        ///   and less than 1.0 when it gets applied to a nonnegative 64-bit signed integer.
        /// </summary>
        const double Int64ToDoubleMultiplier = 1.0 / (Int64.MaxValue + 1.0);

        /// <summary>
        /// Represents the multiplier that computes a double-precision floating point number greater than or equal to 0.0 
        ///   and less than 1.0  when it gets applied to a 32-bit unsigned integer.
        /// </summary>
        const double UInt32ToDoubleMultiplier = 1.0 / (UInt32.MaxValue + 1.0);

        /// <summary>
        /// Stores an <see cref="Int32"/> used to generate up to 31 random <see cref="Boolean"/> values.
        /// </summary>
        int _bitBuffer;

        /// <summary>
        /// Stores how many random <see cref="Boolean"/> values still can be generated from <see cref="_bitBuffer"/>.
        /// </summary>
        int _bitCount;

        /// <summary>
        /// Returns a random number of the full Int32 range.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer of the full range, including 0, negative numbers,
        /// <see cref="Int32.MaxValue"/> and <see cref="Int32.MinValue"/>.
        /// </returns>
        /// <seealso cref="Next()"/>
        public override
        int
        NextFullRangeInt32()
        {
            byte[] buffer = new byte[sizeof(int)];
            NextBytes(buffer);
            return BitConverter.ToInt32(buffer, 0);
        }

        /// <summary>
        /// Returns a random number of the full UInt32 range.
        /// </summary>
        /// <returns>
        /// A 32-bit unsigned integer of the full range, including 0,
        /// <see cref="UInt32.MaxValue"/> and <see cref="UInt32.MinValue"/>.
        /// </returns>
        [CLSCompliant(false)]
        public
        uint
        NextFullRangeUInt32()
        {
            byte[] buffer = new byte[sizeof(uint)];
            NextBytes(buffer);
            return BitConverter.ToUInt32(buffer, 0);
        }

        /// <summary>
        /// Returns a random number of the full Int64 range.
        /// </summary>
        /// <returns>
        /// A 64-bit signed integer of the full range, including 0, negative numbers,
        /// <see cref="Int64.MaxValue"/> and <see cref="Int64.MinValue"/>.
        /// </returns>
        /// <seealso cref="NextInt64()"/>
        public override
        long
        NextFullRangeInt64()
        {
            byte[] buffer = new byte[sizeof(long)];
            NextBytes(buffer);
            return BitConverter.ToInt64(buffer, 0);
        }

        /// <summary>
        /// Returns a random number of the full UInt64 range.
        /// </summary>
        /// <returns>
        /// A 64-bit unsigned integer of the full range, including 0,
        /// <see cref="UInt64.MaxValue"/> and <see cref="UInt64.MinValue"/>.
        /// </returns>
        [CLSCompliant(false)]
        public
        ulong
        NextFullRangeUInt64()
        {
            byte[] buffer = new byte[sizeof(ulong)];
            NextBytes(buffer);
            return BitConverter.ToUInt64(buffer, 0);
        }

        /// <summary>
        /// Returns a nonnegative random number.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer greater than or equal to zero and less than <see cref="Int32.MaxValue"/>.
        /// </returns>
        public override
        int
        Next()
        {
            byte[] buffer = new byte[sizeof(int)];
            NextBytes(buffer);
            int candidate = BitConverter.ToInt32(buffer, 0);

            // wrap negative numbers around, mapping every negative number to a distinct nonnegative number
            // MinValue -> 0, -1 -> MaxValue
            candidate &= Int32.MaxValue;

            // skip candidate if it is MaxValue. Recursive since rare.
            return (candidate == Int32.MaxValue) ? Next() : candidate;
        }

        /// <summary>
        /// Returns a nonnegative random number less than the specified maximum.
        /// </summary>
        /// <param name="maxValue">
        /// The exclusive upper bound of the random number to be generated.
        /// <paramref name="maxValue"/> must be greater than or equal to 0.
        /// </param>
        /// <returns>
        /// A 32-bit signed integer greater than or equal to 0, and less than <paramref name="maxValue"/>; that is, 
        ///   the range of return values includes 0 but not <paramref name="maxValue"/>. 
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="maxValue"/> is less than 0.
        /// </exception>
        public override
        int
        Next(int maxValue)
        {
            if(maxValue < 0)
            {
                throw new ArgumentOutOfRangeException(
                    "maxValue",
                    maxValue,
                    Properties.LocalStrings.ArgumentOutOfRangeGreaterEqual("maxValue", 0));
            }

            return (int)((int)(NextFullRangeUInt32() >> 1) * Int32ToDoubleMultiplier * maxValue);
        }

        /// <summary>
        /// Returns a random number within a specified range.
        /// </summary>
        /// <param name="minValue">
        /// The inclusive lower bound of the random number to be generated.
        /// </param>
        /// <param name="maxValue">
        /// The exclusive upper bound of the random number to be generated.
        /// <paramref name="maxValue"/> must be greater than or equal to <paramref name="minValue"/>.
        /// </param>
        /// <returns>
        /// A 32-bit signed integer greater than or equal to <paramref name="minValue"/>, and less than
        ///   <paramref name="maxValue"/>; that is, the range of return values includes <paramref name="minValue"/> but
        ///   not <paramref name="maxValue"/>. 
        /// If <paramref name="minValue"/> equals <paramref name="maxValue"/>, <paramref name="minValue"/> is returned.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="minValue"/> is greater than <paramref name="maxValue"/>.
        /// </exception>
        public override
        int
        Next(int minValue, int maxValue)
        {
            if(minValue > maxValue)
            {
                throw new ArgumentOutOfRangeException(
                    "maxValue",
                    maxValue,
                    Properties.LocalStrings.ArgumentOutOfRangeGreaterEqual("maxValue", "minValue"));
            }

            int range = maxValue - minValue;
            if(range < 0)
            {
                // The range is greater than Int32.MaxValue, so we have to use slower floating point arithmetic.
                // Also all 32 random bits (uint) have to be used which again is slower (See comment in NextDouble()).
                return minValue + (int)(NextFullRangeUInt32() * UInt32ToDoubleMultiplier * (maxValue - (double)minValue));
            }

            // 31 random bits (int) will suffice which allows us to shift and cast to an int before the first multiplication and gain better performance.
            // See comment in NextDouble().
            return minValue + (int)((int)(NextFullRangeUInt32() >> 1) * Int32ToDoubleMultiplier * range);
        }

        /// <summary>
        /// Returns a nonnegative random number less than <see cref="Int64.MaxValue"/>.
        /// </summary>
        /// <returns>
        /// A 64-bit signed integer greater than or equal to 0, and less than <see cref="Int64.MaxValue"/>; that is, 
        /// the range of return values includes 0 but not <see cref="long.MaxValue"/>.
        /// </returns>
        /// <seealso cref="NextFullRangeInt64()"/>
        public override
        long
        NextInt64()
        {
            byte[] buffer = new byte[sizeof(long)];
            NextBytes(buffer);
            long candidate = BitConverter.ToInt64(buffer, 0);

            // wrap negative numbers around, mapping every negative number to a distinct nonnegative number
            // MinValue -> 0, -1 -> MaxValue
            candidate &= Int64.MaxValue;

            // skip candidate if it is MaxValue. Recursive since rare.
            return (candidate == Int64.MaxValue) ? NextInt64() : candidate;
        }

        /// <summary>
        /// Returns a nonnegative floating point random number less than 1.0.
        /// </summary>
        /// <returns>
        /// A double-precision floating point number greater than or equal to 0.0, and less than 1.0; that is, 
        ///   the range of return values includes 0.0 but not 1.0.
        /// </returns>
        public override
        double
        NextDouble()
        {
            return (long)(NextFullRangeUInt64() >> 1) * Int64ToDoubleMultiplier;
        }

        /// <summary>
        /// Returns a nonnegative floating point random number less than the specified maximum.
        /// </summary>
        /// <param name="maxValue">
        /// The exclusive upper bound of the random number to be generated.
        /// <paramref name="maxValue"/> must be greater than or equal to zero.
        /// </param>
        /// <returns>
        /// A double-precision floating point number greater than or equal to zero, and less than <paramref name="maxValue"/>;
        ///   that is, the range of return values includes zero but not <paramref name="maxValue"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="maxValue"/> is less than 0.
        /// </exception>
        public override
        double
        NextDouble(double maxValue)
        {
            if(maxValue < 0)
            {
                throw new ArgumentOutOfRangeException(
                    "maxValue",
                    maxValue,
                    Properties.LocalStrings.ArgumentOutOfRangeGreaterEqual("maxValue", 0));
            }

            return (long)(NextFullRangeUInt64() >> 1) * Int64ToDoubleMultiplier * maxValue;
        }

        /// <summary>
        /// Returns a floating point random number within the specified range.
        /// </summary>
        /// <param name="minValue">
        /// The inclusive lower bound of the random number to be generated.
        /// The range between <paramref name="minValue"/> and <paramref name="maxValue"/> must be less than or equal to
        ///   <see cref="Double.MaxValue"/>
        /// </param>
        /// <param name="maxValue">
        /// The exclusive upper bound of the random number to be generated.
        /// <paramref name="maxValue"/> must be greater than or equal to <paramref name="minValue"/>.
        /// The range between <paramref name="minValue"/> and <paramref name="maxValue"/> must be less than or equal to
        ///   <see cref="Double.MaxValue"/>.
        /// </param>
        /// <returns>
        /// A double-precision floating point number greater than or equal to <paramref name="minValue"/>, and less than
        ///   <paramref name="maxValue"/>; that is, the range of return values includes <paramref name="minValue"/> but
        ///   not <paramref name="maxValue"/>.
        /// If <paramref name="minValue"/> equals <paramref name="maxValue"/>, <paramref name="minValue"/> is returned.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="minValue"/> is greater than <paramref name="maxValue"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The range between <paramref name="minValue"/> and <paramref name="maxValue"/> is greater than
        ///   <see cref="Double.MaxValue"/>.
        /// </exception>
        public override
        double
        NextDouble(double minValue, double maxValue)
        {
            if(minValue > maxValue)
            {
                throw new ArgumentOutOfRangeException(
                    "maxValue",
                    maxValue,
                    Properties.LocalStrings.ArgumentOutOfRangeGreaterEqual("maxValue", "minValue"));
            }

            double range = maxValue - minValue;

            if(range == double.PositiveInfinity)
            {
                throw new ArgumentException(Properties.LocalStrings.ArgumentRangeLessEqual("minValue", "maxValue", "Double.MaxValue"));
            }

            return minValue + ((long)(NextFullRangeUInt64() >> 1) * Int64ToDoubleMultiplier * range);
        }

        /// <summary>
        /// Returns a random Boolean value.
        /// </summary>
        /// <remarks>
        /// Buffers 31 random bits (1 int) for future calls, so a new random number is only generated every 31 calls.
        /// </remarks>
        /// <returns>A <see cref="Boolean"/> value.</returns>
        public override
        bool
        NextBoolean()
        {
            if(_bitCount == 0)
            {
                // Generate 31 more bits (1 int) and store it for future calls.
                _bitBuffer = NextFullRangeInt32();

                // Reset the bitCount and use rightmost bit of buffer to generate random bool.
                _bitCount = 30;
                return (_bitBuffer & 0x1) == 1;
            }

            // Decrease the bitCount and use rightmost bit of shifted buffer to generate random bool.
            _bitCount--;
            return ((_bitBuffer >>= 1) & 0x1) == 1;
        }
    }
}