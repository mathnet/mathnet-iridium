//-----------------------------------------------------------------------
// <copyright file="Rational.cs" company="Math.NET Project">
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

namespace MathNet.Numerics
{
    /// <summary>
    /// Rational represents a finite order rational with positive
    /// powers and constant real coefficients for both numerator and denominator.
    /// </summary>
    public class Rational : IComparable, ICloneable
    {
        Polynomial _numerator;
        Polynomial _denominator;

        // TODO: Implement polynomial factorization to normalize rationals

        /// <summary>
        /// Initializes a new instance of the Rational class,
        /// by directly referencing the two provided polynomials (no deep copy).
        /// </summary>
        public
        Rational(
            Polynomial numerator,
            Polynomial denominator)
        {
            _numerator = numerator.Clone();
            _denominator = denominator.Clone();
        }

        /// <summary>
        /// Initializes a new instance of the Rational class,
        /// with the provided numerator and denominator order.
        /// </summary>
        public
        Rational(
            int numeratorOrder,
            int denominatorOrder)
        {
            _numerator = new Polynomial(numeratorOrder);
            _denominator = new Polynomial(denominatorOrder);
        }

        /// <summary>
        /// Initializes a new instance of the Rational class,
        /// by directly referencing the two provided polynomial coefficients.
        /// </summary>
        public
        Rational(
            double[] numeratorCoefficients,
            double[] denominatorCoefficients)
        {
            _numerator = new Polynomial(numeratorCoefficients);
            _denominator = new Polynomial(denominatorCoefficients);
        }

        /// <summary>
        /// Initializes a new instance of the Rational class,
        /// by deep-copy from an existing rational.
        /// </summary>
        /// <param name="copy">A rational to copy from.</param>
        public
        Rational(Rational copy)
        {
            _numerator = new Polynomial(copy._numerator);
            _denominator = new Polynomial(copy._denominator);
        }

        #region Accessors

        /// <summary>
        /// Normalize both numerator and denominator polynomials.
        /// </summary>
        public
        void
        Normalize()
        {
            _numerator.Normalize();
            _denominator.Normalize();
        }

        /// <summary>
        /// The numerator polynomial.
        /// </summary>
        public Polynomial Numerator
        {
            get { return _numerator; }
            set { _numerator = value; }
        }

        /// <summary>
        /// The denominator polynomial.
        /// </summary>
        public Polynomial Denominator
        {
            get { return _denominator; }
            set { _denominator = value; }
        }

        /// <summary>
        /// The order of the numerator polynomial.
        /// </summary>
        public int NumeratorOrder
        {
            get { return _numerator.Order; }
        }

        /// <summary>
        /// The order of the denominator polynomial.
        /// </summary>
        public int DenominatorOrder
        {
            get { return _denominator.Order; }
        }

        #endregion

        #region Operators

        /// <summary>
        /// Check whether two rationals have the same coefficients.
        /// </summary>
        public static
        bool
        operator ==(
            Rational rational1,
            Rational rational2)
        {
            return rational1.Equals(rational2);
        }

        /// <summary>
        /// Check whether two rationals have different coefficients.
        /// </summary>
        public static
        bool
        operator !=(
            Rational rational1,
            Rational rational2)
        {
            return !rational1.Equals(rational2);
        }

        /// <summary>
        /// Check whether a rational is bigger than another rational.
        /// </summary>
        public static
        bool
        operator >(
            Rational rational1,
            Rational rational2)
        {
            return rational1.CompareTo(rational2) == 1;
        }

        /// <summary>
        /// Check whether a rational is smaller than another rational.
        /// </summary>
        public static
        bool
        operator <(
            Rational rational1,
            Rational rational2)
        {
            return rational1.CompareTo(rational2) == -1;
        }

        /// <summary>
        /// Check whether a rational is bigger than or equal to another rational.
        /// </summary>
        public static
        bool
        operator >=(
            Rational rational1,
            Rational rational2)
        {
            int res = rational1.CompareTo(rational2);
            return res == 1 || res == 0;
        }

        /// <summary>
        /// Check whether a rational is smaller than or equal to another rational.
        /// </summary>
        public static
        bool
        operator <=(
            Rational rational1,
            Rational rational2)
        {
            int res = rational1.CompareTo(rational2);
            return res == -1 || res == 0;
        }

