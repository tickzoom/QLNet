/*
 Copyright (C) 2008 Toyin Akin (toyin_akin@hotmail.com)
  
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
using System.Text;

namespace QLNet {

	//! Capped and/or floored floating-rate coupon
//    ! The payoff \f$ P \f$ of a capped floating-rate coupon is:
//        \f[ P = N \times T \times \min(a L + b, C). \f]
//        The payoff of a floored floating-rate coupon is:
//        \f[ P = N \times T \times \max(a L + b, F). \f]
//        The payoff of a collared floating-rate coupon is:
//        \f[ P = N \times T \times \min(\max(a L + b, F), C). \f]
//
//        where \f$ N \f$ is the notional, \f$ T \f$ is the accrual
//        time, \f$ L \f$ is the floating rate, \f$ a \f$ is its
//        gearing, \f$ b \f$ is the spread, and \f$ C \f$ and \f$ F \f$
//        the strikes.
//
//        They can be decomposed in the following manner.
//        Decomposition of a capped floating rate coupon:
//        \f[
//        R = \min(a L + b, C) = (a L + b) + \min(C - b - \xi |a| L, 0)
//        \f]
//        where \f$ \xi = sgn(a) \f$. Then:
//        \f[
//        R = (a L + b) + |a| \min(\frac{C - b}{|a|} - \xi L, 0)
//        \f]
//    
	public class CappedFlooredCoupon : FloatingRateCoupon
	{
		// data
		protected FloatingRateCoupon underlying_;
		protected bool isCapped_;
		protected bool isFloored_;
		protected double? cap_;
		protected double? floor_;

        // need by CashFlowVectors
        public CappedFlooredCoupon() { }

		public CappedFlooredCoupon(FloatingRateCoupon underlying, double? cap) : this(underlying, cap, null)
		{
		}
		public CappedFlooredCoupon(FloatingRateCoupon underlying) : this(underlying, null, null)
		{
		}
		public CappedFlooredCoupon(FloatingRateCoupon underlying, double? cap, double? floor) : base(underlying.date(), underlying.nominal(), underlying.accrualStartDate(), underlying.accrualEndDate(), underlying.fixingDays, underlying.index(), underlying.gearing(), underlying.spread(), underlying.refPeriodStart, underlying.refPeriodEnd, underlying.dayCounter(), underlying.isInArrears())
		{
			underlying_ = underlying;
			isCapped_ = false;
			isFloored_ = false;
	
			if (gearing_ > 0)
			{
				if (cap != null)
				{
					isCapped_ = true;
					cap_ = cap;
				}
				if (floor != null)
				{
					floor_ = floor;
					isFloored_ = true;
				}
				}
			else
			{
				  if (cap != null)
				  {
					floor_ = cap;
					isFloored_ = true;
				  }
				  if (floor != null)
				  {
					isCapped_ = true;
					cap_ = floor;
				  }
			}
			if (isCapped_ && isFloored_)
				if (!(cap >= floor))
                    throw new ApplicationException("cap level (" + cap + ") less than floor level (" + floor + ")");
            underlying.registerWith(update);
		}
		//! \name Coupon interface
		//@{
		public override double rate()
		{
			if (underlying_.pricer() == null)
                throw new ApplicationException("pricer not set");

			double swapletRate = underlying_.rate();
			double floorletRate = 0.0;
			if(isFloored_)
				floorletRate = underlying_.pricer().floorletRate(effectiveFloor());
			double capletRate = 0.0;
			if(isCapped_)
				capletRate = underlying_.pricer().capletRate(effectiveCap());
			return swapletRate + floorletRate - capletRate;
		}
        public override double convexityAdjustment()
		{
			return underlying_.convexityAdjustment();
		}
		//@}
		//! cap
		public double cap()
		{
			if ((gearing_ > 0) && isCapped_)
                return cap_.GetValueOrDefault();
			if ((gearing_ < 0) && isFloored_)
                return floor_.GetValueOrDefault();
			return 0.0;
		}
		//! floor
		public double floor()
		{
			if ((gearing_ > 0) && isFloored_)
                return floor_.GetValueOrDefault();
			if ((gearing_ < 0) && isCapped_)
                return cap_.GetValueOrDefault();
			return 0.0;
		}
		//! effective cap of fixing
		public double effectiveCap()
		{
			return (cap_.GetValueOrDefault() - spread())/gearing();
		}
		//! effective floor of fixing
		public double effectiveFloor()
		{
            return (floor_.GetValueOrDefault() - spread()) / gearing();
		}

		public bool isCapped()
		{
			return isCapped_;
		}
		public bool isFloored()
		{
			return isFloored_;
		}

		public override void setPricer(FloatingRateCouponPricer pricer) {
            base.setPricer(pricer);
            underlying_.setPricer(pricer);
		}

        // Factory - for Leg generators
        public virtual CashFlow factory(Date paymentDate, double nominal, Date startDate, Date endDate, int fixingDays, InterestRateIndex index, double gearing, double spread, double? cap, double? floor, Date refPeriodStart, Date refPeriodEnd, DayCounter dayCounter, bool isInArrears)
        {
            return new CappedFlooredCoupon(new FloatingRateCoupon(paymentDate, nominal, startDate, endDate, fixingDays, index, gearing, spread, refPeriodStart, refPeriodEnd, dayCounter, isInArrears), cap, floor);
        }
	}

	public class CappedFlooredIborCoupon : CappedFlooredCoupon
	{
        // need by CashFlowVectors
        public CappedFlooredIborCoupon() {}

        public CappedFlooredIborCoupon(Date paymentDate, double nominal, Date startDate, Date endDate, int fixingDays, IborIndex index, double gearing, double spread, double? cap, double? floor, Date refPeriodStart, Date refPeriodEnd, DayCounter dayCounter)
            : this(paymentDate, nominal, startDate, endDate, fixingDays, index, gearing, spread, cap, floor, refPeriodStart, refPeriodEnd, dayCounter, false)
		{
		}
        public CappedFlooredIborCoupon(Date paymentDate, double nominal, Date startDate, Date endDate, int fixingDays, IborIndex index, double gearing, double spread, double? cap, double? floor, Date refPeriodStart, Date refPeriodEnd)
            : this(paymentDate, nominal, startDate, endDate, fixingDays, index, gearing, spread, cap, floor, refPeriodStart, refPeriodEnd, new DayCounter(), false)
		{
		}
        public CappedFlooredIborCoupon(Date paymentDate, double nominal, Date startDate, Date endDate, int fixingDays, IborIndex index, double gearing, double spread, double? cap, double? floor, Date refPeriodStart)
            : this(paymentDate, nominal, startDate, endDate, fixingDays, index, gearing, spread, cap, floor, refPeriodStart, new Date(), new DayCounter(), false)
		{
		}
        public CappedFlooredIborCoupon(Date paymentDate, double nominal, Date startDate, Date endDate, int fixingDays, IborIndex index, double gearing, double spread, double? cap, double? floor)
            : this(paymentDate, nominal, startDate, endDate, fixingDays, index, gearing, spread, cap, floor, new Date(), new Date(), new DayCounter(), false)
		{
		}
        public CappedFlooredIborCoupon(Date paymentDate, double nominal, Date startDate, Date endDate, int fixingDays, IborIndex index, double gearing, double spread, double? cap)
            : this(paymentDate, nominal, startDate, endDate, fixingDays, index, gearing, spread, cap, null, new Date(), new Date(), new DayCounter(), false)
		{
		}
		public CappedFlooredIborCoupon(Date paymentDate, double nominal, Date startDate, Date endDate, int fixingDays, IborIndex index, double gearing, double spread) : this(paymentDate, nominal, startDate, endDate, fixingDays, index, gearing, spread, null, null, new Date(), new Date(), new DayCounter(), false)
		{
		}
		public CappedFlooredIborCoupon(Date paymentDate, double nominal, Date startDate, Date endDate, int fixingDays, IborIndex index, double gearing) : this(paymentDate, nominal, startDate, endDate, fixingDays, index, gearing, 0.0, null, null, new Date(), new Date(), new DayCounter(), false)
		{
		}
		public CappedFlooredIborCoupon(Date paymentDate, double nominal, Date startDate, Date endDate, int fixingDays, IborIndex index) : this(paymentDate, nominal, startDate, endDate, fixingDays, index, 1.0, 0.0, null, null, new Date(), new Date(), new DayCounter(), false)
		{
		}
        public CappedFlooredIborCoupon(Date paymentDate, double nominal, Date startDate, Date endDate, int fixingDays, IborIndex index, double gearing, double spread, double? cap, double? floor, Date refPeriodStart, Date refPeriodEnd, DayCounter dayCounter, bool isInArrears)
            : base(new IborCoupon(paymentDate, nominal, startDate, endDate, fixingDays, index, gearing, spread, refPeriodStart, refPeriodEnd, dayCounter, isInArrears) as FloatingRateCoupon, cap, floor)
		{
		}

        // Factory - for Leg generators
        public virtual CashFlow factory(Date paymentDate, double nominal, Date startDate, Date endDate, int fixingDays, IborIndex index, double gearing, double spread, double? cap, double? floor, Date refPeriodStart, Date refPeriodEnd, DayCounter dayCounter, bool isInArrears)
        {
            return new CappedFlooredIborCoupon(paymentDate, nominal, startDate, endDate, fixingDays, index, gearing, spread, cap, floor, refPeriodStart, refPeriodEnd, dayCounter, isInArrears);
        }
	}

	public class CappedFlooredCmsCoupon : CappedFlooredCoupon
	{
        // need by CashFlowVectors
        public CappedFlooredCmsCoupon() { }

        public CappedFlooredCmsCoupon(Date paymentDate, double nominal, Date startDate, Date endDate, int fixingDays, SwapIndex index, double gearing, double spread, double? cap, double? floor, Date refPeriodStart, Date refPeriodEnd, DayCounter dayCounter)
            : this(paymentDate, nominal, startDate, endDate, fixingDays, index, gearing, spread, cap, floor, refPeriodStart, refPeriodEnd, dayCounter, false)
		{
		}
        public CappedFlooredCmsCoupon(Date paymentDate, double nominal, Date startDate, Date endDate, int fixingDays, SwapIndex index, double gearing, double spread, double? cap, double? floor, Date refPeriodStart, Date refPeriodEnd)
            : this(paymentDate, nominal, startDate, endDate, fixingDays, index, gearing, spread, cap, floor, refPeriodStart, refPeriodEnd, new DayCounter(), false)
		{
		}
        public CappedFlooredCmsCoupon(Date paymentDate, double nominal, Date startDate, Date endDate, int fixingDays, SwapIndex index, double gearing, double spread, double? cap, double? floor, Date refPeriodStart)
            : this(paymentDate, nominal, startDate, endDate, fixingDays, index, gearing, spread, cap, floor, refPeriodStart, new Date(), new DayCounter(), false)
		{
		}
        public CappedFlooredCmsCoupon(Date paymentDate, double nominal, Date startDate, Date endDate, int fixingDays, SwapIndex index, double gearing, double spread, double? cap, double? floor)
            : this(paymentDate, nominal, startDate, endDate, fixingDays, index, gearing, spread, cap, floor, new Date(), new Date(), new DayCounter(), false)
		{
		}
        public CappedFlooredCmsCoupon(Date paymentDate, double nominal, Date startDate, Date endDate, int fixingDays, SwapIndex index, double gearing, double spread, double? cap)
            : this(paymentDate, nominal, startDate, endDate, fixingDays, index, gearing, spread, cap, null, new Date(), new Date(), new DayCounter(), false)
		{
		}
		public CappedFlooredCmsCoupon(Date paymentDate, double nominal, Date startDate, Date endDate, int fixingDays, SwapIndex index, double gearing, double spread) : this(paymentDate, nominal, startDate, endDate, fixingDays, index, gearing, spread, null, null, new Date(), new Date(), new DayCounter(), false)
		{
		}
		public CappedFlooredCmsCoupon(Date paymentDate, double nominal, Date startDate, Date endDate, int fixingDays, SwapIndex index, double gearing) : this(paymentDate, nominal, startDate, endDate, fixingDays, index, gearing, 0.0, null, null, new Date(), new Date(), new DayCounter(), false)
		{
		}
		public CappedFlooredCmsCoupon(Date paymentDate, double nominal, Date startDate, Date endDate, int fixingDays, SwapIndex index) : this(paymentDate, nominal, startDate, endDate, fixingDays, index, 1.0, 0.0, null, null, new Date(), new Date(), new DayCounter(), false)
		{
		}
        public CappedFlooredCmsCoupon(Date paymentDate, double nominal, Date startDate, Date endDate, int fixingDays, SwapIndex index, double gearing, double spread, double? cap, double? floor, Date refPeriodStart, Date refPeriodEnd, DayCounter dayCounter, bool isInArrears)
            : base(new CmsCoupon(paymentDate, nominal, startDate, endDate, fixingDays, index, gearing, spread, refPeriodStart, refPeriodEnd, dayCounter, isInArrears) as FloatingRateCoupon, cap, floor)
		{
		}

        // Factory - for Leg generators
        public virtual CashFlow factory(Date paymentDate, double nominal, Date startDate, Date endDate, int fixingDays, SwapIndex index, double gearing, double spread, double? cap, double? floor, Date refPeriodStart, Date refPeriodEnd, DayCounter dayCounter, bool isInArrears)
        {
            return new CappedFlooredCmsCoupon(paymentDate, nominal, startDate, endDate, fixingDays, index, gearing, spread, cap, floor, refPeriodStart, refPeriodEnd, dayCounter, isInArrears);
        }
	}

}
