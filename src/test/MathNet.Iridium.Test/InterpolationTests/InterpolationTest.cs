//-----------------------------------------------------------------------
// <copyright file="InterpolationTest.cs" company="Math.NET Project">
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
using NUnit.Framework;

namespace Iridium.Test.InterpolationTests
{
    using MathNet.Numerics.Distributions;
    using MathNet.Numerics.Interpolation;
    using MathNet.Numerics.Interpolation.Algorithms;

    [TestFixture]
    public class InterpolationTest
    {
        [Test]
        public void TestInterpolationMethod_NevillePolynomial()
        {
            double[] t = new double[] { 0.0, 1.0, 3.0, 4.0 };
            double[] x = new double[] { 0.0, 3.0, 1.0, 3.0 };

            IInterpolationMethod method = Interpolation.CreatePolynomial(t, x);
            Assert.That(method, Is.TypeOf(typeof(PolynomialInterpolation)), "Type");

            double dx, d2x;

            for(int i = 0; i < t.Length; i++)
            {
                // verify the interpolated values exactly at the sample points.
                Assert.That(method.Interpolate(t[i]), Is.EqualTo(x[i]), "A Exact Point " + i.ToString());
                Assert.That(method.Differentiate(t[i], out dx, out d2x), Is.EqualTo(x[i]), "B Exact Point " + i.ToString());
            }

            // Maple: "with(CurveFitting);"
            // Maple: "evalf(subs({x=0.1},PolynomialInterpolation([[0,0],[1,3],[3,1],[4,3]], x)),20);"
            Assert.That(method.Interpolate(0.1), NumericIs.AlmostEqualTo(.57225000000000000000, 1e-15), "A 0.1");
            Assert.That(method.Differentiate(0.1, out dx, out d2x), NumericIs.AlmostEqualTo(.57225000000000000000, 1e-15), "B 0.1");
            Assert.That(method.Interpolate(0.4), NumericIs.AlmostEqualTo(1.8840000000000000000, 1e-15), "A 0.4");
            Assert.That(method.Differentiate(0.4, out dx, out d2x), NumericIs.AlmostEqualTo(1.8840000000000000000, 1e-15), "B 0.4");
            Assert.That(method.Interpolate(1.1), NumericIs.AlmostEqualTo(3.0314166666666666667, 1e-15), "A 1.1");
            Assert.That(method.Differentiate(1.1, out dx, out d2x), NumericIs.AlmostEqualTo(3.0314166666666666667, 1e-15), "B 1.1");
            Assert.That(method.Interpolate(3.2), NumericIs.AlmostEqualTo(1.034666666666666667, 1e-15), "A 3.2");
            Assert.That(method.Differentiate(3.2, out dx, out d2x), NumericIs.AlmostEqualTo(1.034666666666666667, 1e-15), "B 3.2");
            Assert.That(method.Interpolate(4.5), NumericIs.AlmostEqualTo(6.281250000000000000, 1e-15), "A 4.5");
            Assert.That(method.Differentiate(4.5, out dx, out d2x), NumericIs.AlmostEqualTo(6.281250000000000000, 1e-15), "B 4.5");
            Assert.That(method.Interpolate(10.0), NumericIs.AlmostEqualTo(277.50000000000000000, 1e-15), "A 10.0");
            Assert.That(method.Differentiate(10.0, out dx, out d2x), NumericIs.AlmostEqualTo(277.50000000000000000, 1e-15), "B 10.0");
            Assert.That(method.Interpolate(-10.0), NumericIs.AlmostEqualTo(-1010.8333333333333333, 1e-15), "A -10.0");
            Assert.That(method.Differentiate(-10.0, out dx, out d2x), NumericIs.AlmostEqualTo(-1010.8333333333333333, 1e-15), "B -10.0");

            // Test Linear Case
            for(int k = 2; k < 7; k++)
            {
                double[] linx, liny, linxtest, linytest;
                BuildLinearCase(2, k, out linx, out liny, out linxtest, out linytest);
                IInterpolationMethod linearMethod = Interpolation.CreatePolynomial(linx, liny);
                for(int i = 0; i < linxtest.Length; i++)
                {
                    Assert.That(linearMethod.Interpolate(linxtest[i]), NumericIs.AlmostEqualTo(linytest[i], 1e-12), String.Format("Linear k={0} i={1}", k, i));
                }
            }
        }

