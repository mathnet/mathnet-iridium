//-----------------------------------------------------------------------
// <copyright file="Histogram.cs" company="Math.NET Project">
//    Copyright (c) 2004-2009, Joannes Vermorel.
//    All Right Reserved.
// </copyright>
// <author>
//    Joannes Vermorel, http://www.vermorel.com
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MathNet.Numerics.Statistics
{
    /// <summary>
    /// Base class for the <i>histogram</i> algorithms.
    /// </summary>
    [Serializable]
    public class Histogram
    {
        /// <summary>
        /// Contains all the <c>Bucket</c>s of the <c>Histogram</c>.
        /// </summary>
        readonly List<Bucket> _buckets;

        /// <summary>
        /// Indicates whether the elements of <c>buckets</c> are
        /// currently sorted.
        /// </summary>
        bool _areBucketsSorted;

        /// <summary>
        /// Initializes a new instance of the Histogram class.
        /// </summary>
        public Histogram()
        {
            _buckets = new List<Bucket>();
            _areBucketsSorted = true;
        }

        /// <summary>
        /// Adds a <c>Bucket</c> to the <c>Histogram</c>.
        /// </summary>
        public void Add(Bucket bucket)
        {
            _buckets.Add(bucket);
            _areBucketsSorted = false;
        }

        /// <summary>
        /// Returns the <c>Bucket</c> that contains the <c>value</c>. 
        /// </summary>
        public Bucket GetContainerOf(double value)
        {
            LazySort();
            int index = Searching.BinaryMapSearch(_buckets, value);

            if(index < 0)
            {
                throw new ArgumentException(Properties.LocalStrings.ArgumentHistogramContainsNot(value));
            }

            return _buckets[index];
        }

        /// <summary>
        /// Returns the index in the <c>Histogram</c> of the <c>Bucket</c>
        /// that contains the <c>value</c>.
        /// </summary>
        public int GetContainerIndexOf(double value)
        {
            LazySort();
            return Searching.BinaryMapSearch(_buckets, value);
        }

        /// <summary>
        /// Joins the boundaries of the successive buckets.
        /// </summary>
        public void JoinBuckets()
        {
            if(_buckets.Count == 0)
            {
                throw new ArgumentException(Properties.LocalStrings.InvalidOperationHistogramEmpty);
            }

            LazySort();
            for(int i = 0; i < _buckets.Count - 2; i++)
            {
                Bucket u = _buckets[i];
                Bucket v = _buckets[i + 1];
                u.UpperBound = v.LowerBound = (u.UpperBound + v.LowerBound) / 2;
            }
        }

        private void LazySort()
        {
            if(!_areBucketsSorted)
            {
                _buckets.Sort();
                _areBucketsSorted = true;
            }
        }

        /// <summary>
        /// Sort the buckets.
        /// </summary>
        public void Sort()
        {
            _buckets.Sort();
        }

        /// <summary>
        /// Gets the <c>Bucket</c> indexed by <c>index</c>.
        /// </summary>
        public Bucket this[int index]
        {
            get
            {
                LazySort();
                return _buckets[index];
            }

            set
            {
                LazySort();
                _buckets[index] = value;
            }
        }

        /// <summary>
        /// Gets the number of buckets.
        /// </summary>
        public int Count
        {
            get { return _buckets.Count; }
        }

        /// <summary>
        /// Gets the sum of the bucket depths.
        /// </summary>
        public double TotalDepth
        {
            get
            {
                double totalDepth = 0;
                foreach(Bucket b in _buckets)
                {
                    totalDepth += b.Depth;
                }

                return totalDepth;
            }
        }

        /// <summary>
        /// Prints the buckets contained in the <see cref="Histogram"/>.
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach(Bucket b in _buckets)
            {
                sb.Append(b.ToString());
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns the optimal dispersion histogram.
        /// </summary>
        public static Histogram OptimalDispersion(int bucketCount, ICollection<double> distribution)
        {
            if(distribution.Count < Math.Max(bucketCount, 2))
            {
                throw new ArgumentException(Properties.LocalStrings.InvalidOperationHistogramNotEnoughPoints);
            }

            // "values" contains the sorted distribution.
            double[] values = new double[distribution.Count];
            distribution.CopyTo(values, 0);
            Array.Sort(values);

            // 'optimalCost[i,k]' contains the optimal costs for an
            // histogram with the 'i+1' first values and 'k' buckets.
            double[,] optimalCost = new double[values.Length, bucketCount];

            // 'lastBucketIndex[i,k]' contains the index of the first
            // value of the last bucket for optimal histogram comprising
            // the 'i+1' first values and 'k' buckets.
            int[,] lastBucketIndex = new int[values.Length, bucketCount];

            // 'prefixSum[i]' contains the sum of the 'i-1' first values.
            double[] prefixSum = new double[values.Length + 1];

            // Initialization of the prefix sums
            for(int i = 0; i < values.Length; i++)
            {
                prefixSum[i + 1] = prefixSum[i] + values[i];
            }

            // "One bucket" histograms initialization
            for(int i = 0, avg = 0; i < values.Length; i++)
            {
                while((avg + 1) < values.Length &&
                    values[avg + 1] < prefixSum[i + 1] / (i + 1))
                {
                    avg++;
                }

                optimalCost[i, 0] =
                    prefixSum[i + 1]
                    - (2 * prefixSum[avg + 1])
                    + (((2 * avg) - i + 1) * (prefixSum[i + 1] / (i + 1)));
            }

            // "One value per bucket" histograms initialization
            for(int k = 0; k < bucketCount; k++)
            {
                // optimalCost[k, k] = 0;
                lastBucketIndex[k, k] = k;
            }

            // ----- Dynamic programming part -----

            // Loop on the number of buckets 
            // (note that there are 'k+1' buckets)
            for(int k = 1; k < bucketCount; k++)
            {
                // Loop on the number of considered values
                // (note that there are 'i+1' considered values)
                for(int i = k; i < values.Length; i++)
                {
                    optimalCost[i, k] = double.PositiveInfinity;

                    // Loop for finding the optimal boundary of the last bucket
                    // ('j+1' is the index of the first value in the last bucket)
                    for(int j = (k - 1), avg = (k - 1); j < i; j++)
                    {
                        while((avg + 1) < values.Length &&
                            values[avg + 1] < (prefixSum[i + 1] - prefixSum[j + 1]) / (i - j))
                        {
                            avg++;
                        }

                        double currentCost =
                            optimalCost[j, k - 1]
                            + prefixSum[i + 1]
                            + prefixSum[j + 1]
                            - (2 * prefixSum[avg + 1])
                            + (((2 * avg) - i - j) * (prefixSum[i + 1] - prefixSum[j + 1]) / (i - j));

                        if(currentCost < optimalCost[i, k])
                        {
                            optimalCost[i, k] = currentCost;
                            lastBucketIndex[i, k] = j + 1;
                        }
                    }
                }
            }

            // ----- Reconstitution of the histogram -----
            Histogram histogram = new Histogram();
            int index = values.Length - 1;
            for(int k = (bucketCount - 1); k >= 0; k--)
            {
                histogram.Add(new Bucket(
                    values[lastBucketIndex[index, k]],
                    values[index],
                    index - lastBucketIndex[index, k] + 1));

                index = lastBucketIndex[index, k] - 1;
            }

            return histogram;
        }

        /// <summary>
        /// Returns the optimal variance histogram.
        /// </summary>
        /// <param name="bucketCount">The number of buckets in the histogram.</param>
        /// <param name="distribution"><c>double</c> elements expected.</param>
        /// <remarks>Requires a computations time quadratic to 
        /// <c>distribution.Length</c>.</remarks>
        public static Histogram OptimalVariance(int bucketCount, ICollection<double> distribution)
        {
            if(distribution.Count < bucketCount)
            {
                throw new ArgumentException(Properties.LocalStrings.InvalidOperationHistogramNotEnoughPoints);
            }

            // "values" contains the sorted distribution.
            double[] values = new double[distribution.Count];
            distribution.CopyTo(values, 0);
            Array.Sort(values);

            // 'optimalCost[i,k]' contains the optimal costs for an
            // histogram with the 'i+1' first values and 'k' buckets.
            double[,] optimalCost = new double[values.Length, bucketCount];

            // 'lastBucketIndex[i,k]' contains the index of the first
            // value of the last bucket for optimal histogram comprising
            // the 'i+1' first values and 'k' buckets.
            int[,] lastBucketIndex = new int[values.Length, bucketCount];

            // 'prefixSum[i]' contains the sum of the 'i-1' first values.
            double[] prefixSum = new double[values.Length + 1];

            // 'sqPrefixSum' contains the sum of the 'i-1' first squared values.
            double[] sqPrefixSum = new double[values.Length + 1];

            // Initialization of the prefix sums
            for(int i = 0; i < values.Length; i++)
            {
                prefixSum[i + 1] = prefixSum[i] + values[i];
                sqPrefixSum[i + 1] = sqPrefixSum[i] + (values[i] * values[i]);
            }

            // "One bucket" histograms initialization
            for(int i = 0; i < values.Length; i++)
            {
                optimalCost[i, 0] =
                    sqPrefixSum[i + 1]
                    - (prefixSum[i + 1] * prefixSum[i + 1] / (i + 1));
            }

            // "One value per bucket" histograms initialization
            for(int k = 0; k < bucketCount; k++)
            {
                // optimalCost[k, k] = 0;
                lastBucketIndex[k, k] = k;
            }

            // ----- Dynamic programming part -----

            // Loop on the number of buckets 
            // (note that there are 'k+1' buckets)
            for(int k = 1; k < bucketCount; k++)
            {
                // Loop on the number of considered values
                // (note that there are 'i+1' considered values)
                for(int i = k; i < values.Length; i++)
                {
                    optimalCost[i, k] = double.PositiveInfinity;

                    // Loop for finding the optimal boundary of the last bucket
                    // ('j+1' is the index of the first value in the last bucket)
                    for(int j = (k - 1); j < i; j++)
                    {
                        double currentCost =
                            optimalCost[j, k - 1]
                            + sqPrefixSum[i + 1]
                            - sqPrefixSum[j + 1]
                            - ((prefixSum[i + 1] - prefixSum[j + 1]) * (prefixSum[i + 1] - prefixSum[j + 1]) / (i - j));

                        if(currentCost < optimalCost[i, k])
                        {
                            optimalCost[i, k] = currentCost;
                            lastBucketIndex[i, k] = j + 1;
                        }
                    }
                }
            }

            // ----- Reconstitution of the histogram -----
            Histogram histogram = new Histogram();
            int index = values.Length - 1;
            for(int k = (bucketCount - 1); k >= 0; k--)
            {
                histogram.Add(new Bucket(
                    values[lastBucketIndex[index, k]],
                    values[index],
                    index - lastBucketIndex[index, k] + 1));

                index = lastBucketIndex[index, k] - 1;
            }

            return histogram;
        }

        /// <summary>
        /// Returns the optimal freedom histogram.
        /// </summary>
        public static Histogram OptimalFreedom(int bucketCount, ICollection<double> distribution)
        {
            if(distribution.Count < Math.Max(bucketCount, 2))
            {
                throw new ArgumentException(Properties.LocalStrings.InvalidOperationHistogramNotEnoughPoints);
            }

            // "values" contains the sorted distribution.
            double[] values = new double[distribution.Count];
            distribution.CopyTo(values, 0);
            Array.Sort(values);

            // 'optimalCost[i,k]' contains the optimal costs for an
            // histogram with the 'i+1' first values and 'k' buckets.
            double[,] optimalCost = new double[values.Length, bucketCount];

            // 'lastBucketIndex[i,k]' contains the index of the first
            // value of the last bucket for optimal histogram comprising
            // the 'i+1' first values and 'k' buckets.
            int[,] lastBucketIndex = new int[values.Length, bucketCount];

            // "One bucket" histograms initialization
            for(int i = 0; i < values.Length; i++)
            {
                optimalCost[i, 0] = (values[i] - values[0]) * (i + 1);
            }

            // "One value per bucket" histograms initialization
            for(int k = 0; k < bucketCount; k++)
            {
                // optimalCost[k, k] = 0;
                lastBucketIndex[k, k] = k;
            }

            // ----- Dynamic programming part -----

            // Loop on the number of buckets 
            // (note that there are 'k+1' buckets)
            for(int k = 1; k < bucketCount; k++)
            {
                // Loop on the number of considered values
                // (note that there are 'i+1' considered values)
                for(int i = k; i < values.Length; i++)
                {
                    optimalCost[i, k] = double.PositiveInfinity;

                    // Loop for finding the optimal boundary of the last bucket
                    // ('j+1' is the index of the first value in the last bucket)
                    for(int j = (k - 1); j < i; j++)
                    {
                        double currentCost =
                            optimalCost[j, k - 1]
                            + ((values[i] - values[j + 1]) * (i - j));

                        if(currentCost < optimalCost[i, k])
                        {
                            optimalCost[i, k] = currentCost;
                            lastBucketIndex[i, k] = j + 1;
                        }
                    }
                }
            }

            // ----- Reconstitution of the histogram -----
            Histogram histogram = new Histogram();
            int index = values.Length - 1;
            for(int k = (bucketCount - 1); k >= 0; k--)
            {
                histogram.Add(new Bucket(
                    values[lastBucketIndex[index, k]],
                    values[index],
                    index - lastBucketIndex[index, k] + 1));

                index = lastBucketIndex[index, k] - 1;
            }

            return histogram;
        }

        /// <summary>
        /// Returns the optimal squared freedom histogram.
        /// </summary>
        public static Histogram OptimalSquaredFreedom(int histSize, ICollection<double> distribution)
        {
            if(distribution.Count < Math.Max(histSize, 2))
            {
                throw new ArgumentException(Properties.LocalStrings.InvalidOperationHistogramNotEnoughPoints);
            }

            // "values" contains the sorted distribution.
            double[] values = new double[distribution.Count];
            distribution.CopyTo(values, 0);
            Array.Sort(values);

            // 'optimalCost[i,k]' contains the optimal costs for an
            // histogram with the 'i+1' first values and 'k' buckets.
            double[,] optimalCost = new double[values.Length, histSize];

            // 'lastBucketIndex[i,k]' contains the index of the first
            // value of the last bucket for optimal histogram comprising
            // the 'i+1' first values and 'k' buckets.
            int[,] lastBucketIndex = new int[values.Length, histSize];

            // "One bucket" histograms initialization
            for(int i = 0; i < values.Length; i++)
            {
                optimalCost[i, 0] =
                    (values[i] - values[0]) * (values[i] - values[0]) * (i + 1);
            }

            // "One value per bucket" histograms initialization
            for(int k = 0; k < histSize; k++)
            {
                // optimalCost[k, k] = 0;
                lastBucketIndex[k, k] = k;
            }

            // ----- Dynamic programming part -----

            // Loop on the number of buckets 
            // (note that there are 'k+1' buckets)
            for(int k = 1; k < histSize; k++)
            {
                // Loop on the number of considered values
                // (note that there are 'i+1' considered values)
                for(int i = k; i < values.Length; i++)
                {
                    optimalCost[i, k] = double.PositiveInfinity;

                    // Loop for finding the optimal boundary of the last bucket
                    // ('j+1' is the index of the first value in the last bucket)
                    for(int j = (k - 1); j < i; j++)
                    {
                        double currentCost =
                            optimalCost[j, k - 1]
                            + ((values[i] - values[j + 1]) * (values[i] - values[j + 1]) * (i - j));

                        if(currentCost < optimalCost[i, k])
                        {
                            optimalCost[i, k] = currentCost;
                            lastBucketIndex[i, k] = j + 1;
                        }
                    }
                }
            }

            // ----- Reconstitution of the histogram -----
            Histogram histogram = new Histogram();
            int index = values.Length - 1;
            for(int k = (histSize - 1); k >= 0; k--)
            {
                histogram.Add(new Bucket(
                    values[lastBucketIndex[index, k]],
                    values[index],
                    index - lastBucketIndex[index, k] + 1));

                index = lastBucketIndex[index, k] - 1;
            }

            return histogram;
        }
    }

    /// <summary>
    /// A <see cref="Histogram"/> consists of a series of <see cref="Bucket"/>s, 
    /// each representing a region limited by an upper and a lower bound.
    /// </summary>
    [Serializable]
    public class Bucket : IComparable<double>, IComparable<Bucket>, ICloneable
    {
        /// <summary>Lower boundary of the <c>Bucket</c>.</summary>
        double _lowerBound;

        /// <summary>Upper boundary of the <c>Bucket</c>.</summary>
        double _upperBound;

        /// <summary>Number of points inside the <c>Bucket</c>.</summary>
        double _depth;

        /// <summary>
        /// Initializes a new instance of the Bucket class.
        /// </summary>
        public Bucket(double lowerBound, double upperBound)
        {
            Debug.Assert(
                lowerBound <= upperBound,
                "lowerBound should be smaller than the upperBound.");

            _lowerBound = lowerBound;
            _upperBound = upperBound;
        }

        /// <summary>
        /// Initializes a new instance of the Bucket class.
        /// </summary>
        public Bucket(double lowerBound, double upperBound, double depth)
        {
            _lowerBound = lowerBound;
            _upperBound = upperBound;
            _depth = depth;
        }

        /// <summary>
        /// Initializes a new instance of the Bucket class
        /// by deep copy from an existing bucket.
        /// </summary>
        private Bucket(Bucket bucket)
        {
            _lowerBound = bucket._lowerBound;
            _upperBound = bucket._upperBound;
            _depth = bucket._depth;
        }

        /// <summary>
        /// Lower Bound of the Bucket.
        /// </summary>
        public double LowerBound
        {
            get { return _lowerBound; }
            set { _lowerBound = value; }
        }

        /// <summary>
        /// Upper Bound of the Bucket.
        /// </summary>
        public double UpperBound
        {
            get { return _upperBound; }
            set { _upperBound = value; }
        }

        /// <summary>
        /// Width of the Bucket.
        /// </summary>
        public double Width
        {
            get { return _upperBound - _lowerBound; }
        }

        /// <summary>
        /// Depth of the Bucket.
        /// </summary>
        public double Depth
        {
            get { return _depth; }
            set { _depth = value; }
        }

        /// <summary>
        /// Comparison of two buckets.
        /// </summary>
        public int CompareTo(Bucket bucket)
        {
            if(_lowerBound >= bucket._lowerBound)
            {
                if(_upperBound <= bucket._upperBound)
                {
                    return 0;
                }

                return 1;
            }

            if(_upperBound >= bucket._upperBound)
            {
                return 0;
            }

            return -1;
        }

        ///<summary>
        /// Comparison of a bucket with a value, matching values to buckets.
        ///</summary>
        public int CompareTo(double value)
        {
            if(value > _upperBound)
            {
                return -1;
            }

            if(value < _lowerBound)
            {
                return 1;
            }

            return 0;
        }

        /// <summary>
        /// Creates a deep copy of this bucket.
        /// </summary>
        public object Clone()
        {
            return new Bucket(this);
        }

        /// <summary>
        /// Checks whether two Buckets are equal.
        /// </summary>
        public override bool Equals(object obj)
        {
            if(!(obj is Bucket))
            {
                return false;
            }

            Bucket b = (Bucket)obj;
            return Number.AlmostEqual(_lowerBound, b._lowerBound)
                && Number.AlmostEqual(_upperBound, b._upperBound)
                && Number.AlmostEqual(_depth, b._depth);
        }

        /// <summary>
        /// Provides a hash code for this bucket.
        /// </summary>
        public override int GetHashCode()
        {
            return _lowerBound.GetHashCode()
                ^ _upperBound.GetHashCode()
                ^ _depth.GetHashCode();
        }

        /// <summary>
        /// Formats a human-readable string for this bucket.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "[" + _lowerBound + ";" + _upperBound + "]";
        }
    }
}
