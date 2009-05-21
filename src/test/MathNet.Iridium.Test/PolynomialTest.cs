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

            Assert.That(p1.Order, Is.EqualTo(2), "P1 Order");
            Assert.That(p2.Order, Is.EqualTo(0), "P2 Order");
            Assert.That(p3.Order, Is.EqualTo(2), "P3 Order");

            Polynomial r1 = p1 + p3;
            Assert.That(r1.ToString(), Is.EqualTo("4 x^2 - 3"));
            Assert.That(r1.Order, Is.EqualTo(2), "R1 Order");

            Polynomial r2 = p1 - p3;
            Assert.That(r2.ToString(), Is.EqualTo("1"));;
            Assert.That(r2.Order, Is.EqualTo(0), "R2 Order");

            Polynomial r3 = p1 + (-p3);
            Assert.That(r3.ToString(), Is.EqualTo("1"));;
            Assert.That(r3.Order, Is.EqualTo(0), "R3 Order");

            Polynomial r4 = p1 + p2;
            Assert.That(r4.ToString(), Is.EqualTo("2 x^2 + 3"));
            Assert.That(r4.Order, Is.EqualTo(2), "R4 Order");

            Polynomial r5 = p2 + p1;
            Assert.That(r5.ToString(), Is.EqualTo("2 x^2 + 3"));
            Assert.That(r5.Order, Is.EqualTo(2), "R5 Order");

            Polynomial r6 = p1 - (-p2);
            Assert.That(r6.ToString(), Is.EqualTo("2 x^2 + 3"));
            Assert.That(r6.Order, Is.EqualTo(2), "R6 Order");

            Polynomial r7 = p2 - (-p1);
            Assert.That(r7.ToString(), Is.EqualTo("2 x^2 + 3"));
            Assert.That(r7.Order, Is.EqualTo(2), "R7 Order");
        }
    }
}
