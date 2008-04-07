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
using System.Reflection;

namespace QLNet {

    // Coupon pricers
    //! generic pricer for floating-rate coupons
    public abstract class FloatingRateCouponPricer : IObservable, IObserver {
        public abstract void initialize(FloatingRateCoupon coupon);

        //! required interface
        public abstract double swapletPrice();
        public abstract double swapletRate();
        public abstract double capletPrice(double effectiveCap);
        public abstract double capletRate(double effectiveCap);
        public abstract double floorletPrice(double effectiveFloor);
        public abstract double floorletRate(double effectiveFloor);

        protected abstract double optionletPrice(Option.Type optionType, double effStrike);

        #region Observer & observable
        public event Callback notifyObserversEvent;
        public void registerWith(Callback handler) { notifyObserversEvent += handler; }
        public void unregisterWith(Callback handler) { notifyObserversEvent -= handler; }
        protected void notifyObservers() {
            Callback handler = notifyObserversEvent;
            if (handler != null) {
                handler();
            }
        }

        // observer interface
        public void update() { notifyObservers(); } 
        #endregion
    }

    //! base pricer for capped/floored Ibor coupons
    public abstract class IborCouponPricer : FloatingRateCouponPricer {
        private Handle<OptionletVolatilityStructure> capletVol_;
        public Handle<OptionletVolatilityStructure> capletVolatility() { return capletVol_; }

        // public IborCouponPricer(OptionletVolatilityStructure v = OptionletVolatilityStructure>())
        public IborCouponPricer(Handle<OptionletVolatilityStructure> v) {
            capletVol_ = v;

            if (!capletVol_.empty())
                capletVol_.registerWith(update);
        }

        // public void setCapletVolatility(OptionletVolatilityStructure v = OptionletVolatilityStructure>()) {
        public void setCapletVolatility(Handle<OptionletVolatilityStructure> v) {
            capletVol_.unregisterWith(update);
            capletVol_ = v;
            if (!capletVol_.empty())
                capletVol_.registerWith(update);

            update();
        }
    }


    //===========================================================================//
    //                              BlackIborCouponPricer                        //
    //===========================================================================//
    //! Black-formula pricer for capped/floored Ibor coupons
    public class BlackIborCouponPricer : IborCouponPricer {
        private IborCoupon coupon_;
        private double discount_;
        private double gearing_;
        private double spread_;
        private double spreadLegValue_;
        private double accrualPeriod_;

        public BlackIborCouponPricer() : this(new Handle<OptionletVolatilityStructure>()) { }
        public BlackIborCouponPricer(Handle<OptionletVolatilityStructure> v) : base(v) { }


        public override void initialize(FloatingRateCoupon coupon) {
            coupon_ = (IborCoupon)coupon;
            gearing_ = coupon_.gearing();
            spread_ = coupon_.spread();

            Date paymentDate = coupon_.date();
            InterestRateIndex index = coupon_.index();
            Handle<YieldTermStructure> rateCurve = index.termStructure();

            Date today = Settings.evaluationDate();

            if (paymentDate > today)
                discount_ = rateCurve.link.discount(paymentDate);
            else
                discount_ = 1.0;

            // to be done in the future
            // accrualPeriod_ = coupon_.accrualPeriod();

            spreadLegValue_ = spread_ * coupon_.accrualPeriod() * discount_;
        }

        public override double swapletPrice() {
            // past or future fixing is managed in InterestRateIndex.fixing()
            double swapletPrice = adjustedFixing() * coupon_.accrualPeriod() * discount_;
            double result = gearing_ * swapletPrice + spreadLegValue_;
            return result;
        }

        public override double swapletRate() {
            double result = swapletPrice() / (coupon_.accrualPeriod() * discount_);
            return result;
        }

        public override double capletPrice(double effectiveCap) {
            double capletPrice = optionletPrice(Option.Type.Call, effectiveCap);
            return gearing_ * capletPrice;
        }

        public override double capletRate(double effectiveCap) {
            return capletPrice(effectiveCap)/(coupon_.accrualPeriod()*discount_);
        }

        public override double floorletPrice(double effectiveFloor) {
            double floorletPrice = optionletPrice(Option.Type.Put, effectiveFloor);
            return gearing_ * floorletPrice;
        }

        public override double floorletRate(double effectiveFloor) {
            return floorletPrice(effectiveFloor) / (coupon_.accrualPeriod()*discount_);
        }

        protected override double optionletPrice(Option.Type optionType, double effStrike) {
            Date fixingDate = coupon_.fixingDate();
            if (fixingDate <= Settings.evaluationDate()) {
                // the amount is determined
                double a, b;
                if (optionType == Option.Type.Call) {
                    a = coupon_.indexFixing();
                    b = effStrike;
                } else {
                    a = effStrike;
                    b = coupon_.indexFixing();
                }
                return Math.Max(a - b, 0.0) * coupon_.accrualPeriod() * discount_;
            } else {
                throw new NotImplementedException();
                //QL_REQUIRE(!capletVolatility().empty(),
                //           "missing optionlet volatility");
                //// not yet determined, use Black model
                //double variance = Math.Sqrt(capletVolatility().blackVariance(fixingDate, effStrike));
                //double fixing = blackFormula(optionType, effStrike, adjustedFixing(), variance);
                //return fixing * coupon_.accrualPeriod() * discount_;
            }
        }

