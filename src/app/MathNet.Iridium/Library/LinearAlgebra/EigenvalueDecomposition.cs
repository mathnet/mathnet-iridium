//-----------------------------------------------------------------------
// <copyright file="EigenvalueDecomposition.cs" company="Math.NET Project">
//    Copyright (c) 2004-2009, Joannes Vermorel, Christoph Rüegg.
//    All Right Reserved.
// </copyright>
// <author>
//    Joannes Vermorel, http://www.vermorel.com
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
// <contribution>
//    The MathWorks
//    NIST
// </contribution>
//-----------------------------------------------------------------------

using System;

namespace MathNet.Numerics.LinearAlgebra
{
    /// <summary>
    /// Eigenvalues and eigenvectors of a real matrix.
    /// </summary>
    /// <remarks>
    /// If A is symmetric, then A = V*D*V' where the eigenvalue matrix D is
    /// diagonal and the eigenvector matrix V is orthogonal.
    /// I.e. A = V.Multiply(D.Multiply(V.Transpose())) and 
    /// V.Multiply(V.Transpose()) equals the identity matrix.
    /// If A is not symmetric, then the eigenvalue matrix D is block diagonal
    /// with the real eigenvalues in 1-by-1 blocks and any complex eigenvalues,
    /// lambda + i*mu, in 2-by-2 blocks, [lambda, mu; -mu, lambda].  The
    /// columns of V represent the eigenvectors in the sense that A*V = V*D,
    /// i.e. A.Multiply(V) equals V.Multiply(D).  The matrix V may be badly
    /// conditioned, or even singular, so the validity of the equation
    /// A = V*D*Inverse(V) depends upon V.cond().
    /// </remarks>
    [Serializable]
    public class EigenvalueDecomposition
    {
        /// <summary>Row and column dimension (square matrix).</summary>
        readonly int _n;

        /// <summary>Symmetry flag.</summary>
        readonly bool _isSymmetric;

        /// <summary>Arrays for internal storage of eigenvalues.</summary>
        readonly double[] _d;

        /// <summary>Arrays for internal storage of eigenvalues.</summary>
        readonly double[] _e;

        /// <summary>Array for internal storage of eigenvectors.</summary>
        readonly double[][] _v;

        /// <summary>Array for internal storage of non symmetric Hessenberg form.</summary>
        readonly double[][] _h;

        readonly ComplexVector _eigenValues;
        readonly Vector _eigenValuesReal;
        readonly Vector _eigenValuesImag;

        [NonSerialized]
        double _cdivr, _cdivi;

        OnDemandComputation<Matrix> _blockDiagonalOnDemand;
        OnDemandComputation<Matrix> _eigenVectorsOnDemand;
        ////OnDemandComputation<ComplexVector> _eigenValuesOnDemand;

        /// <summary>
        /// Initializes a new instance of the EigenvalueDecomposition class, which check
        /// the matrix for symmetry and then construct the eigenvalue decomposition.
        /// </summary>
        /// <remarks>Provides access to D and V</remarks>
        /// <param name="arg">Square matrix</param>
        public
        EigenvalueDecomposition(Matrix arg)
        {
            double[][] a = arg;
            _n = arg.ColumnCount;
            _v = Matrix.CreateMatrixData(_n, _n);
            _d = new double[_n];
            _e = new double[_n];

            _isSymmetric = true;
            for(int j = 0; (j < _n) & _isSymmetric; j++)
            {
                for(int i = 0; (i < _n) & _isSymmetric; i++)
                {
                    _isSymmetric &= (a[i][j] == a[j][i]);
                }
            }

            if(_isSymmetric)
            {
                for(int i = 0; i < _n; i++)
                {
                    for(int j = 0; j < _n; j++)
                    {
                        _v[i][j] = a[i][j];
                    }
                }

                SymmetricTridiagonalize();

                SymmetricDiagonalize();
            }
            else
            {
                _h = new double[_n][];
                for(int i = 0; i < _n; i++)
                {
                    double[] hi = new double[_n];
                    double[] ai = a[i];

                    for(int j = 0; j < _n; j++)
                    {
                        hi[j] = ai[j];
                    }

                    _h[i] = hi;
                }

                NonsymmetricReduceToHessenberg();

                NonsymmetricReduceHessenberToRealSchur();
            }

            _eigenValuesReal = new Vector(_d);
            _eigenValuesImag = new Vector(_e);
            _eigenValues = ComplexVector.Create(_d, _e);

            InitOnDemandComputations();
        }

