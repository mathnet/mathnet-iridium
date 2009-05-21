//-----------------------------------------------------------------------
// <copyright file="NumericAssert.cs" company="Math.NET Project">
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
using NUnit.Framework.Constraints;

using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace Iridium.Test
{
    public static class NumericAssert
    {
        public static void AreAlmostEqual(Matrix expected, Matrix actual, string message)
        {
            IEqualityComparer<Matrix> comparer = EqualityComparers.ForMatrix(10 * Number.DefaultRelativeAccuracy);
            Assert.That(actual, Is.EqualTo(expected).Using(comparer), message);
        }

        public static void AreAlmostEqual(Matrix expected, Matrix actual, double relativeAccuracy, string message)
        {
            IEqualityComparer<Matrix> comparer = EqualityComparers.ForMatrix(relativeAccuracy);
            Assert.That(actual, Is.EqualTo(expected).Using(comparer), message);
        }

        public static void AreAlmostEqual(ComplexMatrix expected, ComplexMatrix actual, string message)
        {
            IEqualityComparer<ComplexMatrix> comparer = EqualityComparers.ForComplexMatrix(10 * Number.DefaultRelativeAccuracy);
            Assert.That(actual, Is.EqualTo(expected).Using(comparer), message);
        }

        public static void AreAlmostEqual(ComplexMatrix expected, ComplexMatrix actual, double relativeAccuracy, string message)
        {
            IEqualityComparer<ComplexMatrix> comparer = EqualityComparers.ForComplexMatrix(relativeAccuracy);
            Assert.That(actual, Is.EqualTo(expected).Using(comparer), message);
        }

        public static void AreAlmostEqual(Vector expected, Vector actual, string message)
        {
            IEqualityComparer<double> comparer = EqualityComparers.ForDouble(10 * Number.DefaultRelativeAccuracy);
            Assert.That(actual, Is.EqualTo(expected).Using(comparer), message);
        }

        public static void AreAlmostEqual(Vector expected, Vector actual, double relativeAccuracy, string message)
        {
            IEqualityComparer<double> comparer = EqualityComparers.ForDouble(relativeAccuracy);
            Assert.That(actual, Is.EqualTo(expected).Using(comparer), message);
        }

        public static void AreAlmostEqual(ComplexVector expected, ComplexVector actual, string message)
        {
            IEqualityComparer<Complex> comparer = EqualityComparers.ForComplex(10 * Number.DefaultRelativeAccuracy);
            Assert.That(actual, Is.EqualTo(expected).Using(comparer), message);
        }

        public static void AreAlmostEqual(ComplexVector expected, ComplexVector actual, double relativeAccuracy, string message)
        {
            IEqualityComparer<Complex> comparer = EqualityComparers.ForComplex(relativeAccuracy);
            Assert.That(actual, Is.EqualTo(expected).Using(comparer), message);
        }

        public static void AreAlmostEqual(double expected, double actual, double relativeAccuracy, string message)
        {
            IEqualityComparer<double> comparer = EqualityComparers.ForDouble(relativeAccuracy);
            Assert.That(actual, Is.EqualTo(expected).Using(comparer), message);
        }

        public static void AreAlmostEqual(double expected, double actual, string message)
        {
            IEqualityComparer<double> comparer = EqualityComparers.ForDouble(Number.DefaultRelativeAccuracy);
            Assert.That(actual, Is.EqualTo(expected).Using(comparer), message);
        }

        public static void AreAlmostEqual(Complex expected, Complex actual, double relativeAccuracy, string message)
        {
            IEqualityComparer<Complex> comparer = EqualityComparers.ForComplex(relativeAccuracy);
            Assert.That(actual, Is.EqualTo(expected).Using(comparer), message);
        }

        public static void AreAlmostEqual(Complex expected, Complex actual, string message)
        {
            IEqualityComparer<Complex> comparer = EqualityComparers.ForComplex(Number.DefaultRelativeAccuracy);
            Assert.That(actual, Is.EqualTo(expected).Using(comparer), message);
        }
    }
}