        [Test]
        public void TestInterpolationMethod_EquidistantBarycentricPolynomial()
        {
            double[] x = new double[] { 0.0, 3.0, 2.5, 1.0, 3.0 };

            IInterpolationMethod method = Interpolation.CreateOnEquidistantPoints(0.0, 4.0, x);
            Assert.That(method, Is.TypeOf(typeof(EquidistantPolynomialInterpolation)), "Type");

            for(int i = 0; i < 4; i++)
            {
                // verify the interpolated values exactly at the sample points.
                Assert.That(method.Interpolate(i), Is.EqualTo(x[i]), "A Exact Point " + i.ToString());
            }

            // Maple: "with(CurveFitting);"
            // Maple: "evalf(subs({x=0.1},PolynomialInterpolation([[0,0],[1,3],[2,2.5],[3,1],[4,3]], x)),20);"
            Assert.That(method.Interpolate(0.1), NumericIs.AlmostEqualTo(.48742500000000000000, 1e-15), "A 0.1");
            Assert.That(method.Interpolate(0.4), NumericIs.AlmostEqualTo(1.6968000000000000000, 1e-15), "A 0.4");
            Assert.That(method.Interpolate(1.1), NumericIs.AlmostEqualTo(3.0819250000000000000, 1e-15), "A 1.1");
            Assert.That(method.Interpolate(3.2), NumericIs.AlmostEqualTo(.940800000000000001, 1e-15), "A 3.2");
            Assert.That(method.Interpolate(4.5), NumericIs.AlmostEqualTo(7.265625000000000001, 1e-15), "A 4.5");
            Assert.That(method.Interpolate(10.0), NumericIs.AlmostEqualTo(592.50000000000000000, 1e-13), "A 10.0");
            Assert.That(method.Interpolate(-10.0), NumericIs.AlmostEqualTo(657.50000000000000000, 1e-12), "A -10.0");

            // Test Linear Case
            for(int k = 2; k < 7; k++)
            {
                double[] linx, liny, linxtest, linytest;
                BuildLinearCase(2, k, out linx, out liny, out linxtest, out linytest);
                IInterpolationMethod linearMethod = Interpolation.CreateOnEquidistantPoints(2, Math.Max(k, 3), liny);
                for(int i = 0; i < linxtest.Length; i++)
                {
                    Assert.That(linearMethod.Interpolate(linxtest[i]), NumericIs.AlmostEqualTo(linytest[i], 1e-12), String.Format("Linear k={0} i={1}", k, i));
                }
            }
        }

        [Test]
        public void TestInterpolationMethod_Chebyshev1BarycentricPolynomial()
        {
            double[] x = new double[] { 0.0, 3.0, 2.5, 1.0, 3.0 };

            IInterpolationMethod method = Interpolation.CreateOnChebyshevFirstKindPoints(0.0, 4.0, x);
            Assert.That(method, Is.TypeOf(typeof(ChebyshevFirstKindPolynomialInterpolation)), "Type");

            double[] t = Interpolation.GenerateChebyshevFirstKindSamplePoints(0.0, 4.0, 5);
            for(int i = 0; i < 4; i++)
            {
                // verify the generated chebyshev1 points
                double tt = 2.0 + (2.0 * Math.Cos(Math.PI * 0.1 * ((2 * i) + 1)));
                Assert.That(tt, NumericIs.AlmostEqualTo(t[i]), "Point " + i.ToString());

                // verify the interpolated values exactly at the sample points.
                Assert.That(method.Interpolate(tt), NumericIs.AlmostEqualTo(x[i]), "A Exact Point " + i.ToString());
            }

            // Maple: "with(CurveFitting);"
            // Maple: "evalf(subs({x=0.1},PolynomialInterpolation(evalf([[2*cos(Pi/10)+2,0],[2*cos(3*Pi/10)+2,3],[2*cos(5*Pi/10)+2,2.5],[2*cos(7*Pi/10)+2,1],[2*cos(9*Pi/10)+2,3]]), x)),20);"
            Assert.That(method.Interpolate(0.1), NumericIs.AlmostEqualTo(2.9882560375702001608, 1e-15), "A 0.1");
            Assert.That(method.Interpolate(0.4), NumericIs.AlmostEqualTo(1.7097090371118968872, 1e-15), "A 0.4");
            Assert.That(method.Interpolate(1.1), NumericIs.AlmostEqualTo(1.0462830804302586508, 1e-15), "A 1.1");
            Assert.That(method.Interpolate(3.2), NumericIs.AlmostEqualTo(2.951922899377369724, 1e-15), "A 3.2");
            Assert.That(method.Interpolate(4.5), NumericIs.AlmostEqualTo(-5.394317844683536750, 1e-15), "A 4.5");
            Assert.That(method.Interpolate(10.0), NumericIs.AlmostEqualTo(-228.01438153088988107, 1e-13), "A 10.0");
            Assert.That(method.Interpolate(-10.0), NumericIs.AlmostEqualTo(1979.2646653044133954, 1e-12), "A -10.0");
        }

