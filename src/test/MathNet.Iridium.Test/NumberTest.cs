//-----------------------------------------------------------------------
// <copyright file="NumberTest.cs" company="Math.NET Project">
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
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using MathNet.Numerics;

namespace Iridium.Test
{
    [TestFixture]
    public class NumberTest
    {
        [Test]
        public void TestIncrementDecrementAtZero()
        {
            double x = 2 * double.Epsilon;
            Assert.That(x, Is.EqualTo(2 * double.Epsilon), "A");
            x = Number.Decrement(x);
            x = Number.Decrement(x);
            Assert.That(x, Is.EqualTo(0), "B");
            x = Number.Decrement(x);
            x = Number.Decrement(x);
            Assert.That(x, Is.EqualTo(-2 * double.Epsilon), "C");
            x = Number.Increment(x);
            x = Number.Increment(x);
            Assert.That(x, Is.EqualTo(0), "D");
            x = Number.Increment(x);
            x = Number.Increment(x);
            Assert.That(x, Is.EqualTo(2 * double.Epsilon), "E");
        }

        [Test]
        public void TestIncrementDecrementAtMinMax()
        {
            double x = double.MaxValue;
            Assert.That(x, Is.EqualTo(double.MaxValue), "A");
            x = Number.Decrement(x);
            Assert.That(x, Is.LessThan(double.MaxValue), "B");
            x = Number.Increment(x);
            Assert.That(x, Is.EqualTo(double.MaxValue), "C");
            x = Number.Increment(x);
            Assert.That(x, Is.EqualTo(double.PositiveInfinity), "D");

            x = double.MinValue;
            Assert.That(x, Is.EqualTo(double.MinValue), "E");
            x = Number.Increment(x);
            Assert.That(x, Is.GreaterThan(double.MinValue), "F");
            x = Number.Decrement(x);
            Assert.That(x, Is.EqualTo(double.MinValue), "G");
            x = Number.Decrement(x);
            Assert.That(x, Is.EqualTo(double.NegativeInfinity), "H");
        }

        [Test]
        public void TestIncrementDecrementStep()
        {
            double x0 = 1e-100;
            double x1 = 1e+0;
            double x2 = 1e+100;
            double x3 = 1e+200;
            double x4 = -1e+100;

            double y0 = Number.Increment(x0) - x0;
            double y1 = Number.Increment(x1) - x1;
            double y2 = Number.Increment(x2) - x2;
            double y3 = Number.Increment(x3) - x3;
            double y4 = Number.Increment(x4) - x4;

            Assert.That(y0 < y1, "A");
            Assert.That(y1 < y2, "B");
            Assert.That(y2 < y3, "C");
            Assert.That(y2 == y4, "D");
         }

        [Test]
        public void TestEpsilonOf()
        {
            IFormatProvider format = System.Globalization.CultureInfo.InvariantCulture;
            Assert.That(Number.EpsilonOf(1.0).ToString(format), Is.EqualTo("1.11022302462516E-16"), "A");
            Assert.That(Number.RelativeAccuracy.ToString(format), Is.EqualTo("1.11022302462516E-16"), "A2");
            Assert.That(Number.PositiveRelativeAccuracy.ToString(format), Is.EqualTo("2.22044604925031E-16"), "A3");
            Assert.That(Number.EpsilonOf(-1.0).ToString(format), Is.EqualTo("1.11022302462516E-16"), "B");
            Assert.That(Number.EpsilonOf(0.0).ToString(format), Is.EqualTo("4.94065645841247E-324"), "C");
            Assert.That(Number.EpsilonOf(1.0e+100).ToString(format), Is.EqualTo("1.94266889222573E+84"), "D");
            Assert.That(Number.EpsilonOf(-1.0e+100).ToString(format), Is.EqualTo("1.94266889222573E+84"), "E");
            Assert.That(Number.EpsilonOf(1.0e-100).ToString(format), Is.EqualTo("1.26897091865782E-116"), "F");
            Assert.That(Number.EpsilonOf(-1.0e-100).ToString(format), Is.EqualTo("1.26897091865782E-116"), "G");
            Assert.That(Number.EpsilonOf(double.MaxValue).ToString(format), Is.EqualTo("1.99584030953472E+292"), "H");
            Assert.That(Number.EpsilonOf(double.MinValue).ToString(format), Is.EqualTo("1.99584030953472E+292"), "I");
            Assert.That(Number.EpsilonOf(double.Epsilon).ToString(format), Is.EqualTo("4.94065645841247E-324"), "J");
            Assert.That(Number.EpsilonOf(-double.Epsilon).ToString(format), Is.EqualTo("4.94065645841247E-324"), "K");
            Assert.That(Number.EpsilonOf(double.NaN), Is.NaN, "L");
            Assert.That(Number.EpsilonOf(double.PositiveInfinity), Is.NaN, "M");
            Assert.That(Number.EpsilonOf(double.NegativeInfinity), Is.NaN, "N");
        }

