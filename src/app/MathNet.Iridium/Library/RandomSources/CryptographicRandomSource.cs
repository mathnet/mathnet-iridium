//-----------------------------------------------------------------------
// <copyright file="CryptographicRandomSource.cs" company="Math.NET Project">
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
using System.Security.Cryptography;

namespace MathNet.Numerics.RandomSources
{
    using ObjectModel;

    /// <summary>
    /// Represents a cryptographically strong pseudo-random number generator.
    /// </summary>
    /// <remarks>
    /// The <see cref="CryptographicRandomSource"/> type internally uses an instance of the
    /// <see cref="System.Security.Cryptography.RNGCryptoServiceProvider"/> type to generate pseudo-random numbers.
    /// </remarks>
    public class CryptographicRandomSource :
        RandomByteSourceAdapter
    {
        /// <summary>
        /// Stores an instance of <see cref="System.Random"/> type that is used to generate random numbers.
        /// </summary>
        readonly RNGCryptoServiceProvider _generator;

        /// <summary>
        /// Initializes a new instance of the <see cref="CryptographicRandomSource"/> class.
        /// </summary>
        public
        CryptographicRandomSource()
        {
            _generator = new RNGCryptoServiceProvider();
        }

        /// <summary>
        /// Fills the elements of a specified array of bytes with random numbers. 
        /// </summary>
        /// <remarks>
        /// Each element of the array of bytes is set to a random number greater than or equal to zero, and less than or 
        ///   equal to <see cref="Byte.MaxValue"/>.
        /// </remarks>
        /// <param name="buffer">An array of bytes to contain random numbers.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="buffer"/> is a null reference (<see langword="Nothing"/> in Visual Basic). 
        /// </exception>
        public override
        void
        NextBytes(byte[] buffer)
        {
            _generator.GetBytes(buffer);
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        public override
        void
        Reset()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="CryptographicRandomSource"/> can be reset, so that it produces the 
        /// same pseudo-random number sequence again.
        /// </summary>
        public override bool CanReset
        {
            get { return false; }
        }
    }
}