        [Test]
        public void TestInterpolationMethod_Chebyshev2BarycentricPolynomial()
        {
            double[] x = new double[] { 0.0, 3.0, 2.5, 1.0, 3.0 };

            IInterpolationMethod method = Interpolation.CreateOnChebyshevSecondKindPoints(0.0, 4.0, x);
            Assert.That(method, Is.TypeOf(typeof(ChebyshevSecondKindPolynomialInterpolation)), "Type");

            double[] t = Interpolation.GenerateChebyshevSecondKindSamplePoints(0.0, 4.0, 5);
            for(int i = 0; i < 4; i++)
            {
                // verify the generated chebyshev2 points
                double tt = 2.0 + (2.0 * Math.Cos(Math.PI * i * 0.25));
                Assert.That(tt, NumericIs.AlmostEqualTo(t[i]), "Point " + i.ToString());

                // verify the interpolated values exactly at the sample points.
                Assert.That(method.Interpolate(tt), NumericIs.AlmostEqualTo(x[i]), "A Exact Point " + i.ToString());
            }

            // Maple: "with(CurveFitting);"
            // Maple: "evalf(subs({x=0.1},PolynomialInterpolation(evalf([[2*cos(0*Pi/4)+2,0],[2*cos(1*Pi/4)+2,3],[2*cos(2*Pi/4)+2,2.5],[2*cos(3*Pi/4)+2,1],[2*cos(4*Pi/4)+2,3]]), x)),20);"
            Assert.That(method.Interpolate(0.1), NumericIs.AlmostEqualTo(2.4826419375703841423, 1e-14), "A 0.1");
            Assert.That(method.Interpolate(0.4), NumericIs.AlmostEqualTo(1.3814129880730972522, 1e-14), "A 0.4");
            Assert.That(method.Interpolate(1.1), NumericIs.AlmostEqualTo(.8808232156067110292, 1e-15), "A 1.1");
            Assert.That(method.Interpolate(3.2), NumericIs.AlmostEqualTo(3.478116015902536997, 1e-15), "A 3.2");
            Assert.That(method.Interpolate(4.5), NumericIs.AlmostEqualTo(-5.035612822087164912, 1e-15), "A 4.5");
            Assert.That(method.Interpolate(10.0), NumericIs.AlmostEqualTo(-369.20562748477140583, 1e-13), "A 10.0");
            Assert.That(method.Interpolate(-10.0), NumericIs.AlmostEqualTo(1199.4696961966999204, 1e-12), "A -10.0");
        }

        [Test]
        public void TestInterpolationMethod_RationalPoleFreeBarycentric()
        {
            /**************************************************************************************************
             1st: polynomial case (equidistant polynomial generates the same values; rational would have pole)
            **************************************************************************************************/

            double[] t = new double[] { -2.0, -1.0, 0.0, 1.0, 2.0 };
            double[] x = new double[] { 1.0, 2.0, -1.0, 0.0, 1.0 };

            IInterpolationMethod method = Interpolation.Create(t, x);
            Assert.That(method, Is.TypeOf(typeof(RationalPoleFreeInterpolation)), "Type");

            for(int i = 0; i < t.Length; i++)
            {
                // verify the interpolated values exactly at the sample points.
                Assert.That(method.Interpolate(t[i]), Is.EqualTo(x[i]), "A Exact Point " + i.ToString());
            }

            // Maple: "with(CurveFitting);"
            // Maple: "PolynomialInterpolation([[-2,1],[-1,2],[0,-1],[1,0],[2,1]], x);"
            Assert.That(method.Interpolate(-2.4), NumericIs.AlmostEqualTo(-4.5968, 1e-15), "A -2.4");
            Assert.That(method.Interpolate(-0.9), NumericIs.AlmostEqualTo(1.65395, 1e-15), "A -0.9");
            Assert.That(method.Interpolate(-0.5), NumericIs.AlmostEqualTo(0.21875, 1e-15), "A -0.5");
            Assert.That(method.Interpolate(-0.1), NumericIs.AlmostEqualTo(-0.84205, 1e-15), "A -0.1");
            Assert.That(method.Interpolate(0.1), NumericIs.AlmostEqualTo(-1.10805, 1e-15), "A 0.1");
            Assert.That(method.Interpolate(0.4), NumericIs.AlmostEqualTo(-1.1248, 1e-15), "A 0.4");
            Assert.That(method.Interpolate(1.2), NumericIs.AlmostEqualTo(0.5392, 1e-15), "A 1.2");
            Assert.That(method.Interpolate(10.0), NumericIs.AlmostEqualTo((double) (-4431), 1e-12), "A 10.0");
            Assert.That(method.Interpolate(-10.0), NumericIs.AlmostEqualTo((double) (-5071), 1e-12), "A -10.0");

            /******************************************************************************
             2nd: x(t) = 1/(1+t^2), t=-5..5 (polynomial can' t interpolate that function!)
            ******************************************************************************/

            t = new double[40];
            x = new double[40];

            const double step = 10.0 / 39.0;
            for(int i = 0; i < t.Length; i++)
            {
                double tt = -5 + (i * step);
                t[i] = tt;
                x[i] = 1.0 / (1.0 + (tt * tt));
            }

            RationalPoleFreeInterpolation methodTyped = (RationalPoleFreeInterpolation)method;
            methodTyped.Init(t, x); // re-initialize for another set of points/values.

            for(int i = 0; i < t.Length; i++)
            {
                // verify the interpolated values exactly at the sample points.
                Assert.That(method.Interpolate(t[i]), Is.EqualTo(x[i]), "B Exact Point " + i.ToString());
            }

            // Maple: "with(CurveFitting);"
            // Maple: "tt := [seq(-5+(i-1)*10/39,i=1..40)]: xx := [seq(1/(1+tt[i]*tt[i]),i=1..40)]:"
            // Maple: "RationalInterpolation(tt, xx, x);"

            // Test Linear Case
            for(int k = 2; k < 7; k++)
            {
                double[] linx, liny, linxtest, linytest;
                BuildLinearCase(2, k, out linx, out liny, out linxtest, out linytest);
                IInterpolationMethod linearMethod = Interpolation.Create(linx, liny);
                for(int i = 0; i < linxtest.Length; i++)
                {
                    Assert.That(linearMethod.Interpolate(linxtest[i]), NumericIs.AlmostEqualTo(linytest[i], 1e-12), String.Format("Linear k={0} i={1}", k, i));
                }
            }
        }

