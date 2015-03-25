//-----------------------------------------------------------------------
// <copyright file="Complex.cs" company="Math.NET Project">
//    Copyright (c) 2002-2009, Christoph Rüegg, Joannes Vermorel.
//    All Right Reserved.
// </copyright>
// <author>
//    Christoph Rüegg, http://christoph.ruegg.name
//    Joannes Vermorel, http://www.vermorel.com
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
using System.Collections;
using System.Globalization;  
using System.Text;

namespace MathNet.Numerics
{
    using Distributions;

    /// <summary>
    /// Complex numbers class.
    /// </summary>
    /// <remarks>
    /// <p>
    /// The class <c>Complex</c> provides all elementary operations
    /// on complex numbers. All the operators <c>+</c>, <c>-</c>,
    /// <c>*</c>, <c>/</c>, <c>==</c>, <c>!=</c> are defined in the
    /// canonical way. Additional complex trigonometric functions such 
    /// as <see cref="Complex.Cosine"/>, ... 
    /// are also provided. Note that the <c>Complex</c> structures 
    /// has two special constant values <see cref="Complex.NaN"/> and 
    /// <see cref="Complex.Infinity"/>.
    /// </p>
    /// <p>
    /// In order to avoid possible ambiguities resulting from a 
    /// <c>Complex(double, double)</c> constructor, the static methods 
    /// <see cref="Complex.FromRealImaginary"/> and <see cref="Complex.FromModulusArgument"/>
    /// are provided instead.
    /// </p>
    /// <code>
    /// Complex x = Complex.FromRealImaginary(1d, 2d);
    /// Complex y = Complex.FromModulusArgument(1d, Math.Pi);
    /// Complex z = (x + y) / (x - y);
    /// </code>
    /// <p>
    /// Since there is no canonical order among the complex numbers,
    /// <c>Complex</c> does not implement <c>IComparable</c> but several
    /// lexicographic <c>IComparer</c> implementations are provided, see 
    /// <see cref="Complex.RealImaginaryComparer"/>,
    /// <see cref="Complex.ModulusArgumentComparer"/> and
    /// <see cref="Complex.ArgumentModulusComparer"/>.
    /// </p>
    /// <p>
    /// For mathematical details about complex numbers, please
    /// have a look at the <a href="http://en.wikipedia.org/wiki/Complex_number">
    /// Wikipedia</a>
    /// </p>
    /// </remarks>
    [Serializable]
    public struct Complex :
        IEquatable<Complex>,
        IAlmostEquatable<Complex>,
        IComparable<Complex>
    {
        #region Complex comparers

        private sealed class RealImaginaryLexComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                throw new NotImplementedException();
            }
        }

        private sealed class ModulusArgumentLexComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                throw new NotImplementedException();
            }
        }

        private sealed class ArgumentModulusLexComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                throw new NotImplementedException();
            }
        }

        private static IComparer _realImaginaryComparer;
        private static IComparer _modulusArgumentComparer;
        private static IComparer _argumentModulusComparer;

        /// <summary>
        /// Gets the lexicographical comparer based on <c>(real, imaginary)</c>. 
        /// </summary>
        public static IComparer RealImaginaryComparer
        {
            get
            {
                if(_realImaginaryComparer == null)
                {
                    _realImaginaryComparer = new RealImaginaryLexComparer();
                }

                return _realImaginaryComparer;
            }
        }

        /// <summary>
        /// Gets the lexicographical comparer based on <c>(modulus, argument)</c>.
        /// </summary>
        public static IComparer ModulusArgumentComparer
        {
            get
            {
                if(_modulusArgumentComparer == null)
                {
                    _modulusArgumentComparer = new ModulusArgumentLexComparer();
                }

                return _modulusArgumentComparer;
            }
        }

        /// <summary>
        /// Gets the lexicographical comparer based on <c>(argument, modulus)</c>.
        /// </summary>
        public static IComparer ArgumentModulusComparer
        {
            get
            {
                if(_argumentModulusComparer == null)
                {
                    _argumentModulusComparer = new ArgumentModulusLexComparer();
                }

                return _argumentModulusComparer;
            }
        }

        #endregion

        double _real;
        double _imag;

        #region Constructors and Constants

        /// <summary>
        /// Initializes a new instance of the Complex 
        /// from its real and imaginary parts.
        /// </summary>
        public
        Complex(double real, double imag)
        {
            _real = real;
            _imag = imag;
        }

        /// <summary>
        /// Constructs a <c>Complex</c> from its real
        /// and imaginary parts.
        /// </summary>
        public static
        Complex
        FromRealImaginary(double real, double imag)
        {
            return new Complex(real, imag);
        }

        /// <summary>
        /// Constructs a <c>Complex</c> from its modulus and
        /// argument.
        /// </summary>
        /// <param name="modulus">Must be non-negative.</param>
        /// <param name="argument">Real number.</param>
        public static
        Complex
        FromModulusArgument(double modulus, double argument)
        {
            if(modulus < 0d)
            {
                throw new ArgumentOutOfRangeException(
                    "modulus",
                    modulus,
                    Properties.LocalStrings.ArgumentNotNegative);
            }

            return new Complex(
                modulus * Math.Cos(argument),
                modulus * Math.Sin(argument));
        }

        /// <summary>
        /// Constructs a complex number with random real and imaginary value.
        /// </summary>
        /// <param name="realRandomDistribution">Continuous random distribution or source for the real part.</param>
        /// <param name="imagRandomDistribution">Continuous random distribution or source for the imaginary part.</param>
        public static
        Complex
        Random(
            IContinuousGenerator realRandomDistribution,
            IContinuousGenerator imagRandomDistribution)
        {
            return new Complex(
                realRandomDistribution.NextDouble(),
                imagRandomDistribution.NextDouble());
        }

        /// <summary>
        /// Constructs a complex number with random real and imaginary value.
        /// </summary>
        /// <param name="randomDistribution">Continuous random distribution or source for the real and imaginary parts.</param>
        public static
        Complex
        Random(IContinuousGenerator randomDistribution)
        {
            return new Complex(
                randomDistribution.NextDouble(),
                randomDistribution.NextDouble());
        }

        /// <summary>
        /// Constructs a complex number with random modulus and argument.
        /// </summary>
        /// <param name="modulusRandomDistribution">Continuous random distribution or source for the modulus.</param>
        /// <param name="argumentRandomDistribution">Continuous random distribution or source for the argument.</param>
        public static
        Complex
        RandomPolar(
            IContinuousGenerator modulusRandomDistribution,
            IContinuousGenerator argumentRandomDistribution)
        {
            return FromModulusArgument(
                modulusRandomDistribution.NextDouble(),
                argumentRandomDistribution.NextDouble());
        }

        /// <summary>
        /// Constructs a complex number on the unit circle with random argument.
        /// </summary>
        /// <param name="argumentRandomDistribution">Continuous random distribution or source for the argument.</param>
        public static
        Complex
        RandomUnitCircle(IContinuousGenerator argumentRandomDistribution)
        {
            return FromModulusArgument(
                1d,
                argumentRandomDistribution.NextDouble());
        }

        /// <summary>
        /// Represents the zero value. This field is constant.
        /// </summary>
        public static Complex Zero
        {
            get { return new Complex(0d, 0d); }
        }

        /// <summary>
        /// Indicates whether the <c>Complex</c> is zero.
        /// </summary>
        public bool IsZero
        {
            get { return Number.AlmostZero(_real) && Number.AlmostZero(_imag); }
        }

        /// <summary>
        /// Represents the <c>1</c> value. This field is constant.
        /// </summary>
        public static Complex One
        {
            get { return new Complex(1d, 0d); }
        }

        /// <summary>
        /// Indicates whether the <c>Complex</c> is one.
        /// </summary>
        public bool IsOne
        {
            get { return Number.AlmostEqual(_real, 1) && Number.AlmostZero(_imag); }
        }

        /// <summary>
        /// Represents the imaginary unit number. This field is constant.
        /// </summary>
        public static Complex I
        {
            get { return new Complex(0d, 1d); }
        }

        /// <summary>
        /// Indicates whether the <c>Complex</c> is the imaginary unit.
        /// </summary>
        public bool IsI
        {
            get { return Number.AlmostZero(_real) && Number.AlmostEqual(_imag, 1); }
        }

        /// <summary>
        /// Represents a value that is not a number. This field is constant.
        /// </summary>
        public static Complex NaN
        {
            get { return new Complex(double.NaN, double.NaN); }
        }

        /// <summary>
        /// Indicates whether the provided <c>Complex</c> evaluates to a
        /// value that is not a number.
        /// </summary>
        public bool IsNaN
        {
            get { return double.IsNaN(_real) || double.IsNaN(_imag); }
        }

        /// <summary>
        /// Represents the infinity value. This field is constant.
        /// </summary>
        /// <remarks>
        /// The semantic associated to this value is a <c>Complex</c> of 
        /// infinite real and imaginary part. If you need more formal complex
        /// number handling (according to the Riemann Sphere and the extended
        /// complex plane C*, or using directed infinity) please check out the
        /// alternative MathNet.PreciseNumerics and MathNet.Symbolics packages
        /// instead.
        /// </remarks>
        public static Complex Infinity
        {
            get { return new Complex(double.PositiveInfinity, double.PositiveInfinity); }
        }

        /// <summary>
        /// Indicates the provided <c>Complex</c> evaluates to an
        /// infinite value.
        /// </summary>
        /// <remarks>
        /// True if it either evaluates to a complex infinity
        /// or to a directed infinity.
        /// </remarks>
        public bool IsInfinity
        {
            get { return double.IsInfinity(_real) || double.IsInfinity(_imag); }
        }

        /// <summary>
        /// Indicates the provided <c>Complex</c> is real.
        /// </summary>
        public bool IsReal
        {
            get { return Number.AlmostZero(_imag); }
        }

        /// <summary>
        /// Indicates the provided <c>Complex</c> is real and not negative, that is >= 0.
        /// </summary>
        public bool IsRealNonNegative
        {
            get { return Number.AlmostZero(_imag) && _real >= 0; }
        }

        /// <summary>
        /// Indicates the provided <c>Complex</c> is imaginary.
        /// </summary>
        public bool IsImaginary
        {
            get { return Number.AlmostZero(_real); }
        }

        #endregion

        #region Cartesian and Polar Components

        /// <summary>
        /// Gets or sets the real part of this <c>Complex</c>.
        /// </summary>
        /// <seealso cref="Imag"/>
        public double Real
        {
            get { return _real; }
            set { _real = value; }
        }

        /// <summary>
        /// Gets or sets the imaginary part of this <c>Complex</c>.
        /// </summary>
        /// <seealso cref="Real"/>
        public double Imag
        {
            get { return _imag; }
            set { _imag = value; }
        }

        /// <summary>
        /// Gets or sets the modulus of this <c>Complex</c>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if an attempt is made to set a negative modulus.
        /// </exception>
        /// <remarks>
        /// If this <c>Complex</c> is zero when the modulus is set,
        /// the Complex is assumed to be positive real with an argument of zero.
        /// </remarks>
        /// <seealso cref="Argument"/>
        public double Modulus
        {
            get
            {
                return Math.Sqrt((_real * _real) + (_imag * _imag));
            }

            set
            {
                if(value < 0d)
                {
                    throw new ArgumentOutOfRangeException(
                        "value",
                        value,
                        Properties.LocalStrings.ArgumentNotNegative);
                }

                if(double.IsInfinity(value))
                {
                    _real = value;
                    _imag = value;
                }
                else
                {
                    if(IsZero)
                    {
                        _real = value;
                        _imag = 0;
                    }
                    else
                    {
                        double factor = value / Math.Sqrt((_real * _real) + (_imag * _imag));
                        _real *= factor;
                        _imag *= factor;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the squared modulus of this <c>Complex</c>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if an attempt is made to set a negative modulus.
        /// </exception>
        /// <remarks>
        /// If this <c>Complex</c> is zero when the modulus is set,
        /// the Complex is assumed to be positive real with an argument of zero.
        /// </remarks>
        /// <seealso cref="Argument"/>
        public double ModulusSquared
        {
            get
            {
                return (_real * _real) + (_imag * _imag);
            }

            set
            {
                if(value < 0d)
                {
                    throw new ArgumentOutOfRangeException(
                        "value",
                        value,
                        Properties.LocalStrings.ArgumentNotNegative);
                }

                if(double.IsInfinity(value))
                {
                    _real = value;
                    _imag = value;
                }
                else
                {
                    if(IsZero)
                    {
                        _real = Math.Sqrt(value);
                        _imag = 0;
                    }
                    else
                    {
                        double factor = value / ((_real * _real) + (_imag * _imag));
                        _real *= factor;
                        _imag *= factor;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the argument of this <c>Complex</c>.
        /// </summary>
        /// <remarks>
        /// Argument always returns a value bigger than negative Pi and
        /// smaller or equal to Pi. If this <c>Complex</c> is zero, the Complex
        /// is assumed to be positive real with an argument of zero.
        /// </remarks>
        public double Argument
        {
            get
            {
                if(IsReal && _real < 0)
                {
                    return Math.PI;
                }

                if(IsRealNonNegative)
                {
                    return 0;
                }

                return Math.Atan2(_imag, _real);
            }

            set
            {
                double modulus = Modulus;
                _real = Math.Cos(value) * modulus;
                _imag = Math.Sin(value) * modulus;
            }
        }

        /// <summary>
        /// Gets the unity of this complex (same argument, but on the unit circle; exp(I*arg))
        /// </summary>
        public Complex Sign
        {
            get
            {
                if(double.IsPositiveInfinity(_real) && double.IsPositiveInfinity(_imag))
                {
                    return new Complex(Constants.Sqrt1_2, Constants.Sqrt1_2);
                }

                if(double.IsPositiveInfinity(_real) && double.IsNegativeInfinity(_imag))
                {
                    return new Complex(Constants.Sqrt1_2, -Constants.Sqrt1_2);
                }

                if(double.IsNegativeInfinity(_real) && double.IsPositiveInfinity(_imag))
                {
                    return new Complex(-Constants.Sqrt1_2, -Constants.Sqrt1_2);
                }

                if(double.IsNegativeInfinity(_real) && double.IsNegativeInfinity(_imag))
                {
                    return new Complex(-Constants.Sqrt1_2, Constants.Sqrt1_2);
                }

                // don't replace this with "Modulus"!
                double mod = Fn.Hypot(_real, _imag);
                if(mod == 0)
                {
                    return One;
                }

                return new Complex(_real / mod, _imag / mod);
            }
        }

        #endregion

        /// <summary>
        /// Gets or sets the conjugate of this <c>Complex</c>.
        /// </summary>
        /// <remarks>
        /// The semantic of <i>setting the conjugate</i> is such that
        /// <code>
        /// // a, b of type Complex
        /// a.Conjugate = b;
        /// </code>
        /// is equivalent to
        /// <code>
        /// // a, b of type Complex
        /// a = b.Conjugate
        /// </code>
        /// </remarks>
        public Complex Conjugate
        {
            get { return new Complex(_real, -_imag); }
            set { this = value.Conjugate; }
        }

        #region Equality & Hashing

        /// <summary>
        /// Indicates whether <c>obj</c> is equal to this instance.
        /// </summary>
        public override
        bool
        Equals(object obj)
        {
            return (obj is Complex) && Equals((Complex)obj);
        }

        /// <summary>
        /// Indicates whether <c>z</c> is equal to this complex number.
        /// </summary>
        public
        bool
        Equals(Complex other)
        {
            return !IsNaN
                && !other.IsNaN
                && (_real == other._real)
                && (_imag == other._imag);
        }

        /// <summary>
        /// Indicates whether <paramref name="other"/> is almost equal to this complex number, up to the default maximum relative error.
        /// </summary>
        public
        bool
        AlmostEquals(Complex other)
        {
            return !IsNaN
                && !other.IsNaN
                && Number.AlmostEqual(_real, other._real)
                && Number.AlmostEqual(_imag, other._imag);
        }

        /// <summary>
        /// Indicates whether <paramref name="other"/> is almost equal to this complex number, up to the provided maximum relative error.
        /// </summary>
        public
        bool
        AlmostEquals(
            Complex other,
            double maximumRelativeError)
        {
            return !IsNaN
                && !other.IsNaN
                && Number.AlmostEqual(_real, other._real, maximumRelativeError)
                && Number.AlmostEqual(_imag, other._imag, maximumRelativeError);
        }

        /// <summary>
        /// Returns true if two complex numbers are almost equal, up to the provided maximum relative error.
        /// </summary>
        public static
        bool
        AlmostEqual(
            Complex x,
            Complex y,
            double maximumRelativeError)
        {
            return x.AlmostEquals(y, maximumRelativeError);
        }

        /// <summary>
        /// Returns true if two complex numbers are almost equal, up to the default maximum relative error.
        /// </summary>
        public static
        bool
        AlmostEqual(
            Complex x,
            Complex y)
        {
            return x.AlmostEquals(y);
        }

        /// <summary>
        /// Gets the hash code of this <c>Complex</c>.
        /// </summary>
        public override
        int
        GetHashCode()
        {
            return _real.GetHashCode() ^ (-_imag.GetHashCode());
        }

        /// <summary>
        /// Compare this complex number with another complex number.
        /// </summary>
        /// <remarks>
        /// The complex number's real part takes precedence over the imaginary part.
        /// </remarks>
        /// <param name="other">The complex number to compare with.</param>
        public
        int
        CompareTo(Complex other)
        {
            int res = Real.CompareTo(other.Real);
            if(res != 0)
            {
                return res;
            }

            return Imag.CompareTo(other.Imag);
        }

        #endregion

        #region Operators

        /// <summary>
        /// Equality test.
        /// </summary>
        public static bool operator ==(Complex complex1, Complex complex2)
        {
            return complex1.Equals(complex2);
        }

        /// <summary>
        /// Inequality test.
        /// </summary>
        public static bool operator !=(Complex complex1, Complex complex2)
        {
            return !complex1.Equals(complex2);
        }

        /// <summary>
        /// Less-than test.
        /// </summary>
        public static bool operator <(Complex complex1, Complex complex2)
        {
            return complex1.CompareTo(complex2) < 0;
        }

        /// <summary>
        /// Greater-than test.
        /// </summary>
        public static bool operator >(Complex complex1, Complex complex2)
        {
            return complex1.CompareTo(complex2) > 0;
        }

        /// <summary>
        /// Less-than-or-equal test.
        /// </summary>
        public static bool operator <=(Complex complex1, Complex complex2)
        {
            return complex1.CompareTo(complex2) <= 0;
        }

        /// <summary>
        /// Greater-than-or-equal test.
        /// </summary>
        public static bool operator >=(Complex complex1, Complex complex2)
        {
            return complex1.CompareTo(complex2) >= 0;
        }

        /// <summary>
        /// Unary addition.
        /// </summary>
        public static Complex operator +(Complex summand)
        {
            return summand;
        }

        /// <summary>
        /// Unary minus.
        /// </summary>
        public static Complex operator -(Complex subtrahend)
        {
            return new Complex(-subtrahend._real, -subtrahend._imag);
        }

        /// <summary>
        /// Complex addition.
        /// </summary>
        public static Complex operator +(Complex summand1, Complex summand2)
        {
            return new Complex(summand1._real + summand2._real, summand1._imag + summand2._imag);
        }

        /// <summary>
        /// Complex subtraction.
        /// </summary>
        public static Complex operator -(Complex minuend, Complex subtrahend)
        {
            return new Complex(minuend._real - subtrahend._real, minuend._imag - subtrahend._imag);
        }

        /// <summary>
        /// Complex addition.
        /// </summary>
        public static Complex operator +(Complex summand1, double summand2)
        {
            return new Complex(summand1._real + summand2, summand1._imag);
        }

        /// <summary>
        /// Complex subtraction.
        /// </summary>
        public static Complex operator -(Complex minuend, double subtrahend)
        {
            return new Complex(minuend._real - subtrahend, minuend._imag);
        }

        /// <summary>
        /// Complex addition.
        /// </summary>
        public static Complex operator +(double summand1, Complex summand2)
        {
            return new Complex(summand2._real + summand1, summand2._imag);
        }

        /// <summary>
        /// Complex subtraction.
        /// </summary>
        public static Complex operator -(double minuend, Complex subtrahend)
        {
            return new Complex(minuend - subtrahend._real, -subtrahend._imag);
        }

        /// <summary>
        /// Complex multiplication.
        /// </summary>
        public static Complex operator *(Complex multiplicand, Complex multiplier)
        {
            return new Complex((multiplicand._real * multiplier._real) - (multiplicand._imag * multiplier._imag), (multiplicand._real * multiplier._imag) + (multiplicand._imag * multiplier._real));
        }

        /// <summary>
        /// Complex multiplication.
        /// </summary>
        public static Complex operator *(double multiplicand, Complex multiplier)
        {
            return new Complex(multiplier._real * multiplicand, multiplier._imag * multiplicand);
        }

        /// <summary>
        /// Complex multiplication.
        /// </summary>
        public static Complex operator *(Complex multiplicand, double multiplier)
        {
            return new Complex(multiplicand._real * multiplier, multiplicand._imag * multiplier);
        }

        /// <summary>
        /// Complex division.
        /// </summary>
        public static Complex operator /(Complex dividend, Complex divisor)
        {
            if(divisor.IsZero)
            {
                return Infinity;
            }

            if(Math.Abs(divisor._real) >= Math.Abs(divisor._imag))
            {
                double r = divisor._imag / divisor._real;
                double den = divisor._real + (r * divisor._imag);
                return new Complex(
                    (dividend._real + (dividend._imag * r)) / den,
                    (dividend._imag - (dividend._real * r)) / den);
            }
            else
            {
                double r = divisor._real / divisor._imag;
                double den = divisor._imag + (r * divisor._real);
                return new Complex(
                    ((dividend._real * r) + dividend._imag) / den,
                    ((dividend._imag * r) - dividend._real) / den);
            }
        }

        /// <summary>
        /// Complex division.
        /// </summary>
        public static Complex operator /(double dividend, Complex divisor)
        {
            if(divisor.IsZero)
            {
                return Infinity;
            }

            if(Math.Abs(divisor._real) >= Math.Abs(divisor._imag))
            {
                double r = divisor._imag / divisor._real;
                double den = divisor._real + (r * divisor._imag);
                return new Complex(
                    dividend / den,
                    -dividend * r / den);
            }
            else
            {
                double r = divisor._real / divisor._imag;
                double den = divisor._imag + (r * divisor._real);
                return new Complex(
                    dividend * r / den,
                    -dividend / den);
            }
        }

        /// <summary>
        /// Complex division.
        /// </summary>
        public static Complex operator /(Complex dividend, double divisor)
        {
            if(Number.AlmostZero(divisor))
            {
                return Infinity;
            }

            return new Complex(dividend._real / divisor, dividend._imag / divisor);
        }

        /// <summary>
        /// Implicit conversion of a real double to a real <c>Complex</c>.
        /// </summary>
        public static implicit operator Complex(double number)
        {
            return new Complex(number, 0d);
        }

        #endregion

        #region Trigonometric Functions

        /// <summary>
        /// Trigonometric Sine (sin, Sinus) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        Sine()
        {
            if(IsReal)
            {
                return new Complex(Trig.Sine(_real), 0d);
            }

            return new Complex(
                Trig.Sine(_real) * Trig.HyperbolicCosine(_imag),
                Trig.Cosine(_real) * Trig.HyperbolicSine(_imag));
        }

        /// <summary>
        /// Trigonometric Cosine (cos, Cosinus) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        Cosine()
        {
            if(IsReal)
            {
                return new Complex(Trig.Cosine(_real), 0d);
            }

            return new Complex(
                Trig.Cosine(_real) * Trig.HyperbolicCosine(_imag),
                -Trig.Sine(_real) * Trig.HyperbolicSine(_imag));
        }

        /// <summary>
        /// Trigonometric Tangent (tan, Tangens) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        Tangent()
        {
            if(IsReal)
            {
                return new Complex(Trig.Tangent(_real), 0d);
            }

            double cosr = Trig.Cosine(_real);
            double sinhi = Trig.HyperbolicSine(_imag);
            double denom = (cosr * cosr) + (sinhi * sinhi);

            return new Complex(
                Trig.Sine(_real) * cosr / denom,
                sinhi * Trig.HyperbolicCosine(_imag) / denom);
        }

        /// <summary>
        /// Trigonometric Cotangent (cot, Cotangens) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        Cotangent()
        {
            if(IsReal)
            {
                return new Complex(Trig.Cotangent(_real), 0d);
            }

            double sinr = Trig.Sine(_real);
            double sinhi = Trig.HyperbolicSine(_imag);
            double denom = (sinr * sinr) + (sinhi * sinhi);

            return new Complex(
                sinr * Trig.Cosine(_real) / denom,
                -sinhi * Trig.HyperbolicCosine(_imag) / denom);
        }

        /// <summary>
        /// Trigonometric Secant (sec, Sekans) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        Secant()
        {
            if(IsReal)
            {
                return new Complex(Trig.Secant(_real), 0d);
            }

            double cosr = Trig.Cosine(_real);
            double sinhi = Trig.HyperbolicSine(_imag);
            double denom = (cosr * cosr) + (sinhi * sinhi);

            return new Complex(
                cosr * Trig.HyperbolicCosine(_imag) / denom,
                Trig.Sine(_real) * sinhi / denom);
        }

        /// <summary>
        /// Trigonometric Cosecant (csc, Cosekans) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        Cosecant()
        {
            if(IsReal)
            {
                return new Complex(Trig.Cosecant(_real), 0d);
            }

            double sinr = Trig.Sine(_real);
            double sinhi = Trig.HyperbolicSine(_imag);
            double denom = (sinr * sinr) + (sinhi * sinhi);

            return new Complex(
                sinr * Trig.HyperbolicCosine(_imag) / denom,
                -Trig.Cosine(_real) * sinhi / denom);
        }

        #endregion

        #region Trigonometric Arcus Functions

        /// <summary>
        /// Trigonometric Arcus Sine (asin, Arkussinus) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        InverseSine()
        {
            return -I * ((1 - Square()).SquareRoot() + (I * this)).NaturalLogarithm();
        }

        /// <summary>
        /// Trigonometric Arcus Cosine (acos, Arkuscosinus) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        InverseCosine()
        {
            return -I * (this + (I * (1 - Square()).SquareRoot())).NaturalLogarithm();
        }

        /// <summary>
        /// Trigonometric Arcus Tangent (atan, Arkustangens) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        InverseTangent()
        {
            Complex iz = new Complex(-_imag, _real); // I*this
            return new Complex(0, 0.5) * ((1 - iz).NaturalLogarithm() - (1 + iz).NaturalLogarithm());
        }

        /// <summary>
        /// Trigonometric Arcus Cotangent (acot, Arkuscotangens) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        InverseCotangent()
        {
            Complex iz = new Complex(0, 1) / this;
            return new Complex(0, 0.5) * ((1 - iz).NaturalLogarithm() - (1 + iz).NaturalLogarithm());
        }

        /// <summary>
        /// Trigonometric Arcus Secant (asec, Arkussekans) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        InverseSecant()
        {
            Complex inv = 1 / this;
            return -I * (inv + (I * (1 - inv.Square()).SquareRoot())).NaturalLogarithm();
        }

        /// <summary>
        /// Trigonometric Arcus Cosecant (acsc, Arkuscosekans) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        InverseCosecant()
        {
            Complex inv = 1 / this;
            return -I * ((I * inv) + (1 - inv.Square()).SquareRoot()).NaturalLogarithm();
        }

        #endregion

        #region Trigonometric Hyperbolic Functions

        /// <summary>
        /// Trigonometric Hyperbolic Sine (sinh, Sinus hyperbolicus) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        HyperbolicSine()
        {
            if(IsReal)
            {
                return new Complex(Trig.HyperbolicSine(_real), 0d);
            }

            return new Complex(
                Trig.HyperbolicSine(_real) * Trig.Cosine(_imag),
                Trig.HyperbolicCosine(_real) * Trig.Sine(_imag));
        }

        /// <summary>
        /// Trigonometric Hyperbolic Cosine (cosh, Cosinus hyperbolicus) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        HyperbolicCosine()
        {
            if(IsReal)
            {
                return new Complex(Trig.HyperbolicCosine(_real), 0d);
            }

            return new Complex(
                Trig.HyperbolicCosine(_real) * Trig.Cosine(_imag),
                Trig.HyperbolicSine(_real) * Trig.Sine(_imag));
        }

        /// <summary>
        /// Trigonometric Hyperbolic Tangent (tanh, Tangens hyperbolicus) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        HyperbolicTangent()
        {
            if(IsReal)
            {
                return new Complex(Trig.HyperbolicTangent(_real), 0d);
            }

            double cosi = Trig.Cosine(_imag);
            double sinhr = Trig.HyperbolicSine(_real);
            double denom = (cosi * cosi) + (sinhr * sinhr);

            return new Complex(
                Trig.HyperbolicCosine(_real) * sinhr / denom,
                cosi * Trig.Sine(_imag) / denom);
        }

        /// <summary>
        /// Trigonometric Hyperbolic Cotangent (coth, Cotangens hyperbolicus) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        HyperbolicCotangent()
        {
            if(IsReal)
            {
                return new Complex(Trig.HyperbolicCotangent(_real), 0d);
            }

            double sini = Trig.Sine(_imag);
            double sinhr = Trig.HyperbolicSine(_real);
            double denom = (sini * sini) + (sinhr * sinhr);

            return new Complex(
                sinhr * Trig.HyperbolicCosine(_real) / denom,
                sini * Trig.Cosine(_imag) / denom);
        }

        /// <summary>
        /// Trigonometric Hyperbolic Secant (sech, Secans hyperbolicus) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        HyperbolicSecant()
        {
            if(IsReal)
            {
                return new Complex(Trig.HyperbolicSecant(_real), 0d);
            }

            Complex exp = Exponential();
            return 2 * exp / (exp.Square() + 1);
        }

        /// <summary>
        /// Trigonometric Hyperbolic Cosecant (csch, Cosecans hyperbolicus) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        HyperbolicCosecant()
        {
            if(IsReal)
            {
                return new Complex(Trig.HyperbolicCosecant(_real), 0d);
            }

            Complex exp = Exponential();
            return 2 * exp / (exp.Square() - 1);
        }

        #endregion

        #region Trigonometric Hyperbolic Area Functions

        /// <summary>
        /// Trigonometric Hyperbolic Area Sine (asinh, reasinus hyperbolicus) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        InverseHyperbolicSine()
        {
            return (this + (Square() + 1).SquareRoot()).NaturalLogarithm();
        }

        /// <summary>
        /// Trigonometric Hyperbolic Area Cosine (acosh, Areacosinus hyperbolicus) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        InverseHyperbolicCosine()
        {
            return (this + ((this - 1).SquareRoot() * (this + 1).SquareRoot())).NaturalLogarithm();
        }

        /// <summary>
        /// Trigonometric Hyperbolic Area Tangent (atanh, Areatangens hyperbolicus) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        InverseHyperbolicTangent()
        {
            return 0.5 * ((1 + this).NaturalLogarithm() - (1 - this).NaturalLogarithm());
        }

        /// <summary>
        /// Trigonometric Hyperbolic Area Cotangent (acoth, Areacotangens hyperbolicus) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        InverseHyperbolicCotangent()
        {
            return 0.5 * ((this + 1).NaturalLogarithm() - (this - 1).NaturalLogarithm());
        }

        /// <summary>
        /// Trigonometric Hyperbolic Area Secant (asech, Areasekans hyperbolicus) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        InverseHyperbolicSecant()
        {
            Complex inv = 1 / this;
            return (inv + ((inv - 1).SquareRoot() * (inv + 1).SquareRoot())).NaturalLogarithm();
        }

        /// <summary>
        /// Trigonometric Hyperbolic Area Cosecant (acsch, Areacosekans hyperbolicus) of this <c>Complex</c>.
        /// </summary>
        public
        Complex
        InverseHyperbolicCosecant()
        {
            Complex inv = 1 / this;
            return (inv + (inv.Square() + 1).SquareRoot()).NaturalLogarithm();
        }

        #endregion

        #region Exponential Functions

        /// <summary>
        /// Exponential of this <c>Complex</c> (exp(x), E^x).
        /// </summary>
        public
        Complex
        Exponential()
        {
            double exp = Math.Exp(_real);
            if(IsReal)
            {
                return new Complex(exp, 0d);
            }

            return new Complex(
                exp * Trig.Cosine(_imag),
                exp * Trig.Sine(_imag));
        }

        /// <summary>
        /// Natural Logarithm of this <c>Complex</c> (Base E).
        /// </summary>
        public
        Complex
        NaturalLogarithm()
        {
            if(IsRealNonNegative)
            {
                return new Complex(Math.Log(_real), 0d);
            }

            return new Complex(
                0.5d * Math.Log(ModulusSquared),
                Argument);
        }

        /// <summary>
        /// Raise this <c>Complex</c> to the given value.
        /// </summary>
        public
        Complex
        Power(Complex exponent)
        {
            if(IsZero)
            {
                if(exponent.IsZero)
                {
                    return One;
                }

                if(exponent.Real > 0)
                {
                    return Zero;
                }

                if(exponent.Real < 0)
                {
                    if(Number.AlmostEqual(0d, exponent.Imag))
                    {
                        return new Complex(double.PositiveInfinity, 0d);
                    }

                    return new Complex(double.PositiveInfinity, double.PositiveInfinity);
                }

                return NaN;
            }

            return (exponent * NaturalLogarithm()).Exponential();
        }

        /// <summary>
        /// Raise this <c>Complex</c> to the inverse of the given value.
        /// </summary>
        public
        Complex
        Root(Complex rootExponent)
        {
            return Power(1 / rootExponent);
        }

        /// <summary>
        /// The Square (power 2) of this <c>Complex</c>
        /// </summary>
        public
        Complex
        Square()
        {
            if(IsReal)
            {
                return new Complex(_real * _real, 0d);
            }

            return new Complex(
                (_real * _real) - (_imag * _imag),
                2 * _real * _imag);
        }

        /// <summary>
        /// The Square Root (power 1/2) of this <c>Complex</c>
        /// </summary>
        public
        Complex
        SquareRoot()
        {
            if(IsRealNonNegative)
            {
                return new Complex(Math.Sqrt(_real), 0d);
            }

            double mod = Modulus;

            if(_imag > 0 || (_imag == 0 && _real < 0))
            {
                return new Complex(
                    Constants.Sqrt1_2 * Math.Sqrt(mod + _real),
                    Constants.Sqrt1_2 * Math.Sqrt(mod - _real));
            }

            return new Complex(
                Constants.Sqrt1_2 * Math.Sqrt(mod + _real),
                -Constants.Sqrt1_2 * Math.Sqrt(mod - _real));
        }

        #endregion

        #region ToString and Parse

        /// <summary>
        /// Parse a string into a <c>Complex</c>.
        /// </summary>
        /// <remarks>
        /// The adopted string representation for the complex numbers is 
        /// <i>UVW+I*XYZ</i> where <i>UVW</i> and <i>XYZ</i> are <c>double</c> 
        /// strings. Some alternative representations are <i>UVW+XYZi</i>,
        /// <i>UVW+iXYZ</i>, <i>UVW</i> and <i>iXYZ</i>. 
        /// Additionally the string <c>"NaN"</c> is mapped to 
        /// <c>Complex.NaN</c>, the string <c>"Infinity"</c> to 
        /// <c>Complex.ComplexInfinity</c>, <c>"PositiveInfinity"</c>
        /// to <c>Complex.DirectedInfinity(Complex.One)</c>,
        /// <c>"NegativeInfinity"</c> to <c>Complex.DirectedInfinity(-Complex.One)</c>
        /// and finally <c>"DirectedInfinity(WVW+I*XYZ)"</c> to <c>Complex.DirectedInfinity(WVW+I*XYZ)</c>.
        /// <code>
        /// Complex z = Complex.Parse("12.5+I*7");
        /// Complex nan = Complex.Parse("NaN");
        /// Complex infinity = Complex.Parse("Infinity");
        /// </code>
        /// This method is symmetric to <see cref="Complex.ToString()"/>.
        /// </remarks>
        public static Complex Parse(string complex)
        {
            return Parse(complex, NumberFormatInfo.CurrentInfo);
        }

        /// <summary>
        /// Parse a string into a <c>Complex</c>.
        /// </summary>
        public static Complex Parse(string complex, NumberFormatInfo numberFormat)
        {
            ComplexParser parser = new ComplexParser(complex, numberFormat);
            return parser.Complex;
        }

        /// <summary>
        /// Formats this <c>Complex</c> into a <c>string</c>.
        /// </summary>
        public override string ToString()
        {
            return ToString(NumberFormatInfo.CurrentInfo);
        }

        /// <summary>
        /// Formats this <c>Complex</c> into a <c>string</c>.
        /// </summary>
        public string ToString(NumberFormatInfo numberFormat)
        {
            if(IsInfinity)
            {
                return "Infinity";
            }

            if(IsNaN)
            {
                return numberFormat.NaNSymbol;
            }

            if(IsReal)
            {
                return _real.ToString(numberFormat);
            }

            // note: there's a difference between the negative sign and the subtraction operator!
            if(IsImaginary)
            {
                if(Number.AlmostEqual(_imag, 1))
                {
                    return "I";
                }

                if(Number.AlmostEqual(_imag, -1))
                {
                    return numberFormat.NegativeSign + "I";
                }

                if(_imag < 0)
                {
                    return numberFormat.NegativeSign + "I*" + (-_imag).ToString(numberFormat);
                }

                return "I*" + _imag.ToString(numberFormat);
            }

            if(Number.AlmostEqual(_imag, 1))
            {
                return _real.ToString(numberFormat) + "+I";
            }

            if(Number.AlmostEqual(_imag, -1))
            {
                return _real.ToString(numberFormat) + "-I";
            }

            if(_imag < 0)
            {
                return _real.ToString(numberFormat) + "-I*"
                       + (-_imag).ToString(numberFormat);
            }

            return _real + "+I*" + _imag.ToString(numberFormat);
        }

        private sealed class ComplexParser
        {
            Complex _complex;
            int _cursor; // = 0;
            readonly string _source;
            readonly NumberFormatInfo _numberFormat;

            public
            ComplexParser(
                string complex,
                NumberFormatInfo numberFormat)
            {
                _numberFormat = numberFormat;
                _source = complex.ToLower().Trim();
                _complex = ScanComplex();
            }

            #region Infrastructure

            char
            Consume()
            {
                return _source[_cursor++];
            }

            char LookAheadCharacterOrNull
            {
                get
                {
                    if(_cursor >= _source.Length)
                    {
                        return '\0';
                    }
                    
                    return _source[_cursor];
                }
            }

            char LookAheadCharacter
            {
                get
                {
                    if(_cursor >= _source.Length)
                    {
                        throw new ArgumentException(Properties.LocalStrings.ArgumentParseComplexNumber, "complex");
                    }
                    
                    return _source[_cursor];
                }
            }

            #endregion

            #region Scanners

            Complex
            ScanComplex()
            {
                if(_source.Equals("i"))
                {
                    return I;
                }

                if(_source.Equals(_numberFormat.NaNSymbol.ToLower()))
                {
                    return NaN;
                }

                if(_source.Equals("infinity") || _source.Equals("infty"))
                {
                    return Infinity;
                }

                ScanSkipWhitespace();
                Complex complex = ScanSignedComplexNumberPart();
                ScanSkipWhitespace();

                if(IsSign(LookAheadCharacterOrNull))
                {
                    complex += ScanSignedComplexNumberPart();
                }

                return complex;
            }

            Complex
            ScanSignedComplexNumberPart()
            {
                bool negativeSign = false;

                if(IsSign(LookAheadCharacterOrNull))
                {
                    if(IsNegativeSign(LookAheadCharacter))
                    {
                        negativeSign = true;
                    }

                    Consume();
                    ScanSkipWhitespace();
                }

                if(negativeSign)
                {
                    return -ScanComplexNumberPart();
                }

                return ScanComplexNumberPart();
            }

            Complex
            ScanComplexNumberPart()
            {
                bool imaginary = false;

                if(IsI(LookAheadCharacter))
                {
                    Consume();
                    ScanSkipWhitespace();

                    if(IsMult(LookAheadCharacterOrNull))
                    {
                        Consume();
                    }

                    ScanSkipWhitespace();
                    imaginary = true;
                }

                if(!IsNumber(LookAheadCharacterOrNull) && !IsSign(LookAheadCharacterOrNull))
                {
                    return new Complex(0d, 1d);
                }

                double part = ScanNumber();
                ScanSkipWhitespace();

                if(IsMult(LookAheadCharacterOrNull))
                {
                    Consume();
                    ScanSkipWhitespace();
                }

                if(IsI(LookAheadCharacterOrNull))
                {
                    Consume();
                    ScanSkipWhitespace();
                    imaginary = true;
                }

                if(imaginary)
                {
                    return new Complex(0d, part);
                }

                return new Complex(part, 0d);
            }

            double
            ScanNumber()
            {
                StringBuilder sb = new StringBuilder();

                if(IsSign(LookAheadCharacter))
                {
                    sb.Append(Consume());
                }

                ScanSkipWhitespace();
                ScanInteger(sb);
                ScanSkipWhitespace();

                if(IsDecimal(LookAheadCharacterOrNull))
                {
                    Consume();
                    sb.Append(_numberFormat.NumberDecimalSeparator);
                    ScanInteger(sb);
                }

                if(IsE(LookAheadCharacterOrNull))
                {
                    Consume();
                    sb.Append('e');

                    if(IsSign(LookAheadCharacter))
                    {
                        sb.Append(Consume());
                    }

                    ScanInteger(sb);
                }

                return double.Parse(
                    sb.ToString(),
                    NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.AllowLeadingSign,
                    _numberFormat);
            }

            void
            ScanInteger(StringBuilder sb)
            {
                sb.Append(Consume());
                while(IsNumber(LookAheadCharacterOrNull) || IsGroup(LookAheadCharacterOrNull))
                {
                    char c = Consume();
                    if(!IsGroup(c))
                    {
                        sb.Append(c);
                    }
                }
            }

            void
            ScanSkipWhitespace()
            {
                while(_cursor < _source.Length && !IsNotWhiteSpace(LookAheadCharacter))
                {
                    Consume();
                }
            }

            #endregion

            #region Indicators

            bool
            IsNotWhiteSpace(char c)
            {
                return IsNumber(c) || IsDecimal(c) || IsE(c) || IsI(c) || IsSign(c) || IsMult(c);
            }
            
            static
            bool
            IsNumber(char c)
            {
                // TODO: consider using _numberFormat.NativeDigits
                return c >= '0' && c <= '9';
            }
            
            bool
            IsDecimal(char c)
            {
                return _numberFormat.NumberDecimalSeparator.Equals(c.ToString());
            }

            bool
            IsGroup(char c)
            {
                return _numberFormat.NumberGroupSeparator.Equals(c.ToString());
            }

            static
            bool
            IsE(char c)
            {
                return c == 'e';
            }

            static
            bool
            IsI(char c)
            {
                return c == 'i' || c == 'j';
            }

            bool
            IsSign(char c)
            {
                return _numberFormat.PositiveSign.Equals(c.ToString()) || _numberFormat.NegativeSign.Equals(c.ToString());
            }

            bool
            IsNegativeSign(char c)
            {
                return _numberFormat.NegativeSign.Equals(c.ToString());
            }

            static
            bool
            IsMult(char c)
            {
                return c == '*';
            }

            #endregion

            public Complex Complex
            {
                get { return _complex; }
            }

            public double Real
            {
                get { return _complex._real; }
            }

            public double Imaginary
            {
                get { return _complex._imag; }
            }
        }

        #endregion
    }
}
