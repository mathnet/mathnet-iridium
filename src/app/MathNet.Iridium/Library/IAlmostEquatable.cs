//-----------------------------------------------------------------------
// <copyright file="IAlmostEquatable.cs" company="Math.NET Project">
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
    /// Almost-Equality support, similar to the native <see cref="IEquatable{T}"/> but
    /// considering a maximum relative error.
    /// </summary>
    public interface IAlmostEquatable<T>
    {
        /// <summary>
        /// True if two instances of T are almost equal, up to the default maximum relative error.
        /// </summary>
        bool AlmostEquals(T other);

        /// <summary>
        /// True if two instances of T are almost equal, up to the provided maximum relative error.
        /// </summary>
        bool AlmostEquals(T other, double maximumRelativeError);
    }
}
