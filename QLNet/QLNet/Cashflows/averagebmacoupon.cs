/*
 Copyright (C) 2008, 2009 Siarhei Novik (snovik@gmail.com)
  
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
using QLNet.Time;

namespace QLNet
{
	/// <summary>
	/// Average BMA coupon
	/// 
	/// Coupon paying a BMA index, where the coupon rate is a
	/// weighted average of relevant fixings.
	/// 
	/// The weighted average is computed based on the
	/// actual calendar days for which a given fixing is valid and
	/// contributing to the given interest period.
	/// 
	/// Before weights are computed, the fixing schedule is adjusted
	/// for the index's fixing day gap. See rate() method for details.
	/// </summary>
	public class AverageBMACoupon : FloatingRateCoupon
	{
		private readonly Schedule fixingSchedule_;
		private int bmaCutoffDays;

		// double gearing = 1.0, double spread = 0.0, 
		// Date refPeriodStart = Date(), Date refPeriodEnd = Date(), DayCounter dayCounter = DayCounter());
		public AverageBMACoupon(double nominal, Date paymentDate, Date startDate, Date endDate, BMAIndex index,
								double gearing, double spread, Date refPeriodStart, Date refPeriodEnd, DayCounter dayCounter)
			: base(nominal, paymentDate, startDate, endDate, index.fixingDays(), index, gearing, spread,
						 refPeriodStart, refPeriodEnd, dayCounter, false)
		{
			fixingSchedule_ = index.fixingSchedule(
								index.fixingCalendar()
									.advance(startDate, new Period(-index.fixingDays() + bmaCutoffDays, TimeUnit.Days),
												   BusinessDayConvention.Preceding), endDate);
			setPricer(new AverageBMACouponPricer());
		}

		//! \name FloatingRateCoupon interface
		//@{
		//! not applicable here; use fixingDates() instead
		public override Date fixingDate()
		{
			throw new ApplicationException("no single fixing date for average-BMA coupon");
		}

		//! fixing dates of the rates to be averaged
		public List<Date> fixingDates() { return fixingSchedule_.dates(); }

		//! not applicable here; use indexFixings() instead
		public override double indexFixing()
		{
			throw new ApplicationException("no single fixing date for average-BMA coupon");
		}

		//! fixings of the underlying index to be averaged
		public List<double> indexFixings() { return fixingSchedule_.dates().Select(d => index_.fixing(d)).ToList(); }

		public override double convexityAdjustment()
		{
			throw new ApplicationException("not defined for average-BMA coupon");
		}
	}
}
