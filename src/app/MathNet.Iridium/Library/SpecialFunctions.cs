//-----------------------------------------------------------------------
// <copyright file="SpecialFunctions.cs" company="Math.NET Project">
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
//    Fn.IntLog2 by Ben Houston, http://www.exocortex.org
//    Cephes Math Library, Stephen L. Moshier
//    ALGLIB, Sergey Bochkanov
// </contribution>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

using MathNet.Numerics.SpecialFunctions.Algorithms;

namespace MathNet.Numerics
{
    /// <summary>
    /// Double-precision special functions toolkit.
    /// </summary>
    public static class Fn
    {
        /// <summary> Returns <code>sqrt(a<sup>2</sup> + b<sup>2</sup>)</code> 
        /// without underflow/overflow.</summary>
        public static
        double
        Hypot(
            double a,
            double b)
        {
            if(Math.Abs(a) > Math.Abs(b))
            {
                double r = b / a;
                return Math.Abs(a) * Math.Sqrt(1 + (r * r));
            }

            if(!Number.AlmostZero(b))
            {
                double r = a / b;
                return Math.Abs(b) * Math.Sqrt(1 + (r * r));
            }

            return 0d;
        }

        /// <summary>
        /// Integer Power
        /// </summary>
        [CLSCompliant(false)]
        public static
        long
        IntPow(
            long radix,
            uint exponent)
        {
            // TODO: investigate for a better solution
            return (long)Math.Round(Math.Pow(radix, exponent));
        }

        #region Base 2 Integer Exponentiation

        /// <summary>
        /// Raises 2 to the provided integer exponent (0 &lt;= exponent &lt; 31).
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public static
        int
        IntPow2(int exponent)
        {
            if(exponent < 0 || exponent >= 31)
            {
                throw new ArgumentOutOfRangeException("exponent");
            }

            return 1 << exponent;
        }

        /// <summary>
        /// Evaluates the logarithm to base 2 of the provided integer value.
        /// </summary>
        public static
        int
        IntLog2(int x)
        {
            if(x <= 65536)
            {
                if(x <= 256)
                {
                    if(x <= 16)
                    {
                        if(x <= 4)
                        {
                            if(x <= 2)
                            {
                                if(x <= 1)
                                {
                                    return 0;
                                }

                                return 1;
                            }

                            return 2;
                        }

                        if(x <= 8)
                        {
                            return 3;
                        }

                        return 4;
                    }

                    if(x <= 64)
                    {
                        if(x <= 32)
                        {
                            return 5;
                        }

                        return 6;
                    }

                    if(x <= 128)
                    {
                        return 7;
                    }

                    return 8;
                }

                if(x <= 4096)
                {
                    if(x <= 1024)
                    {
                        if(x <= 512)
                        {
                            return 9;
                        }

                        return 10;
                    }

                    if(x <= 2048)
                    {
                        return 11;
                    }

                    return 12;
                }

                if(x <= 16384)
                {
                    if(x <= 8192)
                    {
                        return 13;
                    }

                    return 14;
                }

                if(x <= 32768)
                {
                    return 15;
                }

                return 16;
            }

            if(x <= 16777216)
            {
                if(x <= 1048576)
                {
                    if(x <= 262144)
                    {
                        if(x <= 131072)
                        {
                            return 17;
                        }

                        return 18;
                    }

                    if(x <= 524288)
                    {
                        return 19;
                    }

                    return 20;
                }

                if(x <= 4194304)
                {
                    if(x <= 2097152)
                    {
                        return 21;
                    }

                    return 22;
                }

                if(x <= 8388608)
                {
                    return 23;
                }

                return 24;
            }

            if(x <= 268435456)
            {
                if(x <= 67108864)
                {
                    if(x <= 33554432)
                    {
                        return 25;
                    }

                    return 26;
                }

                if(x <= 134217728)
                {
                    return 27;
                }

                return 28;
            }

            if(x <= 1073741824)
            {
                if(x <= 536870912)
                {
                    return 29;
                }

                return 30;
            }

            // since int is unsigned it can never be higher than 2,147,483,647
            return 31;
        }

        /// <summary>
        /// Returns the smallest integer power of two bigger or equal to the value. 
        /// </summary>
        public static
        int
        CeilingToPowerOf2(int value)
        {
            return value <= 0
                ? 0
                : IntPow2(IntLog2(value));
        }