        [Test]
        public void TestInterpolationMethod_RationalWithPoles()
        {
            double[] t = new double[] { 0, 1, 3, 4, 5 };
            double[] x = new double[] { 0, 3, 1000, -1000, 3 };

            RationalInterpolation method = new RationalInterpolation();
            method.Init(t, x);

            for(int i = 0; i < t.Length; i++)
            {
                Assert.That(method.Interpolate(t[i]), Is.EqualTo(x[i]), "Exact Point " + i.ToString());
            }

            // Maple: "with(CurveFitting);"
            // Maple: "evalf(subs({x=0.1},RationalInterpolation([[0,0],[1,3],[3,1000],[4,-1000], [5,3]], x)),20);"
            Assert.That(method.Interpolate(0.1), NumericIs.AlmostEqualTo(.19389203383553566255, 1e-14), "A 0.1");
            Assert.That(method.Interpolate(0.4), NumericIs.AlmostEqualTo(.88132900698869875369, 1e-14), "A 0.4");
            Assert.That(method.Interpolate(1.1), NumericIs.AlmostEqualTo(3.5057665681580626913, 1e-15), "A 1.1");
            Assert.That(method.Interpolate(3.01), NumericIs.AlmostEqualTo(1548.7666642693586902, 1e-13), "A 3.01");
            Assert.That(method.Interpolate(3.02), NumericIs.AlmostEqualTo(3362.2564334253633516, 1e-13), "A 3.02");
            Assert.That(method.Interpolate(3.03), NumericIs.AlmostEqualTo(-22332.603641443806014, 1e-12), "A 3.03");
            Assert.That(method.Interpolate(3.1), NumericIs.AlmostEqualTo(-440.30323769822443789, 1e-14), "A 3.1");
            Assert.That(method.Interpolate(3.2), NumericIs.AlmostEqualTo(-202.42421196280566349, 1e-14), "A 3.2");
            Assert.That(method.Interpolate(4.5), NumericIs.AlmostEqualTo(21.208249625210155439, 1e-14), "A 4.5");
            Assert.That(method.Interpolate(10.0), NumericIs.AlmostEqualTo(-4.8936986959784751517, 1e-13), "A 10.0");
            Assert.That(method.Interpolate(-10.0), NumericIs.AlmostEqualTo(-3.6017584308603731307, 1e-13), "A -10.0");

            // Test Linear Case
            for(int k = 2; k < 6; k++)
            {
                double[] linx, liny, linxtest, linytest;
                BuildLinearCase(2, k, out linx, out liny, out linxtest, out linytest);
                IInterpolationMethod linearMethod = Interpolation.CreateRational(linx, liny);
                for(int i = 0; i < linxtest.Length; i++)
                {
                    // very weak test, but rational with poles is incredibly bad in the linear case
                    Assert.That(linearMethod.Interpolate(linxtest[i]), Is.Not.NaN, String.Format("Linear k={0} i={1}", k, i));
                }
            }
        }

