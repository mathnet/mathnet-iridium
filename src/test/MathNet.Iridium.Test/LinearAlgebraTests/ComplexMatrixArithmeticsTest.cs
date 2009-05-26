//-----------------------------------------------------------------------
// <copyright file="ComplexMatrixArithmeticsTest.cs" company="Math.NET Project">
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

using NUnit.Framework;

namespace Iridium.Test.LinearAlgebraTests
{
    using MathNet.Numerics;
    using MathNet.Numerics.LinearAlgebra;

    [TestFixture]
    public class ComplexMatrixArithmeticsTest
    {
        readonly Complex j = Complex.I;
        readonly Complex s = new Complex(1, 1);
        Matrix ma3x2, mb3x2, mc2x2, md2x4;
        ComplexMatrix ra3x2, rb3x2, rc2x2, rd2x4;
        ComplexMatrix ia3x2, ib3x2, ic2x2, id2x4;
        ComplexMatrix ca3x2, cb3x2, cc2x2, cd2x4;
        Vector v2;
        ComplexVector cv2;

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

            // MATLAB: ra3x2 = ma3x2 + 2
            ra3x2 = ComplexMatrix.Create(ma3x2) + 2;

            // MATLAB: rb3x2 = mb3x2 - 1
            rb3x2 = ComplexMatrix.Create(mb3x2) - 1;

            // MATLAB: rc2x2 = mc2x2 + 5
            rc2x2 = ComplexMatrix.Create(mc2x2) + 5;

            // MATLAB: rd2x4 = md2x4 * 2
            rd2x4 = ComplexMatrix.Create(md2x4) * 2;

            // MATLAB:  ia3x2 = (ra3x2 * 2) * j
            ia3x2 = (ra3x2 * 2) * j;

            // MATLAB: ib3x2 = (rb3x2 * 3 + 1) * j
            ib3x2 = ((rb3x2 * 3) + 1) * j;

            // MATLAB: ic2x2 = (rc2x2 + 2) * j
            ic2x2 = (rc2x2 + 2) * j;

            // MATLAB: id2x4 = (rd2x4 - 5) * j
            id2x4 = (rd2x4 - 5) * j;

            // MATLAB: ca3x2 = 2*ra3x2 - 2*ia3x2
            ca3x2 = (2 * ra3x2) - (2 * ia3x2);

            // MATLAB: cb3x2 = rb3x2 + 3*ib3x2
            cb3x2 = rb3x2 + (3 * ib3x2);

            // MATLAB: cc2x2 = rc2x2 + 2 - 3*ic2x2
            cc2x2 = rc2x2 + 2 - (3 * ic2x2);

            // MATLAB: cd2x4 = -2*rd2x4 + id2x4 + 1-j
            cd2x4 = (-2 * rd2x4) + id2x4 + (1 - j);

            // MATLAB: v2 = [5 -2]
            v2 = new Vector(new double[] { 5, -2 });

            // MATLAB: cv2 = [5+j, -2+3j]
            cv2 = new ComplexVector(new Complex[] { 5 + j, -2 + (3 * j) });
        }

