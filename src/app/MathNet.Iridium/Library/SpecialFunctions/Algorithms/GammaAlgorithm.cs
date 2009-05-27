//-----------------------------------------------------------------------
// <copyright file="GammaAlgorithm.cs" company="Math.NET Project">
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
//    Cephes Math Library, Stephen L. Moshier
//    ALGLIB, Sergey Bochkanov
// </contribution>
//-----------------------------------------------------------------------

using System;

namespace MathNet.Numerics.SpecialFunctions.Algorithms
{
    internal class GammaAlgorithm
    {
        /// <summary>
        /// Returns the gamma function for real values (except at 0, -1, -2, ...).
        /// For numeric stability, consider to use <see cref="GammaLn"/> for positive values.
        /// </summary>
        /// <returns>A value Gamma(a) for a != 0,-1,-2,...</returns>
        public static
        double
        Gamma(double a)
        {
            if(a > 0.0)
            {
                return Math.Exp(GammaLn(a));
            }

            double reflection = 1.0 - a;
            double s = Math.Sin(Math.PI * reflection);

            if(Number.AlmostEqual(0.0, s))
            {
                return double.NaN; // singularity, undefined
            }

            return Math.PI / (s * Math.Exp(GammaLn(reflection)));
        }

        /// <summary>
        /// Returns the natural logarithm of the absolute of the Gamma function for real values.
        /// </summary>
        /// <remarks>Positive infinite at all non-positive integers.</remarks>
        /// <returns>A value <c>ln</c>|Gamma(a)|.</returns>
        public static
        double
        GammaLn(double x)
        {
            if(x < -34.0)
            {
                // Reflection to positive range
                double y = -x;
                double decimalPart = y - Math.Floor(y);
                double z = y * Math.Sin(Math.PI * decimalPart);
                return Constants.LnPi - Math.Log(z) - GammaLnLargePositive(y);
            }

            if(x < 13)
            {
                return GammaLnSmall(x);
            }

            return GammaLnLargePositive(x);
        }

        static double GammaLnSmall(double x)
        {
            double z = 1;

            // normalize to interval [2..3) by incr/decr, build z
            double normalized = x;
            double offset = 0;

            while(normalized >= 3)
            {
                offset--;
                normalized = x + offset;
                z *= normalized;
            }

            while(normalized < 2)
            {
                z /= normalized;
                offset++;
                normalized = x + offset;
            }

            if(z < 0)
            {
                z = -z;
            }

            // integer case
            if(normalized == 2)
            {
                return Math.Log(z);
            }

            // normalize x to range [0..1) by incr/decr
            x = x + offset - 2;

            double b = -1378.25152569120859100;
            b = -38801.6315134637840924 + (x * b);
            b = -331612.992738871184744 + (x * b);
            b = -1162370.97492762307383 + (x * b);
            b = -1721737.00820839662146 + (x * b);
            b = -853555.664245765465627 + (x * b);

            double c = 1;
            c = -351.815701436523470549 + (x * c);
            c = -17064.2106651881159223 + (x * c);
            c = -220528.590553854454839 + (x * c);
            c = -1139334.44367982507207 + (x * c);
            c = -2532523.07177582951285 + (x * c);
            c = -2018891.41433532773231 + (x * c);

            return Math.Log(z) + (x * b / c);
        }

        static double GammaLnLargePositive(double x)
        {
            // Stirling
            double q = ((x - 0.5) * Math.Log(x)) - x + Constants.Ln2Pi_2;
            if(x > 100000000)
            {
                return q;
            }

            double p = 1 / (x * x);
            if(x >= 1000.0)
            {
                double a = 7.9365079365079365079365e-4;
                a = -2.7777777777777777777778e-3 + (p * a);
                a = 0.0833333333333333333333 + (p * a);

                return q + (a / x);
            }

            double b = 8.11614167470508450300e-4;
            b = -5.95061904284301438324e-4 + (p * b);
            b = 7.93650340457716943945e-4 + (p * b);
            b = -2.77777777730099687205e-3 + (p * b);
            b = 8.33333333333331927722e-2 + (p * b);

            return q + (b / x);
        }
    }
}
