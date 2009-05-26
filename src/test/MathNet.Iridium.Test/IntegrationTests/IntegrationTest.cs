//-----------------------------------------------------------------------
// <copyright file="IntegrationTest.cs" company="Math.NET Project">
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

namespace Iridium.Test.IntegrationTests
{
    using MathNet.Numerics;
    using MathNet.Numerics.Integration;
    using MathNet.Numerics.Integration.Algorithms;

    [TestFixture]
    public class IntegrationTest
    {
        private const double TargetAreaA = 9.1082396073229965070;
        private const double StartA = 0;
        private const double StopA = 10;

        private double TargetFunctionA(double x)
        {
            // f(x) = exp(-x/5) (2 + sin(2 * x))
            return Math.Exp(-x / 5) * (2 + Math.Sin(2 * x));
        }

        [Test]
        public void TestPortal()
        {
            Assert.That(Integrate.OnClosedInterval(TargetFunctionA, StartA, StopA), NumericIs.AlmostEqualTo(TargetAreaA, 1e-5), "Basic");

            Assert.That(Integrate.OnClosedInterval(TargetFunctionA, StartA, StopA, 1e-10), NumericIs.AlmostEqualTo(TargetAreaA, 1e-10), "Basic Target 1e-10");
        }

        [Test]
        public void TestTrapeziumRule()
        {
            TrapeziumRule algorithm = new TrapeziumRule();

            Assert.That(algorithm.IntegrateTwoPoint(TargetFunctionA, StartA, StopA), NumericIs.AlmostEqualTo(TargetAreaA, 2.5e-1), "Direct (1 Partition)");

            Assert.That(algorithm.IntegrateComposite(TargetFunctionA, StartA, StopA, 1), NumericIs.AlmostEqualTo(TargetAreaA, 2.5e-1), "Composite 1 Partition");

            Assert.That(algorithm.IntegrateComposite(TargetFunctionA, StartA, StopA, 5), NumericIs.AlmostEqualTo(TargetAreaA, 1e-1), "Composite 5 Partitions");

            Assert.That(algorithm.IntegrateComposite(TargetFunctionA, StartA, StopA, 10), NumericIs.AlmostEqualTo(TargetAreaA, 2e-2), "Composite 10 Partitions");

            Assert.That(algorithm.IntegrateComposite(TargetFunctionA, StartA, StopA, 50), NumericIs.AlmostEqualTo(TargetAreaA, 6e-4), "Composite 50 Partitions");

            Assert.That(algorithm.IntegrateComposite(TargetFunctionA, StartA, StopA, 1000), NumericIs.AlmostEqualTo(TargetAreaA, 1.5e-6), "Composite 1000 Partitions");

            Assert.That(algorithm.IntegrateAdaptive(TargetFunctionA, StartA, StopA, 1e-1), NumericIs.AlmostEqualTo(TargetAreaA, 1e-1), "Adaptive Target 1e-1");

            Assert.That(algorithm.IntegrateAdaptive(TargetFunctionA, StartA, StopA, 1e-5), NumericIs.AlmostEqualTo(TargetAreaA, 1e-5), "Adaptive Target 1e-5");

            Assert.That(algorithm.IntegrateAdaptive(TargetFunctionA, StartA, StopA, 1e-10), NumericIs.AlmostEqualTo(TargetAreaA, 1e-10), "Adaptive Target 1e-10");
        }

        [Test]
        public void TestSimpsonRule()
        {
            SimpsonRule algorithm = new SimpsonRule();

            Assert.That(algorithm.IntegrateThreePoint(TargetFunctionA, StartA, StopA), NumericIs.AlmostEqualTo(TargetAreaA, 1.7e-1), "Direct (2 Partitions)");

            Assert.That(algorithm.IntegrateComposite(TargetFunctionA, StartA, StopA, 2), NumericIs.AlmostEqualTo(TargetAreaA, 1.7e-1), "Composite 2 Partitions");

            Assert.That(algorithm.IntegrateComposite(TargetFunctionA, StartA, StopA, 6), NumericIs.AlmostEqualTo(TargetAreaA, 1.2e-1), "Composite 6 Partitions");

            Assert.That(algorithm.IntegrateComposite(TargetFunctionA, StartA, StopA, 10), NumericIs.AlmostEqualTo(TargetAreaA, 8e-3), "Composite 10 Partitions");

            Assert.That(algorithm.IntegrateComposite(TargetFunctionA, StartA, StopA, 50), NumericIs.AlmostEqualTo(TargetAreaA, 8e-6), "Composite 50 Partitions");

            Assert.That(algorithm.IntegrateComposite(TargetFunctionA, StartA, StopA, 1000), NumericIs.AlmostEqualTo(TargetAreaA, 5e-11), "Composite 1000 Partitions");
        }

        [Test]
        public void TestDoubleExponentialTransformationAlgorithm()
        {
            DoubleExponentialTransformation det = new DoubleExponentialTransformation();

            Assert.That(det.Integrate(TargetFunctionA, StartA, StopA, 1e-5), NumericIs.AlmostEqualTo(TargetAreaA, 1e-5), "Adaptive Target 1e-5");

            Assert.That(det.Integrate(TargetFunctionA, StartA, StopA, 1e-10), NumericIs.AlmostEqualTo(TargetAreaA, 1e-10), "Adaptive Target 1e-10");
        }
    }
}
