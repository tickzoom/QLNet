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

using System;
using System.Collections.Generic;
using System.Linq;

namespace QLNet
{

    //! Bootstrap traits to use for PiecewiseZeroInflationCurve
    public class YoYInflationTraits
    {
        public BootstrapHelper<YoYInflationTermStructure> helper;
        public static int maxIterations() { return 5; } // calibration is trivial,

        // should be immediate
        public static Date initialDate(YoYInflationTermStructure t)
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

        public static double initialValue(YoYInflationTermStructure t)
        {
            return t.baseRate();
        }

        public static double initialGuess() { return 0.02; }

        // further guesses
        public static double guess(YoYInflationTermStructure termStructur, Date date)
        {
            return 0.02;    // initial guess at flat inflation
        }
        // possible constraints based on previous values
        public static double minValueAfter(int size, List<double> rate)
        {
            return -0.3 + Const.QL_Epsilon;
        }

        public static double maxValueAfter(int size, List<double> rate)
        {
            return 0.5 - Const.QL_Epsilon;
        }

        // update with new guess
        public static void updateGuess(List<double> data, double level, int i)
        {
            data[i] = level;
        }
    }

    //! Piecewise zero-inflation term structure
    public abstract class PiecewiseYoYInflationCurve<Interpolator> :
        InterpolatedYoYInflationCurve<Interpolator>,
        IGenericCurve<YoYInflationTermStructure> where Interpolator : class, IInterpolationFactory
    {
        #region InterpolatedCurve
 
        #endregion

        #region BootstrapTraits
        //protected BootstrapTraits<ZeroInflationTermStructure> traits_;
        protected YoYInflationTraits traits_;
        public Date initialDate(YoYInflationTermStructure c)
        {
            return YoYInflationTraits.initialDate(c);
        }
        public double initialValue(YoYInflationTermStructure c) { return YoYInflationTraits.initialValue(c); }
        public bool dummyInitialValue() { return YoYInflationTraits.dummyInitialValue(); }
        public double initialGuess() { return YoYInflationTraits.initialGuess(); }
        public double guess(YoYInflationTermStructure c, Date d) { return YoYInflationTraits.guess(c, d); }
        public double minValueAfter(int s, List<double> l) { return YoYInflationTraits.minValueAfter(s, l); }
        public double maxValueAfter(int s, List<double> l) { return YoYInflationTraits.maxValueAfter(s, l); }
        public void updateGuess(List<double> data, double discount, int i) { YoYInflationTraits.updateGuess(data, discount, i); }
        public int maxIterations() { return YoYInflationTraits.maxIterations(); }

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
        public List<BootstrapHelper<YoYInflationTermStructure>> instruments_;
        public List<BootstrapHelper<YoYInflationTermStructure>> instruments()
        {
            return instruments_;
        }
        protected Date latestReference_;
        protected IBootStrap<YoYInflationTermStructure> bootstrap_;
        public bool updated()
        {
            throw new NotImplementedException();
        }
        public bool moving()
        {
            throw new NotImplementedException();
        }
        #endregion

        public PiecewiseYoYInflationCurve(Date referenceDate,
                                            Calendar calendar,
                                            DayCounter dayCounter,
                                            Period lag,
                                            Frequency frequency,
                                            bool indexIsInterpolated,
                                            double baseZeroRate,
                                            Handle<YieldTermStructure> nominalTS, Interpolator i)
            : base(referenceDate, calendar, dayCounter, lag, frequency, indexIsInterpolated, baseZeroRate, nominalTS, i) { }

    }

    public class PiecewiseYoYInflationCurve<Interpolator, BootStrap, Traits>
        : PiecewiseYoYInflationCurve<Interpolator>
        where Interpolator : class, IInterpolationFactory, new()
        where BootStrap : IterativeBootstrap<YoYInflationTermStructure>, new()
        where Traits : YoYInflationTraits, new()
    {

        public PiecewiseYoYInflationCurve(
                 Date referenceDate,
                 Calendar calendar,
                 DayCounter dayCounter,
                 Period lag,
                 Frequency frequency,
                 bool indexIsInterpolated,
                 double baseZeroRate,
                 Handle<YieldTermStructure> nominalTS,
                 List<BootstrapHelper<YoYInflationTermStructure>> instruments,
                 double accuracy,
                 Interpolator i)
            : base(referenceDate, calendar, dayCounter, lag, frequency, indexIsInterpolated, baseZeroRate, nominalTS, i)
        {
            interpolator_ = i;
            instruments_ = instruments;
            accuracy_ = accuracy;
            traits_ = new Traits();
            bootstrap_ = new BootStrap();
            bootstrap_.setup(this);
        }


        protected override void performCalculations()
        {
            bootstrap_.calculate();
        }
        public override void update()
        {
            base.update();
        }
    }

    public class PiecewiseYoYInflationCurve<Interpolator, Traits>
    : PiecewiseYoYInflationCurve<Interpolator, IterativeBootstrap<YoYInflationTermStructure>, Traits>
        where Interpolator : class, IInterpolationFactory, new()
        where Traits : YoYInflationTraits, new()
    {

        public PiecewiseYoYInflationCurve(
                 Date referenceDate,
                 Calendar calendar,
                 DayCounter dayCounter,
                 Period lag,
                 Frequency frequency,
                 bool indexIsInterpolated,
                 double baseZeroRate,
                 Handle<YieldTermStructure> nominalTS,
                 List<BootstrapHelper<YoYInflationTermStructure>> instruments,
                 double accuracy,
                 Interpolator i)
            : base(referenceDate, calendar, dayCounter, lag, frequency, indexIsInterpolated, baseZeroRate, nominalTS,
                instruments, accuracy, i)
        { }

        public PiecewiseYoYInflationCurve(
         Date referenceDate,
         Calendar calendar,
         DayCounter dayCounter,
         Period lag,
         Frequency frequency,
         bool indexIsInterpolated,
         double baseZeroRate,
         Handle<YieldTermStructure> nominalTS,
         List<BootstrapHelper<YoYInflationTermStructure>> instruments,
         Interpolator i)
            : this(referenceDate, calendar, dayCounter, lag, frequency, indexIsInterpolated, baseZeroRate, nominalTS,
                instruments, 1.0e-12, i)
        { }
   
    }
}

