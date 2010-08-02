using System;
using QLNet.Currencies;
using QLNet.Time;

namespace QLNet
{
	/// <summary>
	/// Rate helper for bootstrapping over FRA rates
	/// </summary>
	public class FRARateHelper : RelativeDateRateHelper
	{
		private Date fixingDate_;
		private Period periodToStart_;
		private IborIndex iborIndex_;
		
		// need to init this because it is used before the handle has any link, i.e. setTermStructure will be used after ctor
		RelinkableHandle<YieldTermStructure> termStructureHandle_ = new RelinkableHandle<YieldTermStructure>();


		public FRARateHelper(Handle<Quote> rate, int monthsToStart, int monthsToEnd, int fixingDays,
		                     Calendar calendar, BusinessDayConvention convention, bool endOfMonth,
		                     DayCounter dayCounter) :
		                     	base(rate)
		{
			periodToStart_ = new Period(monthsToStart, TimeUnit.Months);

			if (!(monthsToEnd > monthsToStart)) throw new ArgumentException("monthsToEnd must be grater than monthsToStart");
			iborIndex_ = new IborIndex("no-fix", new Period(monthsToEnd - monthsToStart, TimeUnit.Months), fixingDays,
			                           new Currency(), calendar, convention, endOfMonth, dayCounter, termStructureHandle_);
			initializeDates();
		}

		public FRARateHelper(double rate, int monthsToStart, int monthsToEnd, int fixingDays, Calendar calendar,
		                     BusinessDayConvention convention, bool endOfMonth, DayCounter dayCounter)
			: base(rate)
		{
			periodToStart_ = new Period(monthsToStart, TimeUnit.Months);

			if (!(monthsToEnd > monthsToStart)) throw new ArgumentException("monthsToEnd must be grater than monthsToStart");
			iborIndex_ = new IborIndex("no-fix", new Period(monthsToEnd - monthsToStart, TimeUnit.Months), fixingDays,
			                           new Currency(), calendar, convention, endOfMonth, dayCounter, termStructureHandle_);
			initializeDates();
		}

		public FRARateHelper(Handle<Quote> rate, int monthsToStart, IborIndex i)
			: base(rate)
		{
			periodToStart_ = new Period(monthsToStart, TimeUnit.Months);

			iborIndex_ = new IborIndex("no-fix",  // never take fixing into account
			                           i.tenor(), i.fixingDays(), new Currency(),
			                           i.fixingCalendar(), i.businessDayConvention(),
			                           i.endOfMonth(), i.dayCounter(), termStructureHandle_);

			initializeDates();
		}

		public FRARateHelper(double rate, int monthsToStart, IborIndex i)
			: base(rate)
		{
			periodToStart_ = new Period(monthsToStart, TimeUnit.Months);

			iborIndex_ = new IborIndex("no-fix",  // never take fixing into account
			                           i.tenor(), i.fixingDays(), new Currency(),
			                           i.fixingCalendar(), i.businessDayConvention(),
			                           i.endOfMonth(), i.dayCounter(), termStructureHandle_);

			initializeDates();
		}

		public override void setTermStructure(YieldTermStructure t)
		{
			// no need to register---the index is not lazy
			termStructureHandle_.linkTo(t, false);
			base.setTermStructure(t);
		}


		/////////////////////////////////////////////////////////
		//! RateHelper interface
		public override double impliedQuote()
		{
			if (termStructure_ == null) throw new ArgumentException("term structure not set");
			return iborIndex_.fixing(fixingDate_, true);
		}

		protected override void initializeDates()
		{
			// why not using index_->fixingDays instead of settlementDays_
			Date settlement = iborIndex_.fixingCalendar().advance(evaluationDate_, iborIndex_.fixingDays(), TimeUnit.Days);
			earliestDate_ = iborIndex_.fixingCalendar().advance(settlement, periodToStart_,
			                                                    iborIndex_.businessDayConvention(), iborIndex_.endOfMonth());
			latestDate_ = iborIndex_.maturityDate(earliestDate_);
			fixingDate_ = iborIndex_.fixingDate(earliestDate_);
		}
	}
}