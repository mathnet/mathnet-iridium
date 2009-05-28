//-----------------------------------------------------------------------
// <copyright file="ErrorFunctionAlgorithm.cs" company="Math.NET Project">
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
    internal class ErrorFunctionAlgorithm
    {
        /// <summary>
        /// Returns the error function erf(x) = 2/sqrt(pi) * int(exp(-t^2),t=0..x)
        /// </summary>
        public static
        double
        Erf(double x)
        {
            if(double.IsNegativeInfinity(x))
            {
                return -1.0;
            }

            if(double.IsPositiveInfinity(x))
            {
                return 1.0;
            }

            return x < 0.0
                ? -GammaRegularizedAlgorithm.GammaRegularized(0.5, x * x)
                : GammaRegularizedAlgorithm.GammaRegularized(0.5, x * x);
        }

        /// <summary>
        /// Returns the inverse error function erf^-1(x).
        /// </summary>
        /// <remarks>
        /// <para>
        /// The algorithm uses a minimax approximation by rational functions
        /// and the result has a relative error whose absolute value is less
        /// than 1.15e-9.
        /// </para>
        /// <para>
        /// See the page <see href="http://home.online.no/~pjacklam/notes/invnorm/"/>
        /// for more details.
        /// </para>
        /// </remarks>
        public static
        double
        ErfInverse(double x)
        {
            //if(x < -1.0 || x > 1.0)
            //{
            //    throw new ArgumentOutOfRangeException("x", x, Properties.LocalStrings.ArgumentInIntervalXYInclusive(-1, 1));
            //}

            x = 0.5 * (x + 1.0);

            // Define break-points.
            const double Plow = 0.02425;
            const double Phigh = 1 - Plow;

            double q;

            // Rational approximation for lower region:
            if(x < Plow)
            {
                q = Math.Sqrt(-2 * Math.Log(x));
                return ((((((((((ErfInvC[0] * q) + ErfInvC[1]) * q) + ErfInvC[2]) * q) + ErfInvC[3]) * q) + ErfInvC[4]) * q) + ErfInvC[5]) /
                    ((((((((ErfInvD[0] * q) + ErfInvD[1]) * q) + ErfInvD[2]) * q) + ErfInvD[3]) * q) + 1)
                    * Constants.Sqrt1_2;
            }

            // Rational approximation for upper region:
            if(Phigh < x)
            {
                q = Math.Sqrt(-2 * Math.Log(1 - x));
                return -((((((((((ErfInvC[0] * q) + ErfInvC[1]) * q) + ErfInvC[2]) * q) + ErfInvC[3]) * q) + ErfInvC[4]) * q) + ErfInvC[5]) /
                    ((((((((ErfInvD[0] * q) + ErfInvD[1]) * q) + ErfInvD[2]) * q) + ErfInvD[3]) * q) + 1)
                    * Constants.Sqrt1_2;
            }

            // Rational approximation for central region:
            q = x - 0.5;
            double r = q * q;
            return ((((((((((ErfInvA[0] * r) + ErfInvA[1]) * r) + ErfInvA[2]) * r) + ErfInvA[3]) * r) + ErfInvA[4]) * r) + ErfInvA[5]) * q /
                ((((((((((ErfInvB[0] * r) + ErfInvB[1]) * r) + ErfInvB[2]) * r) + ErfInvB[3]) * r) + ErfInvB[4]) * r) + 1)
                * Constants.Sqrt1_2;
        }

        private static readonly double[] ErfInvA = {
            -3.969683028665376e+01, 2.209460984245205e+02,
            -2.759285104469687e+02, 1.383577518672690e+02,
            -3.066479806614716e+01, 2.506628277459239e+00
            };

        private static readonly double[] ErfInvB = {
            -5.447609879822406e+01, 1.615858368580409e+02,
            -1.556989798598866e+02, 6.680131188771972e+01,
            -1.328068155288572e+01
            };

        private static readonly double[] ErfInvC = {
            -7.784894002430293e-03, -3.223964580411365e-01,
            -2.400758277161838e+00, -2.549732539343734e+00,
            4.374664141464968e+00, 2.938163982698783e+00
            };

        private static readonly double[] ErfInvD = {
            7.784695709041462e-03, 3.224671290700398e-01,
            2.445134137142996e+00, 3.754408661907416e+00
            };
    }
}
