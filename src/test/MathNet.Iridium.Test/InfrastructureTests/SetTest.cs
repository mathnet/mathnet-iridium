//-----------------------------------------------------------------------
// <copyright file="SetTest.cs" company="Math.NET Project">
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

using System.Collections.Generic;
using NUnit.Framework;

namespace Iridium.Test.InfrastructureTests
{
    using MathNet.Numerics;

    [TestFixture]
    public class SetTest
    {
        [Test]
        public void SetIntersectionTest()
        {
            Set<int> a = new Set<int>();
            for(int i = 0; i < 16; i += 2)
            {
                a.Add(i);
            }

            Set<int> b = new Set<int>();
            for(int i = 0; i < 16; i += 3)
            {
                b.Add(i);
            }

            Set<int> inter = a.Intersect(b);

            Assert.That(inter.Count, Is.EqualTo(3), "A01");
            Assert.That(inter[0], Is.EqualTo(0), "A02");
            Assert.That(inter[1], Is.EqualTo(6), "A03");
            Assert.That(inter[2], Is.EqualTo(12), "A04");
        }

        [Test]
        public void PortedSetIntersectionTest()
        {
            int[] array1 = new int[100];
            int[] array2 = new int[100];

            for(int i = 0; i < array1.Length; i++)
            {
                array1[i] = i;
                array2[i] = i / 2;
            }

            IList<int> intersection = Set<int>.Intersect(array1, array2);

            Assert.That(intersection.Count, Is.EqualTo(50));
            foreach(int k in intersection)
            {
                Assert.That(k, Is.GreaterThanOrEqualTo(0));
                Assert.That(k, Is.LessThanOrEqualTo(50));
            }
        }

        [Test]
        public void PortedSetConcatCountTest()
        {
            int[] array0 = new int[0];
            int[] array1 = new int[7];
            int[] array2 = new int[13];

            Assert.That(Set<int>.Concatenate(array0, array0).Count, Is.EqualTo(array0.Length + array0.Length));
            Assert.That(Set<int>.Concatenate(array0, array1).Count, Is.EqualTo(array0.Length + array1.Length));
            Assert.That(Set<int>.Concatenate(array0, array2).Count, Is.EqualTo(array0.Length + array2.Length));
            Assert.That(Set<int>.Concatenate(array1, array0).Count, Is.EqualTo(array1.Length + array0.Length));
            Assert.That(Set<int>.Concatenate(array1, array1).Count, Is.EqualTo(array1.Length + array1.Length));
            Assert.That(Set<int>.Concatenate(array1, array2).Count, Is.EqualTo(array1.Length + array2.Length));
        }

        [Test]
        public void PortedSetConcatTest()
        {
            // generating two arrays
            int[] array1 = new int[10];
            int[] array2 = new int[13];

            for(int i = 0; i < array1.Length; i++)
            {
                array1[i] = i;
            }

            for(int i = 0; i < array2.Length; i++)
            {
                array2[i] = i + array1.Length;
            }

            Set<int> union = Set<int>.Concatenate(array1, array2);

            int index = 0;
            foreach(int value in union)
            {
                Assert.That(value, Is.EqualTo(index++));
            }

            Assert.That(index, Is.EqualTo(array1.Length + array2.Length));
        }

        [Test]
        public void PortedSetUnionTest()
        {
            int[] array1 = new int[100];
            int[] array2 = new int[100];

            for(int i = 0; i < array1.Length; i++)
            {
                array1[i] = i;
                array2[i] = i / 2;
            }

            IList<int> union = Set<int>.Union(array1, array2);

            Assert.That(union.Count, Is.EqualTo(100));
            foreach(int k in union)
            {
                Assert.That(k, Is.GreaterThanOrEqualTo(0));
                Assert.That(k, Is.LessThanOrEqualTo(100));
            }
        }

        [Test]
        public void PortedSetSubtractTest()
        {
            int[] array1 = new int[100];
            int[] array2 = new int[100];

            for(int i = 0; i < array1.Length; i++)
            {
                array1[i] = i;
                array2[i] = i / 2;
            }

            Set<int> minus = Set<int>.Subtract(array1, array2);

            Assert.That(minus.Count, Is.EqualTo(50));
            foreach(int k in minus)
            {
                Assert.That(k, Is.GreaterThanOrEqualTo(50));
                Assert.That(k, Is.LessThanOrEqualTo(100));
            }
        }
    }
}
