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
    public class PiecewiseYieldCurve<Traits, Interpolator> : PiecewiseYieldCurve<Traits, Interpolator, IterativeBootstrap<Traits, Interpolator>>
        where Traits : ITraits, new()
        where Interpolator : IInterpolationFactory, new() {

        //public InterpolatedYieldCurve(Date referenceDate, List<BootstrapHelper<YieldTermStructure>> instruments,
        //                      DayCounter dayCounter) :
        //    this(referenceDate, instruments, dayCounter, new SimpleQuote()) { }
        //public InterpolatedYieldCurve(Date referenceDate, List<BootstrapHelper<YieldTermStructure>> instruments,
        //                              DayCounter dayCounter, Quote turnOfYearEffect) :
        //    this(referenceDate, instruments, dayCounter, turnOfYearEffect, 1.0e-12) { }
        //public InterpolatedYieldCurve(Date referenceDate, List<BootstrapHelper<YieldTermStructure>> instruments,
        //                              DayCounter dayCounter, Handle<Quote> turnOfYearEffect, double accuracy) :
        //    this(referenceDate, instruments, dayCounter, turnOfYearEffect, accuracy, new Interpolator()) { }
        public PiecewiseYieldCurve(Date referenceDate, List<BootstrapHelper<YieldTermStructure>> instruments,
                                      DayCounter dayCounter, Handle<Quote> turnOfYearEffect, double accuracy, Interpolator i)
            : base(referenceDate, instruments, dayCounter, turnOfYearEffect, accuracy, i) {
        }

        //public InterpolatedYieldCurve(int settlementDays, Calendar calendar, List<BootstrapHelper<YieldTermStructure>> instruments,
        //                              DayCounter dayCounter) :
        //    this(settlementDays, calendar, instruments, dayCounter, new SimpleQuote()) { }
        //public InterpolatedYieldCurve(int settlementDays, Calendar calendar, List<BootstrapHelper<YieldTermStructure>> instruments,
        //                              DayCounter dayCounter, Quote turnOfYearEffect) :
        //    this(settlementDays, calendar, instruments, dayCounter, turnOfYearEffect, 1.0e-12) { }
        //public InterpolatedYieldCurve(int settlementDays, Calendar calendar, List<BootstrapHelper<YieldTermStructure>> instruments,
        //                              DayCounter dayCounter, Quote turnOfYearEffect, double accuracy) :
        //    this(settlementDays, calendar, instruments, dayCounter, turnOfYearEffect, accuracy, new Interpolator()) { }
        public PiecewiseYieldCurve(int settlementDays, Calendar calendar, List<BootstrapHelper<YieldTermStructure>> instruments,
                                      DayCounter dayCounter, Handle<Quote> turnOfYearEffect, double accuracy, Interpolator i)
            : base(settlementDays, calendar, instruments, dayCounter, turnOfYearEffect, accuracy, i) { }
    }

    public class PiecewiseYieldCurve<Traits, Interpolator, BootStrap> : YieldTermStructure, ITraits
        where Traits : ITraits, new()
        where Interpolator : IInterpolationFactory, new()
        where BootStrap : IBootStrap, new() {

        #region Properties
        public List<Date> dates_;
        public List<double> times_ = new List<double>();
        public List<double> data_;

        protected Handle<Quote> turnOfYearEffect_;
        public double accuracy_;
        protected Date latestReference_;
        protected double turnOfYear_;

        protected List<BootstrapHelper<YieldTermStructure>> instruments_;
        public List<BootstrapHelper<YieldTermStructure>> instruments { get { return instruments_; } }

        public Interpolation interpolation_;
        public Interpolator interpolator_ = new Interpolator();

        protected BootStrap bootstrap_ = new BootStrap();
        #endregion

        #region Traits wrapper
        ITraits traits_ = new Traits();
        public Date initialDate(YieldTermStructure c) { return traits_.initialDate(c); }
        public double initialValue(YieldTermStructure c) { return traits_.initialValue(c); }
        public bool dummyInitialValue() { return traits_.dummyInitialValue(); }
        public double initialGuess() { return traits_.initialGuess(); }
        public double guess(YieldTermStructure c, Date d) { return traits_.guess(c, d); }
        public double minValueAfter(int s, List<double> l) { return traits_.minValueAfter(s, l); }
        public double maxValueAfter(int s, List<double> l) { return traits_.maxValueAfter(s, l); }
        public void updateGuess(List<double> data, double discount, int i) { traits_.updateGuess(data, discount, i); }
        public int maxIterations() { return traits_.maxIterations(); }
        #endregion

        #region Constructors
        //public InterpolatedYieldCurve(Date referenceDate, List<BootstrapHelper<YieldTermStructure>> instruments,
        //                      DayCounter dayCounter) :
        //    this(referenceDate, instruments, dayCounter, new SimpleQuote()) { }
        //public InterpolatedYieldCurve(Date referenceDate, List<BootstrapHelper<YieldTermStructure>> instruments,
        //                              DayCounter dayCounter, Quote turnOfYearEffect) :
        //    this(referenceDate, instruments, dayCounter, turnOfYearEffect, 1.0e-12) { }
        //public InterpolatedYieldCurve(Date referenceDate, List<BootstrapHelper<YieldTermStructure>> instruments,
        //                              DayCounter dayCounter, Handle<Quote> turnOfYearEffect, double accuracy) :
        //    this(referenceDate, instruments, dayCounter, turnOfYearEffect, accuracy, new Interpolator()) { }
        public PiecewiseYieldCurve(Date referenceDate, List<BootstrapHelper<YieldTermStructure>> instruments,
                                      DayCounter dayCounter, Handle<Quote> turnOfYearEffect, double accuracy, Interpolator i)
            : base(referenceDate, new Calendar(), dayCounter) {
            instruments_ = instruments;
            turnOfYearEffect_ = turnOfYearEffect;
            accuracy_ = accuracy;

            setTurnOfYear();
            turnOfYearEffect_.registerWith(update);
            bootstrap_.setup(this);
        }

        //public InterpolatedYieldCurve(int settlementDays, Calendar calendar, List<BootstrapHelper<YieldTermStructure>> instruments,
        //                              DayCounter dayCounter) :
        //    this(settlementDays, calendar, instruments, dayCounter, new SimpleQuote()) { }
        //public InterpolatedYieldCurve(int settlementDays, Calendar calendar, List<BootstrapHelper<YieldTermStructure>> instruments,
        //                              DayCounter dayCounter, Quote turnOfYearEffect) :
        //    this(settlementDays, calendar, instruments, dayCounter, turnOfYearEffect, 1.0e-12) { }
        //public InterpolatedYieldCurve(int settlementDays, Calendar calendar, List<BootstrapHelper<YieldTermStructure>> instruments,
        //                              DayCounter dayCounter, Quote turnOfYearEffect, double accuracy) :
        //    this(settlementDays, calendar, instruments, dayCounter, turnOfYearEffect, accuracy, new Interpolator()) { }
        public PiecewiseYieldCurve(int settlementDays, Calendar calendar, List<BootstrapHelper<YieldTermStructure>> instruments,
                                      DayCounter dayCounter, Handle<Quote> turnOfYearEffect, double accuracy, Interpolator i)
            : base(settlementDays, calendar, dayCounter) {
            instruments_ = instruments;
            turnOfYearEffect_ = turnOfYearEffect;
            accuracy_ = accuracy;

            setTurnOfYear();
            turnOfYearEffect_.registerWith(update);
            bootstrap_.setup(this);
        } 
        #endregion

        // here we do not refer to the base curve as in QL because our base curve is YieldTermStructure and not Traits::base_curve
        public override Date maxDate() { calculate(); return dates_.Last(); }
        public List<Date> dates() { calculate(); return dates_; }
        public List<double> times() { calculate(); return times_; }
        public List<double> data() { calculate(); return data_; }

        public Dictionary<Date, double> nodes() {
            calculate();
            Dictionary<Date, double> results = new Dictionary<Date, double>();
            for (int i = 0; i < dates_.Count; ++i)
                results.Add(dates_[i], data_[i]);
            return results;
        }

        // observer interface
        public override void update() {
            base.update();
            if (referenceDate() != latestReference_)
                setTurnOfYear();
        }

        // recheck
        //protected double discountImplFactor(double t) {
        //    calculate();

        //    if (!turnOfYearEffect_.empty() && t > turnOfYear_) {
        //        if (!turnOfYearEffect_.link.isValid())
        //            throw new ArgumentException("invalid turnOfYearEffect quote");
        //        double turnOfYearEffect = turnOfYearEffect_.link.value();
        //        if (!(turnOfYearEffect > 0.0 && turnOfYearEffect <= 1.0))
        //            throw new ArgumentException("invalid turnOfYearEffect value: " + turnOfYearEffect);
        //        return turnOfYearEffect;
        //    }
        //    return 1;
        //}

        private void setTurnOfYear() {
            Date refDate = referenceDate();
            Date turnOfYear = new Date(31, Month.December, refDate.Year);
            turnOfYear_ = timeFromReference(turnOfYear);
            latestReference_ = refDate;
        }

        protected override void performCalculations() {
            // this code was origionally in IterativeBootStrap but it does not make sense to have it there
            // because it relies too much on this class

            //prepare instruments
            int n = instruments_.Count;

            // ensure rate helpers are sorted
            instruments_.Sort((x, y) => x.latestDate().CompareTo(y.latestDate()));

            // check that there is no instruments with the same maturity
            for (int i = 1; i < n; ++i) {
                Date m1 = instruments_[i - 1].latestDate(),
                     m2 = instruments_[i].latestDate();
                if (m1 == m2) throw new ArgumentException("two instruments have the same maturity (" + m1 + ")");
            }

            // check that there is no instruments with invalid quote
            for (int i = 0; i < n; ++i)
                if (!instruments_[i].quoteIsValid())
                    throw new ArgumentException("instrument " + i + " (maturity: " + instruments_[i].latestDate() +
                           ") has an invalid quote");

            // setup instruments
            for (int i = 0; i < n; ++i) {
                // There is a significant interaction with observability.
                instruments_[i].setTermStructure(this);
            }

            // calculate dates and times
            dates_ = new Array<Date>(n + 1);
            times_ = new Array<double>(n + 1);
            dates_[0] = initialDate(this);
            times_[0] = timeFromReference(dates_[0]);
            for (int i = 0; i < n; ++i) {
                dates_[i + 1] = instruments_[i].latestDate();
                times_[i + 1] = timeFromReference(dates_[i + 1]);
            }

            // now comes the bootstrapping
            // just delegate to the bootstrapper
            bootstrap_.calculate();
        }

        protected override double discountImpl(double t) { return interpolation_.value(t, true); }
    }
}
