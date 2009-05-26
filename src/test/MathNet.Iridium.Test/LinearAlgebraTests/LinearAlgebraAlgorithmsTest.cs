//-----------------------------------------------------------------------
// <copyright file="LinearAlgebraAlgorithmsTest.cs" company="Math.NET Project">
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

namespace Iridium.Test.LinearAlgebraTests
{
    using MathNet.Numerics.Distributions;
    using MathNet.Numerics.LinearAlgebra;
    using MathNet.Numerics.LinearAlgebra.Algorithms;
    using MathNet.Numerics;

    [TestFixture]
    public class LinearAlgebraAlgorithmsTest
    {
        [Test]
        public void TestOrthogonalRotation()
        {
            StandardDistribution gaussian = new StandardDistribution();
            for(int i = 0; i < 100; i++)
            {
                Vector v = Vector.Random(2, gaussian);
                Matrix rotation = Orthogonal.Rotation(v);
                Vector res = rotation * v;
                Assert.That(Matrix.Transpose(rotation) * rotation, NumericIs.AlmostEqualTo(Matrix.Identity(2, 2)), "orthogonal rotation matrix");
                Assert.That(rotation.Norm2(), NumericIs.AlmostEqualTo((double) 1), "rotation matrix norm");
                Assert.That(Math.Abs(res[0]), NumericIs.AlmostEqualTo(v.Norm(), 1e-12), "res(1)");
                Assert.That(res[1], NumericIs.AlmostEqualTo((double) 0, 1e-12), "res(2)");
            }
        }

        [Test]
        public void TestOrthogonalRotationComplex()
        {
            StandardDistribution gaussian = new StandardDistribution();
            for(int i = 0; i < 100; i++)
            {
                ComplexVector v = new ComplexVector(new Complex[] { Complex.Random(gaussian), Complex.Random(gaussian) });
                ComplexMatrix rotation = Orthogonal.Rotation(v);
                ComplexVector res = rotation * v;
                Assert.That(rotation.HermitianTranspose() * rotation, NumericIs.AlmostEqualTo(ComplexMatrix.Identity(2, 2)), "unitary rotation matrix");
                Assert.That(rotation[0, 0].IsReal, "c1 real");
                Assert.That(rotation[1, 1].IsReal, "c2 real");
                Assert.That(res[0].Modulus, NumericIs.AlmostEqualTo(v.Norm(), 1e-12), "res(1)");
                Assert.That(res[1], NumericIs.AlmostEqualTo((Complex) 0, 1e-12), "res(2)");
            }
        }

        [Test]
        public void TestOrthogonalReflection()
        {
            StandardDistribution gaussian = new StandardDistribution();
            for(int i = 0; i < 100; i++)
            {
                Vector v = Vector.Random(4, gaussian);
                Matrix reflection = Orthogonal.Reflection(v);
                Vector res = reflection * v;
                Assert.That(Matrix.Transpose(reflection) * reflection, NumericIs.AlmostEqualTo(Matrix.Identity(4, 4)), "orthogonal reflection matrix");
                Assert.That(reflection.Norm2(), NumericIs.AlmostEqualTo((double) 1), "reflection matrix norm");
                Assert.That(Math.Abs(res[0]), NumericIs.AlmostEqualTo(v.Norm(), 1e-12), "res(1)");
                Assert.That(res[1], NumericIs.AlmostEqualTo((double) 0, 1e-12), "res(2)");
                Assert.That(res[2], NumericIs.AlmostEqualTo((double) 0, 1e-12), "res(3)");
                Assert.That(res[3], NumericIs.AlmostEqualTo((double) 0, 1e-12), "res(4)");
            }
        }

        [Test]
        public void TestOrthogonalReflectionComplex()
        {
            StandardDistribution gaussian = new StandardDistribution();
            for(int i = 0; i < 100; i++)
            {
                ComplexVector v = new ComplexVector(new Complex[] { Complex.Random(gaussian), Complex.Random(gaussian), Complex.Random(gaussian), Complex.Random(gaussian) });
                ComplexMatrix reflection = Orthogonal.Reflection(v);
                ComplexVector res = reflection * v;
                Assert.That(reflection.HermitianTranspose() * reflection, NumericIs.AlmostEqualTo(ComplexMatrix.Identity(4, 4)), "orthogonal reflection matrix");
                Assert.That(reflection[0, 0].IsReal, "c1 real");
                Assert.That(reflection[1, 1].IsReal, "c2 real");
                Assert.That(reflection[2, 2].IsReal, "c3 real");
                Assert.That(reflection[3, 3].IsReal, "c4 real");
                Assert.That(res[0].Modulus, NumericIs.AlmostEqualTo(v.Norm(), 1e-12), "res(1)");
                Assert.That(res[1], NumericIs.AlmostEqualTo((Complex) 0, 1e-12), "res(2)");
                Assert.That(res[2], NumericIs.AlmostEqualTo((Complex) 0, 1e-12), "res(3)");
                Assert.That(res[3], NumericIs.AlmostEqualTo((Complex) 0, 1e-12), "res(4)");
            }
        }
    }
}
