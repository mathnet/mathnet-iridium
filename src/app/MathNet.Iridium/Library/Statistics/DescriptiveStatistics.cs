//-----------------------------------------------------------------------
// <copyright file="DescriptiveStatistics.cs" company="Math.NET Project">
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
using System.Collections.Generic;
using MathNet.Numerics.NumberTheory;

namespace MathNet.Numerics.Statistics
{
    /// <summary>
    /// Descriptive Statistics and Order Statistics for Parameter Estimation
    /// </summary>
    public static class DescriptiveStatistics
    {
        /// <summary>
        /// Estimates the population mean.
        /// </summary>
        public static
        double
        Mean(IEnumerable<double> samples)
        {
            /* TODO: Evaluate whether an alternative approach with
             * absolute sorting might have advantages */

            long count = 0;
            double mean = 0;

            foreach(double sample in samples)
            {
                count++;
                mean += ((sample - mean) / count);
            }

            return mean;
        }

        /// <summary>
        /// Estimates the unbiased population variance.
        /// </summary>
        public static
        double
        Variance(IEnumerable<double> samples)
        {
            /* TODO: Evaluate whether an alternative approach with
             * absolute sorting might have advantages */

            IEnumerator<double> iterator = samples.GetEnumerator();
            if(!iterator.MoveNext())
            {
                return 0d;
            }

            long count = 1;
            double mean = iterator.Current;
            double varianceSum = 0;

            while(iterator.MoveNext())
            {
                count++;
                double difference = iterator.Current - mean;
                mean += difference / count;
                varianceSum += difference * (iterator.Current - mean);
            }

            return varianceSum / (count - 1);
        }

        /// <summary>
        /// Estimates the biased sample variance, considering the provided samples as the whole population.
        /// </summary>
        public static
        double
        SampleVariance(IEnumerable<double> samples)
        {
             /* TODO: Evaluate whether an alternative approach with
              * absolute sorting might have advantages */

            IEnumerator<double> iterator = samples.GetEnumerator();
            if(!iterator.MoveNext())
            {
                return 0d;
            }

            long count = 1;
            double mean = iterator.Current;
            double varianceSum = 0;

            while(iterator.MoveNext())
            {
                count++;
                double difference = iterator.Current - mean;
                mean += difference / count;
                varianceSum += difference * (iterator.Current - mean);
            }

            return varianceSum / count;
        }

        /// <summary>
        /// Estimates the unbiased population standard deviation (sigma).
        /// </summary>
        public static
        double
        StandardDeviation(IEnumerable<double> samples)
        {
            return Math.Sqrt(Variance(samples));
        }

        /// <summary>
        /// Order Statistics: Evaluates the minimum value of the provided samples.
        /// </summary>
        public static
        double
        Min(IEnumerable<double> samples)
        {
            IEnumerator<double> iterator = samples.GetEnumerator();
            if(!iterator.MoveNext())
            {
                return double.PositiveInfinity;
            }

            double min = iterator.Current;

            while(iterator.MoveNext())
            {
                if(min > iterator.Current)
                {
                    min = iterator.Current;
                }
            }

            return min;
        }

        /// <summary>
        /// Order Statistics: Evaluates the maximum value of the provided samples.
        /// </summary>
        public static
        double
        Max(IEnumerable<double> samples)
        {
            IEnumerator<double> iterator = samples.GetEnumerator();
            if(!iterator.MoveNext())
            {
                return double.NegativeInfinity;
            }

            double max = iterator.Current;

            while(iterator.MoveNext())
            {
                if(max < iterator.Current)
                {
                    max = iterator.Current;
                }
            }

            return max;
        }

        /// <summary>
        /// Order Statistics: Evaluates the lower median value of the provided samples.
        /// </summary>
        public static
        double
        Median(IEnumerable<double> samples)
        {
            List<double> list = new List<double>(samples);
            int lowerMidpoint = list.Count / 2;
            return OrderSelect(list, 0, list.Count - 1, lowerMidpoint + 1);
        }

        /// <summary>
        /// Order Statistics: Evaluates the upper median value of the provided samples.
        /// </summary>
        public static
        double
        UpperMedian(IEnumerable<double> samples)
        {
            List<double> list = new List<double>(samples);
            int lowerMidpoint = IntegerTheory.IsEven(list.Count) ? ((list.Count / 2) + 1) : (list.Count / 2);
            return OrderSelect(list, 0, list.Count - 1, lowerMidpoint + 1);
        }

        /// <summary>
        /// Order Statistics: Evaluate the i-order (1..N) statistic of the provided samples.
        /// </summary>
        public static
        double
        OrderStatistic(IEnumerable<double> samples, int order)
        {
            if(order == 1)
            {
                // Can be done in linear time by Min()
                return Min(samples);
            }

            List<double> list = new List<double>(samples);
            if(order < 1 || order > list.Count)
            {
                throw new ArgumentOutOfRangeException("order", Properties.LocalStrings.ArgumentInIntervalXYInclusive(1, list.Count));
            }

            if(order == list.Count)
            {
                // Can be done in linear time by Max()
                return Max(list);
            }

            return OrderSelect(list, 0, list.Count - 1, order);
        }

        static
        double
        OrderSelect(
            IList<double> samples,
            int left,
            int right,
            int order)
        {
            if(left == right)
            {
                return samples[left];
            }

            // Pivoting
            int a = left;
            int b = right;
            int p = a + ((b - a) >> 1); // midpoint

            if(samples[a] > samples[p])
            {
                Sorting.Swap(samples, a, p);
            }

            if(samples[a] > samples[b])
            {
                Sorting.Swap(samples, a, b);
            }

            if(samples[p] > samples[b])
            {
                Sorting.Swap(samples, p, b);
            }

            double pivot = samples[p];

            // Hoare Partitioning
            do
            {
                while(samples[a] < pivot)
                {
                    a++;
                }

                while(samples[b] > pivot)
                {
                    b--;
                }

                if(a > b)
                {
                    break;
                }

                if(a < b)
                {
                    Sorting.Swap(samples, a, b);
                }
            }
            while(a < b);

            int k = b - left + 1;
            if(order == k)
            {
                return samples[p];
            }

            // partial quicksort: only sort the interesting partition further
            if(order < k)
            {
                return OrderSelect(samples, left, b - 1, order);
            }

            return OrderSelect(samples, b + 1, right, order - k);
        }
    }
}
