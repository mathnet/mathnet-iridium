//-----------------------------------------------------------------------
// <copyright file="Orthogonal.cs" company="Math.NET Project">
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

namespace MathNet.Numerics.LinearAlgebra.Algorithms
{
    /// <summary>
    /// Orthogonal and Unitary Transformations
    /// </summary>
    public static class Orthogonal
    {
        /// <summary>
        /// Givens Rotation: Evaluate c and s in orthogonal G = [c,s;-s,c] with norm([c,s]) = 1 such that |G*v| = [norm(v);0]
        /// </summary>
        public static void Rotation(double v1, double v2, out double c, out double s)
        {
            if(Number.AlmostZero(v2))
            {
                c = 1;
                s = 0;
                return;
            }

            if(Number.AlmostZero(v1))
            {
                c = 0;
                s = Math.Sign(v2);
                return;
            }

            double len = Fn.Hypot(v1, v2);
            c = Math.Abs(v1) / len;
            s = Math.Sign(v1) * v2 / len;
        }

        /// <summary>
        /// Givens Rotation: Evaluate orthogonal G = [c,s;-s,c] such that |G*v| = [norm(v);0] 
        /// </summary>
        public static Matrix Rotation(Vector v)
        {
            double c;
            double s;

            Rotation(v[0], v[1], out c, out s);

            double[][] m = Matrix.CreateMatrixData(2, 2);
            m[0][0] = c;
            m[0][1] = s;
            m[1][0] = -s;
            m[1][1] = c;

            return new Matrix(m);
        }

        /// <summary>
        /// Givens Rotation: Evaluate c and s in unitary G = [c,s;-s,c] with norm([c,s]) = 1 such that |G*v| = [norm(v);0] and c is real. 
        /// </summary>
        public static void Rotation(Complex v1, Complex v2, out double c, out Complex s)
        {
            double v1NormSqr = v1.ModulusSquared;
            double v2NormSqr = v2.ModulusSquared;

            if(Number.AlmostZero(v2NormSqr))
            {
                c = 1;
                s = Complex.Zero;
                return;
            }

            if(Number.AlmostZero(v1NormSqr))
            {
                c = 0;
                s = v2.Conjugate.Sign;
                return;
            }

            double len = Math.Sqrt(v1NormSqr + v2NormSqr);
            c = Math.Sqrt(v1NormSqr) / len;
            s = v1.Sign * v2.Conjugate / len;
        }

        /// <summary>
        /// Givens Rotation: Evaluate unitary G = [c,s;-s,c] such that |G*v| = [norm(v);0] and c is real. 
        /// </summary>
        public static ComplexMatrix Rotation(ComplexVector v)
        {
            double c;
            Complex s;

            Rotation(v[0], v[1], out c, out s);

            Complex[][] m = ComplexMatrix.CreateMatrixData(2, 2);
            m[0][0] = c;
            m[0][1] = s;
            m[1][0] = -s.Conjugate;
            m[1][1] = c;

            return new ComplexMatrix(m);
        }

        /// <summary>
        /// Householder Reflection: Evaluate symmetric orthogonal Q such that Q*v = -sigma*||v||*e1 
        /// </summary>
        public static Matrix Reflection(Vector v)
        {
            double sigma = v[0] >= 0 ? 1 : -1;

            Vector u = v.Clone();
            u[0] += sigma * v.Norm();

            Matrix m = Matrix.Identity(v.Length, v.Length);
            m.MultiplyAccumulateInplace(u.TensorMultiply(u), -2d / u.ScalarMultiply(u));

            return m;
        }

        /// <summary>
        /// Householder Reflection: Evaluate symmetric? unitary Q such that Q*v = -sigma*||v||*e1 
        /// </summary>
        public static ComplexMatrix Reflection(ComplexVector v)
        {
            Complex sigma = v[0].Sign;

            ComplexVector u = v.Clone();
            u[0] += sigma * v.Norm();

            ComplexMatrix m = ComplexMatrix.Identity(v.Length, v.Length);
            m.MultiplyAccumulateInplace(u.TensorMultiply(u), -2d / u.ScalarMultiply(u));

            return m;
        }
    }
}
