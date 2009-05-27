//-----------------------------------------------------------------------
// <copyright file="Collection.cs" company="Math.NET Project">
//    Copyright (c) 2004-2009, Joannes Vermorel.
//    All Right Reserved.
// </copyright>
// <author>
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

namespace MathNet.Numerics
{
    /// <summary>
    /// The class <c>Collection</c> contains several utilities performing
    /// some basic collection operations (like union, intersection...).
    /// </summary>
    public static class Collection
    {
        /// <summary>
        /// The class <c>ConcatCollection</c> is used to perform the 
        /// mathematical concatenation between two collections.
        /// </summary>
        /// <seealso cref="Collection.Concat"/>
        private sealed class ConcatCollection : ICollection
        {
            private sealed class ConcatEnumerator : IEnumerator
            {
                readonly IEnumerator _enumerator1;
                readonly IEnumerator _enumerator2;

                bool _isEnumator1Current;

                public ConcatEnumerator(ConcatCollection union)
                {
                    _enumerator1 = union._c1.GetEnumerator();
                    _enumerator2 = union._c2.GetEnumerator();
                    _isEnumator1Current = true;
                }

                public void Reset()
                {
                    _enumerator1.Reset();
                    _enumerator2.Reset();
                    _isEnumator1Current = true;
                }

                public object Current
                {
                    get
                    {
                        if(_isEnumator1Current)
                        {
                            return _enumerator1.Current;
                        }
                        
                        return _enumerator2.Current;
                    }
                }

                public bool MoveNext()
                {
                    if(_isEnumator1Current && _enumerator1.MoveNext())
                    {
                        return true;
                    }

                    _isEnumator1Current = false;
                    return _enumerator2.MoveNext();
                }
            }

            readonly ICollection _c1;
            readonly ICollection _c2;

            public ConcatCollection(ICollection c1, ICollection c2)
            {
                _c1 = c1;
                _c2 = c2;
            }

            public bool IsSynchronized
            {
                get { return _c1.IsSynchronized && _c2.IsSynchronized; }
            }

            public int Count
            {
                get { return _c1.Count + _c2.Count; }
            }

            public void CopyTo(Array array, int index)
            {
                int indexArray = index;

                foreach(object obj in _c1)
                {
                    array.SetValue(obj, indexArray++);
                }

                foreach(object obj in _c2)
                {
                    array.SetValue(obj, indexArray++);
                }
            }

            public object SyncRoot
            {
                get { return _c1.SyncRoot; }
            }

            public IEnumerator GetEnumerator()
            {
                return new ConcatEnumerator(this);
            }
        }

        /// <summary>
        /// The class <c>InterCollection</c> is used to perform the
        /// mathematical intersection between two collections.
        /// </summary>
        /// <seealso cref="Collection.Inter"/>
        [Obsolete("Use Set<T>.Intersect() instead.", false)]
        private sealed class InterCollection : ICollection
        {
            private readonly ArrayList _intersection;

            public InterCollection(ICollection c1, ICollection c2)
            {
                // swap in order to have <c>c1.Count <= c2.Count</c>
                if(c1.Count > c2.Count)
                {
                    ICollection c1Bis = c1;
                    c1 = c2;
                    c2 = c1Bis;
                }

                Hashtable table = new Hashtable(c1.Count);
                foreach(object obj in c1)
                {
                    if(!table.Contains(obj))
                    {
                        table.Add(obj, null);
                    }
                }

                // building the intersection
                _intersection = new ArrayList();
                foreach(object obj in c2)
                {
                    if(table.Contains(obj))
                    {
                        _intersection.Add(obj);
                        table.Remove(obj);
                    }
                }

                _intersection.TrimToSize();
            }

            #region ICollection Members

            public IEnumerator GetEnumerator()
            {
                return _intersection.GetEnumerator();
            }

            public bool IsSynchronized
            {
                get { return _intersection.IsSynchronized; }
            }

            public int Count
            {
                get { return _intersection.Count; }
            }

