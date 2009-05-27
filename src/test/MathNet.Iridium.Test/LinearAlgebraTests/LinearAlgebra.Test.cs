//-----------------------------------------------------------------------
// <copyright file="LinearAlgebra.Test.cs" company="Math.NET Project">
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

namespace Iridium.Test.LinearAlgebraTests
{
    using MathNet.Numerics.LinearAlgebra;
    using MathNet.Numerics.Distributions;

    /// <summary>TestMatrix tests the functionality of the 
    /// Matrix class and associated decompositions.</summary>
    /// <remarks>
    /// <para>
    /// Detailed output is provided indicating the functionality being tested
    /// and whether the functionality is correctly implemented. Exception handling
    /// is also tested.
    /// </para>
    /// <para>
    /// The test is designed to run to completion and give a summary of any implementation errors
    /// encountered. The final output should be:
    /// </para>
    /// <BLOCKQUOTE><PRE><CODE>
    /// TestMatrix completed.
    /// Total errors reported: n1
    /// Total warning reported: n2
    /// </CODE></PRE></BLOCKQUOTE>
    /// <para>
    /// If the test does not run to completion, this indicates that there is a 
    /// substantial problem within the implementation that was not anticipated in the test design.  
    /// The stopping point should give an indication of where the problem exists.
    /// </para>
    /// </remarks>
    [TestFixture]
    public class LinearAlgebraTests
    {
        static readonly IDiscreteGenerator SizeRandom = new DiscreteUniformDistribution(8, 13);
        static readonly IContinuousGenerator ValueRandom = new StableDistribution();

        [Test]
        public void MultiplyByDiagonal()
        {
            Matrix a = Matrix.Create(
                new double[3, 4] {
                    { 1, 2, 3, 4 },
                    { 3, 4, 5, 6 },
                    { 5, 6, 7, 8 }
                    });

            double[] diagonal = new double[] { 0, 1, 2 };

            a.MultiplyLeftDiagonalInplace(new Vector(diagonal));

            Assert.That(a[0, 0], Is.EqualTo(0), "#A00");
            Assert.That(a[0, 1], Is.EqualTo(0), "#A01");
            Assert.That(a[1, 0], Is.EqualTo(3), "#A02");
            Assert.That(a[1, 1], Is.EqualTo(4), "#A03");
            Assert.That(a[2, 0], Is.EqualTo(10), "#A04");
            Assert.That(a[2, 1], Is.EqualTo(12), "#A05");
        }

        [Test]
        public void MultiplyByMatrix()
        {
            Matrix a = Matrix.Create(
                new double[3, 4] {
                    { 10, -61, -8, -29 },
                    { 95, 11, -49, -47 },
                    { 40, -81, 91, 68 }
                    });

            Matrix b = Matrix.Create(
                new double[4, 2] {
                    { 72, 37 },
                    { -23, 87 },
                    { 44, 29 },
                    { 98, -23 }
                    });

            Matrix c = Matrix.Create(
                new double[3, 2] {
                    { -1071, -4502 },
                    {  -175, 4132 },
                    { 15411, -4492 }
                    });

            Matrix p = a.Multiply(b);

            Assert.That(p.ColumnCount, Is.EqualTo(c.ColumnCount), "#A00 Invalid column count in linear product.");
            Assert.That(p.RowCount, Is.EqualTo(c.RowCount), "#A01 Invalid row count in linear product.");
            Assert.That(p, NumericIs.AlmostEqualTo(c), "#A02 Unexpected product value.");
        }

