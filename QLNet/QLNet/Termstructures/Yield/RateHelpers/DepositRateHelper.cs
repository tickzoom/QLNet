using System;
using QLNet.Currencies;
using QLNet.Time;

namespace QLNet
{
	/// <summary>
	/// Rate helper for bootstrapping over deposit rates
	/// </summary>
	public class DepositRateHelper : RelativeDateRateHelper
	{
		private Date fixingDate_;
		IborIndex iborIndex_;
		// need to init this because it is used before the handle has any link, i.e. setTermStructure will be used after ctor
		RelinkableHandle<YieldTermStructure> termStructureHandle_ = new RelinkableHandle<YieldTermStructure>();

		///////////////////////////////////////////
		// constructors
		public DepositRateHelper(Handle<Quote> rate, Period tenor, int fixingDays, Calendar calendar,
		                         BusinessDayConvention convention, bool endOfMonth, DayCounter dayCounter) :
		                         	base(rate)
		{
			iborIndex_ = new IborIndex("no-fix", tenor, fixingDays, new Currency(), calendar, convention,
			                           endOfMonth, dayCounter, termStructureHandle_);
			initializeDates();
		}

		public DepositRateHelper(double rate, Period tenor, int fixingDays, Calendar calendar,
		                         BusinessDayConvention convention, bool endOfMonth, DayCounter dayCounter) :
		                         	base(rate)
		{
			iborIndex_ = new IborIndex("no-fix", tenor, fixingDays, new Currency(), calendar, convention,
			                           endOfMonth, dayCounter, termStructureHandle_);
			initializeDates();
		}

		public DepositRateHelper(Handle<Quote> rate, IborIndex i)
			: base(rate)
		{
			iborIndex_ = new IborIndex("no-fix", // never take fixing into account
			                           i.tenor(), i.fixingDays(), new Currency(),
			                           i.fixingCalendar(), i.businessDayConvention(),
			                           i.endOfMonth(), i.dayCounter(), termStructureHandle_);
			initializeDates();
		}
		public DepositRateHelper(double rate, IborIndex i)
			: base(rate)
		{
			iborIndex_ = new IborIndex("no-fix", // never take fixing into account
			                           i.tenor(), i.fixingDays(), new Currency(),
			                           i.fixingCalendar(), i.businessDayConvention(),
			                           i.endOfMonth(), i.dayCounter(), termStructureHandle_);
			initializeDates();
		}


		/////////////////////////////////////////
		//! RateHelper interface
		public override double impliedQuote()
		{
			if (termStructure_ == null) throw new ArgumentException("term structure not set");
			return iborIndex_.fixing(fixingDate_, true);
		}

		public override void setTermStructure(YieldTermStructure t)
		{
			// no need to register---the index is not lazy
			termStructureHandle_.linkTo(t, false);
			base.setTermStructure(t);
		}

		protected override void initializeDates()
		{
			earliestDate_ = iborIndex_.fixingCalendar().advance(evaluationDate_, iborIndex_.fixingDays(), TimeUnit.Days);
			latestDate_ = iborIndex_.maturityDate(earliestDate_);
			fixingDate_ = iborIndex_.fixingCalendar().advance(earliestDate_, -iborIndex_.fixingDays(), TimeUnit.Days);
		}
	}
}