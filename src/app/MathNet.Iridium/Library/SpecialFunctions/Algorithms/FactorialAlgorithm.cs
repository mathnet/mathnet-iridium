//-----------------------------------------------------------------------
// <copyright file="FactorialAlgorithm.cs" company="Math.NET Project">
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

namespace MathNet.Numerics.SpecialFunctions.Algorithms
{
    internal class FactorialAlgorithm
    {
        /// <summary>
        /// Returns the natural logarithm of the factorial (n!) for an integer value > 0.
        /// </summary>
        /// <returns>A value <c>ln</c>(value!) for value > 0</returns>
        public static
        double
        FactorialLn(int value)
        {
            //if(value < 0)
            //{
            //    throw new ArgumentOutOfRangeException("value", Properties.LocalStrings.ArgumentPositive);
            //}

            if(value <= 1)
            {
                return 0.0d;
            }

            if(value >= FactorialLnCacheSize)
            {
                return GammaAlgorithm.GammaLn(value + 1.0);
            }

            if(factorialLnCache == null)
            {
                factorialLnCache = new double[FactorialLnCacheSize];
            }

            return factorialLnCache[value] != 0.0
                ? factorialLnCache[value]
                : (factorialLnCache[value] = GammaAlgorithm.GammaLn(value + 1.0));
        }

        /// <summary>
        /// Returns the factorial (n!) of an integer number > 0. Consider using <see cref="FactorialLn"/> instead.
        /// </summary>
        /// <returns>A value value! for value > 0</returns>
        /// <remarks>
        /// If you need to multiply or divide various such factorials, consider
        /// using the logarithmic version <see cref="FactorialLn"/> instead
        /// so you can add instead of multiply and subtract instead of divide, and
        /// then exponentiate the result using <see cref="System.Math.Exp"/>.
        /// This will also completely circumvent the problem that factorials
        /// easily become very large.
        /// </remarks>
        public static
        double
        Factorial(int value)
        {
            //if(value < 0)
            //{
            //    throw new ArgumentOutOfRangeException("value", Properties.LocalStrings.ArgumentPositive);
            //}

            if(value >= FactorialPrecompSize)
            {
                return GammaAlgorithm.Gamma(value + 1.0);
            }

            return factorialPrecomp[value];
        }

        const int FactorialLnCacheSize = 2 * FactorialPrecompSize;
        const int FactorialPrecompSize = 32;
        static double[] factorialLnCache;
        static readonly double[] factorialPrecomp = new double[] {
            1d,
            1d,
            2d,
            6d,
            24d,
            120d,
            720d,
            5040d,
            40320d,
            362880d,
            3628800d,
            39916800d,
            479001600d,
            6227020800d,
            87178291200d,
            1307674368000d,
            20922789888000d,
            355687428096000d,
            6402373705728000d,
            121645100408832000d,
            2432902008176640000d,
            51090942171709440000d,
            1124000727777607680000d,
            25852016738884976640000d,
            620448401733239439360000d,
            15511210043330985984000000d,
            403291461126605635584000000d,
            10888869450418352160768000000d,
            304888344611713860501504000000d,
            8841761993739701954543616000000d,
            265252859812191058636308480000000d,
            8222838654177922817725562880000000d
        };
    }
}