        [Test]
        public void SolveRobust()
        {
            Matrix a1 = Matrix.Create(
                new double[6, 2] {
                    { 1, 1 },
                    { 1, 2 },
                    { 1, 2 },
                    { 1, -1 },
                    { 0, 1 },
                    { 2, 1 }
                    });

            Matrix b1 = Matrix.Create(
                new double[6, 1] {
                    { 2 },
                    { 2 },
                    { 2 },
                    { 2 },
                    { 2 },
                    { 2 }
                    });

            Matrix x1 = a1.SolveRobust(b1);

            // [vermorel] Values have been computed with LAD function of Systat 12
            Assert.That(x1[0, 0], NumericIs.AlmostEqualTo(1.2), "#A00 Unexpected robust regression result.");
            Assert.That(x1[1, 0], NumericIs.AlmostEqualTo(0.4), "#A01 Unexpected robust regression result.");

            Matrix a2 = Matrix.Create(
                new double[6, 3] {
                    { 2, -1, 2 },
                    { 3, 2, 0 },
                    { 1, 2, 4 },
                    { 1, -1, -1 },
                    { 0, 1, 2 },
                    { 2, 1, 1 }
                    });

            Matrix b2 = Matrix.Create(
                new double[6, 1] {
                    { 0 },
                    { 4 },
                    { 2 },
                    { -3 },
                    { 2 },
                    { 1 }
                    });

            Matrix x2 = a2.SolveRobust(b2);

            // [vermorel] Values have been computed with LAD function of Systat 12
            Assert.That(x2[0, 0], NumericIs.AlmostEqualTo(0.667, 1e-3), "#A02 Unexpected robust regression result.");
            Assert.That(x2[1, 0], NumericIs.AlmostEqualTo(1.0, 1e-5), "#A03 Unexpected robust regression result.");
            Assert.That(x2[2, 0], NumericIs.AlmostEqualTo(-0.167, 1e-2), "#A04 Unexpected robust regression result.");

            Matrix a3 = Matrix.Create(
                new double[10, 4] {
                    { -8, -29, 95, 11 },
                    { -47, 40, -81, 91 },
                    { -10, 31, -51, 77 },
                    { 1, 1, 55, -28 },
                    { 30, -27, -15, -59 },
                    { 72, -87, 47, -90 },
                    { 92, -91, -88, -48 },
                    { -28, 5, 13, -10 },
                    { 71, 16, 83, 9 },
                    { -83, 98, -48, -19 }
                    });

            Matrix b3 = Matrix.Create(
                new double[10, 1] {
                    { -49 },
                    { 68 },
                    { 95 },
                    { 16 },
                    { -96 },
                    { 43 },
                    { 53 },
                    { -82 },
                    { -60 },
                    { 62 }
                    });

            Matrix x3 = a3.SolveRobust(b3);

            // [vermorel] Values have been computed with LAD function of Systat 12
            Assert.That(x3[0, 0], NumericIs.AlmostEqualTo(-0.104, 1e-2), "#A05 Unexpected robust regression result.");
            Assert.That(x3[1, 0], NumericIs.AlmostEqualTo(-0.216, 1e-2), "#A06 Unexpected robust regression result.");
            Assert.That(x3[2, 0], NumericIs.AlmostEqualTo(-0.618, 1e-3), "#A07 Unexpected robust regression result.");
            Assert.That(x3[3, 0], NumericIs.AlmostEqualTo(0.238, 1e-3), "#A08 Unexpected robust regression result.");
        }

        /// <summary>
        /// Testing the method <see cref="Matrix.SingularValueDecomposition"/>.
        /// </summary>
        [Test, Repeat(20)]
        public void SingularValueDecomposition()
        {
            Matrix matrix = Matrix.Random(10, SizeRandom.NextInt32(), ValueRandom);

            SingularValueDecomposition svd = matrix.SingularValueDecomposition;

            Matrix u = svd.LeftSingularVectors;
            Matrix vt = svd.RightSingularVectors;
            vt.TransposeInplace();
            Matrix product = u*svd.S*vt;

            Assert.That(product, NumericIs.AlmostEqualTo(matrix, 1e-14));
        }

        [Test]
        public void MatrixCreateIdentity()
        {
            Matrix m = new Matrix(new double[][] { new double[] { 1.0, 0.0, 0.0, 0.0 }, new double[] { 0.0, 1.0, 0.0, 0.0 }, new double[] { 0.0, 0.0, 1.0, 0.0 } });
            Matrix u = Matrix.Identity(3, 4);
            Assert.That(u, Is.Not.SameAs(m));
            Assert.That(u, Is.EqualTo(m));
            Assert.That(Matrix.Identity(4, 3), Is.EqualTo(Matrix.Transpose(m)));

            // verify there are no side effects (like reused identity matrix)
            u[0, 1] = 0.1;
            Assert.That(u, Is.Not.EqualTo(Matrix.Identity(3,4)));
        }

