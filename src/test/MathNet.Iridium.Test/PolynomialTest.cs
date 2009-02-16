//-----------------------------------------------------------------------
// <copyright file="PolynomialTest.cs" company="Math.NET Project">
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

namespace Iridium.Test
{
    using MathNet.Numerics;

    [TestFixture]
    public class PolynomialTest
    {
        [Test]
        public void TestPolynomial_Additive()
        {
            Polynomial p1 = new Polynomial(new double[] { -1, 0, 2 });
            Polynomial p2 = new Polynomial(new double[] { 4 });
            Polynomial p3 = new Polynomial(new double[] { -2, 0, 2 });

            Assert.AreEqual(2, p1.Order, "P1 Order");
            Assert.AreEqual(0, p2.Order, "P2 Order");
            Assert.AreEqual(2, p3.Order, "P3 Order");

            Polynomial r1 = p1 + p3;
            Assert.AreEqual("4 x^2 - 3", r1.ToString());
            Assert.AreEqual(2, r1.Order, "R1 Order");

            Polynomial r2 = p1 - p3;
            Assert.AreEqual("1", r2.ToString());
            Assert.AreEqual(0, r2.Order, "R2 Order");

            Polynomial r3 = p1 + (-p3);
            Assert.AreEqual("1", r3.ToString());
            Assert.AreEqual(0, r3.Order, "R3 Order");

            Polynomial r4 = p1 + p2;
            Assert.AreEqual("2 x^2 + 3", r4.ToString());
            Assert.AreEqual(2, r4.Order, "R4 Order");

            Polynomial r5 = p2 + p1;
            Assert.AreEqual("2 x^2 + 3", r5.ToString());
            Assert.AreEqual(2, r5.Order, "R5 Order");

            Polynomial r6 = p1 - (-p2);
            Assert.AreEqual("2 x^2 + 3", r6.ToString());
            Assert.AreEqual(2, r6.Order, "R6 Order");

            Polynomial r7 = p2 - (-p1);
            Assert.AreEqual("2 x^2 + 3", r7.ToString());
            Assert.AreEqual(2, r7.Order, "R7 Order");
        }
    }
}