        /// <summary>
        /// Returns the biggest integer power of two smaller or equal to the value. 
        /// </summary>
        public static
        int
        FloorToPowerOf2(int value)
        {
            int log = IntLog2(value);

            int retHalf = log == 0
                ? 0
                : IntPow2(log - 1);

            return retHalf == value >> 1
                ? value
                : retHalf;
        }

        #endregion

        #region Number Theory

        /// <summary>
        /// Returns the greatest common divisor of two integers using Euclid's algorithm.
        /// </summary>
        /// <returns>gcd(a,b)</returns>
        public static
        long
        Gcd(long a, long b)
        {
            while(b != 0)
            {
                long rem = a % b;
                a = b;
                b = rem;
            }

            return Math.Abs(a);
        }

        /// <summary>
        /// Returns the greatest common divisor of a set of integers using Euclid's algorithm.
        /// </summary>
        /// <returns>gcd(list of integers)</returns>
        public static
        long
        Gcd(IList<long> integers)
        {
            if(null == integers)
            {
                throw new ArgumentNullException("integers");
            }

            if(integers.Count == 0)
            {
                return 0;
            }

            long gcd = Math.Abs(integers[0]);

            for(int i = 1; (i < integers.Count) && (gcd > 1); i++)
            {
                gcd = Gcd(gcd, integers[i]);
            }

            return gcd;
        }

        /// <summary>
        /// Returns the greatest common divisor of a set of integers using Euclid's algorithm.
        /// </summary>
        /// <returns>gcd(list of integers)</returns>
        public static
        long
        Gcd(params long[] integers)
        {
            return Gcd((IList<long>)integers);
        }

        /// <summary>
        /// Computes the extended greatest common divisor, such that a*x + b*y = gcd(a,b).
        /// </summary>
        /// <returns>gcd(a,b)</returns>
        /// <example>
        /// <code>
        /// long x,y,d;
        /// d = Fn.Gcd(45,18,out x, out y);
        /// -> d == 9 &amp;&amp; x == 1 &amp;&amp; y == -2
        /// </code>
        /// The gcd of 45 and 18 is 9: 18 = 2*9, 45 = 5*9. 9 = 1*45 -2*18, therefore x=1 and y=-2.
        /// </example>
        public static
        long
        Gcd(
            long a,
            long b,
            out long x,
            out long y)
        {
            long mp = 1, np = 0, m = 0, n = 1;

            while(b != 0)
            {
                long quot = a / b;
                long rem = a % b;
                a = b;
                b = rem;

                long tmp = m;
                m = mp - (quot * m);
                mp = tmp;

                tmp = n;
                n = np - (quot * n);
                np = tmp;
            }

            if(a >= 0)
            {
                x = mp;
                y = np;
                return a;
            }

            x = -mp;
            y = -np;
            return -a;
        }

        /// <summary>
        /// Returns the least common multiple of two integers using Euclid's algorithm.
        /// </summary>
        /// <returns>lcm(a,b)</returns>
        public static
        long
        Lcm(long a, long b)
        {
            if((a == 0) || (b == 0))
            {
                return 0;
            }
            
            return Math.Abs((a / Gcd(a, b)) * b);
        }

        /// <summary>
        /// Returns the least common multiple of a set of integers using Euclid's algorithm.
        /// </summary>
        /// <returns>lcm(list of integers)</returns>
        public static
        long
        Lcm(IList<long> integers)
        {
            if(null == integers)
            {
                throw new ArgumentNullException("integers");
            }

            if(integers.Count == 0)
            {
                return 1;
            }

            long lcm = Math.Abs(integers[0]);

            for(int i = 1; i < integers.Count; i++)
            {
                lcm = Lcm(lcm, integers[i]);
            }

            return lcm;
        }

        /// <summary>
        /// Returns the least common multiple of a set of integers using Euclid's algorithm.
        /// </summary>
        /// <returns>lcm(list of integers)</returns>
        public static
        long
        Lcm(params long[] integers)
        {
            return Lcm((IList<long>)integers);
        }

        #endregion

        #region Sinc

        /// <summary>
        /// Normalized Sinc (sinus cardinalis) Function.
        /// </summary>
        /// <remarks>sinc(x) = sin(pi * x) / (pi * x)</remarks>
        public static
        double
        Sinc(double x)
        {
            if(double.IsNaN(x))
            {
                return double.NaN;
            }

            if(double.IsInfinity(x))
            {
                return 0.0;
            }

            double a = Math.PI * x;
            double sinc = Math.Sin(a) / a;

            if(double.IsInfinity(sinc) || double.IsNaN(sinc))
            {
                return 1.0;
            }

            return sinc;
        }