        /// <summary>
        /// Initializes a new instance of the EigenvalueDecomposition class,
        /// by decomposing symmetrical, tridiagonal matrices.
        /// </summary>
        public
        EigenvalueDecomposition(
            double[] d,
            double[] e)
        {
            /* TODO: unit test missing for EigenvalueDecomposition constructor. */

            _n = d.Length;
            _v = Matrix.CreateMatrixData(_n, _n);

            _d = new double[_n];
            Array.Copy(d, 0, _d, 0, _n);

            _e = new double[_n];
            Array.Copy(e, 0, _e, 1, _n - 1);

            for(int i = 0; i < _n; i++)
            {
                _v[i][i] = 1;
            }

            SymmetricDiagonalize();

            _eigenValuesReal = new Vector(_d);
            _eigenValuesImag = new Vector(_e);
            _eigenValues = ComplexVector.Create(_d, _e);

            InitOnDemandComputations();
        }

        /// <summary>
        /// Gets the eigenvalues.
        /// </summary>
        /// <returns>diag(D)</returns>
        public ComplexVector EigenValues
        {
            get { return _eigenValues; }
        }

        /// <summary>
        /// Gets the real part of the eigenvalues.
        /// </summary>
        /// <returns>real(diag(D))</returns>
        public Vector RealEigenvalues
        {
            get { return _eigenValuesReal; }
        }

        /// <summary>
        /// Gets the imaginary part of the eigenvalues.
        /// </summary>
        /// <returns>imag(diag(D))</returns>
        public Vector ImagEigenvalues
        {
            get { return _eigenValuesImag; }
        }

        /// <summary>
        /// Gets the block diagonal eigenvalue matrix
        /// </summary>
        public Matrix BlockDiagonal
        {
            get { return _blockDiagonalOnDemand.Compute(); }
        }

        /// <summary>
        /// Returns the eigenvector matrix
        /// </summary>
        public Matrix EigenVectors
        {
            get { return _eigenVectorsOnDemand.Compute(); }
        }

        void
        InitOnDemandComputations()
        {
            _blockDiagonalOnDemand = new OnDemandComputation<Matrix>(ComputeBlockDiagonalMatrix);
            ////_eigenValuesOnDemand = new OnDemandComputation<ComplexVector>(ComputeEigenValues);
            _eigenVectorsOnDemand = new OnDemandComputation<Matrix>(ComputeEigentVectors);
        }

        ////ComplexVector
        ////ComputeEigenValues()
        ////{
        ////    Complex[] eigenvalues = new Complex[n];

        ////    for(int i = 0; i < eigenvalues.Length; i++)
        ////    {
        ////        eigenvalues[i] = new Complex(d[i], e[i]);
        ////    }

        ////    return new ComplexVector(eigenvalues);
        ////}

        Matrix
        ComputeBlockDiagonalMatrix()
        {
            double[][] d = Matrix.CreateMatrixData(_n, _n);
            for(int i = 0; i < _n; i++)
            {
                for(int j = 0; j < _n; j++)
                {
                    d[i][j] = 0.0;
                }

                d[i][i] = _d[i];

                if(_e[i] > 0)
                {
                    d[i][i + 1] = _e[i];
                }
                else if(_e[i] < 0)
                {
                    d[i][i - 1] = _e[i];
                }
            }

            return new Matrix(d);
        }

        Matrix
        ComputeEigentVectors()
        {
            return new Matrix(_v);
        }