        [Test]
        public void MatrixCreateColumnwise()
        {
            const int Stride = 3;
            const int InvalidStride = 5;

            double[] columnwise = { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 11.0, 12.0 };

            // verif invalid parameter behavior
            Assert.That(delegate { new Matrix(columnwise, Stride); }, Throws.Nothing);
            Assert.That(delegate { new Matrix(columnwise, InvalidStride); }, Throws.TypeOf<ArgumentException>());

            // verify columnwise to rectangular transformation
            Matrix a = new Matrix(columnwise, Stride);
            Assert.That(a[0, 0], Is.EqualTo(columnwise[0]));
            Assert.That(a[1, 0], Is.EqualTo(columnwise[1]));
            Assert.That(a[2, 0], Is.EqualTo(columnwise[2]));
            Assert.That(a[0, 1], Is.EqualTo(columnwise[3]));
            Assert.That(a[0, 2], Is.EqualTo(columnwise[6]));
            Assert.That(a[0, 3], Is.EqualTo(columnwise[9]));
            Assert.That(a[1, 3], Is.EqualTo(columnwise[10]));
            Assert.That(a[2, 3], Is.EqualTo(columnwise[11]));

            // verify no side effects
            Assert.That(new Matrix(columnwise, Stride), Is.EqualTo(a));
            columnwise[0] = 1.5;
            Assert.That(new Matrix(columnwise, Stride), Is.Not.EqualTo(a));
            Assert.That(a.GetArray(), Is.Not.EqualTo(columnwise));
        }

        [Test]
        public void MatrixCreateDirect()
        {
            double[][] avals = {
                new double[] { 1.0, 4.0, 7.0, 10.0 },
                new double[] { 2.0, 5.0, 8.0, 11.0 },
                new double[] { 3.0, 6.0, 9.0, 12.0 }
            };

            Matrix a = new Matrix(avals);

            // verify value access
            Assert.That(a[0, 0], Is.EqualTo(avals[0][0]));
            Assert.That(a[2, 0], Is.EqualTo(avals[2][0]));
            Assert.That(a[0, 3], Is.EqualTo(avals[0][3]));
            Assert.That(a[2, 3], Is.EqualTo(avals[2][3]));

            // verify direct use, hence side effects
            Assert.That(a.GetArray(), Is.SameAs(avals));
            Matrix aClone = a.Clone();
            Assert.That(aClone, Is.Not.SameAs(a));
            Assert.That(aClone, Is.EqualTo(a));
            Assert.That(aClone.GetArray(), Is.Not.SameAs(avals));
            Assert.That(aClone.GetArray(), Is.EqualTo(avals));
            Assert.That(new Matrix(avals), Is.EqualTo(aClone));
            avals[0][0] = 1.5;
            Assert.That(new Matrix(avals), Is.EqualTo(a));
            Assert.That(new Matrix(avals), Is.Not.EqualTo(aClone));
            Assert.That(a.GetArray(), Is.EqualTo(avals));
        }

        [Test]
        public void MatrixCreateFromJaggedArray()
        {
            double[][] avals = {
                new double[] { 1.0, 4.0, 7.0, 10.0 },
                new double[] { 2.0, 5.0, 8.0, 11.0 },
                new double[] { 3.0, 6.0, 9.0, 12.0 }
            };

            Matrix a = Matrix.Create(avals);

            // verify value access
            Assert.That(a[0, 0], Is.EqualTo(avals[0][0]));
            Assert.That(a[2, 0], Is.EqualTo(avals[2][0]));
            Assert.That(a[0, 3], Is.EqualTo(avals[0][3]));
            Assert.That(a[2, 3], Is.EqualTo(avals[2][3]));

            // verify no side effects
            Assert.That(a.GetArray(), Is.Not.SameAs(avals));
            Assert.That(a.GetArray(), Is.EqualTo(avals));
            Matrix aClone = a.Clone();
            Assert.That(aClone, Is.Not.SameAs(a));
            Assert.That(aClone, Is.EqualTo(a));
            Assert.That(Matrix.Create(avals), Is.EqualTo(aClone));
            avals[0][0] = 1.5;
            Assert.That(Matrix.Create(avals), Is.Not.EqualTo(a));
            Assert.That(Matrix.Create(avals), Is.Not.EqualTo(aClone));
            Assert.That(a.GetArray(), Is.Not.EqualTo(avals));
        }

