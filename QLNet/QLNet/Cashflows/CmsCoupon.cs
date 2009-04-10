/*
 Copyright (C) 2008 Toyin Akin (toyin_akin@hotmail.com)
 Copyright (C) 2009 Siarhei Novik (snovik@gmail.com)
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QLNet {
    //! CMS coupon class
    //    ! \warning This class does not perform any date adjustment,
    //                 i.e., the start and end date passed upon construction
    //                 should be already rolled to a business day.
    //    
    public class CmsCoupon : FloatingRateCoupon {
        // need by CashFlowVectors
        public CmsCoupon() { }

        public CmsCoupon(double nominal, Date paymentDate, Date startDate, Date endDate, int fixingDays, SwapIndex swapIndex, double gearing, double spread, Date refPeriodStart, Date refPeriodEnd, DayCounter dayCounter)
            : this(nominal, paymentDate, startDate, endDate, fixingDays, swapIndex, gearing, spread, refPeriodStart, refPeriodEnd, dayCounter, false) {
        }
        public CmsCoupon(double nominal, Date paymentDate, Date startDate, Date endDate, int fixingDays, SwapIndex swapIndex, double gearing, double spread, Date refPeriodStart, Date refPeriodEnd)
            : this(nominal, paymentDate, startDate, endDate, fixingDays, swapIndex, gearing, spread, refPeriodStart, refPeriodEnd, new DayCounter(), false) {
        }
        public CmsCoupon(double nominal, Date paymentDate, Date startDate, Date endDate, int fixingDays, SwapIndex swapIndex, double gearing, double spread, Date refPeriodStart)
            : this(nominal, paymentDate, startDate, endDate, fixingDays, swapIndex, gearing, spread, refPeriodStart, new Date(), new DayCounter(), false) {
        }
        public CmsCoupon(double nominal, Date paymentDate, Date startDate, Date endDate, int fixingDays, SwapIndex swapIndex, double gearing, double spread)
            : this(nominal, paymentDate, startDate, endDate, fixingDays, swapIndex, gearing, spread, new Date(), new Date(), new DayCounter(), false) {
        }
        public CmsCoupon(double nominal, Date paymentDate, Date startDate, Date endDate, int fixingDays, SwapIndex swapIndex, double gearing)
            : this(nominal, paymentDate, startDate, endDate, fixingDays, swapIndex, gearing, 0.0, new Date(), new Date(), new DayCounter(), false) {
        }
        public CmsCoupon(double nominal, Date paymentDate, Date startDate, Date endDate, int fixingDays, SwapIndex swapIndex)
            : this(nominal, paymentDate, startDate, endDate, fixingDays, swapIndex, 1.0, 0.0, new Date(), new Date(), new DayCounter(), false) {
        }
        public CmsCoupon(double nominal, Date paymentDate, Date startDate, Date endDate, int fixingDays, SwapIndex swapIndex, double gearing, double spread, Date refPeriodStart, Date refPeriodEnd, DayCounter dayCounter, bool isInArrears)
            : base(nominal, paymentDate, startDate, endDate, fixingDays, swapIndex, gearing, spread, refPeriodStart, refPeriodEnd, dayCounter, isInArrears) {
            swapIndex_ = swapIndex;
        }
        //! \name Inspectors
        //@{
        public SwapIndex swapIndex() {
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
    public class CmsLeg : Cashflows.FloatingLegBase {
        public CmsLeg(Schedule schedule, SwapIndex swapIndex) {
            schedule_ = schedule;
            index_ = swapIndex;
            paymentAdjustment_ = BusinessDayConvention.Following;
            inArrears_ = false;
            zeroPayments_ = false;
        }

        public override List<CashFlow> value() {
            return CashFlowVectors.FloatingLeg<SwapIndex, CmsCoupon, CappedFlooredCmsCoupon>(
                notionals_, schedule_, index_ as SwapIndex, paymentDayCounter_, paymentAdjustment_, fixingDays_, gearings_, spreads_, caps_, floors_, inArrears_, zeroPayments_);
        }
    }
}
