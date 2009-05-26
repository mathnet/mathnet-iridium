//-----------------------------------------------------------------------
// <copyright file="SerializationTest.cs" company="Math.NET Project">
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

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;

namespace Iridium.Test.InfrastructureTests
{
    using MathNet.Numerics;
    using MathNet.Numerics.Distributions;
    using MathNet.Numerics.LinearAlgebra;

    [TestFixture]
    public class SerializationTest
    {
        IContinuousGenerator _random;
        BinaryFormatter _formatter;

        [SetUp]
        public void Setup()
        {
            _random = new StableDistribution(0.0, 1.0, 0.5, 0.75);
            _formatter = new BinaryFormatter();
        }

        T SerializeDeserialize<T>(T before)
        {
            // Serialize to new in-memory stream
            MemoryStream stream = new MemoryStream();
            _formatter.Serialize(stream, before);

            // Deserialize from stream
            stream.Seek(0, SeekOrigin.Begin);
            T after = (T)_formatter.Deserialize(stream);
            stream.Dispose();

            return after;
        }

        [Test, Repeat(3)]
        public void TestSerializeComplex()
        {
            Complex before = Complex.Random(_random);
            Complex after = SerializeDeserialize(before);

            Assert.That(after, Is.Not.EqualTo(default(Complex)), "Not Null");
            Assert.That(after, Is.EqualTo(before), "Equal");
            Assert.That(after, NumericIs.AlmostEqualTo(before), "Almost Equal");
        }

        [Test, Repeat(3)]
        public void TestSerializeVector()
        {
            Vector before = Vector.Random(10, _random);
            Vector after = SerializeDeserialize(before);

            Assert.That(after, Is.Not.Null, "Not Null");
            Assert.That(after, Is.EqualTo(before), "Equal");
            Assert.That(after, NumericIs.AlmostEqualTo(before), "Almost Equal");
        }

        [Test, Repeat(3)]
        public void TestSerializeComplexVector()
        {
            ComplexVector before = ComplexVector.Random(10, _random);
            ComplexVector after = SerializeDeserialize(before);

            Assert.That(after, Is.Not.Null, "Not Null");
            Assert.That(after, Is.EqualTo(before), "Equal");
            Assert.That(after, NumericIs.AlmostEqualTo(before), "Almost Equal");
        }

        [Test, Repeat(3)]
        public void TestSerializeMatrix()
        {
            Matrix before = Matrix.Random(3, 5, _random);
            Matrix after = SerializeDeserialize(before);

            Assert.That(after, Is.Not.Null, "Not Null");
            Assert.That(after, Is.EqualTo(before), "Equal");
            Assert.That(after, NumericIs.AlmostEqualTo(before), "Almost Equal");
        }

        [Test, Repeat(3)]
        public void TestSerializeComplexMatrix()
        {
            ComplexMatrix before = ComplexMatrix.Random(3, 5, _random);
            ComplexMatrix after = SerializeDeserialize(before);

            Assert.That(after, Is.Not.Null, "Not Null");
            Assert.That(after, Is.EqualTo(before), "Equal");
            Assert.That(after, NumericIs.AlmostEqualTo(before), "Almost Equal");
        }

        [Test, Repeat(3)]
        public void TestSerializePolynomial()
        {
            Polynomial before = new Polynomial(Vector.Random(10, _random));
            Polynomial after = SerializeDeserialize(before);

            Assert.That(after, Is.Not.Null, "Not Null");
            Assert.That(after, Is.EqualTo(before), "Equal");
            Assert.That(after, NumericIs.AlmostEqualTo(before), "Almost Equal");
        }

        [Test, Repeat(3)]
        public void TestSerializeComplexPolynomial()
        {
            ComplexPolynomial before = new ComplexPolynomial(ComplexVector.Random(10, _random));
            ComplexPolynomial after = SerializeDeserialize(before);

            Assert.That(after, Is.Not.Null, "Not Null");
            Assert.That(after, Is.EqualTo(before), "Equal");
            Assert.That(after, NumericIs.AlmostEqualTo(before), "Almost Equal");
        }
    }
}