        [Test]
        public void MatrixCastArray()
        {
            double[][] avals = {
                new double[] { 1.0, 4.0, 7.0, 10.0 },
                new double[] { 2.0, 5.0, 8.0, 11.0 },
                new double[] { 3.0, 6.0, 9.0, 12.0 }
            };

            Matrix a = new Matrix(avals);
            Assert.That(a.RowCount, Is.EqualTo(3), "getRowDimension");
            Assert.That(a.ColumnCount, Is.EqualTo(4), "getColumnDimension");

            double[][] aData = a;
            Assert.That(aData, Is.SameAs(avals), "getArray");

            double[][] bData = a.Clone();
            Assert.That(bData, Is.Not.SameAs(avals), "getArrayCopy");
            Assert.That(a, Is.EqualTo(new Matrix(bData)), "getArrayCopy II");
        }

        [Test]
        public void MatrixGetSubmatrix()
        {
            const int RowTop = 1;
            const int RowBottom = 2;
            const int ColumnLeft = 1;
            const int ColumnRight = 3;

            int[] rowindexset = new int[] { 1, 2 };
            int[] badrowindexset = new int[] { 1, 3 };
            int[] columnindexset = new int[] { 1, 2, 3 };
            int[] badcolumnindexset = new int[] { 1, 2, 4 };

            double[][] avals = {
                new double[] { 1.0, 4.0, 7.0, 10.0 },
                new double[] { 2.0, 5.0, 8.0, 11.0 },
                new double[] { 3.0, 6.0, 9.0, 12.0 }
            };

            double[][] subavals = {
                new double[] { 5.0, 8.0, 11.0 },
                new double[] { 6.0, 9.0, 12.0 }
            };

            Matrix a = new Matrix(avals);
            Matrix sub = new Matrix(subavals);

            // cells
            Assert.That(a[a.RowCount - 1, a.ColumnCount - 1], Is.EqualTo(avals[a.RowCount - 1][a.ColumnCount - 1]), "get(int,int)");
            Assert.That(delegate { double x = a[a.RowCount, a.ColumnCount - 1]; }, Throws.TypeOf<IndexOutOfRangeException>());
            Assert.That(delegate { double x = a[a.RowCount - 1, a.ColumnCount]; }, Throws.TypeOf<IndexOutOfRangeException>());

            // submatrices by intervals
            Assert.That(a.GetMatrix(RowTop, RowBottom, ColumnLeft, ColumnRight), NumericIs.AlmostEqualTo(sub), "GetMatrix(int,int,int,int)");
            Assert.That(delegate { Matrix m = a.GetMatrix(RowTop, RowBottom + a.RowCount + 1, ColumnLeft, ColumnRight); }, Throws.TypeOf<IndexOutOfRangeException>());
            Assert.That(delegate { Matrix m = a.GetMatrix(RowTop, RowBottom, ColumnLeft, ColumnRight + a.ColumnCount + 1); }, Throws.TypeOf<IndexOutOfRangeException>());
            
            // submatrices by column index sets
            Assert.That(a.GetMatrix(RowTop, RowBottom, columnindexset), Is.EqualTo(sub), "GetMatrix(int,int,int[])");
            Assert.That(delegate { Matrix m = a.GetMatrix(RowTop, RowBottom, badcolumnindexset); }, Throws.TypeOf<IndexOutOfRangeException>());
            Assert.That(delegate { Matrix m = a.GetMatrix(RowTop, RowBottom + a.RowCount + 1, columnindexset); }, Throws.TypeOf<IndexOutOfRangeException>());

            // submatrices by row index sets
            Assert.That(a.GetMatrix(rowindexset, ColumnLeft, ColumnRight), Is.EqualTo(sub), "GetMatrix(int[],int,int)");
            Assert.That(delegate { Matrix m = a.GetMatrix(badrowindexset, ColumnLeft, ColumnRight); }, Throws.TypeOf<IndexOutOfRangeException>());
            Assert.That(delegate { Matrix m = a.GetMatrix(rowindexset, ColumnLeft, ColumnRight + a.ColumnCount + 1); }, Throws.TypeOf<IndexOutOfRangeException>());

            // submatrices by column/row index sets
            Assert.That(a.GetMatrix(rowindexset, columnindexset), Is.EqualTo(sub), "GetMatrix(int[],int[])");
            Assert.That(delegate { Matrix m = a.GetMatrix(badrowindexset, columnindexset); }, Throws.TypeOf<IndexOutOfRangeException>());
            Assert.That(delegate { Matrix m = a.GetMatrix(rowindexset, badcolumnindexset); }, Throws.TypeOf<IndexOutOfRangeException>());
        }

