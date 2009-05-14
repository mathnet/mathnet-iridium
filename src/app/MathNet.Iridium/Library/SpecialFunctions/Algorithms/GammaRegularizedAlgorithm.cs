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
            const int MaxIterations = 100;
            double eps = Number.RelativeAccuracy;
            double fpmin = Number.SmallestNumberGreaterThanZero / eps;

            if(a < 0d || x < 0d)
            {
                throw new ArgumentOutOfRangeException("a,x", Properties.LocalStrings.ArgumentNotNegative);
            }

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

            double gln = GammaAlgorithm.GammaLn(a);
            if(x < a + 1d)
            {
                /* Series Representation */

                if(x <= 0d)
                {
                    /* Yes, I know we've already checked for x<0.0 */

                    return 0d;
                }
                else
                {
                    double ap = a;
                    double del, sum = del = 1.0 / a;

                    for(int n = 0; n < MaxIterations; n++)
                    {
                        ++ap;
                        del *= x / ap;
                        sum += del;

                        if(Math.Abs(del) < Math.Abs(sum) * eps)
                        {
                            return sum * Math.Exp(-x + (a * Math.Log(x)) - gln);
                        }
                    }
                }
            }
            else
            {
                /* Continued fraction representation */

                double b = x + 1.0 - a;
                double c = 1.0 / fpmin;
                double d = 1.0 / b;
                double h = d;

                for(int i = 1; i <= MaxIterations; i++)
                {
                    double an = -i * (i - a);
                    b += 2.0;
                    d = (an * d) + b;

                    if(Math.Abs(d) < fpmin)
                    {
                        d = fpmin;
                    }

                    c = b + (an / c);

                    if(Math.Abs(c) < fpmin)
                    {
                        c = fpmin;
                    }

                    d = 1.0 / d;
                    double del = d * c;
                    h *= del;

                    if(Math.Abs(del - 1.0) <= eps)
                    {
                        return 1.0 - (Math.Exp(-x + (a * Math.Log(x)) - gln) * h);
                    }
                }
            }

            throw new ArgumentException(Properties.LocalStrings.ArgumentTooLargeForIterationLimit, "a");
        }

        /// <summary>
        /// Returns the inverse P^(-1) of the regularized lower incomplete gamma function
        /// P(a,x) = 1/Gamma(a) * int(exp(-t)t^(a-1),t=0..x) for real a &gt; 0, x &gt; 0,
        /// such that P^(-1)(a,P(a,x)) == x.
        /// </summary>
        public static
        double
        InverseGammaRegularized(double a, double y0)
        {
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

            const double epsilon = 0.000000000000001;
            const double big = 4503599627370496.0;
            const double threshold = 5 * epsilon;

            double xUpper = big;
            double xLower = 0;
            double yUpper = 1;
            double yLower = 0;

            // Initial Guess
            double d = 1 / (9 * a);
            double y = 1 - d - 0.98 * Constants.Sqrt2 * Fn.ErfInverse((2.0 * y0) - 1.0) * Math.Sqrt(d);
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
                if(Math.Abs(d / x) < epsilon)
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

            if(xUpper == big)
            {
                if(x <= 0)
                {
                    x = 1;
                }

                while(xUpper == big)
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
                if(Math.Abs(lgm) < threshold)
                {
                    return x;
                }

                lgm = (y - y0) / y0;
                if(Math.Abs(lgm) < threshold)
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
