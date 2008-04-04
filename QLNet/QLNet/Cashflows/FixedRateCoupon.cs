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
using QLNet;

namespace QLNet {
    public class FixedRateCoupon : Coupon {
        private InterestRate rate_;
        private DayCounter dayCounter_;

        // properties access
        public override double rate() { return rate_.rate(); }
        public InterestRate InterestRate { get { return rate_; } }
        public override DayCounter dayCounter() { return dayCounter_; }
        public override FloatingRateCouponPricer pricer() { return null; }

        // constructors
        public FixedRateCoupon(double nominal, Date paymentDate, double rate, DayCounter dayCounter,
                                Date accrualStartDate, Date accrualEndDate) :
            this(nominal, paymentDate, rate, dayCounter, accrualStartDate, accrualEndDate, null, null) { }
        public FixedRateCoupon(double nominal, Date paymentDate, double rate, DayCounter dayCounter,
                               Date accrualStartDate, Date accrualEndDate, Date refPeriodStart, Date refPeriodEnd) :
            base(nominal, paymentDate, accrualStartDate, accrualEndDate, refPeriodStart, refPeriodEnd) {
            rate_ = new InterestRate(rate, dayCounter, Compounding.Simple);
            dayCounter_ = dayCounter; 
        }

        public FixedRateCoupon(double nominal, Date paymentDate, InterestRate interestRate, DayCounter dayCounter, Date accrualStartDate, Date accrualEndDate) :
            this(nominal, paymentDate, interestRate, dayCounter, accrualStartDate, accrualEndDate, null, null) { }
        public FixedRateCoupon(double nominal, Date paymentDate, InterestRate interestRate, DayCounter dayCounter,
                               Date accrualStartDate, Date accrualEndDate, Date refPeriodStart, Date refPeriodEnd) :
            base(nominal, paymentDate, accrualStartDate, accrualEndDate, refPeriodStart, refPeriodEnd) {
            rate_ = interestRate;
            dayCounter_ = dayCounter; 
        }

        //! CashFlow interface
        public override double amount() {
            return nominal()*(rate_.compoundFactor(accrualStartDate_,
                                                       accrualEndDate_, refPeriodStart_, refPeriodEnd_) - 1.0); }

        //! Coupon interface
        public override double accruedAmount(Date d) {
            if (d <= accrualStartDate_ || d > paymentDate_)
                return 0;
            else
                return nominal() * (rate_.compoundFactor(accrualStartDate_, Date.Min(d, accrualEndDate_),
                                                     refPeriodStart_, refPeriodEnd_) - 1.0);
        }
    }

    //! helper class building a sequence of fixed rate coupons
    public class FixedRateLeg {
        // properties
        private Schedule schedule_;
        private List<double> notionals_ = new List<double>();
        private List<InterestRate> couponRates_ = new List<InterestRate>();
        private DayCounter paymentDayCounter_, firstPeriodDayCounter_ = null;
        private BusinessDayConvention paymentAdjustment_;

        // constructor
        public FixedRateLeg(Schedule schedule, DayCounter paymentDayCounter) {
            schedule_ = schedule;
            paymentDayCounter_ = paymentDayCounter;
            paymentAdjustment_ = BusinessDayConvention.Following;
        }

        // other initializers
        public FixedRateLeg withNotionals(double notional) {
            notionals_.Clear();
            notionals_.Add(notional);
            return this;
        }
        public FixedRateLeg withNotionals(List<double> notionals) {
            notionals_ = notionals;
            return this;
        }

        public FixedRateLeg withCouponRates(double couponRate) {
            couponRates_.Clear();
            couponRates_.Add(new InterestRate(couponRate, paymentDayCounter_, Compounding.Simple));
            return this;
        }
        public FixedRateLeg withCouponRates(InterestRate couponRate) {
            couponRates_.Clear();
            couponRates_.Add(couponRate);
            return this;
        }
        public FixedRateLeg withCouponRates(List<double> couponRates) {
            couponRates_.Clear();
            foreach (double r in couponRates)
                couponRates_.Add(new InterestRate(r, paymentDayCounter_, Compounding.Simple));
            return this;
        }
        public FixedRateLeg withCouponRates(List<InterestRate>couponRates) {
            couponRates_ = couponRates;
            return this;
        }

        public FixedRateLeg withPaymentAdjustment(BusinessDayConvention c) {
            paymentAdjustment_ = c;
            return this; 
        }
        public FixedRateLeg withFirstPeriodDayCounter(DayCounter dayCounter) {
            firstPeriodDayCounter_ = dayCounter;
            return this;
        }

        public List<CashFlow> value() {
            if (couponRates_.Count == 0) throw new ArgumentException("coupon rates not specified");
            if (notionals_.Count == 0) throw new ArgumentException("nominals not specified");

            List<CashFlow> leg = new List<CashFlow>();

            // the following is not always correct
            Calendar calendar = schedule_.calendar();

            // first period might be short or long
            Date start = schedule_[0], end = schedule_[1];
            Date paymentDate = calendar.adjust(end, paymentAdjustment_);
            InterestRate rate = couponRates_[0];
            double nominal = notionals_[0];
            if (schedule_.isRegular(1)) {
                if (!(firstPeriodDayCounter_ == null || firstPeriodDayCounter_ == paymentDayCounter_))
                    throw new ArgumentException("regular first coupon does not allow a first-period day count");
                leg.Add(new FixedRateCoupon(nominal, paymentDate, rate, paymentDayCounter_, start, end, start, end));
            } else {
                Date refer = end - schedule_.tenor();
                refer = calendar.adjust(refer, schedule_.businessDayConvention());
                DayCounter dc = firstPeriodDayCounter_ == null ? paymentDayCounter_ : firstPeriodDayCounter_;
                leg.Add(new FixedRateCoupon(nominal, paymentDate, rate, dc, start, end, refer, end));
            }

            // regular periods
            for (int i=2; i<schedule_.Count-1; ++i) {
				start = end; end = schedule_[i];
                paymentDate = calendar.adjust(end, paymentAdjustment_);
                if ((i - 1) < couponRates_.Count) rate = couponRates_[i - 1];
                else                              rate = couponRates_.Last();
                if ((i - 1) < notionals_.Count)   nominal = notionals_[i - 1];
                else                              nominal = notionals_.Last();

                leg.Add(new FixedRateCoupon(nominal, paymentDate, rate, paymentDayCounter_, start, end, start, end));
            }

            if (schedule_.Count > 2) {
                // last period might be short or long
                int N = schedule_.Count;
                start = end; end = schedule_[N-1];
                paymentDate = calendar.adjust(end, paymentAdjustment_);

                if ((N - 2) < couponRates_.Count) rate = couponRates_[N - 2];
                else                              rate = couponRates_.Last();
                if ((N - 2) < notionals_.Count)   nominal = notionals_[N - 2];
                else                              nominal = notionals_.Last();

                if (schedule_.isRegular(N-1))
                    leg.Add(new FixedRateCoupon(nominal, paymentDate, rate, paymentDayCounter_, start, end, start, end));
                else {
                    Date refer = start + schedule_.tenor();
                    refer = calendar.adjust(refer, schedule_.businessDayConvention());
                    leg.Add(new FixedRateCoupon(nominal, paymentDate, rate, paymentDayCounter_, start, end, start, refer));
                }
            }
            return leg;
        }
    }
}