        [Test]
        public void MatrixSetSubmatrix()
        {
            const int RowTop = 1;
            const int RowBottom = 2;
            const int ColumnLeft = 1;
            const int ColumnRight = 3;

            int[] rowindexset = new int[] { 1, 2 };
            int[] badrowindexset = new int[] { 1, 3 };
            int[] columnindexset = new int[] { 1, 2, 3 };
            int[] badcolumnindexset = new int[] { 1, 2, 4 };

            double[][] avals = {
                new double[] { 1.0, 4.0, 7.0, 10.0 },
                new double[] { 2.0, 5.0, 8.0, 11.0 },
                new double[] { 3.0, 6.0, 9.0, 12.0 }
            };

            Matrix a = new Matrix(avals);

            Matrix sub = new Matrix(2, 3, 0.0);

            // cells
            Assert.That(delegate { a[a.RowCount, a.ColumnCount - 1] = 0.0; }, Throws.TypeOf<IndexOutOfRangeException>());
            Assert.That(delegate { a[a.RowCount - 1, a.ColumnCount] = 0.0; }, Throws.TypeOf<IndexOutOfRangeException>());
            Matrix b = a.Clone();
            b[RowTop, ColumnLeft] = 0.0;
            Assert.That(b[RowTop, ColumnLeft], Is.EqualTo(0.0), "set(int,int,double)");

            // submatrices by intervals
            Assert.That(delegate { a.SetMatrix(RowTop, RowBottom + a.RowCount + 1, ColumnLeft, ColumnRight, sub); }, Throws.TypeOf<IndexOutOfRangeException>());
            Assert.That(delegate { a.SetMatrix(RowTop, RowBottom, ColumnLeft, ColumnRight + a.ColumnCount + 1, sub); }, Throws.TypeOf<IndexOutOfRangeException>());
            b = a.Clone();
            b.SetMatrix(RowTop, RowBottom, ColumnLeft, ColumnRight, sub);
            Assert.That(b.GetMatrix(RowTop, RowBottom, ColumnLeft, ColumnRight), Is.EqualTo(sub), "SetMatrix(int,int,int,int,Matrix)");

            // submatrices by column index sets
            Assert.That(delegate { a.SetMatrix(RowTop, RowBottom + a.RowCount + 1, columnindexset, sub); }, Throws.TypeOf<IndexOutOfRangeException>());
            Assert.That(delegate { a.SetMatrix(RowTop, RowBottom, badcolumnindexset, sub); }, Throws.TypeOf<IndexOutOfRangeException>());
            b = a.Clone();
            b.SetMatrix(RowTop, RowBottom, columnindexset, sub);
            Assert.That(b.GetMatrix(RowTop, RowBottom, ColumnLeft, ColumnRight), Is.EqualTo(sub), "SetMatrix(int,int,int[],Matrix)");

            // submatrices by row index sets
            Assert.That(delegate { a.SetMatrix(rowindexset, ColumnLeft, ColumnRight + a.ColumnCount + 1, sub); }, Throws.TypeOf<IndexOutOfRangeException>());
            Assert.That(delegate { a.SetMatrix(badrowindexset, ColumnLeft, ColumnRight, sub); }, Throws.TypeOf<IndexOutOfRangeException>());
            b = a.Clone();
            b.SetMatrix(rowindexset, ColumnLeft, ColumnRight, sub);
            Assert.That(b.GetMatrix(RowTop, RowBottom, ColumnLeft, ColumnRight), Is.EqualTo(sub), "SetMatrix(int[],int,int,Matrix)");

            // submatrices by column/row index sets
            Assert.That(delegate { a.SetMatrix(rowindexset, badcolumnindexset, sub); }, Throws.TypeOf<IndexOutOfRangeException>());
            Assert.That(delegate { a.SetMatrix(badrowindexset, columnindexset, sub); }, Throws.TypeOf<IndexOutOfRangeException>());
            b = a.Clone();
            b.SetMatrix(rowindexset, columnindexset, sub);
            Assert.That(b.GetMatrix(RowTop, RowBottom, ColumnLeft, ColumnRight), Is.EqualTo(sub), "SetMatrix(int[],int[],Matrix)");
        }

