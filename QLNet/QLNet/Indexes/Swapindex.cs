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
    public class SwapIndex : InterestRateIndex {
        protected IborIndex iborIndex_;
        public IborIndex iborIndex() { return iborIndex_; }

        protected Period fixedLegTenor_;
        public Period fixedLegTenor() { return fixedLegTenor_; }
        
        BusinessDayConvention fixedLegConvention_;
        public BusinessDayConvention fixedLegConvention() { return fixedLegConvention_; }


        public SwapIndex(string familyName, Period tenor, int settlementDays, Currency currency,
                          Calendar calendar, Period fixedLegTenor, BusinessDayConvention fixedLegConvention,
                          DayCounter fixedLegDayCounter, IborIndex iborIndex) :
            base(familyName, tenor, settlementDays, currency, calendar, fixedLegDayCounter) {
            tenor_ = tenor;
            iborIndex_ = iborIndex;
            fixedLegTenor_ = fixedLegTenor;
            fixedLegConvention_ = fixedLegConvention;

            iborIndex_.registerWith(update);
        }

        
        /////////////////////////////////////
        //! InterestRateIndex interface
        public override Handle<YieldTermStructure> termStructure() { return iborIndex_.termStructure(); }
        public override Date maturityDate(Date valueDate) {
            return fixingCalendar().advance(valueDate, tenor_, BusinessDayConvention.Unadjusted, false);
        }

        protected override double forecastFixing(Date fixingDate) {
            return underlyingSwap(fixingDate).fairRate();
        }

        // \warning Relinking the term structure underlying the index will not have effect on the returned swap.
        // recheck
        public VanillaSwap underlyingSwap(Date fixingDate) {
            double fixedRate = 0.0;
            return new MakeVanillaSwap(tenor_, iborIndex_, fixedRate)
                    .withEffectiveDate(valueDate(fixingDate))
                    .withFixedLegCalendar(fixingCalendar())
                    .withFixedLegDayCount(dayCounter_)
                    .withFixedLegTenor(fixedLegTenor_)
                    .withFixedLegConvention(fixedLegConvention_)
                    .withFixedLegTerminationDateConvention(fixedLegConvention_)
                    .value();
        }
    }
}
