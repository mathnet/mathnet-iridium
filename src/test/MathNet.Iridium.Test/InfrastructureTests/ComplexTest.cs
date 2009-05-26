//-----------------------------------------------------------------------
// <copyright file="ComplexTest.cs" company="Math.NET Project">
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
using System.Globalization;
using NUnit.Framework;

namespace Iridium.Test.InfrastructureTests
{
    using MathNet.Numerics;

    [TestFixture]
    public class ComplexTest
    {
        [Test]
        public void TestParser_Invariant()
        {
            // Invariant Culture
            Complex c1 = Complex.Parse("1231.2", NumberFormatInfo.InvariantInfo);
            Assert.That(c1.Real, Is.EqualTo(1231.2d), "A1");
            Assert.That(c1.Imag, Is.EqualTo(0.0d), "A2");

            Complex c2 = Complex.Parse("1.5-I*34.56", NumberFormatInfo.InvariantInfo);
            Assert.That(c2.Real, Is.EqualTo(1.5d), "B1");
            Assert.That(c2.Imag, Is.EqualTo(-34.56), "B2");

            Complex c3 = Complex.Parse("-1.5 + I * -34.56", NumberFormatInfo.InvariantInfo);
            Assert.That(c3.Real, Is.EqualTo(-1.5d), "C1");
            Assert.That(c3.Imag, Is.EqualTo(-34.56), "C2");

            Complex c4 = Complex.Parse("-34.56 * I", NumberFormatInfo.InvariantInfo);
            Assert.That(c4.Real, Is.EqualTo(0.0d), "D1");
            Assert.That(c4.Imag, Is.EqualTo(-34.56), "D2");
        }

        [Test]
        public void TestParser_DE_CH()
        {
            // Swiss (German) Culture
            NumberFormatInfo format = CultureInfo.GetCultureInfo("de-CH").NumberFormat;
            Complex c1 = Complex.Parse("1'231.2", format);
            Assert.That(c1.Real, Is.EqualTo(1231.2d), "A1");
            Assert.That(c1.Imag, Is.EqualTo(0.0d), "A2");
        }

        [Test]
        public void TestParser_DE_DE()
        {
            // German Culture
            NumberFormatInfo format = CultureInfo.GetCultureInfo("de-DE").NumberFormat;
            Complex c1 = Complex.Parse("1.231,2", format);
            Assert.That(c1.Real, Is.EqualTo(1231.2d), "A1");
            Assert.That(c1.Imag, Is.EqualTo(0.0d), "A2");
        }

        [Test]
        public void TestParser_TH_TH() 
        {
            // Thai Culture
            NumberFormatInfo format = CultureInfo.GetCultureInfo("th-TH").NumberFormat;
            string number = 1231.2.ToString(format);
            Complex c1 = Complex.Parse(number, format);
            Assert.That(c1.Real, Is.EqualTo(1231.2d), "A1");
            Assert.That(c1.Imag, Is.EqualTo(0.0d), "A2");
        }

