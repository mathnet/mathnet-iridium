//-----------------------------------------------------------------------
// <copyright file="MatrixStressTest.cs" company="Math.NET Project">
//    Copyright (c) 2002-2009, Christoph Rüegg, Joannès Vermorel.
//    All Right Reserved.
// </copyright>
// <author>
//    Christoph Rüegg, http://christoph.ruegg.name
//    Joannès Vermorel, http://www.vermorel.com
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

namespace Iridium.Test.LinearAlgebraTests
{
    using MathNet.Numerics.LinearAlgebra;

    [TestFixture]
    public class MatrixStressTest
    {
        static readonly Random Rnd = new Random();

        [Test, Sequential]
        public void SolveLinearSystem(
            [Values(10, 20, 50, 100, 200, 500)] int n)
        {
            double[][] a = Matrix.CreateMatrixData(n, n);
            double[][] x = Matrix.CreateMatrixData(n, 1);

            for(int i = 0; i < n; i++)
            {
                for(int j = 0; j < n; j++)
                {
                    if(j == 0)
                    {
                        x[i][0] = Rnd.NextDouble();
                    }

                    a[i][j] = Rnd.NextDouble();
                }
            }

            Matrix ma = new Matrix(a);
            Matrix mx = new Matrix(x);
            Matrix mb = ma*mx;
            Assert.That(ma.Solve(mb), NumericIs.AlmostEqualTo(mx, 1e-10));
        }
    }
}
