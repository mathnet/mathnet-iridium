//-----------------------------------------------------------------------
// <copyright file="ExcelStatisticsTests.cs" company="Math.NET Project">
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

namespace Iridium.Test.SpecialFunctionsTests
{
    using MathNet.Numerics.Distributions;

    [TestFixture]
    public class ExcelStatisticsTests
    {
        double TDIST(double x, int degrees_freedom, int tails)
        {
            StudentsTDistribution dist = new StudentsTDistribution(degrees_freedom);
            switch(tails)
            {
                case 1:
                    return 1d - dist.CumulativeDistribution(x);
                case 2:
                    return 1d - dist.CumulativeDistribution(x) + dist.CumulativeDistribution(-x);
                default:
                    throw new ArgumentOutOfRangeException("tails");
            }
        }

        [Test]
        public void TestTDIST()
        {
            NumericAssert.AreAlmostEqual(5.000000000000000E-01, TDIST(0, 2, 1), 1e-8, "A1");
            NumericAssert.AreAlmostEqual(4.129611720298140E-01, TDIST(0.25, 2, 1), 1e-8, "B1");
            NumericAssert.AreAlmostEqual(3.333333333480060E-01, TDIST(0.5, 2, 1), 1e-8, "C1");
            NumericAssert.AreAlmostEqual(2.113248654306010E-01, TDIST(1, 2, 1), 1e-8, "D1");
            NumericAssert.AreAlmostEqual(9.175170951687770E-02, TDIST(2, 2, 1), 1e-8, "E1");
            NumericAssert.AreAlmostEqual(4.926228511229080E-03, TDIST(10, 2, 1), 1e-8, "F1");

            NumericAssert.AreAlmostEqual(1.000000000000000E+00, TDIST(0, 2, 2), 1e-8, "A2");
            NumericAssert.AreAlmostEqual(8.259223440596280E-01, TDIST(0.25, 2, 2), 1e-8, "B2");
            NumericAssert.AreAlmostEqual(6.666666666960120E-01, TDIST(0.5, 2, 2), 1e-8, "C2");
            NumericAssert.AreAlmostEqual(4.226497308612030E-01, TDIST(1, 2, 2), 1e-8, "D2");
            NumericAssert.AreAlmostEqual(1.835034190337550E-01, TDIST(2, 2, 2), 1e-8, "E2");
            NumericAssert.AreAlmostEqual(9.852457022458160E-03, TDIST(10, 2, 2), 1e-8, "F2");
        }

        double GAMMADIST(double x, double alpha, double beta, bool cumulative)
        {
            GammaDistribution dist = new GammaDistribution(alpha, beta);

            return cumulative
                ? dist.CumulativeDistribution(x)
                : dist.ProbabilityDensity(x);
        }

        [Test]
        public void TestGAMMADIST()
        {
            NumericAssert.AreAlmostEqual(0.000000000000000E+00, GAMMADIST(0, 2, 1.5, true), 1e-8, "A1");
            NumericAssert.AreAlmostEqual(1.243798761594900E-02, GAMMADIST(0.25, 2, 1.5, true), 1e-8, "B1");
            NumericAssert.AreAlmostEqual(4.462491913500840E-02, GAMMADIST(0.5, 2, 1.5, true), 1e-8, "C1");
            NumericAssert.AreAlmostEqual(1.443047990137690E-01, GAMMADIST(1, 2, 1.5, true), 1e-6, "D1");
            NumericAssert.AreAlmostEqual(3.849400091283540E-01, GAMMADIST(2, 2, 1.5, true), 1e-8, "E1");
            NumericAssert.AreAlmostEqual(9.902431408555580E-01, GAMMADIST(10, 2, 1.5, true), 1e-8, "F1");

            NumericAssert.AreAlmostEqual(0.000000000000000E+00, GAMMADIST(0, 2, 1.5, false), 1e-8, "A2");
            NumericAssert.AreAlmostEqual(9.405352499591280E-02, GAMMADIST(0.25, 2, 1.5, false), 1e-8, "B2");
            NumericAssert.AreAlmostEqual(1.592291801411650E-01, GAMMADIST(0.5, 2, 1.5, false), 1e-8, "C2");
            NumericAssert.AreAlmostEqual(2.281853862562790E-01, GAMMADIST(1, 2, 1.5, false), 1e-8, "D2");
            NumericAssert.AreAlmostEqual(2.343085672340750E-01, GAMMADIST(2, 2, 1.5, false), 1e-8, "E2");
            NumericAssert.AreAlmostEqual(5.656150228662040E-03, GAMMADIST(10, 2, 1.5, false), 1e-8, "F2");
        }

        double GAMMAINV(double x, double alpha, double beta)
        {
            GammaDistribution dist = new GammaDistribution(alpha, beta);
            return dist.InverseCumulativeDistribution(x);
        }

        [Test]
        public void TestGAMMAINV()
        {
            NumericAssert.AreAlmostEqual(0.000000000000000E+00, GAMMAINV(0, 2, 1.5), 1e-8, "A1");
            NumericAssert.AreAlmostEqual(2.228321104302450E-01, GAMMAINV(0.01, 2, 1.5), 1e-8, "B1");
            NumericAssert.AreAlmostEqual(1.441918172091760E+00, GAMMAINV(0.25, 2, 1.5), 1e-6, "C1");
            NumericAssert.AreAlmostEqual(2.517520501047540E+00, GAMMAINV(0.5, 2, 1.5), 1e-8, "D1");
            NumericAssert.AreAlmostEqual(4.038951793510970E+00, GAMMAINV(0.75, 2, 1.5), 1e-8, "E1");
            NumericAssert.AreAlmostEqual(9.957528102138740E+00, GAMMAINV(0.99, 2, 1.5), 1e-8, "F1");
            NumericAssert.AreAlmostEqual(1.763455683388200E+01, GAMMAINV(0.9999, 2, 1.5), 1e-8, "G1");
        }
    }
}