        [Test]
        public void TestLexicographicalOrder()
        {
            Assert.That(Number.ToLexicographicalOrderedUInt64(2 * double.Epsilon), Is.EqualTo(2), "A");
            Assert.That(Number.ToLexicographicalOrderedUInt64(1 * double.Epsilon), Is.EqualTo(1), "B");
            Assert.That(Number.ToLexicographicalOrderedUInt64(0.0), Is.EqualTo(0), "C");
            Assert.That(Number.ToLexicographicalOrderedUInt64(-1 * double.Epsilon), Is.EqualTo(0xFFFFFFFFFFFFFFFF), "D");
            Assert.That(Number.ToLexicographicalOrderedUInt64(-2 * double.Epsilon), Is.EqualTo(0xFFFFFFFFFFFFFFFE), "E");

            Assert.That(Number.ToLexicographicalOrderedInt64(2 * double.Epsilon), Is.EqualTo(2), "N");
            Assert.That(Number.ToLexicographicalOrderedInt64(1 * double.Epsilon), Is.EqualTo(1), "O");
            Assert.That(Number.ToLexicographicalOrderedInt64(0.0), Is.EqualTo(0), "P");
            Assert.That(Number.ToLexicographicalOrderedInt64(-1 * double.Epsilon), Is.EqualTo(-1), "Q");
            Assert.That(Number.ToLexicographicalOrderedInt64(-2 * double.Epsilon), Is.EqualTo(-2), "R");
        }

        [Test]
        public void TestSignedMagnitudeToTwosComplement()
        {
            Assert.That(Number.SignedMagnitudeToTwosComplementUInt64(2), Is.EqualTo(2), "A");
            Assert.That(Number.SignedMagnitudeToTwosComplementUInt64(1), Is.EqualTo(1), "B");
            Assert.That(Number.SignedMagnitudeToTwosComplementUInt64(0), Is.EqualTo(0), "C");
            Assert.That(Number.SignedMagnitudeToTwosComplementUInt64(-9223372036854775808), Is.EqualTo(0), "D");
            Assert.That(Number.SignedMagnitudeToTwosComplementUInt64(-9223372036854775808 + 1), Is.EqualTo(0xFFFFFFFFFFFFFFFF), "E");
            Assert.That(Number.SignedMagnitudeToTwosComplementUInt64(-9223372036854775808 + 2), Is.EqualTo(0xFFFFFFFFFFFFFFFE), "F");

            Assert.That(Number.SignedMagnitudeToTwosComplementInt64(2), Is.EqualTo(2), "M");
            Assert.That(Number.SignedMagnitudeToTwosComplementInt64(1), Is.EqualTo(1), "O");
            Assert.That(Number.SignedMagnitudeToTwosComplementInt64(0), Is.EqualTo(0), "P");
            Assert.That(Number.SignedMagnitudeToTwosComplementInt64(-9223372036854775808), Is.EqualTo(0), "Q");
            Assert.That(Number.SignedMagnitudeToTwosComplementInt64(-9223372036854775808 + 1), Is.EqualTo(-1), "R");
            Assert.That(Number.SignedMagnitudeToTwosComplementInt64(-9223372036854775808 + 2), Is.EqualTo(-2), "S");
        }

