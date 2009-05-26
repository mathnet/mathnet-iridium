//-----------------------------------------------------------------------
// <copyright file="MatrixArithmeticsTest.cs" company="Math.NET Project">
//    Copyright (c) 2002-2009, Christoph Rüegg, Kevin Whitefoot.
//    All Right Reserved.
// </copyright>
// <author>
//    Christoph Rüegg, http://christoph.ruegg.name
//    Kevin Whitefoot, kwhitefoot@hotmail.com
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
    public class MatrixArithmeticsTest
    {
        Matrix ma3x2, mb3x2, mc2x2, md2x4;

        [SetUp]
        public void Setup()
        {
            // MATLAB: ma3x2 = [1 -2;-1 4;5 7]
            ma3x2 = new Matrix(new double[][] {
                new double[] { 1, -2 },
                new double[] { -1, 4 },
                new double[] { 5, 7 }
                });

            // MATLAB: mb3x2 = [10 2.5;-3 -1.5;19 -6]
            mb3x2 = new Matrix(new double[][] {
                new double[] { 10, 2.5 },
                new double[] { -3, -1.5 },
                new double[] { 19, -6 }
                });

            // MATLAB: mc2x2 = [1 2;3 4]
            mc2x2 = new Matrix(new double[][] {
                new double[] { 1, 2 },
                new double[] { 3, 4 }
                });

            // MATLAB: md2x4 = [1 2 -3 12;3 3.1 4 2]
            md2x4 = new Matrix(new double[][] {
                new double[] { 1, 2, -3, 12 },
                new double[] { 3, 3.1, 4, 2 }
                });
        }

        [Test]
        public void TestMatrix_Create()
        {
            double[][] a = { new double[] { 1, 2 }, new double[] { 2, 3 } };
            Matrix ma = Matrix.Create(a);
            double[][] b = { new double[] { 1.0, 2.0 }, new double[] { 2.0, 3.0 } };
            Matrix mb = Matrix.Create(a);

            Assert.That(ma, Is.EqualTo(mb));
            Assert.That(ma.Equals(mb), Is.True);
            Assert.That(ma.AlmostEquals(mb), Is.True);
        }

        [Test]
        public void TestMatrix_Transpose()
        {
            /* 2x3 rectangular case */

            // MATLAB: trans_m = ma3x2'
            Matrix trans_m = new Matrix(new double[][] {
                new double[] { 1, -1, 5 },
                new double[] { -2, 4, 7 }
                });

            Assert.That(Matrix.Transpose(ma3x2), NumericIs.AlmostEqualTo(trans_m), "trans m 1");
            Assert.That(Matrix.Transpose(ma3x2), Is.Not.SameAs(ma3x2));
            Assert.That(Matrix.Transpose(ma3x2).GetArray(), Is.Not.SameAs(ma3x2.GetArray()));

            Matrix trans_inplace = ma3x2.Clone();
            Assert.That(trans_inplace, Is.Not.SameAs(ma3x2));
            Assert.That(trans_inplace.GetArray(), Is.Not.SameAs(ma3x2.GetArray()));

            double[][] internalArray = trans_inplace.GetArray();
            trans_inplace.TransposeInplace();
            Assert.That(trans_inplace, NumericIs.AlmostEqualTo(trans_m), "trans m 2");
            // 2009-05-23: Note, this internal behavior might change in a future release:
            Assert.That(trans_inplace.GetArray(), Is.Not.SameAs(internalArray));
        }

        [Test]
        public void TestMatrix_Additive()
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

            Assert.That(ma3x2 + mb3x2, NumericIs.AlmostEqualTo(sum), "sum 1");
            Matrix sum_inplace = ma3x2.Clone();
            sum_inplace.AddInplace(mb3x2);
            Assert.That(sum_inplace, NumericIs.AlmostEqualTo(sum), "sum 2");

            Matrix diff = new Matrix(new double[][] {
                new double[] { -9, -4.5 },
                new double[] { 2, 5.5 },
                new double[] { -14, 13 }
                });

            Assert.That(ma3x2 - mb3x2, NumericIs.AlmostEqualTo(diff), "diff 1");
            Matrix diff_inplace = ma3x2.Clone();
            diff_inplace.SubtractInplace(mb3x2);
            Assert.That(diff_inplace, NumericIs.AlmostEqualTo(diff), "diff 2");

            Matrix neg_m = new Matrix(new double[][] {
                new double[] { -1, 2 },
                new double[] { 1, -4 },
                new double[] { -5, -7 }
                });

            Assert.That(-ma3x2, NumericIs.AlmostEqualTo(neg_m), "neg 1");
        }

        [Test]
        public void TestMatrix_Multiplicative()
        {
            /*
            MATLAB:
            prod = ma3x2 * md2x4
            prod_s = ma3x2 * 2
            */

            Matrix prod = new Matrix(new double[][] {
                new double[] { -5, -4.2, -11, 8 },
                new double[] { 11, 10.4, 19, -4 },
                new double[] { 26, 31.7, 13, 74 }
                });

            Assert.That(ma3x2 * md2x4, NumericIs.AlmostEqualTo(prod), "prod 1");
            Assert.That(ma3x2.Multiply(md2x4), NumericIs.AlmostEqualTo(prod), "prod 2");

            Matrix prod_s = new Matrix(new double[][] {
                new double[] { 2, -4 },
                new double[] { -2, 8 },
                new double[] { 10, 14 }
                });

            Assert.That(ma3x2 * 2, NumericIs.AlmostEqualTo(prod_s), "prod s 1");
            Matrix prod_s_inplace = ma3x2.Clone();
            prod_s_inplace.MultiplyInplace(2);
            Assert.That(prod_s_inplace, NumericIs.AlmostEqualTo(prod_s), "prod s 2");
        }

        [Test]
        public void TestMatrix_LUDecomposition()
        {
            /*
            MATLAB:
            [L_mc, U_mc, P_mc] = lu(mc2x2)
            P_mcv = P_mc * [0:1:length(P_mc)-1]'
            det(mc2x2)
            [L_mch, U_mch, P_mch] = lu(mc2x2')
            P_mchv = P_mch * [0:1:length(P_mch)-1]'
            det(mc2x2')
            */

            Converter<int, double> int2double = delegate(int i) { return i; };

            LUDecomposition LU = mc2x2.LUDecomposition;
            Matrix L_mc = new Matrix(new double[][] {
                new double[] { 1, 0 },
                new double[] { 1d/3d, 1 }
                });

            Matrix U_mc = new Matrix(new double[][] {
                new double[] { 3, 4 },
                new double[] { 0, 2d/3d }
                });

            Matrix P_mc = new Matrix(new double[][] {
                new double[] { 0, 1 },
                new double[] { 1, 0 }
                });

            Vector P_mcv = new Vector(new double[] { 1, 0 });
            Assert.That(LU.L, NumericIs.AlmostEqualTo(L_mc), "real LU L-matrix");
            Assert.That(LU.U, NumericIs.AlmostEqualTo(U_mc), "real LU U-matrix");
            Assert.That(LU.PermutationMatrix, NumericIs.AlmostEqualTo(P_mc), "real LU permutation matrix");
            Assert.That(LU.PivotVector, NumericIs.AlmostEqualTo(P_mcv), "real LU pivot");
            Assert.That((Vector) Array.ConvertAll(LU.Pivot, int2double), NumericIs.AlmostEqualTo(P_mcv), "real LU pivot II");
            Assert.That(LU.Determinant(), NumericIs.AlmostEqualTo((double) (-2)), "real LU determinant");
            Assert.That(mc2x2.Determinant(), NumericIs.AlmostEqualTo((double) (-2)), "real LU determinant II");
            Assert.That(LU.IsNonSingular, "real LU non-singular");
            Assert.That(LU.L * LU.U, NumericIs.AlmostEqualTo(LU.PermutationMatrix * mc2x2), "real LU product");

            Matrix mc2x2h = Matrix.Transpose(mc2x2);
            LUDecomposition LUH = mc2x2h.LUDecomposition;
            Matrix L_mch = new Matrix(new double[][] {
                new double[] { 1, 0 },
                new double[] { 0.5, 1 }
                });

            Matrix U_mch = new Matrix(new double[][] {
                new double[] { 2, 4 },
                new double[] { 0, 1 }
                });

            Matrix P_mch = new Matrix(new double[][] {
                new double[] { 0, 1 },
                new double[] { 1, 0 }
                });

            Vector P_mchv = new Vector(new double[] { 1, 0 });
            Assert.That(LUH.L, NumericIs.AlmostEqualTo(L_mch), "real LU L-matrix (H)");
            Assert.That(LUH.U, NumericIs.AlmostEqualTo(U_mch), "real LU U-matrix (H)");
            Assert.That(LUH.PermutationMatrix, NumericIs.AlmostEqualTo(P_mch), "real LU permutation matrix (H)");
            Assert.That(LUH.PivotVector, NumericIs.AlmostEqualTo(P_mchv), "real LU pivot (H)");
            Assert.That((Vector) Array.ConvertAll(LUH.Pivot, int2double), NumericIs.AlmostEqualTo(P_mchv), "real LU pivot II (H)");
            Assert.That(LUH.Determinant(), NumericIs.AlmostEqualTo((double) (-2)), "real LU determinant (H)");
            Assert.That(mc2x2h.Determinant(), NumericIs.AlmostEqualTo((double) (-2)), "real LU determinant II (H)");
            Assert.That(LUH.IsNonSingular, "real LU non-singular (H)");
            Assert.That(LUH.L * LUH.U, NumericIs.AlmostEqualTo(LUH.PermutationMatrix * mc2x2h), "real LU product (H)");
        }

        [Test]
        public void TestMatrix_QRDecomposition()
        {
            /*
            MATLAB:
            [Q0, R0] = qr(md2x4',0)
            S_mdh = sign(tril(R0))
            Q_mdh = Q0*S_mdh
            R_mdh = S_mdh(:,1:2)*R0
            [Q0, R0] = qr(md2x4,0)
            S_md = sign(tril(R0))
            Q_md = Q0*S_md
            R_md = S_md(:,1:2)*R0
            */

            Matrix md2x4h = Matrix.Transpose(md2x4);
            QRDecomposition QRH = md2x4h.QRDecomposition;
            Matrix Q_mdh = new Matrix(new double[][] {
                new double[] { 0.07955572841757, 0.47920079143039 },
                new double[] { 0.15911145683515, 0.47348594100255 },
                new double[] { -0.23866718525272, 0.73615739955605 },
                new double[] { 0.95466874101088, 0.06519162710272 }
                });

            Matrix R_mdh = new Matrix(new double[][] {
                new double[] { 12.56980508997654, 1.68658144245255 },
                new double[] { 0, 5.98042164382869 }
                });

            Matrix PInv_mdh = new Matrix(new double[][] {
                new double[] { -0.00442227310854, 0.00203505965379, -0.03550382768177, 0.07448672256297 },
                new double[] { 0.08012826184670, 0.07917266861796, 0.12309456479807, 0.01090084127596 }
                });

            Assert.That(QRH.Q, NumericIs.AlmostEqualTo(Q_mdh, 1e-14), "real QR Q-matrix (H)");
            Assert.That(QRH.R, NumericIs.AlmostEqualTo(R_mdh), "real QR R-matrix (H)");
            Assert.That(QRH.Q * QRH.R, NumericIs.AlmostEqualTo(md2x4h), "real QR product (H)");
            Assert.That(Matrix.Transpose(QRH.Q) * QRH.Q, NumericIs.AlmostEqualTo(Matrix.Identity(2, 2)), "real QR QHQ=I (H)");
            Assert.That(QRH.Solve(Matrix.Identity(4, 4)), NumericIs.AlmostEqualTo(PInv_mdh, 1e-13), "real QR Solve/Pseudoinverse (H)");
            Assert.That(md2x4h.Inverse(), NumericIs.AlmostEqualTo(PInv_mdh, 1e-13), "real pseudoinverse (H)");

            QRDecomposition QR = md2x4.QRDecomposition;
            Matrix Q_md = new Matrix(new double[][] {
                new double[] { 0.31622776601684, 0.94868329805051 },
                new double[] { 0.94868329805051, -0.31622776601684 }
                });

            Matrix R_md = new Matrix(new double[][] {
                new double[] { 3.16227766016838, 3.57337375599027, 2.84604989415154, 5.69209978830308 },
                new double[] { 0, 0.91706052144883, -4.11096095821889, 10.75174404457249 }
                });

            Matrix PInv_md = new Matrix(new double[][] {
                new double[] { -0.00442227310854, 0.08012826184670 },
                new double[] { 0.00203505965379, 0.07917266861796 },
                new double[] { -0.03550382768177, 0.12309456479807 },
                new double[] { 0.07448672256297, 0.01090084127596 }
                });

            Assert.That(QR.Q, NumericIs.AlmostEqualTo(Q_md, 1e-14), "real QR Q-matrix");
            Assert.That(QR.R, NumericIs.AlmostEqualTo(R_md), "real QR R-matrix");
            Assert.That(QR.Q * QR.R, NumericIs.AlmostEqualTo(md2x4), "real QR product");
            Assert.That(Matrix.Transpose(QR.Q) * QR.Q, NumericIs.AlmostEqualTo(Matrix.Identity(2, 2)), "real QR QHQ=I");
            /*
            NOTE: QR.Solve won't work yet (LQ would be required instead of QR).
            Hence check the matrix Solve instead, which is supposed to compute the transposed QR in this case.
            NumericAssert.AreAlmostEqual(PInv_md, md2x4.Solve(Matrix.Identity(2, 2)), 1e-13, "real QR Solve/Pseudoinverse");
            */
            Assert.That(md2x4.Inverse(), NumericIs.AlmostEqualTo(PInv_md, 1e-13), "real pseudoinverse");
        }

        [Test]
        public void TestMatrix_Solve()
        {
            double[][] a = { new double[] { 1, 2 }, new double[] { 3, 5 } };
            Matrix ma = Matrix.Create(a);
            double[][] b = { new double[] { 29.0 }, new double[] { 76.0 } };
            Matrix mb = Matrix.Create(b);
            double[][] r = { new double[] { 7 }, new double[] { 11.0 } };
            Matrix mr = Matrix.Create(r);
            ////Console.WriteLine("a");
            ////Console.WriteLine(ma.ToString());
            ////Console.WriteLine("b");
            ////Console.WriteLine(mb.ToString());
            Matrix mx = null;
            ////MyStopwatch.MethodToTime m = delegate
            ////{
            mx = ma.Solve(mb);
            ////};
            ////Console.Write("Solve Time (ms): ");
            ////MyStopwatch.Time(m);

            ////Console.WriteLine("solution");
            ////Console.WriteLine(mx.ToString());
            ////Console.WriteLine("expected solution");
            ////Console.WriteLine(mr.ToString());

            Assert.That(mr.ToString(), Is.EqualTo(mx.ToString()), "Matrices should be equal");

            // Check by multiplying a by x
            Matrix mc = ma * mx;
            Assert.That(mb.ToString(), Is.EqualTo(mc.ToString()), "Matrices should be equal");
        }

        [Test]
        public void TestMatrix_SolveA()
        {
            TestMatrix(
                new double[][] { new double[] { 1, 2 }, new double[] { 3, 5 } },
                new double[][] { new double[] { 7 }, new double[] { 11.0 } },
                1e-13,
                false);
        }

        [Test]
        public void TestMatrix_SolveB()
        {
            TestMatrix(
                new double[][] { new double[] { 1,  2,  3 }, new double[] { 5,  7, 11 }, new double[] { 13, 17, 19 } },
                new double[][] { new double[] { 23 }, new double[] { 29 }, new double[] { 31 } },
                1e-13,
                false);
        }

        [Test]
        public void TestMatrix_Solve010()
        {
            TestMatrix_NxN(10, 1e-12, false);
        }

        [Test]
        public void TestMatrix_Solve020()
        {
            TestMatrix_NxN(20, 1e-12, false);
        }

        [Test]
        public void TestMatrix_Solve040()
        {
            TestMatrix_NxN(40, 1e-9, false);
        }

        [Test]
        public void TestMatrix_Solve060()
        {
            TestMatrix_NxN(60, 1e-9, false);
        }

        [Test]
        public void TestMatrix_Solve080()
        {
            TestMatrix_NxN(80, 1e-9, false);
        }

        [Test]
        public void TestMatrix_Solve100()
        {
            TestMatrix_NxN(100, 1e-9, false);
        }

        [Test]
        public void TestMatrix_Solve110()
        {
            TestMatrix_NxN(110, 1e-9, false);
        }

        [Test]
        public void TestMatrix_Solve120()
        {
            TestMatrix_NxN(120, 1e-9, false);
        }

        [Test]
        public void TestMatrix_Solve150()
        {
            TestMatrix_NxN(150, 1e-9, false);
        }

        [Test]
        public void TestMatrix_Solve200()
        {
            TestMatrix_NxN(200, 1e-9, false);
        }

        [Test]
        public void TestMatrix_Solve330()
        {
            TestMatrix_NxN(330, 1e-9, false);
        }

        ////[Test]
        ////public void TestMatrix_Solve1000()
        ////{
        ////    TestMatrix_NxN(1000, 1e-9, false);
        ////}

        ////[Test]
        ////public void TestMatrix_Solve4000()
        ////{
        ////    TestMatrix_NxN(4000, 1e-9, false);
        ////}

        private static void TestMatrix_NxN(int n, double epsilon, bool show)
        {
            Random r = new Random();
            double[][] a = Matrix.CreateMatrixData(n, n);
            double[][] x = Matrix.CreateMatrixData(n, 1);
            for(int i = 0; i < n; i++)
            {
                for(int j = 0; j < n; j++)
                {
                    if(j == 0)
                    {
                        x[i][0] = r.NextDouble();
                    }

                    a[i][j] = r.NextDouble();
                }
            }

            TestMatrix(a, x, epsilon, show);
        }

        // Calculate right hand side and then solve for x, show all matrices on console out.
        private static void TestMatrix(double[][] a, double[][] x, double epsilon, bool show)
        {
            Matrix ma = Matrix.Create(a);
            Matrix mx = Matrix.Create(x);
            if(show)
            {
                Console.WriteLine("a");
                Console.WriteLine(ma.ToString());
                Console.WriteLine("x");
                Console.WriteLine(mx.ToString());
            }

            Matrix ms = TestMatrix_Solutions(ma, mx, epsilon, show);
            if(show)
            {
                Console.WriteLine("solution");
                Console.WriteLine(ms.ToString());
                Console.WriteLine("expected solution");
                Console.WriteLine(mx.ToString());
            }
        }

        /*Test a given solution by calculating b and then solving for x.
        Shows only the elapsed time on console out so that we can use 
        matrices too large to print.*/
        private static Matrix TestMatrix_Solutions(Matrix ma, Matrix mx, double epsilon, bool showB)
        {
            Matrix mb = ma * mx;
            if(showB)
            {
                Console.WriteLine("b");
                Console.WriteLine(mb.ToString());
            }

            Matrix ms = null;
            ////MyStopwatch.MethodToTime m = delegate
            ////{
            ms = ma.Solve(mb);
            ////};
            ////Console.Write("Solve Time (ms) for " + ma.ColumnCount + ": ");
            ////MyStopwatch.Time(m);

            Assert.That(CompareMatrices(ms, mx, epsilon), "Matrices should be equal");
            ////Assert.That(mx.ToString(), Is.EqualTo(ms.ToString()), "Matrices should be equal");

            return ms;
        }

        private static bool CompareMatrices(Matrix a, Matrix b, double epsilon)
        {
            Matrix c = a - b;
            for(int i = 0; i < c.RowCount; i++)
            {
                for(int j = 0; j < c.ColumnCount; j++)
                {
                    if(epsilon < Math.Abs(c[i, j]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