        [Test]
        public void TestInterpolationMethod_LimitedOrderPolynomial()
        {
            double[] t = new double[] { 0.0, 1.0, 3.0, 4.0 };
            double[] x = new double[] { 0.0, 3.0, 1.0, 3.0 };

            LimitedOrderPolynomialInterpolation method = new LimitedOrderPolynomialInterpolation();
            method.Init(t, x);

            for(int i = 0; i < t.Length; i++)
            {
                Assert.That(method.Interpolate(t[i]), Is.EqualTo(x[i]), "Exact Point " + i.ToString());
            }

            // Maple: "with(CurveFitting);"
            // Maple: "evalf(subs({x=0.1},PolynomialInterpolation([[0,0],[1,3],[3,1],[4,3]], x)),20);"
            Assert.That(method.Interpolate(0.1), NumericIs.AlmostEqualTo(.57225000000000000000, 1e-15), "A 0.1");
            Assert.That(method.Interpolate(0.4), NumericIs.AlmostEqualTo(1.8840000000000000000, 1e-15), "A 0.4");
            Assert.That(method.Interpolate(1.1), NumericIs.AlmostEqualTo(3.0314166666666666667, 1e-15), "A 1.1");
            Assert.That(method.Interpolate(3.2), NumericIs.AlmostEqualTo(1.034666666666666667, 1e-15), "A 3.2");
            Assert.That(method.Interpolate(4.5), NumericIs.AlmostEqualTo(6.281250000000000000, 1e-15), "A 4.5");
            Assert.That(method.Interpolate(10.0), NumericIs.AlmostEqualTo(277.50000000000000000, 1e-15), "A 10.0");
            Assert.That(method.Interpolate(-10.0), NumericIs.AlmostEqualTo(-1010.8333333333333333, 1e-15), "A -10.0");
        }

        [Test]
        public void TestInterpolationMethod_LimitedOrderRationalWithPoles()
        {
            double[] t = new double[] { 0, 1, 3,    4,     5 };
            double[] x = new double[] { 0, 3, 1000, -1000, 3 };

            LimitedOrderRationalInterpolation method = new LimitedOrderRationalInterpolation();
            method.Init(t, x);

            for(int i = 0; i < t.Length; i++)
            {
                Assert.That(method.Interpolate(t[i]), Is.EqualTo(x[i]), "Exact Point " + i.ToString());
            }

            // Maple: "with(CurveFitting);"
            // Maple: "evalf(subs({x=0.1},RationalInterpolation([[0,0],[1,3],[3,1000],[4,-1000], [5,3]], x)),20);"
            Assert.That(method.Interpolate(0.1), NumericIs.AlmostEqualTo(.19389203383553566255, 1e-15), "A 0.1");
            Assert.That(method.Interpolate(0.4), NumericIs.AlmostEqualTo(.88132900698869875369, 1e-14), "A 0.4");
            Assert.That(method.Interpolate(1.1), NumericIs.AlmostEqualTo(3.5057665681580626913, 1e-15), "A 1.1");
            Assert.That(method.Interpolate(3.01), NumericIs.AlmostEqualTo(1548.7666642693586902, 1e-13), "A 3.01");
            Assert.That(method.Interpolate(3.02), NumericIs.AlmostEqualTo(3362.2564334253633516, 1e-13), "A 3.02");
            Assert.That(method.Interpolate(3.03), NumericIs.AlmostEqualTo(-22332.603641443806014, 1e-12), "A 3.03");
            Assert.That(method.Interpolate(3.1), NumericIs.AlmostEqualTo(-440.30323769822443789, 1e-14), "A 3.1");
            Assert.That(method.Interpolate(3.2), NumericIs.AlmostEqualTo(-202.42421196280566349, 1e-14), "A 3.2");
            Assert.That(method.Interpolate(4.5), NumericIs.AlmostEqualTo(21.208249625210155439, 1e-14), "A 4.5");
            Assert.That(method.Interpolate(10.0), NumericIs.AlmostEqualTo(-4.8936986959784751517, 1e-13), "A 10.0");
            Assert.That(method.Interpolate(-10.0), NumericIs.AlmostEqualTo(-3.6017584308603731307, 1e-13), "A -10.0");
        }

