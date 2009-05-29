//-----------------------------------------------------------------------
// <copyright file="SearchingTest.cs" company="Math.NET Project">
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
using NUnit.Framework;

namespace Iridium.Test.InfrastructureTests
{
    using MathNet.Numerics;

    [TestFixture]
    public class SearchingTest
    {
        private class ItemWithMap : IComparable<int>
        {
            public ItemWithMap(int value)
            {
                _value = value;
            }

            int _value;

            public int CompareTo(int other)
            {
                return _value.CompareTo(other);
            }
        }

        [Test]
        public void TestBinarySearchOnArray()
        {
            int[] data = { -2, 3, 5, 20, 21, 1000, 2500 };

            Assert.That(Searching.BinarySearch(data, -100), Is.LessThan(0));
            Assert.That(Searching.BinarySearch(data, 0), Is.LessThan(0));
            Assert.That(Searching.BinarySearch(data, 100000), Is.LessThan(0));

            for(int i=0; i<data.Length; i++)
            {
                Assert.That(Searching.BinarySearch(data, data[i]), Is.EqualTo(i));
            }
        }

        [Test]
        public void TestBinarySearchOnGenericList()
        {
            List<int> data = new List<int>(new int[] { -2, 3, 5, 20, 21, 1000, 2500 });

            Assert.That(Searching.BinarySearch(data, -100), Is.LessThan(0));
            Assert.That(Searching.BinarySearch(data, 0), Is.LessThan(0));
            Assert.That(Searching.BinarySearch(data, 100000), Is.LessThan(0));

            for(int i = 0; i < data.Count; i++)
            {
                Assert.That(Searching.BinarySearch(data, data[i]), Is.EqualTo(i));
            }
        }

        [Test]
        public void TestBinaryMapSearchOnArray()
        {
            int[] intData = { -2, 3, 5, 20, 21, 1000, 2500 };
            ItemWithMap[] data = new ItemWithMap[intData.Length];
            for(int i = 0; i < data.Length; i++ )
            {
                data[i] = new ItemWithMap(intData[i]);
            }

            Assert.That(Searching.BinaryMapSearch(data, -100), Is.LessThan(0));
            Assert.That(Searching.BinaryMapSearch(data, 0), Is.LessThan(0));
            Assert.That(Searching.BinaryMapSearch(data, 100000), Is.LessThan(0));

            for(int i = 0; i < data.Length; i++)
            {
                Assert.That(Searching.BinaryMapSearch(data, intData[i]), Is.EqualTo(i));
            }
        }
    }
}