        /// <summary>
        /// Symmetric Householder reduction to tridiagonal form.
        /// </summary>
        void
        SymmetricTridiagonalize()
        {
            /*
            This is derived from the Algol procedures tred2 by
            Bowdler, Martin, Reinsch, and Wilkinson, Handbook for
            Auto. Comp., Vol.ii-Linear Algebra, and the corresponding
            Fortran subroutine in EISPACK.
            */

            for(int j = 0; j < _n; j++)
            {
                _d[j] = _v[_n - 1][j];
            }

            /* Householder reduction to tridiagonal form */

            for(int i = _n - 1; i > 0; i--)
            {
                /* Scale to avoid under/overflow */

                double scale = 0.0;
                double h = 0.0;

                for(int k = 0; k < i; k++)
                {
                    scale = scale + Math.Abs(_d[k]);
                }

                if(scale == 0.0)
                {
                    _e[i] = _d[i - 1];
                    for(int j = 0; j < i; j++)
                    {
                        _d[j] = _v[i - 1][j];
                        _v[i][j] = 0.0;
                        _v[j][i] = 0.0;
                    }
                }
                else
                {
                    /* Generate Householder vector. */

                    for(int k = 0; k < i; k++)
                    {
                        _d[k] /= scale;
                        h += _d[k] * _d[k];
                    }

                    double f = _d[i - 1];
                    double g = Math.Sqrt(h);
                    if(f > 0)
                    {
                        g = -g;
                    }

                    _e[i] = scale * g;
                    h = h - (f * g);
                    _d[i - 1] = f - g;
                    for(int j = 0; j < i; j++)
                    {
                        _e[j] = 0.0;
                    }

                    /* Apply similarity transformation to remaining columns */

                    for(int j = 0; j < i; j++)
                    {
                        f = _d[j];
                        _v[j][i] = f;
                        g = _e[j] + (_v[j][j] * f);

                        for(int k = j + 1; k <= i - 1; k++)
                        {
                            g += _v[k][j] * _d[k];
                            _e[k] += _v[k][j] * f;
                        }

                        _e[j] = g;
                    }

                    f = 0.0;

                    for(int j = 0; j < i; j++)
                    {
                        _e[j] /= h;
                        f += _e[j] * _d[j];
                    }

                    double hh = f / (h + h);

                    for(int j = 0; j < i; j++)
                    {
                        _e[j] -= hh * _d[j];
                    }

                    for(int j = 0; j < i; j++)
                    {
                        f = _d[j];
                        g = _e[j];

                        for(int k = j; k <= i - 1; k++)
                        {
                            _v[k][j] -= ((f * _e[k]) + (g * _d[k]));
                        }

                        _d[j] = _v[i - 1][j];
                        _v[i][j] = 0.0;
                    }
                }

                _d[i] = h;
            }

            /* Accumulate transformations */

            for(int i = 0; i < _n - 1; i++)
            {
                _v[_n - 1][i] = _v[i][i];
                _v[i][i] = 1.0;
                double h = _d[i + 1];
                if(h != 0.0)
                {
                    for(int k = 0; k <= i; k++)
                    {
                        _d[k] = _v[k][i + 1] / h;
                    }

                    for(int j = 0; j <= i; j++)
                    {
                        double g = 0.0;
                        for(int k = 0; k <= i; k++)
                        {
                            g += _v[k][i + 1] * _v[k][j];
                        }

                        for(int k = 0; k <= i; k++)
                        {
                            _v[k][j] -= g * _d[k];
                        }
                    }
                }

                for(int k = 0; k <= i; k++)
                {
                    _v[k][i + 1] = 0.0;
                }
            }

            for(int j = 0; j < _n; j++)
            {
                _d[j] = _v[_n - 1][j];
                _v[_n - 1][j] = 0.0;
            }

            _v[_n - 1][_n - 1] = 1.0;
            _e[0] = 0.0;
        }