        [Test]
        public void TestNumbersBetween()
        {
            Assert.That(Number.NumbersBetween(1.0, 1.0), Is.EqualTo(0), "A");
            Assert.That(Number.NumbersBetween(0, 0), Is.EqualTo(0), "B");
            Assert.That(Number.NumbersBetween(-1.0, -1.0), Is.EqualTo(0), "C");
            Assert.That(Number.NumbersBetween(0, double.Epsilon), Is.EqualTo(1), "D");
            Assert.That(Number.NumbersBetween(0, -double.Epsilon), Is.EqualTo(1), "E");
            Assert.That(Number.NumbersBetween(double.Epsilon, 0), Is.EqualTo(1), "D2");
            Assert.That(Number.NumbersBetween(-double.Epsilon, 0), Is.EqualTo(1), "E2");
            Assert.That(Number.NumbersBetween(0, 2*double.Epsilon), Is.EqualTo(2), "F");
            Assert.That(Number.NumbersBetween(0, -2 * double.Epsilon), Is.EqualTo(2), "G");
            Assert.That(Number.NumbersBetween(-double.Epsilon, 2 * double.Epsilon), Is.EqualTo(3), "H");
            Assert.That(Number.NumbersBetween(double.Epsilon, -2 * double.Epsilon), Is.EqualTo(3), "I");

            double test = Math.PI * 1e+150;
            Assert.That(Number.NumbersBetween(test, test + (10 * Number.EpsilonOf(test))), Is.EqualTo(10), "J");
            Assert.That(Number.NumbersBetween(test, test - (10 * Number.EpsilonOf(test))), Is.EqualTo(10), "K");

            Assert.That(Number.NumbersBetween(1.0001, 1.0002), Is.EqualTo(450359962737), "L");
            Assert.That(Number.NumbersBetween(10000.0001, 10000.0002), Is.EqualTo(54975582), "M");
            Assert.That(Number.NumbersBetween(10000000.0001, 10000000.0002), Is.EqualTo(53687), "N");
            Assert.That(Number.NumbersBetween(10000000000.0001, 10000000000.0002), Is.EqualTo(53), "O");

            Assert.That(Number.NumbersBetween(double.MinValue, double.MaxValue), Is.EqualTo(0xFFDFFFFFFFFFFFFE), "R");
        }

        [Test]
        public void TestAlmostEqual()
        {
            IFormatProvider format = System.Globalization.CultureInfo.InvariantCulture;

            double max = double.MaxValue;
            double min = double.MinValue;

            Assert.That(Number.AlmostEqual(0.0, 0.0, 0), "A");
            Assert.That(Number.AlmostEqual(0.0, 0.0, 50), "B");
            Assert.That(Number.AlmostEqual(max, max, 0), "C");
            Assert.That(Number.AlmostEqual(min, min, 0), "D");

            Assert.That(!Number.AlmostEqual(0.0, 0.0 + double.Epsilon, 0), "E");
            Assert.That(Number.AlmostEqual(0.0, 0.0 + double.Epsilon, 1), "F");

            Assert.That(!Number.AlmostEqual(max, max - (2 * Number.EpsilonOf(max)), 0), "G");
            Assert.That(!Number.AlmostEqual(max, max - (2 * Number.EpsilonOf(max)), 1), "H");
            Assert.That(Number.AlmostEqual(max, max - (2 * Number.EpsilonOf(max)), 2), "I");

            Assert.That(Convert.ToDouble("3.170404", format) == 3.170404, "J");
            Assert.That(Convert.ToDouble("4.170404", format) != 4.170404, "K");

            Assert.That(Number.AlmostEqual(Convert.ToDouble("3.170404", format), 3.170404, 0), "L");
            Assert.That(!Number.AlmostEqual(Convert.ToDouble("4.170404", format), 4.170404, 0), "M");
            Assert.That(Number.AlmostEqual(Convert.ToDouble("4.170404", format), 4.170404, 1), "N");

            Assert.That(!Number.AlmostEqual(double.NaN, double.NaN, 25), "O");
            Assert.That(!Number.AlmostEqual(double.PositiveInfinity, double.NegativeInfinity, 25), "P");
            Assert.That(Number.AlmostEqual(double.PositiveInfinity, double.PositiveInfinity, 25), "Q");
        }