        [Test]
        public void MatrixArrayArithmetics()
        {
            IContinuousGenerator uniform = new ContinuousUniformDistribution(-1, 2);
            
            Matrix r = Matrix.Random(3, 4, uniform);
            Matrix r2 = Matrix.Random(3, 4, uniform);
   
            // add/subtraction with non-matching sizes should fail
            Assert.That(delegate { Matrix m = r - Matrix.Zeros(4); }, Throws.TypeOf<ArgumentException>());
            Assert.That(delegate { Matrix m = r + Matrix.Zeros(4); }, Throws.TypeOf<ArgumentException>());
            Assert.That(delegate { r.SubtractInplace(Matrix.Zeros(4)); }, Throws.TypeOf<ArgumentException>());
            Assert.That(delegate { r.AddInplace(Matrix.Zeros(4)); }, Throws.TypeOf<ArgumentException>());

            // subtraction & addition
            Matrix a = r - r2;
            Assert.That(a + r2, NumericIs.AlmostEqualTo(r));
            a.AddInplace(r2);
            Assert.That(a, NumericIs.AlmostEqualTo(r));

            // subtraction with itself should be zero
            Assert.That((r - r).Norm1(), Is.EqualTo(0.0), "Subtract I: difference of identical Matrices is nonzero,\nSubsequent use of Subtract should be suspect");
            Assert.That((r.Clone() - r).Norm1(), Is.EqualTo(0.0), "Subtract II: difference of identical Matrices is nonzero,\nSubsequent use of Subtract should be suspect");
            Matrix b = r.Clone();
            b.SubtractInplace(r);
            Assert.That(b.Norm1(), Is.EqualTo(0.0), "Subtract III: difference of identical Matrices is nonzero,\nSubsequent use of Subtract should be suspect");
            Assert.That(b, Is.EqualTo(new Matrix(r.RowCount, r.ColumnCount)));

            // addition with negative of itself should be zero
            Matrix c = r.Clone();
            c.NegateInplace();
            Assert.That(c, Is.EqualTo(-r));
            Assert.That((c + r).Norm1(), Is.EqualTo(0.0));

            // array-division
            Assert.That(delegate { Matrix.ArrayDivide(Matrix.Ones(3), Matrix.Ones(4)); }, Throws.TypeOf<ArgumentException>());
            Assert.That(delegate { r.ArrayDivide(Matrix.Ones(4)); }, Throws.TypeOf<ArgumentException>());
            Assert.That(Matrix.ArrayDivide(Matrix.Zeros(4), Matrix.Ones(4)), Is.EqualTo(Matrix.Zeros(4)));
            Matrix d = r.Clone();
            d.ArrayDivide(r);
            Assert.That(d, NumericIs.AlmostEqualTo(new Matrix(r.RowCount, r.ColumnCount, 1.0)));

            // array-multiplication
            Assert.That(delegate { Matrix.ArrayMultiply(Matrix.Ones(3), Matrix.Ones(4)); }, Throws.TypeOf<ArgumentException>());
            Assert.That(delegate { r.ArrayMultiply(Matrix.Ones(4)); }, Throws.TypeOf<ArgumentException>());
            Assert.That(Matrix.ArrayMultiply(r, new Matrix(r.RowCount, r.ColumnCount, 1.0)), Is.EqualTo(r));
            Matrix e = r.Clone();
            e.ArrayMultiply(new Matrix(r.RowCount, r.ColumnCount));
            Assert.That(e, Is.EqualTo(new Matrix(r.RowCount, r.ColumnCount)));

            // mixed array multiplication & division
            Matrix f = r.Clone();
            f.ArrayMultiply(r2);
            f.ArrayDivide(r2);
            Assert.That(f, NumericIs.AlmostEqualTo(r));
        }

