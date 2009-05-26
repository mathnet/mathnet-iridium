//-----------------------------------------------------------------------
// <copyright file="AlmostEqualityTest.cs" company="Math.NET Project">
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

namespace Iridium.Test.InfrastructureTests
{
    using MathNet.Numerics;
    using MathNet.Numerics.LinearAlgebra;
    using MathNet.Numerics.Distributions;

    [TestFixture]
    public class AlmostEqualityTest
    {
        [Test]
        public void TestFrameworkNullEquality()
        {
            object o1 = new object();
            object o2 = new object();

            Assert.That(object.ReferenceEquals(null, null), Is.True);
            Assert.That(object.ReferenceEquals(o1, o1), Is.True);

            Assert.That(object.ReferenceEquals(o1, null), Is.False);
            Assert.That(object.ReferenceEquals(o2, null), Is.False);
            Assert.That(object.ReferenceEquals(o1, o2), Is.False);

            Assert.That(object.Equals(null, null), Is.True);
            Assert.That(object.Equals(o1, o1), Is.True);

            Assert.That(object.Equals(o1, null), Is.False);
            Assert.That(object.Equals(o2, null), Is.False);
            Assert.That(object.Equals(o1, o2), Is.False);
        }

        [Test]
        public void TestFrameworkValueTypeEquality()
        {
            const int one1 = 1;
            const int one2 = 1;
            const int two = 2;

            // remember, boxing!
            Assert.That(object.ReferenceEquals(one1, one1), Is.False);
            Assert.That(object.ReferenceEquals(one1, one2), Is.False);
            Assert.That(object.ReferenceEquals(one1, two), Is.False);

            // Int32 overrides object.Equals which is called by the static Equals method,
            // so boxing does no harm here
            Assert.That(object.Equals(one1, one1), Is.True);
            Assert.That(object.Equals(one1, one2), Is.True);
            Assert.That(object.Equals(one1, two), Is.False);
        }