        /// <summary>
        /// Symmetric tridiagonal QL algorithm.
        /// </summary>
        void
        SymmetricDiagonalize()
        {
            /*
            This is derived from the Algol procedures tql2, by
            Bowdler, Martin, Reinsch, and Wilkinson, Handbook for
            Auto. Comp., Vol.ii-Linear Algebra, and the corresponding
            Fortran subroutine in EISPACK.
            */

            for(int i = 1; i < _n; i++)
            {
                _e[i - 1] = _e[i];
            }

            _e[_n - 1] = 0.0;

            double f = 0.0;
            double tst1 = 0.0;
            double eps = Number.PositiveRelativeAccuracy;
            for(int l = 0; l < _n; l++)
            {
                /* Find small subdiagonal element */

                tst1 = Math.Max(tst1, Math.Abs(_d[l]) + Math.Abs(_e[l]));
                int m = l;
                while(m < _n)
                {
                    if(Math.Abs(_e[m]) <= eps * tst1)
                    {
                        break;
                    }

                    m++;
                }

                /*
                If m == l, d[l] is an eigenvalue,
                otherwise, iterate.
                */

                if(m > l)
                {
                    int iter = 0;
                    do
                    {
                        iter = iter + 1; // (Could check iteration count here.)

                        /* Compute implicit shift */

                        double g = _d[l];
                        double p = (_d[l + 1] - g) / (2.0 * _e[l]);
                        double r = Fn.Hypot(p, 1.0);
                        if(p < 0)
                        {
                            r = -r;
                        }

                        _d[l] = _e[l] / (p + r);
                        _d[l + 1] = _e[l] * (p + r);

                        double dl1 = _d[l + 1];
                        double h = g - _d[l];
                        for(int i = l + 2; i < _n; i++)
                        {
                            _d[i] -= h;
                        }

                        f = f + h;

                        /* Implicit QL transformation */

                        p = _d[m];
                        double c = 1.0;
                        double c2 = c;
                        double c3 = c;
                        double el1 = _e[l + 1];
                        double s = 0.0;
                        double s2 = 0.0;
                        for(int i = m - 1; i >= l; i--)
                        {
                            c3 = c2;
                            c2 = c;
                            s2 = s;
                            g = c * _e[i];
                            h = c * p;
                            r = Fn.Hypot(p, _e[i]);
                            if(r == 0)
                            {
                                _e[i + 1] = 0;
                                _d[i + 1] = 0;
                            }
                            else
                            {
                                _e[i + 1] = s * r;
                                s = _e[i] / r;
                                c = p / r;
                                p = (c * _d[i]) - (s * g);
                                _d[i + 1] = h + (s * ((c * g) + (s * _d[i])));
                            }

                            /* Accumulate transformation */

                            for(int k = 0; k < _n; k++)
                            {
                                h = _v[k][i + 1];
                                _v[k][i + 1] = (s * _v[k][i]) + (c * h);
                                _v[k][i] = (c * _v[k][i]) - (s * h);
                            }
                        }

                        p = (-s) * s2 * c3 * el1 * _e[l] / dl1;
                        _e[l] = s * p;
                        _d[l] = c * p;

                        /* Check for convergence */
                    }
                    while(Math.Abs(_e[l]) > eps * tst1);
                }

                _d[l] = _d[l] + f;
                _e[l] = 0.0;
            }

            /* Sort eigenvalues and corresponding vectors */

            for(int i = 0; i < _n - 1; i++)
            {
                int k = i;
                double p = _d[i];
                for(int j = i + 1; j < _n; j++)
                {
                    if(_d[j] < p)
                    {
                        k = j;
                        p = _d[j];
                    }
                }

                if(k != i)
                {
                    _d[k] = _d[i];
                    _d[i] = p;
                    for(int j = 0; j < _n; j++)
                    {
                        p = _v[j][i];
                        _v[j][i] = _v[j][k];
                        _v[j][k] = p;
                    }
                }
            }
        }

