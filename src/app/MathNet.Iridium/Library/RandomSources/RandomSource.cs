//-----------------------------------------------------------------------
// <copyright file="RandomSource.cs" company="Math.NET Project">
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
// <contribution>
//    Troschuetz.Random Class Library, Stefan Troschütz (stefan@troschuetz.de)
// </contribution>
//-----------------------------------------------------------------------

using System;

namespace MathNet.Numerics.RandomSources
{
    using Distributions;

    /// <summary>
    /// Declares common functionality for all random number generators.
    /// </summary>
    public abstract class RandomSource :
        IContinuousGenerator
    {
        /// <summary>
        /// Returns a nonnegative random number less than <see cref="Int32.MaxValue"/>.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer greater than or equal to 0, and less than <see cref="Int32.MaxValue"/>; that is, 
        ///   the range of return values includes 0 but not <see name="Int32.MaxValue"/>.
        /// </returns>
        /// <seealso cref="NextFullRangeInt32()"/>
        public abstract
        int
        Next();

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
        public abstract
        int
        Next(int maxValue);

        /// <summary>
        /// Returns a random number within the specified range. 
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
        /// </returns>
        public abstract
        int
        Next(int minValue, int maxValue);

        /// <summary>
        /// Returns a nonnegative random number less than <see cref="Int64.MaxValue"/>.
        /// </summary>
        /// <returns>
        /// A 64-bit signed integer greater than or equal to 0, and less than <see cref="Int64.MaxValue"/>; that is, 
        /// the range of return values includes 0 but not <see name="Int64.MaxValue"/>.
        /// </returns>
        /// <seealso cref="NextFullRangeInt64()"/>
        public virtual
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
        /// Returns a random number of the full Int32 range.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer of the full range, including 0, negative numbers,
        /// <see cref="Int32.MaxValue"/> and <see cref="Int32.MinValue"/>.
        /// </returns>
        /// <seealso cref="Next()"/>
        public virtual
        int
        NextFullRangeInt32()
        {
            byte[] buffer = new byte[sizeof(int)];
            NextBytes(buffer);
            return BitConverter.ToInt32(buffer, 0);
        }

        /// <summary>
        /// Returns a random number of the full Int64 range.
        /// </summary>
        /// <returns>
        /// A 64-bit signed integer of the full range, including 0, negative numbers,
        /// <see cref="Int64.MaxValue"/> and <see cref="Int64.MinValue"/>.
        /// </returns>
        /// <seealso cref="NextInt64()"/>
        public virtual
        long
        NextFullRangeInt64()
        {
            byte[] buffer = new byte[sizeof(long)];
            NextBytes(buffer);
            return BitConverter.ToInt64(buffer, 0);
        }

        /// <summary>
        /// Returns a nonnegative floating point random number less than 1.0.
        /// </summary>
        /// <returns>
        /// A double-precision floating point number greater than or equal to 0.0, and less than 1.0; that is, 
        ///   the range of return values includes 0.0 but not 1.0. 
        /// </returns>
        public abstract
        double
        NextDouble();

        /// <summary>
        /// Returns a nonnegative floating point random number less than the specified maximum.
        /// </summary>
        /// <param name="maxValue">
        /// The exclusive upper bound of the random number to be generated. 
        /// <paramref name="maxValue"/> must be greater than or equal to 0.0. 
        /// </param>
        /// <returns>
        /// A double-precision floating point number greater than or equal to 0.0, and less than <paramref name="maxValue"/>; 
        ///   that is, the range of return values includes 0 but not <paramref name="maxValue"/>. 
        /// </returns>
        public abstract
        double
        NextDouble(double maxValue);

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
        /// </returns>
        public abstract
        double
        NextDouble(double minValue, double maxValue);

        /// <summary>
        /// Returns a nonnegative decimal floating point random number less than 1.0.
        /// </summary>
        /// <returns>
        /// A decimal floating point number greater than or equal to 0.0, and less than 1.0; that is, 
        /// the range of return values includes 0.0 but not 1.0.
        /// </returns>
        public virtual
        decimal
        NextDecimal()
        {
            decimal candidate;

            // 50.049 % chance that the number is below 1.0. Try until we have one.
            // Guarantees that any decimal in the interval can
            // indeed be reached, with uniform probability.
            do
            {
                candidate = new decimal(
                    NextFullRangeInt32(),
                    NextFullRangeInt32(),
                    NextFullRangeInt32(),
                    false,
                    28);
            }
            while(candidate >= 1.0m);

            return candidate;
        }

        /// <summary>
        /// Returns a random Boolean value.
        /// </summary>
        /// <remarks>
        /// Buffers 31 random bits for future calls, so the random number generator is only invoked once in every 31 calls.
        /// </remarks>
        /// <returns>A <see cref="Boolean"/> value.</returns>
        public abstract
        bool
        NextBoolean();

        /// <summary>
        /// Fills the elements of a specified array of bytes with random numbers. 
        /// </summary>
        /// <remarks>
        /// Each element of the array of bytes is set to a random number greater than or equal to 0, and less than or 
        ///   equal to <see cref="Byte.MaxValue"/>.
        /// </remarks>
        /// <param name="buffer">An array of bytes to contain random numbers.</param>
        public abstract
        void
        NextBytes(byte[] buffer);

        /// <summary>
        /// Resets the random number generator, so that it produces the same random number sequence again.
        /// </summary>
        public abstract
        void
        Reset();

        /// <summary>
        /// Gets a value indicating whether the random number generator can be reset, so that it produces the same 
        ///   random number sequence again.
        /// </summary>
        public abstract bool CanReset
        {
            get;
        }
    }
}