        [Test]
        public void TestInterpolationMethod_LinearSpline()
        {
            double[] t = new double[] { -2.0, -1.0, 0.0, 1.0, 2.0 };
            double[] x = new double[] { 1.0, 2.0, -1.0, 0.0, 1.0 };

            IInterpolationMethod method = Interpolation.CreateLinearSpline(t, x);
            Assert.That(method, Is.TypeOf(typeof(LinearSplineInterpolation)), "Type");

            for(int i = 0; i < t.Length; i++)
            {
                // verify the interpolated values exactly at the sample points.
                Assert.That(method.Interpolate(t[i]), Is.EqualTo(x[i]), "A Exact Point " + i.ToString());
            }

            // Maple: "f := x -> piecewise(x<-1,3+x,x<0,-1-3*x,x<1,-1+x,-1+x);"
            // Maple: "f(x)"
            Assert.That(method.Interpolate(-2.4), NumericIs.AlmostEqualTo(.6, 1e-15), "A -2.4");
            Assert.That(method.Interpolate(-0.9), NumericIs.AlmostEqualTo(1.7, 1e-15), "A -0.9");
            Assert.That(method.Interpolate(-0.5), NumericIs.AlmostEqualTo(.5, 1e-15), "A -0.5");
            Assert.That(method.Interpolate(-0.1), NumericIs.AlmostEqualTo(-.7, 1e-15), "A -0.1");
            Assert.That(method.Interpolate(0.1), NumericIs.AlmostEqualTo(-.9, 1e-15), "A 0.1");
            Assert.That(method.Interpolate(0.4), NumericIs.AlmostEqualTo(-.6, 1e-15), "A 0.4");
            Assert.That(method.Interpolate(1.2), NumericIs.AlmostEqualTo(.2, 1e-15), "A 1.2");
            Assert.That(method.Interpolate(10.0), NumericIs.AlmostEqualTo(9.0, 1e-15), "A 10.0");
            Assert.That(method.Interpolate(-10.0), NumericIs.AlmostEqualTo(-7.0, 1e-15), "A -10.0");

            // Test Linear Case
            for(int k = 2; k < 6; k++)
            {
                double[] linx, liny, linxtest, linytest;
                BuildLinearCase(2, k + 1, out linx, out liny, out linxtest, out linytest);
                IInterpolationMethod linearMethod = Interpolation.CreateLinearSpline(linx, liny);
                for(int i = 0; i < linxtest.Length; i++)
                {
                    Assert.That(linearMethod.Interpolate(linxtest[i]), NumericIs.AlmostEqualTo(linytest[i], 1e-12), String.Format("Linear k={0} i={1}", k, i));
                }
            }
        }

        [Test]
        public void TestInterpolationMethod_CubicSpline_BoundaryNatural()
        {
            double[] t = new double[] { -2.0, -1.0, 0.0, 1.0, 2.0 };
            double[] x = new double[] { 1.0, 2.0, -1.0, 0.0, 1.0 };

            IInterpolationMethod method = Interpolation.CreateNaturalCubicSpline(t, x);
            Assert.That(method, Is.TypeOf(typeof(CubicSplineInterpolation)), "Type");

            for(int i = 0; i < t.Length; i++)
            {
                // verify the interpolated values exactly at the sample points.
                Assert.That(method.Interpolate(t[i]), Is.EqualTo(x[i]), "A Exact Point " + i.ToString());
            }

            // Maple: "with(CurveFitting);"
            // Maple: "evalf(subs({x=-2.4},Spline([[-2,1],[-1,2],[0,-1],[1,0],[2,1]], x, degree=3, endpoints='natural')),20);"
            Assert.That(method.Interpolate(-2.4), NumericIs.AlmostEqualTo(.144000000000000000, 1e-15), "A -2.4");
            Assert.That(method.Interpolate(-0.9), NumericIs.AlmostEqualTo(1.7906428571428571429, 1e-15), "A -0.9");
            Assert.That(method.Interpolate(-0.5), NumericIs.AlmostEqualTo(.47321428571428571431, 1e-15), "A -0.5");
            Assert.That(method.Interpolate(-0.1), NumericIs.AlmostEqualTo(-.80992857142857142857, 1e-15), "A -0.1");
            Assert.That(method.Interpolate(0.1), NumericIs.AlmostEqualTo(-1.1089285714285714286, 1e-15), "A 0.1");
            Assert.That(method.Interpolate(0.4), NumericIs.AlmostEqualTo(-1.0285714285714285714, 1e-15), "A 0.4");
            Assert.That(method.Interpolate(1.2), NumericIs.AlmostEqualTo(.30285714285714285716, 1e-15), "A 1.2");
            Assert.That(method.Interpolate(10.0), NumericIs.AlmostEqualTo((double) 189, 1e-15), "A 10.0");
            Assert.That(method.Interpolate(-10.0), NumericIs.AlmostEqualTo((double) 677, 1e-15), "A -10.0");

            // Test Linear Case
            for(int k = 2; k < 6; k++)
            {
                double[] linx, liny, linxtest, linytest;
                BuildLinearCase(2, k + 1, out linx, out liny, out linxtest, out linytest);
                IInterpolationMethod linearMethod = Interpolation.CreateNaturalCubicSpline(linx, liny);
                for(int i = 0; i < linxtest.Length; i++)
                {
                    Assert.That(linearMethod.Interpolate(linxtest[i]), NumericIs.AlmostEqualTo(linytest[i], 1e-12), String.Format("Linear k={0} i={1}", k, i));
                }
            }
        }

