/*
 Copyright (C) 2008 Siarhei Novik (snovik@gmail.com)
  
 This file is part of QLNet Project http://www.qlnet.org

 QLNet is free software: you can redistribute it and/or modify it
 under the terms of the QLNet license.  You should have received a
 copy of the license along with this program; if not, license is  
 available online at <http://trac2.assembla.com/QLNet/wiki/License>.
  
 QLNet is a based on QuantLib, a free-software/open-source library
 for financial quantitative analysts and developers - http://quantlib.org/
 The QuantLib license is available online at http://quantlib.org/license.shtml.
 
 This program is distributed in the hope that it will be useful, but WITHOUT
 ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 FOR A PARTICULAR PURPOSE.  See the license for more details.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QLNet {
    public static class Comparison {
        /*! Follows somewhat the advice of Knuth on checking for floating-point
            equality. The closeness relationship is:
            \f[
            \mathrm{close}(x,y,n) \equiv |x-y| \leq \varepsilon |x|
                                  \wedge |x-y| \leq \varepsilon |y|
            \f]
            where \f$ \varepsilon \f$ is \f$ n \f$ times the machine accuracy;
            \f$ n \f$ equals 42 if not given.  */
        public static bool close(double x, double y) { return close(x, y, 42); }
        public static bool close(double x, double y, int n) {
            double diff = System.Math.Abs(x - y), tolerance = n * Const.QL_Epsilon;
            return diff <= tolerance * System.Math.Abs(x) && diff <= tolerance * System.Math.Abs(y);
        }

    }
}
