//-----------------------------------------------------------------------
// <copyright file="DistributionTest.cs" company="Math.NET Project">
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
using System.Text;
using NUnit.Framework;

namespace Iridium.Test.DistributionTests
{
    using MathNet.Numerics;
    using MathNet.Numerics.Distributions;

    public sealed class DistributionShape
    {
        readonly double _scale, _offset;
        readonly int _buckets;
        int[] _shape;
        int _underflow, _overflow;

        private DistributionShape(int buckets, double scale, double offset)
        {
            _scale = scale;
            _offset = offset;
            _buckets = buckets;
            _shape = new int[buckets];
        }

        /// <summary>Create a min-max distribution shape</summary>
        /// <param name="buckets">number of buckets.</param>
        /// <param name="min">inclusive minimum.</param>
        /// <param name="max">exclusive maximum.</param>
        public static DistributionShape CreateMinMax(int buckets, double min, double max)
        {
            return new DistributionShape(buckets, buckets / (max - min), -min);
        }

        public void Push(double value)
        {
            double bucket = (value + _offset) * _scale;
            if(bucket < 0)
            {
                _underflow++;
            }
            else if(bucket >= _buckets)
            {
                _overflow++;
            }
            else
            {
                _shape[(int)bucket]++;
            }
        }

        public int Underflow
        {
            get { return _underflow; }
        }

        public int Overflow
        {
            get { return _overflow; }
        }

        public int this[int bucket]
        {
            get { return _shape[bucket]; }
        }

        public int BucketCount
        {
            get { return _buckets; }
        }
    }

    [TestFixture]
    public class DistributionTest
    {
        [Test]
        public void TestDistributionShapeTestHelper()
        {
            DistributionShape shape = DistributionShape.CreateMinMax(2, -1.0, +1.0);
            shape.Push(-1.5); // underflow
            shape.Push(-1.0); // 0
            shape.Push(-0.5); // 0
            shape.Push(0.0); // 1
            shape.Push(0.5); // 1
            shape.Push(1.0); // overflow
            shape.Push(1.5); // overflow

            Assert.That(shape.Underflow, Is.EqualTo(1), "underflow");
            Assert.That(shape.Overflow, Is.EqualTo(2), "overflow");
            Assert.That(shape[0], Is.EqualTo(2), "0");
            Assert.That(shape[1], Is.EqualTo(2), "1");
        }

        private void TestContinuousDistributionShape(
            ContinuousDistribution distribution,
            double min,
            double max,
            double[] expectedShape,
            double expectedUnderflow,
            double expectedOverflow,
            int avgSamplesPerBucket,
            double absoluteAccuracy,
            string message)
        {
            DistributionShape shape = DistributionShape.CreateMinMax(expectedShape.Length, min, max);
            int sampleCount = expectedShape.Length * avgSamplesPerBucket;
            for(int i = 0; i < sampleCount; i++)
            {
                shape.Push(distribution.NextDouble());
            }

            double scale = 1.0 / (avgSamplesPerBucket * expectedShape.Length);
            Assert.That(shape.Underflow * scale, Is.EqualTo(expectedUnderflow).Within(absoluteAccuracy), message + " Underflow");
            Assert.That(shape.Overflow * scale, Is.EqualTo(expectedOverflow).Within(absoluteAccuracy), message + " Overflow");
            for(int i = 0; i < expectedShape.Length; i++)
            {
                Assert.That(shape[i] * scale, Is.EqualTo(expectedShape[i]).Within(absoluteAccuracy), message + " Bucket " + i.ToString());
            }
        }

        private void TestContinuousDistributionShapeMatchesCumulativeDensity(
            ContinuousDistribution distribution,
            double min,
            double max,
            int numberOfBuckets,
            int avgSamplesPerBucket,
            double absoluteAccuracy,
            string message)
        {
            double[] shape = new double[numberOfBuckets];
            double bucketWidth = (max - min) / numberOfBuckets;
            double previous = distribution.CumulativeDistribution(min);
            double underflow = previous;
            double position = min;
            for(int i = 0; i < numberOfBuckets; i++)
            {
                position += bucketWidth;
                double current = distribution.CumulativeDistribution(position);
                shape[i] = current - previous;
                previous = current;
            }

            double overflow = 1 - previous;
            
            TestContinuousDistributionShape(
                distribution,
                min,
                max,
                shape,
                underflow,
                overflow,
                avgSamplesPerBucket,
                absoluteAccuracy,
                message);
        }

