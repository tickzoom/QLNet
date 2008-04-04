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
    public static class CashFlowVectors {

        public static List<CashFlow> FloatingLeg<FloatingCouponType>(List<double> nominals,
                                                Schedule schedule,
                                                InterestRateIndex index,
                                                DayCounter paymentDayCounter,
                                                BusinessDayConvention paymentAdj,
                                                List<int> fixingDays,
                                                List<double> gearings,
                                                List<double> spreads,
                                                List<double> caps,
                                                List<double> floors,
                                                bool isInArrears,
                                                bool isZero)
                where FloatingCouponType : FloatingRateCoupon, new() {

            int n = schedule.Count;
            if (nominals.Count == 0) throw new ArgumentException("no nominal given");
            if (nominals.Count > n) throw new ArgumentException(
                       "too many nominals (" + nominals.Count + "), only " + n + " required");
            if (gearings != null && gearings.Count > n) throw new ArgumentException(
                       "too many gearings (" + gearings.Count + "), only " + n + " required");
            if (spreads != null && spreads.Count > n) throw new ArgumentException(
                       "too many spreads (" + spreads.Count + "), only " + n + " required");
            if (caps != null && caps.Count > n) throw new ArgumentException(
                       "too many caps (" + caps.Count + "), only " + n + " required");
            if (floors != null && floors.Count > n) throw new ArgumentException(
                       "too many floors (" + floors.Count + "), only " + n + " required");
            if (isZero && isInArrears) throw new ArgumentException("in-arrears and zero features are not compatible");

            List<CashFlow> leg = new List<CashFlow>();

            // the following is not always correct
            Calendar calendar = schedule.calendar();

            Date refStart, start, refEnd, end;
            Date lastPaymentDate = calendar.adjust(schedule[n-1], paymentAdj);

            for (int i=0; i<n-1; ++i) {
                refStart = start = schedule[i];
                refEnd   =   end = schedule[i+1];
                Date paymentDate = isZero ? lastPaymentDate : calendar.adjust(end, paymentAdj);
                if (i==0   && !schedule.isRegular(i+1))
                    refStart = calendar.adjust(end - schedule.tenor(), schedule.businessDayConvention());
                if (i==n-1 && !schedule.isRegular(i+1))
                    refEnd = calendar.adjust(start + schedule.tenor(), schedule.businessDayConvention());

                if (Utils.Get(gearings, i, 1) == 0) {                               // fixed coupon
                    leg.Add(new FixedRateCoupon(Utils.Get(nominals, i),
                                                paymentDate,
                                                Utils.effectiveFixedRate(spreads,caps, floors,i),
                                                paymentDayCounter,
                                                start, end, refStart, refEnd));
                } else {                                                            // floating coupon
                    if (Utils.noOption(caps, floors, i)) {
                        leg.Add(new FloatingCouponType().factory(paymentDate,
                                                Utils.Get(nominals, i),
                                                start, end,
                                                Utils.Get(fixingDays, i, 2),
                                                index,
                                                Utils.Get(gearings, i, 1),
                                                Utils.Get(spreads, i),
                                                refStart, refEnd, paymentDayCounter,
                                                isInArrears));
                    } else {
                        throw new NotImplementedException();
                    }
                }
            }
            return leg;
        }
    }
}