            public void CopyTo(Array array, int index)
            {
                _intersection.CopyTo(array, index);
            }

            public object SyncRoot
            {
                get { return _intersection.SyncRoot; }
            }

            #endregion
        }

        /// <summary>
        /// The class <c>UnionCollection</c> is used to perform the
        /// mathematical union between two collections.
        /// </summary>
        [Obsolete("Use Set<T>.Union() instead.", false)]
        private sealed class UnionCollection : ICollection
        {
            private readonly ArrayList _union;

            public UnionCollection(ICollection c1, ICollection c2)
            {
                Hashtable table1 = new Hashtable(c1.Count);
                foreach(object obj in c1)
                {
                    if(!table1.Contains(obj))
                    {
                        table1.Add(obj, null);
                    }
                }

                Hashtable table2 = new Hashtable(c2.Count);
                foreach(object obj in c2)
                {
                    if(!table2.Contains(obj))
                    {
                        table2.Add(obj, null);
                    }
                }

                // building the union
                _union = new ArrayList(Math.Max(table1.Count, table2.Count));
                _union.AddRange(table1.Keys);
                foreach(object obj in c2)
                {
                    if(!table1.Contains(obj))
                    {
                        _union.Add(obj);
                    }
                }

                _union.TrimToSize();
            }

            #region ICollection Members

            public bool IsSynchronized
            {
                get { return _union.IsSynchronized; }
            }

            public int Count
            {
                get { return _union.Count; }
            }

            public void CopyTo(Array array, int index)
            {
                _union.CopyTo(array, index);
            }

            public IEnumerator GetEnumerator()
            {
                return _union.GetEnumerator();
            }

            public object SyncRoot
            {
                get { return _union.SyncRoot; }
            }

            #endregion
        }

        /// <summary>
        /// The collection <c>MinusCollection</c> is used to perform
        /// the mathematical subtraction of two collections.
        /// </summary>
        /// <seealso cref="Collection.Minus"/>
        [Obsolete("Use Set<T>.Subtract() instead.", false)]
        private sealed class MinusCollection : ICollection
        {
            private readonly ArrayList _minus;

            public MinusCollection(ICollection c1, ICollection c2)
            {
                Hashtable table1 = new Hashtable(c1.Count);
                foreach(object obj in c1)
                {
                    if(!table1.Contains(obj))
                    {
                        table1.Add(obj, null);
                    }
                }

                Hashtable table2 = new Hashtable(c2.Count);
                foreach(object obj in c2)
                {
                    if(!table2.Contains(obj))
                    {
                        table2.Add(obj, null);
                    }
                }

                // building minus collection
                _minus = new ArrayList(Math.Max(c1.Count - c2.Count, 10));
                foreach(object obj in table1.Keys)
                {
                    if(!table2.Contains(obj))
                    {
                        _minus.Add(obj);
                    }
                }

                _minus.TrimToSize();
            }

            #region ICollection Members

            public bool IsSynchronized
            {
                get { return _minus.IsSynchronized; }
            }

            public int Count
            {
                get { return _minus.Count; }
            }

            public void CopyTo(Array array, int index)
            {
                _minus.CopyTo(array, index);
            }

            public IEnumerator GetEnumerator()
            {
                return _minus.GetEnumerator();
            }

            public object SyncRoot
            {
                get { return _minus.SyncRoot; }
            }

            #endregion
        }

        /// <summary>
        /// Returns a collection resulting from the concatenation from
        /// <c>c1</c> and <c>c2</c>.
        /// </summary>
        /// <param name="c1">Should not be null.</param>
        /// <param name="c2">Should not be null.</param>
        /// <remarks>The call is performed in <c>O(1)</c> computational time, the
        /// concatenated collection is not built explicitly.</remarks>
        public static ICollection Concat(ICollection c1, ICollection c2)
        {
            if(c1 == null)
            {
                throw new ArgumentNullException("c1", Properties.LocalStrings.ArgumentNull("c1"));
            }

            if(c2 == null)
            {
                throw new ArgumentNullException("c2", Properties.LocalStrings.ArgumentNull("c2"));
            }

            return new ConcatCollection(c1, c2);
        }

