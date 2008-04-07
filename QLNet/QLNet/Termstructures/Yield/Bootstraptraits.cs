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

    //! Forward-curve traits
    struct ForwardRate<Interpolator> {
        static Date initialDate(YieldTermStructure c) { return c.referenceDate(); }   // start of curve data
        static double initialValue(YieldTermStructure c) { return 0.02; } // dummy value at reference date
        static bool dummyInitialValue() { return true; }    // true if the initialValue is just a dummy value
        static double initialGuess() { return 0.02; } // initial guess
        // further guesses
        static double guess(YieldTermStructure c, Date d) {
            return c.forwardRate(d, d, c.dayCounter(), Compounding.Continuous, Frequency.Annual, true).rate();
        }
        // possible constraints based on previous values
        static double minValueAfter(int v, List<double> l) { return double.MinValue; }
        static double maxValueAfter(int v, List<double> l) {
            // no constraints.
            // We choose as max a value very unlikely to be exceeded.
            return 3;
        }
        // update with new guess
        static void updateGuess(List<decimal> data, decimal forward, int i) {
            data[i] = forward;
            if (i == 1)
                data[0] = forward; // first point is updated as well
        }
        // upper bound for convergence loop
        static int maxIterations() { return 25; }
    };
}