        [Test]
        public void TestContinuousDistributions_Uniform()
        {
            TestContinuousDistributionShapeMatchesCumulativeDensity(
                new ContinuousUniformDistribution(0.0, 1.0),
                0.0,
                1.0,
                5,
                100000,
                0.01,
                "ContinuousUniform(0.0,1.0)");

            TestContinuousDistributionShapeMatchesCumulativeDensity(
                new ContinuousUniformDistribution(-2.0, 2.0),
                -2.0,
                2.0,
                10,
                100000,
                0.01,
                "ContinuousUniform(-2.0,2.0)");
        }

        [Test]
        public void TestContinuousDistributions_Triangular()
        {
            TestContinuousDistributionShapeMatchesCumulativeDensity(
                new TriangularDistribution(2.0, 4.0, 2.5),
                2.0,
                4.0,
                10,
                100000,
                0.01,
                "TriangularDistribution(2.0,4.0,2.5)");
        }

        [Test]
        public void TestContinuousDistributions_Gaussian()
        {
            TestContinuousDistributionShapeMatchesCumulativeDensity(
                new StandardDistribution(),
                -2.0,
                2.0,
                10,
                100000,
                0.01,
                "StandardDistribution()");

            TestContinuousDistributionShapeMatchesCumulativeDensity(
                new NormalDistribution(-5.0, 2.0),
                -9.0,
                -1.0,
                10,
                100000,
                0.01,
                "NormalDistribution(-5.0,2.0)");

            TestContinuousDistributionShapeMatchesCumulativeDensity(
                new LognormalDistribution(1.0, 0.5),
                0.0,
                8.0,
                10,
                100000,
                0.01,
                "LognormalDistribution(1.0,0.5)");

            // Test Parameter Estimation
            NormalDistribution source = new NormalDistribution(4.0, 2.0);
            NormalDistribution target = new NormalDistribution();
            target.EstimateDistributionParameters(source.EnumerateDoubles(1000));
            Assert.That(target.Mu, NumericIs.AlmostEqualTo(4.0, 0.1), "Normal Parameter Estimation: Mu");
            Assert.That(target.Sigma, NumericIs.AlmostEqualTo(2.0, 0.1), "Normal Parameter Estimation: Sigma");
        }

        [Test]
        public void TestContinuousDistributions_Exponential()
        {
            TestContinuousDistributionShapeMatchesCumulativeDensity(
                new ExponentialDistribution(0.75),
                0.0,
                7.0,
                10,
                100000,
                0.01,
                "ExponentialDistribution(0.75)");
        }

        [Test]
        public void TestContinuousDistributions_GammaBeta()
        {
            TestContinuousDistributionShapeMatchesCumulativeDensity(
                new GammaDistribution(2.0, 2.0),
                0.0,
                12.0,
                10,
                100000,
                0.01,
                "GammaDistribution(2.0,2.0)");

            TestContinuousDistributionShapeMatchesCumulativeDensity(
                new BetaDistribution(2.0, 5.0),
                0.0,
                1.0,
                10,
                100000,
                0.01,
                "BetaDistribution(2.0,5.0)");
        }

        [Test]
        public void TestContinuousDistributions_Laplace()
        {
            TestContinuousDistributionShapeMatchesCumulativeDensity(
                new LaplaceDistribution(4.0, 1.5),
                0.0,
                8.0,
                10,
                100000,
                0.01,
                "LaplaceDistribution(4.0,1.5)");
        }

        [Test]
        public void TestContinuousDistributions_Chi()
        {
            TestContinuousDistributionShapeMatchesCumulativeDensity(
                new ChiDistribution(3),
                0.0,
                8.0,
                10,
                100000,
                0.01,
                "ChiDistribution(3)");

            TestContinuousDistributionShapeMatchesCumulativeDensity(
                new ChiSquareDistribution(2),
                0.0,
                8.0,
                10,
                100000,
                0.01,
                "ChiSquareDistribution(2)");
        }

        [Test]
        public void TestContinuousDistributions_Pareto()
        {
            TestContinuousDistributionShapeMatchesCumulativeDensity(
                new ParetoDistribution(1.0, 2.0),
                1.0,
                4.0,
                10,
                100000,
                0.01,
                "ParetoDistribution(1.0,2.0)");
        }

