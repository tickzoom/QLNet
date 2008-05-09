/*
 Copyright (C) 2008 Siarhei Novik (snovik@gmail.com)
  
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
    //! Abstract base class for dividend engines
    /*! \todo The dividend class really needs to be made more
              sophisticated to distinguish between fixed dividends and
              fractional dividends
    */
    public abstract class FDDividendEngineBase : FDMultiPeriodEngine {
        
        //public FDDividendEngineBase(GeneralizedBlackScholesProcess process,
        //    Size timeSteps = 100, Size gridPoints = 100, bool timeDependent = false)
        public FDDividendEngineBase(GeneralizedBlackScholesProcess process, int timeSteps, int gridPoints, bool timeDependent)
            : base(process, timeSteps, gridPoints, timeDependent) {}

        protected override void setupArguments(IPricingEngineArguments a) {
            DividendVanillaOption.Arguments args = a as DividendVanillaOption.Arguments;
            if (args == null) throw new ApplicationException("incorrect argument type");
            List<Event> events = new List<Event>();
            foreach (Event e in args.cashFlow)
                events_.Add(e);
            base.setupArguments(a, events);
        }

        protected double getDividendAmount(int i) {
            Dividend dividend = events_[i] as Dividend;
            if (dividend != null) {
                return dividend.amount();
            } else {
                return 0.0;
            }
        }

        protected double getDiscountedDividend(int i) {
            double dividend = getDividendAmount(i);
            double discount = process_.riskFreeRate().link.discount(events_[i].date()) /
                              process_.dividendYield().link.discount(events_[i].date());
            return dividend * discount;
        }
    }
}
