/*
 Copyright (C) 2008 Siarhei Novik (snovik@gmail.com)
 Copyright (C) 2008 Toyin Akin (toyin_akin@hotmail.com)
 * 
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

using QLNet.Currencies;
using QLNet.Time;

namespace QLNet
{
	public class SwapIndex : InterestRateIndex
	{
		protected IborIndex iborIndex_;
		protected Period fixedLegTenor_;
		private readonly BusinessDayConvention fixedLegConvention_;
		private readonly bool exogenousDiscount_;
		private readonly Handle<YieldTermStructure> discount_;

		public SwapIndex()
		{
		}

		public SwapIndex(string familyName, Period tenor, int settlementDays, Currency currency, Calendar calendar, Period fixedLegTenor, BusinessDayConvention fixedLegConvention, DayCounter fixedLegDayCounter, IborIndex iborIndex)
			: this(familyName, tenor, settlementDays, currency, calendar, fixedLegTenor, fixedLegConvention, fixedLegDayCounter, iborIndex, new Handle<YieldTermStructure>())
		{
		}

		public SwapIndex(string familyName, Period tenor, int settlementDays, Currency currency, Calendar calendar, Period fixedLegTenor, BusinessDayConvention fixedLegConvention, DayCounter fixedLegDayCounter, IborIndex iborIndex, Handle<YieldTermStructure> discountingTermStructure)
			: base(familyName, tenor, settlementDays, currency, calendar, fixedLegDayCounter)
		{
			tenor_ = tenor;
			iborIndex_ = iborIndex;
			fixedLegTenor_ = fixedLegTenor;
			fixedLegConvention_ = fixedLegConvention;
			exogenousDiscount_ = true;
			
			discount_ = discountingTermStructure ?? new Handle<YieldTermStructure>();

			iborIndex_.registerWith(update);
		}

		public override Date maturityDate(Date valueDate)
		{
			Date fixDate = fixingDate(valueDate);
			return underlyingSwap(fixDate).maturityDate();
		}

		public Period fixedLegTenor()
		{
			return fixedLegTenor_;
		}

		public BusinessDayConvention fixedLegConvention()
		{
			return fixedLegConvention_;
		}

		public IborIndex iborIndex()
		{
			return iborIndex_;
		}

		public Handle<YieldTermStructure> forwardingTermStructure()
		{
			return iborIndex_.forwardingTermStructure();
		}

		public bool exogenousDiscount()
		{
			return exogenousDiscount_;
		}

		// \warning Relinking the term structure underlying the index will not have effect on the returned swap.
		// recheck
		public VanillaSwap underlyingSwap(Date fixingDate)
		{
			double fixedRate = 0.0;

			if (exogenousDiscount_)
			{
				return new MakeVanillaSwap(tenor_, iborIndex_, fixedRate)
					.withEffectiveDate(valueDate(fixingDate))
					.withFixedLegCalendar(fixingCalendar())
					.withFixedLegDayCount(dayCounter_)
					.withFixedLegTenor(fixedLegTenor_)
					.withFixedLegConvention(fixedLegConvention_)
					.withFixedLegTerminationDateConvention(fixedLegConvention_)
					.withDiscountingTermStructure(discount_)
					.value();
			}
			
			return new MakeVanillaSwap(tenor_, iborIndex_, fixedRate)
				.withEffectiveDate(valueDate(fixingDate))
				.withFixedLegCalendar(fixingCalendar())
				.withFixedLegDayCount(dayCounter_)
				.withFixedLegTenor(fixedLegTenor_)
				.withFixedLegConvention(fixedLegConvention_)
				.withFixedLegTerminationDateConvention(fixedLegConvention_)
				.value();
		}

		public virtual SwapIndex clone(Handle<YieldTermStructure> forwarding)
		{
			if (exogenousDiscount_)
			{
				return new SwapIndex(familyName(),
				                     tenor(),
				                     fixingDays(),
				                     currency(),
				                     fixingCalendar(),
				                     fixedLegTenor(),
				                     fixedLegConvention(),
				                     dayCounter(),
				                     iborIndex_.clone(forwarding),
				                     discount_);
			}
			
			return new SwapIndex(familyName(),
			                     tenor(),
			                     fixingDays(),
			                     currency(),
			                     fixingCalendar(),
			                     fixedLegTenor(),
			                     fixedLegConvention(),
			                     dayCounter(),
			                     iborIndex_.clone(forwarding));
		}

		protected override double forecastFixing(Date fixingDate)
		{
			return underlyingSwap(fixingDate).fairRate();
		}
	}
}
