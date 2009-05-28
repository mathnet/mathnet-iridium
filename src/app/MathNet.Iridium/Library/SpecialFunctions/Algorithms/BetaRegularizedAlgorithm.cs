//-----------------------------------------------------------------------
// <copyright file="BetaRegularizedAlgorithm.cs" company="Math.NET Project">
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
    internal class BetaRegularizedAlgorithm
    {
        /// <summary>
        /// Returns the regularized lower incomplete beta function
        /// I_x(a,b) = 1/Beta(a,b) * int(t^(a-1)*(1-t)^(b-1),t=0..x) for real a &gt; 0, b &gt; 0, 1 &gt;= x &gt;= 0.
        /// </summary>
        public static
        double
        BetaRegularized(
            double a,
            double b,
            double x)
        {
            //if(a < 0.0)
            //{
            //    throw new ArgumentOutOfRangeException("a", Properties.LocalStrings.ArgumentNotNegative);
            //}

            //if(b < 0.0)
            //{
            //    throw new ArgumentOutOfRangeException("b", Properties.LocalStrings.ArgumentNotNegative);
            //}

            //if(x < 0.0 || x > 1.0)
            //{
            //    throw new ArgumentOutOfRangeException("x", Properties.LocalStrings.ArgumentInIntervalXYInclusive(0, 1));
            //}

            double bt = (x == 0.0 || x == 1.0)
                ? 0.0
                : Math.Exp(GammaAlgorithm.GammaLn(a + b) - GammaAlgorithm.GammaLn(a) - GammaAlgorithm.GammaLn(b) + (a * Math.Log(x)) + (b * Math.Log(1.0 - x)));

            bool symmetryTransformation = (x >= (a + 1.0) / (a + b + 2.0));

            /* Continued fraction representation */

            const int MaxIterations = 100;
            double eps = Number.RelativeAccuracy;
            double fpmin = Number.SmallestNumberGreaterThanZero / eps;

            if(symmetryTransformation)
            {
                x = 1.0 - x;
                double swap = a;
                a = b;
                b = swap;
            }

            double qab = a + b;
            double qap = a + 1.0;
            double qam = a - 1.0;
            double c = 1.0;
            double d = 1.0 - (qab * x / qap);

            if(Math.Abs(d) < fpmin)
            {
                d = fpmin;
            }

            d = 1.0 / d;
            double h = d;

            for(int m = 1, m2 = 2; m <= MaxIterations; m++, m2 += 2)
            {
                double aa = m * (b - m) * x / ((qam + m2) * (a + m2));
                d = 1.0 + (aa * d);

                if(Math.Abs(d) < fpmin)
                {
                    d = fpmin;
                }

                c = 1.0 + (aa / c);
                if(Math.Abs(c) < fpmin)
                {
                    c = fpmin;
                }

                d = 1.0 / d;
                h *= d * c;
                aa = -(a + m) * (qab + m) * x / ((a + m2) * (qap + m2));
                d = 1.0 + (aa * d);

                if(Math.Abs(d) < fpmin)
                {
                    d = fpmin;
                }

                c = 1.0 + (aa / c);

                if(Math.Abs(c) < fpmin)
                {
                    c = fpmin;
                }

                d = 1.0 / d;
                double del = d * c;
                h *= del;

                if(Math.Abs(del - 1.0) <= eps)
                {
                    if(symmetryTransformation)
                    {
                        return 1.0 - (bt * h / a);
                    }

                    return bt * h / a;
                }
            }

            throw new ArgumentException(Properties.LocalStrings.ArgumentTooLargeForIterationLimit, "a,b");
        }
    }
}
