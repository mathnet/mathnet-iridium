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
        /// For numeric stability, consider to use GammaLn for positive values.
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
        /// Returns the natural logarithm of Gamma for a real value &gt; 0.
        /// </summary>
        /// <returns>A value ln|Gamma(a))| for a &gt; 0</returns>
        public static
        double
        GammaLn(double a)
        {
            double x, y, ser, temp;
            double[] coefficient = new double[] {
                76.18009172947146,
                -86.50532032941677,
                24.01409824083091,
                -1.231739572450155,
                0.1208650973866179e-2,
                -0.5395239384953e-5
                };

            y = x = a;
            temp = x + 5.5;
            temp -= ((x + 0.5) * Math.Log(temp));
            ser = 1.000000000190015;

            for(int j = 0; j <= 5; j++)
            {
                ser += (coefficient[j] / ++y);
            }

            return -temp + Math.Log(2.5066282746310005 * ser / x);
        }


    }
}
