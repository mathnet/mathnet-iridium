//-----------------------------------------------------------------------
// <copyright file="FftStressTest.cs" company="Math.NET Project">
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
using NUnit.Framework;

namespace Iridium.Test.TransformationsTests
{
    using MathNet.Numerics.Transformations;

    [TestFixture]
    public class FftStressTest
    {
        private RealFourierTransformation _fft;

        [SetUp]
        public void SetUp()
        {
            _fft = new RealFourierTransformation();
            _fft.Convention = TransformationConvention.Default;
        }

        [Test, Sequential]
        public void FftSingleReal(
            [Values(1024, 4096, 8192, 16384, 65536, 262144, 1048576)] int numSamples)
        {
            int half = numSamples >> 1;

            double[] dataEven = new double[numSamples];
            for(int i = 0; i < numSamples; i++)
            {
                double z = (double)(i - half) / half;
                dataEven[i] = 1.0 / ((z * z) + 1.0);
            }

            double[] evenReal, evenImag;
            _fft.TransformForward(dataEven, out evenReal, out evenImag);

            // round trip check
            double[] dataEven2;
            _fft.TransformBackward(evenReal, evenImag, out dataEven2);

            // Compare with original samples
            for(int i = 0; i < numSamples; i += 2)
            {
                Assert.That(dataEven2[i], NumericIs.AlmostEqualTo(dataEven[i], 1e-5), i.ToString());
            }
        }
    }
}