        [Test]
        public void TestInterpolationMethod_CubicSpline_BoundaryFirstDerivativeFixed()
        {
            double[] t = new double[] { -2.0, -1.0, 0.0, 1.0, 2.0 };
            double[] x = new double[] { 1.0, 2.0, -1.0, 0.0, 1.0 };

            IInterpolationMethod method = Interpolation.CreateCubicSpline(t, x, SplineBoundaryCondition.FirstDerivative, 1.0, SplineBoundaryCondition.FirstDerivative, -1.0);
            Assert.That(method, Is.TypeOf(typeof(CubicSplineInterpolation)), "Type");

            for(int i = 0; i < t.Length; i++)
            {
                // verify the interpolated values exactly at the sample points.
                Assert.That(method.Interpolate(t[i]), Is.EqualTo(x[i]), "A Exact Point " + i.ToString());
            }

            // Maple: "with(CurveFitting);"
            // Maple: "evalf(subs({x=-2.4},Spline([[-2,1],[-1,2],[0,-1],[1,0],[2,1]], x, degree=3, endpoints=[1,-1])),20);"
            Assert.That(method.Interpolate(-2.4), NumericIs.AlmostEqualTo(1.120000000000000001, 1e-15), "A -2.4");
            Assert.That(method.Interpolate(-0.9), NumericIs.AlmostEqualTo(1.8243928571428571428, 1e-15), "A -0.9");
            Assert.That(method.Interpolate(-0.5), NumericIs.AlmostEqualTo(.54910714285714285715, 1e-15), "A -0.5");
            Assert.That(method.Interpolate(-0.1), NumericIs.AlmostEqualTo(-.78903571428571428572, 1e-15), "A -0.1");
            Assert.That(method.Interpolate(0.1), NumericIs.AlmostEqualTo(-1.1304642857142857143, 1e-15), "A 0.1");
            Assert.That(method.Interpolate(0.4), NumericIs.AlmostEqualTo(-1.1040000000000000000, 1e-15), "A 0.4");
            Assert.That(method.Interpolate(1.2), NumericIs.AlmostEqualTo(.4148571428571428571, 1e-15), "A 1.2");
            Assert.That(method.Interpolate(10.0), NumericIs.AlmostEqualTo(-608.14285714285714286, 1e-15), "A 10.0");
            Assert.That(method.Interpolate(-10.0), NumericIs.AlmostEqualTo(1330.1428571428571429, 1e-15), "A -10.0");
        }

        [Test]
        public void TestInterpolationMethod_CubicSpline_BoundarySecondDerivativeFixed()
        {
            double[] t = new double[] { -2.0, -1.0, 0.0, 1.0, 2.0 };
            double[] x = new double[] { 1.0, 2.0, -1.0, 0.0, 1.0 };

            IInterpolationMethod method = Interpolation.CreateCubicSpline(t, x, SplineBoundaryCondition.SecondDerivative, -5.0, SplineBoundaryCondition.SecondDerivative, -1.0);
            Assert.That(method, Is.TypeOf(typeof(CubicSplineInterpolation)), "Type");

            for(int i = 0; i < t.Length; i++)
            {
                // verify the interpolated values exactly at the sample points.
                Assert.That(method.Interpolate(t[i]), Is.EqualTo(x[i]), "A Exact Point " + i.ToString());
            }

            // Maple: "with(CurveFitting);"
            // Maple: "evalf(subs({x=-2.4},Spline([[-2,1],[-1,2],[0,-1],[1,0],[2,1]], x, degree=3, endpoints=Matrix(2,13,{(1,3)=1,(1,13)=-5,(2,10)=1,(2,13)=-1}))),20);"
            Assert.That(method.Interpolate(-2.4), NumericIs.AlmostEqualTo(-.8999999999999999993, 1e-15), "A -2.4");
            Assert.That(method.Interpolate(-0.9), NumericIs.AlmostEqualTo(1.7590357142857142857, 1e-15), "A -0.9");
            Assert.That(method.Interpolate(-0.5), NumericIs.AlmostEqualTo(.41517857142857142854, 1e-15), "A -0.5");
            Assert.That(method.Interpolate(-0.1), NumericIs.AlmostEqualTo(-.82010714285714285714, 1e-15), "A -0.1");
            Assert.That(method.Interpolate(0.1), NumericIs.AlmostEqualTo(-1.1026071428571428572, 1e-15), "A 0.1");
            Assert.That(method.Interpolate(0.4), NumericIs.AlmostEqualTo(-1.0211428571428571429, 1e-15), "A 0.4");
            Assert.That(method.Interpolate(1.2), NumericIs.AlmostEqualTo(.31771428571428571421, 1e-15), "A 1.2");
            Assert.That(method.Interpolate(10.0), NumericIs.AlmostEqualTo((double) 39, 1e-14), "A 10.0");
            Assert.That(method.Interpolate(-10.0), NumericIs.AlmostEqualTo((double) (-37), 1e-14), "A -10.0");
        }

