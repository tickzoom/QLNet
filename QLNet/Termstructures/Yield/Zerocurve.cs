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
    //! Term structure based on interpolation of zero yields
    public class InterpolatedZeroCurve<Interpolator, BootStrap> : InterpolatedYieldCurve<Interpolator, BootStrap>
        where Interpolator : IInterpolationFactory, new()
        where BootStrap : IBootStrap, new() {

        public InterpolatedZeroCurve(Date referenceDate, List<BootstrapHelper<YieldTermStructure>> instruments,
                                     DayCounter dayCounter, Handle<Quote> turnOfYearEffect, double accuracy, Interpolator i) 
            : base(referenceDate, instruments, dayCounter, turnOfYearEffect, accuracy, i) 
        { }
//        // constructor
//        public InterpolatedZeroCurve(List<Date> dates, List<double> yields,
//                                      DayCounter dayCounter) :
//            this(dates, yields, dayCounter, new Interpolator()) { }
//        public InterpolatedZeroCurve(List<Date> dates, List<double> yields,
//                                      DayCounter dayCounter, Interpolator interpolator) :
//            base(dates[0], new Calendar(), dayCounter) {
//            dates_ = dates;
//            data_ = yields;

//            if (dates_.Count==0) throw new ArgumentException("too few dates");
//            if (data_.Count != dates_.Count) throw new ArgumentException("dates/yields count mismatch");

//            times_.Clear();
//            times_.Add(0);

//            for (int i = 1; i < dates_.Count; i++) {
//                if (!(dates_[i] > dates_[i-1]))
//                    throw new ArgumentException("invalid date (" + dates_[i] + ", vs " + dates_[i-1] + ")");
                
//                if (data_[i] < 0) throw new ArgumentException("negative yield");
//                times_.Add(dayCounter.yearFraction(dates_[0], dates_[i]));
                
//                if (Comparison.close(times_[i],times_[i-1]))
//                    throw new ArgumentException("two dates correspond to the same time under this curve's day count convention");
//            }

////            interpolator_.init(times_.First(), times_.Last(), data_.First());
//            interpolator_.update();
//        }


        // Inspectors
        public override Date maxDate() { return dates_.Last(); }
        public List<double> zeroRates { get { return data_; } }

//        protected override decimal zeroYieldImpl(decimal t) {
//            return interpolator_.interpolate(t, true);
//        }


        #region Zero-curve traits
        public override Date initialDate(YieldTermStructure c) { return c.referenceDate(); }   // start of curve data
        public override double initialValue(YieldTermStructure c) { return 0.02; } // dummy value at reference date
        public override bool dummyInitialValue() { return true; }    // true if the initialValue is just a dummy value
        public override double initialGuess() { return 0.02; } // initial guess
        // further guesses
        public override double guess(YieldTermStructure c, Date d) {
            return c.zeroRate(d, c.dayCounter(), Compounding.Continuous, Frequency.Annual, true).rate();
        }
        // possible constraints based on previous values
        public override double minValueAfter(int v, List<double> l) { return double.MinValue; }
        public override double maxValueAfter(int v, List<double> l) {
            // no constraints.
            // We choose as max a value very unlikely to be exceeded.
            return 3;
        }
        // update with new guess
        public override void updateGuess(List<double> data, double rate, int i) {
            data[i] = rate;
            if (i == 1)
                data[0] = rate; // first point is updated as well
        }
        // upper bound for convergence loop
        public override int maxIterations() { return 25; } 
        #endregion
    }
}
