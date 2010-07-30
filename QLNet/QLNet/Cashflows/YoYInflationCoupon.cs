/*
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

using QLNet.Time;

namespace QLNet
{
	/// <summary>
	/// Coupon paying a YoY-inflation type index
	/// </summary>
	public class YoYInflationCoupon : InflationCoupon
	{
		private YoYInflationIndex yoyIndex_;
		protected double gearing_;
		protected double spread_;

		public YoYInflationCoupon(Date paymentDate, double nominal, Date startDate, Date endDate, int fixingDays, YoYInflationIndex yoyIndex, Period observationLag, DayCounter dayCounter)
			: this(paymentDate, nominal, startDate, endDate, fixingDays, yoyIndex, observationLag, dayCounter, 1.0, 0.0, null, null)
		{
		}

		public YoYInflationCoupon(Date paymentDate, double nominal, Date startDate, Date endDate, int fixingDays, YoYInflationIndex yoyIndex, Period observationLag, DayCounter dayCounter, double gearing, double spread, Date refPeriodStart, Date refPeriodEnd)
			: base(paymentDate, nominal, startDate, endDate, fixingDays, yoyIndex, observationLag, dayCounter, refPeriodStart, refPeriodEnd)
		{
			yoyIndex_ = yoyIndex;
			gearing_ = gearing;
			spread_ = spread;
		}

		/// <summary>
		/// index gearing, i.e. multiplicative coefficient for the index
		/// </summary>
		/// <returns></returns>
		public double gearing()
		{
			return gearing_;
		}
		
		/// <summary>
		/// spread paid over the fixing of the underlying index
		/// </summary>
		/// <returns></returns>
		public double spread()
		{
			return spread_;
		}
		
		public double adjustedFixing()
		{
			return (rate() - spread()) / gearing();
		}
		
		public YoYInflationIndex yoyIndex()
		{
			return yoyIndex_;
		}
		
		protected override bool checkPricerImpl(InflationCouponPricer i)
		{
			return (i is YoYInflationCouponPricer);
		}
	}
}
