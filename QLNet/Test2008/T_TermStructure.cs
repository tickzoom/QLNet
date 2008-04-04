using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QLNet;

namespace TestSuite {
    public class TermStructure {
        public class CommonVars {
            public struct Datum {
                public int n;
                public TimeUnit units;
                public double rate;
            }

            public Datum[] depositData = new Datum[] {
                new Datum { n = 1, units = TimeUnit.Months, rate = 4.581 },
                new Datum { n = 2, units = TimeUnit.Months, rate = 4.573 },
                new Datum { n = 3, units = TimeUnit.Months, rate = 4.557 },
                new Datum { n = 6, units = TimeUnit.Months, rate = 4.496 },
                new Datum { n = 9, units = TimeUnit.Months, rate = 4.490 }
            };

            public Datum[] swapData = new Datum[] {
                new Datum { n =  1, units = TimeUnit.Years, rate = 4.54 },
                new Datum { n =  5, units = TimeUnit.Years, rate = 4.99 },
                new Datum { n = 10, units = TimeUnit.Years, rate = 5.47 },
                new Datum { n = 20, units = TimeUnit.Years, rate = 5.89 },
                new Datum { n = 30, units = TimeUnit.Years, rate = 5.96 }
            };

            // common data
            Calendar calendar;
            int settlementDays;
            YieldTermStructure termStructure;
            YieldTermStructure dummyTermStructure;

            // cleanup
            // SavedSettings backup;

            // setup
            public CommonVars() {
                calendar = new TARGET();
                settlementDays = 2;
                Date today = calendar.adjust(Date.Today);
                Settings.setEvaluationDate(today);
                Date settlement = calendar.advance(today,settlementDays,TimeUnit.Days);

                int deposits = depositData.Length,
                    swaps = swapData.Length;

                public var instruments = new Array<BootstrapHelper<YieldTermStructure>>(deposits+swaps);
                
                for (int i=0; i<deposits; i++) {
                    instruments[i] = new DepositRateHelper(depositData[i].rate/100,
                                          depositData[i].n*depositData[i].units,
                                          settlementDays, calendar,
                                          ModifiedFollowing, true,
                                          Actual360()));
                }
                boost::shared_ptr<IborIndex> index(new IborIndex("dummy",
                                                                 6*Months,
                                                                 settlementDays,
                                                                 Currency(),
                                                                 calendar,
                                                                 ModifiedFollowing,
                                                                 false,
                                                                 Actual360()));
                for (int i=0; i<swaps; ++i) {
                    instruments[i+deposits] = boost::shared_ptr<RateHelper>(new
                        SwapRateHelper(swapData[i].rate/100,
                                       swapData[i].n*swapData[i].units,
                                       calendar,
                                       Annual, Unadjusted, Thirty360(),
                                       index));
                }
                termStructure = new PiecewiseYieldCurve<Discount,LogLinear>(settlement,
                                                            instruments, Actual360()));
                dummyTermStructure = new PiecewiseYieldCurve<Discount,LogLinear>(settlement,
                                                            instruments, Actual360()));
            }
        }
    }
}
