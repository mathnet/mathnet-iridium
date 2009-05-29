using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using MathNet.Numerics.Statistics;
using MathNet.Numerics.Distributions;

namespace Iridium.Test.StatisticsTests
{
    [TestFixture]
    public class HistogramTest
    {
        [Test]
        public void TestOptimalDispersion()
        {
            ContinuousUniformDistribution dist = new ContinuousUniformDistribution(0, 10);
            Histogram h = Histogram.OptimalDispersion(10, new List<double>(dist.EnumerateDoubles(1000)));

            h.Sort();
            Assert.That(h.Count, Is.EqualTo(10));
            
            Console.WriteLine(h.ToString());
        }

        [Test]
        public void TestOptimalFreedom()
        {
            ContinuousUniformDistribution dist = new ContinuousUniformDistribution(0, 10);
            Histogram h = Histogram.OptimalFreedom(10, new List<double>(dist.EnumerateDoubles(1000)));

            h.JoinBuckets();
            Assert.That(h.Count, Is.EqualTo(10));

            Console.WriteLine(h.ToString());
        }

        [Test]
        public void TestOptimalSquaredFreedom()
        {
            ContinuousUniformDistribution dist = new ContinuousUniformDistribution(0, 10);
            Histogram h = Histogram.OptimalSquaredFreedom(10, new List<double>(dist.EnumerateDoubles(1000)));

            h.JoinBuckets();
            Assert.That(h.GetContainerOf(8).LowerBound, Is.LessThanOrEqualTo(8.0));
            Assert.That(h.GetContainerOf(8).UpperBound, Is.GreaterThanOrEqualTo(8.0));
            Assert.That(h.GetContainerIndexOf(11), Is.LessThan(0.0));
            Assert.That(h.GetContainerIndexOf(-1), Is.LessThan(0.0));
            Assert.That(delegate { h.GetContainerOf(11); }, Throws.TypeOf(typeof(ArgumentException)));
            Assert.That(delegate { h.GetContainerOf(-1); }, Throws.TypeOf(typeof(ArgumentException)));
            Assert.That(h.Count, Is.EqualTo(10));

            Console.WriteLine(h.ToString());
        }

        [Test]
        public void TestOptimalVariance()
        {
            ContinuousUniformDistribution dist = new ContinuousUniformDistribution(0, 10);
            Histogram h = Histogram.OptimalVariance(10, new List<double>(dist.EnumerateDoubles(1000)));

            Assert.That(h.Count, Is.EqualTo(10));

            Console.WriteLine(h.ToString());
        }
    }
}
