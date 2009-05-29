//-----------------------------------------------------------------------
// <copyright file="Searching.cs" company="Math.NET Project">
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

namespace MathNet.Numerics
{
    /// <summary>
    /// Searching Algorithms
    /// </summary>
    public static class Searching
    {
        /// <summary>
        /// In an already sorted list, search an item by binary search. If the item is not
        /// found, a negative number is returned.
        /// </summary>
        public static
        int
        BinarySearch<T>(IList<T> sortedList, T value)
            where T : IComparable<T>
        {
            T[] listAsArray = sortedList as T[];
            if(listAsArray != null)
            {
                return Array.BinarySearch(listAsArray, value);
            }

            List<T> listAsGenericList = sortedList as List<T>;
            if(listAsGenericList == null)
            {
                listAsGenericList = new List<T>(sortedList);
            }

            return listAsGenericList.BinarySearch(value);
        }

        /// <summary>
        /// In a list with a mapping, already sorted according to that mapping, search an
        /// item by binary search. The mapping is provided as implementing 
        /// <see cref="IComparable{TValue}"/> with <typeparamref name="TValue"/> being the
        /// mapped type. If the item is not found, a negative number is returned.
        /// </summary>
        public static
        int
        BinaryMapSearch<T,TValue>(IList<T> sortedList, TValue value)
            where T : IComparable<TValue>
        {
            int first = 0;
            int last = sortedList.Count - 1;

            while(first <= last)
            {
                int mid = first + ((last - first) >> 1);
                int comparisonResult = sortedList[mid].CompareTo(value);

                if(comparisonResult == 0)
                {
                    return mid;
                }

                if(comparisonResult < 0)
                {
                    first = mid + 1;
                }
                else
                {
                    last = mid - 1;
                }
            }

            return ~first;
        }
    }
}
