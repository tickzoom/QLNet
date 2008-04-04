/*
 Copyright (C) 2008 Siarhei Novik (snovik@gmail.com)
  
 This file is part of QLNet Project http://trac2.assembla.com/QLNet

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
﻿using System;
using System.Collections.Generic;
using System.Text;
using QLNet;

namespace QLNet
{
    //! %coupon accruing over a fixed period
    //! This class implements part of the CashFlow interface but it is
    //  still abstract and provides derived classes with methods for accrual period calculations.
    public abstract class Coupon : CashFlow
    {
        protected double nominal_;
        protected Date paymentDate_, accrualStartDate_, accrualEndDate_, refPeriodStart_, refPeriodEnd_;

        // access to properties
        public double nominal() { return nominal_; }
        public override Date date() { return paymentDate_; }
        public Date accrualStartDate() { return accrualStartDate_; }
        public Date accrualEndDate { get { return accrualEndDate_; } }
        public Date refPeriodStart { get { return refPeriodStart_; } }
        public Date refPeriodEnd { get { return refPeriodEnd_; } }

        // virtual get methods to be defined in derived classes
        public abstract double rate();                   //! accrued rate
        public abstract DayCounter dayCounter();         //! day counter for accrual calculation
        public abstract double accruedAmount(Date d);         //! accrued amount at the given date
        public abstract FloatingRateCouponPricer pricer();


        // Constructors
        public Coupon() { }       // default constructor
        // coupon does not adjust the payment date which must already be a business day
        public Coupon(double nominal, Date paymentDate, Date accrualStartDate, Date accrualEndDate) :
            this(nominal, paymentDate, accrualStartDate, accrualEndDate, null, null) { }
        public Coupon(double nominal, Date paymentDate, Date accrualStartDate, Date accrualEndDate, Date refPeriodStart) :
            this(nominal, paymentDate, accrualStartDate, accrualEndDate, refPeriodStart, null) { }
        public Coupon(double nominal, Date paymentDate, Date accrualStartDate, Date accrualEndDate, Date refPeriodStart, Date refPeriodEnd)
        {
            nominal_ = nominal;
            paymentDate_ = paymentDate;
            accrualStartDate_ = accrualStartDate;
            accrualEndDate_ = accrualEndDate;
            refPeriodStart_ = refPeriodStart;
            refPeriodEnd_ = refPeriodEnd;

            if (refPeriodStart_ == null) refPeriodStart_ = accrualStartDate_;
            if (refPeriodEnd_ == null) refPeriodEnd_ = accrualEndDate_;
        }

        public double accrualPeriod()
        {                          //! accrual period as fraction of year
            return dayCounter().yearFraction(accrualStartDate_, accrualEndDate_, refPeriodStart_, refPeriodEnd_);
        }
        public int accrualDays
        {                                //! accrual period in days
            get { return dayCounter().dayCount(accrualStartDate_, accrualEndDate_); }
        }

        // recheck
        //void Coupon::accept(AcyclicVisitor& v) {
        //    Visitor<Coupon>* v1 = dynamic_cast<Visitor<Coupon>*>(&v);
        //    if (v1 != 0)
        //        v1->visit(*this);
        //    else
        //        CashFlow::accept(v);
        //}
    }
}
