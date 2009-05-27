//-----------------------------------------------------------------------
// <copyright file="DigammaAlgorithm.cs" company="Math.NET Project">
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
// <contribution>
//    Cephes Math Library, Stephen L. Moshier
//    ALGLIB, Sergey Bochkanov
// </contribution>
//-----------------------------------------------------------------------

using System;

namespace MathNet.Numerics.SpecialFunctions.Algorithms
{
    internal class DigammaAlgorithm
    {
        /// <summary>
        /// Returns the digamma (psi) function of real values (except at 0, -1, -2, ...).
        /// Digamma is the logarithmic derivative of the <see cref="Fn.Gamma"/> function.
        /// </summary>
        public static
        double
        Digamma(double x)
        {
            double y;
            double nz = 0.0;
            bool negative = (x <= 0);

            if(negative)
            {
                double q = x;
                double p = Math.Floor(q);

                if(Number.AlmostEqual(p, q))
                {
                    return double.NaN; // singularity, undefined
                }

                nz = q - p;

                if(nz != 0.5)
                {
                    if(nz > 0.5)
                    {
                        p = p + 1.0;
                        nz = q - p;
                    }

                    nz = Math.PI / Math.Tan(Math.PI * nz);
                }
                else
                {
                    nz = 0.0;
                }

                x = 1.0 - x;
            }

            if((x <= 10.0) && (x == Math.Floor(x)))
            {
                y = 0.0;
                int n = (int)Math.Floor(x);

                for(int i = 1; i <= n - 1; i++)
                {
                    y = y + (1.0 / i);
                }

                y = y - Constants.EulerGamma;
            }
            else
            {
                double s = x;
                double w = 0.0;

                while(s < 10.0)
                {
                    w = w + (1.0 / s);
                    s = s + 1.0;
                }

                if(s < 1.0e17)
                {
                    double z = 1.0 / (s * s);
                    double polv = 8.33333333333333333333e-2;
                    polv = (polv * z) - 2.10927960927960927961e-2;
                    polv = (polv * z) + 7.57575757575757575758e-3;
                    polv = (polv * z) - 4.16666666666666666667e-3;
                    polv = (polv * z) + 3.96825396825396825397e-3;
                    polv = (polv * z) - 8.33333333333333333333e-3;
                    polv = (polv * z) + 8.33333333333333333333e-2;
                    y = z * polv;
                }
                else
                {
                    y = 0.0;
                }

                y = Math.Log(s) - (0.5 / s) - y - w;
            }

            if(negative)
            {
                return y - nz;
            }

            return y;
        }
    }
}
