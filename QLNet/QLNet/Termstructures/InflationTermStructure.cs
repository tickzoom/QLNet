/*
 Copyright (C) 2008 Toyin Akin (toyin_akin@hotmail.com)
 Copyright (C) 2008 Siarhei Novik (snovik@gmail.com)
 Copyright (C) 2008, 2009 , 2010  Andrea Maggiulli (a.maggiulli@gmail.com)
  
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
using QLNet.Time;

namespace QLNet
{
	/// <summary>
	/// Interface for inflation term structures.
	/// </summary>
	public class InflationTermStructure : TermStructure
	{
		public InflationTermStructure()
		{
		}

		public InflationTermStructure(double baseRate, Period observationLag, Frequency frequency, bool indexIsInterpolated, Handle<YieldTermStructure> yTS)
			: this(baseRate, observationLag, frequency, indexIsInterpolated, yTS, new DayCounter(), new Seasonality())
		{
		}

		public InflationTermStructure(double baseRate, Period observationLag, Frequency frequency, bool indexIsInterpolated, Handle<YieldTermStructure> yTS, DayCounter dayCounter)
			: this(baseRate, observationLag, frequency, indexIsInterpolated, yTS, dayCounter, new Seasonality())
		{
		}

		public InflationTermStructure(double baseRate, Period observationLag, Frequency frequency, bool indexIsInterpolated, Handle<YieldTermStructure> yTS, DayCounter dayCounter, Seasonality seasonality)
			: base(dayCounter)
		{
			nominalTermStructure_ = yTS;
			observationLag_ = observationLag;
			frequency_ = frequency;
			indexIsInterpolated_ = indexIsInterpolated;
			baseRate_ = baseRate;
			nominalTermStructure_.registerWith(update);
			setSeasonality(seasonality);
		}

		public InflationTermStructure(Date referenceDate, double baseRate, Period observationLag, Frequency frequency, bool indexIsInterpolated, Handle<YieldTermStructure> yTS)
			: this(referenceDate, baseRate, observationLag, frequency, indexIsInterpolated, yTS, new Calendar(), new DayCounter(), new Seasonality())
		{
		}

		public InflationTermStructure(Date referenceDate, double baseRate, Period observationLag, Frequency frequency, bool indexIsInterpolated, Handle<YieldTermStructure> yTS, Calendar calendar)
			: this(referenceDate, baseRate, observationLag, frequency, indexIsInterpolated, yTS, calendar, new DayCounter(), new Seasonality())
		{
		}

		public InflationTermStructure(Date referenceDate, double baseRate, Period observationLag, Frequency frequency, bool indexIsInterpolated, Handle<YieldTermStructure> yTS, Calendar calendar, DayCounter dayCounter)
			: this(referenceDate, baseRate, observationLag, frequency, indexIsInterpolated, yTS, calendar, dayCounter, new Seasonality())
		{
		}

		public InflationTermStructure(Date referenceDate, double baseRate, Period observationLag, Frequency frequency, bool indexIsInterpolated, Handle<YieldTermStructure> yTS, Calendar calendar, DayCounter dayCounter, Seasonality seasonality)
			: base(referenceDate, calendar, dayCounter)
		{
			nominalTermStructure_ = yTS;
			observationLag_ = observationLag;
			frequency_ = frequency;
			indexIsInterpolated_ = indexIsInterpolated;
			baseRate_ = baseRate;
			nominalTermStructure_.registerWith(update);
			setSeasonality(seasonality);
		}

		public InflationTermStructure(int settlementDays, Calendar calendar, double baseRate, Period observationLag, Frequency frequency, bool indexIsInterpolated, Handle<YieldTermStructure> yTS)
			: this(settlementDays, calendar, baseRate, observationLag, frequency, indexIsInterpolated, yTS, new DayCounter(), new Seasonality())
		{
		}

		public InflationTermStructure(int settlementDays, Calendar calendar, double baseRate, Period observationLag, Frequency frequency, bool indexIsInterpolated, Handle<YieldTermStructure> yTS, DayCounter dayCounter)
			: this(settlementDays, calendar, baseRate, observationLag, frequency, indexIsInterpolated, yTS, dayCounter, new Seasonality())
		{
		}

		public InflationTermStructure(int settlementDays, Calendar calendar, double baseRate, Period observationLag, Frequency frequency, bool indexIsInterpolated, Handle<YieldTermStructure> yTS, DayCounter dayCounter, Seasonality seasonality)
			: base(settlementDays, calendar, dayCounter)
		{
			nominalTermStructure_ = yTS;
			observationLag_ = observationLag;
			frequency_ = frequency;
			indexIsInterpolated_ = indexIsInterpolated;
			baseRate_ = baseRate;
			nominalTermStructure_.registerWith(update);
			setSeasonality(seasonality);
		}

		//! \name Inflation interface
		//@{
		//! The TS observes with a lag that is usually different from the
		//! availability lag of the index.  An inflation rate is given,
		//! by default, for the maturity requested assuming this lag.
		public virtual Period observationLag() { return observationLag_; }
		public virtual Frequency frequency() { return frequency_; }
		public virtual bool indexIsInterpolated() { return indexIsInterpolated_; }
		public virtual double baseRate() { return baseRate_; }
		public virtual Handle<YieldTermStructure> nominalTermStructure()
		{ return nominalTermStructure_; }

		//! minimum (base) date
		/*! Important in inflation since it starts before nominal
			reference date.  Changes depending whether index is
			interpolated or not.  When interpolated the base date
			is just observation lag before nominal.  When not
			interpolated it is the beginning of the relevant period
			(hence it is easy to create interpolated fixings from
			 a not-interpolated curve because interpolation, usually,
			 of fixings is forward looking).
		*/
		public virtual Date baseDate() { return null; }
		//@}

		//! Functions to set and get seasonality.
		/*! Calling setSeasonality with no arguments means unsetting
			as the default is used to choose unsetting.
		*/
		public void setSeasonality() { setSeasonality(new Seasonality()); }
		public void setSeasonality(Seasonality seasonality)
		{
			// always reset, whether with null or new pointer
			seasonality_ = seasonality;
			if (seasonality_ == null)
			{
				if (!seasonality_.isConsistent(this))
					throw new ApplicationException("Seasonality inconsistent with " +
												  "inflation term structure");
			}
			notifyObservers();
		}

		public Seasonality seasonality() { return seasonality_; }
		public bool hasSeasonality() { return seasonality_ != null; }


		protected Handle<YieldTermStructure> nominalTermStructure_;
		protected Period observationLag_;
		protected Frequency frequency_;
		protected bool indexIsInterpolated_;
		protected double baseRate_;

		// This next part is required for piecewise- constructors
		// because, for inflation, they need more than just the
		// instruments to build the term structure, since the rate at
		// time 0-lag is non-zero, since we deal (effectively) with
		// "forwards".
		protected virtual void setBaseRate(double r) { baseRate_ = r; }

		// range-checking
		void checkRange(Date d, bool extrapolate)
		{
			if (d < baseDate())
				throw new ApplicationException("date (" + d + ") is before base date");

			if (!extrapolate && allowsExtrapolation() && d > maxDate())
				throw new ApplicationException("date (" + d + ") is past max curve date ("
												+ maxDate() + ")");
		}

		Seasonality seasonality_;
	}


	//! Interface for zero inflation term structures.
	// Child classes use templates but do not want that exposed to
	// general users.
	public class ZeroInflationTermStructure : InflationTermStructure
	{
		public ZeroInflationTermStructure() { }

		//! \name Constructors
		//@{
		public ZeroInflationTermStructure(DayCounter dayCounter,
										  double baseZeroRate,
										  Period lag,
										  Frequency frequency,
										  bool indexIsInterpolated,
										  Handle<YieldTermStructure> yTS)
			: this(dayCounter, baseZeroRate, lag, frequency, indexIsInterpolated, yTS,
				  new Seasonality()) { }


		public ZeroInflationTermStructure(DayCounter dayCounter,
										  double baseZeroRate,
										  Period observationLag,
										  Frequency frequency,
										  bool indexIsInterpolated,
										  Handle<YieldTermStructure> yTS,
										  Seasonality seasonality)
			: base(baseZeroRate, observationLag, frequency, indexIsInterpolated,
				  yTS, dayCounter, seasonality)
		{ }


		public ZeroInflationTermStructure(Date referenceDate,
										  Calendar calendar,
										  DayCounter dayCounter,
										  double baseZeroRate,
										  Period lag,
										  Frequency frequency,
										  bool indexIsInterpolated,
										  Handle<YieldTermStructure> yTS)
			: this(referenceDate, calendar, dayCounter, baseZeroRate, lag, frequency,
				  indexIsInterpolated, yTS, new Seasonality()) { }

		public ZeroInflationTermStructure(Date referenceDate,
										   Calendar calendar,
										   DayCounter dayCounter,
										   double baseZeroRate,
										   Period observationLag,
										   Frequency frequency,
										   bool indexIsInterpolated,
										   Handle<YieldTermStructure> yTS,
										   Seasonality seasonality)
			: base(referenceDate, baseZeroRate, observationLag, frequency, indexIsInterpolated,
				   yTS, calendar, dayCounter, seasonality) { }



		public ZeroInflationTermStructure(int settlementDays,
										  Calendar calendar,
										  DayCounter dayCounter,
										  double baseZeroRate,
										  Period lag,
										  Frequency frequency,
										  bool indexIsInterpolated,
										  Handle<YieldTermStructure> yTS)
			: this(settlementDays, calendar, dayCounter, baseZeroRate, lag, frequency,
				  indexIsInterpolated, yTS, new Seasonality()) { }

		public ZeroInflationTermStructure(int settlementDays,
										  Calendar calendar,
										  DayCounter dayCounter,
										  double baseZeroRate,
										  Period observationLag,
										  Frequency frequency,
										  bool indexIsInterpolated,
										  Handle<YieldTermStructure> yTS,
										  Seasonality seasonality)
			: base(settlementDays, calendar, baseZeroRate, observationLag, frequency,
				  indexIsInterpolated, yTS, dayCounter, seasonality) { }
		//@}


		//! \name Inspectors
		//@{
		//! zero-coupon inflation rate for an instrument with maturity (pay date) d
		//! that observes with given lag and interpolation.
		//! Since inflation is highly linked to dates (lags, interpolation, months for seasonality, etc)
		//! we do NOT provide a "time" version of the rate lookup.
		/*! Essentially the fair rate for a zero-coupon inflation swap
			(by definition), i.e. the zero term structure uses yearly
			compounding, which is assumed for ZCIIS instrument quotes.
			N.B. by default you get the same as lag and interpolation
			as the term structure.
			If you want to get predictions of RPI/CPI/etc then use an
			index.
		*/
		public double zeroRate(Date d)
		{
			return zeroRate(d, new Period(-1, TimeUnit.Days), false, false);
		}
		public double zeroRate(Date d, Period instObsLag)
		{
			return zeroRate(d, instObsLag, false, false);
		}
		public double zeroRate(Date d, Period instObsLag, bool forceLinearInterpolation)
		{
			return zeroRate(d, instObsLag, forceLinearInterpolation, false);
		}

		public double zeroRate(Date d, Period instObsLag,
							   bool forceLinearInterpolation,
							   bool extrapolate)
		{
			Period useLag = instObsLag;
			if (instObsLag == new Period(-1, TimeUnit.Days))
			{
				useLag = observationLag();
			}

			double zeroRate;
			if (forceLinearInterpolation)
			{
				KeyValuePair<Date, Date> dd = Utils.inflationPeriod(d - useLag, frequency());
				Date ddValue = dd.Value + new Period(1, TimeUnit.Days);
				double dp = ddValue - dd.Key;
				double dt = d - dd.Key;
				// if we are interpolating we only check the exact point
				// this prevents falling off the end at curve maturity
				base.checkRange(d, extrapolate);
				double t1 = timeFromReference(dd.Key);
				double t2 = timeFromReference(ddValue);
				zeroRate = zeroRateImpl(t1) + zeroRateImpl(t2) * (dt / dp);
			}
			else
			{
				if (indexIsInterpolated())
				{
					base.checkRange(d - useLag, extrapolate);
					double t = timeFromReference(d - useLag);
					zeroRate = zeroRateImpl(t);
				}
				else
				{
					KeyValuePair<Date, Date> dd = Utils.inflationPeriod(d - useLag, frequency());
					base.checkRange(dd.Key, extrapolate);
					double t = timeFromReference(dd.Key);
					zeroRate = zeroRateImpl(t);
				}
			}

			if (hasSeasonality())
			{
				zeroRate = seasonality().correctZeroRate(d - useLag, zeroRate, this);
			}

			return zeroRate;
		}

		protected virtual double zeroRateImpl(double t)
		{
			return 0;
		}
	}
}