        /// <summary>
        /// Returns a collection resulting from the mathematical intersection
        /// of <c>c1</c> and <c>c2</c>.
        /// </summary>
        /// <param name="c1">Should not be null.</param>
        /// <param name="c2">Should not be null.</param>
        /// <remarks>
        /// <p>The call is performed in <c>O(c1.Count+c2.Count)</c> and
        /// the intersection is built explicitly.</p>
        /// <p>The resulting collection will not contain several identical elements.</p>
        /// <p>Example: Inter({1;1;2;3},{0;1;1;3;4}) = {1;3}.</p>
        /// </remarks>
        [Obsolete("Use Set<T>.Intersect() instead.", false)]
        public static ICollection Inter(ICollection c1, ICollection c2)
        {
            if(c1 == null)
            {
                throw new ArgumentNullException("c1", Properties.LocalStrings.ArgumentNull("c1"));
            }

            if(c2 == null)
            {
                throw new ArgumentNullException("c2", Properties.LocalStrings.ArgumentNull("c2"));
            }

            return new InterCollection(c1, c2);
        }

        /// <summary>
        /// Returns a collection resulting from the subtraction of
        /// the items of <c>c2</c> to the collection <c>c1</c>. 
        /// </summary>
        /// <param name="c1">Should not be null.</param>
        /// <param name="c2">Should not be null.</param>
        /// <remarks>The call is performed in <c>O(c1.Count+c2.Count)</c></remarks>
        [Obsolete("Use Set<T>.Subtract() instead.", false)]
        public static ICollection Minus(ICollection c1, ICollection c2)
        {
            if(c1 == null)
            {
                throw new ArgumentNullException("c1", Properties.LocalStrings.ArgumentNull("c1"));
            }

            if(c2 == null)
            {
                throw new ArgumentNullException("c2", Properties.LocalStrings.ArgumentNull("c2"));
            }

            return new MinusCollection(c1, c2);
        }

        /// <summary>
        /// Returns the Cartesian product of the two collections <c>c1</c>
        /// and <c>c2</c>.
        /// </summary>
        /// <param name="c1">Should not be null.</param>
        /// <param name="c2">Should not be null.</param>
        public static ICollection Product(ICollection c1, ICollection c2)
        {
            if(c1 == null)
            {
                throw new ArgumentNullException("c1", Properties.LocalStrings.ArgumentNull("c1"));
            }

            if(c2 == null)
            {
                throw new ArgumentNullException("c2", Properties.LocalStrings.ArgumentNull("c2"));
            }

            return null;
        }

        /// <summary>
        /// Returns a collection resulting from the union of the items
        /// of <c>c1</c> and <c>c2</c>.
        /// </summary>
        /// <param name="c1">Should not be null.</param>
        /// <param name="c2">Should not be null.</param>
        /// <remarks>
        /// <p>The call is performed in <c>O(c1.Count+c2.Count)</c>
        /// computational time.</p>
        /// <p>The resulting collection will not contain several identical elements.</p>
        /// <p>Example: Union({1;1;3},{0;1;2;3}) = {0;1;2;3}</p>
        /// </remarks>
        [Obsolete("Use Set<T>.Union() instead.", false)]
        public static ICollection Union(ICollection c1, ICollection c2)
        {
            if(c1 == null)
            {
                throw new ArgumentNullException("c1", Properties.LocalStrings.ArgumentNull("c1"));
            }

            if(c2 == null)
            {
                throw new ArgumentNullException("c2", Properties.LocalStrings.ArgumentNull("c2"));
            }

            return new UnionCollection(c1, c2);
        }

