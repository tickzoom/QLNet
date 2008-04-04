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
    //! base class for Inter-Bank-Offered-Rate indexes (e.g. %Libor, etc.)
    public class IborIndex : InterestRateIndex {
        protected BusinessDayConvention convention_;
        public BusinessDayConvention businessDayConvention() { return convention_; }

        protected Handle<YieldTermStructure> termStructure_;
        // InterestRateIndex interface
        public override Handle<YieldTermStructure> termStructure() { return termStructure_; }

        bool endOfMonth_;
        public bool endOfMonth() { return endOfMonth_; }

        public IborIndex(string familyName, Period tenor, int settlementDays, Currency currency,
                         Calendar fixingCalendar, BusinessDayConvention convention, bool endOfMonth,
                         DayCounter dayCounter, Handle<YieldTermStructure> h) : 
            base(familyName, tenor, settlementDays, currency, fixingCalendar, dayCounter) {
            convention_ = convention;
            termStructure_ = h;
            endOfMonth_ = endOfMonth;

            // observer interface
            if (!termStructure_.empty())
                termStructure_.registerWith(update);
        }

        //! Date calculations
        public override Date maturityDate(Date valueDate) {
            return fixingCalendar().advance(valueDate, tenor_, convention_, endOfMonth_);
        }

        protected override double forecastFixing(Date fixingDate) {
            if (termStructure_.empty())
                throw new ArgumentException("no forecasting term structure set to " + name());

            Date fixingValueDate = valueDate(fixingDate);
            Date endValueDate = maturityDate(fixingValueDate);
            double fixingDiscount = termStructure().link.discount(fixingValueDate);
            double endDiscount = termStructure().link.discount(endValueDate);
            double fixingPeriod = dayCounter().yearFraction(fixingValueDate, endValueDate);
            return (fixingDiscount / endDiscount - 1.0) / fixingPeriod;
        }
    }
}
