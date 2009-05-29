//-----------------------------------------------------------------------
// <copyright file="GammaRegularizedAlgorithm.cs" company="Math.NET Project">
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
    internal class GammaRegularizedAlgorithm
    {
        /// <summary>
        /// Returns the regularized lower incomplete gamma function
        /// P(a,x) = 1/Gamma(a) * int(exp(-t)t^(a-1),t=0..x) for real a &gt; 0, x &gt; 0.
        /// </summary>
        public static
        double
        GammaRegularized(double a, double x)
        {
            const double Epsilon = 0.000000000000001;
            const double BigNumber = 4503599627370496.0;
            const double BigNumberInverse = 2.22044604925031308085e-16;

            //if(a < 0.0)
            //{
            //    throw new ArgumentOutOfRangeException("a", Properties.LocalStrings.ArgumentNotNegative);
            //}

            //if(x < 0.0)
            //{
            //    throw new ArgumentOutOfRangeException("x", Properties.LocalStrings.ArgumentNotNegative);
            //}

            if(Number.AlmostZero(a))
            {
                if(Number.AlmostZero(x))
                {
                    // either 0 or 1, depending on the limit direction
                    return Double.NaN;
                }

                return 1d;
            }

            if(Number.AlmostZero(x))
            {
                return 0d;
            }

            double ax = (a * Math.Log(x)) - x - GammaAlgorithm.GammaLn(a);
            if(ax < -709.78271289338399)
            {
                return 1d;
            }

            if(x <= 1 || x <= a)
            {
                double r2 = a;
                double c2 = 1;
                double ans2 = 1;

                do
                {
                    r2 = r2 + 1;
                    c2 = c2 * x / r2;
                    ans2 += c2;
                }
                while((c2 / ans2) > Epsilon);

                return Math.Exp(ax) * ans2 / a;
            }

            int c = 0;
            double y = 1 - a;
            double z = x + y + 1;

            double p3 = 1;
            double q3 = x;
            double p2 = x + 1;
            double q2 = z * x;
            double ans = p2 / q2;

            double error;

            do
            {
                c++;
                y += 1;
                z += 2;
                double yc = y * c;

                double p = (p2 * z) - (p3 * yc);
                double q = (q2 * z) - (q3 * yc);

                if(q != 0)
                {
                    double nextans = p / q;
                    error = Math.Abs((ans - nextans) / nextans);
                    ans = nextans;
                }
                else
                {
                    // zero div, skip
                    error = 1;
                }

                // shift
                p3 = p2;
                p2 = p;
                q3 = q2;
                q2 = q;

                // normalize fraction when the numerator becomes large
                if(Math.Abs(p) > BigNumber)
                {
                    p3 *= BigNumberInverse;
                    p2 *= BigNumberInverse;
                    q3 *= BigNumberInverse;
                    q2 *= BigNumberInverse;
                }
            }
            while(error > Epsilon);

            return 1d - (Math.Exp(ax) * ans);
        }

        /// <summary>
        /// Returns the inverse P^(-1) of the regularized lower incomplete gamma function
        /// P(a,x) = 1/Gamma(a) * int(exp(-t)t^(a-1),t=0..x) for real a &gt; 0, x &gt; 0,
        /// such that P^(-1)(a,P(a,x)) == x.
        /// </summary>
        public static
        double
        GammaRegularizedInverse(double a, double y0)
        {
            const double Epsilon = 0.000000000000001;
            const double BigNumber = 4503599627370496.0;
            const double Threshold = 5 * Epsilon;

            // TODO: Consider to throw an out-of-range exception instead of NaN
            if(a < 0 || Number.AlmostZero(a) || y0 < 0 || y0 > 1)
            {
                return Double.NaN;
            }

            if(Number.AlmostZero(y0))
            {
                return 0d;
            }

            if(Number.AlmostEqual(y0, 1))
            {
                return Double.PositiveInfinity;
            }

            y0 = 1 - y0;

            double xUpper = BigNumber;
            double xLower = 0;
            double yUpper = 1;
            double yLower = 0;

            // Initial Guess
            double d = 1 / (9 * a);
            double y = 1 - d - (0.98 * Constants.Sqrt2 * ErrorFunctionAlgorithm.ErfInverse((2.0 * y0) - 1.0) * Math.Sqrt(d));
            double x = a * y * y * y;
            double lgm = GammaAlgorithm.GammaLn(a);

            for(int i = 0; i < 10; i++)
            {
                if(x < xLower || x > xUpper)
                {
                    d = 0.0625;
                    break;
                }

                y = 1 - GammaRegularized(a, x);
                if(y < yLower || y > yUpper)
                {
                    d = 0.0625;
                    break;
                }

                if(y < y0)
                {
                    xUpper = x;
                    yLower = y;
                }
                else
                {
                    xLower = x;
                    yUpper = y;
                }

                d = ((a - 1) * Math.Log(x)) - x - lgm;
                if(d < -709.78271289338399)
                {
                    d = 0.0625;
                    break;
                }

                d = -Math.Exp(d);
                d = (y - y0) / d;
                if(Math.Abs(d / x) < Epsilon)
                {
                    return x;
                }

                if((d > (x / 4)) && (y0 < 0.05))
                {
                    // Naive heuristics for cases near the singularity
                    d = x / 10;
                }

                x -= d;
            }

            if(xUpper == BigNumber)
            {
                if(x <= 0)
                {
                    x = 1;
                }

                while(xUpper == BigNumber)
                {
                    x = (1 + d) * x;
                    y = 1 - GammaRegularized(a, x);
                    if(y < y0)
                    {
                        xUpper = x;
                        yLower = y;
                        break;
                    }

                    d = d + d;
                }
            }

            int dir = 0;
            d = 0.5;
            for(int i = 0; i < 400; i++)
            {
                x = xLower + (d * (xUpper - xLower));
                y = 1 - GammaRegularized(a, x);
                lgm = (xUpper - xLower) / (xLower + xUpper);
                if(Math.Abs(lgm) < Threshold)
                {
                    return x;
                }

                lgm = (y - y0) / y0;
                if(Math.Abs(lgm) < Threshold)
                {
                    return x;
                }

                if(x <= 0d)
                {
                    return 0d;
                }

                if(y >= y0)
                {
                    xLower = x;
                    yUpper = y;
                    if(dir < 0)
                    {
                        dir = 0;
                        d = 0.5;
                    }
                    else
                    {
                        if(dir > 1)
                        {
                            d = (0.5 * d) + 0.5;
                        }
                        else
                        {
                            d = (y0 - yLower) / (yUpper - yLower);
                        }
                    }

                    dir = dir + 1;
                }
                else
                {
                    xUpper = x;
                    yLower = y;
                    if(dir > 0)
                    {
                        dir = 0;
                        d = 0.5;
                    }
                    else
                    {
                        if(dir < -1)
                        {
                            d = 0.5 * d;
                        }
                        else
                        {
                            d = (y0 - yLower) / (yUpper - yLower);
                        }
                    }

                    dir = dir - 1;
                }
            }

            return x;
        }
    }
}
