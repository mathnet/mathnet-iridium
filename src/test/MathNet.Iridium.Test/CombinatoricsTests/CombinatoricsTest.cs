//-----------------------------------------------------------------------
// <copyright file="CombinatoricsTest.cs" company="Math.NET Project">
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

namespace Iridium.Test.CombinatoricsTests
{
    using MathNet.Numerics;

    [TestFixture]
    public class CombinatoricsTest
    {
        [Test]
        public void CountingVariationsTest()
        {
            Assert.That(Combinatorics.Variations(0, 0), Is.EqualTo(1), "Var(0,0)");
            Assert.That(Combinatorics.Variations(1, 0), Is.EqualTo(1), "Var(1,0)");
            Assert.That(Combinatorics.Variations(0, 1), Is.EqualTo(0), "Var(0,1)");
            Assert.That(Combinatorics.Variations(10, 0), Is.EqualTo(1), "Var(10,0)");
            Assert.That(Combinatorics.Variations(10, 2), Is.EqualTo(90), "Var(10,2)");
            Assert.That(Combinatorics.Variations(10, 4), Is.EqualTo(5040), "Var(10,4)");
            Assert.That(Combinatorics.Variations(10, 6), Is.EqualTo(151200), "Var(10,6)");
            Assert.That(Combinatorics.Variations(10, 9), Is.EqualTo(3628800), "Var(10,9)");
            Assert.That(Combinatorics.Variations(10, 10), Is.EqualTo(3628800), "Var(10,10)");
            Assert.That(Combinatorics.Variations(10, 11), Is.EqualTo(0), "Var(10,11)");
        }

        [Test]
        public void CountingVariationsWithRepetitionTest()
        {
            Assert.That(Combinatorics.VariationsWithRepetition(0, 0), Is.EqualTo(1), "VarRep(0,0)");
            Assert.That(Combinatorics.VariationsWithRepetition(1, 0), Is.EqualTo(1), "VarRep(1,0)");
            Assert.That(Combinatorics.VariationsWithRepetition(0, 1), Is.EqualTo(0), "VarRep(0,1)");
            Assert.That(Combinatorics.VariationsWithRepetition(10, 0), Is.EqualTo(1), "VarRep(10,0)");
            Assert.That(Combinatorics.VariationsWithRepetition(10, 2), Is.EqualTo(100), "VarRep(10,2)");
            Assert.That(Combinatorics.VariationsWithRepetition(10, 4), Is.EqualTo(10000), "VarRep(10,4)");
            Assert.That(Combinatorics.VariationsWithRepetition(10, 6), Is.EqualTo(1000000), "VarRep(10,6)");
            Assert.That(Combinatorics.VariationsWithRepetition(10, 9), Is.EqualTo(1000000000), "VarRep(10,9)");
            Assert.That(Combinatorics.VariationsWithRepetition(10, 10), Is.EqualTo(10000000000), "VarRep(10,10)");
            Assert.That(Combinatorics.VariationsWithRepetition(10, 11), Is.EqualTo(100000000000), "VarRep(10,11)");
        }

        [Test]
        public void CountingCombinationsTest()
        {
            Assert.That(Combinatorics.Combinations(0, 0), Is.EqualTo(1), "Comb(0,0)");
            Assert.That(Combinatorics.Combinations(1, 0), Is.EqualTo(1), "Comb(1,0)");
            Assert.That(Combinatorics.Combinations(0, 1), Is.EqualTo(0), "Comb(0,1)");
            Assert.That(Combinatorics.Combinations(10, 0), Is.EqualTo(1), "Comb(10,0)");
            Assert.That(Combinatorics.Combinations(10, 2), Is.EqualTo(45), "Comb(10,2)");
            Assert.That(Combinatorics.Combinations(10, 4), Is.EqualTo(210), "Comb(10,4)");
            Assert.That(Combinatorics.Combinations(10, 6), Is.EqualTo(210), "Comb(10,6)");
            Assert.That(Combinatorics.Combinations(10, 9), Is.EqualTo(10), "Comb(10,9)");
            Assert.That(Combinatorics.Combinations(10, 10), Is.EqualTo(1), "Comb(10,10)");
            Assert.That(Combinatorics.Combinations(10, 11), Is.EqualTo(0), "Comb(10,11)");
        }

        [Test]
        public void CountingCombinationsWithRepetitionTest()
        {
            Assert.That(Combinatorics.CombinationsWithRepetition(0, 0), Is.EqualTo(1), "CombRep(0,0)");
            Assert.That(Combinatorics.CombinationsWithRepetition(1, 0), Is.EqualTo(1), "CombRep(1,0)");
            Assert.That(Combinatorics.CombinationsWithRepetition(0, 1), Is.EqualTo(0), "CombRep(0,1)");
            Assert.That(Combinatorics.CombinationsWithRepetition(10, 0), Is.EqualTo(1), "CombRep(10,0)");
            Assert.That(Combinatorics.CombinationsWithRepetition(10, 2), Is.EqualTo(55), "CombRep(10,2)");
            Assert.That(Combinatorics.CombinationsWithRepetition(10, 4), Is.EqualTo(715), "CombRep(10,4)");
            Assert.That(Combinatorics.CombinationsWithRepetition(10, 6), Is.EqualTo(5005), "CombRep(10,6)");
            Assert.That(Combinatorics.CombinationsWithRepetition(10, 9), Is.EqualTo(48620), "CombRep(10,9)");
            Assert.That(Combinatorics.CombinationsWithRepetition(10, 10), Is.EqualTo(92378), "CombRep(10,10)");
            Assert.That(Combinatorics.CombinationsWithRepetition(10, 11), Is.EqualTo(167960), "CombRep(10,11)");
        }

        [Test]
        public void CountingPermutations()
        {
            Assert.That(Combinatorics.Permutations(0), Is.EqualTo(1), "Perm(0)");
            Assert.That(Combinatorics.Permutations(1), Is.EqualTo(1), "Perm(1)");
            Assert.That(Combinatorics.Permutations(2), Is.EqualTo(2), "Perm(2)");
            Assert.That(Combinatorics.Permutations(8), Is.EqualTo(40320), "Perm(8)");
            Assert.That(Combinatorics.Permutations(15), Is.EqualTo(1307674368000), "Perm(15)");
            Assert.That(Combinatorics.Permutations(30), Is.EqualTo(265252859812191058636308480000000d), "Perm(30)");
            Assert.That(Combinatorics.Permutations(128), NumericIs.AlmostEqualTo(0.3856204824e216, 1e-10), "Perm(128)");
        }
    }
}
