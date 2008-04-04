using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QLNet;

namespace TestSuite {
    public class InterpolatedYieldCurveTest {
        public class CommonVars {
            #region Values
            public struct Datum {
                public int n;
                public TimeUnit units;
                public double rate;
            };
            public struct BondDatum {
                public int n;
                public TimeUnit units;
                public int length;
                public Frequency frequency;
                public double coupon;
                public double price;
            };

            public Datum[] depositData = new Datum[] {
                new Datum { n = 1, units = TimeUnit.Weeks,  rate = 4.559 },
                new Datum { n = 1, units = TimeUnit.Months, rate = 4.581 },
                new Datum { n = 2, units = TimeUnit.Months, rate = 4.573 },
                new Datum { n = 3, units = TimeUnit.Months, rate = 4.557 },
                new Datum { n = 6, units = TimeUnit.Months, rate = 4.496 },
                new Datum { n = 9, units = TimeUnit.Months, rate = 4.490 }
            };

            public Datum[] fraData = new Datum[] {
                new Datum { n = 1, units = TimeUnit.Months, rate = 4.581 },
                new Datum { n = 2, units = TimeUnit.Months, rate = 4.573 },
                new Datum { n = 3, units = TimeUnit.Months, rate = 4.557 },
                new Datum { n = 6, units = TimeUnit.Months, rate = 4.496 },
                new Datum { n = 9, units = TimeUnit.Months, rate = 4.490 }
            };

            public Datum[] swapData = new Datum[] {
                new Datum { n =  1, units = TimeUnit.Years, rate = 4.54 },
                new Datum { n =  2, units = TimeUnit.Years, rate = 4.63 },
                new Datum { n =  3, units = TimeUnit.Years, rate = 4.75 },
                new Datum { n =  4, units = TimeUnit.Years, rate = 4.86 },
                new Datum { n =  5, units = TimeUnit.Years, rate = 4.99 },
                new Datum { n =  6, units = TimeUnit.Years, rate = 5.11 },
                new Datum { n =  7, units = TimeUnit.Years, rate = 5.23 },
                new Datum { n =  8, units = TimeUnit.Years, rate = 5.33 },
                new Datum { n =  9, units = TimeUnit.Years, rate = 5.41 },
                new Datum { n = 10, units = TimeUnit.Years, rate = 5.47 },
                new Datum { n = 12, units = TimeUnit.Years, rate = 5.60 },
                new Datum { n = 15, units = TimeUnit.Years, rate = 5.75 },
                new Datum { n = 20, units = TimeUnit.Years, rate = 5.89 },
                new Datum { n = 25, units = TimeUnit.Years, rate = 5.95 },
                new Datum { n = 30, units = TimeUnit.Years, rate = 5.96 }
            };

            public BondDatum[] bondData = new BondDatum[] {
                new BondDatum { n =  6, units = TimeUnit.Months, length = 5,  frequency = Frequency.Semiannual, coupon = 4.75, price = 101.320 },
                new BondDatum { n =  1, units = TimeUnit.Years,  length = 3,  frequency = Frequency.Semiannual, coupon = 2.75, price = 100.590 },
                new BondDatum { n =  2, units = TimeUnit.Years,  length = 5,  frequency = Frequency.Semiannual, coupon = 5.00, price = 105.650 },
                new BondDatum { n =  5, units = TimeUnit.Years,  length = 11, frequency = Frequency.Semiannual, coupon = 5.50, price = 113.610 },
                new BondDatum { n = 10, units = TimeUnit.Years,  length = 11, frequency = Frequency.Semiannual, coupon = 3.75, price = 104.070 }
            };

            public Datum[] bmaData = new Datum[] {
                new Datum { n =  1, units = TimeUnit.Years, rate = 67.56 },
                new Datum { n =  2, units = TimeUnit.Years, rate = 68.00 },
                new Datum { n =  3, units = TimeUnit.Years, rate = 68.25 },
                new Datum { n =  4, units = TimeUnit.Years, rate = 68.50 },
                new Datum { n =  5, units = TimeUnit.Years, rate = 68.81 },
                new Datum { n =  7, units = TimeUnit.Years, rate = 69.50 },
                new Datum { n = 10, units = TimeUnit.Years, rate = 70.44 },
                new Datum { n = 15, units = TimeUnit.Years, rate = 71.69 },
                new Datum { n = 20, units = TimeUnit.Years, rate = 72.69 },
                new Datum { n = 30, units = TimeUnit.Years, rate = 73.81 }
            };
            #endregion

