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

    public interface ITraits {
        Date initialDate(YieldTermStructure c);     // start of curve data
        double initialValue(YieldTermStructure c);   // value at reference date
        bool dummyInitialValue();                    // true if the initialValue is just a dummy value
        double initialGuess();                       // initial guess
        double guess(YieldTermStructure c, Date d); // further guesses
        // possible constraints based on previous values
        double minValueAfter(int s, List<double> l);
        double maxValueAfter(int i, List<double> data);
        // update with new guess
        void updateGuess(List<double> data, double discount, int i);
        int maxIterations();                          // upper bound for convergence loop

        // 
        double discountImpl(Interpolation i, double t);
        double zeroYieldImpl(Interpolation i, double t);
        double forwardImpl(Interpolation i, double t);
    }

    public class Discount : ITraits {
        public Date initialDate(YieldTermStructure c) { return c.referenceDate(); }   // start of curve data
        public double initialValue(YieldTermStructure c) { return 1; }    // value at reference date
        public bool dummyInitialValue() { return false; }   // true if the initialValue is just a dummy value
        public double initialGuess() { return 0.9; }   // initial guess
        public double guess(YieldTermStructure c, Date d) { return c.discount(d, true); }  // further guesses
        // possible constraints based on previous values
        public double minValueAfter(int s, List<double> l) {
            // replace with Epsilon
            return 2.2204460492503131e-016;
        }
        public double maxValueAfter(int i, List<double> data) { return data[i - 1]; }
        // update with new guess
        public void updateGuess(List<double> data, double discount, int i) { data[i] = discount; }
        public int maxIterations() { return 300; }   // upper bound for convergence loop

        public double discountImpl(Interpolation i, double t) { return i.value(t, true); }
        public double zeroYieldImpl(Interpolation i, double t) { throw new NotSupportedException(); }
        public double forwardImpl(Interpolation i, double t) { throw new NotSupportedException(); }
    }

    //! Zero-curve traits
    public class ZeroYield : ITraits {
        const double avgRate = 0.05;

        public Date initialDate(YieldTermStructure c) { return c.referenceDate(); }   // start of curve data
        public double initialValue(YieldTermStructure c) { return avgRate; }    // value at reference date
        public bool dummyInitialValue() { return true; }   // true if the initialValue is just a dummy value
        public double initialGuess() { return avgRate; }   // initial guess
        public double guess(YieldTermStructure c, Date d) {
            return c.zeroRate(d, c.dayCounter(), Compounding.Continuous, Frequency.Annual, true).rate();
        }  // further guesses
        // possible constraints based on previous values
        public double minValueAfter(int s, List<double> l) {
            #if QL_NEGATIVE_RATES
            // no constraints.
            // We choose as min a value very unlikely to be exceeded.
            return -3.0;
            #else
            return 2.2204460492503131e-016;
            #endif
        }
        public double maxValueAfter(int i, List<double> data) {
            // no constraints.
            // We choose as max a value very unlikely to be exceeded.
            return 3.0;
        }
        // update with new guess
        public void updateGuess(List<double> data, double rate, int i) {
            data[i] = rate;
            if (i == 1)
                data[0] = rate; // first point is updated as well
        }
        public int maxIterations() { return 30; }   // upper bound for convergence loop

        public double discountImpl(Interpolation i, double t) { 
            double r = zeroYieldImpl(i, t);
            return Math.Exp(-r*t);
        }
        public double zeroYieldImpl(Interpolation i, double t) { return i.value(t, true); }
        public double forwardImpl(Interpolation i, double t) { throw new NotSupportedException(); }
    }

    //! Forward-curve traits
    public class ForwardRate : ITraits {
        public Date initialDate(YieldTermStructure c) { return c.referenceDate(); }   // start of curve data
        public double initialValue(YieldTermStructure c) { return 0.02; } // dummy value at reference date
        public bool dummyInitialValue() { return true; }    // true if the initialValue is just a dummy value
        public double initialGuess() { return 0.02; } // initial guess
        // further guesses
        public double guess(YieldTermStructure c, Date d) {
            return c.forwardRate(d, d, c.dayCounter(), Compounding.Continuous, Frequency.Annual, true).rate();
        }
        // possible constraints based on previous values
        public double minValueAfter(int v, List<double> l) { return 2.2204460492503131e-016; }
        public double maxValueAfter(int v, List<double> l) {
            // no constraints.
            // We choose as max a value very unlikely to be exceeded.
            return 3;
        }
        // update with new guess
        public void updateGuess(List<double> data, double forward, int i) {
            data[i] = forward;
            if (i == 1)
                data[0] = forward; // first point is updated as well
        }
        // upper bound for convergence loop
        public int maxIterations() { return 30; }

        public double discountImpl(Interpolation i, double t) {
            double r = zeroYieldImpl(i, t);
            return Math.Exp(-r * t);
        }
        public double zeroYieldImpl(Interpolation i, double t) {
            if (t == 0.0)
                return forwardImpl(i, 0.0);
            else
                return i.primitive(t, true) / t;
        }
        public double forwardImpl(Interpolation i, double t) {
            return i.value(t, true);
        }
    }
}
