//-----------------------------------------------------------------------
// <copyright file="MatrixDecompositionTest.cs" company="Math.NET Project">
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
    public class MatrixDecompositionTest
    {
        static readonly IDiscreteGenerator SizeRandom = new DiscreteUniformDistribution(8, 13);
        static readonly IContinuousGenerator ValueRandom = new StableDistribution();

        [Test]
        public void MatrixSolveRobust()
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

        [Test]
        public void MatrixSolve1()
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
        public void MatrixSolve2()
        {
            Matrix ma = new Matrix(new double[][] { new double[] { 1, 2 }, new double[] { 3, 5 } });
            Matrix mb = new Matrix(new double[][] { new double[] { 29.0 }, new double[] { 76.0 } });
            Matrix mr = new Matrix(new double[][] { new double[] { 7 }, new double[] { 11.0 } });
            Matrix mx = ma.Solve(mb);
            Assert.That(mx, NumericIs.AlmostEqualTo(mr));
            Assert.That(ma * mx, NumericIs.AlmostEqualTo(mb));
        }

        [Test]
        public void MatrixSolve3()
        {
            Matrix ma = new Matrix(new double[][] { new double[] { 1, 2 }, new double[] { 3, 5 } });
            Matrix mx = new Matrix(new double[][] { new double[] { 7 }, new double[] { 11.0 } });
            Matrix mb = ma * mx;
            Assert.That(ma.Solve(mb), NumericIs.AlmostEqualTo(mx));
        }

        [Test]
        public void MatrixSolve4()
        {
            Matrix ma = new Matrix(new double[][] { new double[] { 1, 2, 3 }, new double[] { 5, 7, 11 }, new double[] { 13, 17, 19 } });
            Matrix mx = new Matrix(new double[][] { new double[] { 23 }, new double[] { 29 }, new double[] { 31 } });
            Matrix mb = ma * mx;
            Assert.That(ma.Solve(mb), NumericIs.AlmostEqualTo(mx));
        }

        [Test]
        public void MatrixLUDecomposition1()
        {
            double[] columnwise = { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 11.0, 12.0 };
            Matrix b = new Matrix(columnwise, 4);
            b[0, 0] = 0.0;

            LUDecomposition lu = b.LUDecomposition;
            Assert.That(lu.L * lu.U, NumericIs.AlmostEqualTo(b.GetMatrix(lu.Pivot, 0, b.ColumnCount - 1)), "LUDecomposition");
        }

        [Test]
        public void MatrixLUDecomposition2()
        {
            /*
            MATLAB:
            mc2x2 = [1 2;3 4]
            [L_mc, U_mc, P_mc] = lu(mc2x2)
            P_mcv = P_mc * [0:1:length(P_mc)-1]'
            det(mc2x2)
            [L_mch, U_mch, P_mch] = lu(mc2x2')
            P_mchv = P_mch * [0:1:length(P_mch)-1]'
            det(mc2x2')
            */

            Converter<int, double> int2Double = delegate(int i) { return i; };

            Matrix mc2X2 = new Matrix(new double[][]
                {
                    new double[] { 1, 2 },
                    new double[] { 3, 4 }
                });

            LUDecomposition mcLU = mc2X2.LUDecomposition;
            Matrix mcL = new Matrix(new double[][] {
                new double[] { 1, 0 },
                new double[] { 1d/3d, 1 }
                });

            Matrix mcU = new Matrix(new double[][] {
                new double[] { 3, 4 },
                new double[] { 0, 2d/3d }
                });

            Matrix mcP = new Matrix(new double[][] {
                new double[] { 0, 1 },
                new double[] { 1, 0 }
                });

            Vector mcPv = new Vector(new double[] { 1, 0 });
            Assert.That(mcLU.L, NumericIs.AlmostEqualTo(mcL), "real LU L-matrix");
            Assert.That(mcLU.U, NumericIs.AlmostEqualTo(mcU), "real LU U-matrix");
            Assert.That(mcLU.PermutationMatrix, NumericIs.AlmostEqualTo(mcP), "real LU permutation matrix");
            Assert.That(mcLU.PivotVector, NumericIs.AlmostEqualTo(mcPv), "real LU pivot");
            Assert.That((Vector)Array.ConvertAll(mcLU.Pivot, int2Double), NumericIs.AlmostEqualTo(mcPv), "real LU pivot II");
            Assert.That(mcLU.Determinant(), NumericIs.AlmostEqualTo((double)(-2)), "real LU determinant");
            Assert.That(mc2X2.Determinant(), NumericIs.AlmostEqualTo((double)(-2)), "real LU determinant II");
            Assert.That(mcLU.IsNonSingular, "real LU non-singular");
            Assert.That(mcLU.L * mcLU.U, NumericIs.AlmostEqualTo(mcLU.PermutationMatrix * mc2X2), "real LU product");

            Matrix mc2X2H = Matrix.Transpose(mc2X2);
            LUDecomposition mchLU = mc2X2H.LUDecomposition;
            Matrix mchL = new Matrix(new double[][] {
                new double[] { 1, 0 },
                new double[] { 0.5, 1 }
                });

            Matrix mchU = new Matrix(new double[][] {
                new double[] { 2, 4 },
                new double[] { 0, 1 }
                });

            Matrix mchP = new Matrix(new double[][] {
                new double[] { 0, 1 },
                new double[] { 1, 0 }
                });

            Vector mchPv = new Vector(new double[] { 1, 0 });
            Assert.That(mchLU.L, NumericIs.AlmostEqualTo(mchL), "real LU L-matrix (H)");
            Assert.That(mchLU.U, NumericIs.AlmostEqualTo(mchU), "real LU U-matrix (H)");
            Assert.That(mchLU.PermutationMatrix, NumericIs.AlmostEqualTo(mchP), "real LU permutation matrix (H)");
            Assert.That(mchLU.PivotVector, NumericIs.AlmostEqualTo(mchPv), "real LU pivot (H)");
            Assert.That((Vector)Array.ConvertAll(mchLU.Pivot, int2Double), NumericIs.AlmostEqualTo(mchPv), "real LU pivot II (H)");
            Assert.That(mchLU.Determinant(), NumericIs.AlmostEqualTo((double)(-2)), "real LU determinant (H)");
            Assert.That(mc2X2H.Determinant(), NumericIs.AlmostEqualTo((double)(-2)), "real LU determinant II (H)");
            Assert.That(mchLU.IsNonSingular, "real LU non-singular (H)");
            Assert.That(mchLU.L * mchLU.U, NumericIs.AlmostEqualTo(mchLU.PermutationMatrix * mc2X2H), "real LU product (H)");
        }

        [Test]
        public void MatrixQRDecomposition1()
        {
            double[] columnwise = { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 11.0, 12.0 };

            Matrix b = new Matrix(columnwise, 4);
            QRDecomposition qr = b.QRDecomposition;
            Matrix r = qr.R;
            Assert.That(qr.Q * r, NumericIs.AlmostEqualTo(b), "QRDecomposition");
        }

        [Test]
        public void MatrixQRDecomposition2()
        {
            /*
            MATLAB:
            md2x4 = [1 2 -3 12;3 3.1 4 2]
            [Q0, R0] = qr(md2x4',0)
            S_mdh = sign(tril(R0))
            Q_mdh = Q0*S_mdh
            R_mdh = S_mdh(:,1:2)*R0
            [Q0, R0] = qr(md2x4,0)
            S_md = sign(tril(R0))
            Q_md = Q0*S_md
            R_md = S_md(:,1:2)*R0
            */

            Matrix md2X4 = new Matrix(new double[][]
                {
                    new double[] { 1, 2, -3, 12 },
                    new double[] { 3, 3.1, 4, 2 }
                });

            Matrix md2X4H = Matrix.Transpose(md2X4);
            QRDecomposition mdhQR = md2X4H.QRDecomposition;
            Matrix mdhQ = new Matrix(new double[][] {
                new double[] { 0.07955572841757, 0.47920079143039 },
                new double[] { 0.15911145683515, 0.47348594100255 },
                new double[] { -0.23866718525272, 0.73615739955605 },
                new double[] { 0.95466874101088, 0.06519162710272 }
                });

            Matrix mdhR = new Matrix(new double[][] {
                new double[] { 12.56980508997654, 1.68658144245255 },
                new double[] { 0, 5.98042164382869 }
                });

            Matrix mdhPInv = new Matrix(new double[][] {
                new double[] { -0.00442227310854, 0.00203505965379, -0.03550382768177, 0.07448672256297 },
                new double[] { 0.08012826184670, 0.07917266861796, 0.12309456479807, 0.01090084127596 }
                });

            Assert.That(mdhQR.Q, NumericIs.AlmostEqualTo(mdhQ, 1e-14), "real QR Q-matrix (H)");
            Assert.That(mdhQR.R, NumericIs.AlmostEqualTo(mdhR), "real QR R-matrix (H)");
            Assert.That(mdhQR.Q * mdhQR.R, NumericIs.AlmostEqualTo(md2X4H), "real QR product (H)");
            Assert.That(Matrix.Transpose(mdhQR.Q) * mdhQR.Q, NumericIs.AlmostEqualTo(Matrix.Identity(2, 2)), "real QR QHQ=I (H)");
            Assert.That(mdhQR.Solve(Matrix.Identity(4, 4)), NumericIs.AlmostEqualTo(mdhPInv, 1e-13), "real QR Solve/Pseudoinverse (H)");
            Assert.That(md2X4H.Inverse(), NumericIs.AlmostEqualTo(mdhPInv, 1e-13), "real pseudoinverse (H)");

            QRDecomposition mdQR = md2X4.QRDecomposition;
            Matrix mdQ = new Matrix(new double[][] {
                new double[] { 0.31622776601684, 0.94868329805051 },
                new double[] { 0.94868329805051, -0.31622776601684 }
                });

            Matrix mdR = new Matrix(new double[][] {
                new double[] { 3.16227766016838, 3.57337375599027, 2.84604989415154, 5.69209978830308 },
                new double[] { 0, 0.91706052144883, -4.11096095821889, 10.75174404457249 }
                });

            Matrix mdPInv = new Matrix(new double[][] {
                new double[] { -0.00442227310854, 0.08012826184670 },
                new double[] { 0.00203505965379, 0.07917266861796 },
                new double[] { -0.03550382768177, 0.12309456479807 },
                new double[] { 0.07448672256297, 0.01090084127596 }
                });

            Assert.That(mdQR.Q, NumericIs.AlmostEqualTo(mdQ, 1e-14), "real QR Q-matrix");
            Assert.That(mdQR.R, NumericIs.AlmostEqualTo(mdR), "real QR R-matrix");
            Assert.That(mdQR.Q * mdQR.R, NumericIs.AlmostEqualTo(md2X4), "real QR product");
            Assert.That(Matrix.Transpose(mdQR.Q) * mdQR.Q, NumericIs.AlmostEqualTo(Matrix.Identity(2, 2)), "real QR QHQ=I");
            /*
            NOTE: QR.Solve won't work yet (LQ would be required instead of QR).
            Hence check the matrix Solve instead, which is supposed to compute the transposed QR in this case.
            NumericAssert.AreAlmostEqual(PInv_md, md2x4.Solve(Matrix.Identity(2, 2)), 1e-13, "real QR Solve/Pseudoinverse");
            */
            Assert.That(md2X4.Inverse(), NumericIs.AlmostEqualTo(mdPInv, 1e-13), "real pseudoinverse");
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

        [Test, Repeat(20)]
        public void MatrixSingularValueDecompositionRandom()
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