        [Test]
        public void TestNaturalLogarithm()
        {
            // ln(0) = -infty
            Complex zero = Complex.Zero;
            Complex lnZero = zero.NaturalLogarithm();
            Assert.That(lnZero.Real, Is.EqualTo(double.NegativeInfinity), "Re{ln(0)} = -infinity");
            Assert.That(lnZero.Imag, Is.EqualTo(0d), "Im{ln(0)} = 0");

            // ln(1) = 0
            Complex one = Complex.One;
            Complex lnOne = one.NaturalLogarithm();
            Assert.That(lnOne.Real, Is.EqualTo(0d), "Re{ln(1)} = 0");
            Assert.That(lnOne.Imag, Is.EqualTo(0d), "Im{ln(1)} = 0");

            // ln(i) = Pi/2 * i
            Complex I = Complex.I;
            Complex lnI = I.NaturalLogarithm();
            Assert.That(lnI.Real, Is.EqualTo(0d), "Re{ln(i)} = 0");
            Assert.That(lnI.Imag, Is.EqualTo(Constants.Pi_2), "Im{ln(i)} = Pi/2");

            // ln(-1) = Pi * i
            Complex mOne = -Complex.One;
            Complex lnMOne = mOne.NaturalLogarithm();
            Assert.That(lnMOne.Real, Is.EqualTo(0d), "Re{ln(-1)} = 0");
            Assert.That(lnMOne.Imag, Is.EqualTo(Constants.Pi), "Im{ln(-1)} = Pi");

            // ln(-i) = -Pi/2 * i
            Complex mI = -Complex.I;
            Complex lnMI = mI.NaturalLogarithm();
            Assert.That(lnMI.Real, Is.EqualTo(0d), "Re{ln(-i)} = 0");
            Assert.That(lnMI.Imag, Is.EqualTo(-Constants.Pi_2), "Im{ln(-i)} = -Pi/2");

            // ln(i+1) = ln(2)/2 + Pi/4 * i
            Complex onePlusI = Complex.One + Complex.I;
            Complex lnOnePlusI = onePlusI.NaturalLogarithm();
            Assert.That(lnOnePlusI.Real, Is.EqualTo(Constants.Ln2 * 0.5), "Re{ln(i+1)} = ln(2)/2");
            Assert.That(lnOnePlusI.Imag, Is.EqualTo(Constants.Pi_4), "Im{ln(i+1)} = Pi/4");
        }

        [Test]
        public void TestExponential()
        {
            // exp(0) = 1
            Complex zero = Complex.Zero;
            Complex expZero = zero.Exponential();
            Assert.That(expZero.Real, Is.EqualTo(1d), "Re{exp(0)} = 1");
            Assert.That(expZero.Imag, Is.EqualTo(0d), "Im{exp(0)} = 0");

            // exp(1) = e
            Complex one = Complex.One;
            Complex expOne = one.Exponential();
            Assert.That(expOne.Real, Is.EqualTo(Constants.E), "Re{exp(1)} = e");
            Assert.That(expOne.Imag, Is.EqualTo(0d), "Im{exp(1)} = 0");

            // exp(i) = cos(1) + sin(1) * i
            Complex I = Complex.I;
            Complex expI = I.Exponential();
            Assert.That(expI.Real, Is.EqualTo(Trig.Cosine(1d)), "Re{exp(i)} = cos(1)");
            Assert.That(expI.Imag, Is.EqualTo(Trig.Sine(1d)), "Im{exp(i)} = sin(1)");

            // exp(-1) = 1/e
            Complex mOne = -Complex.One;
            Complex expMOne = mOne.Exponential();
            Assert.That(expMOne.Real, Is.EqualTo(1.0 / Constants.E), "Re{exp(-1)} = 1/e");
            Assert.That(expMOne.Imag, Is.EqualTo(0d), "Im{exp(-1)} = 0");

            // exp(-i) = cos(1) - sin(1) * i
            Complex mI = -Complex.I;
            Complex expMI = mI.Exponential();
            Assert.That(expMI.Real, Is.EqualTo(Trig.Cosine(1d)), "Re{exp(-i)} = cos(1)");
            Assert.That(expMI.Imag, Is.EqualTo(-Trig.Sine(1d)), "Im{exp(-i)} = -sin(1)");

            // exp(i+1) = e * cos(1) + e * sin(1) * i
            Complex onePlusI = Complex.One + Complex.I;
            Complex expOnePlusI = onePlusI.Exponential();
            Assert.That(expOnePlusI.Real, Is.EqualTo(Constants.E * Trig.Cosine(1d)), "Re{exp(i+1)} = e * cos(1)");
            Assert.That(expOnePlusI.Imag, Is.EqualTo(Constants.E * Trig.Sine(1d)), "Im{exp(i+1)} = e * sin(1)");
        }