        [Test]
        public void TestCoerceZero()
        {
            Assert.That(Number.CoerceZero(0d), Is.EqualTo(0.0), "A1");
            Assert.That(Number.CoerceZero(Number.Increment(0.0)), Is.EqualTo(0), "A2");
            Assert.That(Number.CoerceZero(Number.Decrement(0.0)), Is.EqualTo(0), "A3");

            Assert.That(Number.CoerceZero(1d), Is.EqualTo(1.0), "B1");
            Assert.That(Number.CoerceZero(-1d), Is.EqualTo(-1.0), "B2");
            Assert.That(Number.CoerceZero(0.5d), Is.EqualTo(0.5), "B3");
            Assert.That(Number.CoerceZero(-0.5d), Is.EqualTo(-0.5), "B4");

            Assert.That(Number.CoerceZero(double.PositiveInfinity), Is.EqualTo(double.PositiveInfinity), "C1");
            Assert.That(Number.CoerceZero(double.NegativeInfinity), Is.EqualTo(double.NegativeInfinity), "C2");
            Assert.That(Number.CoerceZero(double.MaxValue), Is.EqualTo(double.MaxValue), "C3");
            Assert.That(Number.CoerceZero(double.MinValue), Is.EqualTo(double.MinValue), "C4");
            Assert.That(Number.CoerceZero(double.NaN), Is.NaN, "C5");

            Assert.That(Number.CoerceZero(double.Epsilon), Is.EqualTo(0.0), "D1");
            Assert.That(Number.CoerceZero(double.Epsilon * 1e3), Is.EqualTo(0.0), "D2");
            Assert.That(Number.CoerceZero(double.Epsilon * 1e6), Is.EqualTo(0.0), "D3");
            Assert.That(Number.CoerceZero(double.Epsilon * 1e200), Is.EqualTo(0.0), "D4");
            Assert.That(Number.CoerceZero(double.Epsilon), Is.EqualTo(0.0), "D5");
            Assert.That(Number.CoerceZero(double.Epsilon * -1e200), Is.EqualTo(0.0), "D6");

            Assert.That(Number.CoerceZero(Number.PositiveEpsilonOf(1e-5)), Is.EqualTo(0.0), "E1");
            Assert.That(Number.CoerceZero(Number.PositiveEpsilonOf(1)), Is.EqualTo(0.0), "E2");
            Assert.That(Number.CoerceZero(Number.PositiveEpsilonOf(1e+1)), Is.Not.EqualTo(0.0), "E3");
            Assert.That(Number.CoerceZero(Number.PositiveEpsilonOf(1e+5)), Is.Not.EqualTo(0.0), "E4");
            Assert.That(Number.CoerceZero(Number.PositiveEpsilonOf(-1)), Is.EqualTo(0.0), "E5");
            Assert.That(Number.CoerceZero(Number.PositiveEpsilonOf(-1e+1)), Is.Not.EqualTo(0.0), "E6");

            Assert.That(Number.CoerceZero(1e-15), Is.EqualTo(0.0), "F1");
            Assert.That(Number.CoerceZero(1e-14), Is.Not.EqualTo(0.0), "F2");
            Assert.That(Number.CoerceZero(-1e-15), Is.EqualTo(0.0), "F3");
            Assert.That(Number.CoerceZero(-1e-14), Is.Not.EqualTo(0.0), "F4");

            Assert.That(Number.CoerceZero(1e-6, 1e-5), Is.EqualTo(0.0), "G1");
            Assert.That(Number.CoerceZero(1e-4, 1e-5), Is.Not.EqualTo(0.0), "G2");
            Assert.That(Number.CoerceZero(1e-5, 1e-5), Is.Not.EqualTo(0.0), "G3");
            Assert.That(Number.CoerceZero(-1e-6, 1e-5), Is.EqualTo(0.0), "G4");
            Assert.That(Number.CoerceZero(-1e-4, 1e-5), Is.Not.EqualTo(0.0), "G5");
            Assert.That(Number.CoerceZero(-1e-5, 1e-5), Is.Not.EqualTo(0.0), "G6");

            Assert.That(Number.CoerceZero(1e+6, 1e+5), Is.Not.EqualTo(0.0), "H1");
            Assert.That(Number.CoerceZero(1e+4, 1e+5), Is.EqualTo(0.0), "H2");
            Assert.That(Number.CoerceZero(1e+5, 1e+5), Is.Not.EqualTo(0.0), "H3");
            Assert.That(Number.CoerceZero(-1e+6, 1e+5), Is.Not.EqualTo(0.0), "H4");
            Assert.That(Number.CoerceZero(-1e+4, 1e+5), Is.EqualTo(0.0), "H5");
            Assert.That(Number.CoerceZero(-1e+5, 1e+5), Is.Not.EqualTo(0.0), "H6");
        }
    }
}