        /// <summary>
        /// Add a rational to a rational.
        /// </summary>
        public static
        Rational
        operator +(
            Rational rational1,
            Rational rational2)
        {
            return rational1.Add(rational2);
        }

        /// <summary>
        /// Add a polynomial to a rational.
        /// </summary>
        public static
        Rational
        operator +(
            Rational rational,
            Polynomial polynomial)
        {
            Rational ret = new Rational(rational);
            ret.AddInplace(polynomial);
            return ret;
        }

        /// <summary>
        /// Add a rational to a polynomial.
        /// </summary>
        public static
        Rational
        operator +(
            Polynomial polynomial,
            Rational rational)
        {
            Rational ret = new Rational(rational);
            ret.AddInplace(polynomial);
            return ret;
        }

        /// <summary>
        /// Add a real number to a rational.
        /// </summary>
        public static
        Rational
        operator +(
            Rational rational,
            double n)
        {
            Rational ret = new Rational(rational);
            ret.AddInplace(n);
            return ret;
        }

        /// <summary>
        /// Add a rational to a real number.
        /// </summary>
        public static
        Rational
        operator +(
            double n,
            Rational rational)
        {
            Rational ret = new Rational(rational);
            ret.AddInplace(n);
            return ret;
        }

        /// <summary>
        /// (nop)
        /// </summary>
        public static
        Rational
        operator +(Rational rational)
        {
            return rational;
        }

        /// <summary>
        /// Subtract a rational from a rational.
        /// </summary>
        public static
        Rational
        operator -(
            Rational rational1,
            Rational rational2)
        {
            return rational1.Subtract(rational2);
        }

        /// <summary>
        /// Subtract a polynomial from a rational.
        /// </summary>
        public static
        Rational
        operator -(
            Rational rational,
            Polynomial polynomial)
        {
            Rational ret = new Rational(rational);
            ret.SubtractInplace(polynomial);
            return ret;
        }

        /// <summary>
        /// Subtract a rational from a polynomial.
        /// </summary>
        public static
        Rational
        operator -(
            Polynomial polynomial,
            Rational rational)
        {
            Rational ret = new Rational(rational);
            ret.NegateInplace();
            ret.AddInplace(polynomial);
            return ret;
        }

        /// <summary>
        /// Subtract a real number from a rational.
        /// </summary>
        public static
        Rational
        operator -(
            Rational rational,
            double n)
        {
            Rational ret = new Rational(rational);
            ret.SubtractInplace(n);
            return ret;
        }

        /// <summary>
        /// Subtract a rational from a real number.
        /// </summary>
        public static
        Rational
        operator -(
            double n,
            Rational rational)
        {
            Rational ret = new Rational(rational);
            ret.NegateInplace();
            ret.AddInplace(n);
            return ret;
        }

        /// <summary>
        /// Negate a rational.
        /// </summary>
        public static
        Rational
        operator -(Rational rational)
        {
            Rational ret = new Rational(rational);
            ret.NegateInplace();
            return ret;
        }

        /// <summary>
        /// Multiply/Convolute two rationals.
        /// </summary>
        public static
        Rational
        operator *(
            Rational rational1,
            Rational rational2)
        {
            return rational1.Multiply(rational2);
        }

        /// <summary>
        /// Multiply/Convolute a rationals with a polynomial.
        /// </summary>
        public static
        Rational
        operator *(
            Rational rational,
            Polynomial polynomial)
        {
            return rational.Multiply(polynomial);
        }

        /// <summary>
        /// Multiply/Convolute a polynomial with a rational.
        /// </summary>
        public static
        Rational
        operator *(
            Polynomial polynomial,
            Rational rational)
        {
            return rational.Multiply(polynomial);
        }

        /// <summary>
        /// Stretch a rational with a real number factor.
        /// </summary>
        public static
        Rational
        operator *(
            Rational rational,
            double n)
        {
            Rational ret = new Rational(rational);
            ret.MultiplyInplace(n);
            return ret;
        }

        /// <summary>
        /// Stretch a polynomial with a real number factor.
        /// </summary>
        public static
        Rational
        operator *(
            double n,
            Rational rational)
        {
            Rational ret = new Rational(rational);
            ret.MultiplyInplace(n);
            return ret;
        }

        /// <summary>
        /// Divide two rationals.
        /// </summary>
        public static
        Rational
        operator /(
            Rational rational1,
            Rational rational2)
        {
            return rational1.Divide(rational2);
        }

