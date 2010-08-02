using System;
using QLNet.Time;

namespace QLNet
{
	/// <summary>
	/// Rate helper for bootstrapping over interest-rate futures prices
	/// </summary>
	public class FuturesRateHelper : RateHelper
	{
		private double yearFraction_;
		private Handle<Quote> convAdj_;

		// constructors. special case when convexityAdjustment is really delivered as Quote
		public FuturesRateHelper(Handle<Quote> price, Date immDate, int lengthInMonths, Calendar calendar,
		                         BusinessDayConvention convention, bool endOfMonth, DayCounter dayCounter)
			: this(price, immDate, lengthInMonths, calendar, convention, endOfMonth, dayCounter, new Handle<Quote>()) { }
		public FuturesRateHelper(Handle<Quote> price, Date immDate, int nMonths, Calendar calendar,
		                         BusinessDayConvention convention, bool endOfMonth, DayCounter dayCounter,
		                         Handle<Quote> convexityAdjustment)
			: base(price)
		{
			convAdj_ = convexityAdjustment;

			if (!IMM.isIMMdate(immDate, false)) throw new ArgumentException(immDate + "is not a valid IMM date");
			earliestDate_ = immDate;

			latestDate_ = calendar.advance(immDate, new Period(nMonths, TimeUnit.Months), convention, endOfMonth);
			yearFraction_ = dayCounter.yearFraction(earliestDate_, latestDate_);

			convAdj_.registerWith(update);
		}

		// overloaded constructors
		public FuturesRateHelper(double price, Date immDate, int nMonths, Calendar calendar, BusinessDayConvention convention,
		                         bool endOfMonth, DayCounter dayCounter, double convAdj)
			: base(price)
		{
			convAdj_ = new Handle<Quote>(new SimpleQuote(convAdj));

			if (!IMM.isIMMdate(immDate, false)) throw new ArgumentException(immDate + "is not a valid IMM date");
			earliestDate_ = immDate;

			latestDate_ = calendar.advance(immDate, new Period(nMonths, TimeUnit.Months), convention, endOfMonth);
			yearFraction_ = dayCounter.yearFraction(earliestDate_, latestDate_);
		}

		public FuturesRateHelper(Handle<Quote> price, Date immDate, IborIndex i, Handle<Quote> convAdj)
			: base(price)
		{
			convAdj_ = convAdj;

			if (!IMM.isIMMdate(immDate, false)) throw new ArgumentException(immDate + "is not a valid IMM date");
			earliestDate_ = immDate;

			Calendar cal = i.fixingCalendar();
			latestDate_ = cal.advance(immDate, i.tenor(), i.businessDayConvention());
			yearFraction_ = i.dayCounter().yearFraction(earliestDate_, latestDate_);

			convAdj_.registerWith(update);
		}

		public FuturesRateHelper(double price, Date immDate, IborIndex i, double convAdj)
			: base(price)
		{
			convAdj_ = new Handle<Quote>(new SimpleQuote(convAdj));

			if (!IMM.isIMMdate(immDate, false)) throw new ArgumentException(immDate + "is not a valid IMM date");
			earliestDate_ = immDate;

			Calendar cal = i.fixingCalendar();
			latestDate_ = cal.advance(immDate, i.tenor(), i.businessDayConvention());
			yearFraction_ = i.dayCounter().yearFraction(earliestDate_, latestDate_);
		}


		/////////////////////////////////////////////////////
		//! RateHelper interface
		public override double impliedQuote()
		{
			if (termStructure_ == null) throw new ArgumentException("term structure not set");

			double forwardRate = (termStructure_.discount(earliestDate_) /
			                      termStructure_.discount(latestDate_) - 1) / yearFraction_;
			double convAdj = convAdj_.link.value();

			if (convAdj < 0) throw new ArgumentException("Negative (" + convAdj + ") futures convexity adjustment");
			double futureRate = forwardRate + convAdj;
			return 100.0 * (1.0 - futureRate);
		}


		/////////////////////////////////////////////////////
		//! FuturesRateHelper inspectors
		public double convexityAdjustment()
		{
			return convAdj_.empty() ? 0.0 : convAdj_.link.value();
		}
	}
}