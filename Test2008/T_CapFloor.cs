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

namespace TestSuite
{
   [TestClass()]
   public class T_CapFloor
   {
      class CommonVars
      {
         // common data
         Date settlement;
         List<double> nominals;
         BusinessDayConvention convention;
         Frequency frequency;
         IborIndex index;
         Calendar calendar;
         int fixingDays;
         RelinkableHandle<YieldTermStructure> termStructure;

         // cleanup
         SavedSettings backup;

         // setup
         public CommonVars()
         {
            nominals = new List<double>(){100};
            frequency = Frequency.Semiannual;
            index = (IborIndex)new Euribor6M(termStructure);
            calendar = index.fixingCalendar();
            convention = BusinessDayConvention.ModifiedFollowing;
            Date today = calendar.adjust(Date.Today);
            Settings.setEvaluationDate(today);
            int settlementDays = 2;
            fixingDays = 2;
            settlement = calendar.advance(today, settlementDays,TimeUnit.Days);
            termStructure.linkTo(Utilities.flatRate(settlement, 0.05,
                                          new ActualActual(ActualActual.Convention.ISDA)));

         }

         // utilities
         List<CashFlow> makeLeg(Date startDate, int length)
         {
            Date endDate = calendar.advance(startDate, new Period(length , TimeUnit.Years), convention);
            Schedule schedule = new Schedule(startDate, endDate, new Period(frequency), calendar,
                                             convention, convention, DateGeneration.Rule.Forward, 
                                             false);
            return new IborLeg(schedule, index)
                .withNotionals(nominals)
                .withPaymentDayCounter(index.dayCounter())
                .withPaymentAdjustment(convention)
                .withFixingDays(fixingDays).value();
         }

         IPricingEngine makeEngine(double volatility) 
         {
            Handle<Quote> vol = new Handle<Quote>(new SimpleQuote(volatility)) ;

            return (IPricingEngine) new BlackCapFloorEngine(termStructure, vol);
        }
      }

   }
}
