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
        readonly Complex _s = new Complex(1, 1);
        Matrix _ma3X2, _mb3X2, _mc2X2, _md2X4;
        ComplexMatrix _ra3X2, _rb3X2, _rc2X2, _rd2X4;
        ComplexMatrix _ia3X2, _ib3X2, _ic2X2, _id2X4;
        ComplexMatrix _ca3X2, _cb3X2, _cc2X2, _cd2X4;
        Vector _v2;
        ComplexVector _cv2;

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

            // MATLAB: mc2x2 = [1 2;3 4]
            _mc2X2 = new Matrix(new double[][] {
                new double[] { 1, 2 },
                new double[] { 3, 4 }
                });

            // MATLAB: md2x4 = [1 2 -3 12;3 3.1 4 2]
            _md2X4 = new Matrix(new double[][] {
                new double[] { 1, 2, -3, 12 },
                new double[] { 3, 3.1, 4, 2 }
                });

            // MATLAB: ra3x2 = ma3x2 + 2
            _ra3X2 = ComplexMatrix.Create(_ma3X2) + 2;

            // MATLAB: rb3x2 = mb3x2 - 1
            _rb3X2 = ComplexMatrix.Create(_mb3X2) - 1;

            // MATLAB: rc2x2 = mc2x2 + 5
            _rc2X2 = ComplexMatrix.Create(_mc2X2) + 5;

            // MATLAB: rd2x4 = md2x4 * 2
            _rd2X4 = ComplexMatrix.Create(_md2X4) * 2;

            // MATLAB:  ia3x2 = (ra3x2 * 2) * j
            _ia3X2 = (_ra3X2 * 2) * j;

            // MATLAB: ib3x2 = (rb3x2 * 3 + 1) * j
            _ib3X2 = ((_rb3X2 * 3) + 1) * j;

            // MATLAB: ic2x2 = (rc2x2 + 2) * j
            _ic2X2 = (_rc2X2 + 2) * j;

            // MATLAB: id2x4 = (rd2x4 - 5) * j
            _id2X4 = (_rd2X4 - 5) * j;

            // MATLAB: ca3x2 = 2*ra3x2 - 2*ia3x2
            _ca3X2 = (2 * _ra3X2) - (2 * _ia3X2);

            // MATLAB: cb3x2 = rb3x2 + 3*ib3x2
            _cb3X2 = _rb3X2 + (3 * _ib3X2);

            // MATLAB: cc2x2 = rc2x2 + 2 - 3*ic2x2
            _cc2X2 = _rc2X2 + 2 - (3 * _ic2X2);

            // MATLAB: cd2x4 = -2*rd2x4 + id2x4 + 1-j
            _cd2X4 = (-2 * _rd2X4) + _id2X4 + (1 - j);

            // MATLAB: v2 = [5 -2]
            _v2 = new Vector(new double[] { 5, -2 });

            // MATLAB: cv2 = [5+j, -2+3j]
            _cv2 = new ComplexVector(new Complex[] { 5 + j, -2 + (3 * j) });
        }

        [Test]
        public void ComplexMatrixTranspose()
        {
            /* 2x3 rectangular case */

            // MATLAB: trans_c = ca3x2.'
            ComplexMatrix u = new ComplexMatrix(new Complex[][] {
                new Complex[] { 6-(12*j), 2-(4*j), 14-(28*j) },
                new Complex[] { 0, 12-(24*j), 18-(36*j) }
                });

            Assert.That(_ca3X2.Transpose(), NumericIs.AlmostEqualTo(u), "trans c 1");
            Assert.That(_ca3X2.Transpose(), Is.Not.SameAs(_ca3X2));
            Assert.That(_ca3X2.Transpose().GetArray(), Is.Not.SameAs(_ca3X2.GetArray()));

            /* 2x2 square case */

            // MATLAB: trans_c2 = cc2x2.'
            ComplexMatrix v = new ComplexMatrix(new Complex[][] {
                new Complex[] { 8-(24*j), 10-(30*j) },
                new Complex[] { 9-(27*j), 11-(33*j) }
                });

            Assert.That(_cc2X2.Transpose(), NumericIs.AlmostEqualTo(v), "trans c2 1");
            Assert.That(_cc2X2.Transpose(), Is.Not.SameAs(_cc2X2));
            Assert.That(_cc2X2.Transpose().GetArray(), Is.Not.SameAs(_cc2X2.GetArray()));

            ComplexMatrix vInplace = _cc2X2.Clone();
            Assert.That(vInplace, Is.Not.SameAs(_cc2X2));
            Assert.That(vInplace.GetArray(), Is.Not.SameAs(_cc2X2.GetArray()));

            Complex[][] internalArray = vInplace.GetArray();
            vInplace.TransposeInplace();
            Assert.That(vInplace, NumericIs.AlmostEqualTo(v), "trans c2 2");
            Assert.That(vInplace.GetArray(), Is.SameAs(internalArray));
        }

        [Test]
        public void ComplexMatrixHermitianTranspose()
        {
            /* 2x3 rectangular case */

            // MATLAB: htrans_c = ca3x2'
            ComplexMatrix u = new ComplexMatrix(new Complex[][] {
                new Complex[] { 6+(12*j), 2+(4*j), 14+(28*j) },
                new Complex[] { 0, 12+(24*j), 18+(36*j) }
                });

            Assert.That(_ca3X2.HermitianTranspose(), NumericIs.AlmostEqualTo(u), "htrans c 1");
            Assert.That(_ca3X2.HermitianTranspose(), Is.Not.SameAs(_ca3X2));
            Assert.That(_ca3X2.HermitianTranspose().GetArray(), Is.Not.SameAs(_ca3X2.GetArray()));

            /* 2x2 square case */

            // MATLAB: htrans_c2 = cc2x2'
            ComplexMatrix v = new ComplexMatrix(new Complex[][] {
                new Complex[] { 8+(24*j), 10+(30*j) },
                new Complex[] { 9+(27*j), 11+(33*j) }
                });

            Assert.That(_cc2X2.HermitianTranspose(), NumericIs.AlmostEqualTo(v), "htrans c2 1");
            Assert.That(_cc2X2.HermitianTranspose(), Is.Not.SameAs(_cc2X2));
            Assert.That(_cc2X2.HermitianTranspose().GetArray(), Is.Not.SameAs(_cc2X2.GetArray()));

            ComplexMatrix vInplace = _cc2X2.Clone();
            Assert.That(vInplace, Is.Not.SameAs(_cc2X2));
            Assert.That(vInplace.GetArray(), Is.Not.SameAs(_cc2X2.GetArray()));

            Complex[][] internalArray = vInplace.GetArray();
            vInplace.HermitianTransposeInplace();
            Assert.That(vInplace, NumericIs.AlmostEqualTo(v), "htrans c2 2");
            Assert.That(vInplace.GetArray(), Is.SameAs(internalArray));
        }

        [Test]
        public void ComplexMatrixConjugate()
        {
            // MATLAB: conj_c = conj(ca3x2)
            ComplexMatrix u = new ComplexMatrix(new Complex[][] {
                new Complex[] { 6+(12*j), 0 },
                new Complex[] { 2+(4*j), 12+(24*j) },
                new Complex[] { 14+(28*j), 18+(36*j) }
                });

            Assert.That(_ca3X2.Conjugate(), NumericIs.AlmostEqualTo(u), "conj c 1");
            Assert.That(_ca3X2.Conjugate(), Is.Not.SameAs(_ca3X2));
            Assert.That(_ca3X2.Conjugate().GetArray(), Is.Not.SameAs(_ca3X2.GetArray()));

            ComplexMatrix uInplace = _ca3X2.Clone();
            Complex[][] internalArray = uInplace.GetArray();
            uInplace.ConjugateInplace();
            Assert.That(uInplace, NumericIs.AlmostEqualTo(u), "conj c 2");
            Assert.That(internalArray, Is.Not.SameAs(_ca3X2.GetArray()));
            Assert.That(internalArray, Is.SameAs(uInplace.GetArray()));
        }

        [Test]
        public void ComplexMatrixAddition()
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
            ComplexMatrix sumCmCm = new ComplexMatrix(new Complex[][] {
                new Complex[] { 15+(72*j), 1.5+(16.5*j) },
                new Complex[] { -2-(37*j), 9.5-(43.5*j) },
                new Complex[] { 32+(137*j), 11-(96*j) }
                });

            Assert.That(_ca3X2 + _cb3X2, NumericIs.AlmostEqualTo(sumCmCm), "sum cc 1");
            ComplexMatrix sumCmCmInplace = _ca3X2.Clone();
            Assert.That(sumCmCmInplace.Add(_cb3X2), NumericIs.AlmostEqualTo(sumCmCm), "sum cc 2");
            sumCmCmInplace.AddInplace(_cb3X2);
            Assert.That(sumCmCmInplace, NumericIs.AlmostEqualTo(sumCmCm), "sum cc 3");

            // ComplexMatrix - ComplexMatrix
            ComplexMatrix diffCmCm = new ComplexMatrix(new Complex[][] {
                new Complex[] { -3-(96*j), -1.5-(16.5*j) },
                new Complex[] { 6+(29*j), 14.5-(4.5*j) },
                new Complex[] { -4-(193*j), 25+(24*j) }
                });

            Assert.That(_ca3X2 - _cb3X2, NumericIs.AlmostEqualTo(diffCmCm), "diff cc 1");
            ComplexMatrix diffCmCmInplace = _ca3X2.Clone();
            Assert.That(diffCmCmInplace.Subtract(_cb3X2), NumericIs.AlmostEqualTo(diffCmCm), "diff cc 2");
            diffCmCmInplace.SubtractInplace(_cb3X2);
            Assert.That(diffCmCmInplace, NumericIs.AlmostEqualTo(diffCmCm), "diff cc 3");

            // ComplexMatrix + Matrix
            ComplexMatrix sumCmM = new ComplexMatrix(new Complex[][] {
                new Complex[] { 16-(12*j), 2.5 },
                new Complex[] { -1-(4*j), 10.5-(24*j) },
                new Complex[] { 33-(28*j), 12-(36*j) }
                });

            Assert.That(_ca3X2 + _mb3X2, NumericIs.AlmostEqualTo(sumCmM), "sum cm 1");
            ComplexMatrix sumCmMInplace = _ca3X2.Clone();
            Assert.That(sumCmMInplace.Add(_mb3X2), NumericIs.AlmostEqualTo(sumCmM), "sum cm 2");
            sumCmMInplace.AddInplace(_mb3X2);
            Assert.That(sumCmMInplace, NumericIs.AlmostEqualTo(sumCmM), "sum cm 3");

            // ComplexMatrix - Matrix
            ComplexMatrix diffCmM = new ComplexMatrix(new Complex[][] {
                new Complex[] { -4-(12*j), -2.5 },
                new Complex[] { 5-(4*j), 13.5-(24*j) },
                new Complex[] { -5-(28*j), 24-(36*j) }
                });

            Assert.That(_ca3X2 - _mb3X2, NumericIs.AlmostEqualTo(diffCmM), "diff cm 1");
            ComplexMatrix diffCmMInplace = _ca3X2.Clone();
            Assert.That(diffCmMInplace.Subtract(_mb3X2), NumericIs.AlmostEqualTo(diffCmM), "diff cm 2");
            diffCmMInplace.SubtractInplace(_mb3X2);
            Assert.That(diffCmMInplace, NumericIs.AlmostEqualTo(diffCmM), "diff cm 3");

            // ComplexMatrix + Complex
            ComplexMatrix sumCmC = new ComplexMatrix(new Complex[][] {
                new Complex[] { 7-(11*j), 1+j },
                new Complex[] { 3-(3*j), 13-(23*j) },
                new Complex[] { 15-(27*j), 19-(35*j) }
                });

            Assert.That(_ca3X2 + _s, NumericIs.AlmostEqualTo(sumCmC), "sum cs 1");
            ComplexMatrix sumCmCInplace = _ca3X2.Clone();
            Assert.That(sumCmCInplace.Add(_s), NumericIs.AlmostEqualTo(sumCmC), "sum cs 2");
            sumCmCInplace.AddInplace(_s);
            Assert.That(sumCmCInplace, NumericIs.AlmostEqualTo(sumCmC), "sum cs 3");

            // ComplexMatrix - Complex
            ComplexMatrix diffCmC = new ComplexMatrix(new Complex[][] {
                new Complex[] { 5-(13*j), -1-j },
                new Complex[] { 1-(5*j), 11-(25*j) },
                new Complex[] { 13-(29*j), 17-(37*j) }
                });

            Assert.That(_ca3X2 - _s, NumericIs.AlmostEqualTo(diffCmC), "diff cs 1");
            ComplexMatrix diffCmCInplace = _ca3X2.Clone();
            Assert.That(diffCmCInplace.Subtract(_s), NumericIs.AlmostEqualTo(diffCmC), "diff cs 2");
            diffCmCInplace.SubtractInplace(_s);
            Assert.That(diffCmCInplace, NumericIs.AlmostEqualTo(diffCmC), "diff cs 3");

            // ComplexMatrix Negate
            ComplexMatrix negateCm = new ComplexMatrix(new Complex[][] {
                new Complex[] { -6+(12*j), 0 },
                new Complex[] { -2+(4*j), -12+(24*j) },
                new Complex[] { -14+(28*j), -18+(36*j) }
                });

            Assert.That(-_ca3X2, NumericIs.AlmostEqualTo(negateCm), "neg c 1");
            ComplexMatrix negCmInplace = _ca3X2.Clone();
            Assert.That(negCmInplace.Negate(), NumericIs.AlmostEqualTo(negateCm), "neg c 2");
            negCmInplace.NegateInplace();
            Assert.That(negCmInplace, NumericIs.AlmostEqualTo(negateCm), "neg c 3");
        }

        [Test]
        public void ComplexMatrixMultiplication()
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
            ComplexMatrix prodCmCm = new ComplexMatrix(new Complex[][] {
                new Complex[] { -66+(12*j), -66+(72*j), -66-(228*j), -66+(672*j) },
                new Complex[] { -154+(268*j), -154+(300*j), -154+(308*j), -154+(368*j) },
                new Complex[] { -352+(424*j), -352+(582*j), -352+(44*j), -352+(1784*j) }
                });

            Assert.That(_ca3X2 * _cd2X4, NumericIs.AlmostEqualTo(prodCmCm), "prod cc 1");
            Assert.That(_ca3X2.Multiply(_cd2X4), NumericIs.AlmostEqualTo(prodCmCm), "prod cc 2");

            // ComplexMatrix * Matrix
            ComplexMatrix prodCmM = new ComplexMatrix(new Complex[][] {
                new Complex[] { 6-(12*j), 12-(24*j), -18+(36*j), 72-(144*j) },
                new Complex[] { 38-(76*j), 41.2-(82.4*j), 42-(84*j), 48-(96*j) },
                new Complex[] { 68-(136*j), 83.8-(167.6*j), 30-(60*j), 204-(408*j) }
                });

            Assert.That(_ca3X2 * _md2X4, NumericIs.AlmostEqualTo(prodCmM), "prod cm 1");
            Assert.That(_ca3X2.Multiply(_md2X4), NumericIs.AlmostEqualTo(prodCmM), "prod cm 2");

            // ComplexMatrix * Complex
            ComplexMatrix prodCmC = new ComplexMatrix(new Complex[][] {
                new Complex[] { 18-(6*j), 0 },
                new Complex[] { 6-(2*j), 36-(12*j) },
                new Complex[] { 42-(14*j), 54-(18*j) }
                });

            Assert.That(_ca3X2 * _s, NumericIs.AlmostEqualTo(prodCmC), "prod cs 1");
            Assert.That(_ca3X2.Multiply(_s), NumericIs.AlmostEqualTo(prodCmC), "prod cs 2");

            // ComplexMatrix * ComplexMatrix (Square)
            ComplexMatrix prodCmCmSquare = new ComplexMatrix(new Complex[][] {
                new Complex[] { -1232-(924*j), -1368-(1026*j) },
                new Complex[] { -1520-(1140*j), -1688-(1266*j) }
                });

            Assert.That(_cc2X2 * _cc2X2, NumericIs.AlmostEqualTo(prodCmCmSquare), "prod cc2 1");
            Assert.That(_cc2X2.Multiply(_cc2X2), NumericIs.AlmostEqualTo(prodCmCmSquare), "prod cc2 2");
            ComplexMatrix prodCmCmSquareInplace = _cc2X2.Clone();
            prodCmCmSquareInplace.MultiplyInplace(_cc2X2);
            Assert.That(prodCmCmSquareInplace, NumericIs.AlmostEqualTo(prodCmCmSquare), "prod cc2 3");

            // ComplexMatrix * Matrix (Square)
            ComplexMatrix prodCmMSquare = new ComplexMatrix(new Complex[][] {
                new Complex[] { 35-(105*j), 52-(156*j) },
                new Complex[] { 43-(129*j), 64-(192*j) }
                });

            Assert.That(_cc2X2 * _mc2X2, NumericIs.AlmostEqualTo(prodCmMSquare), "prod cm2 1");
            Assert.That(_cc2X2.Multiply(_mc2X2), NumericIs.AlmostEqualTo(prodCmMSquare), "prod cm2 2");
            ComplexMatrix prodCmMSquareInplace = _cc2X2.Clone();
            prodCmMSquareInplace.MultiplyInplace(_mc2X2);
            Assert.That(prodCmMSquareInplace, NumericIs.AlmostEqualTo(prodCmMSquare), "prod cm2 3");

            // ComplexMatrix * Complex (Square)
            ComplexMatrix prodCmCSquare = new ComplexMatrix(new Complex[][] {
                new Complex[] { 32-(16*j), 36-(18*j) },
                new Complex[] { 40-(20*j), 44-(22*j) }
                });

            Assert.That(_cc2X2 * _s, NumericIs.AlmostEqualTo(prodCmCSquare), "prod cs2 1");
            Assert.That(_cc2X2.Multiply(_s), NumericIs.AlmostEqualTo(prodCmCSquare), "prod cs2 2");
            ComplexMatrix prodCmCSquareInplace = _cc2X2.Clone();
            prodCmCSquareInplace.MultiplyInplace(_s);
            Assert.That(prodCmCSquareInplace, NumericIs.AlmostEqualTo(prodCmCSquare), "prod cs2 3");

            // ComplexMatrix * ComplexVector (Column)
            ComplexVector prodCmCvc = new ComplexVector(new Complex[] { 42 - (54 * j), 62 + (66 * j), 170 });
            Assert.That(_ca3X2 * _cv2, NumericIs.AlmostEqualTo(prodCmCvc), "prod ccv 1");
            Assert.That(_ca3X2.MultiplyRightColumn(_cv2), NumericIs.AlmostEqualTo(prodCmCvc), "prod ccv 2");

            // ComplexMatrix * Vector (Column)
            ComplexVector prodCmVc = new ComplexVector(new Complex[] { 30 - (60 * j), -14 + (28 * j), 34 - (68 * j) });
            Assert.That(_ca3X2 * _v2, NumericIs.AlmostEqualTo(prodCmVc), "prod cv 1");
            Assert.That(_ca3X2.MultiplyRightColumn(_v2), NumericIs.AlmostEqualTo(prodCmVc), "prod cv 2");

            // ComplexMatrix * ComplexVector (Diagonal, Left)
            ComplexMatrix prodCmCvdl = new ComplexMatrix(new Complex[][] {
                new Complex[] { 64-(112*j), 72-(126*j) },
                new Complex[] { 70+(90*j), 77+(99*j) }
                });

            Assert.That(_cc2X2.MultiplyLeftDiagonal(_cv2), NumericIs.AlmostEqualTo(prodCmCvdl), "prod ccv dl 1");
            ComplexMatrix prodCmCvdlInplace = _cc2X2.Clone();
            prodCmCvdlInplace.MultiplyLeftDiagonalInplace(_cv2);
            Assert.That(prodCmCvdlInplace, NumericIs.AlmostEqualTo(prodCmCvdl), "prod ccv dl 2");
            Assert.That(ComplexMatrix.Diagonal(_cv2) * _cc2X2, NumericIs.AlmostEqualTo(prodCmCvdl), "prod ccv dl 3");

            // ComplexMatrix * Vector (Diagonal, Left)
            ComplexMatrix prodCmVdl = new ComplexMatrix(new Complex[][] {
                new Complex[] { 40-(120*j), 45-(135*j) },
                new Complex[] { -20+(60*j), -22+(66*j) }
                });

            Assert.That(_cc2X2.MultiplyLeftDiagonal(_v2), NumericIs.AlmostEqualTo(prodCmVdl), "prod cv dl 1");
            ComplexMatrix prodCmVdlInplace = _cc2X2.Clone();
            prodCmVdlInplace.MultiplyLeftDiagonalInplace(_v2);
            Assert.That(prodCmVdlInplace, NumericIs.AlmostEqualTo(prodCmVdl), "prod cv dl 2");

            // ComplexMatrix * ComplexVector (Diagonal, Right)
            ComplexMatrix prodCmCvdr = new ComplexMatrix(new Complex[][] {
                new Complex[] { 64-(112*j), 63+(81*j) },
                new Complex[] { 80-(140*j), 77+(99*j) }
                });

            Assert.That(_cc2X2.MultiplyRightDiagonal(_cv2), NumericIs.AlmostEqualTo(prodCmCvdr), "prod ccv dr 1");
            ComplexMatrix prodCmCvdrInplace = _cc2X2.Clone();
            prodCmCvdrInplace.MultiplyRightDiagonalInplace(_cv2);
            Assert.That(prodCmCvdrInplace, NumericIs.AlmostEqualTo(prodCmCvdr), "prod ccv dr 2");
            Assert.That(_cc2X2 * ComplexMatrix.Diagonal(_cv2), NumericIs.AlmostEqualTo(prodCmCvdr), "prod ccv dr 3");

            // ComplexMatrix * Vector (Diagonal, Right)
            ComplexMatrix prodCmVdr = new ComplexMatrix(new Complex[][] {
                new Complex[] { 40-(120*j), -18+(54*j) },
                new Complex[] { 50-(150*j), -22+(66*j) }
                });

            Assert.That(_cc2X2.MultiplyRightDiagonal(_v2), NumericIs.AlmostEqualTo(prodCmVdr), "prod cv dr 1");
            ComplexMatrix prodCmVdrInplace = _cc2X2.Clone();
            prodCmVdrInplace.MultiplyRightDiagonalInplace(_v2);
            Assert.That(prodCmVdrInplace, NumericIs.AlmostEqualTo(prodCmVdr), "prod cv dr 2");
        }
    }
}
