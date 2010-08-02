/*
 Copyright (C) 2008 Siarhei Novik (snovik@gmail.com)
 Copyright (C) 2008, 2009 , 2010 Andrea Maggiulli (a.maggiulli@gmail.com)
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

using System;
using QLNet.Currencies;
using QLNet.Time;

namespace QLNet
{
	/// <summary>
	/// base class for Inter-Bank-Offered-Rate indexes (e.g. Libor, etc.)
	/// </summary>
	public class IborIndex : InterestRateIndex
	{
		public bool EndOfMonth { get; private set; }
		public BusinessDayConvention convention_ { get; protected set; }
		public Handle<YieldTermStructure> termStructure_ { get; protected set; }

		public IborIndex()
		{
		}

		public IborIndex(string familyName, Period tenor, int settlementDays, Currency currency, Calendar fixingCalendar, BusinessDayConvention convention, bool endOfMonth, DayCounter dayCounter)
			: this(familyName, tenor, settlementDays, currency, fixingCalendar, convention, endOfMonth, dayCounter, new Handle<YieldTermStructure>())
		{
		}

		public IborIndex(string familyName, Period tenor, int settlementDays, Currency currency, Calendar fixingCalendar, BusinessDayConvention convention, bool endOfMonth, DayCounter dayCounter, Handle<YieldTermStructure> h) :
			base(familyName, tenor, settlementDays, currency, fixingCalendar, dayCounter)
		{
			convention_ = convention;
			termStructure_ = h;
			EndOfMonth = endOfMonth;

			// observer interface
			if (termStructure_!= null && !termStructure_.IsEmpty)
			{
				termStructure_.registerWith(update);
			}
		}

		[Obsolete("Use convention_ property instead.")]
		public BusinessDayConvention businessDayConvention()
		{
			return convention_;
		}

		[Obsolete("Use termStructure_ property instead.")]
		public Handle<YieldTermStructure> forwardingTermStructure()
		{
			return termStructure_;
		}

		[Obsolete("Use endOfMonth property instead.")]
		public bool endOfMonth()
		{
			return EndOfMonth;
		}

		public override Date maturityDate(Date valueDate)
		{
			return fixingCalendar().advance(valueDate, tenor_, convention_, EndOfMonth);
		}

		protected override double forecastFixing(Date fixingDate)
		{
			if (termStructure_.IsEmpty)
			{
				throw new ArgumentException("null term structure set to this instance of " + name());
			}

			Date fixingValueDate = valueDate(fixingDate);
			Date endValueDate = maturityDate(fixingValueDate);
			double fixingDiscount = termStructure_.link.discount(fixingValueDate);
			double endDiscount = termStructure_.link.discount(endValueDate);
			double fixingPeriod = dayCounter().yearFraction(fixingValueDate, endValueDate);
			return (fixingDiscount / endDiscount - 1.0) / fixingPeriod;
		}

		/// <summary>
		/// Returns a copy of itself linked to a different forwarding curve.
		/// </summary>
		/// <param name="forwarding"></param>
		/// <returns></returns>
		public virtual IborIndex clone(Handle<YieldTermStructure> forwarding)
		{
			return new IborIndex(familyName(), tenor(), fixingDays(), currency(), fixingCalendar(), convention_, EndOfMonth, dayCounter(), forwarding);
		}
	}
}
