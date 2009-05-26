//-----------------------------------------------------------------------
// <copyright file="NumericIs.cs" company="Math.NET Project">
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
using NUnit.Framework.Constraints;

namespace Iridium.Test
{
    using MathNet.Numerics;

    /// <summary>
    /// Custom NUnit Assertion Test Constraint Factory
    /// </summary>
    public class NumericIs
    {
        public static Constraint AlmostEqualTo(double expected)
        {
            return new AlmostEqualConstraint(expected);
        }

        public static Constraint AlmostEqualTo(double expected, double maximumRelativeError)
        {
            return new AlmostEqualConstraint(expected, maximumRelativeError);
        }

        public static Constraint AlmostEqualTo<T>(T expected)
            where T : IAlmostEquatable<T>
        {
            return new AlmostEqualGenericConstraint<T>(expected);
        }

        public static Constraint AlmostEqualTo<T>(T expected, double maximumRelativeError)
            where T : IAlmostEquatable<T>
        {
            return new AlmostEqualGenericConstraint<T>(expected, maximumRelativeError);
        }

        private class AlmostEqualConstraint : Constraint
        {
            private readonly double _expected;
            private readonly double? _maximumRelativeError;

            public AlmostEqualConstraint(double expected)
                : base(expected)
            {
                _expected = expected;
            }

            public AlmostEqualConstraint(double expected, double maximumRelativeError)
                : base(expected)
            {
                _expected = expected;
                _maximumRelativeError = maximumRelativeError;
            }

            public override bool Matches(object actual)
            {
                base.actual = actual;

                if(_maximumRelativeError.HasValue)
                {
                    return Number.AlmostEqual(_expected, Convert.ToDouble(actual), _maximumRelativeError.Value);
                }

                return Number.AlmostEqual(_expected, Convert.ToDouble(actual));
            }

            public override void WriteDescriptionTo(MessageWriter writer)
            {
                writer.WriteExpectedValue(_expected);
                if(_maximumRelativeError.HasValue)
                {
                    writer.WriteConnector("+/- relative");
                    writer.WriteExpectedValue(_maximumRelativeError);
                }
            }
        }

        private class AlmostEqualGenericConstraint<T> : Constraint
            where T : IAlmostEquatable<T>
        {
            private readonly T _expected;
            private readonly double? _maximumRelativeError;

            public AlmostEqualGenericConstraint(T expected)
                : base(expected)
            {
                _expected = expected;
            }

            public AlmostEqualGenericConstraint(T expected, double maximumRelativeError)
                : base(expected)
            {
                _expected = expected;
                _maximumRelativeError = maximumRelativeError;
            }

            public override bool Matches(object actual)
            {
                base.actual = actual;

                if(_maximumRelativeError.HasValue)
                {
                    return Number.AlmostEqual(_expected, (T)actual, _maximumRelativeError.Value);
                }

                return Number.AlmostEqual(_expected, (T)actual);
            }

            public override void WriteDescriptionTo(MessageWriter writer)
            {
                writer.WriteExpectedValue(_expected);
                if(_maximumRelativeError.HasValue)
                {
                    writer.WriteConnector("+/- relative");
                    writer.WriteExpectedValue(_maximumRelativeError);
                }
            }
        }
    }
}
