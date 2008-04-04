using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QLNet;

namespace TestSuite {
    public class BondsTest {
        class CommonVars {
            // common data
            public Calendar calendar;
            public Date today;
            public double faceAmount;

            // todo
            // cleanup
            // SavedSettings backup;

            // setup
            public CommonVars() {
                calendar = new TARGET();
                today = calendar.adjust(Date.Today);
                Settings.setEvaluationDate(today);
                faceAmount = 1000000.0;
            }
        }

        void testYield() {

            Console.WriteLine("Testing consistency of bond price/yield calculation...");

            CommonVars vars = new CommonVars();

            double tolerance = 1.0e-7;
            int maxEvaluations = 100;

            int[] issueMonths = new int[] { -24, -18, -12, -6, 0, 6, 12, 18, 24 };
            int[] lengths = new int[] { 3, 5, 10, 15, 20 };
            int settlementDays = 3;
            double[] coupons = new double[] { 0.02, 0.05, 0.08 };
            Frequency[] frequencies = new Frequency[] { Frequency.Semiannual, Frequency.Annual };
            DayCounter bondDayCount = new Thirty360();
            BusinessDayConvention accrualConvention = BusinessDayConvention.Unadjusted;
            BusinessDayConvention paymentConvention = BusinessDayConvention.ModifiedFollowing;
            double redemption = 100.0;

            double[] yields = new double[] { 0.03, 0.04, 0.05, 0.06, 0.07 };
            Compounding[] compounding = new Compounding[] { Compounding.Compounded, Compounding.Continuous };

            for (int i=0; i<issueMonths.Length; i++) {
              for (int j=0; j<lengths.Length; j++) {
                for (int k=0; k<coupons.Length; k++) {
                  for (int l=0; l<frequencies.Length; l++) {
                    for (int n=0; n<compounding.Length; n++) {

                      Date dated = vars.calendar.advance(vars.today, issueMonths[i], TimeUnit.Months);
                      Date issue = dated;
                      Date maturity = vars.calendar.advance(issue, lengths[j], TimeUnit.Years);

                      Schedule sch = new Schedule(dated, maturity, new Period(frequencies[l]), vars.calendar,
                                                  accrualConvention, accrualConvention, DateGeneration.Rule.Backward, false);

                      FixedRateBond bond = new FixedRateBond(settlementDays, vars.faceAmount, sch,
                                                             new List<double>() { coupons[k] },
                                                             bondDayCount, paymentConvention,
                                                             redemption, issue);

                      for (int m=0; m<yields.Length; m++) {

                        double price = bond.cleanPrice(yields[m], bondDayCount, compounding[n], frequencies[l]);
                        double calculated = bond.yield(price, bondDayCount, compounding[n], frequencies[l], null,
                                                       tolerance, maxEvaluations);

                        if (Math.Abs(yields[m]-calculated) > tolerance) {
                          // the difference might not matter
                          double price2 = bond.cleanPrice(calculated, bondDayCount, compounding[n], frequencies[l]);
                          if (Math.Abs(price-price2)/price > tolerance) {
                              Console.WriteLine("yield recalculation failed:\n"
                                  + "    issue:     " + issue + "\n"
                                  + "    maturity:  " + maturity + "\n"
                                  + "    coupon:    " + coupons[k] + "\n"
                                  + "    frequency: " + frequencies[l] + "\n\n"
                                  + "    yield:  " + yields[m] + " "
                                  + (compounding[n] == Compounding.Compounded ? "compounded" : "continuous") + "\n"
                                  + "    price:  " + price + "\n"
                                  + "    yield': " + calculated + "\n"
                                  + "    price': " + price2);
                          }
                        }
                      }
                    }
                  }
                }
              }
            }

        }
        void testTheoretical() {
            Console.WriteLine("Testing theoretical bond price/yield calculation...");

            CommonVars vars = new CommonVars();

            double tolerance = 1.0e-7;
            int maxEvaluations = 100;

            int[] lengths = new int[] { 3, 5, 10, 15, 20 };
            int settlementDays = 3;
            double[] coupons = new double[] { 0.02, 0.05, 0.08 };
            Frequency[] frequencies = new Frequency[] { Frequency.Semiannual, Frequency.Annual };
            DayCounter bondDayCount = new Actual360();
            BusinessDayConvention accrualConvention = BusinessDayConvention.Unadjusted;
            BusinessDayConvention paymentConvention =  BusinessDayConvention.ModifiedFollowing;
            double redemption = 100.0;

            double[] yields = new double[] { 0.03, 0.04, 0.05, 0.06, 0.07 };

            for (int j=0; j<lengths.Length; j++) {
              for (int k=0; k<coupons.Length; k++) {
                for (int l=0; l<frequencies.Length; l++) {

                    Date dated = vars.today;
                    Date issue = dated;
                    Date maturity = vars.calendar.advance(issue, lengths[j], TimeUnit.Years);

                    SimpleQuote rate = new SimpleQuote(0.0);
                    var discountCurve = new Handle<YieldTermStructure>(Utilities.flatRate(vars.today, rate, bondDayCount));

                    Schedule sch = new Schedule(dated, maturity, new Period(frequencies[l]), vars.calendar,
                                                accrualConvention, accrualConvention, DateGeneration.Rule.Backward, false);

                    FixedRateBond bond = new FixedRateBond(settlementDays, vars.faceAmount, sch, new List<double>() { coupons[k] },
                                                           bondDayCount, paymentConvention, redemption, issue);

                    PricingEngine bondEngine = new DiscountingBondEngine(discountCurve);
                    bond.setPricingEngine(bondEngine);

                    for (int m=0; m<yields.Length; m++) {

                        rate.setValue(yields[m]);

                        double price = bond.cleanPrice(yields[m], bondDayCount, Compounding.Continuous, frequencies[l]);
                        double calculatedPrice = bond.cleanPrice();

                        if (Math.Abs(price-calculatedPrice) > tolerance) {
                            Console.WriteLine("price calculation failed:"
                                + "\n    issue:     " + issue
                                + "\n    maturity:  " + maturity
                                + "\n    coupon:    " + coupons[k]
                                + "\n    frequency: " + frequencies[l] + "\n"
                                + "\n    yield:     " + yields[m]
                                + "\n    expected:    " + price
                                + "\n    calculated': " + calculatedPrice
                                + "\n    error':      " + (price-calculatedPrice));
                        }

                        double calculatedYield = bond.yield(bondDayCount, Compounding.Continuous, frequencies[l],
                                                 tolerance, maxEvaluations);
                        if (Math.Abs(yields[m]-calculatedYield) > tolerance) {
                            Console.WriteLine("yield calculation failed:"
                                + "\n    issue:     " + issue
                                + "\n    maturity:  " + maturity
                                + "\n    coupon:    " + coupons[k]
                                + "\n    frequency: " + frequencies[l] + "\n"
                                + "\n    yield:  " + yields[m]
                                + "\n    price:  " + price
                                + "\n    yield': " + calculatedYield);
                        }
                    }
                }
              }
            }
        }
        void testCached() {
            Console.WriteLine("Testing bond price/yield calculation against cached values...");

            CommonVars vars = new CommonVars();

            // with implicit settlement calculation:
            Date today = new Date(2004, Month.November, 22);
            Settings.setEvaluationDate(today);

            Calendar bondCalendar = new NullCalendar();
            DayCounter bondDayCount = new ActualActual(ActualActual.Convention.ISMA);
            int settlementDays = 1;

            var discountCurve = new Handle<YieldTermStructure>(Utilities.flatRate(today, new SimpleQuote(0.03), new Actual360()));

            // actual market values from the evaluation date
            Frequency freq = Frequency.Semiannual;
            Schedule sch1 = new Schedule(new Date(2004, Month.October, 31), new Date(2006, Month.October, 31), new Period(freq),
                                         bondCalendar, BusinessDayConvention.Unadjusted, BusinessDayConvention.Unadjusted,
                                         DateGeneration.Rule.Backward, false);

            FixedRateBond bond1 = new FixedRateBond(settlementDays, vars.faceAmount, sch1, new List<double>() { 0.025 },
                                    bondDayCount, BusinessDayConvention.ModifiedFollowing, 100.0, new Date(2004, Month.November, 1));

            PricingEngine bondEngine = new DiscountingBondEngine(discountCurve);
            bond1.setPricingEngine(bondEngine);

            double marketPrice1 = 99.203125;
            double marketYield1 = 0.02925;

            Schedule sch2 = new Schedule(new Date(2004, Month.November, 15), new Date(2009, Month.November, 15), new Period(freq),
                                         bondCalendar, BusinessDayConvention.Unadjusted, BusinessDayConvention.Unadjusted,
                                         DateGeneration.Rule.Backward, false);

            FixedRateBond bond2 = new FixedRateBond(settlementDays, vars.faceAmount, sch2, new List<double>() { 0.035 },
                                         bondDayCount, BusinessDayConvention.ModifiedFollowing,
                                         100.0, new Date(2004, Month.November, 15));

            bond2.setPricingEngine(bondEngine);

            double marketPrice2 = 99.6875;
            double marketYield2 = 0.03569;

            // calculated values
            double cachedPrice1a = 99.204505, cachedPrice2a = 99.687192;
            double cachedPrice1b = 98.943393, cachedPrice2b = 101.986794;
            double cachedYield1a = 0.029257,  cachedYield2a = 0.035689;
            double cachedYield1b = 0.029045,  cachedYield2b = 0.035375;
            double cachedYield1c = 0.030423,  cachedYield2c = 0.030432;

            // check
            double tolerance = 1.0e-6;
            double price, yield;

            price = bond1.cleanPrice(marketYield1, bondDayCount, Compounding.Compounded, freq);
            if (Math.Abs(price-cachedPrice1a) > tolerance) {
                Console.WriteLine("failed to reproduce cached price:"
                           + "\n    calculated: " + price
                           + "\n    expected:   " + cachedPrice1a
                           + "\n    tolerance:  " + tolerance
                           + "\n    error:      " + (price-cachedPrice1a));
            }

            price = bond1.cleanPrice();
            if (Math.Abs(price-cachedPrice1b) > tolerance) {
                Console.WriteLine("failed to reproduce cached price:"
                           + "\n    calculated: " + price
                           + "\n    expected:   " + cachedPrice1b
                           + "\n    tolerance:  " + tolerance
                           + "\n    error:      " + (price-cachedPrice1b));
            }

            yield = bond1.yield(marketPrice1, bondDayCount, Compounding.Compounded, freq);
            if (Math.Abs(yield-cachedYield1a) > tolerance) {
                Console.WriteLine("failed to reproduce cached compounded yield:"
                           + "\n    calculated: " + yield
                           + "\n    expected:   " + cachedYield1a
                           + "\n    tolerance:  " + tolerance
                           + "\n    error:      " + (yield-cachedYield1a));
            }

            yield = bond1.yield(marketPrice1, bondDayCount, Compounding.Continuous, freq);
            if (Math.Abs(yield-cachedYield1b) > tolerance) {
                Console.WriteLine("failed to reproduce cached continuous yield:"
                           + "\n    calculated: " + yield
                           + "\n    expected:   " + cachedYield1b
                           + "\n    tolerance:  " + tolerance
                           + "\n    error:      " + (yield-cachedYield1b));
            }

            yield = bond1.yield(bondDayCount, Compounding.Continuous, freq);
            if (Math.Abs(yield-cachedYield1c) > tolerance) {
                Console.WriteLine("failed to reproduce cached continuous yield:"
                           + "\n    calculated: " + yield
                           + "\n    expected:   " + cachedYield1c
                           + "\n    tolerance:  " + tolerance
                           + "\n    error:      " + (yield-cachedYield1c));
            }


            price = bond2.cleanPrice(marketYield2, bondDayCount, Compounding.Compounded, freq);
            if (Math.Abs(price-cachedPrice2a) > tolerance) {
                Console.WriteLine("failed to reproduce cached price:"
                           + "\n    calculated: " + price
                           + "\n    expected:   " + cachedPrice2a
                           + "\n    tolerance:  " + tolerance
                           + "\n    error:      " + (price-cachedPrice2a));
            }

            price = bond2.cleanPrice();
            if (Math.Abs(price-cachedPrice2b) > tolerance) {
                Console.WriteLine("failed to reproduce cached price:"
                           + "\n    calculated: " + price
                           + "\n    expected:   " + cachedPrice2b
                           + "\n    tolerance:  " + tolerance
                           + "\n    error:      " + (price-cachedPrice2b));
            }

            yield = bond2.yield(marketPrice2, bondDayCount, Compounding.Compounded, freq);
            if (Math.Abs(yield-cachedYield2a) > tolerance) {
                Console.WriteLine("failed to reproduce cached compounded yield:"
                           + "\n    calculated: " + yield
                           + "\n    expected:   " + cachedYield2a
                           + "\n    tolerance:  " + tolerance
                           + "\n    error:      " + (yield-cachedYield2a));
            }

            yield = bond2.yield(marketPrice2, bondDayCount, Compounding.Continuous, freq);
            if (Math.Abs(yield-cachedYield2b) > tolerance) {
                Console.WriteLine("failed to reproduce cached continuous yield:"
                           + "\n    calculated: " + yield
                           + "\n    expected:   " + cachedYield2b
                           + "\n    tolerance:  " + tolerance
                           + "\n    error:      " + (yield-cachedYield2b));
            }

            yield = bond2.yield(bondDayCount, Compounding.Continuous, freq);
            if (Math.Abs(yield-cachedYield2c) > tolerance) {
                Console.WriteLine("failed to reproduce cached continuous yield:"
                           + "\n    calculated: " + yield
                           + "\n    expected:   " + cachedYield2c
                           + "\n    tolerance:  " + tolerance
                           + "\n    error:      " + (yield-cachedYield2c));
            }

            // with explicit settlement date:
            Schedule sch3 = new Schedule(new Date(2004, Month.November, 30), new Date(2006, Month.November, 30), new Period(freq),
                                         new UnitedStates(UnitedStates.Market.GovernmentBond), BusinessDayConvention.Unadjusted,
                                         BusinessDayConvention.Unadjusted, DateGeneration.Rule.Backward, false);

            FixedRateBond bond3 = new FixedRateBond(settlementDays, vars.faceAmount, sch3, new List<double>() { 0.02875 },
                                new ActualActual(ActualActual.Convention.ISMA),
                                BusinessDayConvention.ModifiedFollowing, 100.0, new Date(2004, Month.November, 30));

            bond3.setPricingEngine(bondEngine);

            double marketYield3 = 0.02997;

            Date settlementDate = new Date(2004, Month.November, 30);
            double cachedPrice3 = 99.764874;

            price = bond3.cleanPrice(marketYield3, bondDayCount, Compounding.Compounded, freq, settlementDate);
            if (Math.Abs(price - cachedPrice3) > tolerance) {
                Console.WriteLine("failed to reproduce cached price:"
                           + "\n    calculated: " + price + ""
                           + "\n    expected:   " + cachedPrice3 + ""
                           + "\n    error:      " + (price - cachedPrice3));
            }

            // this should give the same result since the issue date is the
            // earliest possible settlement date
            Settings.setEvaluationDate(new Date(2004, Month.November, 22));

            price = bond3.cleanPrice(marketYield3, bondDayCount, Compounding.Compounded, freq);
            if (Math.Abs(price - cachedPrice3) > tolerance) {
                Console.WriteLine("failed to reproduce cached price:"
                           + "\n    calculated: " + price + ""
                           + "\n    expected:   " + cachedPrice3 + ""
                           + "\n    error:      " + (price - cachedPrice3));
            }
        }
        void testCachedZero() {
            Console.WriteLine("Testing zero-coupon bond prices against cached values...");

            CommonVars vars = new CommonVars();

            Date today = new Date(2004, Month.November, 22);
            Settings.setEvaluationDate(today);

            int settlementDays = 1;

            var discountCurve = new Handle<YieldTermStructure>(Utilities.flatRate(today, 0.03, new Actual360()));

            double tolerance = 1.0e-6;

            // plain
            ZeroCouponBond bond1 = new ZeroCouponBond(settlementDays, new UnitedStates(UnitedStates.Market.GovernmentBond),
                                 vars.faceAmount, new Date(2008, Month.November, 30), BusinessDayConvention.ModifiedFollowing,
                                 100.0, new Date(2004, Month.November, 30));

            PricingEngine bondEngine = new DiscountingBondEngine(discountCurve);
            bond1.setPricingEngine(bondEngine);

            double cachedPrice1 = 88.551726;

            double price = bond1.cleanPrice();
            if (Math.Abs(price-cachedPrice1) > tolerance) {
                Console.WriteLine("failed to reproduce cached price:\n"
                           + "    calculated: " + price + "\n"
                           + "    expected:   " + cachedPrice1 + "\n"
                           + "    error:      " + (price-cachedPrice1));
            }

            ZeroCouponBond bond2 = new ZeroCouponBond(settlementDays, new UnitedStates(UnitedStates.Market.GovernmentBond),
                                 vars.faceAmount, new Date(2007, Month.November, 30), BusinessDayConvention.ModifiedFollowing,
                                 100.0, new Date(2004, Month.November, 30));

            bond2.setPricingEngine(bondEngine);

            double cachedPrice2 = 91.278949;

            price = bond2.cleanPrice();
            if (Math.Abs(price-cachedPrice2) > tolerance) {
                Console.WriteLine("failed to reproduce cached price:\n"
                           + "    calculated: " + price + "\n"
                           + "    expected:   " + cachedPrice2 + "\n"
                           + "    error:      " + (price-cachedPrice2));
            }

            ZeroCouponBond bond3 = new ZeroCouponBond(settlementDays, new UnitedStates(UnitedStates.Market.GovernmentBond),
                                 vars.faceAmount, new Date(2006, Month.November, 30), BusinessDayConvention.ModifiedFollowing,
                                 100.0, new Date(2004, Month.November, 30));

            bond3.setPricingEngine(bondEngine);

            double cachedPrice3 = 94.098006;

            price = bond3.cleanPrice();
            if (Math.Abs(price - cachedPrice3) > tolerance) {
                Console.WriteLine("failed to reproduce cached price:\n"
                           + "    calculated: " + price + "\n"
                           + "    expected:   " + cachedPrice3 + "\n"
                           + "    error:      " + (price-cachedPrice3));
            }
        }
        void testCachedFixed() {
            Console.WriteLine("Testing fixed-coupon bond prices against cached values...");

            CommonVars vars = new CommonVars();

            Date today = new Date(2004, Month.November, 22);
            Settings.setEvaluationDate(today);

            int settlementDays = 1;

            var discountCurve = new Handle<YieldTermStructure>(Utilities.flatRate(today, 0.03, new Actual360()));

            double tolerance = 1.0e-6;

            // plain
            Schedule sch = new Schedule(new Date(2004, Month.November, 30),
                         new Date(2008, Month.November, 30), new Period(Frequency.Semiannual),
                         new UnitedStates(UnitedStates.Market.GovernmentBond),
                         BusinessDayConvention.Unadjusted, BusinessDayConvention.Unadjusted, DateGeneration.Rule.Backward, false);

            FixedRateBond bond1 = new FixedRateBond(settlementDays, vars.faceAmount, sch, new List<double>() { 0.02875 },
                                new ActualActual(ActualActual.Convention.ISMA), BusinessDayConvention.ModifiedFollowing,
                                100.0, new Date(2004, Month.November, 30));

            PricingEngine bondEngine = new DiscountingBondEngine(discountCurve);
            bond1.setPricingEngine(bondEngine);

            double cachedPrice1 = 99.298100;

            double price = bond1.cleanPrice();
            if (Math.Abs(price-cachedPrice1) > tolerance) {
                Console.WriteLine("failed to reproduce cached price:\n"
                           + "    calculated: " + price + "\n"
                           + "    expected:   " + cachedPrice1 + "\n"
                           + "    error:      " + (price-cachedPrice1));
            }

            // varying coupons
            Array<double> couponRates = new Array<double>(4);
            couponRates[0] = 0.02875;
            couponRates[1] = 0.03;
            couponRates[2] = 0.03125;
            couponRates[3] = 0.0325;

            FixedRateBond bond2 = new FixedRateBond(settlementDays, vars.faceAmount, sch, couponRates,
                                  new ActualActual(ActualActual.Convention.ISMA),
                                  BusinessDayConvention.ModifiedFollowing,
                                  100.0, new Date(2004, Month.November, 30));

            bond2.setPricingEngine(bondEngine);

            double cachedPrice2 = 100.334149;

            price = bond2.cleanPrice();
            if (Math.Abs(price-cachedPrice2) > tolerance) {
                Console.WriteLine("failed to reproduce cached price:\n"
                           + "    calculated: " + price + "\n"
                           + "    expected:   " + cachedPrice2 + "\n"
                           + "    error:      " + (price-cachedPrice2));
            }

            // stub date
            Schedule sch3 = new Schedule(new Date(2004, Month.November, 30),
                          new Date(2009, Month.March, 30), new Period(Frequency.Semiannual),
                          new UnitedStates(UnitedStates.Market.GovernmentBond),
                          BusinessDayConvention.Unadjusted, BusinessDayConvention.Unadjusted, DateGeneration.Rule.Backward, false,
                          null, new Date(2008, Month.November, 30));

            FixedRateBond bond3 = new FixedRateBond(settlementDays, vars.faceAmount, sch3,
                                  couponRates, new ActualActual(ActualActual.Convention.ISMA),
                                  BusinessDayConvention.ModifiedFollowing,
                                  100.0, new Date(2004, Month.November, 30));

            bond3.setPricingEngine(bondEngine);

            double cachedPrice3 = 100.382794;

            price = bond3.cleanPrice();
            if (Math.Abs(price-cachedPrice3) > tolerance) {
                Console.WriteLine("failed to reproduce cached price:\n"
                           + "    calculated: " + price + "\n"
                           + "    expected:   " + cachedPrice3 + "\n"
                           + "    error:      " + (price-cachedPrice3));
            }
        }
        void testCachedFloating() {
            Console.WriteLine("Testing floating-rate bond prices against cached values...");

            CommonVars vars = new CommonVars();

            Date today = new Date(2004, Month.November, 22);
            Settings.setEvaluationDate(today);

            int settlementDays = 1;

            var riskFreeRate = new Handle<YieldTermStructure>(Utilities.flatRate(today, 0.025, new Actual360()));
            var discountCurve = new Handle<YieldTermStructure>(Utilities.flatRate(today, 0.03, new Actual360()));

            IborIndex index = new USDLibor(new Period(6, TimeUnit.Months), riskFreeRate);
            int fixingDays = 1;

            double tolerance = 1.0e-6;

            IborCouponPricer pricer = new BlackIborCouponPricer(new Handle<OptionletVolatilityStructure>());

            // plain
            Schedule sch = new Schedule(new Date(2004, Month.November, 30), new Date(2008, Month.November, 30),
                                        new Period(Frequency.Semiannual), new UnitedStates(UnitedStates.Market.GovernmentBond),
                                        BusinessDayConvention.ModifiedFollowing, BusinessDayConvention.ModifiedFollowing,
                                        DateGeneration.Rule.Backward, false);

            FloatingRateBond bond1 = new FloatingRateBond(settlementDays, vars.faceAmount, sch,
                                   index, new ActualActual(ActualActual.Convention.ISMA),
                                   BusinessDayConvention.ModifiedFollowing, fixingDays,
                                   new List<double>(), new List<double>(),
                                   new List<double>(), new List<double>(),
                                   false,
                                   100.0, new Date(2004, Month.November, 30));

            PricingEngine bondEngine = new DiscountingBondEngine(riskFreeRate);
            bond1.setPricingEngine(bondEngine);

            Utils.setCouponPricer(bond1.cashflows(),pricer);

            #if QL_USE_INDEXED_COUPON
            double cachedPrice1 = 99.874645;
            #else
            double cachedPrice1 = 99.874646;
            #endif


            double price = bond1.cleanPrice();
            if (Math.Abs(price-cachedPrice1) > tolerance) {
                Console.WriteLine("failed to reproduce cached price:\n"
                           + "    calculated: " + price + "\n"
                           + "    expected:   " + cachedPrice1 + "\n"
                           + "    error:      " + (price-cachedPrice1));
            }

            // different risk-free and discount curve
            FloatingRateBond bond2 = new FloatingRateBond(settlementDays, vars.faceAmount, sch,
                                   index, new ActualActual(ActualActual.Convention.ISMA),
                                   BusinessDayConvention.ModifiedFollowing, fixingDays,
                                   new List<double>(), new List<double>(),
                                   new List<double>(), new List<double>(),
                                   false,
                                   100.0, new Date(2004, Month.November, 30));

            PricingEngine bondEngine2 = new DiscountingBondEngine(discountCurve);
            bond2.setPricingEngine(bondEngine2);

            Utils.setCouponPricer(bond2.cashflows(),pricer);

            #if QL_USE_INDEXED_COUPON
            double cachedPrice2 = 97.955904;
            #else
            double cachedPrice2 = 97.955904;
            #endif

            price = bond2.cleanPrice();
            if (Math.Abs(price-cachedPrice2) > tolerance) {
                Console.WriteLine("failed to reproduce cached price:\n"
                           + "    calculated: " + price + "\n"
                           + "    expected:   " + cachedPrice2 + "\n"
                           + "    error:      " + (price-cachedPrice2));
            }

            // varying spread
            Array<double> spreads = new Array<double>(4);
            spreads[0] = 0.001;
            spreads[1] = 0.0012;
            spreads[2] = 0.0014;
            spreads[3] = 0.0016;

            FloatingRateBond bond3 = new FloatingRateBond(settlementDays, vars.faceAmount, sch,
                                   index, new ActualActual(ActualActual.Convention.ISMA),
                                   BusinessDayConvention.ModifiedFollowing, fixingDays,
                                   new List<double>(), spreads,
                                   new List<double>(), new List<double>(),
                                   false,
                                   100.0, new Date(2004, Month.November, 30));

            bond3.setPricingEngine(bondEngine2);

            Utils.setCouponPricer(bond3.cashflows(),pricer);

            #if QL_USE_INDEXED_COUPON
            double cachedPrice3 = 98.495458;
            #else
            double cachedPrice3 = 98.495459;
            #endif

            price = bond3.cleanPrice();
            if (Math.Abs(price-cachedPrice3) > tolerance) {
                Console.WriteLine("failed to reproduce cached price:\n"
                           + "    calculated: " + price + "\n"
                           + "    expected:   " + cachedPrice3 + "\n"
                           + "    error:      " + (price-cachedPrice3));
            }
        }
        void testBrazilianCached() {
            Console.WriteLine("Testing Brazilian public bond prices against cached values...");

            CommonVars vars = new CommonVars();

            Date today = new Date(2007, Month.June, 6);
            Settings.setEvaluationDate(today);

            // NTN-F maturity dates
            Array<Date> maturityDates = new Array<Date>(6);
            maturityDates[0] = new Date(2008, Month.January, 1);
            maturityDates[1] = new Date(2010, Month.January, 1);
            maturityDates[2] = new Date(2010, Month.July, 1);
            maturityDates[3] = new Date(2012, Month.January, 1);
            maturityDates[4] = new Date(2014, Month.January, 1);
            maturityDates[5] = new Date(2017, Month.January, 1);

            // NTN-F yields
            Array<double> yields = new Array<double>(6);
            yields[0] = 0.114614;
            yields[1] = 0.105726;
            yields[2] = 0.105328;
            yields[3] = 0.104283;
            yields[4] = 0.103218;
            yields[5] = 0.102948;

            // NTN-F prices
            Array<double> prices = new Array<double>(6);
            prices[0] = 1034.63031372;
            prices[1] = 1030.09919487;
            prices[2] = 1029.98307160;
            prices[3] = 1028.13585068;
            prices[4] = 1028.33383817;
            prices[5] = 1026.19716497;

            int settlementDays = 1;
            vars.faceAmount = 1000.0;

            // The tolerance is high because Andima truncate yields
            double tolerance = 1.0e-4;

            Array<InterestRate> couponRates = new Array<InterestRate>(1);
            couponRates[0] = new InterestRate(0.1,new Thirty360(), Compounding.Compounded, Frequency.Annual);

            for (int bondIndex = 0; bondIndex < maturityDates.Count; bondIndex++) {

                // plain
                InterestRate yield = new InterestRate(yields[bondIndex], new Business252(new Brazil()),
                                                      Compounding.Compounded, Frequency.Annual);

                Schedule schedule = new Schedule(new Date(2007, Month.January, 1),
                                  maturityDates[bondIndex], new Period(Frequency.Semiannual),
                                  new Brazil(Brazil.Market.Settlement),
                                  BusinessDayConvention.Unadjusted, BusinessDayConvention.Unadjusted,
                                  DateGeneration.Rule.Backward, false);

                // fixed coupons
                List<CashFlow> cashflows = new FixedRateLeg(schedule, new Actual360())
                                .withNotionals(vars.faceAmount)
                                .withCouponRates(couponRates)
                                .withPaymentAdjustment(BusinessDayConvention.ModifiedFollowing).value();
                // redemption
                cashflows.Add(new SimpleCashFlow(vars.faceAmount, cashflows.Last().date()));

                Bond bond = new Bond(settlementDays, new Brazil(Brazil.Market.Settlement),
                                     vars.faceAmount, cashflows.Last().date(),
                                     new Date(2007,Month.January,1), cashflows);

                double cachedPrice = prices[bondIndex];

                double price = vars.faceAmount*bond.dirtyPrice(yield.rate(),
                                                             yield.dayCounter(),
                                                             yield.compounding(),
                                                             yield.frequency(),
                                                             today)/100;
                if (Math.Abs(price-cachedPrice) > tolerance) {
                    Console.WriteLine("failed to reproduce cached price:\n"
                                + "    calculated: " + price + "\n"
                                + "    expected:   " + cachedPrice + "\n"
                                + "    error:      " + (price-cachedPrice) + "\n"
                                );
                }
            }
        }


        public void suite() {
            testYield();
            testTheoretical();
            testCached();
            testCachedZero();
            testCachedFixed();
            testCachedFloating();
            testBrazilianCached();
        }
    }
}
