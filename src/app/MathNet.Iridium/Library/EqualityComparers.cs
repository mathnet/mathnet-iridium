//-----------------------------------------------------------------------
// <copyright file="EqualityComparers.cs" company="Math.NET Project">
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
using MathNet.Numerics.LinearAlgebra;

namespace MathNet.Numerics
{
    /// <summary>
    /// Factory for equality comparers with a parametric maximum relative error
    /// </summary>
    public static class EqualityComparers
    {
        /// <summary>
        /// Create an equality comparer for real floating point numbers with a custom maximum relative error.
        /// </summary>
        /// <param name="maximumRelativeError">Maximum relative error of the two numbers.</param>
        /// <returns></returns>
        public static
        IEqualityComparer<double>
        ForDouble(double maximumRelativeError)
        {
            return new DoubleEqualityComparer(maximumRelativeError);
        }

        /// <summary>
        /// Create an equality comparer for complex floating point numbers with a custom maximum relative error.
        /// </summary>
        /// <param name="maximumRelativeError">Maximum relative error of the absolute difference.</param>
        /// <returns></returns>
        public static
        IEqualityComparer<Complex>
        ForComplex(double maximumRelativeError)
        {
            return new ComplexEqualityComparer(maximumRelativeError);
        }

        /// <summary>
        /// Create an equality comparer for real vectors with a custom maximum relative error.
        /// </summary>
        /// <param name="maximumRelativeError">Maximum relative error of the Vector difference 1-Norm.</param>
        /// <returns></returns>
        public static
        IEqualityComparer<Vector>
        ForVector(double maximumRelativeError)
        {
            return new VectorEqualityComparer(maximumRelativeError);
        }

        /// <summary>
        /// Create an equality comparer for complex vectors with a custom maximum relative error.
        /// </summary>
        /// <param name="maximumRelativeError">Maximum relative error of the Vector difference 1-Norm.</param>
        /// <returns></returns>
        public static
        IEqualityComparer<ComplexVector>
        ForComplexVector(double maximumRelativeError)
        {
            return new ComplexVectorEqualityComparer(maximumRelativeError);
        }

        /// <summary>
        /// Create an equality comparer for real matrices with a custom maximum relative error.
        /// </summary>
        /// <param name="maximumRelativeError">Maximum relative error of the Matrix difference 1-Norm.</param>
        /// <returns></returns>
        public static
        IEqualityComparer<Matrix>
        ForMatrix(double maximumRelativeError)
        {
            return new MatrixEqualityComparer(maximumRelativeError);
        }

        /// <summary>
        /// Create an equality comparer for complex matrices with a custom maximum relative error.
        /// </summary>
        /// <param name="maximumRelativeError">Maximum relative error of the Matrix difference 1-Norm.</param>
        /// <returns></returns>
        public static
        IEqualityComparer<ComplexMatrix>
        ForComplexMatrix(double maximumRelativeError)
        {
            return new ComplexMatrixEqualityComparer(maximumRelativeError);
        }

        private class DoubleEqualityComparer : IEqualityComparer<double>
        {
            private double _maxRelativeError;

            public DoubleEqualityComparer(double maximumRelativeError)
            {
                _maxRelativeError = maximumRelativeError;
            }

            public bool Equals(double x, double y)
            {
                return Number.AlmostEqual(x, y, _maxRelativeError);
            }

            public int GetHashCode(double obj)
            {
                return ((double)obj).GetHashCode();
            }
        }

        private abstract class NormEqualityComparer<T> : IEqualityComparer<T>
        {
            private double _maxRelativeError;

            public NormEqualityComparer(double maximumRelativeError)
            {
                _maxRelativeError = maximumRelativeError;
            }

            protected abstract double NormOf(T value);

            protected abstract double DifferenceNormOf(T x, T y);

            public bool Equals(T x, T y)
            {
                return Number.AlmostEqualNorm(NormOf(x), NormOf(y), DifferenceNormOf(x, y), _maxRelativeError);
            }

            public int GetHashCode(T obj)
            {
                return obj.GetHashCode();
            }
        }

        private class ComplexEqualityComparer : NormEqualityComparer<Complex>
        {
            public ComplexEqualityComparer(double maximumRelativeError)
                : base(maximumRelativeError)
            {
            }

            protected override double NormOf(Complex value)
            {
                return value.Modulus;
            }

            protected override double DifferenceNormOf(Complex x, Complex y)
            {
                return (x - y).Modulus;
            }
        }

        private class MatrixEqualityComparer : NormEqualityComparer<Matrix>
        {
            public MatrixEqualityComparer(double maximumRelativeError)
                : base(maximumRelativeError)
            {
            }

            protected override double NormOf(Matrix value)
            {
                return value.Norm1();
            }

            protected override double DifferenceNormOf(Matrix x, Matrix y)
            {
                return (x - y).Norm1();
            }
        }

        private class ComplexMatrixEqualityComparer : NormEqualityComparer<ComplexMatrix>
        {
            public ComplexMatrixEqualityComparer(double maximumRelativeError)
                : base(maximumRelativeError)
            {
            }

            protected override double NormOf(ComplexMatrix value)
            {
                return value.Norm1();
            }

            protected override double DifferenceNormOf(ComplexMatrix x, ComplexMatrix y)
            {
                return (x - y).Norm1();
            }
        }

        private class VectorEqualityComparer : NormEqualityComparer<Vector>
        {
            public VectorEqualityComparer(double maximumRelativeError)
                : base(maximumRelativeError)
            {
            }

            protected override double NormOf(Vector value)
            {
                return value.Norm1();
            }

            protected override double DifferenceNormOf(Vector x, Vector y)
            {
                return (x - y).Norm1();
            }
        }

        private class ComplexVectorEqualityComparer : NormEqualityComparer<ComplexVector>
        {
            public ComplexVectorEqualityComparer(double maximumRelativeError)
                : base(maximumRelativeError)
            {
            }

            protected override double NormOf(ComplexVector value)
            {
                return value.Norm1();
            }

            protected override double DifferenceNormOf(ComplexVector x, ComplexVector y)
            {
                return (x - y).Norm1();
            }
        }
    }
}