        [Test]
        public void TestComplexMatrix_Transpose()
        {
            /* 2x3 rectangular case */

            // MATLAB: trans_c = ca3x2.'
            ComplexMatrix trans_c = new ComplexMatrix(new Complex[][] {
                new Complex[] { 6-(12*j), 2-(4*j), 14-(28*j) },
                new Complex[] { 0, 12-(24*j), 18-(36*j) }
                });

            Assert.That(ca3x2.Transpose(), NumericIs.AlmostEqualTo(trans_c), "trans c 1");
            Assert.That(ca3x2.Transpose(), Is.Not.SameAs(ca3x2));
            Assert.That(ca3x2.Transpose().GetArray(), Is.Not.SameAs(ca3x2.GetArray()));

            /* 2x2 square case */

            // MATLAB: trans_c2 = cc2x2.'
            ComplexMatrix trans_c2 = new ComplexMatrix(new Complex[][] {
                new Complex[] { 8-(24*j), 10-(30*j) },
                new Complex[] { 9-(27*j), 11-(33*j) }
                });

            Assert.That(cc2x2.Transpose(), NumericIs.AlmostEqualTo(trans_c2), "trans c2 1");
            Assert.That(cc2x2.Transpose(), Is.Not.SameAs(cc2x2));
            Assert.That(cc2x2.Transpose().GetArray(), Is.Not.SameAs(cc2x2.GetArray()));

            ComplexMatrix trans_c2_inplace = cc2x2.Clone();
            Assert.That(trans_c2_inplace, Is.Not.SameAs(cc2x2));
            Assert.That(trans_c2_inplace.GetArray(), Is.Not.SameAs(cc2x2.GetArray()));

            Complex[][] internalArray = trans_c2_inplace.GetArray();
            trans_c2_inplace.TransposeInplace();
            Assert.That(trans_c2_inplace, NumericIs.AlmostEqualTo(trans_c2), "trans c2 2");
            Assert.That(trans_c2_inplace.GetArray(), Is.SameAs(internalArray));
        }

        [Test]
        public void TestComplexMatrix_HermitianTranspose()
        {
            /* 2x3 rectangular case */

            // MATLAB: htrans_c = ca3x2'
            ComplexMatrix htrans_c = new ComplexMatrix(new Complex[][] {
                new Complex[] { 6+(12*j), 2+(4*j), 14+(28*j) },
                new Complex[] { 0, 12+(24*j), 18+(36*j) }
                });

            Assert.That(ca3x2.HermitianTranspose(), NumericIs.AlmostEqualTo(htrans_c), "htrans c 1");
            Assert.That(ca3x2.HermitianTranspose(), Is.Not.SameAs(ca3x2));
            Assert.That(ca3x2.HermitianTranspose().GetArray(), Is.Not.SameAs(ca3x2.GetArray()));

            /* 2x2 square case */

            // MATLAB: htrans_c2 = cc2x2'
            ComplexMatrix htrans_c2 = new ComplexMatrix(new Complex[][] {
                new Complex[] { 8+(24*j), 10+(30*j) },
                new Complex[] { 9+(27*j), 11+(33*j) }
                });

            Assert.That(cc2x2.HermitianTranspose(), NumericIs.AlmostEqualTo(htrans_c2), "htrans c2 1");
            Assert.That(cc2x2.HermitianTranspose(), Is.Not.SameAs(cc2x2));
            Assert.That(cc2x2.HermitianTranspose().GetArray(), Is.Not.SameAs(cc2x2.GetArray()));

            ComplexMatrix htrans_c2_inplace = cc2x2.Clone();
            Assert.That(htrans_c2_inplace, Is.Not.SameAs(cc2x2));
            Assert.That(htrans_c2_inplace.GetArray(), Is.Not.SameAs(cc2x2.GetArray()));

            Complex[][] internalArray = htrans_c2_inplace.GetArray();
            htrans_c2_inplace.HermitianTransposeInplace();
            Assert.That(htrans_c2_inplace, NumericIs.AlmostEqualTo(htrans_c2), "htrans c2 2");
            Assert.That(htrans_c2_inplace.GetArray(), Is.SameAs(internalArray));
        }

        [Test]
        public void TestComplexMatrix_Conjugate()
        {
            // MATLAB: conj_c = conj(ca3x2)
            ComplexMatrix conj_c = new ComplexMatrix(new Complex[][] {
                new Complex[] { 6+(12*j), 0 },
                new Complex[] { 2+(4*j), 12+(24*j) },
                new Complex[] { 14+(28*j), 18+(36*j) }
                });

            Assert.That(ca3x2.Conjugate(), NumericIs.AlmostEqualTo(conj_c), "conj c 1");
            Assert.That(ca3x2.Conjugate(), Is.Not.SameAs(ca3x2));
            Assert.That(ca3x2.Conjugate().GetArray(), Is.Not.SameAs(ca3x2.GetArray()));

            ComplexMatrix conj_c_inplace = ca3x2.Clone();
            Complex[][] internalArray = conj_c_inplace.GetArray();
            conj_c_inplace.ConjugateInplace();
            Assert.That(conj_c_inplace, NumericIs.AlmostEqualTo(conj_c), "conj c 2");
            Assert.That(internalArray, Is.Not.SameAs(ca3x2.GetArray()));
            Assert.That(internalArray, Is.SameAs(conj_c_inplace.GetArray()));
        }

