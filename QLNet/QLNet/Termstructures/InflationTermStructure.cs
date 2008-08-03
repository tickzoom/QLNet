/*
 Copyright (C) 2008 Toyin Akin (toyin_akin@hotmail.com)
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
    public static partial class Utils {
        //! utility function giving the inflation period for a given date
        public static KeyValuePair<Date, Date> inflationPeriod(Date d, Frequency frequency) {
            Month month = (Month)d.Month;
            int year = d.Year;

            Month startMonth;
            Month endMonth;
            switch (frequency) {
                case Frequency.Annual:
                    startMonth = Month.January;
                    endMonth = Month.December;
                    break;
                case Frequency.Semiannual:
                    startMonth = (Month)(6 * ((int)month - 1) / 6 + 1);
                    endMonth = (Month)(startMonth + 5);
                    break;
                case Frequency.Quarterly:
                    startMonth = (Month)(3 * ((int)month - 1) / 3 + 1);
                    endMonth = (Month)(startMonth + 2);
                    break;
                case Frequency.Monthly:
                    startMonth = endMonth = month;
                    break;
                default:
                    throw new ApplicationException("Frequency not handled: " + frequency);
            }

            Date startDate = new Date(1, startMonth, year);
            Date endDate = Date.endOfMonth(new Date(1, endMonth, year));

            return new KeyValuePair<Date, Date>(startDate, endDate);
        }
    }

    //! Interface for inflation term structures.
    //! \ingroup inflationtermstructures 
    public abstract class InflationTermStructure : TermStructure {
        //! \name Constructors
        //@{
        public InflationTermStructure(Period lag, Frequency frequency, double baseRate, Handle<YieldTermStructure> yTS)
            : this(lag, frequency, baseRate, yTS, new DayCounter()) {
        }
        public InflationTermStructure(Period lag, Frequency frequency, double baseRate, Handle<YieldTermStructure> yTS, DayCounter dayCounter)
            : base(dayCounter) {
            nominalTermStructure_ = yTS;
            lag_ = lag;
            frequency_ = frequency;
            baseRate_ = baseRate;
            if (nominalTermStructure_ != null)
                nominalTermStructure_.registerWith(update);

        }
        public InflationTermStructure(Date referenceDate, Period lag, Frequency frequency, double baseRate, Handle<YieldTermStructure> yTS, Calendar calendar)
            : this(referenceDate, lag, frequency, baseRate, yTS, calendar, new DayCounter()) {
        }
        public InflationTermStructure(Date referenceDate, Period lag, Frequency frequency, double baseRate, Handle<YieldTermStructure> yTS)
            : this(referenceDate, lag, frequency, baseRate, yTS, new Calendar(), new DayCounter()) {
        }
        public InflationTermStructure(Date referenceDate, Period lag, Frequency frequency, double baseRate, Handle<YieldTermStructure> yTS, Calendar calendar, DayCounter dayCounter)
            : base(referenceDate, calendar, dayCounter) {
            nominalTermStructure_ = yTS;
            lag_ = lag;
            frequency_ = frequency;
            baseRate_ = baseRate;

            if (nominalTermStructure_ != null)
                nominalTermStructure_.registerWith(update);

        }
        public InflationTermStructure(int settlementDays, Calendar calendar, Period lag, Frequency frequency, double baseRate, Handle<YieldTermStructure> yTS)
            : this(settlementDays, calendar, lag, frequency, baseRate, yTS, new DayCounter()) {
        }
        public InflationTermStructure(int settlementDays, Calendar calendar, Period lag, Frequency frequency, double baseRate, Handle<YieldTermStructure> yTS, DayCounter dayCounter)
            : base(settlementDays, calendar, dayCounter) {
            nominalTermStructure_ = yTS;
            lag_ = lag;
            frequency_ = frequency;
            baseRate_ = baseRate;

            if (nominalTermStructure_ != null)
                nominalTermStructure_.registerWith(update);

        }
        //@}

        //! \name Inflation interface
        //@{
        public Period lag() {
            return lag_;
        }
        public Frequency frequency() {
            return frequency_;
        }
        public double baseRate() {
            return baseRate_;
        }
        public Handle<YieldTermStructure> nominalTermStructure() {
            return nominalTermStructure_;
        }

        //! minimum (base) date
        //        ! Important in inflation since it starts before nominal
        //            reference date.
        //        
        public abstract Date baseDate();
        // public virtual Date baseDate() { throw new NotSupportedException(); }
        //@}

        protected Handle<YieldTermStructure> nominalTermStructure_;

        // connection with base index:
        //  lag to base date
        //  index
        //  whether or not to connect with the index at the short end
        //  (don't if you have no index set up)
        protected Period lag_;
        protected Frequency frequency_;

        // This next part is required for piecewise- constructors
        // because, for inflation, they need more than just the
        // instruments to build the term structure, since the rate at
        // time 0-lag is non-zero, since we deal (effectively) with
        // "forwards".
        protected virtual void setBaseRate(double r) {
            baseRate_ = r;
        }
        protected double baseRate_;

        // range-checking
        protected new void checkRange(Date d, bool extrapolate) {
            if (!(d >= baseDate()))
                throw new ApplicationException("date (" + d + ") is before base date");

            if (!(extrapolate || allowsExtrapolation() || d <= maxDate()))
                throw new ApplicationException("date (" + d + ") is past max curve date (" + maxDate() + ")");
        }
        protected new void checkRange(double t, bool extrapolate) {
            if (!(t >= timeFromReference(baseDate())))
                throw new ApplicationException("time (" + t + ") is before base date");

            if (!(extrapolate || allowsExtrapolation() || t <= maxTime()))
                throw new ApplicationException("time (" + t + ") is past max curve time (" + maxTime() + ")");
        }
    }


    //! Interface for zero inflation term structures.
    // Child classes use templates but do not want that exposed to
    // general users.
    public abstract class ZeroInflationTermStructure : InflationTermStructure {
        //! \name Constructors
        //@{
        public ZeroInflationTermStructure(DayCounter dayCounter, Period lag, Frequency frequency, double baseZeroRate, Handle<YieldTermStructure> yTS)
            : base(lag, frequency, baseZeroRate, yTS, dayCounter) {
        }

        public ZeroInflationTermStructure(Date referenceDate, Calendar calendar, DayCounter dayCounter, Period lag, Frequency frequency, double baseZeroRate, Handle<YieldTermStructure> yTS)
            : base(referenceDate, lag, frequency, baseZeroRate, yTS, calendar, dayCounter) {
        }

        public ZeroInflationTermStructure(int settlementDays, Calendar calendar, DayCounter dayCounter, Period lag, Frequency frequency, double baseZeroRate, Handle<YieldTermStructure> yTS)
            : base(settlementDays, calendar, lag, frequency, baseZeroRate, yTS, dayCounter) {
        }
        //@}

        //! \name Inspectors
        //@{
        //! zero-coupon inflation rate
        //        ! Essentially the fair rate for a zero-coupon inflation swap
        //            (by definition), i.e. the zero term structure uses yearly
        //            compounding, which is assumed for ZCIIS instrument quotes.
        //        
        public double zeroRate(Date d) {
            return zeroRate(d, false);
        }
        public double zeroRate(Date d, bool extrapolate) {
            base.checkRange(d, extrapolate);
            return zeroRateImpl(timeFromReference(d));
        }
        public double zeroRate(double t) {
            return zeroRate(t, false);
        }
        public double zeroRate(double t, bool extrapolate) {
            base.checkRange(t, extrapolate);
            return zeroRateImpl(t);
        }
        //@}
        //! to be defined in derived classes
        protected abstract double zeroRateImpl(double t);
    }


    //! Base class for year-on-year inflation term structures.
    public abstract class YoYInflationTermStructure : InflationTermStructure {
        //! \name Constructors
        //@{
        public YoYInflationTermStructure(DayCounter dayCounter, Period lag, Frequency frequency, double baseYoYRate, 
                                         Handle<YieldTermStructure> yTS)
            : base(lag, frequency, baseYoYRate, yTS, dayCounter) {
        }

        public YoYInflationTermStructure(Date referenceDate, Calendar calendar, DayCounter dayCounter, Period lag, 
                                         Frequency frequency, double baseYoYRate, Handle<YieldTermStructure> yTS)
            : base(referenceDate, lag, frequency, baseYoYRate, yTS, calendar, dayCounter) {
        }

        public YoYInflationTermStructure(int settlementDays, Calendar calendar, DayCounter dayCounter, Period lag, 
                                         Frequency frequency, double baseYoYRate, Handle<YieldTermStructure> yTS)
            : base(settlementDays, calendar, lag, frequency, baseYoYRate, yTS, dayCounter) {
        }
        //@}

        //! \name Inspectors
        //@{
        //! year-on-year inflation rate
        //! \note this is not the year-on-year swap (YYIIS) rate. 
        public double yoyRate(Date d) {
            return yoyRate(d, false);
        }
        public double yoyRate(Date d, bool extrapolate) {
            base.checkRange(d, extrapolate);
            return yoyRateImpl(timeFromReference(d));
        }
        public double yoyRate(double t) {
            return yoyRate(t, false);
        }
        public double yoyRate(double t, bool extrapolate) {
            base.checkRange(t, extrapolate);
            return yoyRateImpl(t);
        }
        //@}
        //! to be defined in derived classes
        protected abstract double yoyRateImpl(double time);
    }

}
