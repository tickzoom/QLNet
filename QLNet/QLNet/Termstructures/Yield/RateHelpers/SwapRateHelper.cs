using System;
using QLNet.Time;

namespace QLNet
{
	/// <summary>
	/// Rate helper for bootstrapping over swap rates
	/// </summary>
	public class SwapRateHelper : RelativeDateRateHelper
	{
		protected Period tenor_;
		protected Calendar calendar_;
		protected BusinessDayConvention fixedConvention_;
		protected Frequency fixedFrequency_;
		protected DayCounter fixedDayCount_;
		protected IborIndex iborIndex_;
		protected VanillaSwap swap_;
		// need to init this because it is used before the handle has any link, i.e. setTermStructure will be used after ctor
		RelinkableHandle<YieldTermStructure> termStructureHandle_ = new RelinkableHandle<YieldTermStructure>();
		protected Handle<Quote> spread_;
		protected Period fwdStart_;


		#region ctors
		//public SwapRateHelper(Quote rate, SwapIndex swapIndex) :
		//    this(rate, swapIndex, new SimpleQuote(), new Period(0, TimeUnit.Days)) { }
		//public SwapRateHelper(Quote rate, SwapIndex swapIndex, Quote spread) :
		//    this(rate, swapIndex, spread, new Period(0, TimeUnit.Days)) { }
		public SwapRateHelper(Handle<Quote> rate, SwapIndex swapIndex, Handle<Quote> spread, Period fwdStart)
			: base(rate)
		{
			tenor_ = swapIndex.tenor();
			calendar_ = swapIndex.fixingCalendar();
			fixedConvention_ = swapIndex.fixedLegConvention();
			fixedFrequency_ = swapIndex.fixedLegTenor().frequency();
			fixedDayCount_ = swapIndex.dayCounter();
			iborIndex_ = swapIndex.iborIndex();
			spread_ = spread;
			fwdStart_ = fwdStart;

			// add observers
			iborIndex_.registerWith(update);
			spread_.registerWith(update);

			initializeDates();
		}

		public SwapRateHelper(Handle<Quote> rate, Period tenor, Calendar calendar,
		                      Frequency fixedFrequency, BusinessDayConvention fixedConvention, DayCounter fixedDayCount,
		                      IborIndex iborIndex) :
		                      	this(rate, tenor, calendar, fixedFrequency, fixedConvention, fixedDayCount, iborIndex,
		                      	     new Handle<Quote>(), new Period(0, TimeUnit.Days)) { }

		public SwapRateHelper(double rate, Period tenor, Calendar calendar,
		                      Frequency fixedFrequency, BusinessDayConvention fixedConvention, DayCounter fixedDayCount,
		                      IborIndex iborIndex) :
		                      	this(rate, tenor, calendar, fixedFrequency, fixedConvention, fixedDayCount, iborIndex,
		                      	     new Handle<Quote>(), new Period(0, TimeUnit.Days)) { }

		//public SwapRateHelper(Quote rate, Period tenor, Calendar calendar,
		//               Frequency fixedFrequency, BusinessDayConvention fixedConvention, DayCounter fixedDayCount,
		//               IborIndex iborIndex, Quote spread) :
		//    this(rate, tenor, calendar, fixedFrequency, fixedConvention, fixedDayCount, iborIndex,
		//         spread, new Period(0, TimeUnit.Days)) { }
		public SwapRateHelper(Handle<Quote> rate, Period tenor, Calendar calendar,
		                      // fixed leg
		                      Frequency fixedFrequency, BusinessDayConvention fixedConvention, DayCounter fixedDayCount,
		                      // floating leg
		                      IborIndex iborIndex, Handle<Quote> spread, Period fwdStart)
			: base(rate)
		{
			tenor_ = tenor;
			calendar_ = calendar;
			fixedConvention_ = fixedConvention;
			fixedFrequency_ = fixedFrequency;
			fixedDayCount_ = fixedDayCount;
			iborIndex_ = iborIndex;
			spread_ = spread;
			fwdStart_ = fwdStart;

			// add observers
			iborIndex_.registerWith(update);
			spread_.registerWith(update);

			initializeDates();
		}