            // global variables
            public Calendar calendar;
            public int settlementDays;
            public Date today, settlement;
            public BusinessDayConvention fixedLegConvention;
            public Frequency fixedLegFrequency;
            public DayCounter fixedLegDayCounter;
            public int bondSettlementDays;
            public DayCounter bondDayCounter;
            public BusinessDayConvention bondConvention;
            public double bondRedemption;
            public Frequency bmaFrequency;
            public BusinessDayConvention bmaConvention;
            public DayCounter bmaDayCounter;

            public int deposits, fras, swaps, bonds, bmas;
            public List<Quote> rates, fraRates, prices, fractions;
            public List<BootstrapHelper<YieldTermStructure>> instruments, fraHelpers, bondHelpers, bmaHelpers;
            public List<Schedule> schedules;
            public YieldTermStructure termStructure;

            // cleanup
            //SavedSettings backup;
            //IndexHistoryCleaner cleaner;

            // setup
            public CommonVars() {
                // data
                calendar = new TARGET();
                settlementDays = 2;
                today = calendar.adjust(Date.Today);
                Settings.setEvaluationDate(today);
                settlement = calendar.advance(today,settlementDays, TimeUnit.Days);
                fixedLegConvention = BusinessDayConvention.Unadjusted;
                fixedLegFrequency = Frequency.Annual;
                fixedLegDayCounter = new Thirty360(Thirty360.Thirty360Convention.European);
                bondSettlementDays = 3;
                bondDayCounter = new ActualActual();
                bondConvention = BusinessDayConvention.Following;
                bondRedemption = 100.0;
                bmaFrequency = Frequency.Quarterly;
                bmaConvention = BusinessDayConvention.Following;
                bmaDayCounter = new ActualActual();

                deposits = depositData.Length;
                fras = fraData.Length;
                swaps = swapData.Length;
                bonds = bondData.Length;
                bmas = bmaData.Length;

                // market elements
                rates = new List<Quote>(deposits+swaps);
                fraRates = new List<Quote>(fras);
                prices = new List<Quote>(bonds);
                fractions = new List<Quote>(bmas);
                for (int i=0; i<deposits; i++) {
                    rates.Add(new SimpleQuote(depositData[i].rate/100));
                }
                for (int i=0; i<swaps; i++) {
                    rates.Add(new SimpleQuote(swapData[i].rate/100));
                }
                for (int i=0; i<fras; i++) {
                    fraRates.Add(new SimpleQuote(fraData[i].rate/100));
                }
                for (int i=0; i<bonds; i++) {
                    prices.Add(new SimpleQuote(bondData[i].price));
                }
                for (int i=0; i<bmas; i++) {
                    fractions.Add(new SimpleQuote(bmaData[i].rate/100));
                }

                // rate helpers
                instruments = new List<BootstrapHelper<YieldTermStructure>>(deposits + swaps);
                fraHelpers = new List<BootstrapHelper<YieldTermStructure>>(fras);
                bondHelpers = new List<BootstrapHelper<YieldTermStructure>>(bonds);
                schedules = new List<Schedule>(bonds);
                bmaHelpers = new List<BootstrapHelper<YieldTermStructure>>(bmas);

                IborIndex euribor6m = new Euribor6M();
                for (int i=0; i<deposits; i++) {
                    Handle<Quote> r = new Handle<Quote>(rates[i]);
                    instruments.Add(new DepositRateHelper(r, new Period(depositData[i].n, depositData[i].units),
                                      euribor6m.fixingDays(), calendar,
                                      euribor6m.businessDayConvention(),
                                      euribor6m.endOfMonth(),
                                      euribor6m.dayCounter()));
                }
                //for (int i = 0; i < swaps; i++) {
                //    Handle<Quote> r = new Handle<Quote>(rates[i + deposits]);
                //    instruments.Add(new SwapRateHelper(r, new Period(swapData[i].n, swapData[i].units), calendar,
                //                    fixedLegFrequency, fixedLegConvention, fixedLegDayCounter, euribor6m));
                //}

                Euribor3M euribor3m = new Euribor3M();
                for (int i=0; i<fras; i++) {
                    Handle<Quote> r = new Handle<Quote>(fraRates[i]);
                    fraHelpers.Add(new FraRateHelper(r, fraData[i].n, fraData[i].n + 3,
                                      euribor3m.fixingDays(),
                                      euribor3m.fixingCalendar(),
                                      euribor3m.businessDayConvention(),
                                      euribor3m.endOfMonth(),
                                      euribor3m.dayCounter()));
                }

                for (int i=0; i<bonds; i++) {
                    Handle<Quote> p = new Handle<Quote>(prices[i]);
                    Date maturity = calendar.advance(today, bondData[i].n, bondData[i].units);
                    Date issue =    calendar.advance(maturity, -bondData[i].length, TimeUnit.Years);
                    List<double> coupons = new List<double>(); coupons.Add(1); coupons.Add(bondData[i].coupon / 100.0);
                    schedules.Add(new Schedule(issue, maturity, new Period(bondData[i].frequency), calendar,
                                               bondConvention, bondConvention, DateGeneration.Rule.Backward, false));
                    bondHelpers.Add(new FixedRateBondHelper(p, bondSettlementDays, bondRedemption, schedules[i],
                                                coupons, bondDayCounter, bondConvention, bondRedemption, issue));
                }
            }
        }