        [Test]
        public void TestContinuousDistributions_Cauchy()
        {
            TestContinuousDistributionShapeMatchesCumulativeDensity(
                new CauchyLorentzDistribution(1.0, 0.5),
                0.0,
                3.0,
                10,
                100000,
                0.01,
                "CauchyLorentzDistribution(1.0,0.5)");
        }

        [Test]
        public void TestContinuousDistributions_Erlang()
        {
            TestContinuousDistributionShapeMatchesCumulativeDensity(
                new ErlangDistribution(2, 2.0),
                0.0,
                10.0,
                10,
                100000,
                0.01,
                "ErlangDistribution(2,2.0)");
        }

        [Test]
        public void TestContinuousDistributions_Rayleigh()
        {
            TestContinuousDistributionShapeMatchesCumulativeDensity(
                new RayleighDistribution(2.0),
                0.0,
                8.0,
                10,
                100000,
                0.01,
                "RayleighDistribution(2.0)");

            // Test Parameter Estimation
            RayleighDistribution source = new RayleighDistribution(4.0);
            RayleighDistribution target = new RayleighDistribution();
            target.EstimateDistributionParameters(source.EnumerateDoubles(1000));
            Assert.That(target.Sigma, NumericIs.AlmostEqualTo(4.0, 0.1), "Rayleigh Parameter Estimation: Sigma");
        }

        [Test]
        public void TestContinuousDistributions_FisherSnedecor()
        {
            TestContinuousDistributionShapeMatchesCumulativeDensity(
                new FisherSnedecorDistribution(10, 5),
                0.0,
                3.0,
                10,
                100000,
                0.01,
                "FisherSnedecorDistribution(10,5)");

            FisherSnedecorDistribution d = new FisherSnedecorDistribution(100, 100);

            // PDF - Evaluated in Maple with "stats[statevalf,pdf,fratio[100,100]](x);"
            Assert.That(d.ProbabilityDensity(0.00), NumericIs.AlmostEqualTo(0.0), "pdf(0.00)");
            Assert.That(d.ProbabilityDensity(0.50), NumericIs.AlmostEqualTo(0.1102042201e-1, 1e-9), "pdf(0.50)");
            Assert.That(d.ProbabilityDensity(0.75), NumericIs.AlmostEqualTo(0.9462174566, 1e-9), "pdf(0.75)");
            Assert.That(d.ProbabilityDensity(1.00), NumericIs.AlmostEqualTo(1.989730935, 1e-9), "pdf(1.00)");
            Assert.That(d.ProbabilityDensity(1.25), NumericIs.AlmostEqualTo(0.8553281091, 1e-9), "pdf(1.25)");
            Assert.That(d.ProbabilityDensity(1.50), NumericIs.AlmostEqualTo(0.1722918542, 1e-9), "pdf(1.50)");
            Assert.That(d.ProbabilityDensity(2.00), NumericIs.AlmostEqualTo(0.2755105502e-2, 1e-9), "pdf(2.00)");
            Assert.That(d.ProbabilityDensity(5.00), NumericIs.AlmostEqualTo(0.6858025452e-13, 1e-9), "pdf(5.00)");

            // CDF - Evaluated in Maple with "stats[statevalf,cdr,fratio[100,100]](x);"
            Assert.That(d.CumulativeDistribution(0.00), NumericIs.AlmostEqualTo(0.0), "cdf(0.00)");
            Assert.That(d.CumulativeDistribution(0.50), NumericIs.AlmostEqualTo(0.309136863e-3, 1e-8), "cdf(0.50)");
            Assert.That(d.CumulativeDistribution(0.75), NumericIs.AlmostEqualTo(0.7602003436e-1, 1e-9), "cdf(0.75)");
            Assert.That(d.CumulativeDistribution(1.00), NumericIs.AlmostEqualTo(0.5000000000, 1e-9), "cdf(1.00)");
            Assert.That(d.CumulativeDistribution(1.25), NumericIs.AlmostEqualTo(0.8668843977, 1e-9), "cdf(1.25)");
            Assert.That(d.CumulativeDistribution(1.50), NumericIs.AlmostEqualTo(0.9780695579, 1e-9), "cdf(1.50)");
            Assert.That(d.CumulativeDistribution(2.00), NumericIs.AlmostEqualTo(0.9996908631, 1e-9), "cdf(2.00)");
            Assert.That(d.CumulativeDistribution(5.00), NumericIs.AlmostEqualTo(1.0, 1e-9), "cdf(5.00)");

            FisherSnedecorDistribution dOne = new FisherSnedecorDistribution(1, 5);
            Assert.That(dOne.ProbabilityDensity(0.00), Is.EqualTo(double.PositiveInfinity), "pdf[1,5](0.00)");
            Assert.That(dOne.CumulativeDistribution(0.00), NumericIs.AlmostEqualTo(0.0), "cdf[1,5](0.00)");

            FisherSnedecorDistribution dTwo = new FisherSnedecorDistribution(2, 5);
            Assert.That(dTwo.ProbabilityDensity(0.00), NumericIs.AlmostEqualTo(1.0), "pdf[2,5](0.00)");
            Assert.That(dTwo.CumulativeDistribution(0.00), NumericIs.AlmostEqualTo(0.0), "cdf[2,5](0.00)");
        }