        [Test]
        public void TestPower()
        {
            // (1)^(1) = 1
            Complex one = Complex.One;
            Complex onePowOne = one.Power(one);
            Assert.That(onePowOne.Real, Is.EqualTo(1d), "Re{(1)^(1)} = 1");
            Assert.That(onePowOne.Imag, Is.EqualTo(0d), "Im{(1)^(1)} = 0");

            // (i)^(1) = i
            Complex I = Complex.I;
            Complex IPowOne = I.Power(one);
            Assert.That(IPowOne.Real, NumericIs.AlmostEqualTo(0d), "Re{(i)^(1)} = 0");
            Assert.That(IPowOne.Imag, Is.EqualTo(1d), "Im{(i)^(1)} = 1");

            // (1)^(-1) = 1
            Complex mOne = -Complex.One;
            Complex onePowMOne = one.Power(mOne);
            Assert.That(onePowMOne.Real, Is.EqualTo(1d), "Re{(1)^(-1)} = 1");
            Assert.That(onePowMOne.Imag, Is.EqualTo(0d), "Im{(1)^(-1)} = 0");

            // (i)^(-1) = -i
            Complex IPowMOne = I.Power(mOne);
            Assert.That(IPowMOne.Real, NumericIs.AlmostEqualTo(0d), "Re{(i)^(-1)} = 0");
            Assert.That(IPowMOne.Imag, Is.EqualTo(-1d), "Im{(i)^(-1)} = -1");

            // (i)^(-i) = exp(Pi/2)
            Complex mI = -Complex.I;
            Complex IPowMI = I.Power(mI);
            Assert.That(IPowMI.Real, Is.EqualTo(Math.Exp(Constants.Pi_2)), "Re{(i)^(-i)} = exp(Pi/2)");
            Assert.That(IPowMI.Imag, Is.EqualTo(0d), "Im{(i)^(-i)} = 0");

            // (0)^(0) = 1
            Assert.That(Math.Pow(0d, 0d), Is.EqualTo(1d), "(0)^(0) = 1 (.Net Framework Sanity Check)");
            Complex zero = Complex.Zero;
            Complex zeroPowZero = zero.Power(zero);
            Assert.That(zeroPowZero.Real, Is.EqualTo(1d), "Re{(0)^(0)} = 1");
            Assert.That(zeroPowZero.Imag, Is.EqualTo(0d), "Im{(0)^(0)} = 0");

            // (0)^(2) = 0
            Assert.That(Math.Pow(0d, 2d), Is.EqualTo(0d), "(0)^(2) = 0 (.Net Framework Sanity Check)");
            Complex two = new Complex(2d, 0d);
            Complex zeroPowTwo = zero.Power(two);
            Assert.That(zeroPowTwo.Real, Is.EqualTo(0d), "Re{(0)^(2)} = 0");
            Assert.That(zeroPowTwo.Imag, Is.EqualTo(0d), "Im{(0)^(2)} = 0");

            // (0)^(-2) = infty
            Assert.That(Math.Pow(0d, -2d), Is.EqualTo(double.PositiveInfinity), "(0)^(-2) = infty (.Net Framework Sanity Check)");
            Complex mTwo = Complex.FromRealImaginary(-2d, 0d);
            Complex zeroPowMTwo = zero.Power(mTwo);
            Assert.That(zeroPowMTwo.Real, Is.EqualTo(double.PositiveInfinity), "Re{(0)^(-2)} = infty");
            Assert.That(zeroPowMTwo.Imag, Is.EqualTo(0d), "Im{(0)^(-2)} = 0");

            // (0)^(I) = NaN
            Complex zeroPowI = zero.Power(I);
            Assert.That(zeroPowI.Real, Is.NaN, "Re{(0)^(I)} = NaN");
            Assert.That(zeroPowI.Imag, Is.NaN, "Im{(0)^(I)} = NaN");

            // (0)^(-I) = NaN
            Complex zeroPowMI = zero.Power(mI);
            Assert.That(zeroPowMI.Real, Is.NaN, "Re{(0)^(-I)} = NaN");
            Assert.That(zeroPowMI.Imag, Is.NaN, "Im{(0)^(-I)} = NaN");

            // (0)^(1+I) = 0
            Complex onePlusI = Complex.One + Complex.I;
            Complex zeroPowOnePlusI = zero.Power(onePlusI);
            Assert.That(zeroPowOnePlusI.Real, Is.EqualTo(0d), "Re{(0)^(1+I)} = 0");
            Assert.That(zeroPowOnePlusI.Imag, Is.EqualTo(0d), "Im{(0)^(1+I)} = 0");

            // (0)^(1-I) = 0
            Complex oneMinusI = Complex.One - Complex.I;
            Complex zeroPowOneMinusI = zero.Power(oneMinusI);
            Assert.That(zeroPowOneMinusI.Real, Is.EqualTo(0d), "Re{(0)^(1-I)} = 0");
            Assert.That(zeroPowOneMinusI.Imag, Is.EqualTo(0d), "Im{(0)^(1-I)} = 0");

            // (0)^(-1+I) = infty + infty * i
            Complex minusOnePlusI = new Complex(-1d, 1d);
            Complex zeroPowMinusOnePlusI = zero.Power(minusOnePlusI);
            Assert.That(zeroPowMinusOnePlusI.Real, Is.EqualTo(double.PositiveInfinity), "Re{(0)^(-1+I)} = infty");
            Assert.That(zeroPowMinusOnePlusI.Imag, Is.EqualTo(double.PositiveInfinity), "Im{(0)^(-1+I)} = infty");
        }

