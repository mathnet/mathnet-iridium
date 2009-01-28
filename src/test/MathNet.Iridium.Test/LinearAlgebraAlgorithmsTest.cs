//-----------------------------------------------------------------------
// <copyright file="LinearAlgebraAlgorithms.cs" company="Math.NET Project">
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

namespace Iridium.Test
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
                NumericAssert.AreAlmostEqual(Matrix.Identity(2, 2), Matrix.Transpose(rotation) * rotation, "orthogonal rotation matrix");
                NumericAssert.AreAlmostEqual(1, rotation.Norm2(), "rotation matrix norm");
                NumericAssert.AreAlmostEqual(v.Norm(), Math.Abs(res[0]), 1e-12, "res(1)");
                NumericAssert.AreAlmostEqual(0, res[1], 1e-12, "res(2)");
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
                NumericAssert.AreAlmostEqual(ComplexMatrix.Identity(2, 2), rotation.HermitianTranspose() * rotation, "unitary rotation matrix");
                Assert.IsTrue(rotation[0, 0].IsReal, "c1 real");
                Assert.IsTrue(rotation[1, 1].IsReal, "c2 real");
                NumericAssert.AreAlmostEqual(v.Norm(), res[0].Modulus, 1e-12, "res(1)");
                NumericAssert.AreAlmostEqual(0, res[1], 1e-12, "res(2)");
            }
        }
    }
}