        public void testLinearDiscountConsistency() {
            Console.WriteLine("Testing consistency of piecewise-linear discount curve...");

            CommonVars vars = new CommonVars();

            InterpolatedDiscountCurve<Linear, IterativeBootstrap> curve = new InterpolatedDiscountCurve<Linear,IterativeBootstrap>(
                vars.settlement, vars.instruments, new Actual360(), new Handle<Quote>(), 1.0e-12, new Linear());

            testCurveConsistency(curve, vars);
            //testBMACurveConsistency(Discount(), Linear(), vars);
        }

        public void testCurveConsistency(InterpolatedDiscountCurve<Linear, IterativeBootstrap> curve, CommonVars vars) {

            vars.termStructure = curve;
            RelinkableHandle<YieldTermStructure> curveHandle = new RelinkableHandle<YieldTermStructure>();
            curveHandle.linkTo(curve);

            // check deposits
            for (int i=0; i<vars.deposits; i++) {
                Euribor index = new Euribor(new Period(vars.depositData[i].n, vars.depositData[i].units), curveHandle);
                double expectedRate = vars.depositData[i].rate / 100,
                       estimatedRate = index.fixing(vars.today);
                if (Math.Abs(expectedRate-estimatedRate) > 1.0e-9) {
                    throw new ApplicationException(vars.depositData[i].n + " "
                        + (vars.depositData[i].units == TimeUnit.Weeks ? "week(s)" : "month(s)")
                        + " deposit:"
                        + "\n    estimated rate: " + estimatedRate
                        + "\n    expected rate:  " + expectedRate);
                }
            }

            #region MyRegion
            // check swaps
            //IborIndex euribor6m = new IborIndex(new Euribor6M(curveHandle));
            //for (int i=0; i<vars.swaps; i++) {
            //    Period tenor = new Period(swapData[i].n, swapData[i].units);

            //    VanillaSwap swap = MakeVanillaSwap(tenor, euribor6m, 0.0)
            //        .withEffectiveDate(vars.settlement)
            //        .withFixedLegDayCount(vars.fixedLegDayCounter)
            //        .withFixedLegTenor(Period(vars.fixedLegFrequency))
            //        .withFixedLegConvention(vars.fixedLegConvention)
            //        .withFixedLegTerminationDateConvention(vars.fixedLegConvention);

            //    Rate expectedRate = swapData[i].rate/100,
            //         estimatedRate = swap.fairRate();
            //    Real tolerance = 1.0e-9;
            //    Spread error = std::fabs(expectedRate-estimatedRate);
            //    if (error > tolerance) {
            //        BOOST_ERROR(
            //            swapData[i].n << " year(s) swap:\n"
            //            << std::setprecision(8)
            //            << "\n estimated rate: " << io::rate(estimatedRate)
            //            << "\n expected rate:  " << io::rate(expectedRate)
            //            << "\n error:          " << io::rate(error)
            //            << "\n tolerance:      " << io::rate(tolerance));
            //    }
            //}

            //// check bonds
            //vars.termStructure = boost::shared_ptr<YieldTermStructure>(new
            //    PiecewiseYieldCurve<T,I>(vars.settlement, vars.bondHelpers,
            //                             Actual360(), Handle<Quote>(), 1.0e-12,
            //                             interpolator));
            //curveHandle.linkTo(vars.termStructure);

            //for (Size i=0; i<vars.bonds; i++) {
            //    Date maturity = vars.calendar.advance(vars.today,
            //                                          bondData[i].n,
            //                                          bondData[i].units);
            //    Date issue = vars.calendar.advance(maturity,
            //                                       -bondData[i].length,
            //                                       Years);
            //    std::vector<Rate> coupons(1, bondData[i].coupon/100.0);

            //    FixedRateBond bond(vars.bondSettlementDays, 100.0,
            //                       vars.schedules[i], coupons,
            //                       vars.bondDayCounter, vars.bondConvention,
            //                       vars.bondRedemption, issue);

            //    boost::shared_ptr<PricingEngine> bondEngine(
            //                              new DiscountingBondEngine(curveHandle));
            //    bond.setPricingEngine(bondEngine);

            //    Real expectedPrice = bondData[i].price,
            //         estimatedPrice = bond.cleanPrice();
            //    Real tolerance = 1.0e-9;
            //    if (std::fabs(expectedPrice-estimatedPrice) > tolerance) {
            //        BOOST_ERROR(io::ordinal(i+1) << " bond failure:" <<
            //                    std::setprecision(8) <<
            //                    "\n  estimated price: " << estimatedPrice <<
            //                    "\n  expected price:  " << expectedPrice);
            //    }
            //}

            //// check FRA
            //vars.termStructure = boost::shared_ptr<YieldTermStructure>(new
            //    PiecewiseYieldCurve<T,I>(vars.settlement, vars.fraHelpers,
            //                             Actual360(), Handle<Quote>(), 1.0e-12,
            //                             interpolator));
            //curveHandle.linkTo(vars.termStructure);

            //boost::shared_ptr<IborIndex> euribor3m(new Euribor3M(curveHandle));
            //for (Size i=0; i<vars.fras; i++) {
            //    Date start =
            //        vars.calendar.advance(vars.settlement,
            //                              fraData[i].n,
            //                              fraData[i].units,
            //                              euribor3m->businessDayConvention(),
            //                              euribor3m->endOfMonth());
            //    Date end = vars.calendar.advance(start, 3, Months,
            //                                     euribor3m->businessDayConvention(),
            //                                     euribor3m->endOfMonth());

            //    ForwardRateAgreement fra(start, end, Position::Long,
            //                             fraData[i].rate/100, 100.0,
            //                             euribor3m, curveHandle);
            //    Rate expectedRate = fraData[i].rate/100,
            //         estimatedRate = fra.forwardRate();
            //    Real tolerance = 1.0e-9;
            //    if (std::fabs(expectedRate-estimatedRate) > tolerance) {
            //        BOOST_ERROR(io::ordinal(i+1) << " FRA failure:" <<
            //                    std::setprecision(8) <<
            //                    "\n  estimated rate: " << io::rate(estimatedRate) <<
            //                    "\n  expected rate:  " << io::rate(expectedRate));
            //    }
            //} 
            #endregion
        }

        public void suite() {
            testLinearDiscountConsistency();
        }
    }
}