		//public SwapRateHelper(double rate, Period tenor, Calendar calendar,
		//               Frequency fixedFrequency, BusinessDayConvention fixedConvention, DayCounter fixedDayCount,
		//               IborIndex iborIndex) :
		//    this(rate, tenor, calendar, fixedFrequency, fixedConvention, fixedDayCount, iborIndex,
		//         new SimpleQuote(), new Period(0, TimeUnit.Days)) { }
		//public SwapRateHelper(double rate, Period tenor, Calendar calendar,
		//               Frequency fixedFrequency, BusinessDayConvention fixedConvention, DayCounter fixedDayCount,
		//               IborIndex iborIndex, Quote spread) :
		//    this(rate, tenor, calendar, fixedFrequency, fixedConvention, fixedDayCount, iborIndex,
		//         spread, new Period(0, TimeUnit.Days)) { }
		public SwapRateHelper(double rate, Period tenor, Calendar calendar,
		                      // fixed leg
		                      Frequency fixedFrequency, BusinessDayConvention fixedConvention, DayCounter fixedDayCount,
		                      // floating leg
		                      IborIndex iborIndex, Handle<Quote> spread, Period fwdStart)
			: base(rate)
		{
			tenor_ = tenor;
			calendar_ = calendar;
			fixedConvention_ = fixedConvention;
			fixedFrequency_ = fixedFrequency;
			fixedDayCount_ = fixedDayCount;
			iborIndex_ = iborIndex;
			spread_ = spread;
			fwdStart_ = fwdStart;

			// add observers
			iborIndex_.registerWith(update);
			spread_.registerWith(update);

			initializeDates();
		}

		//public SwapRateHelper(double rate, SwapIndex swapIndex)
		//    : this(rate, swapIndex, new SimpleQuote()) { }
		//public SwapRateHelper(double rate, SwapIndex swapIndex, Quote spread)
		//    : this(rate, swapIndex, spread, new Period(0, TimeUnit.Days)) { }
		public SwapRateHelper(double rate, SwapIndex swapIndex, Handle<Quote> spread, Period fwdStart)
			: base(rate)
		{
			tenor_ = swapIndex.tenor();
			calendar_ = swapIndex.fixingCalendar();
			fixedConvention_ = swapIndex.fixedLegConvention();
			fixedFrequency_ = swapIndex.fixedLegTenor().frequency();
			fixedDayCount_ = swapIndex.dayCounter();
			iborIndex_ = swapIndex.iborIndex();
			spread_ = spread;
			fwdStart_ = fwdStart;

			// add observers
			iborIndex_.registerWith(update);
			spread_.registerWith(update);

			initializeDates();
		}
		#endregion


		protected override void initializeDates()
		{
			// dummy ibor index with curve/swap arguments
			IborIndex clonedIborIndex = iborIndex_.clone(termStructureHandle_);

			// do not pass the spread here, as it might be a Quote i.e. it can dinamically change
			swap_ = new MakeVanillaSwap(tenor_, clonedIborIndex, 0.0, fwdStart_)
				.withFixedLegDayCount(fixedDayCount_)
				.withFixedLegTenor(new Period(fixedFrequency_))
				.withFixedLegConvention(fixedConvention_)
				.withFixedLegTerminationDateConvention(fixedConvention_)
				.withFixedLegCalendar(calendar_)
				.withFloatingLegCalendar(calendar_);

			earliestDate_ = swap_.startDate();

			// Usually...
			latestDate_ = swap_.maturityDate();
			// ...but due to adjustments, the last floating coupon might
			// need a later date for fixing
#if QL_USE_INDEXED_COUPON
            FloatingRateCoupon lastFloating = (FloatingRateCoupon)swap_.floatingLeg()[swap_.floatingLeg().Count - 1];
            Date fixingValueDate = iborIndex_.valueDate(lastFloating.fixingDate());
            Date endValueDate = iborIndex_.maturityDate(fixingValueDate);
            latestDate_ = Date.Max(latestDate_, endValueDate);
#endif
		}

		public override void setTermStructure(YieldTermStructure t)
		{
			// do not set the relinkable handle as an observer -
			// force recalculation when needed
			termStructureHandle_.linkTo(t, false);
			base.setTermStructure(t);
		}

		////////////////////////////////////////////////////
		//! RateHelper interface
		public override double impliedQuote()
		{
			if (termStructure_ == null) throw new ArgumentException("term structure not set");
			// we didn't register as observers - force calculation
			swap_.recalculate();                // it is from lazy objects
			// weak implementation... to be improved
			const double basisPoint = 1.0e-4;
			double floatingLegNPV = swap_.floatingLegNPV();
			double spread = this.spread();
			double spreadNPV = swap_.floatingLegBPS() / basisPoint * spread;
			double totNPV = -(floatingLegNPV + spreadNPV);
			double result = totNPV / (swap_.fixedLegBPS() / basisPoint);
			return result;
		}

		//! \name SwapRateHelper inspectors
		public double spread() { return spread_.empty() ? 0.0 : spread_.link.value(); }
		public VanillaSwap swap() { return swap_; }
		public Period forwardStart() { return fwdStart_; }
	}
}