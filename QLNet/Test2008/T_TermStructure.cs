/*
 Copyright (C) 2008 Andrea Maggiulli
  
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QLNet;

namespace Test2008
{
    [TestClass()]
    public class T_TermStructure
    {
        class Datum
        {
            public int n;
            public TimeUnit units;
            public double rate;
            public Datum(int n_, TimeUnit units_, double rate_)
            {
                n = n_;
                units = units_;
                rate = rate_;
            }
        };
        class CommonVars
        {
            Calendar calendar;
            int settlementDays;
            YieldTermStructure termStructure;
            YieldTermStructure dummyTermStructure;

            // cleanup
            //SavedSettings backup;

            public CommonVars()
            {
                calendar = new TARGET();
                settlementDays = 2;
                Date today = calendar.adjust(Date.Today);
                Settings.setEvaluationDate(today);
                Date settlement = calendar.advance(today, settlementDays, TimeUnit.Days);
                Datum[] depositData = {
                    new Datum( 1, TimeUnit.Months, 4.581 ),
                    new Datum( 2, TimeUnit.Months, 4.573 ),
                    new Datum( 3, TimeUnit.Months, 4.557 ),
                    new Datum( 6, TimeUnit.Months, 4.496 ),
                    new Datum( 9, TimeUnit.Months, 4.490 )
                };
                Datum[] swapData = {
                    new Datum(  1, TimeUnit.Years, 4.54 ),
                    new Datum(  5, TimeUnit.Years, 4.99 ),
                    new Datum( 10, TimeUnit.Years, 5.47 ),
                    new Datum( 20, TimeUnit.Years, 5.89 ),
                    new Datum( 30, TimeUnit.Years, 5.96 )
                };
                int deposits = depositData.Length,
                    swaps = swapData.Length;

                var instruments = new List<BootstrapHelper<YieldTermStructure>>(deposits + swaps);

                for (int i = 0; i < deposits; i++)
                {
                    instruments[i] = new DepositRateHelper(depositData[i].rate / 100,
                                            new Period(depositData[i].n, depositData[i].units),
                                            settlementDays, calendar,
                                            BusinessDayConvention.ModifiedFollowing, true,
                                            new Actual360());
                }
                IborIndex index = new IborIndex("dummy",
                                                new Period(6, TimeUnit.Months),
                                                settlementDays,
                                                new Currency(),
                                                calendar,
                                                BusinessDayConvention.ModifiedFollowing,
                                                false,
                                                new Actual360());

                for (int i = 0; i < swaps; ++i)
                {
                    instruments[i + deposits] = new SwapRateHelper(swapData[i].rate / 100,
                                                  new Period(swapData[i].n, swapData[i].units),
                                                  calendar, Frequency.Annual, BusinessDayConvention.Unadjusted,
                                                  new Thirty360(), index);

                }

                // how to do this ?
                //termStructure = new PiecewiseYieldCurve<Discount,LogLinear>(settlement,
                //                                        instruments, new Actual360());
                //dummyTermStructure = new PiecewiseYieldCurve<Discount,LogLinear>(settlement,
                //                                        instruments, new Actual360());

            }

        }
    }
}