        [Test]
        public void TestAlmostEqual()
        {
            IFormatProvider format = System.Globalization.CultureInfo.InvariantCulture;

            const double max = double.MaxValue;
            const double min = double.MinValue;

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

        static void Helper_TestAlmostEqualityForGenericType<T>(
            T value,
            T valueClone,
            T completelyDifferentValue,
            T notablyDifferentValue,
            T almostEqualValue)
            where T : IAlmostEquatable<T>
        {
            // samle value
            Assert.That(value, Is.EqualTo(value));
            Assert.That(Number.AlmostEqual(value, value), Is.True);
            Assert.That(Number.AlmostEqual(value, value, 1e-10), Is.True);

            // default/null value
            Assert.That(value, Is.Not.EqualTo(default(T)));
            Assert.That(Number.AlmostEqual(value, default(T)), Is.False);
            Assert.That(Number.AlmostEqual(value, default(T), 1e-10), Is.False);

            // samle value
            Assert.That(value, Is.EqualTo(valueClone));
            Assert.That(value, Is.Not.SameAs(valueClone));
            Assert.That(Number.AlmostEqual(value, valueClone), Is.True);
            Assert.That(Number.AlmostEqual(value, valueClone, 1e-10), Is.True);

            // completely different value
            Assert.That(value, Is.Not.EqualTo(completelyDifferentValue));
            Assert.That(Number.AlmostEqual(value, completelyDifferentValue), Is.False);
            Assert.That(Number.AlmostEqual(value, completelyDifferentValue, 1e-10), Is.False);

            // notably different value
            Assert.That(value, Is.Not.EqualTo(notablyDifferentValue));
            Assert.That(Number.AlmostEqual(value, notablyDifferentValue), Is.False);
            Assert.That(Number.AlmostEqual(value, notablyDifferentValue, 1e-10), Is.False);
            Assert.That(Number.AlmostEqual(value, notablyDifferentValue, 1e-2), Is.True);

            // almost (but not) equal value
            Assert.That(value, Is.Not.EqualTo(almostEqualValue));
            Assert.That(Number.AlmostEqual(value, almostEqualValue), Is.True);
            Assert.That(Number.AlmostEqual(value, almostEqualValue, 1e-10), Is.True);
            Assert.That(Number.AlmostEqual(value, almostEqualValue, 1e-2), Is.True);
        }

        [Test]
        public void TestAlmostEquals_Complex()
        {
            Complex a1 = new Complex(1.0, 2.0);
            Complex a2 = new Complex(1.0, 2.0);
            Complex b = new Complex(2.0, 1.0);
            Complex c = new Complex(1.0 + 1e+10 * Number.PositiveEpsilonOf(1.0), 2.0);
            Complex d = new Complex(1.0 + 2 * Number.PositiveEpsilonOf(1.0), 2.0);

            Helper_TestAlmostEqualityForGenericType(a1, a2, b, c, d);

            // Wrapper
            Assert.That(Complex.AlmostEqual(a1, c), Is.False);
            Assert.That(Complex.AlmostEqual(a1, c, 1e-10), Is.False);
            Assert.That(Complex.AlmostEqual(a1, c, 1e-2), Is.True);

            // value type -> boxing
            Assert.That(a1, Is.Not.SameAs(a1));

            // infinity
            Assert.That(a1, Is.Not.EqualTo(Complex.Infinity));
            Assert.That(Number.AlmostEqual(a1, Complex.Infinity), Is.False);
            Assert.That(Number.AlmostEqual(a1, Complex.Infinity, 1e-10), Is.False);

            // NaN
            Assert.That(a1, Is.Not.EqualTo(Complex.NaN));
            Assert.That(Number.AlmostEqual(a1, Complex.NaN), Is.False);
            Assert.That(Number.AlmostEqual(a1, Complex.NaN, 1e-10), Is.False);
        }

        [Test,Repeat(5)]
        public void TestAlmostEquals_Matrix()
        {
            Matrix a1 = Matrix.Random(3, 2, new ContinuousUniformDistribution());
            Matrix a2 = a1.Clone();
            Matrix b = -a1;
            Matrix c = a1 * (1.0 + (1e+10 * Number.PositiveEpsilonOf(1.0)));
            Matrix d = a1 * (1.0 + (2 * Number.PositiveEpsilonOf(1.0)));

            Helper_TestAlmostEqualityForGenericType(a1, a2, b, c, d);

            // Wrapper
            Assert.That(Matrix.AlmostEqual(a1, c), Is.False);
            Assert.That(Matrix.AlmostEqual(a1, c, 1e-10), Is.False);
            Assert.That(Matrix.AlmostEqual(a1, c, 1e-2), Is.True);

            // reference type -> no boxing
            Assert.That(a1, Is.SameAs(a1));

            // transpose
            Assert.That(Matrix.Transpose(a1), Is.Not.EqualTo(a1));
            Assert.That(Number.AlmostEqual(a1, Matrix.Transpose(a1)), Is.False);
            Assert.That(Number.AlmostEqual(a1, Matrix.Transpose(a1), 1e-10), Is.False);
        }

        [Test,Repeat(5)]
        public void TestAlmostEquals_ComplexMatrix()
        {
            ComplexMatrix a1 = ComplexMatrix.Random(3, 2, new ContinuousUniformDistribution());
            ComplexMatrix a2 = a1.Clone();
            ComplexMatrix b = -a1;
            ComplexMatrix c = a1 * (1.0 + (1e+10 * Number.PositiveEpsilonOf(1.0)));
            ComplexMatrix d = a1 * (1.0 + (2 * Number.PositiveEpsilonOf(1.0)));

            Helper_TestAlmostEqualityForGenericType(a1, a2, b, c, d);

            // Wrapper
            Assert.That(ComplexMatrix.AlmostEqual(a1, c), Is.False);
            Assert.That(ComplexMatrix.AlmostEqual(a1, c, 1e-10), Is.False);
            Assert.That(ComplexMatrix.AlmostEqual(a1, c, 1e-2), Is.True);

            // reference type -> no boxing
            Assert.That(a1, Is.SameAs(a1));

            // transpose
            Assert.That(a1.Transpose(), Is.Not.EqualTo(a1));
            Assert.That(Number.AlmostEqual(a1, a1.Transpose()), Is.False);
            Assert.That(Number.AlmostEqual(a1, a1.Transpose(), 1e-10), Is.False);

            // hermitian transpose
            Assert.That(a1.HermitianTranspose(), Is.Not.EqualTo(a1));
            Assert.That(Number.AlmostEqual(a1, a1.HermitianTranspose()), Is.False);
            Assert.That(Number.AlmostEqual(a1, a1.HermitianTranspose(), 1e-10), Is.False);
        }

        [Test, Repeat(5)]
        public void TestAlmostEquals_Vector()
        {
            Vector a1 = Vector.Random(3, new ContinuousUniformDistribution());
            Vector a2 = a1.Clone();
            Vector b = -a1;
            Vector c = a1 * (1.0 + (1e+10 * Number.PositiveEpsilonOf(1.0)));
            Vector d = a1 * (1.0 + (2 * Number.PositiveEpsilonOf(1.0)));

            Helper_TestAlmostEqualityForGenericType(a1, a2, b, c, d);

            // Wrapper
            Assert.That(Vector.AlmostEqual(a1, c), Is.False);
            Assert.That(Vector.AlmostEqual(a1, c, 1e-10), Is.False);
            Assert.That(Vector.AlmostEqual(a1, c, 1e-2), Is.True);

            // reference type -> no boxing
            Assert.That(a1, Is.SameAs(a1));
        }

        [Test, Repeat(5)]
        public void TestAlmostEquals_ComplexVector()
        {
            ComplexVector a1 = ComplexVector.Random(3, new ContinuousUniformDistribution());
            ComplexVector a2 = a1.Clone();
            ComplexVector b = -a1;
            ComplexVector c = a1 * (1.0 + (1e+10 * Number.PositiveEpsilonOf(1.0)));
            ComplexVector d = a1 * (1.0 + (2 * Number.PositiveEpsilonOf(1.0)));

            Helper_TestAlmostEqualityForGenericType(a1, a2, b, c, d);

            // Wrapper
            Assert.That(ComplexVector.AlmostEqual(a1, c), Is.False);
            Assert.That(ComplexVector.AlmostEqual(a1, c, 1e-10), Is.False);
            Assert.That(ComplexVector.AlmostEqual(a1, c, 1e-2), Is.True);

            // reference type -> no boxing
            Assert.That(a1, Is.SameAs(a1));
        }

        [Test, Repeat(5)]
        public void TestAlmostEquals_Polynomial()
        {
            Polynomial a1 = new Polynomial(Vector.Random(5, new ContinuousUniformDistribution()));
            Polynomial a2 = a1.Clone();
            Polynomial b = -a1;
            Polynomial c = a1 * (1.0 + (1e+10 * Number.PositiveEpsilonOf(1.0)));
            Polynomial d = a1 * (1.0 + (2 * Number.PositiveEpsilonOf(1.0)));

            Helper_TestAlmostEqualityForGenericType(a1, a2, b, c, d);

            // Wrapper
            Assert.That(Polynomial.AlmostEqual(a1, c), Is.False);
            Assert.That(Polynomial.AlmostEqual(a1, c, 1e-10), Is.False);
            Assert.That(Polynomial.AlmostEqual(a1, c, 1e-2), Is.True);

            // reference type -> no boxing
            Assert.That(a1, Is.SameAs(a1));
        }

        [Test, Repeat(5)]
        public void TestAlmostEquals_ComplexPolynomial()
        {
            ComplexPolynomial a1 = new ComplexPolynomial(ComplexVector.Random(5, new ContinuousUniformDistribution()));
            ComplexPolynomial a2 = a1.Clone();
            ComplexPolynomial b = -a1;
            ComplexPolynomial c = a1 * (1.0 + (1e+10 * Number.PositiveEpsilonOf(1.0)));
            ComplexPolynomial d = a1 * (1.0 + (2 * Number.PositiveEpsilonOf(1.0)));

            Helper_TestAlmostEqualityForGenericType(a1, a2, b, c, d);

            // Wrapper
            Assert.That(ComplexPolynomial.AlmostEqual(a1, c), Is.False);
            Assert.That(ComplexPolynomial.AlmostEqual(a1, c, 1e-10), Is.False);
            Assert.That(ComplexPolynomial.AlmostEqual(a1, c, 1e-2), Is.True);

            // reference type -> no boxing
            Assert.That(a1, Is.SameAs(a1));
        }
    }
}
