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

	//! CMS coupon class
//    ! \warning This class does not perform any date adjustment,
//                 i.e., the start and end date passed upon construction
//                 should be already rolled to a business day.
//    
	public class CmsCoupon : FloatingRateCoupon
	{
        // need by CashFlowVectors
        public CmsCoupon() { }

		public CmsCoupon(Date paymentDate, double nominal, Date startDate, Date endDate, int fixingDays, SwapIndex swapIndex, double gearing, double spread, Date refPeriodStart, Date refPeriodEnd, DayCounter dayCounter) : this(paymentDate, nominal, startDate, endDate, fixingDays, swapIndex, gearing, spread, refPeriodStart, refPeriodEnd, dayCounter, false)
		{
		}
		public CmsCoupon(Date paymentDate, double nominal, Date startDate, Date endDate, int fixingDays, SwapIndex swapIndex, double gearing, double spread, Date refPeriodStart, Date refPeriodEnd) : this(paymentDate, nominal, startDate, endDate, fixingDays, swapIndex, gearing, spread, refPeriodStart, refPeriodEnd, new DayCounter(), false)
		{
		}
		public CmsCoupon(Date paymentDate, double nominal, Date startDate, Date endDate, int fixingDays, SwapIndex swapIndex, double gearing, double spread, Date refPeriodStart) : this(paymentDate, nominal, startDate, endDate, fixingDays, swapIndex, gearing, spread, refPeriodStart, new Date(), new DayCounter(), false)
		{
		}
		public CmsCoupon(Date paymentDate, double nominal, Date startDate, Date endDate, int fixingDays, SwapIndex swapIndex, double gearing, double spread) : this(paymentDate, nominal, startDate, endDate, fixingDays, swapIndex, gearing, spread, new Date(), new Date(), new DayCounter(), false)
		{
		}
		public CmsCoupon(Date paymentDate, double nominal, Date startDate, Date endDate, int fixingDays, SwapIndex swapIndex, double gearing) : this(paymentDate, nominal, startDate, endDate, fixingDays, swapIndex, gearing, 0.0, new Date(), new Date(), new DayCounter(), false)
		{
		}
		public CmsCoupon(Date paymentDate, double nominal, Date startDate, Date endDate, int fixingDays, SwapIndex swapIndex) : this(paymentDate, nominal, startDate, endDate, fixingDays, swapIndex, 1.0, 0.0, new Date(), new Date(), new DayCounter(), false)
		{
		}
		public CmsCoupon(Date paymentDate, double nominal, Date startDate, Date endDate, int fixingDays, SwapIndex swapIndex, double gearing, double spread, Date refPeriodStart, Date refPeriodEnd, DayCounter dayCounter, bool isInArrears) : base(paymentDate, nominal, startDate, endDate, fixingDays, swapIndex, gearing, spread, refPeriodStart, refPeriodEnd, dayCounter, isInArrears)
		{
			swapIndex_ = swapIndex;
		}
		//! \name Inspectors
		//@{
		public SwapIndex swapIndex()
		{
			return swapIndex_;
		}
		//@}
		//! \name Visitability
		//@{
        //public void accept(ref AcyclicVisitor v)
        //{
        //    Visitor<CmsCoupon> v1 = v as Visitor<CmsCoupon>;
        //    if (v1 != 0)
        //        v1.visit( this);
        //    else
        //        base.accept(ref v);
        //}
		//@}
		private SwapIndex swapIndex_;
	}


	//! helper class building a sequence of capped/floored cms-rate coupons
	public class CmsLeg
	{
		public CmsLeg(Schedule schedule, SwapIndex swapIndex)
		{
			schedule_ = schedule;
			swapIndex_ = swapIndex;
			paymentAdjustment_ = BusinessDayConvention.Following;
			inArrears_ = false;
			zeroPayments_ = false;
		}
		public CmsLeg withNotionals(double notional)
		{
            notionals_ = new List<double>(); notionals_.Add(notional);
			return this;
		}
		public CmsLeg withNotionals(List<double> notionals)
		{
			notionals_ = notionals;
			return this;
		}
		public CmsLeg withPaymentDayCounter(DayCounter dayCounter)
		{
			paymentDayCounter_ = dayCounter;
			return this;
		}
		public CmsLeg withPaymentAdjustment(BusinessDayConvention convention)
		{
			paymentAdjustment_ = convention;
			return this;
		}
		public CmsLeg withFixingDays(int fixingDays)
		{
            fixingDays_ = new List<int>(); fixingDays_.Add(fixingDays);
			return this;
		}
		public CmsLeg withFixingDays(List<int> fixingDays)
		{
			fixingDays_ = fixingDays;
			return this;
		}
		public CmsLeg withGearings(double gearing)
		{
			gearings_ = new List<double>(); gearings_.Add(gearing);
			return this;
		}
		public CmsLeg withGearings(List<double> gearings)
		{
			gearings_ = gearings;
			return this;
		}
		public CmsLeg withSpreads(double spread)
		{
            spreads_ = new List<double>(); spreads_.Add(spread);
			return this;
		}
		public CmsLeg withSpreads(List<double> spreads)
		{
			spreads_ = spreads;
			return this;
		}
		public CmsLeg withCaps(double cap)
		{
            caps_ = new List<double>(); caps_.Add(cap);
			return this;
		}
		public CmsLeg withCaps(List<double> caps)
		{
			caps_ = caps;
			return this;
		}
		public CmsLeg withFloors(double floor)
		{
            floors_ = new List<double>(); floors_.Add(floor);
			return this;
		}
		public CmsLeg withFloors(List<double> floors)
		{
			floors_ = floors;
			return this;
		}
		public CmsLeg inArrears()
		{
			return inArrears(true);
		}
		public CmsLeg inArrears(bool flag)
		{
			inArrears_ = flag;
			return this;
		}
		public CmsLeg withZeroPayments()
		{
			return withZeroPayments(true);
		}
		public CmsLeg withZeroPayments(bool flag)
		{
			zeroPayments_ = flag;
			return this;
		}

        public List<CashFlow> value()
		{
            return CashFlowVectors.FloatingLeg<SwapIndex, CmsCoupon, CappedFlooredCmsCoupon>(notionals_, schedule_, swapIndex_, paymentDayCounter_, paymentAdjustment_, fixingDays_, gearings_, spreads_, caps_, floors_, inArrears_, zeroPayments_);
		}

		private Schedule schedule_;
		private SwapIndex swapIndex_;
		private List<double> notionals_;
		private DayCounter paymentDayCounter_;
		private BusinessDayConvention paymentAdjustment_;
		private List<int> fixingDays_;
		private List<double> gearings_;
		private List<double> spreads_;
		private List<double> caps_;
        private List<double> floors_;
		private bool inArrears_;
		private bool zeroPayments_;
	}

}
