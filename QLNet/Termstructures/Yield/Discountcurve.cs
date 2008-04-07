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
    // Term structure based on bootstrapping
    public class InterpolatedDiscountCurve<Interpolator, BootStrap> : InterpolatedYieldCurve<Interpolator, BootStrap>
        where Interpolator : IInterpolationFactory, new()
        where BootStrap : IBootStrap, new() {

        #region Properties
        public List<double> discounts() { return data_; } 
        #endregion

        #region Constructors
        public InterpolatedDiscountCurve(Date referenceDate, List<BootstrapHelper<YieldTermStructure>> instruments,
                                         DayCounter dayCounter, Handle<Quote> turnOfYearEffect, double accuracy, Interpolator i) 
            : base(referenceDate, instruments, dayCounter, turnOfYearEffect, accuracy, i) { }
        #endregion

        // we do not need to override it. Check comments to InterpolatedYieldCurve
        //public override Date maxDate() { return dates_.Last(); }

        protected override double discountImpl(double t) { return interpolation_.value(t, true) * discountImplFactor(t); }

        #region Discount-curve traits
        public override Date initialDate(YieldTermStructure c) { return c.referenceDate(); }   // start of curve data
        public override double initialValue(YieldTermStructure c) { return 1; }    // value at reference date
        public override bool dummyInitialValue() { return false; }   // true if the initialValue is just a dummy value
        public override double initialGuess() { return 0.9; }   // initial guess
        public override double guess(YieldTermStructure c, Date d) { return c.discount(d, true); }  // further guesses
        // possible constraints based on previous values
		public override double minValueAfter(int s, List<double> l) { 
			// replace with Epsilon
			return 2.2204460492503131e-016;
		}
        public override double maxValueAfter(int i, List<double> data) { return data[i - 1]; }
        // update with new guess
        public override void updateGuess(List<double> data, double discount, int i) { data[i] = discount; }
        public override int maxIterations() { return 25; }   // upper bound for convergence loop 
        #endregion
    }

    // Term structure based on interpolation of discount factors
    public class InterpolatedDiscountCurve<Interpolator> : YieldTermStructure
            where Interpolator : IInterpolationFactory, new() {

        protected List<Date> dates_;
        public List<Date> dates() { return dates_; }

        protected List<double> times_ = new List<double>();

        protected List<double> data_;
        public List<double> discounts() { return data_; }

        protected Interpolation interpolation_;
        protected Interpolator interpolator_;


        public InterpolatedDiscountCurve(List<Date> dates, List<double> discounts, DayCounter dayCounter) :
            this(dates, discounts, dayCounter, new Calendar()) { }
        public InterpolatedDiscountCurve(List<Date> dates, List<double> discounts, DayCounter dayCounter, Calendar cal) :
            this(dates, discounts, dayCounter, cal, new Interpolator()) { }
        public InterpolatedDiscountCurve(List<Date> dates, List<double> discounts, DayCounter dayCounter,
                                         Calendar cal, Interpolator interpolator) :
            base(dates[0], cal, dayCounter) {
            dates_ = dates;
            data_ = discounts;
            interpolator_ = interpolator;

            if (dates_.Count == 0) throw new ArgumentException("no input dates given");
            if (data_.Count == 0) throw new ArgumentException("no input discount factors given");
            if (data_.Count != dates_.Count) throw new ArgumentException("dates/discount factors count mismatch");
            if (data_[0] != 1) throw new ArgumentException("the first discount must be == 1.0 to flag the corrsponding date as settlement date");

            times_.Add(0);
            for (int i = 1; i < dates_.Count; i++) {
                if (!(dates_[i] > dates_[i - 1]))
                    throw new ArgumentException("invalid date (" + dates_[i] + ", vs " + dates_[i - 1] + ")");
                if (data_[i] < 0) throw new ArgumentException("negative discount");
                times_.Add(dayCounter.yearFraction(dates_[0], dates_[i]));
                if (Comparison.close(times_[i], times_[i - 1]))
                    throw new ArgumentException("two dates correspond to the same time under this curve's day count convention");
            }

            interpolation_ = interpolator_.interpolate(times_, times_.Count, data_);
            interpolation_.update();
        }

        protected override double discountImpl(double t) {
            return interpolation_.value(t, true);
        }
    }
}
