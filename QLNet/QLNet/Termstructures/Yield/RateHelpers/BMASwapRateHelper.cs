/*
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
using QLNet.Time;

namespace QLNet
{
	/// <summary>
	/// Rate helper for bootstrapping over BMA swap rates
	/// </summary>
	public class BMASwapRateHelper : RelativeDateRateHelper
	{
		protected Period tenor_;
		protected int settlementDays_;
		protected Calendar calendar_;
		protected Period bmaPeriod_;
		protected BusinessDayConvention bmaConvention_;
		protected DayCounter bmaDayCount_;
		protected BMAIndex bmaIndex_;
		protected IborIndex iborIndex_;

		protected BMASwap swap_;
		protected RelinkableHandle<YieldTermStructure> termStructureHandle_ = new RelinkableHandle<YieldTermStructure>();

		public BMASwapRateHelper(Handle<Quote> liborFraction, Period tenor, int settlementDays, Calendar calendar,
			// bma leg
						  Period bmaPeriod, BusinessDayConvention bmaConvention, DayCounter bmaDayCount, BMAIndex bmaIndex,
			// ibor leg
						  IborIndex iborIndex)
			: base(liborFraction)
		{
			tenor_ = tenor;
			settlementDays_ = settlementDays;
			calendar_ = calendar;
			bmaPeriod_ = bmaPeriod;
			bmaConvention_ = bmaConvention;
			bmaDayCount_ = bmaDayCount;
			bmaIndex_ = bmaIndex;
			iborIndex_ = iborIndex;

			iborIndex_.registerWith(update);
			bmaIndex_.registerWith(update);

			initializeDates();
		}

		//! \name RateHelper interface
		public override double impliedQuote()
		{
			if (termStructure_ == null)
				throw new ApplicationException("term structure not set");
			// we didn't register as observers - force calculation
			swap_.recalculate();
			return swap_.fairLiborFraction();
		}

		public override void setTermStructure(YieldTermStructure t)
		{
			// do not set the relinkable handle as an observer -
			// force recalculation when needed
			termStructureHandle_.linkTo(t, false);
			base.setTermStructure(t);
		}

		protected override void initializeDates()
		{
			earliestDate_ = calendar_.advance(evaluationDate_, new Period(settlementDays_, TimeUnit.Days),
											  BusinessDayConvention.Following);

			Date maturity = earliestDate_ + tenor_;

			// dummy BMA index with curve/swap arguments
			BMAIndex clonedIndex = new BMAIndex(termStructureHandle_);

			Schedule bmaSchedule = new MakeSchedule().from(earliestDate_).to(maturity)
						  .withTenor(bmaPeriod_)
						  .withCalendar(bmaIndex_.fixingCalendar())
						  .withConvention(bmaConvention_)
						  .backwards()
						  .value();

			Schedule liborSchedule = new MakeSchedule().from(earliestDate_).to(maturity)
						  .withTenor(iborIndex_.tenor())
						  .withCalendar(iborIndex_.fixingCalendar())
						  .withConvention(iborIndex_.businessDayConvention())
						  .endOfMonth(iborIndex_.endOfMonth())
						  .backwards()
						  .value();

			swap_ = new BMASwap(BMASwap.Type.Payer, 100.0, liborSchedule, 0.75, // arbitrary
								0.0, iborIndex_, iborIndex_.dayCounter(), bmaSchedule, clonedIndex, bmaDayCount_);
			swap_.setPricingEngine(new DiscountingSwapEngine(iborIndex_.forwardingTermStructure()));

			Date d = calendar_.adjust(swap_.maturityDate(), BusinessDayConvention.Following);
			int w = d.weekday();
			Date nextWednesday = (w >= 4) ? d + new Period((11 - w), TimeUnit.Days) :
											d + new Period((4 - w), TimeUnit.Days);
			latestDate_ = clonedIndex.valueDate(clonedIndex.fixingCalendar().adjust(nextWednesday));
		}
	}
}