        #region NUnit testing suite
#if DEBUG2
        /// <summary>
        /// Testing the class <see cref="Collection"/>.
        /// </summary>
        [TestFixture]
        public class TestingSuite
        {
            /// <summary>
            /// Testing the method <see cref="ConcatCollection.Count"/>.
            /// </summary>
            [Test] public void ConcatCount()
            {
                int[] array0 = new int[0], array1 = new int[7], array2 = new int[13];

                Assert.AreEqual(array0.Length + array0.Length,
                    (new ConcatCollection(array0, array0)).Count, "#A00");
                Assert.AreEqual(array0.Length + array1.Length,
                    (new ConcatCollection(array0, array1)).Count, "#A01");
                Assert.AreEqual(array0.Length + array2.Length,
                    (new ConcatCollection(array0, array2)).Count, "#A02");

                Assert.AreEqual(array1.Length + array0.Length,
                    (new ConcatCollection(array1, array0)).Count, "#A03");
                Assert.AreEqual(array1.Length + array1.Length,
                    (new ConcatCollection(array1, array1)).Count, "#A04");
                Assert.AreEqual(array1.Length + array2.Length,
                    (new ConcatCollection(array1, array2)).Count, "#A05");
            }

            /// <summary>
            /// Testing the method <see cref="ConcatCollection.GetEnumerator"/>.
            /// </summary>
            [Test] public void ConcatGetEnumerator()
            {
                // generating two arrays
                int[] array1 = new int[10], array2 = new int[13];
                for(int i = 0; i < array1.Length; i++) array1[i] = i;
                for(int i = 0; i < array2.Length; i++) array2[i] = i + array1.Length;

                ConcatCollection union = new ConcatCollection(array1, array2);

                int index = 0;

                foreach(int value in union)
                {
                    Assert.AreEqual(index++, value, "#A00 Unexpected value in collection.");
                }

                Assert.AreEqual(array1.Length + array2.Length, index, 
                    "#A01 Unexpected count of enumerated element in collection.");
            }

            /// <summary>
            /// Testing the method <see cref="InterCollection.GetEnumerator"/>
            /// </summary>
            [Test] public void InterGetEnumerator()
            {
                int LENGTH = 100;
                int[] array1 = new int[LENGTH], array2 = new int[LENGTH];

                for(int i = 0; i < LENGTH; i++)
                {
                    array1[i] = i;
                    array2[i] = i / 2;
                }

                ICollection intersection = Collection.Inter(array1, array2);

                Assert.AreEqual(LENGTH / 2, intersection.Count,
                    "#A00 Unexpected intersection count.");

                foreach(int i in intersection)
                    Assert.IsTrue(i >= 0 && i <= LENGTH,
                        "#A01-" + i + " Unexpected intersection item.");
            }

            /// <summary>
            /// Testing the method <see cref="MinusCollection.GetEnumerator"/>
            /// </summary>
            [Test] public void MinusGetEnumerator()
            {
                int LENGTH = 100;
                int[] array1 = new int[LENGTH], array2 = new int[LENGTH];

                for(int i = 0; i < LENGTH; i++)
                {
                    array1[i] = i;
                    array2[i] = i / 2;
                }

                ICollection minus = Collection.Minus(array1, array2);
                
                Assert.AreEqual(LENGTH / 2, minus.Count,
                    "#A00 Unexpected minus count.");

                foreach(int i in minus)
                    Assert.IsTrue(i >= LENGTH / 2,
                        "#A01-" + i + " Unexpected minus item.");
            }

            /// <summary>
            /// Testing the method <see cref="UnionCollection.GetEnumerator"/>.
            /// </summary>
            [Test] public void UnionGetEnumerator()
            {
                int LENGTH = 100;

                int[] array1 = new int[LENGTH], array2 = new int[LENGTH];

                for(int i = 0; i < LENGTH; i++)
                {
                    array1[i] = i;
                    array2[i] = i / 2;
                }

                ICollection union = Collection.Union(array1, array2);

                Assert.AreEqual(LENGTH, union.Count, 
                    "#A00 Unexpected union count.");

                foreach(int i in union)
                    Assert.IsTrue(i >= 0 && i < LENGTH,
                        "#A01-" + i + " Unexpected union item.");
            }
        }
#endif
        #endregion
    }
}