        /// <summary>
        /// Divide a rational to a polynomial.
        /// </summary>
        public static
        Rational
        operator /(
            Rational rational,
            Polynomial polynomial)
        {
            return rational.Divide(polynomial);
        }

        /// <summary>
        /// Divide a polynomial to a rational.
        /// </summary>
        public static
        Rational
        operator /(
            Polynomial polynomial,
            Rational rational)
        {
            Rational ret = rational.Divide(polynomial);
            ret.InvertInplace();
            return ret;
        }

        /// <summary>
        /// Stretch a rational with a real number quotient.
        /// </summary>
        public static
        Rational
        operator /(
            Rational rational,
            double n)
        {
            Rational ret = new Rational(rational);
            ret.DivideInplace(n);
            return ret;
        }

        /// <summary>
        /// Stretch a the inverse of a rational with a real number quotient.
        /// </summary>
        public static
        Rational
        operator /(
            double n,
            Rational rational)
        {
            Rational ret = new Rational(rational);
            ret.InvertInplace();
            ret.MultiplyInplace(n);
            return ret;
        }

        #endregion

        #region Inplace Arithmetic Methods

        /// <summary>
        /// Create a new rational as the result of adding a rational to this rational.
        /// </summary>
        /// <param name="rational">The rational to add</param>
        public
        Rational
        Add(Rational rational)
        {
            if(_denominator.Equals(rational._denominator))
            {
                return new Rational(
                    _numerator + rational._numerator,
                    _denominator.Clone());
            }

            Polynomial num = (_numerator * rational._denominator) + (rational._numerator * _denominator);
            Polynomial denom = _denominator * rational._denominator;
            return new Rational(num, denom);
        }

        /// <summary>
        /// Add a polynomial directly inplace to this rational.
        /// </summary>
        /// <param name="polynomial">The polynomial to add.</param>
        public
        void
        AddInplace(Polynomial polynomial)
        {
            _numerator.AddInplace(_denominator * polynomial);
        }

        /// <summary>
        /// Add a floating point number directly inplace to this rational.
        /// </summary>
        /// <param name="n">The floating point number to add.</param>
        public
        void
        AddInplace(double n)
        {
            _numerator.AddInplace(_denominator * n);
        }

        /// <summary>
        /// Create a new rational as the result of subtracting a rational from this rational.
        /// </summary>
        /// <param name="rational">The rational to subtract.</param>
        public
        Rational
        Subtract(Rational rational)
        {
            if(_denominator.Equals(rational._denominator))
            {
                return new Rational(
                    _numerator - rational._numerator,
                    _denominator.Clone());
            }

            Polynomial num = (_numerator * rational._denominator) - (rational._numerator * _denominator);
            Polynomial denom = _denominator * rational._denominator;
            return new Rational(num, denom);
        }

        /// <summary>
        /// Subtract a polynomial directly inplace from this rational.
        /// </summary>
        /// <param name="polynomial">The polynomial to subtract.</param>
        public
        void
        SubtractInplace(Polynomial polynomial)
        {
            _numerator.SubtractInplace(_denominator * polynomial);
        }

        /// <summary>
        /// Subtract a floating point number directly inplace from this rational.
        /// </summary>
        /// <param name="n">The floating point number to subtract.</param>
        public
        void
        SubtractInplace(double n)
        {
            _numerator.SubtractInplace(_denominator * n);
        }

        /// <summary>
        /// Negate this rational directly inplace.
        /// </summary>
        public
        void
        NegateInplace()
        {
            _numerator.NegateInplace();
        }

        /// <summary>
        /// Create a new rational as the result of multiplying a rational to this rational.
        /// </summary>
        /// <param name="rational">The rational to multiply with.</param>
        public
        Rational
        Multiply(Rational rational)
        {
            return new Rational(
                _numerator * rational._numerator,
                _denominator * rational._denominator);
        }

        /// <summary>
        /// Create a new rational as the result of multiplying a polynomial to this rational.
        /// </summary>
        /// <param name="polynomial">The polynomial to multiply with.</param>
        public
        Rational
        Multiply(Polynomial polynomial)
        {
            return new Rational(
                _numerator * polynomial,
                _denominator.Clone());
        }