        [Test]
        public void TestContinuousDistributions_StudensT()
        {
            TestContinuousDistributionShapeMatchesCumulativeDensity(
                new StudentsTDistribution(2),
                -2.0,
                5.0,
                10,
                100000,
                0.01,
                "StudentsTDistribution(2)");

            StudentsTDistribution d = new StudentsTDistribution(2);

            // PDF - Evaluated in Maple with "stats[statevalf,pdf,studentst[2]](x);"
            Assert.That(d.ProbabilityDensity(0.0), NumericIs.AlmostEqualTo(0.3535533906, 1e-9), "pdf(0)");
            Assert.That(d.ProbabilityDensity(1.0), NumericIs.AlmostEqualTo(0.1924500897, 1e-9), "pdf(1)");
            Assert.That(d.ProbabilityDensity(2.0), NumericIs.AlmostEqualTo(0.06804138174, 1e-9), "pdf(2)");
            Assert.That(d.ProbabilityDensity(3.0), NumericIs.AlmostEqualTo(0.02741012223, 1e-9), "pdf(3)");
            Assert.That(d.ProbabilityDensity(4.0), NumericIs.AlmostEqualTo(0.01309457002, 1e-9), "pdf(4)");
            Assert.That(d.ProbabilityDensity(-1.0), NumericIs.AlmostEqualTo(0.1924500897, 1e-9), "pdf(-1)");
            Assert.That(d.ProbabilityDensity(-2.0), NumericIs.AlmostEqualTo(0.06804138174, 1e-9), "pdf(-2)");
            Assert.That(d.ProbabilityDensity(-3.0), NumericIs.AlmostEqualTo(0.02741012223, 1e-9), "pdf(-3)");
            Assert.That(d.ProbabilityDensity(-4.0), NumericIs.AlmostEqualTo(0.01309457002, 1e-9), "pdf(-4)");

            // CDF - Evaluated in Maple with "stats[statevalf,cdf,studentst[2]](x);"
            Assert.That(d.CumulativeDistribution(0.0), NumericIs.AlmostEqualTo(0.5000000000), "cdf(0)");
            Assert.That(d.CumulativeDistribution(1.0), NumericIs.AlmostEqualTo(0.7886751346, 1e-9), "cdf(1)");
            Assert.That(d.CumulativeDistribution(2.0), NumericIs.AlmostEqualTo(0.9082482905, 1e-9), "cdf(2)");
            Assert.That(d.CumulativeDistribution(3.0), NumericIs.AlmostEqualTo(0.9522670169, 1e-9), "cdf(3)");
            Assert.That(d.CumulativeDistribution(4.0), NumericIs.AlmostEqualTo(0.9714045208, 1e-9), "cdf(4)");
            Assert.That(d.CumulativeDistribution(-1.0), NumericIs.AlmostEqualTo(0.2113248654, 1e-9), "cdf(-1)");
            Assert.That(d.CumulativeDistribution(-2.0), NumericIs.AlmostEqualTo(0.09175170954, 1e-9), "cdf(-2)");
            Assert.That(d.CumulativeDistribution(-3.0), NumericIs.AlmostEqualTo(0.04773298313, 1e-9), "cdf(-3)");
            Assert.That(d.CumulativeDistribution(-4.0), NumericIs.AlmostEqualTo(0.02859547921, 1e-9), "cdf(-4)");
        }
    }
}