        #endregion

        #region Factorial, Binomial Coefficient

        /// <summary>
        /// Returns the natural logarithm of the factorial (n!) for an integer value > 0.
        /// </summary>
        /// <returns>A value ln(value!) for value > 0</returns>
        public static
        double
        FactorialLn(int value)
        {
            if(value < 0)
            {
                throw new ArgumentOutOfRangeException("value", Properties.LocalStrings.ArgumentPositive);
            }

            return FactorialAlgorithm.FactorialLn(value);
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
            if(value < 0)
            {
                throw new ArgumentOutOfRangeException("value", Properties.LocalStrings.ArgumentPositive);
            }

            return FactorialAlgorithm.Factorial(value);
        }

        /// <summary>
        /// Returns the binomial coefficient of n and k as a double precision number.
        /// </summary>
        /// <remarks>
        /// If you need to multiply or divide various such coefficients, consider
        /// using the logarithmic version <see cref="BinomialCoefficientLn"/> instead
        /// so you can add instead of multiply and subtract instead of divide, and
        /// then exponentiate the result using <see cref="System.Math.Exp"/>.
        /// </remarks>
        public static
        double
        BinomialCoefficient(int n, int k)
        {
            if(k < 0 || n < 0 || k > n)
            {
                return 0.0;
            }

            return Math.Floor(0.5 + Math.Exp(FactorialLn(n) - FactorialLn(k) - FactorialLn(n - k)));
        }

        /// <summary>
        /// Returns the natural logarithm of the binomial coefficient of n and k as a double precision number.
        /// </summary>
        public static
        double
        BinomialCoefficientLn(int n, int k)
        {
            if(k < 0 || n < 0 || k > n)
            {
                return 1.0;
            }

            return FactorialLn(n) - FactorialLn(k) - FactorialLn(n - k);
        }

        #endregion

        #region Gamma Functions

        /// <summary>
        /// Returns the natural logarithm of Gamma for a real value &gt; 0.
        /// </summary>
        /// <returns>A value ln|Gamma(a))| for a &gt; 0</returns>
        public static
        double
        GammaLn(double a)
        {
            return GammaAlgorithm.GammaLn(a);
        }

        /// <summary>
        /// Returns the gamma function for real values (except at 0, -1, -2, ...).
        /// For numeric stability, consider to use <see cref="GammaLn"/> for positive values.
        /// </summary>
        /// <returns>A value Gamma(a) for a != 0,-1,-2,...</returns>
        public static
        double
        Gamma(double a)
        {
            return GammaAlgorithm.Gamma(a);
        }

        /// <summary>
        /// Obsolete. Please use <see cref="GammaRegularized"/> instead, with the same parameters (method was renamed).
        /// </summary>
        [Obsolete("Renamed to GammaRegularized; Hence please migrate to GammaRegularized.")]
        public static
        double
        IncompleteGammaRegularized(double a, double x)
        {
            return GammaRegularized(a, x);
        }

        /// <summary>
        /// Returns the regularized lower incomplete gamma function
        /// P(a,x) = 1/Gamma(a) * int(exp(-t)t^(a-1),t=0..x) for real a &gt; 0, x &gt; 0.
        /// </summary>
        public static
        double
        GammaRegularized(double a, double x)
        {
            if(a < 0.0)
            {
                throw new ArgumentOutOfRangeException("a", Properties.LocalStrings.ArgumentNotNegative);
            }

            if(x < 0.0)
            {
                throw new ArgumentOutOfRangeException("x", Properties.LocalStrings.ArgumentNotNegative);
            }

            return GammaRegularizedAlgorithm.GammaRegularized(a, x);
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
            return GammaRegularizedAlgorithm.GammaRegularizedInverse(a, y0);
        }

        #endregion

        #region Digamma Functions

        /// <summary>
        /// Returns the digamma (psi) function of real values (except at 0, -1, -2, ...).
        /// Digamma is the logarithmic derivative of the <see cref="Gamma"/> function.
        /// </summary>
        public static
        double
        Digamma(double x)
        {
            return DigammaAlgorithm.Digamma(x);
        }

        #endregion

        #region Beta Functions