        [Test]
        public void TestComplexMatrix_Additive()
        {
            /*
            MATLAB:
            sum_cc = ca3x2 + cb3x2
            diff_cc = ca3x2 - cb3x2
            sum_cm = ca3x2 + mb3x2
            diff_cm = ca3x2 - mb3x2
            sum_cs = ca3x2 + s
            diff_cs = ca3x2 - s
            neg_c = -ca3x2
            */

            // ComplexMatrix + ComplexMatrix
            ComplexMatrix sum_cc = new ComplexMatrix(new Complex[][] {
                new Complex[] { 15+(72*j), 1.5+(16.5*j) },
                new Complex[] { -2-(37*j), 9.5-(43.5*j) },
                new Complex[] { 32+(137*j), 11-(96*j) }
                });

            Assert.That(ca3x2 + cb3x2, NumericIs.AlmostEqualTo(sum_cc), "sum cc 1");
            ComplexMatrix sum_cc_inplace = ca3x2.Clone();
            Assert.That(sum_cc_inplace.Add(cb3x2), NumericIs.AlmostEqualTo(sum_cc), "sum cc 2");
            sum_cc_inplace.AddInplace(cb3x2);
            Assert.That(sum_cc_inplace, NumericIs.AlmostEqualTo(sum_cc), "sum cc 3");

            // ComplexMatrix - ComplexMatrix
            ComplexMatrix diff_cc = new ComplexMatrix(new Complex[][] {
                new Complex[] { -3-(96*j), -1.5-(16.5*j) },
                new Complex[] { 6+(29*j), 14.5-(4.5*j) },
                new Complex[] { -4-(193*j), 25+(24*j) }
                });

            Assert.That(ca3x2 - cb3x2, NumericIs.AlmostEqualTo(diff_cc), "diff cc 1");
            ComplexMatrix diff_cc_inplace = ca3x2.Clone();
            Assert.That(diff_cc_inplace.Subtract(cb3x2), NumericIs.AlmostEqualTo(diff_cc), "diff cc 2");
            diff_cc_inplace.SubtractInplace(cb3x2);
            Assert.That(diff_cc_inplace, NumericIs.AlmostEqualTo(diff_cc), "diff cc 3");

            // ComplexMatrix + Matrix
            ComplexMatrix sum_cm = new ComplexMatrix(new Complex[][] {
                new Complex[] { 16-(12*j), 2.5 },
                new Complex[] { -1-(4*j), 10.5-(24*j) },
                new Complex[] { 33-(28*j), 12-(36*j) }
                });

            Assert.That(ca3x2 + mb3x2, NumericIs.AlmostEqualTo(sum_cm), "sum cm 1");
            ComplexMatrix sum_cm_inplace = ca3x2.Clone();
            Assert.That(sum_cm_inplace.Add(mb3x2), NumericIs.AlmostEqualTo(sum_cm), "sum cm 2");
            sum_cm_inplace.AddInplace(mb3x2);
            Assert.That(sum_cm_inplace, NumericIs.AlmostEqualTo(sum_cm), "sum cm 3");

            // ComplexMatrix - Matrix
            ComplexMatrix diff_cm = new ComplexMatrix(new Complex[][] {
                new Complex[] { -4-(12*j), -2.5 },
                new Complex[] { 5-(4*j), 13.5-(24*j) },
                new Complex[] { -5-(28*j), 24-(36*j) }
                });

            Assert.That(ca3x2 - mb3x2, NumericIs.AlmostEqualTo(diff_cm), "diff cm 1");
            ComplexMatrix diff_cm_inplace = ca3x2.Clone();
            Assert.That(diff_cm_inplace.Subtract(mb3x2), NumericIs.AlmostEqualTo(diff_cm), "diff cm 2");
            diff_cm_inplace.SubtractInplace(mb3x2);
            Assert.That(diff_cm_inplace, NumericIs.AlmostEqualTo(diff_cm), "diff cm 3");

            // ComplexMatrix + Complex
            ComplexMatrix sum_cs = new ComplexMatrix(new Complex[][] {
                new Complex[] { 7-(11*j), 1+j },
                new Complex[] { 3-(3*j), 13-(23*j) },
                new Complex[] { 15-(27*j), 19-(35*j) }
                });

            Assert.That(ca3x2 + s, NumericIs.AlmostEqualTo(sum_cs), "sum cs 1");
            ComplexMatrix sum_cs_inplace = ca3x2.Clone();
            Assert.That(sum_cs_inplace.Add(s), NumericIs.AlmostEqualTo(sum_cs), "sum cs 2");
            sum_cs_inplace.AddInplace(s);
            Assert.That(sum_cs_inplace, NumericIs.AlmostEqualTo(sum_cs), "sum cs 3");

            // ComplexMatrix - Complex
            ComplexMatrix diff_cs = new ComplexMatrix(new Complex[][] {
                new Complex[] { 5-(13*j), -1-j },
                new Complex[] { 1-(5*j), 11-(25*j) },
                new Complex[] { 13-(29*j), 17-(37*j) }
                });

            Assert.That(ca3x2 - s, NumericIs.AlmostEqualTo(diff_cs), "diff cs 1");
            ComplexMatrix diff_cs_inplace = ca3x2.Clone();
            Assert.That(diff_cs_inplace.Subtract(s), NumericIs.AlmostEqualTo(diff_cs), "diff cs 2");
            diff_cs_inplace.SubtractInplace(s);
            Assert.That(diff_cs_inplace, NumericIs.AlmostEqualTo(diff_cs), "diff cs 3");

            // ComplexMatrix Negate
            ComplexMatrix neg_c = new ComplexMatrix(new Complex[][] {
                new Complex[] { -6+(12*j), 0 },
                new Complex[] { -2+(4*j), -12+(24*j) },
                new Complex[] { -14+(28*j), -18+(36*j) }
                });

            Assert.That(-ca3x2, NumericIs.AlmostEqualTo(neg_c), "neg c 1");
            ComplexMatrix neg_c_inplace = ca3x2.Clone();
            Assert.That(neg_c_inplace.Negate(), NumericIs.AlmostEqualTo(neg_c), "neg c 2");
            neg_c_inplace.NegateInplace();
            Assert.That(neg_c_inplace, NumericIs.AlmostEqualTo(neg_c), "neg c 3");
        }