        [Test]
        public void TestInterpolationMethod_AkimaSpline()
        {
            double[] t = new double[] { -2.0, -1.0, 0.0, 1.0, 2.0 };
            double[] x = new double[] { 1.0, 2.0, -1.0, 0.0, 1.0 };

            IInterpolationMethod method = Interpolation.CreateAkimaCubicSpline(t, x);
            Assert.That(method, Is.TypeOf(typeof(AkimaSplineInterpolation)), "Type");

            for(int i = 0; i < t.Length; i++)
            {
                // verify the interpolated values exactly at the sample points.
                Assert.That(method.Interpolate(t[i]), Is.EqualTo(x[i]), "A Exact Point " + i.ToString());
            }

            // TODO: Verify the expected values (that they are really the expected ones)
            Assert.That(method.Interpolate(-2.4), NumericIs.AlmostEqualTo(-0.52, 1e-15), "A -2.4");
            Assert.That(method.Interpolate(-0.9), NumericIs.AlmostEqualTo(1.826, 1e-15), "A -0.9");
            Assert.That(method.Interpolate(-0.5), NumericIs.AlmostEqualTo(0.25, 1e-15), "A -0.5");
            Assert.That(method.Interpolate(-0.1), NumericIs.AlmostEqualTo(-1.006, 1e-15), "A -0.1");
            Assert.That(method.Interpolate(0.1), NumericIs.AlmostEqualTo(-0.9, 1e-15), "A 0.1");
            Assert.That(method.Interpolate(0.4), NumericIs.AlmostEqualTo(-0.6, 1e-15), "A 0.4");
            Assert.That(method.Interpolate(1.2), NumericIs.AlmostEqualTo(0.2, 1e-15), "A 1.2");
            Assert.That(method.Interpolate(10.0), NumericIs.AlmostEqualTo((double) 9, 1e-14), "A 10.0");
            Assert.That(method.Interpolate(-10.0), NumericIs.AlmostEqualTo((double) (-151), 1e-14), "A -10.0");

            // Test Linear Case
            for(int k = 2; k < 6; k++)
            {
                double[] linx, liny, linxtest, linytest;
                BuildLinearCase(2, k + 4, out linx, out liny, out linxtest, out linytest);
                IInterpolationMethod linearMethod = Interpolation.CreateAkimaCubicSpline(linx, liny);
                for(int i = 0; i < linxtest.Length; i++)
                {
                    Assert.That(linearMethod.Interpolate(linxtest[i]), NumericIs.AlmostEqualTo(linytest[i], 1e-12), String.Format("Linear k={0} i={1}", k, i));
                }
            }
        }

        static void BuildLinearCase(int start, int stop, out double[] x, out double[] y, out double[] xtest, out double[] ytest)
        {
            const double yOffset = 2.0;
            int samples = stop - start + 1;
            ContinuousUniformDistribution uniform = new ContinuousUniformDistribution();

            // build linear samples
            x = new double[samples];
            y = new double[samples];
            for(int i = 0; i < x.Length; i++)
            {
                int z = start + i;
                x[i] = z;
                y[i] = z + yOffset; // arbitrary small y-axis offset
            }

            // build linear test vectors randomly between the sample points
            xtest = new double[samples+1];
            ytest = new double[samples+1];
            if(samples == 1)
            {
                xtest[0] = start - uniform.NextDouble();
                xtest[1] = start + uniform.NextDouble();
                ytest[0] = ytest[1] = start + yOffset;
            }
            else
            {
                for(int i = 0; i < xtest.Length; i++)
                {
                    double z = (i - 1) + uniform.NextDouble();
                    xtest[i] = z;
                    ytest[i] = z + yOffset;
                }
            }
        }
    }
}
