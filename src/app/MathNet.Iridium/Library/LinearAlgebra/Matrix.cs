//-----------------------------------------------------------------------
// <copyright file="Matrix.cs" company="Math.NET Project">
//    Copyright (c) 2002-2009, Joannes Vermorel, Christoph Rüegg.
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
using System.Collections.Generic;
using System.Text;

namespace MathNet.Numerics.LinearAlgebra
{
    using Distributions;

    /// <summary>
    /// Real matrix.
    /// </summary>
    /// <remarks>
    /// The class <c>Matrix</c> provides the elementary operations
    /// on matrices (addition, multiplication, inversion, transposition, ...).
    /// Helpers to handle sub-matrices are also provided.
    /// </remarks>
    [Serializable]
    public class Matrix :
        IMatrix<double>,
        IEquatable<Matrix>,
        IAlmostEquatable<Matrix>,
        ICloneable
    {
        int _rowCount;
        int _columnCount;

        /// <summary>
        /// Array for internal storage of elements.
        /// </summary>
        double[][] _data;

        /// <summary>
        /// Gets the number of rows.
        /// </summary>
        public int RowCount
        {
            get { return _rowCount; }
        }

        /// <summary>
        /// Gets the number of columns.
        /// </summary>
        public int ColumnCount
        {
            get { return _columnCount; }
        }

        /// <summary>
        /// Gets or set the element indexed by <c>(i, j)</c>
        /// in the <c>Matrix</c>.
        /// </summary>
        /// <param name="i">Row index.</param>
        /// <param name="j">Column index.</param>
        public double this[int i, int j]
        {
            get
            {
                return _data[i][j];
            }

            set
            {
                _data[i][j] = value;

                // NOTE (ruegg, 2008-03-11): The folloing line is cheap,
                // but still expensive if this setter is called
                // a lot of times.
                // - We should recommend out users to build the internal
                //   jagged array first and then create the matrix for it;
                //   or to get the internal double[][] handle.
                // - Consider to omit it here and make the users call
                //   ResetComputations() after they finished chaniging the matrix.
                ResetOnDemandComputations();
            }
        }

        OnDemandComputation<LUDecomposition> _luDecompositionOnDemand;
        OnDemandComputation<QRDecomposition> _qrDecompositionOnDemand;
        OnDemandComputation<CholeskyDecomposition> _choleskyDecompositionOnDemand;
        OnDemandComputation<SingularValueDecomposition> _singularValueDecompositionOnDemand;
        OnDemandComputation<EigenvalueDecomposition> _eigenValueDecompositionOnDemand;
        OnDemandComputation<double> _traceOnDemand;

        #region Data -> Matrix: Constructors and static constructive methods

        /// <summary>
        /// Initializes a new instance of the Matrix class,
        /// for an m-by-n matrix of zeros.
        /// </summary>
        /// <param name="m">Number of rows.</param>
        /// <param name="n">Number of columns.</param>
        public
        Matrix(int m, int n)
        {
            _data = CreateMatrixData(m, n);
            _rowCount = m;
            _columnCount = n;

            InitOnDemandComputations();
        }

        /// <summary>
        /// Initializes a new instance of the Matrix class,
        /// for an m-by-m square matrix.
        /// </summary>
        /// <param name="m">Size of the square matrix.</param>
        /// <param name="s">Diagonal value.</param>
        public
        Matrix(int m, double s)
        {
            _data = new double[m][];
            _rowCount = m;
            _columnCount = m;

            for(int i = 0; i < m; i++)
            {
                double[] col = new double[m];
                col[i] = s;
                _data[i] = col;
            }

            InitOnDemandComputations();
        }

        /// <summary>
        /// Initializes a new instance of the Matrix class,
        /// for an m-by-n constant matrix.
        /// </summary>
        /// <param name="m">Number of rows.</param>
        /// <param name="n">Number of columns.</param>
        /// <param name="s">Fill the matrix with this scalar value.</param>
        public
        Matrix(
            int m,
            int n,
            double s)
        {
            _data = new double[m][];
            _rowCount = m;
            _columnCount = n;

            for(int i = 0; i < m; i++)
            {
                double[] col = new double[n];
                for(int j = 0; j < n; j++)
                {
                    col[j] = s;
                }

                _data[i] = col;
            }

            InitOnDemandComputations();
        }

        /// <summary>
        /// Initializes a new instance of the Matrix class
        /// from a jagged 2-D array, directly using the provided array as internal data structure.
        /// </summary>
        /// <param name="a">Two-dimensional jagged array of doubles.</param>
        /// <exception cref="System.ArgumentException">All rows must have the same length.</exception>
        /// <seealso cref="Matrix.Create(double[][])"/>
        /// <seealso cref="Matrix.Create(double[,])"/>
        public
        Matrix(double[][] a)
        {
            _data = a;
            GetRowColumnCount(_data, out _rowCount, out _columnCount);

            InitOnDemandComputations();
        }

        /// <summary>
        /// Initializes a new instance of the Matrix class
        /// from a 2-D array by deep-copying the provided array to the internal data structure.
        /// </summary>
        /// <param name="a">Two-dimensional array of doubles.</param>
        [Obsolete("Use 'Matrix.Create(double[,])' or 'new Matrix(double[][])' instead")]
        [CLSCompliant(false)]
        public
        Matrix(double[,] a)
        {
            _rowCount = a.GetLength(0);
            _columnCount = a.GetLength(1);
            _data = new double[_rowCount][];

            for(int i = 0; i < _rowCount; i++)
            {
                double[] col = new double[_columnCount];
                for(int j = 0; j < _columnCount; j++)
                {
                    col[j] = a[i, j];
                }

                _data[i] = col;
            }

            InitOnDemandComputations();
        }

        /// <summary>
        /// Initializes a new instance of the Matrix class
        /// from a one-dimensional packed array.
        /// </summary>
        /// <param name="values">One-dimensional array of doubles, packed by columns (ala Fortran).</param>
        /// <param name="m">Number of rows.</param>
        /// <exception cref="System.ArgumentException">Array length must be a multiple of m.</exception>
        public
        Matrix(
            double[] values,
            int m)
        {
            _rowCount = m;
            if(m == 0)
            {
                _columnCount = 0;
                if(values.Length != 0)
                {
                    throw new ArgumentException(Properties.LocalStrings.ArgumentVectorLengthsMultipleOf("m"));
                }
            }
            else
            {
                int rem;
                _columnCount = Math.DivRem(values.Length, m, out rem);
                if(rem != 0)
                {
                    throw new ArgumentException(Properties.LocalStrings.ArgumentVectorLengthsMultipleOf("m"));
                }
            }

            _data = new double[_rowCount][];
            for(int i = 0; i < _rowCount; i++)
            {
                double[] col = new double[_columnCount];
                for(int j = 0; j < _columnCount; j++)
                {
                    col[j] = values[i + (j * _rowCount)];
                }

                _data[i] = col;
            }

            InitOnDemandComputations();
        }

        /// <summary>
        /// Constructs a matrix from a copy of a 2-D array by deep-copy.
        /// </summary>
        /// <param name="a">Two-dimensional array of doubles.</param>
        public static
        Matrix
        Create(double[][] a)
        {
            return new Matrix(CloneMatrixData(a));
        }

        /// <summary>
        /// Constructs a matrix from a copy of a 2-D array by deep-copy.
        /// </summary>
        /// <param name="a">Two-dimensional array of doubles.</param>
        [CLSCompliant(false)]
        public static
        Matrix
        Create(double[,] a)
        {
            int rows = a.GetLength(0);
            int columns = a.GetLength(1);
            double[][] newData = new double[rows][];

            for(int i = 0; i < rows; i++)
            {
                double[] col = new double[columns];
                for(int j = 0; j < columns; j++)
                {
                    col[j] = a[i, j];
                }

                newData[i] = col;
            }

            return new Matrix(newData);
        }

        /// <summary>
        /// Construct a real matrix from a real column vector.
        /// </summary>
        /// <remarks>
        /// Note that double arrays can implicitly cast to <see cref="Vector"/>,
        /// so you can also provide a double[] as parameter. In both cases,
        /// the matrix will copy the provided data to a local structure and is
        /// thus not affected of any later changes on said data.
        /// </remarks>
        public static
        Matrix
        CreateFromColumn(Vector columnVector)
        {
            if(null == columnVector)
            {
                throw new ArgumentNullException("columnVector");
            }

            int rows = columnVector.Length;
            double[][] newData = new double[rows][];

            for(int i = 0; i < rows; i++)
            {
                newData[i] = new double[] { columnVector[i] };
            }

            return new Matrix(newData);
        }

        /// <summary>
        /// Construct a real matrix from a real row vector.
        /// </summary>
        /// <remarks>
        /// Note that double arrays can implicitly cast to <see cref="Vector"/>,
        /// so you can also provide a double[] as parameter. In both cases,
        /// the matrix will copy the provided data to a local structure and is
        /// thus not affected of any later changes on said data.
        /// </remarks>
        public static
        Matrix
        CreateFromRow(Vector rowVector)
        {
            if(null == rowVector)
            {
                throw new ArgumentNullException("rowVector");
            }

            double[][] newData = new double[1][];

            newData[0] = rowVector.CopyToArray();

            return new Matrix(newData);
        }

        /// <summary>
        /// Construct a real matrix from a set of real column vectors.
        /// </summary>
        public static
        Matrix
        CreateFromColumns(IList<Vector> columnVectors)
        {
            if(null == columnVectors)
            {
                throw new ArgumentNullException("columnVectors");
            }

            if(0 == columnVectors.Count)
            {
                throw new ArgumentOutOfRangeException("columnVectors");
            }

            int rows = columnVectors[0].Length;
            int columns = columnVectors.Count;
            double[][] newData = new double[rows][];

            for(int i = 0; i < rows; i++)
            {
                double[] newRow = new double[columns];
                for(int j = 0; j < columns; j++)
                {
                    newRow[j] = columnVectors[j][i];
                }

                newData[i] = newRow;
            }

            return new Matrix(newData);
        }

        /// <summary>
        /// Construct a real matrix from a set of real row vectors.
        /// </summary>
        public static
        Matrix
        CreateFromRows(IList<Vector> rowVectors)
        {
            if(null == rowVectors)
            {
                throw new ArgumentNullException("rowVectors");
            }

            if(0 == rowVectors.Count)
            {
                throw new ArgumentOutOfRangeException("rowVectors");
            }

            int rows = rowVectors.Count;
            double[][] newData = new double[rows][];

            for(int i = 0; i < rows; i++)
            {
                newData[i] = rowVectors[i].CopyToArray();
            }

            return new Matrix(newData);
        }

        /// <summary>
        /// Generates identity matrix
        /// </summary>
        /// <param name="m">Number of rows.</param>
        /// <param name="n">Number of columns.</param>
        /// <returns>An m-by-n matrix with ones on the diagonal and zeros elsewhere.</returns>
        public static
        Matrix
        Identity(int m, int n)
        {
            double[][] data = new double[m][];
            for(int i = 0; i < m; i++)
            {
                double[] col = new double[n];
                if(i < n)
                {
                    col[i] = 1.0;
                }

                data[i] = col;
            }

            return new Matrix(data);
        }

        /// <summary>
        /// Generates an m-by-m matrix filled with 1.
        /// </summary>
        /// <param name="m">Number of rows = Number of columns</param>
        public static
        Matrix
        Ones(int m)
        {
            return new Matrix(m, m, 1.0);
        }

        /// <summary>
        /// Generates an m-by-m matrix filled with 0.
        /// </summary>
        /// <param name="m">Number of rows = Number of columns</param>
        public static
        Matrix
        Zeros(int m)
        {
            return new Matrix(m, m);
        }

        /// <summary>
        /// Creates a new diagonal m-by-n matrix based on the diagonal vector.
        /// </summary>
        /// <param name="diagonalVector">The values of the matrix diagonal.</param>
        /// <param name="m">Number of rows.</param>
        /// <param name="n">Number of columns.</param>
        /// <returns>
        /// An m-by-n matrix with the values from the diagonal vector on the diagonal and zeros elsewhere.
        /// </returns>
        public static
        Matrix
        Diagonal(
            IVector<double> diagonalVector,
            int m,
            int n)
        {
            double[][] data = new double[m][];
            for(int i = 0; i < m; i++)
            {
                double[] col = new double[n];
                if((i < n) && (i < diagonalVector.Length))
                {
                    col[i] = diagonalVector[i];
                }

                data[i] = col;
            }

            return new Matrix(data);
        }

        /// <summary>
        /// Creates a new square diagonal matrix based on the diagonal vector.
        /// </summary>
        /// <param name="diagonalVector">The values of the matrix diagonal.</param>
        /// <returns>
        /// An m-by-n matrix with the values from the diagonal vector on the diagonal and zeros elsewhere.
        /// </returns>
        public static
        Matrix
        Diagonal(IVector<double> diagonalVector)
        {
            return Diagonal(diagonalVector, diagonalVector.Length, diagonalVector.Length);
        }

        /// <summary>
        /// Generates matrix with random elements.
        /// </summary>
        /// <param name="m">Number of rows.</param>
        /// <param name="n">Number of columns.</param>
        /// <param name="randomDistribution">Continuous Random Distribution or Source</param>
        /// <returns>
        /// An m-by-n matrix with elements distributed according to the provided distribution.
        /// </returns>
        public static
        Matrix
        Random(
            int m,
            int n,
            IContinuousGenerator randomDistribution)
        {
            double[][] data = new double[m][];
            for(int i = 0; i < m; i++)
            {
                double[] col = new double[n];
                for(int j = 0; j < n; j++)
                {
                    col[j] = randomDistribution.NextDouble();
                }

                data[i] = col;
            }

            return new Matrix(data);
        }

        /// <summary>
        /// Generates matrix with standard-distributed random elements.
        /// </summary>
        /// <param name="m">Number of rows.</param>
        /// <param name="n">Number of columns.</param>
        /// <returns>
        /// An m-by-n matrix with uniformly distributed
        /// random elements in <c>[0, 1)</c> interval.
        /// </returns>
        public static
        Matrix
        Random(int m, int n)
        {
            return Random(m, n, new StandardDistribution());
        }

        #endregion //  Constructors

        #region Matrix -> Data: Back Conversions

        /// <summary>
        /// Copies the internal data structure to a 2-dimensional array.
        /// </summary>
        public
        double[,]
        CopyToArray()
        {
            double[,] newData = new double[_rowCount, _columnCount];
            for(int i = 0; i < _rowCount; i++)
            {
                for(int j = 0; j < _columnCount; j++)
                {
                    newData[i, j] = _data[i][j];
                }
            }

            return newData;
        }

        /// <summary>
        /// Copies the internal data structure to a jagged rectangular array.
        /// </summary>
        /// <returns></returns>
        public
        double[][]
        CopyToJaggedArray()
        {
            return CloneMatrixData(_data);
        }

        /// <summary>
        /// Returns the internal data structure array.
        /// </summary>
        public
        double[][]
        GetArray()
        {
            return _data;
        }

        /// <summary>Implicit conversion to a <c>double[,]</c> array.</summary>
        [Obsolete("Convert to double[][] instead.")]
        public static explicit
        operator double[,](Matrix m)
        {
            return m.CopyToArray();
        }

        /// <summary>Implicit conversion to a <c>double[][]</c> array.</summary>
        public static implicit
        operator double[][](Matrix m)
        {
            return m._data;
        }

        /// <summary>
        /// Explicit conversion to a <c>double[]</c> array of a single column matrix.
        /// </summary>
        /// <param name="m">Exactly one column expected.</param>
        public static explicit
        operator double[](Matrix m)
        {
            if(m.ColumnCount != 1)
            {
                throw new InvalidOperationException(Properties.LocalStrings.ArgumentMatrixSingleColumn);
            }

            double[] array = new double[m.RowCount];
            for(int i = 0; i < m.RowCount; i++)
            {
                array[i] = m[i, 0];
            }

            return array;
        }

        /// <summary>
        /// Explicit conversion to a <c>double</c> scalar of a single column and row (1-by-1) matrix.
        /// </summary>
        /// <param name="m">1-by-1 Matrix</param>
        public static explicit
        operator double(Matrix m)
        {
            if(m.ColumnCount != 1 || m.RowCount != 1)
            {
                throw new InvalidOperationException(Properties.LocalStrings.ArgumentMatrixSingleColumnRow);
            }

            return m[0, 0];
        }
        #endregion

        #region Internal Data Stucture

        /// <summary>
        /// Create the internal matrix data structure for a matrix of the given size.
        /// Initializing matrices directly on the internal structure may be faster
        /// than accessing the cells through the matrix class.
        /// </summary>
        /// <param name="m">Number of rows.</param>
        /// <param name="n">Number of columns.</param>
        public static
        double[][]
        CreateMatrixData(int m, int n)
        {
            double[][] data = new double[m][];
            for(int i = 0; i < m; i++)
            {
                data[i] = new double[n];
            }

            return data;
        }

        /// <summary>
        /// Creates a copy of a given internal matrix data structure.
        /// </summary>
        public static
        double[][]
        CloneMatrixData(double[][] data)
        {
            int rows, columns;
            GetRowColumnCount(data, out rows, out columns);
            double[][] newData = new double[rows][];
            for(int i = 0; i < rows; i++)
            {
                double[] col = new double[columns];
                for(int j = 0; j < columns; j++)
                {
                    col[j] = data[i][j];
                }

                newData[i] = col;
            }

            return newData;
        }

        /// <summary>
        /// Tries to find out the row column count of a given internal matrix data structure.
        /// </summary>
        public static
        void
        GetRowColumnCount(
            double[][] data,
            out int rows,
            out int columns)
        {
            rows = data.Length;
            columns = (rows == 0) ? 0 : data[0].Length;
        }

        #endregion

        #region Sub-matrices operation

        /// <summary>
        /// Copies a specified column of this matrix to a new vector.
        /// </summary>
        public
        Vector
        GetColumnVector(int columnIndex)
        {
            if(columnIndex < 0 || columnIndex >= _columnCount)
            {
                throw new ArgumentOutOfRangeException("columnIndex");
            }

            double[] newData = new double[_rowCount];

            for(int i = 0; i < _rowCount; i++)
            {
                newData[i] = _data[i][columnIndex];
            }

            return new Vector(newData);
        }

        /// <summary>
        /// Copies a specified row of this matrix to a new vector.
        /// </summary>
        public
        Vector
        GetRowVector(int rowIndex)
        {
            if(rowIndex < 0 || rowIndex >= _rowCount)
            {
                throw new ArgumentOutOfRangeException("rowIndex");
            }

            double[] newData = new double[_columnCount];
            _data[rowIndex].CopyTo(newData, 0);

            return new Vector(newData);
        }

        /// <summary>
        /// Copies a column vector to a specified column of this matrix.
        /// </summary>
        public
        void
        SetColumnVector(
            IVector<double> columnVector,
            int columnIndex)
        {
            if(null == columnVector)
            {
                throw new ArgumentNullException("columnVector");
            }

            if(columnIndex < 0 || columnIndex >= _columnCount)
            {
                throw new ArgumentOutOfRangeException("columnIndex");
            }

            if(columnVector.Length != _rowCount)
            {
                throw new ArgumentOutOfRangeException("columnVector");
            }

            for(int i = 0; i < _rowCount; i++)
            {
                _data[i][columnIndex] = columnVector[i];
            }

            ResetOnDemandComputations();
        }

        /// <summary>
        /// Copies a row vector to a specified row of this matrix.
        /// </summary>
        public
        void
        SetRowVector(
            IVector<double> rowVector,
            int rowIndex)
        {
            if(null == rowVector)
            {
                throw new ArgumentNullException("rowVector");
            }

            if(rowIndex < 0 || rowIndex >= _rowCount)
            {
                throw new ArgumentOutOfRangeException("rowIndex");
            }

            if(rowVector.Length != _columnCount)
            {
                throw new ArgumentOutOfRangeException("rowVector");
            }

            _data[rowIndex] = rowVector.CopyToArray();

            ResetOnDemandComputations();
        }

        /// <summary>
        /// Gets a sub matrix.
        /// </summary>
        /// <param name="i0">First row index.</param>
        /// <param name="i1">Last row index (inclusive).</param>
        /// <param name="j0">First column index.</param>
        /// <param name="j1">Last column index (inclusive).</param>
        /// <returns>A(i0:i1,j0:j1)</returns>
        /// <exception cref="System.IndexOutOfRangeException">Sub matrix indices</exception>
        public virtual
        Matrix
        GetMatrix(
            int i0,
            int i1,
            int j0,
            int j1)
        {
            double[][] newData = CreateMatrixData(i1 - i0 + 1, j1 - j0 + 1);
            try
            {
                for(int i = i0; i <= i1; i++)
                {
                    for(int j = j0; j <= j1; j++)
                    {
                        newData[i - i0][j - j0] = _data[i][j];
                    }
                }
            }
            catch(IndexOutOfRangeException e)
            {
                throw new IndexOutOfRangeException(Properties.LocalStrings.ArgumentMatrixIndexOutOfRange, e);
            }

            return new Matrix(newData);
        }

        /// <summary>
        /// Gets a sub matrix.
        /// </summary>
        /// <param name="r">Array of row indices.</param>
        /// <param name="c">Array of column indices.</param>
        /// <returns>A(r(:),c(:))</returns>
        /// <exception cref="System.IndexOutOfRangeException">Sub matrix indices.</exception>
        public virtual
        Matrix
        GetMatrix(
            int[] r,
            int[] c)
        {
            double[][] newData = CreateMatrixData(r.Length, c.Length);
            try
            {
                for(int i = 0; i < r.Length; i++)
                {
                    for(int j = 0; j < c.Length; j++)
                    {
                        newData[i][j] = _data[r[i]][c[j]];
                    }
                }
            }
            catch(IndexOutOfRangeException e)
            {
                throw new IndexOutOfRangeException(Properties.LocalStrings.ArgumentMatrixIndexOutOfRange, e);
            }

            return new Matrix(newData);
        }

        /// <summary>
        /// Get a sub matrix.
        /// </summary>
        /// <param name="i0">First row index.</param>
        /// <param name="i1">Last row index (inclusive).</param>
        /// <param name="c">Array of column indices.</param>
        /// <returns>A(i0:i1,c(:))</returns>
        /// <exception cref="System.IndexOutOfRangeException">Sub matrix indices.</exception>
        public virtual
        Matrix
        GetMatrix(
            int i0,
            int i1,
            int[] c)
        {
            double[][] newData = CreateMatrixData(i1 - i0 + 1, c.Length);
            try
            {
                for(int i = i0; i <= i1; i++)
                {
                    for(int j = 0; j < c.Length; j++)
                    {
                        newData[i - i0][j] = _data[i][c[j]];
                    }
                }
            }
            catch(IndexOutOfRangeException e)
            {
                throw new IndexOutOfRangeException(Properties.LocalStrings.ArgumentMatrixIndexOutOfRange, e);
            }

            return new Matrix(newData);
        }

        /// <summary>
        /// Get a sub matrix.
        /// </summary>
        /// <param name="r">Array of row indices.</param>
        /// <param name="j0">First column index.</param>
        /// <param name="j1">Last column index (inclusive).</param>
        /// <returns>A(r(:),j0:j1)</returns>
        /// <exception cref="System.IndexOutOfRangeException">Sub matrix indices.</exception>
        public virtual
        Matrix
        GetMatrix(
            int[] r,
            int j0,
            int j1)
        {
            double[][] newData = CreateMatrixData(r.Length, j1 - j0 + 1);
            try
            {
                for(int i = 0; i < r.Length; i++)
                {
                    for(int j = j0; j <= j1; j++)
                    {
                        newData[i][j - j0] = _data[r[i]][j];
                    }
                }
            }
            catch(IndexOutOfRangeException e)
            {
                throw new IndexOutOfRangeException(Properties.LocalStrings.ArgumentMatrixIndexOutOfRange, e);
            }

            return new Matrix(newData);
        }

        /// <summary>
        /// Set a sub matrix.
        /// </summary>
        /// <param name="i0">First row index.</param>
        /// <param name="i1">Last row index (inclusive).</param>
        /// <param name="j0">First column index.</param>
        /// <param name="j1">Last column index (inclusive).</param>
        /// <param name="x">A(i0:i1,j0:j1)</param>
        /// <exception cref="System.IndexOutOfRangeException">Sub matrix indices.</exception>
        public virtual
        void
        SetMatrix(
            int i0,
            int i1,
            int j0,
            int j1,
            IMatrix<double> x)
        {
            try
            {
                for(int i = i0; i <= i1; i++)
                {
                    for(int j = j0; j <= j1; j++)
                    {
                        _data[i][j] = x[i - i0, j - j0];
                    }
                }
            }
            catch(IndexOutOfRangeException e)
            {
                throw new IndexOutOfRangeException(Properties.LocalStrings.ArgumentMatrixIndexOutOfRange, e);
            }

            ResetOnDemandComputations();
        }

        /// <summary>
        /// Sets a sub matrix.
        /// </summary>
        /// <param name="r">Array of row indices.</param>
        /// <param name="c">Array of column indices.</param>
        /// <param name="x">A(r(:),c(:))</param>
        /// <exception cref="System.IndexOutOfRangeException">Sub matrix indices</exception>
        public virtual
        void
        SetMatrix(
            int[] r,
            int[] c,
            IMatrix<double> x)
        {
            try
            {
                for(int i = 0; i < r.Length; i++)
                {
                    for(int j = 0; j < c.Length; j++)
                    {
                        _data[r[i]][c[j]] = x[i, j];
                    }
                }
            }
            catch(IndexOutOfRangeException e)
            {
                throw new IndexOutOfRangeException(Properties.LocalStrings.ArgumentMatrixIndexOutOfRange, e);
            }

            ResetOnDemandComputations();
        }

        /// <summary>
        /// Sets a sub matrix.
        /// </summary>
        /// <param name="r">Array of row indices.</param>
        /// <param name="j0">First column index.</param>
        /// <param name="j1">Last column index (inclusive).</param>
        /// <param name="x">A(r(:),j0:j1)</param>
        /// <exception cref="System.IndexOutOfRangeException">Sub matrix indices</exception>
        public virtual
        void
        SetMatrix(
            int[] r,
            int j0,
            int j1,
            IMatrix<double> x)
        {
            try
            {
                for(int i = 0; i < r.Length; i++)
                {
                    for(int j = j0; j <= j1; j++)
                    {
                        _data[r[i]][j] = x[i, j - j0];
                    }
                }
            }
            catch(IndexOutOfRangeException e)
            {
                throw new IndexOutOfRangeException(Properties.LocalStrings.ArgumentMatrixIndexOutOfRange, e);
            }

            ResetOnDemandComputations();
        }

        /// <summary>
        /// Set a sub matrix.
        /// </summary>
        /// <param name="i0">First row index.</param>
        /// <param name="i1">Last row index (inclusive).</param>
        /// <param name="c">Array of column indices.</param>
        /// <param name="x">A(i0:i1,c(:))</param>
        /// <exception cref="System.IndexOutOfRangeException">Sub matrix indices.</exception>
        public virtual
        void
        SetMatrix(
            int i0,
            int i1,
            int[] c,
            IMatrix<double> x)
        {
            try
            {
                for(int i = i0; i <= i1; i++)
                {
                    for(int j = 0; j < c.Length; j++)
                    {
                        _data[i][c[j]] = x[i - i0, j];
                    }
                }
            }
            catch(IndexOutOfRangeException e)
            {
                throw new IndexOutOfRangeException(Properties.LocalStrings.ArgumentMatrixIndexOutOfRange, e);
            }

            ResetOnDemandComputations();
        }

        #endregion

        #region Norms computations

        /// <summary>One norm</summary>
        /// <returns>Maximum column sum.</returns>
        public
        double
        Norm1()
        {
            // TODO (ruegg, 2008-03-11): Change to property, consider cached on-demand.
            double f = 0;
            for(int j = 0; j < _columnCount; j++)
            {
                double s = 0;
                for(int i = 0; i < _rowCount; i++)
                {
                    s += Math.Abs(_data[i][j]);
                }

                f = Math.Max(f, s);
            }

            return f;
        }

        /// <summary>Two norm</summary>
        /// <returns>Maximum singular value.</returns>
        public
        double
        Norm2()
        {
            // TODO (ruegg, 2008-03-11): Change to property.
            return SingularValueDecomposition.Norm2();
        }

        /// <summary>Infinity norm</summary>
        /// <returns>Maximum row sum.</returns>
        public
        double
        NormInf()
        {
            // TODO (ruegg, 2008-03-11): Change to property, consider cached on-demand.
            double f = 0;
            for(int i = 0; i < _rowCount; i++)
            {
                double s = 0;
                for(int j = 0; j < _columnCount; j++)
                {
                    s += Math.Abs(_data[i][j]);
                }

                f = Math.Max(f, s);
            }

            return f;
        }

        /// <summary>Frobenius norm</summary>
        /// <returns>Sqrt of sum of squares of all elements.</returns>
        public
        double
        NormF()
        {
            // TODO (ruegg, 2008-03-11): Change to property
            double f = 0;
            for(int i = 0; i < _rowCount; i++)
            {
                for(int j = 0; j < _columnCount; j++)
                {
                    f = Fn.Hypot(f, _data[i][j]);
                }
            }

            return f;
        }

        #endregion

        #region Elementary linear operations

        // TODO: Adapt to new method model (X + XInplace). Will be a breaking change.
        // TODO: Extend with additional methods, see ComplexMatrix.

        /// <summary>
        /// In place addition of <c>m</c> to this <c>Matrix</c>.
        /// </summary>
        /// <seealso cref="operator + (Matrix, Matrix)"/>
        [Obsolete("Use 'AddInplace' instead. 'Add' will soon be replaced with an outplace version")]
        public virtual
        void
        Add(IMatrix<double> m)
        {
            AddInplace(m);
        }

        /// <summary>
        /// In place addition of <c>m</c> to this <c>Matrix</c>.
        /// </summary>
        /// <seealso cref="operator + (Matrix, Matrix)"/>
        /// <remarks>
        /// This method changes this matrix.
        /// </remarks>
        public
        void
        AddInplace(IMatrix<double> m)
        {
            CheckMatchingMatrixDimensions(this, m);

            for(int i = 0; i < _rowCount; i++)
            {
                double[] thisRow = _data[i];
                for(int j = 0; j < thisRow.Length; j++)
                {
                    thisRow[j] += m[i, j];
                }
            }

            ResetOnDemandComputations();
        }

        /// <summary>
        /// In place subtraction of <c>m</c> to this <c>Matrix</c>.
        /// </summary>
        /// <seealso cref="operator - (Matrix, Matrix)"/>
        [Obsolete("Use 'SubtractInplace' instead. 'Subtract' will soon be replaced with an outplace version")]
        public virtual
        void
        Subtract(IMatrix<double> m)
        {
            SubtractInplace(m);
        }

        /// <summary>
        /// In place subtraction of <c>m</c> to this <c>Matrix</c>.
        /// </summary>
        /// <seealso cref="operator - (Matrix, Matrix)"/>
        /// <remarks>
        /// This method changes this matrix.
        /// </remarks>
        public
        void
        SubtractInplace(IMatrix<double> m)
        {
            CheckMatchingMatrixDimensions(this, m);

            for(int i = 0; i < _rowCount; i++)
            {
                double[] thisRow = _data[i];
                for(int j = 0; j < thisRow.Length; j++)
                {
                    thisRow[j] -= m[i, j];
                }
            }

            ResetOnDemandComputations();
        }

        /// <summary>
        /// Negate this real matrix.
        /// </summary>
        public
        Matrix
        Negate()
        {
            double[][] newData = new double[_rowCount][];
            for(int i = 0; i < newData.Length; i++)
            {
                double[] thisRow = _data[i];
                double[] newRow = new double[_columnCount];
                for(int j = 0; j < newRow.Length; j++)
                {
                    newRow[j] = -thisRow[j];
                }

                newData[i] = newRow;
            }

            return new Matrix(newData);
        }

        /// <summary>
        /// In place unary minus of the <c>Matrix</c>.
        /// </summary>
        [Obsolete("Use 'NegateInplace' instead. 'UnaryMinus' will soon be removed.")]
        public virtual
        void
        UnaryMinus()
        {
            NegateInplace();
        }

        /// <summary>
        /// In place unary minus of the <c>Matrix</c>.
        /// </summary>
        /// <remarks>
        /// This method changes this matrix.
        /// </remarks>
        public
        void
        NegateInplace()
        {
            for(int i = 0; i < _rowCount; i++)
            {
                double[] thisRow = _data[i];
                for(int j = 0; j < thisRow.Length; j++)
                {
                    thisRow[j] = -thisRow[j];
                }
            }

            ResetOnDemandComputations();
        }

        /// <summary>
        /// Multiplies in place this <c>Matrix</c> by a scalar.
        /// </summary>
        /// <seealso cref="operator * (double, Matrix)"/>
        [Obsolete("Use 'MultiplyInplace' instead. 'Multiply' will soon be replaced with an outplace version")]
        public virtual
        void
        Multiply(double s)
        {
            MultiplyInplace(s);
        }

        /// <summary>
        /// Multiplies in place this <c>Matrix</c> by a scalar.
        /// </summary>
        /// <seealso cref="operator * (double, Matrix)"/>
        /// <remarks>
        /// This method changes this matrix.
        /// </remarks>
        public
        void
        MultiplyInplace(double s)
        {
            for(int i = 0; i < _rowCount; i++)
            {
                for(int j = 0; j < _columnCount; j++)
                {
                    _data[i][j] *= s;
                }
            }

            ResetOnDemandComputations();
        }

        /// <summary>
        /// Linear algebraic matrix multiplication, A * B
        /// </summary>
        /// <exception cref="ArgumentNullException">B must not be null.</exception>
        /// <exception cref="ArgumentException">Matrix inner dimensions must agree.</exception>
        public
        Matrix
        Multiply(Matrix b)
        {
            if(b == null)
            {
                throw new ArgumentNullException("B", Properties.LocalStrings.ArgumentNull("b"));
            }

            if(b.RowCount != _columnCount)
            {
                throw new ArgumentException(Properties.LocalStrings.ArgumentMatrixSameDimensions);
            }

            double[][] newData = CreateMatrixData(_rowCount, b.ColumnCount);
            double[][] bData = b._data;

            for(int j = 0; j < b.ColumnCount; j++)
            {
                // caching the column for performance
                double[] columnB = new double[_columnCount];
                for(int k = 0; k < _columnCount; k++)
                {
                    columnB[k] = bData[k][j];
                }

                // making the line-to-column product
                for(int i = 0; i < _rowCount; i++)
                {
                    double s = 0;
                    double[] datai = _data[i];
                    for(int k = 0; k < datai.Length; k++)
                    {
                        s += datai[k] * columnB[k];
                    }

                    newData[i][j] = s;
                }
            }

            return new Matrix(newData);
        }

        /// <summary>
        /// Inplace real square matrix multiplication.
        /// </summary>
        /// <param name="b">The other square real matrix.</param>
        /// <remarks>
        /// This method changes this matrix. Only square matrices are supported.
        /// </remarks>
        /// <seealso cref="Multiply(Matrix)"/>
        /// <seealso cref="operator * (Matrix, Matrix)"/>
        /// <exception cref="ArgumentNullException">B must not be null.</exception>
        /// <exception cref="ArgumentException">Matrix inner dimensions must agree.</exception>
        public
        void
        MultiplyInplace(IMatrix<double> b)
        {
            if(null == b)
            {
                throw new ArgumentNullException("B");
            }

            if(_rowCount != _columnCount || b.RowCount != b.ColumnCount)
            {
                throw new ArgumentException(Properties.LocalStrings.ArgumentMatrixSquare);
            }

            if(_rowCount != b.RowCount)
            {
                throw new ArgumentException(Properties.LocalStrings.ArgumentMatrixSameDimensions);
            }

            double[] tempRow = new double[_columnCount];
            for(int i = 0; i < _data.Length; i++)
            {
                double[] thisRow = _data[i];
                for(int j = 0; j < tempRow.Length; j++)
                {
                    double s = 0;
                    for(int k = 0; k < thisRow.Length; k++)
                    {
                        s += thisRow[k] * b[k, j];
                    }

                    tempRow[j] = s;
                }

                _data[i] = tempRow;
                tempRow = thisRow;
            }

            ResetOnDemandComputations();
        }

        /// <summary>
        /// Multiply this matrix with a right real column vector.
        /// </summary>
        /// <param name="b">The right real column vector.</param>
        /// <returns>
        /// Vector ret[i] = sum(this[i,k] * b[k])
        /// </returns>
        /// <remarks>
        /// This method has the same effect as the overloaded * operator.
        /// </remarks>
        /// <seealso cref="operator * (Matrix, Vector)"/>
        /// <exception cref="ArgumentNullException">B must not be null.</exception>
        /// <exception cref="ArgumentException">Matrix inner dimensions must agree.</exception>
        public
        Vector
        MultiplyRightColumn(IVector<double> b)
        {
            if(null == b)
            {
                throw new ArgumentNullException("B");
            }

            if(b.Length != _columnCount)
            {
                throw new ArgumentException(Properties.LocalStrings.ArgumentMatrixSameDimensions);
            }

            double[] newData = new double[_rowCount];
            for(int i = 0; i < newData.Length; i++)
            {
                double[] thisRow = _data[i];
                double s = 0;
                for(int j = 0; j < thisRow.Length; j++)
                {
                    s += thisRow[j] * b[j];
                }

                newData[i] = s;
            }

            return new Vector(newData);
        }

        /// <summary>
        /// In place linear algebraic matrix multiplication, D * A where
        /// D is the diagonal matrix.
        /// </summary>
        /// <param name="diagonal">Diagonal values of D.</param>
        /// <exception cref="ArgumentNullException"><c>diagonal</c> must not be null.</exception>
        /// <exception cref="ArgumentException">Matrix inner dimensions must agree.</exception>
        [Obsolete("Use 'MultiplyLeftDiagonalInplace' instead.")]
        public virtual
        void
        Multiply(double[] diagonal)
        {
            MultiplyLeftDiagonalInplace(new Vector(diagonal));
        }

        /// <summary>
        /// Multiply a diagonal real matrix with this matrix. This has the same effect
        /// as scaling the rows of this matrix by the scalar elements of the diagonal.
        /// </summary>
        /// <param name="diagonal">The left diagonal real matrix.</param>
        /// <returns>
        /// Matrix ret[i,j] = this[i,j] * diagonal[i]
        /// </returns>
        /// <remarks>
        /// This method has the same effect as the overloaded * operator.
        /// </remarks>
        public
        Matrix
        MultiplyLeftDiagonal(IVector<double> diagonal)
        {
            if(null == diagonal)
            {
                throw new ArgumentNullException("diagonal");
            }

            if(_rowCount != diagonal.Length)
            {
                throw new ArgumentException(Properties.LocalStrings.ArgumentMatrixSameDimensions);
            }

            double[][] newData = new double[_rowCount][];
            for(int i = 0; i < newData.Length; i++)
            {
                double s = diagonal[i];
                double[] thisRow = _data[i];
                double[] newRow = new double[_columnCount];
                for(int j = 0; j < newRow.Length; j++)
                {
                    newRow[j] = thisRow[j] * s;
                }

                newData[i] = newRow;
            }

            return new Matrix(newData);
        }

        /// <summary>
        /// Inplace multiply a real diagonal matrix with this matrix. This has the same effect
        /// as scaling the rows of this matrix by the scalar elements of the diagonal.
        /// </summary>
        /// <param name="diagonal">The left diagonal real matrix.</param>
        /// <exception cref="ArgumentNullException"><c>diagonal</c> must not be null.</exception>
        /// <exception cref="ArgumentException">Matrix inner dimensions must agree.</exception>
        /// <remarks>
        /// This method changes this matrix.
        /// </remarks>
        public
        void
        MultiplyLeftDiagonalInplace(IVector<double> diagonal)
        {
            if(null == diagonal)
            {
                throw new ArgumentNullException("diagonal", Properties.LocalStrings.ArgumentNull("diagonal"));
            }

            if(_rowCount != diagonal.Length)
            {
                throw new ArgumentException(Properties.LocalStrings.ArgumentMatrixSameDimensions);
            }

            for(int i = 0; i < _rowCount; i++)
            {
                double s = diagonal[i];
                double[] thisRow = _data[i];
                for(int j = 0; j < thisRow.Length; j++)
                {
                    thisRow[j] *= s;
                }
            }

            ResetOnDemandComputations();
        }

        /// <summary>
        /// Multiply this matrix with a real diagonal matrix. This has the same effect
        /// as scaling the columns of this matrix by the scalar elements of the diagonal.
        /// </summary>
        /// <param name="diagonal">The right diagonal real matrix.</param>
        /// <returns>
        /// Matrix ret[i,j] = this[i,j] * diagonal[j]
        /// </returns>
        /// <remarks>
        /// This method has the same effect as the overloaded * operator.
        /// </remarks>
        public
        Matrix
        MultiplyRightDiagonal(IVector<double> diagonal)
        {
            if(null == diagonal)
            {
                throw new ArgumentNullException("diagonal");
            }

            if(_columnCount != diagonal.Length)
            {
                throw new ArgumentException(Properties.LocalStrings.ArgumentMatrixSameDimensions);
            }

            double[][] newData = new double[_rowCount][];
            for(int i = 0; i < newData.Length; i++)
            {
                double[] thisRow = _data[i];
                double[] newRow = new double[_columnCount];
                for(int j = 0; j < newRow.Length; j++)
                {
                    newRow[j] = thisRow[j] * diagonal[j];
                }

                newData[i] = newRow;
            }

            return new Matrix(newData);
        }

        /// <summary>
        /// Inplace Multiply this matrix with a real diagonal matrix. This has the same effect
        /// as scaling the columns of this matrix by the scalar elements of the diagonal.
        /// </summary>
        /// <param name="diagonal">The right diagonal real matrix.</param>
        /// <remarks>
        /// This method changes this matrix.
        /// </remarks>
        public
        void
        MultiplyRightDiagonalInplace(IVector<double> diagonal)
        {
            if(null == diagonal)
            {
                throw new ArgumentNullException("diagonal");
            }

            if(_columnCount != diagonal.Length)
            {
                throw new ArgumentException(Properties.LocalStrings.ArgumentMatrixSameDimensions);
            }

            for(int i = 0; i < _data.Length; i++)
            {
                double[] thisRow = _data[i];
                for(int j = 0; j < thisRow.Length; j++)
                {
                    thisRow[j] *= diagonal[j];
                }
            }

            ResetOnDemandComputations();
        }

        /// <summary>
        /// Adds in place the scaled Matrix <c>s*m</c> to this Matrix.
        /// </summary>
        /// <remarks>
        /// This method changes this matrix.
        /// </remarks>
        public
        void
        MultiplyAccumulateInplace(IMatrix<double> m, double s)
        {
            CheckMatchingMatrixDimensions(this, m);

            for(int i = 0; i < _rowCount; i++)
            {
                double[] thisRow = _data[i];
                for(int j = 0; j < thisRow.Length; j++)
                {
                    thisRow[j] += s * m[i, j];
                }
            }

            ResetOnDemandComputations();
        }

        #endregion

        #region Additional elementary operations

        /// <summary>
        /// In place transposition of this <c>Matrix</c>.
        /// </summary>
        /// <seealso cref="Transpose(IMatrix{double})"/>
        /// <remarks>
        /// In case of non-quadratic matrices, this operation replaces the
        /// internal data structure. Hence, if you hold a reference to it
        /// for faster access, you'll need to get a new reference to it
        /// using <see cref="GetArray"/>.
        /// </remarks>
        [Obsolete("Use 'TransposeInplace' instead. 'Transpose' will soon be replaced with an outplace version")]
        public virtual
        void
        Transpose()
        {
            TransposeInplace();
        }

        /// <summary>
        /// In place transposition of this <c>Matrix</c>.
        /// </summary>
        /// <seealso cref="Transpose(IMatrix{double})"/>
        /// <remarks>
        /// In case of non-quadratic matrices, this operation replaces the
        /// internal data structure. Hence, if you hold a reference to it
        /// for faster access, you'll need to get a new reference to it
        /// using <see cref="GetArray"/>.
        /// </remarks>
        public
        void
        TransposeInplace()
        {
            // TODO: test this method
            int m = _rowCount;
            int n = _columnCount;

            if(m == n)
            {
                // No need for array copy
                for(int i = 0; i < m; i++)
                {
                    for(int j = i + 1; j < n; j++)
                    {
                        double thisIJ = this[i, j];
                        _data[i][j] = _data[j][i];
                        _data[j][i] = thisIJ;
                    }
                }
            }
            else
            {
                double[][] newData = CreateMatrixData(n, m);
                for(int i = 0; i < m; i++)
                {
                    for(int j = 0; j < n; j++)
                    {
                        newData[j][i] = _data[i][j];
                    }
                }

                _data = newData;
                _rowCount = n;
                _columnCount = m;
            }

            ResetOnDemandComputations();
        }

        /// <summary>
        /// Gets the transposition of the provided <c>Matrix</c>.
        /// </summary>
        public static
        Matrix
        Transpose(IMatrix<double> m)
        {
            double[][] newData = CreateMatrixData(m.ColumnCount, m.RowCount);
            for(int i = 0; i < m.RowCount; i++)
            {
                for(int j = 0; j < m.ColumnCount; j++)
                {
                    newData[j][i] = m[i, j];
                }
            }

            return new Matrix(newData);
        }

        /// <summary>
        /// Tensor Product (Kronecker) of this and another matrix.
        /// </summary>
        /// <param name="b">The matrix to operate on.</param>
        /// <returns>
        /// Kronecker Product of this and the given matrix.
        /// </returns>
        public
        Matrix
        TensorMultiply(Matrix b)
        {
            return KroneckerProduct(this, b);
        }

        /// <summary>
        /// Kronecker Product of two matrices.
        /// </summary>
        public static
        Matrix
        KroneckerProduct(
            Matrix a,
            Matrix b)
        {
            // Matrix to be created
            Matrix outMat = new Matrix(a.RowCount * b.RowCount, a.ColumnCount * b.ColumnCount);
            double[][] aData = a._data;

            for(int i = 0; i < a.RowCount; i++)
            {
                int rowOffset = i * b.RowCount;
                for(int j = 0; j < a.ColumnCount; j++)
                {
                    int colOffset = j * b.ColumnCount;
                    Matrix partMat = aData[i][j] * b;

                    outMat.SetMatrix(
                        rowOffset,
                        rowOffset + b.RowCount - 1,
                        colOffset,
                        colOffset + b.RowCount - 1,
                        partMat);
                }
            }

            return outMat;
        }

        #endregion

        #region Array operation on matrices

        /// <summary>
        /// In place element-by-element multiplication, <c>A .*= M</c>.
        /// </summary>
        /// <remarks>
        /// This instance and <c>m</c> must have the same dimensions.
        /// </remarks>
        /// <seealso cref="ArrayMultiply(IMatrix{double}, IMatrix{double})"/>
        [Obsolete("Use 'ArrayMultiplyInplace' instead. 'ArrayMultiply' will soon be replaced with an outplace version")]
        public
        void
        ArrayMultiply(IMatrix<double> m)
        {
            ArrayMultiplyInplace(m);
        }

        /// <summary>
        /// In place element-by-element multiplication, <c>A .*= M</c>.
        /// </summary>
        /// <remarks>
        /// This instance and <c>m</c> must have the same dimensions.
        /// </remarks>
        /// <seealso cref="ArrayMultiply(IMatrix{double}, IMatrix{double})"/>
        public
        void
        ArrayMultiplyInplace(IMatrix<double> m)
        {
            CheckMatchingMatrixDimensions(this, m);

            for(int i = 0; i < _rowCount; i++)
            {
                for(int j = 0; j < _columnCount; j++)
                {
                    _data[i][j] *= m[i, j];
                }
            }

            ResetOnDemandComputations();
        }

        /// <summary>
        /// Element-by-element multiplication, <c>result = M1 .* M2</c>.
        /// </summary>
        /// <remarks>
        /// <c>m1</c> and <c>m2</c> must have the same dimensions.
        /// </remarks>
        /// <seealso cref="ArrayMultiply(IMatrix{double})"/>
        public static
        Matrix
        ArrayMultiply(
            IMatrix<double> m1,
            IMatrix<double> m2)
        {
            CheckMatchingMatrixDimensions(m1, m2);

            double[][] newData = CreateMatrixData(m1.RowCount, m1.ColumnCount);
            for(int i = 0; i < m1.RowCount; i++)
            {
                for(int j = 0; j < m1.ColumnCount; j++)
                {
                    newData[i][j] = m1[i, j] * m2[i, j];
                }
            }

            return new Matrix(newData);
        }

        /// <summary>
        /// In place element-by-element right division, <c>A ./= M</c>.
        /// </summary>
        /// <remarks>
        /// This instance and <c>m</c> must have the same dimensions.
        /// </remarks>
        /// <seealso cref="ArrayDivide(IMatrix{double}, IMatrix{double})"/>
        [Obsolete("Use 'ArrayDivideInplace' instead. 'ArrayDivide' will soon be replaced with an outplace version")]
        public
        void
        ArrayDivide(IMatrix<double> m)
        {
            ArrayDivideInplace(m);
        }

        /// <summary>
        /// In place element-by-element right division, <c>A ./= M</c>.
        /// </summary>
        /// <remarks>
        /// This instance and <c>m</c> must have the same dimensions.
        /// </remarks>
        /// <seealso cref="ArrayDivide(IMatrix{double}, IMatrix{double})"/>
        public
        void
        ArrayDivideInplace(IMatrix<double> m)
        {
            CheckMatchingMatrixDimensions(this, m);

            for(int i = 0; i < _rowCount; i++)
            {
                for(int j = 0; j < _columnCount; j++)
                {
                    _data[i][j] /= m[i, j];
                }
            }

            ResetOnDemandComputations();
        }

        /// <summary>
        /// Element-by-element right division, <c>result = M1 ./ M2</c>.
        /// </summary>
        /// <remarks>
        /// <c>m1</c> and <c>m2</c> must have the same dimensions.
        /// </remarks>
        /// <seealso cref="ArrayDivide(IMatrix{double})"/>
        public static
        Matrix
        ArrayDivide(
            IMatrix<double> m1,
            IMatrix<double> m2)
        {
            CheckMatchingMatrixDimensions(m1, m2);

            double[][] newData = CreateMatrixData(m1.RowCount, m1.ColumnCount);
            for(int i = 0; i < m1.RowCount; i++)
            {
                for(int j = 0; j < m1.ColumnCount; j++)
                {
                    newData[i][j] = m1[i, j] / m2[i, j];
                }
            }

            return new Matrix(newData);
        }

        /// <summary>
        /// In place element-by-element raise to power, <c>A[i,j] = A[i,j]^exponent</c>.
        /// </summary>
        /// <seealso cref="ArrayPower(IMatrix{double}, double)"/>
        [Obsolete("Use 'ArrayPowerInplace' instead. 'ArrayPower' will soon be replaced with an outplace version")]
        public
        void
        ArrayPower(double exponent)
        {
            ArrayPowerInplace(exponent);
        }

        /// <summary>
        /// In place element-by-element raise to power, <c>A[i,j] = A[i,j]^exponent</c>.
        /// </summary>
        /// <seealso cref="ArrayPower(IMatrix{double}, double)"/>
        public
        void
        ArrayPowerInplace(double exponent)
        {
            for(int i = 0; i < _rowCount; i++)
            {
                for(int j = 0; j < _columnCount; j++)
                {
                    _data[i][j] = Math.Pow(_data[i][j], exponent);
                }
            }

            ResetOnDemandComputations();
        }

        /// <summary>
        /// Element-by-element raise to power, <c>result[i,j] = M[i,j]^exponent</c>.
        /// </summary>
        /// <seealso cref="ArrayPower(double)"/>
        public static
        Matrix
        ArrayPower(
            IMatrix<double> m,
            double exponent)
        {
            double[][] newData = CreateMatrixData(m.RowCount, m.ColumnCount);
            for(int i = 0; i < m.RowCount; i++)
            {
                for(int j = 0; j < m.ColumnCount; j++)
                {
                    newData[i][j] = Math.Pow(m[i, j], exponent);
                }
            }

            return new Matrix(newData);
        }

        /// <summary>
        /// In place element-by-element mapping of an arbitrary function, <c>A[i,j] = mapping(A[i,j])</c>.
        /// </summary>
        /// <seealso cref="ArrayMap(IMatrix{double}, Converter{double, double})"/>
        [Obsolete("Use 'ArrayMapInplace' instead. 'ArrayMap' will soon be replaced with an outplace version")]
        public
        void
        ArrayMap(Converter<double, double> mapping)
        {
            ArrayMapInplace(mapping);
        }

        /// <summary>
        /// In place element-by-element mapping of an arbitrary function, <c>A[i,j] = mapping(A[i,j])</c>.
        /// </summary>
        /// <seealso cref="ArrayMap(IMatrix{double}, Converter{double, double})"/>
        public
        void
        ArrayMapInplace(Converter<double, double> mapping)
        {
            for(int i = 0; i < _rowCount; i++)
            {
                for(int j = 0; j < _columnCount; j++)
                {
                    _data[i][j] = mapping(_data[i][j]);
                }
            }

            ResetOnDemandComputations();
        }

        /// <summary>
        /// Element-by-element mapping of an arbitrary function, <c>result[i,j] = mapping(M[i,j])</c>.
        /// </summary>
        /// <seealso cref="ArrayMap(Converter{double, double})"/>
        public static
        Matrix
        ArrayMap(
            IMatrix<double> m,
            Converter<double, double> mapping)
        {
            double[][] newData = CreateMatrixData(m.RowCount, m.ColumnCount);
            for(int i = 0; i < m.RowCount; i++)
            {
                for(int j = 0; j < m.ColumnCount; j++)
                {
                    newData[i][j] = mapping(m[i, j]);
                }
            }

            return new Matrix(newData);
        }

        #endregion

        #region Decompositions

        /// <summary>
        /// LU Decomposition
        /// </summary>
        /// <seealso cref="LUDecomposition"/>
        public LUDecomposition LUDecomposition
        {
            get { return _luDecompositionOnDemand.Compute(); }
        }

        /// <summary>
        /// QR Decomposition
        /// </summary>
        /// <seealso cref="QRDecomposition"/>
        public QRDecomposition QRDecomposition
        {
            get { return _qrDecompositionOnDemand.Compute(); }
        }

        /// <summary>
        /// Cholesky Decomposition
        /// </summary>
        /// <seealso cref="CholeskyDecomposition"/>
        public CholeskyDecomposition CholeskyDecomposition
        {
            get { return _choleskyDecompositionOnDemand.Compute(); }
        }

        /// <summary>
        /// Singular Value Decomposition
        /// </summary>
        /// <seealso cref="SingularValueDecomposition"/>
        public SingularValueDecomposition SingularValueDecomposition
        {
            get { return _singularValueDecompositionOnDemand.Compute(); }
        }

        /// <summary>
        /// Eigenvalue Decomposition
        /// </summary>
        /// <seealso cref="EigenvalueDecomposition"/>
        public EigenvalueDecomposition EigenvalueDecomposition
        {
            get { return _eigenValueDecompositionOnDemand.Compute(); }
        }

        /// <summary>
        /// LU Decomposition
        /// </summary>
        /// <seealso cref="LUDecomposition"/>
        [Obsolete("Use the LUDecomposition property instead.")]
        public virtual
        LUDecomposition
        LUD()
        {
            return _luDecompositionOnDemand.Compute();
        }

        /// <summary>
        /// QR Decomposition
        /// </summary>
        /// <returns><see cref="QRDecomposition"/></returns>
        /// <seealso cref="QRDecomposition"/>
        [Obsolete("Use the QRDecomposition property instead.")]
        public virtual
        QRDecomposition
        QRD()
        {
            return _qrDecompositionOnDemand.Compute();
        }

        /// <summary>
        /// Cholesky Decomposition.
        /// This method will be removed in future versions, please use the <see cref="CholeskyDecomposition"/> property instead.
        /// </summary>
        /// <seealso cref="CholeskyDecomposition"/>
        [Obsolete("Use the CholeskyDecomposition property instead.")]
        public virtual
        CholeskyDecomposition
        chol()
        {
            return _choleskyDecompositionOnDemand.Compute();
        }

        /// <summary>
        /// Singular Value Decomposition
        /// </summary>
        /// <seealso cref="SingularValueDecomposition"/>
        [Obsolete("Use the SingularValueDecomposition property instead.")]
        public virtual
        SingularValueDecomposition
        SVD()
        {
            return _singularValueDecompositionOnDemand.Compute();
        }

        /// <summary>
        /// Eigenvalue Decomposition
        /// </summary>
        /// <seealso cref="EigenvalueDecomposition"/>
        [Obsolete("Use the EigenvalueDecomposition property instead.")]
        public virtual
        EigenvalueDecomposition
        Eigen()
        {
            return _eigenValueDecompositionOnDemand.Compute();
        }

        #endregion

        #region Linear Algebra

        /// <summary>
        /// Solve A*X = B against a Least Square (L2) criterion.
        /// </summary>
        /// <param name="b">right hand side</param>
        /// <returns>solution if A is square, least squares solution otherwise.</returns>
        /// <exception cref="InvalidOperationException">Matrix rank is deficient.</exception>
        public virtual
        Matrix
        Solve(Matrix b)
        {
            // square case:
            if(_rowCount == _columnCount)
            {
                return LUDecomposition.Solve(b);
            }

            // m > n:
            if(_rowCount > _columnCount)
            {
                return QRDecomposition.Solve(b);
            }

            /*
            m < n:
            Here we'd actually need an LQ decomposition instead of QR.
            Unfortunately we don't support that yet.
            */

            throw new NotSupportedException(Properties.LocalStrings.SpecialCasePlannedButNotImplementedYet);
        }

        /// <summary>
        /// Solve A*x = b against a Least Square (L2) criterion, where x and b are column vectors.
        /// </summary>
        /// <param name="b">Right hand side column vector.</param>
        /// <returns>column vector: solution if A is square, least squares solution otherwise.</returns>
        /// <exception cref="InvalidOperationException">Matrix rank is deficient.</exception>
        /// <remarks>
        /// Note that double arrays can implicitly cast to <see cref="Vector"/>,
        /// so you can also provide a double[] as parameter.
        /// </remarks>
        public
        Vector
        Solve(Vector b)
        {
            // Redirect to matrix version (until LU and QR directly support vectors).
            Matrix mb = CreateFromColumn(b);
            Matrix mx = Solve(mb);
            Vector x = mx.GetColumnVector(0);

            return x;
        }

        /// <summary>Solve A*X = B against a Least Absolute Deviation (L1) criterion.</summary>
        /// <param name="b">right hand side</param>
        /// <returns>The implementation relies on the IRLS (iterated Re-weighted Least Square) algorithm.</returns>
        /// <exception cref="InvalidOperationException">Matrix rank is deficient.</exception>
        public virtual
        Matrix
        SolveRobust(Matrix b)
        {
            if(_rowCount == _columnCount)
            {
                return LUDecomposition.Solve(b);
            }

            const double Eta = 1.0e-12;
            const double Epsilon = 1.0e-6;
            const int MaxIteration = 100;

            Matrix a = this;
            Matrix tA = Matrix.Transpose(this);
            Matrix x = null;

            // G is a diagonal matrix - G is initialized as the identity matrix
            double[] g = new double[a.RowCount];
            for(int i = 0; i < g.Length; i++)
            {
                g[i] = 1;
            }

            Vector gv = new Vector(g);

            // IRLS loop
            double maxChange = double.MaxValue;
            for(int k = 0; k < MaxIteration && maxChange > Epsilon; k++)
            {
                Matrix ga = MultiplyLeftDiagonal(gv);
                Matrix gb = b.MultiplyLeftDiagonal(gv);

                Matrix ak = tA.Multiply(ga);
                Matrix bk = tA.Multiply(gb);

                Matrix xk = ak.Solve(bk);
                if(x != null)
                {
                    maxChange = double.MinValue;
                    for(int i = 0; i < x.RowCount; i++)
                    {
                        maxChange = Math.Max(maxChange, Math.Abs(x[i, 0] - xk[i, 0]));
                    }
                }

                x = xk;

                Matrix rk = a.Multiply(xk);
                rk.NegateInplace();
                rk.AddInplace(b);

                // updating the weighting matrix
                for(int i = 0; i < b.RowCount; i++)
                {
                    double r = Math.Abs(rk[i, 0]);
                    if(r < Eta)
                    {
                        r = Eta;
                    }

                    g[i] = 1.0 / r;
                }
            }

            return x;
        }

        /// <summary>Solve X*A = B, which is also A'*X' = B'</summary>
        /// <param name="b">right hand side</param>
        /// <returns>solution if A is square, least squares solution otherwise.</returns>
        public virtual
        Matrix
        SolveTranspose(Matrix b)
        {
            return Transpose(this).Solve(Transpose(b));
        }

        /// <summary>Matrix inverse or pseudo inverse.</summary>
        /// <returns> inverse(A) if A is square, pseudo inverse otherwise.</returns>
        public virtual
        Matrix
        Inverse()
        {
            // m >= n:
            if(_rowCount >= _columnCount)
            {
                return Solve(Identity(_rowCount, _rowCount));
            }

            /*
            m < n:
            Here we'd actually need an LQ decomposition instead of QR.
            Unfortunately we don't support that yet.
            Lukily there is a transpose identity for the inverse:
            */

            Matrix at = Transpose(this);
            Matrix bt = Identity(_columnCount, _columnCount);
            Matrix rt = at.QRDecomposition.Solve(bt);
            return Transpose(rt);
        }

        /// <summary>Matrix determinant</summary>
        public virtual
        double
        Determinant()
        {
            // TODO (ruegg, 2008-03-11): Change to property
            return LUDecomposition.Determinant();
        }

        /// <summary>Matrix rank</summary>
        /// <returns>effective numerical rank, obtained from SVD.</returns>
        public virtual
        int
        Rank()
        {
            // TODO (ruegg, 2008-03-11): Change to property
            return SingularValueDecomposition.Rank();
        }

        /// <summary>Matrix condition (2 norm)</summary>
        /// <returns>ratio of largest to smallest singular value.</returns>
        public virtual
        double
        Condition()
        {
            // TODO (ruegg, 2008-03-11): Change to property
            return SingularValueDecomposition.Condition();
        }

        /// <summary>Matrix trace.</summary>
        /// <returns>sum of the diagonal elements.</returns>
        public virtual
        double
        Trace()
        {
            // TODO (ruegg, 2008-03-11): Change to property
            return _traceOnDemand.Compute();
        }

        /// <summary>
        /// Gets the complex eigen values of this matrix.
        /// </summary>
        /// <remarks>
        /// The eigenvalue decomposition is cached internally..
        /// </remarks>
        public ComplexVector EigenValues
        {
            get { return EigenvalueDecomposition.EigenValues; }
        }

        /// <summary>
        /// Gets the complex eigen vectors of this matrix.
        /// </summary>
        /// <remarks>
        /// The eigenvalue decomposition is cached internally.
        /// </remarks>
        public Matrix EigenVectors
        {
            get { return EigenvalueDecomposition.EigenVectors; }
        }

        #endregion

        #region Arithmetic Operator Overloading

        /// <summary>Addition of matrices</summary>
        public static
        Matrix
        operator +(
            Matrix m1,
            Matrix m2)
        {
            CheckMatchingMatrixDimensions(m1, m2);

            double[][] newData = CreateMatrixData(m1.RowCount, m1.ColumnCount);
            for(int i = 0; i < m1.RowCount; i++)
            {
                for(int j = 0; j < m1.ColumnCount; j++)
                {
                    newData[i][j] = m1[i, j] + m2[i, j];
                }
            }

            return new Matrix(newData);
        }

        /// <summary>Subtraction of matrices</summary>
        public static
        Matrix
        operator -(
            Matrix m1,
            Matrix m2)
        {
            CheckMatchingMatrixDimensions(m1, m2);

            double[][] newData = CreateMatrixData(m1.RowCount, m1.ColumnCount);
            for(int i = 0; i < m1.RowCount; i++)
            {
                for(int j = 0; j < m1.ColumnCount; j++)
                {
                    newData[i][j] = m1[i, j] - m2[i, j];
                }
            }

            return new Matrix(newData);
        }

        /// <summary>Negation of a matrix</summary>
        public static
        Matrix
        operator -(Matrix m1)
        {
            return m1.Negate();
        }

        /// <summary>Linear algebraic matrix multiplication.</summary>
        /// <exception cref="System.ArgumentException">Matrix inner dimensions must agree.</exception>
        public static
        Matrix
        operator *(
            Matrix m1,
            Matrix m2)
        {
            if(m1 == null)
            {
                throw new ArgumentNullException("m1", Properties.LocalStrings.ArgumentNull("m1"));
            }

            return m1.Multiply(m2);
        }

        /// <summary>Multiplication of a matrix by a scalar, C = s*A</summary>
        public static
        Matrix
        operator *(
            double s,
            Matrix m)
        {
            double[][] newData = CreateMatrixData(m.RowCount, m.ColumnCount);
            for(int i = 0; i < m.RowCount; i++)
            {
                for(int j = 0; j < m.ColumnCount; j++)
                {
                    newData[i][j] = s * m[i, j];
                }
            }

            return new Matrix(newData);
        }

        /// <summary>Multiplication of a matrix by a scalar, C = s*A</summary>
        public static
        Matrix
        operator *(
            Matrix m,
            double s)
        {
            return s * m;
        }

        /// <summary>
        /// Multiply a real matrix with a real column vector.
        /// </summary>
        /// <exception cref="System.ArgumentException">Matrix inner dimensions must agree.</exception>
        public static
        Vector
        operator *(
            Matrix m1,
            Vector v2)
        {
            return m1.MultiplyRightColumn(v2);
        }

        #endregion   //Operator Overloading

        #region Various Helpers & Infrastructure

        /// <summary>Check if size(u) == size(v) *</summary>
        private static
        void
        CheckMatchingMatrixDimensions(
            IMatrix<double> u,
            IMatrix<double> v)
        {
            if(u.RowCount != v.RowCount || u.ColumnCount != v.ColumnCount)
            {
                throw new ArgumentException(Properties.LocalStrings.ArgumentMatrixSameDimensions);
            }
        }

        /// <summary>Returns a deep copy of this instance.</summary>
        public
        Matrix
        Clone()
        {
            return new Matrix(CloneMatrixData(_data));
        }

        /// <summary>
        /// Creates an exact copy of this matrix.
        /// </summary>
        object
        ICloneable.Clone()
        {
            return Clone();
        }

        /// <summary>
        /// Indicates whether <c>obj</c> is equal to this instance.
        /// </summary>
        public override
        bool
        Equals(object obj)
        {
            Matrix m = obj as Matrix;
            return ReferenceEquals(m, null) ? false : Equals(m);
        }

        /// <summary>
        /// Indicates whether <c>other</c> is equal to this matrix.
        /// </summary>
        public
        bool
        Equals(Matrix other)
        {
            if(ReferenceEquals(other, null)
                || _rowCount != other.RowCount
                || _columnCount != other.ColumnCount)
            {
                return false;
            }

            // compare all values
            double[][] otherData = other._data;
            for(int i = 0; i < _data.Length; i++)
            {
                double[] x = _data[i];
                double[] y = otherData[i];
                for(int j = 0; j < x.Length; j++)
                {
                    if(x[j] != y[j])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Serves as a hash function for this type
        /// </summary>
        public override
        int
        GetHashCode()
        {
            unchecked
            {
                int result = _rowCount;
                result = (result * 397) ^ _columnCount;
                result = (result * 397) ^ _data.GetHashCode();
                return result;
            }
        }

        /// <summary>
        /// Returns true if two matrices are almost equal, up to the default maximum relative error.
        /// </summary>
        public
        bool
        AlmostEquals(Matrix other)
        {
            if(ReferenceEquals(other, null)
                || _rowCount != other.RowCount
                || _columnCount != other.ColumnCount)
            {
                return false;
            }

            return Number.AlmostEqualNorm(Norm1(), other.Norm1(), (this - other).Norm1());
        }

        /// <summary>
        /// Returns true if two matrices are almost equal, up to the provided maximum relative error.
        /// </summary>
        public
        bool
        AlmostEquals(
            Matrix other,
            double maximumRelativeError)
        {
            if(ReferenceEquals(other, null)
                || _rowCount != other.RowCount
                || _columnCount != other.ColumnCount)
            {
                return false;
            }

            return Number.AlmostEqualNorm(Norm1(), other.Norm1(), (this - other).Norm1(), maximumRelativeError);
        }

        /// <summary>
        /// Returns true if two matrices are almost equal, up to the provided maximum relative error.
        /// </summary>
        public static
        bool
        AlmostEqual(
            Matrix u,
            Matrix v,
            double maximumRelativeError)
        {
            return Number.AlmostEqual(u, v, maximumRelativeError);
        }

        /// <summary>
        /// Returns true if two matrices are almost equal, up to the default maximum relative error.
        /// </summary>
        public static
        bool
        AlmostEqual(
            Matrix u,
            Matrix v)
        {
            return Number.AlmostEqual(u, v);
        }

        /// <summary>
        /// Formats this matrix to a human-readable string
        /// </summary>
        public override
        string
        ToString()
        {
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < _rowCount; i++)
            {
                if(i == 0)
                {
                    sb.Append("[[");
                }
                else
                {
                    sb.Append(" [");
                }

                for(int j = 0; j < _columnCount; j++)
                {
                    if(j != 0)
                    {
                        sb.Append(',');
                    }

                    sb.Append(_data[i][j]);
                }

                if(i == _rowCount - 1)
                {
                    sb.Append("]]");
                }
                else
                {
                    sb.AppendLine("]");
                }
            }

            return sb.ToString();
        }

        #endregion

        #region OnDemandComputations
        void
        InitOnDemandComputations()
        {
            _luDecompositionOnDemand = new OnDemandComputation<LUDecomposition>(ComputeLUDecomposition);
            _qrDecompositionOnDemand = new OnDemandComputation<QRDecomposition>(ComputeQRDecomposition);
            _choleskyDecompositionOnDemand = new OnDemandComputation<CholeskyDecomposition>(ComputeCholeskyDecomposition);
            _singularValueDecompositionOnDemand = new OnDemandComputation<SingularValueDecomposition>(ComputeSingularValueDecomposition);
            _eigenValueDecompositionOnDemand = new OnDemandComputation<EigenvalueDecomposition>(ComputeEigenValueDecomposition);
            _traceOnDemand = new OnDemandComputation<double>(ComputeTrace);
        }

        void
        ResetOnDemandComputations()
        {
            _luDecompositionOnDemand.Reset();
            _qrDecompositionOnDemand.Reset();
            _choleskyDecompositionOnDemand.Reset();
            _singularValueDecompositionOnDemand.Reset();
            _eigenValueDecompositionOnDemand.Reset();
            _traceOnDemand.Reset();
        }

        /// <summary>
        /// Reset various internal computations.
        /// Call this method after you made changes directly
        /// on the the internal double[][] data structure.
        /// </summary>
        public
        void
        ResetComputations()
        {
            ResetOnDemandComputations();
        }

        LUDecomposition
        ComputeLUDecomposition()
        {
            return new LUDecomposition(this);
        }

        QRDecomposition
        ComputeQRDecomposition()
        {
            return new QRDecomposition(this);
        }

        CholeskyDecomposition
        ComputeCholeskyDecomposition()
        {
            return new CholeskyDecomposition(this);
        }

        SingularValueDecomposition
        ComputeSingularValueDecomposition()
        {
            return new SingularValueDecomposition(this);
        }

        EigenvalueDecomposition
        ComputeEigenValueDecomposition()
        {
            return new EigenvalueDecomposition(this);
        }

        double
        ComputeTrace()
        {
            double t = 0;
            for(int i = 0; i < Math.Min(_rowCount, _columnCount); i++)
            {
                t += _data[i][i];
            }

            return t;
        }
        #endregion
    }
}