        private double adjustedFixing() {
            double adjustement = 0.0;

            double fixing = coupon_.indexFixing();

            if (!coupon_.isInArrears()) {
                adjustement = 0.0;
            } else {
                // see Hull, 4th ed., page 550
                if (capletVolatility().empty())
                    throw new ApplicationException("missing optionlet volatility");
                Date d1 = coupon_.fixingDate(),
                     referenceDate = capletVolatility().link.referenceDate();
                if (d1 <= referenceDate) {
                    adjustement = 0.0;
                } else {
                    Date d2 = coupon_.index().maturityDate(d1);
                    double tau = coupon_.index().dayCounter().yearFraction(d1, d2);
                    double variance = capletVolatility().link.blackVariance(d1, fixing);
                    adjustement = fixing * fixing * variance * tau / (1.0 + fixing * tau);
                }
            }
            return fixing + adjustement;
        }
    }


    public class PricerSetter : IAcyclicVisitor {
        private FloatingRateCouponPricer pricer_;

        public PricerSetter(FloatingRateCouponPricer pricer) {
            pricer_ = pricer;
        }

        public void visit(object o) {
            Type[] types = new Type[] { o.GetType() };
            MethodInfo methodInfo = this.GetType().GetMethod("visit", types);
            if (methodInfo != null) {
                methodInfo.Invoke(this, new object[] { o });
            }
        }

        public void visit(CashFlow c) {
            // nothing to do
        }
        public void visit(Coupon c) {
            // nothing to do
        }
        public void visit(IborCoupon c) {
            if (!(pricer_ is IborCouponPricer))
                throw new ApplicationException("pricer not compatible with Ibor coupon");
            c.setPricer(pricer_);
        }

        //public void visit(CappedFlooredIborCoupon c);
        //public void visit(DigitalIborCoupon c);
        //public void visit(CmsCoupon c);
        //public void visit(CappedFlooredCmsCoupon c)
        //public void visit(DigitalCmsCoupon c)
        //public void visit(RangeAccrualFloatersCoupon c)

        //void PricerSetter::visit(DigitalIborCoupon& c) {
        //    const boost::shared_ptr<IborCouponPricer> iborCouponPricer =
        //        boost::dynamic_pointer_cast<IborCouponPricer>(pricer_);
        //    QL_REQUIRE(iborCouponPricer,
        //               "pricer not compatible with Ibor coupon");
        //    c.setPricer(iborCouponPricer);
        //}

        //void PricerSetter::visit(CappedFlooredIborCoupon& c) {
        //    const boost::shared_ptr<IborCouponPricer> iborCouponPricer =
        //        boost::dynamic_pointer_cast<IborCouponPricer>(pricer_);
        //    QL_REQUIRE(iborCouponPricer,
        //               "pricer not compatible with Ibor coupon");
        //    c.setPricer(iborCouponPricer);
        //}

        //void PricerSetter::visit(CmsCoupon& c) {
        //    const boost::shared_ptr<CmsCouponPricer> cmsCouponPricer =
        //        boost::dynamic_pointer_cast<CmsCouponPricer>(pricer_);
        //    QL_REQUIRE(cmsCouponPricer,
        //               "pricer not compatible with CMS coupon");
        //    c.setPricer(cmsCouponPricer);
        //}

        //void PricerSetter::visit(CappedFlooredCmsCoupon& c) {
        //    const boost::shared_ptr<CmsCouponPricer> cmsCouponPricer =
        //        boost::dynamic_pointer_cast<CmsCouponPricer>(pricer_);
        //    QL_REQUIRE(cmsCouponPricer,
        //               "pricer not compatible with CMS coupon");
        //    c.setPricer(cmsCouponPricer);
        //}

        //void PricerSetter::visit(DigitalCmsCoupon& c) {
        //    const boost::shared_ptr<CmsCouponPricer> cmsCouponPricer =
        //        boost::dynamic_pointer_cast<CmsCouponPricer>(pricer_);
        //    QL_REQUIRE(cmsCouponPricer,
        //               "pricer not compatible with CMS coupon");
        //    c.setPricer(cmsCouponPricer);
        //}

        //void PricerSetter::visit(RangeAccrualFloatersCoupon& c) {
        //    const boost::shared_ptr<RangeAccrualPricer> rangeAccrualPricer =
        //        boost::dynamic_pointer_cast<RangeAccrualPricer>(pricer_);
        //    QL_REQUIRE(rangeAccrualPricer,
        //               "pricer not compatible with range-accrual coupon");
        //    c.setPricer(rangeAccrualPricer);
        //}
    }


    partial class Utils {
        public static void setCouponPricer(List<CashFlow> leg, FloatingRateCouponPricer pricer) {
            PricerSetter setter = new PricerSetter(pricer);
            foreach(CashFlow cf in leg) {
                cf.accept(setter);
            }
        }

        public static void setCouponPricers(List<CashFlow> leg, List<FloatingRateCouponPricer> pricers) {
            throw new NotImplementedException();
            //Size nCashFlows = leg.size();
            //QL_REQUIRE(nCashFlows>0, "no cashflows");

            //Size nPricers = pricers.size();
            //QL_REQUIRE(nCashFlows >= nPricers,
            //           "mismatch between leg size (" << nCashFlows <<
            //           ") and number of pricers (" << nPricers << ")");

            //for (Size i=0; i<nCashFlows; ++i) {
            //    PricerSetter setter(i<nPricers ? pricers[i] : pricers[nPricers-1]);
            //    leg[i]->accept(setter);
            //}
        }
    }
}
