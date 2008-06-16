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
    // this is the abstract class to give access to all properties and methods of PiecewiseYieldCurve and avoiding generics
    public abstract class IPiecewiseYieldCurve : YieldTermStructure, ITraits {
        #region Properties
        public List<double> data_;

        public List<BootstrapHelper<YieldTermStructure>> instruments_;

        public Interpolation interpolation_;
        #endregion

        // two constructors to forward down the ctor chain
        public IPiecewiseYieldCurve(Date referenceDate, Calendar cal, DayCounter dc) : base(referenceDate, cal, dc) { }
        public IPiecewiseYieldCurve(int settlementDays, Calendar cal, DayCounter dc) : base(settlementDays, cal, dc) { }

        #region Traits wrapper
        public abstract Date initialDate(YieldTermStructure c);
        public abstract double initialValue(YieldTermStructure c);
        public abstract bool dummyInitialValue();
        public abstract double initialGuess();
        public abstract double guess(YieldTermStructure c, Date d);
        public abstract double minValueAfter(int s, List<double> l);
        public abstract double maxValueAfter(int s, List<double> l);
        public abstract void updateGuess(List<double> data, double discount, int i);
        public abstract int maxIterations();

        protected abstract override double discountImpl(double t);
        protected abstract double zeroYieldImpl(double t);
        protected abstract double forwardImpl(double t);

        // these are dummy methods (for the sake of ITraits and should not be called directly
        public abstract double discountImpl(Interpolation i, double t);
        public abstract double zeroYieldImpl(Interpolation i, double t);
        public abstract double forwardImpl(Interpolation i, double t);
        #endregion
    }


    public class PiecewiseYieldCurve<Traits, Interpolator>
        : PiecewiseYieldCurve<Traits, Interpolator, IterativeBootstrap>
        where Traits : ITraits, new()
        where Interpolator : IInterpolationFactory, new() {

        public PiecewiseYieldCurve(Date referenceDate, List<BootstrapHelper<YieldTermStructure>> instruments, DayCounter dayCounter)
            : base(referenceDate, instruments, dayCounter) { }
        public PiecewiseYieldCurve(Date referenceDate, List<BootstrapHelper<YieldTermStructure>> instruments,
                                   DayCounter dayCounter, Handle<Quote> turnOfYearEffect)
            : base(referenceDate, instruments, dayCounter, turnOfYearEffect) { }
        public PiecewiseYieldCurve(Date referenceDate, List<BootstrapHelper<YieldTermStructure>> instruments,
                                   DayCounter dayCounter, Handle<Quote> turnOfYearEffect, double accuracy)
            : base(referenceDate, instruments, dayCounter, turnOfYearEffect, accuracy) { }
        public PiecewiseYieldCurve(Date referenceDate, List<BootstrapHelper<YieldTermStructure>> instruments,
                                   DayCounter dayCounter, Handle<Quote> turnOfYearEffect, double accuracy, Interpolator i)
            : base(referenceDate, instruments, dayCounter, turnOfYearEffect, accuracy, i) { }

        public PiecewiseYieldCurve(int settlementDays, Calendar calendar, List<BootstrapHelper<YieldTermStructure>> instruments,
                                   DayCounter dayCounter)
            : this(settlementDays, calendar, instruments, dayCounter, new Handle<Quote>(), 1.0e-12) { }
        //public InterpolatedYieldCurve(int settlementDays, Calendar calendar, List<BootstrapHelper<YieldTermStructure>> instruments,
        //                              DayCounter dayCounter, Quote turnOfYearEffect) :
        //    this(settlementDays, calendar, instruments, dayCounter, turnOfYearEffect, 1.0e-12) { }
        public PiecewiseYieldCurve(int settlementDays, Calendar calendar, List<BootstrapHelper<YieldTermStructure>> instruments,
                                      DayCounter dayCounter, Handle<Quote> turnOfYearEffect, double accuracy)
            : base(settlementDays, calendar, instruments, dayCounter, turnOfYearEffect, accuracy) { }
    }


    public class PiecewiseYieldCurve<Traits, Interpolator, BootStrap> : IPiecewiseYieldCurve, ITraits
        where Traits : ITraits, new()
        where Interpolator : IInterpolationFactory, new()
        where BootStrap : IBootStrap, new() {

        #region Properties
        public List<Date> dates_;
        public List<double> times_ = new List<double>();

        protected Handle<Quote> turnOfYearEffect_;
        public double accuracy_;
        protected Date latestReference_;
        protected double turnOfYear_;

        public Interpolator interpolator_;

        protected BootStrap bootstrap_;
        #endregion

        #region Traits wrapper
        protected ITraits traits_ = new Traits();
        public override Date initialDate(YieldTermStructure c) { return traits_.initialDate(c); }
        public override double initialValue(YieldTermStructure c) { return traits_.initialValue(c); }
        public override bool dummyInitialValue() { return traits_.dummyInitialValue(); }
        public override double initialGuess() { return traits_.initialGuess(); }
        public override double guess(YieldTermStructure c, Date d) { return traits_.guess(c, d); }
        public override double minValueAfter(int s, List<double> l) { return traits_.minValueAfter(s, l); }
        public override double maxValueAfter(int s, List<double> l) { return traits_.maxValueAfter(s, l); }
        public override void updateGuess(List<double> data, double discount, int i) { traits_.updateGuess(data, discount, i); }
        public override int maxIterations() { return traits_.maxIterations(); }

        protected override double discountImpl(double t) {
            // recheck
            calculate();

            //    if (!turnOfYearEffect_.empty() && t > turnOfYear_) {
            //        if (!turnOfYearEffect_.link.isValid())
            //            throw new ArgumentException("invalid turnOfYearEffect quote");
            //        double turnOfYearEffect = turnOfYearEffect_.link.value();
            //        if (!(turnOfYearEffect > 0.0 && turnOfYearEffect <= 1.0))
            //            throw new ArgumentException("invalid turnOfYearEffect value: " + turnOfYearEffect);
            //        return turnOfYearEffect;
            //    }
            //}
            return traits_.discountImpl(interpolation_, t);
        }
        protected override double zeroYieldImpl(double t) { return traits_.zeroYieldImpl(interpolation_, t); }
        protected override double forwardImpl(double t) { return traits_.forwardImpl(interpolation_, t); }

        // these are dummy methods (for the sake of ITraits and should not be called directly
        public override double discountImpl(Interpolation i, double t) { throw new NotSupportedException(); }
        public override double zeroYieldImpl(Interpolation i, double t) { throw new NotSupportedException(); }
        public override double forwardImpl(Interpolation i, double t) { throw new NotSupportedException(); }
        #endregion

        #region Constructors
        public PiecewiseYieldCurve(Date referenceDate, List<BootstrapHelper<YieldTermStructure>> instruments, DayCounter dayCounter)
            : this(referenceDate, instruments, dayCounter, new Handle<Quote>(), 1.0e-12, new Interpolator(), new BootStrap()) { }
        public PiecewiseYieldCurve(Date referenceDate, List<BootstrapHelper<YieldTermStructure>> instruments,
                                   DayCounter dayCounter, Handle<Quote> turnOfYearEffect)
            : this(referenceDate, instruments, dayCounter, turnOfYearEffect, 1.0e-12, new Interpolator(), new BootStrap()) { }
        public PiecewiseYieldCurve(Date referenceDate, List<BootstrapHelper<YieldTermStructure>> instruments,
                                   DayCounter dayCounter, Handle<Quote> turnOfYearEffect, double accuracy)
            : this(referenceDate, instruments, dayCounter, turnOfYearEffect, accuracy, new Interpolator(), new BootStrap()) { }
        public PiecewiseYieldCurve(Date referenceDate, List<BootstrapHelper<YieldTermStructure>> instruments,
                                   DayCounter dayCounter, Handle<Quote> turnOfYearEffect, double accuracy, Interpolator i)
            : this(referenceDate, instruments, dayCounter, turnOfYearEffect, accuracy, i, new BootStrap()) { }
        public PiecewiseYieldCurve(Date referenceDate, List<BootstrapHelper<YieldTermStructure>> instruments,
                                   DayCounter dayCounter, Handle<Quote> turnOfYearEffect, double accuracy,
                                   Interpolator i, BootStrap bootstrap)
            : base(referenceDate, new Calendar(), dayCounter) {
            instruments_ = instruments;
            turnOfYearEffect_ = turnOfYearEffect;
            accuracy_ = accuracy;
            interpolator_ = i;
            bootstrap_ = bootstrap;

            setTurnOfYear();
            turnOfYearEffect_.registerWith(update);
            bootstrap_.setup<Traits, Interpolator, BootStrap>(this);
        }

        //public InterpolatedYieldCurve(int settlementDays, Calendar calendar, List<BootstrapHelper<YieldTermStructure>> instruments,
        //                              DayCounter dayCounter) :
        //    this(settlementDays, calendar, instruments, dayCounter, new SimpleQuote()) { }
        //public InterpolatedYieldCurve(int settlementDays, Calendar calendar, List<BootstrapHelper<YieldTermStructure>> instruments,
        //                              DayCounter dayCounter, Quote turnOfYearEffect) :
        //    this(settlementDays, calendar, instruments, dayCounter, turnOfYearEffect, 1.0e-12) { }
        public PiecewiseYieldCurve(int settlementDays, Calendar calendar, List<BootstrapHelper<YieldTermStructure>> instruments,
                                   DayCounter dayCounter, Handle<Quote> turnOfYearEffect, double accuracy)
            : this(settlementDays, calendar, instruments, dayCounter, turnOfYearEffect, accuracy, 
                   new Interpolator(), new BootStrap()) { }
        public PiecewiseYieldCurve(int settlementDays, Calendar calendar, List<BootstrapHelper<YieldTermStructure>> instruments,
                                   DayCounter dayCounter, Handle<Quote> turnOfYearEffect, double accuracy,
                                   Interpolator i, BootStrap bootstrap)
            : base(settlementDays, calendar, dayCounter) {
            instruments_ = instruments;
            turnOfYearEffect_ = turnOfYearEffect;
            accuracy_ = accuracy;
            interpolator_ = i;
            bootstrap_ = bootstrap;

            setTurnOfYear();
            turnOfYearEffect_.registerWith(update);
            bootstrap_.setup<Traits, Interpolator, BootStrap>(this);
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
            // LazyObject::update();        // we do it in the TermStructure 
            if (referenceDate() != latestReference_)
                setTurnOfYear();
        }

        private void setTurnOfYear() {
            Date refDate = referenceDate();
            Date turnOfYear = new Date(31, Month.December, refDate.Year);
            turnOfYear_ = timeFromReference(turnOfYear);
            latestReference_ = refDate;
        }

        protected override void performCalculations() {
            // just delegate to the bootstrapper
            bootstrap_.calculate<Traits, Interpolator, BootStrap>();
        }
    }
}
