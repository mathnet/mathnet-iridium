//-----------------------------------------------------------------------
// <copyright file="SingularValueDecomposition.cs" company="Math.NET Project">
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
    /// <summary>Singular Value Decomposition.</summary>
    /// <remarks>
    /// <para>
    /// For an m-by-n matrix A with m >= n, the singular value decomposition 
    /// is an m-by-n orthogonal matrix U, an n-by-n diagonal matrix S, and
    /// an n-by-n orthogonal matrix V so that A = U*S*V'.
    /// </para>
    /// <para>
    /// The singular values, sigma[k] = S[k, k], are ordered so that
    /// sigma[0] >= sigma[1] >= ... >= sigma[n-1].
    /// </para>
    /// <para>
    /// The singular value decomposition always exists, so the constructor will
    /// never fail.  The matrix condition number and the effective numerical
    /// rank can be computed from this decomposition.
    /// </para>
    /// </remarks>
    [Serializable]
    public class SingularValueDecomposition
    {
        readonly Matrix _u;
        readonly Matrix _v;

        /// <summary>Array for internal storage of singular values.</summary>
        readonly Vector _singular;

        /// <summary>Row dimensions.</summary>
        readonly int _m;

        /// <summary>Column dimensions.</summary>
        readonly int _n;

        /// <summary>Indicates whether all the results provided by the
        /// method or properties should be transposed.</summary>
        /// <remarks>
        /// (vermorel) The initial implementation was assuming that
        /// m &gt;= n, but in fact, it is easy to handle the case m &lt; n
        /// by transposing all the results.
        /// </remarks>
        readonly bool _transpose;

        OnDemandComputation<Matrix> _diagonalSingularValuesOnDemand;
        OnDemandComputation<int> _rankOnDemand;

        private enum IterationStep
        {
            /// <summary>if s[p] and e[k-1] are negligible and k&lt;p.</summary>
            DeflateNeglible,

            /// <summary>if s[k] is negligible and k&lt;p.</summary>
            SplitAtNeglible,

            /// <summary>if e[k-1] is negligible, k&lt;p, and s[k], ..., s[p] are not negligible.</summary>
            QR,

            /// <summary>if e[p-1] is negligible.</summary>
            Convergence
        }

        /// <summary>
        /// Initializes a new instance of the SingularValueDecomposition class.
        /// </summary>
        /// <remarks>Provides access to U, S and V.</remarks>
        /// <param name="arg">Rectangular matrix</param>
        public
        SingularValueDecomposition(IMatrix<double> arg)
        {
            _transpose = (arg.RowCount < arg.ColumnCount);

            // Derived from LINPACK code.
            // Initialize.
            double[][] a;
            if(_transpose)
            {
                // copy of internal data, independent of Arg
                a = Matrix.Transpose(arg).GetArray();
                _m = arg.ColumnCount;
                _n = arg.RowCount;
            }
            else
            {
                a = arg.CopyToJaggedArray();
                _m = arg.RowCount;
                _n = arg.ColumnCount;
            }

            int nu = Math.Min(_m, _n);
            double[] s = new double[Math.Min(_m + 1, _n)];
            double[][] u = Matrix.CreateMatrixData(_m, nu);
            double[][] v = Matrix.CreateMatrixData(_n, _n);

            double[] e = new double[_n];
            double[] work = new double[_m];

            /*
            Reduce A to bidiagonal form, storing the diagonal elements
            in s and the super-diagonal elements in e.
            */

            int nct = Math.Min(_m - 1, _n);
            int nrt = Math.Max(0, Math.Min(_n - 2, _m));
            for(int k = 0; k < Math.Max(nct, nrt); k++)
            {
                if(k < nct)
                {
                    // Compute the transformation for the k-th column and
                    // place the k-th diagonal in s[k].
                    // Compute 2-norm of k-th column without under/overflow.
                    s[k] = 0;

                    for(int i = k; i < _m; i++)
                    {
                        s[k] = Fn.Hypot(s[k], a[i][k]);
                    }

                    if(s[k] != 0.0)
                    {
                        if(a[k][k] < 0.0)
                        {
                            s[k] = -s[k];
                        }

                        for(int i = k; i < _m; i++)
                        {
                            a[i][k] /= s[k];
                        }

                        a[k][k] += 1.0;
                    }

                    s[k] = -s[k];
                }

                for(int j = k + 1; j < _n; j++)
                {
                    if((k < nct) & (s[k] != 0.0))
                    {
                        /* Apply the transformation */

                        double t = 0;
                        for(int i = k; i < _m; i++)
                        {
                            t += a[i][k] * a[i][j];
                        }

                        t = (-t) / a[k][k];
                        for(int i = k; i < _m; i++)
                        {
                            a[i][j] += t * a[i][k];
                        }
                    }

                    /*
                    Place the k-th row of A into e for the
                    subsequent calculation of the row transformation.
                    */

                    e[j] = a[k][j];
                }

                if(k < nct)
                {
                    /*
                    Place the transformation in U for subsequent back
                    multiplication.
                    */

                    for(int i = k; i < _m; i++)
                    {
                        u[i][k] = a[i][k];
                    }
                }

                if(k < nrt)
                {
                    // Compute the k-th row transformation and place the
                    // k-th super-diagonal in e[k].
                    // Compute 2-norm without under/overflow.
                    e[k] = 0;

                    for(int i = k + 1; i < _n; i++)
                    {
                        e[k] = Fn.Hypot(e[k], e[i]);
                    }

                    if(e[k] != 0.0)
                    {
                        if(e[k + 1] < 0.0)
                        {
                            e[k] = -e[k];
                        }

                        for(int i = k + 1; i < _n; i++)
                        {
                            e[i] /= e[k];
                        }

                        e[k + 1] += 1.0;
                    }

                    e[k] = -e[k];

                    if((k + 1 < _m) & (e[k] != 0.0))
                    {
                        /* Apply the transformation */

                        for(int i = k + 1; i < _m; i++)
                        {
                            work[i] = 0.0;
                        }

                        for(int j = k + 1; j < _n; j++)
                        {
                            for(int i = k + 1; i < _m; i++)
                            {
                                work[i] += e[j] * a[i][j];
                            }
                        }

                        for(int j = k + 1; j < _n; j++)
                        {
                            double t = (-e[j]) / e[k + 1];
                            for(int i = k + 1; i < _m; i++)
                            {
                                a[i][j] += t * work[i];
                            }
                        }
                    }

                    /*
                    Place the transformation in V for subsequent
                    back multiplication.
                    */

                    for(int i = k + 1; i < _n; i++)
                    {
                        v[i][k] = e[i];
                    }
                }
            }

            /* Set up the final bidiagonal matrix or order p. */

            int p = Math.Min(_n, _m + 1);

            if(nct < _n)
            {
                s[nct] = a[nct][nct];
            }

            if(_m < p)
            {
                s[p - 1] = 0.0;
            }

            if(nrt + 1 < p)
            {
                e[nrt] = a[nrt][p - 1];
            }

            e[p - 1] = 0.0;

            /* If required, generate U */

            for(int j = nct; j < nu; j++)
            {
                for(int i = 0; i < _m; i++)
                {
                    u[i][j] = 0.0;
                }

                u[j][j] = 1.0;
            }

            for(int k = nct - 1; k >= 0; k--)
            {
                if(s[k] != 0.0)
                {
                    for(int j = k + 1; j < nu; j++)
                    {
                        double t = 0;
                        for(int i = k; i < _m; i++)
                        {
                            t += u[i][k] * u[i][j];
                        }

                        t = (-t) / u[k][k];
                        for(int i = k; i < _m; i++)
                        {
                            u[i][j] += t * u[i][k];
                        }
                    }

                    for(int i = k; i < _m; i++)
                    {
                        u[i][k] = -u[i][k];
                    }

                    u[k][k] = 1.0 + u[k][k];
                    for(int i = 0; i < k - 1; i++)
                    {
                        u[i][k] = 0.0;
                    }
                }
                else
                {
                    for(int i = 0; i < _m; i++)
                    {
                        u[i][k] = 0.0;
                    }

                    u[k][k] = 1.0;
                }
            }

            /* If required, generate V */

            for(int k = _n - 1; k >= 0; k--)
            {
                if((k < nrt) & (e[k] != 0.0))
                {
                    for(int j = k + 1; j < nu; j++)
                    {
                        double t = 0;
                        for(int i = k + 1; i < _n; i++)
                        {
                            t += v[i][k] * v[i][j];
                        }

                        t = (-t) / v[k + 1][k];
                        for(int i = k + 1; i < _n; i++)
                        {
                            v[i][j] += t * v[i][k];
                        }
                    }
                }

                for(int i = 0; i < _n; i++)
                {
                    v[i][k] = 0.0;
                }

                v[k][k] = 1.0;
            }
  
            /* Main iteration loop for the singular values */

            int pp = p - 1;
            int iter = 0;
            double eps = Number.PositiveRelativeAccuracy;
            while(p > 0)
            {
                int k;
                IterationStep step;

                /* Here is where a test for too many iterations would go */

                /*
                This section of the program inspects for
                negligible elements in the s and e arrays.  On
                completion the variables kase and k are set as follows.

                DeflateNeglible:  if s[p] and e[k-1] are negligible and k<p
                SplitAtNeglible:  if s[k] is negligible and k<p
                QR:               if e[k-1] is negligible, k<p, and s[k], ..., s[p] are not negligible.
                Convergence:      if e[p-1] is negligible.
                */

                for(k = p - 2; k >= 0; k--)
                {
                    if(Math.Abs(e[k]) <= eps * (Math.Abs(s[k]) + Math.Abs(s[k + 1])))
                    {
                        e[k] = 0.0;
                        break;
                    }
                }

                if(k == p - 2)
                {
                    step = IterationStep.Convergence;
                }
                else
                {
                    int ks;
                    for(ks = p - 1; ks >= k; ks--)
                    {
                        if(ks == k)
                        {
                            break;
                        }

                        double t = (ks != p ? Math.Abs(e[ks]) : 0.0) + (ks != k + 1 ? Math.Abs(e[ks - 1]) : 0.0);
                        if(Math.Abs(s[ks]) <= eps * t)
                        {
                            s[ks] = 0.0;
                            break;
                        }
                    }

                    if(ks == k)
                    {
                        step = IterationStep.QR;
                    }
                    else if(ks == p - 1)
                    {
                        step = IterationStep.DeflateNeglible;
                    }
                    else
                    {
                        step = IterationStep.SplitAtNeglible;
                        k = ks;
                    }
                }

                k++;

                /* Perform the task indicated by 'step'. */

                switch(step)
                {
                    // Deflate negligible s(p).
                    case IterationStep.DeflateNeglible:
                        {
                            double f = e[p - 2];
                            e[p - 2] = 0.0;
                            for(int j = p - 2; j >= k; j--)
                            {
                                double t = Fn.Hypot(s[j], f);
                                double cs = s[j] / t;
                                double sn = f / t;
                                s[j] = t;

                                if(j != k)
                                {
                                    f = (-sn) * e[j - 1];
                                    e[j - 1] = cs * e[j - 1];
                                }

                                for(int i = 0; i < _n; i++)
                                {
                                    t = (cs * v[i][j]) + (sn * v[i][p - 1]);
                                    v[i][p - 1] = ((-sn) * v[i][j]) + (cs * v[i][p - 1]);
                                    v[i][j] = t;
                                }
                            }
                        }

                        break;

                    // Split at negligible s(k)
                    case IterationStep.SplitAtNeglible:
                        {
                            double f = e[k - 1];
                            e[k - 1] = 0.0;
                            for(int j = k; j < p; j++)
                            {
                                double t = Fn.Hypot(s[j], f);
                                double cs = s[j] / t;
                                double sn = f / t;
                                s[j] = t;
                                f = (-sn) * e[j];
                                e[j] = cs * e[j];

                                for(int i = 0; i < _m; i++)
                                {
                                    t = (cs * u[i][j]) + (sn * u[i][k - 1]);
                                    u[i][k - 1] = ((-sn) * u[i][j]) + (cs * u[i][k - 1]);
                                    u[i][j] = t;
                                }
                            }
                        }

                        break;

                    // Perform one qr step.
                    case IterationStep.QR:
                        {
                            /* Calculate the shift */

                            double scale = Math.Max(Math.Max(Math.Max(Math.Max(Math.Abs(s[p - 1]), Math.Abs(s[p - 2])), Math.Abs(e[p - 2])), Math.Abs(s[k])), Math.Abs(e[k]));
                            double sp = s[p - 1] / scale;
                            double spm1 = s[p - 2] / scale;
                            double epm1 = e[p - 2] / scale;
                            double sk = s[k] / scale;
                            double ek = e[k] / scale;
                            double b = (((spm1 + sp) * (spm1 - sp)) + (epm1 * epm1)) / 2.0;
                            double c = (sp * epm1) * (sp * epm1);
                            double shift = 0.0;
                            if((b != 0.0) | (c != 0.0))
                            {
                                shift = Math.Sqrt((b * b) + c);

                                if(b < 0.0)
                                {
                                    shift = -shift;
                                }

                                shift = c / (b + shift);
                            }

                            double f = ((sk + sp) * (sk - sp)) + shift;
                            double g = sk * ek;

                            /* Chase zeros */

                            for(int j = k; j < p - 1; j++)
                            {
                                double t = Fn.Hypot(f, g);
                                double cs = f / t;
                                double sn = g / t;

                                if(j != k)
                                {
                                    e[j - 1] = t;
                                }

                                f = (cs * s[j]) + (sn * e[j]);
                                e[j] = (cs * e[j]) - (sn * s[j]);
                                g = sn * s[j + 1];
                                s[j + 1] = cs * s[j + 1];

                                for(int i = 0; i < _n; i++)
                                {
                                    t = (cs * v[i][j]) + (sn * v[i][j + 1]);
                                    v[i][j + 1] = ((-sn) * v[i][j]) + (cs * v[i][j + 1]);
                                    v[i][j] = t;
                                }

                                t = Fn.Hypot(f, g);
                                cs = f / t;
                                sn = g / t;
                                s[j] = t;
                                f = (cs * e[j]) + (sn * s[j + 1]);
                                s[j + 1] = ((-sn) * e[j]) + (cs * s[j + 1]);
                                g = sn * e[j + 1];
                                e[j + 1] = cs * e[j + 1];

                                if(j < _m - 1)
                                {
                                    for(int i = 0; i < _m; i++)
                                    {
                                        t = (cs * u[i][j]) + (sn * u[i][j + 1]);
                                        u[i][j + 1] = ((-sn) * u[i][j]) + (cs * u[i][j + 1]);
                                        u[i][j] = t;
                                    }
                                }
                            }

                            e[p - 2] = f;
                            iter = iter + 1;
                        }

                        break;

                    // Convergence.
                    case IterationStep.Convergence:
                        {
                            /* Make the singular values positive */

                            if(s[k] <= 0.0)
                            {
                                s[k] = (s[k] < 0.0 ? -s[k] : 0.0);

                                for(int i = 0; i <= pp; i++)
                                {
                                    v[i][k] = -v[i][k];
                                }
                            }

                            /* Order the singular values */

                            while(k < pp)
                            {
                                if(s[k] >= s[k + 1])
                                {
                                    break;
                                }

                                double t = s[k];
                                s[k] = s[k + 1];
                                s[k + 1] = t;

                                if(k < _n - 1)
                                {
                                    for(int i = 0; i < _n; i++)
                                    {
                                        t = v[i][k + 1];
                                        v[i][k + 1] = v[i][k];
                                        v[i][k] = t;
                                    }
                                }

                                if(k < _m - 1)
                                {
                                    for(int i = 0; i < _m; i++)
                                    {
                                        t = u[i][k + 1];
                                        u[i][k + 1] = u[i][k];
                                        u[i][k] = t;
                                    }
                                }

                                k++;
                            }

                            iter = 0;
                            p--;
                        }

                        break;
                }
            }

            // (vermorel) transposing the results if needed
            if(_transpose)
            {
                // swaping U and V
                double[][] temp = v;
                v = u;
                u = temp;
            }

            _u = new Matrix(u);
            _v = new Matrix(v);
            _singular = new Vector(s);

            InitOnDemandComputations();
        }

        /// <summary>Gets the one-dimensional array of singular values.</summary>
        /// <returns>diagonal of S.</returns>
        public Vector SingularValues
        {
            get { return _singular; }
        }

        /// <summary>Get the diagonal matrix of singular values.</summary>
        public Matrix S
        {
            get
            {
                // TODO: bad name for this property
                return _diagonalSingularValuesOnDemand.Compute();
            }
        }

        /// <summary>Gets the left singular vectors (U matrix).</summary>
        public Matrix LeftSingularVectors
        {
            get { return _u; }
        }

        /// <summary>Gets the right singular vectors (V matrix).</summary>
        public Matrix RightSingularVectors
        {
            get { return _v; }
        }

        /// <summary>Two norm.</summary>
        /// <returns>max(S)</returns>
        public
        double
        Norm2()
        {
            // TODO (ruegg, 2008-03-11): Change to property
            return _singular[0];
        }

        /// <summary>Two norm condition number.</summary>
        /// <returns>max(S)/min(S)</returns>
        public
        double
        Condition()
        {
            // TODO (ruegg, 2008-03-11): Change to property
            return _singular[0] / _singular[Math.Min(_m, _n) - 1];
        }

        /// <summary>Effective numerical matrix rank - Number of non negligible singular values.</summary>
        public
        int
        Rank()
        {
            // TODO (ruegg, 2008-03-11): Change to property
            return _rankOnDemand.Compute();
        }

        void
        InitOnDemandComputations()
        {
            _diagonalSingularValuesOnDemand = new OnDemandComputation<Matrix>(ComputeDiagonalSingularValues);
            _rankOnDemand = new OnDemandComputation<int>(ComputeRank);
        }

        Matrix
        ComputeDiagonalSingularValues()
        {
            return Matrix.Diagonal(_singular);
        }

        int
        ComputeRank()
        {
            double tol = Math.Max(_m, _n) * _singular[0] * Number.PositiveRelativeAccuracy;
            int r = 0;

            for(int i = 0; i < _singular.Length; i++)
            {
                if(_singular[i] > tol)
                {
                    r++;
                }
            }

            return r;
        }
    }
}