        [Test]
        public void MatrixTranspose()
        {
            double[] columnwise = { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 11.0, 12.0 };

            double[][] tvals = {
                new double[] { 1.0, 2.0, 3.0 },
                new double[] { 4.0, 5.0, 6.0 },
                new double[] { 7.0, 8.0, 9.0 },
                new double[] { 10.0, 11.0, 12.0 }
            };

            // matrix transpose
            Matrix a = new Matrix(columnwise, 3);
            Matrix t = Matrix.Create(tvals);
            Assert.That(Matrix.Transpose(t), Is.EqualTo(a));
            Assert.That(t, Is.Not.EqualTo(a));
            t.TransposeInplace();
            Assert.That(t, Is.EqualTo(a));
        }

        [Test]
        public void MatrixNormTraceDeterminant()
        {
            double[] columnwise = { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 11.0, 12.0 };

            const double ColumnSumMax = 33.0;
            const double RowSumMax = 30.0;
            const double SumOfDiagonals = 15;
            const double SumOfSquares = 650;

            Matrix a = new Matrix(columnwise, 3);

            // Norms
            Assert.That(ColumnSumMax, NumericIs.AlmostEqualTo(a.Norm1()), "Norm1");
            Assert.That(RowSumMax, NumericIs.AlmostEqualTo(a.NormInf()), "NormInf");
            Assert.That(Math.Sqrt(SumOfSquares), NumericIs.AlmostEqualTo(a.NormF()), "NormF");

            // Trace
            Assert.That(SumOfDiagonals, NumericIs.AlmostEqualTo(a.Trace()), "Trace");

            // Determinant
            Assert.That(0.0, NumericIs.AlmostEqualTo(a.GetMatrix(0, a.RowCount - 1, 0, a.RowCount - 1).Determinant()), "Determinant");
        }

        [Test]
        public void MatrixLUDecomposition()
        {
            double[] columnwise = { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 11.0, 12.0 };
            Matrix b = new Matrix(columnwise, 4);
            b[0, 0] = 0.0;

            LUDecomposition lu = b.LUDecomposition;
            Assert.That(lu.L * lu.U, NumericIs.AlmostEqualTo(b.GetMatrix(lu.Pivot, 0, b.ColumnCount - 1)), "LUDecomposition");
        }

        [Test]
        public void MatrixQRDecomposition()
        {
            double[] columnwise = { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 11.0, 12.0 };

            Matrix b = new Matrix(columnwise, 4);
            QRDecomposition qr = b.QRDecomposition;
            Matrix r = qr.R;
            Assert.That(qr.Q * r, NumericIs.AlmostEqualTo(b), "QRDecomposition");
        }
        
        [Test]
        public void MatrixPseudoInverse()
        {
            double[] columnwise = { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 11.0, 12.0 };
            Matrix b = new Matrix(columnwise, 4);
            b[0, 0] = 0.0;

            Matrix d = b.Inverse();
            Assert.That(d * b, NumericIs.AlmostEqualTo(Matrix.Identity(3, 3), 1e-13), "Inverse");
        }