        [Test]
        public void TestComplexMatrix_Multiplicative()
        {
            /*
            MATLAB:
            prod_cc = ca3x2 * cd2x4
            prod_cm = ca3x2 * md2x4
            prod_cs = ca3x2 * s
            prod_cc2 = cc2x2 * cc2x2
            prod_cm2 = cc2x2 * mc2x2
            prod_cs2 = cc2x2 * s
            prod_ccv = ca3x2 * cv2.'
            prod_cv = ca3x2 * v2.'
            prod_ccvdl = diag(cv2) * cc2x2
            prod_ccvdr = cc2x2 * diag(cv2)
            prod_cvdl = diag(v2) * cc2x2
            prod_cvdr = cc2x2 * diag(v2)
            */

            // ComplexMatrix * ComplexMatrix
            ComplexMatrix prod_cc = new ComplexMatrix(new Complex[][] {
                new Complex[] { -66+(12*j), -66+(72*j), -66-(228*j), -66+(672*j) },
                new Complex[] { -154+(268*j), -154+(300*j), -154+(308*j), -154+(368*j) },
                new Complex[] { -352+(424*j), -352+(582*j), -352+(44*j), -352+(1784*j) }
                });

            Assert.That(ca3x2 * cd2x4, NumericIs.AlmostEqualTo(prod_cc), "prod cc 1");
            Assert.That(ca3x2.Multiply(cd2x4), NumericIs.AlmostEqualTo(prod_cc), "prod cc 2");

            // ComplexMatrix * Matrix
            ComplexMatrix prod_cm = new ComplexMatrix(new Complex[][] {
                new Complex[] { 6-(12*j), 12-(24*j), -18+(36*j), 72-(144*j) },
                new Complex[] { 38-(76*j), 41.2-(82.4*j), 42-(84*j), 48-(96*j) },
                new Complex[] { 68-(136*j), 83.8-(167.6*j), 30-(60*j), 204-(408*j) }
                });

            Assert.That(ca3x2 * md2x4, NumericIs.AlmostEqualTo(prod_cm), "prod cm 1");
            Assert.That(ca3x2.Multiply(md2x4), NumericIs.AlmostEqualTo(prod_cm), "prod cm 2");

            // ComplexMatrix * Complex
            ComplexMatrix prod_cs = new ComplexMatrix(new Complex[][] {
                new Complex[] { 18-(6*j), 0 },
                new Complex[] { 6-(2*j), 36-(12*j) },
                new Complex[] { 42-(14*j), 54-(18*j) }
                });

            Assert.That(ca3x2 * s, NumericIs.AlmostEqualTo(prod_cs), "prod cs 1");
            Assert.That(ca3x2.Multiply(s), NumericIs.AlmostEqualTo(prod_cs), "prod cs 2");

            // ComplexMatrix * ComplexMatrix (Square)
            ComplexMatrix prod_cc2 = new ComplexMatrix(new Complex[][] {
                new Complex[] { -1232-(924*j), -1368-(1026*j) },
                new Complex[] { -1520-(1140*j), -1688-(1266*j) }
                });

            Assert.That(cc2x2 * cc2x2, NumericIs.AlmostEqualTo(prod_cc2), "prod cc2 1");
            Assert.That(cc2x2.Multiply(cc2x2), NumericIs.AlmostEqualTo(prod_cc2), "prod cc2 2");
            ComplexMatrix prod_cc2_inplace = cc2x2.Clone();
            prod_cc2_inplace.MultiplyInplace(cc2x2);
            Assert.That(prod_cc2_inplace, NumericIs.AlmostEqualTo(prod_cc2), "prod cc2 3");

            // ComplexMatrix * Matrix (Square)
            ComplexMatrix prod_cm2 = new ComplexMatrix(new Complex[][] {
                new Complex[] { 35-(105*j), 52-(156*j) },
                new Complex[] { 43-(129*j), 64-(192*j) }
                });

            Assert.That(cc2x2 * mc2x2, NumericIs.AlmostEqualTo(prod_cm2), "prod cm2 1");
            Assert.That(cc2x2.Multiply(mc2x2), NumericIs.AlmostEqualTo(prod_cm2), "prod cm2 2");
            ComplexMatrix prod_cm2_inplace = cc2x2.Clone();
            prod_cm2_inplace.MultiplyInplace(mc2x2);
            Assert.That(prod_cm2_inplace, NumericIs.AlmostEqualTo(prod_cm2), "prod cm2 3");

            // ComplexMatrix * Complex (Square)
            ComplexMatrix prod_cs2 = new ComplexMatrix(new Complex[][] {
                new Complex[] { 32-(16*j), 36-(18*j) },
                new Complex[] { 40-(20*j), 44-(22*j) }
                });

            Assert.That(cc2x2 * s, NumericIs.AlmostEqualTo(prod_cs2), "prod cs2 1");
            Assert.That(cc2x2.Multiply(s), NumericIs.AlmostEqualTo(prod_cs2), "prod cs2 2");
            ComplexMatrix prod_cs2_inplace = cc2x2.Clone();
            prod_cs2_inplace.MultiplyInplace(s);
            Assert.That(prod_cs2_inplace, NumericIs.AlmostEqualTo(prod_cs2), "prod cs2 3");

            // ComplexMatrix * ComplexVector (Column)
            ComplexVector prod_ccv = new ComplexVector(new Complex[] { 42 - (54 * j), 62 + (66 * j), 170 });
            Assert.That(ca3x2 * cv2, NumericIs.AlmostEqualTo(prod_ccv), "prod ccv 1");
            Assert.That(ca3x2.MultiplyRightColumn(cv2), NumericIs.AlmostEqualTo(prod_ccv), "prod ccv 2");

            // ComplexMatrix * Vector (Column)
            ComplexVector prod_cv = new ComplexVector(new Complex[] { 30 - (60 * j), -14 + (28 * j), 34 - (68 * j) });
            Assert.That(ca3x2 * v2, NumericIs.AlmostEqualTo(prod_cv), "prod cv 1");
            Assert.That(ca3x2.MultiplyRightColumn(v2), NumericIs.AlmostEqualTo(prod_cv), "prod cv 2");

            // ComplexMatrix * ComplexVector (Diagonal, Left)
            ComplexMatrix prod_ccvdl = new ComplexMatrix(new Complex[][] {
                new Complex[] { 64-(112*j), 72-(126*j) },
                new Complex[] { 70+(90*j), 77+(99*j) }
                });

            Assert.That(cc2x2.MultiplyLeftDiagonal(cv2), NumericIs.AlmostEqualTo(prod_ccvdl), "prod ccv dl 1");
            ComplexMatrix prod_ccvdl_inplace = cc2x2.Clone();
            prod_ccvdl_inplace.MultiplyLeftDiagonalInplace(cv2);
            Assert.That(prod_ccvdl_inplace, NumericIs.AlmostEqualTo(prod_ccvdl), "prod ccv dl 2");
            Assert.That(ComplexMatrix.Diagonal(cv2) * cc2x2, NumericIs.AlmostEqualTo(prod_ccvdl), "prod ccv dl 3");

            // ComplexMatrix * Vector (Diagonal, Left)
            ComplexMatrix prod_cvdl = new ComplexMatrix(new Complex[][] {
                new Complex[] { 40-(120*j), 45-(135*j) },
                new Complex[] { -20+(60*j), -22+(66*j) }
                });

            Assert.That(cc2x2.MultiplyLeftDiagonal(v2), NumericIs.AlmostEqualTo(prod_cvdl), "prod cv dl 1");
            ComplexMatrix prod_cvdl_inplace = cc2x2.Clone();
            prod_cvdl_inplace.MultiplyLeftDiagonalInplace(v2);
            Assert.That(prod_cvdl_inplace, NumericIs.AlmostEqualTo(prod_cvdl), "prod cv dl 2");

            // ComplexMatrix * ComplexVector (Diagonal, Right)
            ComplexMatrix prod_ccvdr = new ComplexMatrix(new Complex[][] {
                new Complex[] { 64-(112*j), 63+(81*j) },
                new Complex[] { 80-(140*j), 77+(99*j) }
                });

            Assert.That(cc2x2.MultiplyRightDiagonal(cv2), NumericIs.AlmostEqualTo(prod_ccvdr), "prod ccv dr 1");
            ComplexMatrix prod_ccvdr_inplace = cc2x2.Clone();
            prod_ccvdr_inplace.MultiplyRightDiagonalInplace(cv2);
            Assert.That(prod_ccvdr_inplace, NumericIs.AlmostEqualTo(prod_ccvdr), "prod ccv dr 2");
            Assert.That(cc2x2 * ComplexMatrix.Diagonal(cv2), NumericIs.AlmostEqualTo(prod_ccvdr), "prod ccv dr 3");

            // ComplexMatrix * Vector (Diagonal, Right)
            ComplexMatrix prod_cvdr = new ComplexMatrix(new Complex[][] {
                new Complex[] { 40-(120*j), -18+(54*j) },
                new Complex[] { 50-(150*j), -22+(66*j) }
                });

            Assert.That(cc2x2.MultiplyRightDiagonal(v2), NumericIs.AlmostEqualTo(prod_cvdr), "prod cv dr 1");
            ComplexMatrix prod_cvdr_inplace = cc2x2.Clone();
            prod_cvdr_inplace.MultiplyRightDiagonalInplace(v2);
            Assert.That(prod_cvdr_inplace, NumericIs.AlmostEqualTo(prod_cvdr), "prod cv dr 2");
        }
    }
}
