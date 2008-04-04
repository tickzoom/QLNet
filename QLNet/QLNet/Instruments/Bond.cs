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
    public partial class Utils {
        public static double dirtyPriceFromYield(double faceAmount, List<CashFlow> cashflows, double yield, DayCounter dayCounter,
                             Compounding compounding, Frequency frequency, Date settlement) {

            if (frequency == Frequency.NoFrequency || frequency == Frequency.Once)
                frequency = Frequency.Annual;

            InterestRate y = new InterestRate(yield, dayCounter, compounding, frequency);

            double price = 0.0;
            double discount = 1.0;
            Date lastDate = null;

            for (int i = 0; i < cashflows.Count - 1; ++i) {
                if (cashflows[i].hasOccurred(settlement))
                    continue;

                Date couponDate = cashflows[i].date();
                double amount = cashflows[i].amount();
                if (lastDate == null) {
                    // first not-expired coupon
                    if (i > 0) {
                        lastDate = cashflows[i - 1].date();
                    } else {
                        if (cashflows[i].GetType().IsSubclassOf(typeof(Coupon)))
                            lastDate = ((Coupon)cashflows[i]).accrualStartDate();
                        else
                            lastDate = couponDate - new Period(1, TimeUnit.Years);
                    }
                    discount *= y.discountFactor(settlement, couponDate, lastDate, couponDate);
                } else {
                    discount *= y.discountFactor(lastDate, couponDate);
                }
                lastDate = couponDate;

                price += amount * discount;
            }

            CashFlow redemption = cashflows.Last();
            if (!redemption.hasOccurred(settlement)) {
                Date redemptionDate = redemption.date();
                double amount = redemption.amount();
                if (lastDate == null) {
                    // no coupons
                    lastDate = redemptionDate - new Period(1, TimeUnit.Years);
                    discount *= y.discountFactor(settlement, redemptionDate, lastDate, redemptionDate);
                } else {
                    discount *= y.discountFactor(lastDate, redemptionDate);
                }

                price += amount * discount;
            }

            return price / faceAmount * 100.0;
        }
        
        public static double dirtyPriceFromZSpreadFunction(double faceAmount, List<CashFlow> cashflows, double zSpread,
                                                           DayCounter dc, Compounding comp, Frequency freq, Date settlement,
                                                           Handle<YieldTermStructure> discountCurve) {

            if (!(freq != Frequency.NoFrequency && freq != Frequency.Once))
                throw new ApplicationException("invalid frequency:" + freq);

            Quote zSpreadQuoteHandle = new SimpleQuote(zSpread);

            var spreadedCurve = new ZeroSpreadedTermStructure(discountCurve, zSpreadQuoteHandle, comp, freq, dc);
            
            double price = 0.0;
            foreach (CashFlow cf in cashflows.FindAll(x => !x.hasOccurred(settlement))) {
                Date couponDate = cf.date();
                double amount = cf.amount();
                price += amount * spreadedCurve.discount(couponDate);
            }
            price /= spreadedCurve.discount(settlement);
            return price/faceAmount*100.0;
        }
    }


    public class YieldFinder : ISolver1d {
        private double faceAmount_;
        private List<CashFlow> cashflows_;
        private double dirtyPrice_;
        private Compounding compounding_;
        private DayCounter dayCounter_;
        private Frequency frequency_;
        private Date settlement_;

        public YieldFinder(double faceAmount, List<CashFlow> cashflows, double dirtyPrice, DayCounter dayCounter,
                           Compounding compounding, Frequency frequency, Date settlement) {
            faceAmount_ = faceAmount;
            cashflows_ = cashflows;
            dirtyPrice_ = dirtyPrice;
            compounding_ = compounding;
            dayCounter_ = dayCounter;
            frequency_ = frequency;
            settlement_ = settlement;
        }

        public override double value(double yield) {
            return dirtyPrice_
                 - Utils.dirtyPriceFromYield(faceAmount_, cashflows_, yield, dayCounter_, compounding_, frequency_, settlement_);
        }
    }


    //! Base bond class
    /*! Derived classes must fill the unitialized data members.

        \warning Most methods assume that the cashflows are stored sorted by date, the redemption being the last one.

        \test
        - price/yield calculations are cross-checked for consistency.
        - price/yield calculations are checked against known good values. */
    public class Bond : Instrument {
        #region properties
        protected int settlementDays_;
        public int settlementDays() { return settlementDays_; }

        protected Calendar calendar_;
        public Calendar calendar() { return calendar_; }

        protected double faceAmount_;
        public double faceAmount() { return faceAmount_; }

        protected List<CashFlow> cashflows_;
        public List<CashFlow> cashflows() { return cashflows_; }

        protected Date maturityDate_, issueDate_;
        public Date maturityDate() {
            if (maturityDate_ != null)
                return maturityDate_;
            else
                return cashflows_.Last().date();
        }
        public Date issueDate() { return issueDate_; }

        public CashFlow redemption() { return cashflows_.Last(); } 
        #endregion

        public Bond(int settlementDays, Calendar calendar, double faceAmount, Date maturityDate, Date issueDate) 
            : this(settlementDays, calendar, faceAmount, maturityDate, issueDate, new List<CashFlow>()) { }
        public Bond(int settlementDays, Calendar calendar, double faceAmount, Date maturityDate, Date issueDate,
                    List<CashFlow> cashflows) {
            settlementDays_ = settlementDays;
            calendar_ = calendar;
            faceAmount_ = faceAmount;
            cashflows_ = cashflows;
            maturityDate_ = maturityDate;
            issueDate_ = issueDate;

            Settings.registerWith(update);
        }

        public Date settlementDate() { return settlementDate(null); }
        public Date settlementDate(Date date) {
            Date d = (date==null ? Settings.evaluationDate() : date);

            // usually, the settlement is at T+n...
            Date settlement = calendar_.advance(d, settlementDays_, TimeUnit.Days);
            // ...but the bond won't be traded until the issue date (if given.)
            if (issueDate_ == null)
                return settlement;
            else
                return Date.Max(settlement, issueDate_);
        }

        //@}
        //! \name Calculations
        //@{
        //! theoretical clean price
        /*! The default bond settlement is used for calculation.

            \warning the theoretical price calculated from a flat term structure might differ slightly from the price
                     calculated from the corresponding yield by means of the other overload of this function. If the
                     price from a constant yield is desired, it is advisable to use such other overload. */
        public double cleanPrice() { return dirtyPrice() - accruedAmount(settlementDate()); }

        //! clean price given a yield and settlement date
        /*! The default bond settlement is used if no date is given. */
        public double cleanPrice(double yield, DayCounter dc, Compounding comp, Frequency freq) {
            return cleanPrice(yield, dc, comp, freq, null); }
        public double cleanPrice(double yield, DayCounter dc, Compounding comp, Frequency freq, Date settlement) {
            if (settlement == null)
                settlement = settlementDate();
            return dirtyPrice(yield, dc, comp, freq, settlement) - accruedAmount(settlement);
        }

        //! theoretical dirty price
        /*! The default bond settlement is used for calculation.

            \warning the theoretical price calculated from a flat term structure might differ slightly from the price
                     calculated from the corresponding yield by means of the other overload of this function. If the
                     price from a constant yield is desired, it is advisable to use such other overload.
        */
        public double dirtyPrice() { return NPV() / faceAmount_ * 100.0; }

        //! dirty price given a yield and settlement date
        /*! The default bond settlement is used if no date is given. */
        public double dirtyPrice(double yield, DayCounter dc, Compounding comp, Frequency freq, Date settlement) {
            if (settlement == null)
                settlement = settlementDate();
            return Utils.dirtyPriceFromYield(faceAmount_, cashflows_, yield, dc, comp, freq, settlement);
        }


        //! theoretical bond yield
        /*! The default bond settlement and theoretical price are used for calculation. */
//        public double yield(DayCounter dc, Compounding comp, Frequency freq, double accuracy = 1.0e-8, int maxEvaluations = 100) {
        public double yield(DayCounter dc, Compounding comp, Frequency freq) {
            return yield(dc, comp, freq, 1.0e-8);
        }
        public double yield(DayCounter dc, Compounding comp, Frequency freq, double accuracy) {
            return yield(dc, comp, freq, accuracy, 100);
        }
        public double yield(DayCounter dc, Compounding comp, Frequency freq, double accuracy, int maxEvaluations) {
            Brent solver = new Brent();
            solver.setMaxEvaluations(maxEvaluations);
            YieldFinder objective = new YieldFinder(faceAmount_, cashflows_, dirtyPrice(), dc, comp, freq, settlementDate());
            return solver.solve(objective, accuracy, 0.02, 0.0, 1.0);
        }

        public double yield(double cleanPrice, DayCounter dc, Compounding comp, Frequency freq) {
            return yield(cleanPrice, dc, comp, freq, null, 1.0e-8, 100);
        }
        public double yield(double cleanPrice, DayCounter dc, Compounding comp, Frequency freq, Date settlement,
                            double accuracy, int maxEvaluations) {
            if (settlement == null)
                settlement = settlementDate();
            Brent solver = new Brent();
            solver.setMaxEvaluations(maxEvaluations);
            double dirtyPrice = cleanPrice + accruedAmount(settlement);
            YieldFinder objective = new YieldFinder(faceAmount_, cashflows_, dirtyPrice, dc, comp, freq, settlement);
            return solver.solve(objective, accuracy, 0.02, 0.0, 1.0);
        }

        //! clean price given Z-spread
        /*! Z-spread compounding, frequency, daycount are taken into account
            The default bond settlement is used if no date is given.
            For details on Z-spread refer to:
            "Credit Spreads Explained", Lehman Brothers European Fixed Income Research - March 2004, D. O'Kane*/
        public double cleanPriceFromZSpread(double zSpread, DayCounter dc, Compounding comp, Frequency freq, Date settlement) {
            double p = dirtyPriceFromZSpread(zSpread, dc, comp, freq, settlement);
            return p - accruedAmount(settlement);
        }

        //! dirty price given Z-spread
        /*! Z-spread compounding, frequency, daycount are taken into account
            The default bond settlement is used if no date is given.
            For details on Z-spread refer to:
            "Credit Spreads Explained", Lehman Brothers European Fixed Income Research - March 2004, D. O'Kane*/
        public double dirtyPriceFromZSpread(double zSpread, DayCounter dc, Compounding comp, Frequency freq, Date settlement) {
            if (settlement == null)
                 settlement = settlementDate();

            if (engine_ == null)
                throw new ApplicationException("null pricing engine");

            if (!engine_.GetType().IsSubclassOf(typeof(DiscountingBondEngine)))
                throw new ApplicationException("engine not compatible with calculation");

             return Utils.dirtyPriceFromZSpreadFunction(faceAmount_, cashflows_, zSpread, dc, comp, freq,
                                                  settlement, ((DiscountingBondEngine)engine_).discountCurve());
        }

        //! accrued amount at a given date
        /*! The default bond settlement is used if no date is given. */
        public double accruedAmount(Date settlement) {
            if (settlement==null)
                settlement = settlementDate();

            CashFlow cf = CashFlows.nextCashFlow(cashflows_, settlement);
            if (cf==cashflows_.Last()) return 0.0;

            Date paymentDate = cf.date();
            bool firstCouponFound = false;
            double nominal = 0;
            double accrualPeriod = 0;
            DayCounter dc = null;
            double result = 0.0;
            foreach(CashFlow x in cashflows_.FindAll(x => x.date()==paymentDate && x.GetType().IsSubclassOf(typeof(Coupon)))) {
                Coupon cp = (Coupon)x;
                if (firstCouponFound) {
                    if (!(nominal == cp.nominal() && accrualPeriod == cp.accrualPeriod() && dc == cp.dayCounter()))
                        throw new ApplicationException("cannot aggregate accrued amount of two different coupons on " + paymentDate);
                } else {
                    firstCouponFound = true;
                    nominal = cp.nominal();
                    accrualPeriod = cp.accrualPeriod();
                    dc = cp.dayCounter();
                }
                result += cp.accruedAmount(settlement);
            }
            // accruedAmount cannot throw, must return zero
            // for bond algebra to work
            //QL_ENSURE(firstCouponFound,
            //          "next cashflow (" << paymentDate << ") is not a coupon");
            return result/faceAmount_*100.0;
        }

        public override bool isExpired() {
            return cashflows_.Last().hasOccurred(settlementDate());
        }

        /*! Expected next coupon: depending on (the bond and) the given date the coupon can be historic, deterministic
            or expected in a stochastic sense. When the bond settlement date is used the coupon
            is the already-fixed not-yet-paid one.

            The current bond settlement is used if no date is given. */
        public double nextCoupon(Date settlement) {
            if (settlement == null)
                settlement = settlementDate();
            return CashFlows.nextCouponRate(cashflows_, settlement);
        }

        //! Previous coupon already paid at a given date
        /*! Expected previous coupon: depending on (the bond and) the given date the coupon can be historic, deterministic
            or expected in a stochastic sense. When the bond settlement date is used the coupon is the last paid one.

            The current bond settlement is used if no date is given. */
        public double previousCoupon(Date settlement) {
            if (settlement == null)
                settlement = settlementDate();
            return CashFlows.previousCouponRate(cashflows_, settlement);
        }

        public override void setupArguments(PricingEngine.Arguments args) {
            if (!(args is PricingEngine.Arguments))
                throw new ApplicationException("wrong argument type");

            Bond.Arguments arguments = (Bond.Arguments)args;
            arguments.settlementDate = settlementDate();
            arguments.cashflows = cashflows_;
            arguments.calendar = calendar_;
        }


        public class Engine : GenericEngine<Arguments, Results> { }

        public new class Results : Instrument.Results { }

        public class Arguments : PricingEngine.Arguments {
            public Date settlementDate;
            public List<CashFlow> cashflows;
            public Calendar calendar;

            public override void Validate() {
                if (settlementDate == null)
                    throw new ApplicationException("no settlement date provided");
                foreach(CashFlow cf in cashflows)
                    if (cf == null)
                        throw new ApplicationException("null coupon provided");
            }
        };
    }
}