        /// <summary>
        /// Non symmetric reduction to Hessenberg form.
        /// </summary>
        void
        NonsymmetricReduceToHessenberg()
        {
            /*
            This is derived from the Algol procedures orthes and ortran,
            by Martin and Wilkinson, Handbook for Auto. Comp.,
            Vol.ii-Linear Algebra, and the corresponding
            Fortran subroutines in EISPACK.
            */

            int low = 0;
            int high = _n - 1;

            double[] ort = new double[_n];

            for(int m = low + 1; m <= high - 1; m++)
            {
                /* Scale column */

                double scale = 0.0;
                for(int i = m; i <= high; i++)
                {
                    scale = scale + Math.Abs(_h[i][m - 1]);
                }

                if(scale != 0.0)
                {
                    /* Compute Householder transformation */

                    double h = 0.0;
                    for(int i = high; i >= m; i--)
                    {
                        ort[i] = _h[i][m - 1] / scale;
                        h += ort[i] * ort[i];
                    }

                    double g = Math.Sqrt(h);
                    if(ort[m] > 0)
                    {
                        g = -g;
                    }

                    h = h - (ort[m] * g);
                    ort[m] = ort[m] - g;

                    /*
                    Apply Householder similarity transformation
                    H = (I-u*u'/h)*H*(I-u*u')/h)
                    */

                    for(int j = m; j < _n; j++)
                    {
                        double f = 0.0;
                        for(int i = high; i >= m; i--)
                        {
                            f += ort[i] * _h[i][j];
                        }

                        f = f / h;
                        for(int i = m; i <= high; i++)
                        {
                            _h[i][j] -= f * ort[i];
                        }
                    }

                    for(int i = 0; i <= high; i++)
                    {
                        double f = 0.0;
                        for(int j = high; j >= m; j--)
                        {
                            f += ort[j] * _h[i][j];
                        }

                        f = f / h;
                        for(int j = m; j <= high; j++)
                        {
                            _h[i][j] -= f * ort[j];
                        }
                    }

                    ort[m] = scale * ort[m];
                    _h[m][m - 1] = scale * g;
                }
            }

            /* Accumulate transformations (Algol's ortran) */

            for(int i = 0; i < _n; i++)
            {
                for(int j = 0; j < _n; j++)
                {
                    _v[i][j] = (i == j ? 1.0 : 0.0);
                }
            }

            for(int m = high - 1; m >= low + 1; m--)
            {
                if(_h[m][m - 1] != 0.0)
                {
                    for(int i = m + 1; i <= high; i++)
                    {
                        ort[i] = _h[i][m - 1];
                    }

                    for(int j = m; j <= high; j++)
                    {
                        double g = 0.0;
                        for(int i = m; i <= high; i++)
                        {
                            g += ort[i] * _v[i][j];
                        }

                        // Double division avoids possible underflow
                        g = (g / ort[m]) / _h[m][m - 1];
                        for(int i = m; i <= high; i++)
                        {
                            _v[i][j] += g * ort[i];
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Non symmetric reduction from Hessenberg to real Schur form.
        /// </summary>
        void
        NonsymmetricReduceHessenberToRealSchur()
        {
            /*
            This is derived from the Algol procedure hqr2,
            by Martin and Wilkinson, Handbook for Auto. Comp.,
            Vol.ii-Linear Algebra, and the corresponding
            Fortran subroutine in EISPACK.
            */

            /* Initialize */

            int nn = _n;
            int n = nn - 1;
            int low = 0;
            int high = nn - 1;
            double eps = Number.PositiveRelativeAccuracy;
            double exshift = 0.0;
            double p = 0, q = 0, r = 0, s = 0, z = 0;
            double w, x, y;

            /* Store roots isolated by balanc and compute matrix norm */

            double norm = 0.0;
            for(int i = 0; i < nn; i++)
            {
                if(i < low | i > high)
                {
                    _d[i] = _h[i][i];
                    _e[i] = 0.0;
                }

                for(int j = Math.Max(i - 1, 0); j < nn; j++)
                {
                    norm = norm + Math.Abs(_h[i][j]);
                }
            }

            /* Outer loop over eigenvalue index */

            int iter = 0;
            while(n >= low)
            {
                /* Look for single small sub-diagonal element */

                int l = n;
                while(l > low)
                {
                    s = Math.Abs(_h[l - 1][l - 1]) + Math.Abs(_h[l][l]);

                    if(s == 0.0)
                    {
                        s = norm;
                    }

                    if(Math.Abs(_h[l][l - 1]) < eps * s)
                    {
                        break;
                    }

                    l--;
                }

                /*
                Check for convergence
                One root found
                */

                if(l == n)
                {
                    _h[n][n] = _h[n][n] + exshift;
                    _d[n] = _h[n][n];
                    _e[n] = 0.0;
                    n--;
                    iter = 0;

                    // Two roots found
                }
                else if(l == n - 1)
                {
                    w = _h[n][n - 1] * _h[n - 1][n];
                    p = (_h[n - 1][n - 1] - _h[n][n]) / 2.0;
                    q = (p * p) + w;
                    z = Math.Sqrt(Math.Abs(q));
                    _h[n][n] = _h[n][n] + exshift;
                    _h[n - 1][n - 1] = _h[n - 1][n - 1] + exshift;
                    x = _h[n][n];

                    /* Real pair */

                    if(q >= 0)
                    {
                        if(p >= 0)
                        {
                            z = p + z;
                        }
                        else
                        {
                            z = p - z;
                        }

                        _d[n - 1] = x + z;

                        _d[n] = _d[n - 1];
                        if(z != 0.0)
                        {
                            _d[n] = x - (w / z);
                        }

                        _e[n - 1] = 0.0;
                        _e[n] = 0.0;
                        x = _h[n][n - 1];
                        s = Math.Abs(x) + Math.Abs(z);
                        p = x / s;
                        q = z / s;
                        r = Math.Sqrt((p * p) + (q * q));
                        p = p / r;
                        q = q / r;

                        /* Row modification */

                        for(int j = n - 1; j < nn; j++)
                        {
                            z = _h[n - 1][j];
                            _h[n - 1][j] = (q * z) + (p * _h[n][j]);
                            _h[n][j] = (q * _h[n][j]) - (p * z);
                        }

                        /* Column modification */

                        for(int i = 0; i <= n; i++)
                        {
                            z = _h[i][n - 1];
                            _h[i][n - 1] = (q * z) + (p * _h[i][n]);
                            _h[i][n] = (q * _h[i][n]) - (p * z);
                        }

                        /* Accumulate transformations */

                        for(int i = low; i <= high; i++)
                        {
                            z = _v[i][n - 1];
                            _v[i][n - 1] = (q * z) + (p * _v[i][n]);
                            _v[i][n] = (q * _v[i][n]) - (p * z);
                        }

                        // Complex pair
                    }
                    else
                    {
                        _d[n - 1] = x + p;
                        _d[n] = x + p;
                        _e[n - 1] = z;
                        _e[n] = -z;
                    }

                    n = n - 2;
                    iter = 0;

                    // No convergence yet
                }
                else
                {
                    /* Form shift */

                    x = _h[n][n];
                    y = 0.0;
                    w = 0.0;
                    if(l < n)
                    {
                        y = _h[n - 1][n - 1];
                        w = _h[n][n - 1] * _h[n - 1][n];
                    }

                    /* Wilkinson's original ad hoc shift */

                    if(iter == 10)
                    {
                        exshift += x;
                        for(int i = low; i <= n; i++)
                        {
                            _h[i][i] -= x;
                        }

                        s = Math.Abs(_h[n][n - 1]) + Math.Abs(_h[n - 1][n - 2]);
                        x = y = 0.75 * s;
                        w = (-0.4375) * s * s;
                    }

                    /* MATLAB's new ad hoc shift */

                    if(iter == 30)
                    {
                        s = (y - x) / 2.0;
                        s = (s * s) + w;
                        if(s > 0)
                        {
                            s = Math.Sqrt(s);
                            if(y < x)
                            {
                                s = -s;
                            }

                            s = x - (w / (((y - x) / 2.0) + s));
                            for(int i = low; i <= n; i++)
                            {
                                _h[i][i] -= s;
                            }

                            exshift += s;
                            x = y = w = 0.964;
                        }
                    }

                    iter = iter + 1; // (Could check iteration count here.)

                    /* Look for two consecutive small sub-diagonal elements */

                    int m = n - 2;
                    while(m >= l)
                    {
                        z = _h[m][m];
                        r = x - z;
                        s = y - z;
                        p = (((r * s) - w) / _h[m + 1][m]) + _h[m][m + 1];
                        q = _h[m + 1][m + 1] - z - r - s;
                        r = _h[m + 2][m + 1];
                        s = Math.Abs(p) + Math.Abs(q) + Math.Abs(r);
                        p = p / s;
                        q = q / s;
                        r = r / s;

                        if(m == l)
                        {
                            break;
                        }

                        if(Math.Abs(_h[m][m - 1]) * (Math.Abs(q) + Math.Abs(r)) < eps * (Math.Abs(p) * (Math.Abs(_h[m - 1][m - 1]) + Math.Abs(z) + Math.Abs(_h[m + 1][m + 1]))))
                        {
                            break;
                        }

                        m--;
                    }

                    for(int i = m + 2; i <= n; i++)
                    {
                        _h[i][i - 2] = 0.0;
                        if(i > m + 2)
                        {
                            _h[i][i - 3] = 0.0;
                        }
                    }

                    /* Double QR step involving rows l:n and columns m:n */

                    for(int k = m; k <= n - 1; k++)
                    {
                        bool notlast = (k != n - 1);

                        if(k != m)
                        {
                            p = _h[k][k - 1];
                            q = _h[k + 1][k - 1];
                            r = (notlast ? _h[k + 2][k - 1] : 0.0);
                            x = Math.Abs(p) + Math.Abs(q) + Math.Abs(r);
                            if(x != 0.0)
                            {
                                p = p / x;
                                q = q / x;
                                r = r / x;
                            }
                        }

                        if(x == 0.0)
                        {
                            break;
                        }

                        s = Math.Sqrt((p * p) + (q * q) + (r * r));
                        if(p < 0)
                        {
                            s = -s;
                        }

                        if(s != 0.0)
                        {
                            if(k != m)
                            {
                                _h[k][k - 1] = (-s) * x;
                            }
                            else if(l != m)
                            {
                                _h[k][k - 1] = -_h[k][k - 1];
                            }

                            p = p + s;
                            x = p / s;
                            y = q / s;
                            z = r / s;
                            q = q / p;
                            r = r / p;

                            /* Row modification */

                            for(int j = k; j < nn; j++)
                            {
                                p = _h[k][j] + (q * _h[k + 1][j]);

                                if(notlast)
                                {
                                    p = p + (r * _h[k + 2][j]);
                                    _h[k + 2][j] = _h[k + 2][j] - (p * z);
                                }

                                _h[k][j] = _h[k][j] - (p * x);
                                _h[k + 1][j] = _h[k + 1][j] - (p * y);
                            }

                            /* Column modification */

                            for(int i = 0; i <= Math.Min(n, k + 3); i++)
                            {
                                double[] hi = _h[i];
                                p = (x * hi[k]) + (y * hi[k + 1]);

                                if(notlast)
                                {
                                    p = p + (z * hi[k + 2]);
                                    hi[k + 2] = hi[k + 2] - (p * r);
                                }

                                hi[k] = hi[k] - p;
                                hi[k + 1] = hi[k + 1] - (p * q);
                            }

                            /* Accumulate transformations */

                            for(int i = low; i <= high; i++)
                            {
                                double[] vi = _v[i];
                                p = (x * vi[k]) + (y * vi[k + 1]);

                                if(notlast)
                                {
                                    p = p + (z * vi[k + 2]);
                                    vi[k + 2] = vi[k + 2] - (p * r);
                                }

                                vi[k] = vi[k] - p;
                                vi[k + 1] = vi[k + 1] - (p * q);
                            }
                        } // (s != 0)
                    } // k loop
                } // check convergence
            } // while (n >= low)

            /* Backsubstitute to find vectors of upper triangular form */

            if(norm == 0.0)
            {
                return;
            }

            for(n = nn - 1; n >= 0; n--)
            {
                p = _d[n];
                q = _e[n];

                /* Real vector */

                double t;
                if(q == 0.0)
                {
                    int l = n;
                    _h[n][n] = 1.0;
                    for(int i = n - 1; i >= 0; i--)
                    {
                        w = _h[i][i] - p;
                        r = 0.0;
                        for(int j = l; j <= n; j++)
                        {
                            r = r + (_h[i][j] * _h[j][n]);
                        }

                        if(_e[i] < 0.0)
                        {
                            z = w;
                            s = r;
                        }
                        else
                        {
                            l = i;
                            if(_e[i] == 0.0)
                            {
                                if(w != 0.0)
                                {
                                    _h[i][n] = (-r) / w;
                                }
                                else
                                {
                                    _h[i][n] = (-r) / (eps * norm);
                                }

                                // Solve real equations
                            }
                            else
                            {
                                x = _h[i][i + 1];
                                y = _h[i + 1][i];
                                q = ((_d[i] - p) * (_d[i] - p)) + (_e[i] * _e[i]);
                                t = ((x * s) - (z * r)) / q;
                                _h[i][n] = t;
                                if(Math.Abs(x) > Math.Abs(z))
                                {
                                    _h[i + 1][n] = (-r - (w * t)) / x;
                                }
                                else
                                {
                                    _h[i + 1][n] = (-s - (y * t)) / z;
                                }
                            }

                            /* Overflow control */

                            t = Math.Abs(_h[i][n]);
                            if((eps * t) * t > 1)
                            {
                                for(int j = i; j <= n; j++)
                                {
                                    _h[j][n] = _h[j][n] / t;
                                }
                            }
                        }
                    }

                    // Complex vector
                }
                else if(q < 0)
                {
                    int l = n - 1;

                    /* Last vector component imaginary so matrix is triangular */

                    if(Math.Abs(_h[n][n - 1]) > Math.Abs(_h[n - 1][n]))
                    {
                        _h[n - 1][n - 1] = q / _h[n][n - 1];
                        _h[n - 1][n] = (-(_h[n][n] - p)) / _h[n][n - 1];
                    }
                    else
                    {
                        ComplexScalarDivide(0.0, -_h[n - 1][n], _h[n - 1][n - 1] - p, q);
                        _h[n - 1][n - 1] = _cdivr;
                        _h[n - 1][n] = _cdivi;
                    }

                    _h[n][n - 1] = 0.0;
                    _h[n][n] = 1.0;
                    for(int i = n - 2; i >= 0; i--)
                    {
                        double ra = 0.0;
                        double sa = 0.0;
                        for(int j = l; j <= n; j++)
                        {
                            ra = ra + (_h[i][j] * _h[j][n - 1]);
                            sa = sa + (_h[i][j] * _h[j][n]);
                        }

                        w = _h[i][i] - p;

                        if(_e[i] < 0.0)
                        {
                            z = w;
                            r = ra;
                            s = sa;
                        }
                        else
                        {
                            l = i;
                            if(_e[i] == 0.0)
                            {
                                ComplexScalarDivide(-ra, -sa, w, q);
                                _h[i][n - 1] = _cdivr;
                                _h[i][n] = _cdivi;
                            }
                            else
                            {
                                /* Solve complex equations */

                                x = _h[i][i + 1];
                                y = _h[i + 1][i];
                                
                                double vr = ((_d[i] - p) * (_d[i] - p)) + (_e[i] * _e[i]) - (q * q);
                                double vi = (_d[i] - p) * 2.0 * q;
                                if((vr == 0.0) && (vi == 0.0))
                                {
                                    vr = eps * norm * (Math.Abs(w) + Math.Abs(q) + Math.Abs(x) + Math.Abs(y) + Math.Abs(z));
                                }

                                ComplexScalarDivide((x * r) - (z * ra) + (q * sa), (x * s) - (z * sa) - (q * ra), vr, vi);
                                _h[i][n - 1] = _cdivr;
                                _h[i][n] = _cdivi;
                                if(Math.Abs(x) > (Math.Abs(z) + Math.Abs(q)))
                                {
                                    _h[i + 1][n - 1] = (-ra - (w * _h[i][n - 1]) + (q * _h[i][n])) / x;
                                    _h[i + 1][n] = (-sa - (w * _h[i][n]) - (q * _h[i][n - 1])) / x;
                                }
                                else
                                {
                                    ComplexScalarDivide(-r - (y * _h[i][n - 1]), -s - (y * _h[i][n]), z, q);
                                    _h[i + 1][n - 1] = _cdivr;
                                    _h[i + 1][n] = _cdivi;
                                }
                            }

                            /* Overflow control */

                            t = Math.Max(Math.Abs(_h[i][n - 1]), Math.Abs(_h[i][n]));
                            if((eps * t) * t > 1)
                            {
                                for(int j = i; j <= n; j++)
                                {
                                    _h[j][n - 1] = _h[j][n - 1] / t;
                                    _h[j][n] = _h[j][n] / t;
                                }
                            }
                        }
                    }
                }
            }

            /* Vectors of isolated roots */

            for(int i = 0; i < nn; i++)
            {
                if(i < low | i > high)
                {
                    for(int j = i; j < nn; j++)
                    {
                        _v[i][j] = _h[i][j];
                    }
                }
            }

            /* Back transformation to get eigenvectors of original matrix */

            for(int j = nn - 1; j >= low; j--)
            {
                for(int i = low; i <= high; i++)
                {
                    z = 0.0;
                    for(int k = low; k <= Math.Min(j, high); k++)
                    {
                        z = z + (_v[i][k] * _h[k][j]);
                    }

                    _v[i][j] = z;
                }
            }
        }

        /// <summary>
        /// Complex scalar division.
        /// </summary>
        void
        ComplexScalarDivide(
            double xr,
            double xi,
            double yr,
            double yi)
        {
            /* TODO (ruegg, 2008-03-11): Refactor */

            double r, d;
            if(Math.Abs(yr) > Math.Abs(yi))
            {
                r = yi / yr;
                d = yr + (r * yi);
                _cdivr = (xr + (r * xi)) / d;
                _cdivi = (xi - (r * xr)) / d;
            }
            else
            {
                r = yr / yi;
                d = yi + (r * yr);
                _cdivr = ((r * xr) + xi) / d;
                _cdivi = ((r * xi) - xr) / d;
            }
        }
    }
}
