//-----------------------------------------------------------------------
// <copyright file="NumberTheoryTest.cs" company="Math.NET Project">
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

namespace Iridium.Test.NumberTheoryTests
{
    using MathNet.Numerics.NumberTheory;

    [TestFixture]
    public class NumberTheoryTest
    {
        [Test]
        public void TestEvenOdd32()
        {
            Assert.That(IntegerTheory.IsEven(0), "0 is even");
            Assert.That(!IntegerTheory.IsOdd(0), "0 is not odd");

            Assert.That(!IntegerTheory.IsEven(1), "1 is not even");
            Assert.That(IntegerTheory.IsOdd(1), "1 is odd");

            Assert.That(!IntegerTheory.IsEven(-1), "-1 is not even");
            Assert.That(IntegerTheory.IsOdd(-1), "-1 is odd");

            Assert.That(!IntegerTheory.IsEven(Int32.MaxValue), "Int32.Max is not even");
            Assert.That(IntegerTheory.IsOdd(Int32.MaxValue), "Int32.Max is odd");

            Assert.That(IntegerTheory.IsEven(Int32.MinValue), "Int32.Min is even");
            Assert.That(!IntegerTheory.IsOdd(Int32.MinValue), "Int32.Min is not odd");
        }

        [Test]
        public void TestEvenOdd64()
        {
            Assert.That(IntegerTheory.IsEven((long)0), "0 is even");
            Assert.That(!IntegerTheory.IsOdd((long)0), "0 is not odd");

            Assert.That(!IntegerTheory.IsEven((long)1), "1 is not even");
            Assert.That(IntegerTheory.IsOdd((long)1), "1 is odd");

            Assert.That(!IntegerTheory.IsEven((long)-1), "-1 is not even");
            Assert.That(IntegerTheory.IsOdd((long)-1), "-1 is odd");

            Assert.That(!IntegerTheory.IsEven(Int64.MaxValue), "Int64.Max is not even");
            Assert.That(IntegerTheory.IsOdd(Int64.MaxValue), "Int64.Max is odd");

            Assert.That(IntegerTheory.IsEven(Int64.MinValue), "Int64.Min is even");
            Assert.That(!IntegerTheory.IsOdd(Int64.MinValue), "Int64.Min is not odd");
        }

        [Test]
        public void TestIsPerfectSquare32()
        {
            // Test all known suares
            int lastRadix = (int)Math.Floor(Math.Sqrt(Int32.MaxValue));
            for(int i = 0; i <= lastRadix; i++)
            {
                Assert.That(IntegerTheory.IsPerfectSquare(i * i), i + "^2 (+)");
            }

            // Test 1-offset from all known squares
            for(int i = 2; i <= lastRadix; i++)
            {
                Assert.That(!IntegerTheory.IsPerfectSquare((i * i) - 1), i + "^2-1 (-)");
                Assert.That(!IntegerTheory.IsPerfectSquare((i * i) + 1), i + "^2+1 (-)");
            }

            // Selected Cases
            Assert.That(IntegerTheory.IsPerfectSquare(100000000), "100000000 (+)");
            Assert.That(!IntegerTheory.IsPerfectSquare(100000001), "100000001 (-)");
            Assert.That(!IntegerTheory.IsPerfectSquare(99999999), "99999999 (-)");
            Assert.That(!IntegerTheory.IsPerfectSquare(-4), "-4 (-)");
            Assert.That(!IntegerTheory.IsPerfectSquare(Int32.MinValue), "Int32.MinValue (-)");
            Assert.That(!IntegerTheory.IsPerfectSquare(Int32.MaxValue), "Int32.MaxValue (-)");
            Assert.That(IntegerTheory.IsPerfectSquare(1), "1 (+)");
            Assert.That(IntegerTheory.IsPerfectSquare(0), "0 (+)");
            Assert.That(!IntegerTheory.IsPerfectSquare(-1), "-1 (-)");
        }

        [Test]
        public void TestIsPerfectSquare64()
        {
            // Test all known suares
            for(int i = 0; i < 32; i++)
            {
                long t = ((long)1) << i;
                Assert.That(IntegerTheory.IsPerfectSquare(t * t), t + "^2 (+)");
            }

            // Test 1-offset from all known squares
            for(int i = 1; i < 32; i++)
            {
                long t = ((long)1) << i;
                Assert.That(!IntegerTheory.IsPerfectSquare((t * t) - 1), t + "^2-1 (-)");
                Assert.That(!IntegerTheory.IsPerfectSquare((t * t) + 1), t + "^2+1 (-)");
            }

            // Selected Cases
            Assert.That(IntegerTheory.IsPerfectSquare((long)1000000000000000000), "1000000000000000000 (+)");
            Assert.That(!IntegerTheory.IsPerfectSquare((long)1000000000000000001), "1000000000000000001 (-)");
            Assert.That(!IntegerTheory.IsPerfectSquare((long)999999999999999999), "999999999999999999 (-)");
            Assert.That(!IntegerTheory.IsPerfectSquare((long)999999999999999993), "999999999999999993 (-)");
            Assert.That(!IntegerTheory.IsPerfectSquare((long)-4), "-4 (-)");
            Assert.That(!IntegerTheory.IsPerfectSquare(Int64.MinValue), "Int32.MinValue (-)");
            Assert.That(!IntegerTheory.IsPerfectSquare(Int64.MaxValue), "Int32.MaxValue (-)");
            Assert.That(IntegerTheory.IsPerfectSquare((long)1), "1 (+)");
            Assert.That(IntegerTheory.IsPerfectSquare((long)0), "0 (+)");
            Assert.That(!IntegerTheory.IsPerfectSquare((long)-1), "-1 (-)");
        }
    }
}
