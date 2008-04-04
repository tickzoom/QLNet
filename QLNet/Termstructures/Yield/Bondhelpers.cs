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
using System.Linq;
using System.Text;

namespace QLNet {
    //! fixed-coupon bond helper
    /*! \warning This class assumes that the reference date
                 does not change between calls of setTermStructure().
    */
    public class FixedRateBondHelper : BootstrapHelper<YieldTermStructure> {
        protected FixedRateBond bond_;
        public FixedRateBond bond() { return bond_; }

        // need to init this because it is used before the handle has any link, i.e. setTermStructure will be used after ctor
        RelinkableHandle<YieldTermStructure> termStructureHandle_ = new RelinkableHandle<YieldTermStructure>();

        //public FixedRateBondHelper(Quote cleanPrice, int settlementDays, double faceAmount, Schedule schedule,
        //                   List<double> coupons, DayCounter dayCounter,
        //                   BusinessDayConvention paymentConv = Following,
        //                   double redemption = 100.0,
        //                   Date issueDate = null);
        public FixedRateBondHelper(Handle<Quote> cleanPrice, int settlementDays, double faceAmount, Schedule schedule,
                                   List<double> coupons, DayCounter dayCounter, BusinessDayConvention paymentConvention,
                                   double redemption, Date issueDate)
                : base(cleanPrice) {
            bond_ = new FixedRateBond(settlementDays, faceAmount, schedule, coupons, dayCounter, paymentConvention,
                                      redemption, issueDate);

            latestDate_ = bond_.maturityDate();

            Settings.registerWith(update);

            PricingEngine bondEngine = new DiscountingBondEngine(termStructureHandle_);
            bond_.setPricingEngine(bondEngine);
        }

        public FixedRateBondHelper(Handle<Quote> cleanPrice, FixedRateBond bond)
                : base(cleanPrice) {
            latestDate_ = bond_.maturityDate();

            Settings.registerWith(update);

            PricingEngine bondEngine = new DiscountingBondEngine(termStructureHandle_);
            bond_.setPricingEngine(bondEngine);
        }

        //! \name BootstrapHelper interface
        //@{
        public override double impliedQuote() {
            if (termStructure_ == null)
                throw new ApplicationException("term structure not set");

            // we didn't register as observers - force calculation
            bond_.recalculate();
            return bond_.cleanPrice();
        }

        public override void setTermStructure(YieldTermStructure ts) {
            //recheck
            throw new NotImplementedException();
            termStructureHandle_.linkTo(ts);
            base.setTermStructure(ts);
        }
    }
}