        [Test]
        public void TestDivision()
        {
            // 0/1
            Complex zeroDivOne = Complex.Zero / Complex.One;
            Assert.That(zeroDivOne.Real, Is.EqualTo(0), "Re{0/1} = 0");
            Assert.That(zeroDivOne.Imag, Is.EqualTo(0), "Im{0/1} = 0");

            // 1/0
            // TODO: verify this is really what should happen
            Complex oneDivZero = Complex.One / Complex.Zero;
            Assert.That(oneDivZero.Real, Is.EqualTo(double.PositiveInfinity), "Re{1/0} = infty");
            Assert.That(oneDivZero.Imag, Is.EqualTo(double.PositiveInfinity), "Im{1/0} = infty");

            // (1+2I)/(3+4I)
            Complex onePlus2I = new Complex(1, 2);
            Complex threePlus4I = new Complex(3, 4);
            Complex onPlus2IDivthreePlus4I = onePlus2I / threePlus4I;
            Assert.That(onPlus2IDivthreePlus4I.Real, Is.EqualTo(11d / 25d), "Re{(1+2I)/(3+4I)} = 11/25");
            Assert.That(onPlus2IDivthreePlus4I.Imag, Is.EqualTo(2d / 25d), "Im{(1+2I)/(3+4I)} = 2/25");

            // (big+big*I)/(2*big+2*big*I)
            double big1 = double.MaxValue / 4;
            double big2 = double.MaxValue / 2;
            Complex big1PlusBig1I = new Complex(big1, big1);
            Complex big2PlusBig2I = new Complex(big2, big2);
            Complex big1PlusBig1IDivBig2PlusBig2I = big1PlusBig1I / big2PlusBig2I;
            Assert.That(big1PlusBig1IDivBig2PlusBig2I.Real, Is.EqualTo(0.5), "Re{(big+big*I)/(2*big+2*big*I)} = 0.5");
            Assert.That(big1PlusBig1IDivBig2PlusBig2I.Imag, Is.EqualTo(0), "Im{((big+big*I)/(2*big+2*big*I)} = 0");
        }
    }
}