        /// <summary>
        /// Returns the Euler Beta function of real valued z > 0, w > 0.
        /// Beta(z,w) = Beta(w,z).
        /// </summary>
        public static
        double
        Beta(double z, double w)
        {
            return Math.Exp(GammaLn(z) + GammaLn(w) - GammaLn(z + w));
        }

        /// <summary>
        /// Returns the natural logarithm of the Euler Beta function of real valued z > 0, w > 0.
        /// BetaLn(z,w) = BetaLn(w,z).
        /// </summary>
        public static
        double
        BetaLn(double z, double w)
        {
            return GammaLn(z) + GammaLn(w) - GammaLn(z + w);
        }

        /// <summary>
        /// Obsolete. Please use <see cref="BetaRegularized"/> instead, with the same parameters (method was renamed).
        /// </summary>
        [Obsolete("Renamed to BetaRegularized; Hence please migrate to BetaRegularized.")]
        public static
        double
        IncompleteBetaRegularized(
            double a,
            double b,
            double x)
        {
            return BetaRegularized(a, b, x);
        }

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
            if(a < 0.0)
            {
                throw new ArgumentOutOfRangeException("a", Properties.LocalStrings.ArgumentNotNegative);
            }

            if(b < 0.0)
            {
                throw new ArgumentOutOfRangeException("b", Properties.LocalStrings.ArgumentNotNegative);
            }

            if(x < 0.0 || x > 1.0)
            {
                throw new ArgumentOutOfRangeException("x", Properties.LocalStrings.ArgumentInIntervalXYInclusive(0, 1));
            }

            return BetaRegularizedAlgorithm.BetaRegularized(a, b, x);
        }

        #endregion

        #region Error Functions

        /// <summary>
        /// Returns the error function erf(x) = 2/sqrt(pi) * int(exp(-t^2),t=0..x)
        /// </summary>
        public static
        double
        Erf(double x)
        {
            return ErrorFunctionAlgorithm.Erf(x);
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
            if(x < -1.0 || x > 1.0)
            {
                throw new ArgumentOutOfRangeException("x", x, Properties.LocalStrings.ArgumentInIntervalXYInclusive(-1, 1));
            }

            return ErrorFunctionAlgorithm.ErfInverse(x);
        }

        #endregion

        #region Harmonic Numbers

        /// <summary>
        /// Evaluates the n-th harmonic number Hn = sum(1/k,k=1..n).
        /// </summary>
        /// <param name="n">n >= 0</param>
        /// <remarks>
        /// See <a href="http://en.wikipedia.org/wiki/Harmonic_Number">Wikipedia - Harmonic Number</a>
        /// </remarks>
        public static
        double
        HarmonicNumber(int n)
        {
            if(n < 0)
            {
                throw new ArgumentOutOfRangeException("n", Properties.LocalStrings.ArgumentNotNegative);
            }

            if(n >= HarmonicPrecompSize)
            {
                double n2 = n * (double)n;
                double n4 = n2 * n2;
                return Constants.EulerGamma
                    + Math.Log(n)
                    + (0.5 / n)
                    - (1.0 / (12.0 * n2))
                    + (1.0 / (120.0 * n4));
            }

            return harmonicPrecomp[n];
        }

        #region Precomputed Static Array
        const int HarmonicPrecompSize = 32;
        static double[] harmonicPrecomp = new double[] {
            0.0,
            1.0,
            1.5,
            1.833333333333333333333333,
            2.083333333333333333333333,
            2.283333333333333333333333,
            2.45,
            2.592857142857142857142857,
            2.717857142857142857142857,
            2.828968253968253968253968,
            2.928968253968253968253968,
            3.019877344877344877344877,
            3.103210678210678210678211,
            3.180133755133755133755134,
            3.251562326562326562326562,
            3.318228993228993228993229,
            3.380728993228993228993229,
            3.439552522640757934875582,
            3.495108078196313490431137,
            3.547739657143681911483769,
            3.597739657143681911483769,
            3.645358704762729530531388,
            3.690813250217274985076843,
            3.734291511086840202468147,
            3.775958177753506869134814,
            3.815958177753506869134814,
            3.854419716215045330673275,
            3.891456753252082367710312,
            3.927171038966368081996027,
            3.961653797587057737168440,
            3.994987130920391070501774,
            4.027245195436520102759838
        };
        #endregion

        #endregion
    }
}