        /// <summary>
        /// Multiply a floating point number directly inplace to this rational.
        /// </summary>
        /// <param name="n">The floating point number to multiply with.</param>
        public
        void
        MultiplyInplace(double n)
        {
            _numerator.MultiplyInplace(n);
        }

        /// <summary>
        /// Create a new rational as the result of dividing a rational from this rational.
        /// </summary>
        /// <param name="rational">The rational to divide with.</param>
        public
        Rational
        Divide(Rational rational)
        {
            return new Rational(
                _numerator * rational._denominator,
                _denominator * rational._numerator);
        }

        /// <summary>
        /// Create a new rational as the result of dividing a polynomial from this rational.
        /// </summary>
        /// <param name="polynomial">The polynomial to divide with.</param>
        public
        Rational
        Divide(Polynomial polynomial)
        {
            return new Rational(
                _numerator.Clone(),
                _denominator * polynomial);
        }

        /// <summary>
        /// Divide a floating point number directly inplace from this rational.
        /// </summary>
        /// <param name="n">The floating point number to divide with.</param>
        public
        void
        DivideInplace(double n)
        {
            _denominator.MultiplyInplace(n);
        }

        /// <summary>
        /// Invert this rational directly inplace.
        /// </summary>
        public
        void
        InvertInplace()
        {
            Polynomial temp = _denominator;
            _denominator = _numerator;
            _numerator = temp;
        }

        #endregion

        #region Evaluation

        /// <summary>
        /// Evaluate the value of this rational at a given point.
        /// </summary>
        /// <param name="value">The point where to evaluate the rational</param>
        public
        double
        Evaluate(double value)
        {
            /*
            TODO: correct Rational.Evaluate implementation
            the formula here below is accurate iff the Rational cannot be 'simplified'
            */

            return _numerator.Evaluate(value) / _denominator.Evaluate(value);
        }

        #endregion

        #region String Formatting and Parsing

        /// <summary>
        /// Format a human-readable string of this rational with the given string as base variable (e.g. "x").
        /// </summary>
        public
        string
        ToString(string baseVariable)
        {
            return "(" + _numerator.ToString(baseVariable) + ")/(" + _denominator.ToString(baseVariable) + ")";
        }

        /// <summary>
        /// Format a human-readable string of this rational with "x" as base variable.
        /// </summary>
        public override
        string
        ToString()
        {
            return ToString("x");
        }

        #endregion

        #region .NET Integration: Hashing, Equality, Ordering, Cloning

        /// <summary>
        /// Serves as a hash function for rationals.
        /// </summary>
        public override
        int
        GetHashCode()
        {
            return _numerator.GetHashCode() ^ _denominator.GetHashCode();
        }

        /// <summary>
        /// Check whether this rational is equal to another rational.
        /// </summary>
        public override
        bool
        Equals(object obj)
        {
            if(obj == null || !(obj is Rational))
            {
                return false;
            }

            return Equals((Rational)obj);
        }

        /// <summary>
        /// Check whether this rational is equal to another rational.
        /// </summary>
        public
        bool
        Equals(Rational rational)
        {
            return _numerator.Equals(rational._numerator)
                && _denominator.Equals(rational._denominator);
        }

        /// <summary>
        /// Check whether two rationals are equal.
        /// </summary>
        public static
        bool
        Equals(
            Rational rational1,
            Rational rational2)
        {
            return rational1.Equals(rational2);
        }

        /// <summary>
        /// Compare this rational to another rational.
        /// </summary>
        int
        IComparable.CompareTo(object obj)
        {
            if(obj == null)
            {
                return 1;
            }

            if(!(obj is Rational))
            {
                throw new ArgumentException(Properties.LocalStrings.ArgumentTypeMismatch, "obj");
            }

            return CompareTo((Rational)obj);
        }

        /// <summary>
        /// Compare this rational to another rational.
        /// </summary>
        public
        int
        CompareTo(Rational rational)
        {
            int n = _numerator.CompareTo(rational._numerator);
            if(n == 0)
            {
                n = _denominator.CompareTo(rational._denominator);
            }

            return n;
        }

        /// <summary>
        /// Create a copy of this rational.
        /// </summary>
        object
        ICloneable.Clone()
        {
            return Clone();
        }

        /// <summary>
        /// Create a copy of this rational.
        /// </summary>
        public
        Rational
        Clone()
        {
            return new Rational(this);
        }

        #endregion
    }
}
