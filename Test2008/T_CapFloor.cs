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
         public Date settlement;
         public List<double> nominals;
         public BusinessDayConvention convention;
         public Frequency frequency;
         public IborIndex index;
         public Calendar calendar;
         public int fixingDays;
         public RelinkableHandle<YieldTermStructure> termStructure = new RelinkableHandle<YieldTermStructure> ();

         // cleanup
         public SavedSettings backup;

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
         public List<CashFlow> makeLeg(Date startDate, int length)
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
         
         public CapFloor makeCapFloor(CapFloorType type,
                               List<CashFlow> leg,
                               double strike,
                               double volatility) 
         {
            CapFloor result;
            switch (type) 
            {
              case CapFloorType.Cap:
                result = (CapFloor) new Cap(leg, new List<double>(){strike});
                break;
              case CapFloorType.Floor:
                result = (CapFloor)new Floor(leg, new List<double>() { strike });
                break;
              default:
                throw new ArgumentException("unknown cap/floor type");
            }
            result.setPricingEngine(makeEngine(volatility));
            return result;
         }

      }

      bool checkAbsError(double x1, double x2, double tolerance)
      {
         return Math.Abs(x1 - x2) < tolerance;
      }

      string typeToString(CapFloorType type) 
      {
        switch (type) {
          case CapFloorType.Cap:
            return "cap";
          case CapFloorType.Floor:
            return "floor";
          case CapFloorType.Collar:
            return "collar";
          default:
            throw new ArgumentException("unknown cap/floor type");
        }
    
      }

      [TestMethod()]
      public void testVega() 
      {
         CommonVars vars = new CommonVars();

         int[] lengths = { 1, 2, 3, 4, 5, 6, 7, 10, 15, 20, 30 };
         double[] vols = { 0.01, 0.05, 0.10, 0.15, 0.20 };
         double[] strikes = { 0.01, 0.02, 0.03, 0.04, 0.05, 0.06, 0.07, 0.08, 0.09 };
         CapFloorType[] types = { CapFloorType.Cap, CapFloorType.Floor};

         Date startDate = vars.termStructure.link.referenceDate();
         double shift = 1e-8;
         double tolerance = 0.005;

         for (int i=0; i<lengths.Length; i++) 
         {
            for (int j=0; j<vols.Length; j++) 
            {
               for (int k=0; k<strikes.Length; k++) 
               {
                  for (int h=0; h<types.Length; h++) 
                  {
                     List<CashFlow> leg = vars.makeLeg(startDate, lengths[i]);
                     CapFloor capFloor = vars.makeCapFloor(types[h],leg, strikes[k],vols[j]);
                     CapFloor shiftedCapFloor2 = vars.makeCapFloor(types[h],leg,strikes[k],vols[j]+shift);
                     CapFloor shiftedCapFloor1 = vars.makeCapFloor(types[h],leg,strikes[k],vols[j]-shift);

                     double value1 = shiftedCapFloor1.NPV();
                     double value2 = shiftedCapFloor2.NPV();

                     double numericalVega = (value2 - value1) / (2*shift);

                          
                     if (numericalVega>1.0e-4) 
                     {
                              double analyticalVega = capFloor.result("vega");
                              double discrepancy = Math.Abs(numericalVega - analyticalVega);
                              discrepancy /= numericalVega;
                              if (discrepancy > tolerance)
                                  Assert.Fail(
                                      "failed to compute cap/floor vega:" +
                                      "\n   lengths:     " + new Period(lengths[j],TimeUnit.Years) +
                                      "\n   strike:      " + strikes[k] +
                                      "\n   types:       " + types[h] +
                                      "\n   calculated:  " + analyticalVega +
                                      "\n   expected:    " + numericalVega +
                                      "\n   discrepancy: " + discrepancy +
                                      "\n   tolerance:   " + tolerance);
                           
                     }
                  }
               }
            }
         }
      }
   }
}
