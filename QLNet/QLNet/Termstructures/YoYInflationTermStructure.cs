using System.Collections.Generic;
using QLNet.Time;

namespace QLNet
{
	/// <summary>
	/// Base class for year-on-year inflation term structures.
	/// </summary>
	public class YoYInflationTermStructure : InflationTermStructure
	{
		public YoYInflationTermStructure()
		{
		}

		public YoYInflationTermStructure(DayCounter dayCounter, double baseYoYRate, Period lag, Frequency frequency, bool indexIsInterpolated, Handle<YieldTermStructure> yieldTS)
			: this(dayCounter, baseYoYRate, lag, frequency, indexIsInterpolated, yieldTS, new Seasonality())
		{
		}

		public YoYInflationTermStructure(DayCounter dayCounter, double baseYoYRate, Period observationLag, Frequency frequency, bool indexIsInterpolated, Handle<YieldTermStructure> yTS, Seasonality seasonality)
			: base(baseYoYRate, observationLag, frequency, indexIsInterpolated, yTS, dayCounter, seasonality)
		{
		}

		public YoYInflationTermStructure(Date referenceDate, Calendar calendar, DayCounter dayCounter, double baseYoYRate, Period lag, Frequency frequency, bool indexIsInterpolated, Handle<YieldTermStructure> yieldTS)
			: this(referenceDate, calendar, dayCounter, baseYoYRate, lag, frequency, indexIsInterpolated, yieldTS, new Seasonality())
		{
		}

		public YoYInflationTermStructure(Date referenceDate, Calendar calendar, DayCounter dayCounter, double baseYoYRate, Period observationLag, Frequency frequency, bool indexIsInterpolated, Handle<YieldTermStructure> yTS, Seasonality seasonality)
			: base(referenceDate, baseYoYRate, observationLag, frequency, indexIsInterpolated, yTS, calendar, dayCounter, seasonality)
		{
		}

		public YoYInflationTermStructure(int settlementDays, Calendar calendar, DayCounter dayCounter, double baseYoYRate, Period lag, Frequency frequency, bool indexIsInterpolated, Handle<YieldTermStructure> yieldTS)
			: this(settlementDays, calendar, dayCounter, baseYoYRate, lag, frequency, indexIsInterpolated, yieldTS, new Seasonality())
		{
		}

		public YoYInflationTermStructure(int settlementDays, Calendar calendar, DayCounter dayCounter, double baseYoYRate, Period observationLag, Frequency frequency, bool indexIsInterpolated, Handle<YieldTermStructure> yTS, Seasonality seasonality)
			: base(settlementDays, calendar, baseYoYRate, observationLag, frequency, indexIsInterpolated, yTS, dayCounter, seasonality)
		{
		}

		//! year-on-year inflation rate, forceLinearInterpolation
		//! is relative to the frequency of the TS.
		//! Since inflation is highly linked to dates (lags, interpolation, months for seasonality etc)
		//! we do NOT provide a "time" version of the rate lookup.
		/*! \note this is not the year-on-year swap (YYIIS) rate. */
		public double yoyRate(Date d)
		{
			return yoyRate(d, new Period(-1, TimeUnit.Days), false, false);
		}

		public double yoyRate(Date d, Period instObsLag)
		{
			return yoyRate(d, instObsLag, false, false);
		}

		public double yoyRate(Date d, Period instObsLag, bool forceLinearInterpolation)
		{
			return yoyRate(d, instObsLag, forceLinearInterpolation, false);
		}

		public double yoyRate(Date d, Period instObsLag, bool forceLinearInterpolation,
		                      bool extrapolate)
		{
			Period useLag = instObsLag;
			if (instObsLag == new Period(-1, TimeUnit.Days))
			{
				useLag = observationLag();
			}

			double yoyRate;
			if (forceLinearInterpolation)
			{
				KeyValuePair<Date, Date> dd = Utils.inflationPeriod(d - useLag, frequency());
				Date ddValue = dd.Value + new Period(1, TimeUnit.Days);
				double dp = ddValue - dd.Key;
				double dt = (d - useLag) - dd.Key;
				// if we are interpolating we only check the exact point
				// this prevents falling off the end at curve maturity
				base.checkRange(d, extrapolate);
				double t1 = timeFromReference(dd.Key);
				double t2 = timeFromReference(dd.Value);
				yoyRate = yoyRateImpl(t1) + (yoyRateImpl(t2) - yoyRateImpl(t1)) * (dt / dp);
			}
			else
			{
				if (indexIsInterpolated())
				{
					base.checkRange(d - useLag, extrapolate);
					double t = timeFromReference(d - useLag);
					yoyRate = yoyRateImpl(t);
				}
				else
				{
					KeyValuePair<Date, Date> dd = Utils.inflationPeriod(d - useLag, frequency());
					base.checkRange(dd.Key, extrapolate);
					double t = timeFromReference(dd.Key);
					yoyRate = yoyRateImpl(t);
				}
			}

			if (hasSeasonality())
			{
				yoyRate = seasonality().correctYoYRate(d - useLag, yoyRate, this);
			}
			return yoyRate;
		}

		protected virtual double yoyRateImpl(double time) { return 0; }
	}
}