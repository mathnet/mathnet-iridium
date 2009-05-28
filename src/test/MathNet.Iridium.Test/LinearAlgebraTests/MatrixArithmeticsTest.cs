//-----------------------------------------------------------------------
// <copyright file="MatrixArithmeticsTest.cs" company="Math.NET Project">
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

    [TestFixture]
    public class MatrixArithmeticsTest
    {
        Matrix _ma3X2, _mb3X2, _md2X4;

        [SetUp]
        public void Setup()
        {
            // MATLAB: ma3x2 = [1 -2;-1 4;5 7]
            _ma3X2 = new Matrix(new double[][] {
                new double[] { 1, -2 },
                new double[] { -1, 4 },
                new double[] { 5, 7 }
                });

            // MATLAB: mb3x2 = [10 2.5;-3 -1.5;19 -6]
            _mb3X2 = new Matrix(new double[][] {
                new double[] { 10, 2.5 },
                new double[] { -3, -1.5 },
                new double[] { 19, -6 }
                });

            // MATLAB: md2x4 = [1 2 -3 12;3 3.1 4 2]
            _md2X4 = new Matrix(new double[][] {
                new double[] { 1, 2, -3, 12 },
                new double[] { 3, 3.1, 4, 2 }
                });
        }

        [Test]
        public void MatrixCreate()
        {
            double[][] a = { new double[] { 1, 2 }, new double[] { 2, 3 } };
            Matrix ma = new Matrix(a);
            double[][] b = { new double[] { 1.0, 2.0 }, new double[] { 2.0, 3.0 } };
            Matrix mb = Matrix.Create(b);

            Assert.That(ma, Is.EqualTo(mb));
            Assert.That(ma.Equals(mb), Is.True);
            Assert.That(ma.AlmostEquals(mb), Is.True);
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
            Assert.That(u, Is.Not.EqualTo(Matrix.Identity(3, 4)));
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
        public void MatrixTranspose1()
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
        public void MatrixTranspose2()
        {
            /* 2x3 rectangular case */

            // MATLAB: trans_m = ma3x2'
            Matrix u = new Matrix(new double[][] {
                new double[] { 1, -1, 5 },
                new double[] { -2, 4, 7 }
                });

            Assert.That(Matrix.Transpose(_ma3X2), NumericIs.AlmostEqualTo(u), "trans m 1");
            Assert.That(Matrix.Transpose(_ma3X2), Is.Not.SameAs(_ma3X2));
            Assert.That(Matrix.Transpose(_ma3X2).GetArray(), Is.Not.SameAs(_ma3X2.GetArray()));

            Matrix uInplace = _ma3X2.Clone();
            Assert.That(uInplace, Is.Not.SameAs(_ma3X2));
            Assert.That(uInplace.GetArray(), Is.Not.SameAs(_ma3X2.GetArray()));

            double[][] internalArray = uInplace.GetArray();
            uInplace.TransposeInplace();
            Assert.That(uInplace, NumericIs.AlmostEqualTo(u), "trans m 2");
            // 2009-05-23: Note, this internal behavior might change in a future release:
            Assert.That(uInplace.GetArray(), Is.Not.SameAs(internalArray));
        }

        [Test]
        public void MatrixAddition()
        {
            /*
            MATLAB:
            sum = ma3x2 + mb3x2
            diff = ma3x2 - mb3x2
            neg_m = -ma3x2
            */

            Matrix sum = new Matrix(new double[][] {
                new double[] { 11, 0.5 },
                new double[] { -4, 2.5 },
                new double[] { 24, 1 }
                });

            Assert.That(_ma3X2 + _mb3X2, NumericIs.AlmostEqualTo(sum), "sum 1");
            Matrix sumInplace = _ma3X2.Clone();
            sumInplace.AddInplace(_mb3X2);
            Assert.That(sumInplace, NumericIs.AlmostEqualTo(sum), "sum 2");

            Matrix diff = new Matrix(new double[][] {
                new double[] { -9, -4.5 },
                new double[] { 2, 5.5 },
                new double[] { -14, 13 }
                });

            Assert.That(_ma3X2 - _mb3X2, NumericIs.AlmostEqualTo(diff), "diff 1");
            Matrix diffInplace = _ma3X2.Clone();
            diffInplace.SubtractInplace(_mb3X2);
            Assert.That(diffInplace, NumericIs.AlmostEqualTo(diff), "diff 2");

            Matrix negative = new Matrix(new double[][] {
                new double[] { -1, 2 },
                new double[] { 1, -4 },
                new double[] { -5, -7 }
                });

            Assert.That(-_ma3X2, NumericIs.AlmostEqualTo(negative), "neg 1");
        }

        [Test]
        public void MatrixMultiplication()
        {
            /*
            MATLAB:
            prod = ma3x2 * md2x4
            prod_s = ma3x2 * 2
            */

            Matrix product = new Matrix(new double[][] {
                new double[] { -5, -4.2, -11, 8 },
                new double[] { 11, 10.4, 19, -4 },
                new double[] { 26, 31.7, 13, 74 }
                });

            Assert.That(_ma3X2 * _md2X4, NumericIs.AlmostEqualTo(product), "prod 1");
            Assert.That(_ma3X2.Multiply(_md2X4), NumericIs.AlmostEqualTo(product), "prod 2");

            Matrix scaled = new Matrix(new double[][] {
                new double[] { 2, -4 },
                new double[] { -2, 8 },
                new double[] { 10, 14 }
                });

            Assert.That(_ma3X2 * 2, NumericIs.AlmostEqualTo(scaled), "prod s 1");
            Matrix scaledInplace = _ma3X2.Clone();
            scaledInplace.MultiplyInplace(2);
            Assert.That(scaledInplace, NumericIs.AlmostEqualTo(scaled), "prod s 2");
        }

        [Test]
        public void MatrixMultiplicationByDiagonal()
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
        public void MatrixMultiplicationByMatrix()
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
            Assert.That(delegate { r.ArrayDivideInplace(Matrix.Ones(4)); }, Throws.TypeOf<ArgumentException>());
            Assert.That(Matrix.ArrayDivide(Matrix.Zeros(4), Matrix.Ones(4)), Is.EqualTo(Matrix.Zeros(4)));
            Matrix d = r.Clone();
            d.ArrayDivideInplace(r);
            Assert.That(d, NumericIs.AlmostEqualTo(new Matrix(r.RowCount, r.ColumnCount, 1.0)));

            // array-multiplication
            Assert.That(delegate { Matrix.ArrayMultiply(Matrix.Ones(3), Matrix.Ones(4)); }, Throws.TypeOf<ArgumentException>());
            Assert.That(delegate { r.ArrayMultiplyInplace(Matrix.Ones(4)); }, Throws.TypeOf<ArgumentException>());
            Assert.That(Matrix.ArrayMultiply(r, new Matrix(r.RowCount, r.ColumnCount, 1.0)), Is.EqualTo(r));
            Matrix e = r.Clone();
            e.ArrayMultiplyInplace(new Matrix(r.RowCount, r.ColumnCount));
            Assert.That(e, Is.EqualTo(new Matrix(r.RowCount, r.ColumnCount)));

            // mixed array multiplication & division
            Matrix f = r.Clone();
            f.ArrayMultiplyInplace(r2);
            f.ArrayDivideInplace(r2);
            Assert.That(f, NumericIs.AlmostEqualTo(r));
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
    }
}