        [Test]
        public void MatrixSolve()
        {
            double[][] subavals = {
                new double[] { 5.0, 8.0, 11.0 },
                new double[] { 6.0, 9.0, 12.0 }
            };

            double[][] sqSolution = {
                new double[] { 13.0 },
                new double[] { 15.0 }
            };

            Matrix sub = new Matrix(subavals);
            Matrix o = new Matrix(sub.RowCount, 1, 1.0);
            Matrix sol = new Matrix(sqSolution);
            Matrix sq = sub.GetMatrix(0, sub.RowCount - 1, 0, sub.RowCount - 1);
            Assert.That(sq.Solve(sol), NumericIs.AlmostEqualTo(o), "Solve");
        }

        [Test]
        public void MatrixSingularValueDecomposition()
        {
            double[] columnwise = { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 11.0, 12.0 };

            double[][] condmat = {
                new double[] { 1.0, 3.0 },
                new double[] { 7.0, 9.0 }
            };

            Matrix b = new Matrix(columnwise, 4);
            SingularValueDecomposition svd = b.SingularValueDecomposition;
            Assert.That(svd.LeftSingularVectors * (svd.S * Matrix.Transpose(svd.RightSingularVectors)), NumericIs.AlmostEqualTo(b), "SingularValueDecomposition");

            // Matrix Rank (of rank deficient matrix)
            Matrix def = new Matrix(columnwise, 3);
            Assert.That(def.Rank(), Is.EqualTo(Math.Min(def.RowCount, def.ColumnCount) - 1), "Rank");

            // Matrix Condition
            Matrix c = new Matrix(condmat);
            svd = c.SingularValueDecomposition;
            double[] singularvalues = svd.SingularValues;
            Assert.That(c.Condition(), NumericIs.AlmostEqualTo(singularvalues[0] / singularvalues[Math.Min(c.RowCount, c.ColumnCount) - 1]), "Condition");
        }

        [Test]
        public void MatrixCholeskyDecomposition()
        {
            double[][] pvals = {
                new double[] { 25, -5, 10 },
                new double[] { -5, 17, 10 },
                new double[] { 10, 10, 62 }
            };

            Matrix e = new Matrix(pvals);
            CholeskyDecomposition chol = e.CholeskyDecomposition;
            Matrix l = chol.TriangularFactor;
            Assert.That(l * Matrix.Transpose(l), NumericIs.AlmostEqualTo(e), "CholeskyDecomposition");
            Matrix g = chol.Solve(Matrix.Identity(3, 3));
            Assert.That(e * g, NumericIs.AlmostEqualTo(Matrix.Identity(3, 3)), "CholeskyDecomposition Solve");
        }

        [Test]
        public void MatrixEigenvalueDecomposition()
        {
            double[][] pvals = {
                new double[] { 25, -5, 10 },
                new double[] { -5, 17, 10 },
                new double[] { 10, 10, 62 }
            };

            double[][] evals = {
                new double[] { 0.0, 1.0, 0.0, 0.0 },
                new double[] { 1.0, 0.0, 2e-7, 0.0 },
                new double[] { 0.0, -2e-7, 0.0, 1.0 },
                new double[] { 0.0, 0.0, 1.0, 0.0 }
            };

            Matrix e = new Matrix(pvals);
            EigenvalueDecomposition eig = e.EigenvalueDecomposition;
            Matrix eigd = eig.BlockDiagonal;
            Matrix eigv = eig.EigenVectors;
            Assert.That(eigv * eigd, NumericIs.AlmostEqualTo(e * eigv), "EigenvalueDecomposition (symmetric)");
            Matrix h = new Matrix(evals);
            eig = h.EigenvalueDecomposition;
            eigd = eig.BlockDiagonal;
            eigv = eig.EigenVectors;
            Assert.That(eigv * eigd, NumericIs.AlmostEqualTo(h * eigv, 1e-14), "EigenvalueDecomposition (nonsymmetric)");
        }
    }
}
