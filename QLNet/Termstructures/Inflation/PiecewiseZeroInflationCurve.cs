/*
 Copyright (C) 2008, 2009 , 2010  Andrea Maggiulli (a.maggiulli@gmail.com)
 Copyright (C) 2010 Philippe Real (ph_real@hotmail.com)

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

/*! \file piecewisezeroinflationcurve.hpp
    \brief Piecewise zero-inflation term structure
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace QLNet
{

    //! Bootstrap traits to use for PiecewiseZeroInflationCurve
    public class ZeroInflationTraits
    {
        public BootstrapHelper<ZeroInflationTermStructure> helper;
        public static int maxIterations() { return 5; } // calibration is trivial,
        
        // should be immediate
        public static Date initialDate(ZeroInflationTermStructure t)
        {
            if (t.indexIsInterpolated())
            {
                return t.referenceDate() - t.observationLag();
            }
            else
            {
                return Utils.inflationPeriod(t.referenceDate() - t.observationLag(),
                                       t.frequency()).Key;
            }

        }
        
        public static bool dummyInitialValue() { return false; }
        
        public static double initialValue(ZeroInflationTermStructure t)
        {
            return t.baseRate();
        }
        
        public static double initialGuess() { return 0.02; }
        
        // further guesses
        public static double guess(ZeroInflationTermStructure termStructur, Date date)
        {
            return 0.02;    // initial guess at flat inflation
        }
        // possible constraints based on previous values
        public static double minValueAfter(int size, List<double> rate)
        {
            return -0.1 + Const.QL_Epsilon;
        }

        public static double maxValueAfter(int size, List<double> rate)
        {
            return 0.3 - Const.QL_Epsilon;
        }

        // update with new guess
        public static void updateGuess(List<double> data,double level,int i)
        {
            data[i] = level;
        }
    }

    //! Piecewise zero-inflation term structure
    public abstract class PiecewiseZeroInflationCurve<Interpolator> : 
        InterpolatedZeroInflationCurve<Interpolator>, 
        IGenericCurve<ZeroInflationTermStructure> where Interpolator : class, IInterpolationFactory
    {
        #region InterpolatedCurve
        /*
        protected PiecewiseZeroInflationCurve(Date referenceDate, Calendar cal, DayCounter dc) : base(referenceDate, cal, dc)
        {
        }

        protected PiecewiseZeroInflationCurve(int settlementDays, Calendar cal, DayCounter dc) : base(settlementDays, cal, dc)
        {
        }
        //public List<double> times_ { get; set; }
        //public List<double> times() { calculate(); return times_; }
        //public Interpolation interpolation_ { get; set; }
        //public IInterpolationFactory interpolator_ { get; set; }
        public Dictionary<Date, double> nodes()
        {
            calculate();
            Dictionary<Date, double> results = new Dictionary<Date, double>();
            dates_.ForEach((i, x) => results.Add(x, data_[i]));
            return results;
        }
        public void setupInterpolation()
        {
            interpolation_ = interpolator_.interpolate(times_, times_.Count, data_);
        }
        public object Clone()
        {
            InterpolatedCurve copy = this.MemberwiseClone() as InterpolatedCurve;
            copy.times_ = new List<double>(times_);
            copy.data_ = new List<double>(data_);
            copy.dates_ = new List<Date>(dates_);
            copy.interpolator_ = interpolator_;
            copy.setupInterpolation();
            return copy;
        }*/
        #endregion

        #region BootstrapTraits
        //protected BootstrapTraits<ZeroInflationTermStructure> traits_;
        protected ZeroInflationTraits traits_;
        public Date initialDate(ZeroInflationTermStructure c)
        {
            return ZeroInflationTraits.initialDate(c);
        }
        public double initialValue(ZeroInflationTermStructure c) { return ZeroInflationTraits.initialValue(c); }
        public bool dummyInitialValue() { return ZeroInflationTraits.dummyInitialValue(); }
        public double initialGuess() { return ZeroInflationTraits.initialGuess(); }
        public double guess(ZeroInflationTermStructure c, Date d) { return ZeroInflationTraits.guess(c, d); }
        public double minValueAfter(int s, List<double> l) { return ZeroInflationTraits.minValueAfter(s, l); }
        public double maxValueAfter(int s, List<double> l) { return ZeroInflationTraits.maxValueAfter(s, l); }
        public void updateGuess(List<double> data, double discount, int i) { ZeroInflationTraits.updateGuess(data, discount, i); }
        public int maxIterations() { return ZeroInflationTraits.maxIterations(); }
  
        // these are dummy methods (for the sake of ITraits and should not be called directly
        public double discountImpl(Interpolation i, double t) { throw new NotSupportedException(); }
        public double zeroYieldImpl(Interpolation i, double t) { throw new NotSupportedException(); }
        public double forwardImpl(Interpolation i, double t) { throw new NotSupportedException(); }
        #endregion

        #region Interface curve
        public override Date baseDate()
        {
            calculate();
            return dates_.First();
        }
        public override Date maxDate()
        {
            calculate();
            return dates_.Last();
        }
        public double accuracy_;
        public double accuracy()
        {
            return accuracy_;
        }
        public List<BootstrapHelper<ZeroInflationTermStructure>> instruments_;
        public List<BootstrapHelper<ZeroInflationTermStructure>> instruments()
        {
            return instruments_;
        }
        protected Date latestReference_;
        protected IBootStrap<ZeroInflationTermStructure> bootstrap_;
        public bool updated()
        {
            throw new NotImplementedException();
        }
        public bool moving()
        {
            throw new NotImplementedException();
        }
        #endregion

        public PiecewiseZeroInflationCurve( Date referenceDate,
                                            Calendar calendar,
                                            DayCounter dayCounter,
                                            Period lag,
                                            Frequency frequency,
                                            bool indexIsInterpolated,
                                            double baseZeroRate,
                                            Handle<YieldTermStructure> nominalTS, Interpolator i)
            : base(referenceDate, calendar, dayCounter, lag, frequency,indexIsInterpolated, baseZeroRate, nominalTS,i) { }

    }

    public class PiecewiseZeroInflationCurve<Interpolator, BootStrap, Traits>
        : PiecewiseZeroInflationCurve<Interpolator>
        where Interpolator : class, IInterpolationFactory, new()
        where BootStrap     :   IterativeBootstrap<ZeroInflationTermStructure>, new()
        where Traits        :   ZeroInflationTraits, new()
    {

        public PiecewiseZeroInflationCurve(
                 Date referenceDate,
                 Calendar calendar,
                 DayCounter dayCounter,
                 Period lag,
                 Frequency frequency,
                 bool indexIsInterpolated,
                 double baseZeroRate,
                 Handle<YieldTermStructure> nominalTS,
                 List<BootstrapHelper<ZeroInflationTermStructure>> instruments,
                 double accuracy,
                 Interpolator i)
        : base(referenceDate, calendar, dayCounter,lag, frequency,indexIsInterpolated, baseZeroRate,nominalTS,i)
        {
            interpolator_ = i;
            instruments_ = instruments;
            accuracy_ = accuracy;
            traits_ = new Traits();
            bootstrap_ = new BootStrap();
            bootstrap_.setup(this);
        }

        /*public PiecewiseZeroInflationCurve(
                 Date referenceDate,
                 Calendar calendar,
                 DayCounter dayCounter,
                 Period lag,
                 Frequency frequency,
                 bool indexIsInterpolated,
                 double baseZeroRate,
                 Handle<YieldTermStructure> nominalTS,
                 List<BootstrapHelper<ZeroInflationTermStructure>> instruments,
                 Interpolator i)
        :this(referenceDate,calendar,dayCounter,lag,frequency,baseZeroRate,nominalTS,
            instruments,1.0e-12, i)
        {}

        public PiecewiseZeroInflationCurve(
             Date referenceDate,
             Calendar calendar,
             DayCounter dayCounter,
             Period lag,
             Frequency frequency,
             double baseZeroRate,
             Handle<YieldTermStructure> nominalTS,
             List<BootstrapHelper<ZeroInflationTermStructure>> instruments,
             double accuracy)
            : this(referenceDate, calendar, dayCounter, lag, frequency, baseZeroRate, nominalTS,
                instruments, accuracy, new Interpolator())
        { }

        public PiecewiseZeroInflationCurve(
             Date referenceDate,
             Calendar calendar,
             DayCounter dayCounter,
             Period lag,
             Frequency frequency,
             double baseZeroRate,
             Handle<YieldTermStructure> nominalTS,
             List<BootstrapHelper<ZeroInflationTermStructure>> instruments)
            : this(referenceDate, calendar, dayCounter, lag, frequency, baseZeroRate, nominalTS,
                instruments, 1.0e-12, new Interpolator())
        { }
        */

        protected override void performCalculations(){
            bootstrap_.calculate();
        }
        public override void update()
        {
            base.update();
        }
    }

        public class PiecewiseZeroInflationCurve<Interpolator,Traits>
        : PiecewiseZeroInflationCurve<Interpolator, IterativeBootstrap<ZeroInflationTermStructure>,Traits >
        where Interpolator  : class, IInterpolationFactory, new()
        where Traits        :   ZeroInflationTraits, new()
    {

        public PiecewiseZeroInflationCurve(
                 Date referenceDate,
                 Calendar calendar,
                 DayCounter dayCounter,
                 Period lag,
                 Frequency frequency,
                 bool indexIsInterpolated,
                 double baseZeroRate,
                 Handle<YieldTermStructure> nominalTS,
                 List<BootstrapHelper<ZeroInflationTermStructure>> instruments,
                 double accuracy,
                 Interpolator i)
            : base(referenceDate, calendar, dayCounter, lag, frequency,indexIsInterpolated, baseZeroRate, nominalTS,
                instruments, accuracy, i)
        {}

        public PiecewiseZeroInflationCurve(
         Date referenceDate,
         Calendar calendar,
         DayCounter dayCounter,
         Period lag,
         Frequency frequency,
         bool indexIsInterpolated,
         double baseZeroRate,
         Handle<YieldTermStructure> nominalTS,
         List<BootstrapHelper<ZeroInflationTermStructure>> instruments,
         Interpolator i)
                : this(referenceDate, calendar, dayCounter, lag, frequency,indexIsInterpolated, baseZeroRate, nominalTS,
                    instruments, 1.0e-12, i)
        { }
        /*
        public PiecewiseZeroInflationCurve(
             Date referenceDate,
             Calendar calendar,
             DayCounter dayCounter,
             Period lag,
             Frequency frequency,
             double baseZeroRate,
             Handle<YieldTermStructure> nominalTS,
             List<BootstrapHelper<ZeroInflationTermStructure>> instruments,
             double accuracy)
            : this(referenceDate, calendar, dayCounter, lag, frequency, baseZeroRate, nominalTS,
                instruments, accuracy, new Interpolator())
        { }

        public PiecewiseZeroInflationCurve(
             Date referenceDate,
             Calendar calendar,
             DayCounter dayCounter,
             Period lag,
             Frequency frequency,
             double baseZeroRate,
             Handle<YieldTermStructure> nominalTS,
             List<BootstrapHelper<ZeroInflationTermStructure>> instruments)
            : this(referenceDate, calendar, dayCounter, lag, frequency, baseZeroRate, nominalTS,
                instruments, 1.0e-12, new Interpolator())
        { }
        */
    }
}

