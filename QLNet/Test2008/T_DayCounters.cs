/*
 Copyright (C) 2008 Andrea Maggiulli
  
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

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QLNet;
using System.Diagnostics;

namespace TestSuite
{
   [TestClass()]
   public class T_DayCounters
   {
      public struct SingleCase
      {
         public SingleCase(ActualActual.Convention convention, DDate start, DDate end, DDate refStart, DDate refEnd, double result)
         {
            _convention = convention;
            _start = start;
            _end = end;
            _refStart = refStart;
            _refEnd = refEnd;
            _result = result;
         }
         public SingleCase(ActualActual.Convention convention, DDate start, DDate end, double result)
         {
            _convention = convention;
            _start = start;
            _end = end;
            _refStart = new DDate();
            _refEnd = new DDate();
            _result = result;
         }
         public ActualActual.Convention _convention;
         public DDate _start;
         public DDate _end;
         public DDate _refStart;
         public DDate _refEnd;
         public double _result;
      }
      [TestMethod()]
      public void testActualActual()
      {
         SingleCase[] testCases = 
         {
            // first example
            new SingleCase(ActualActual.Convention.ISDA,
                           new DDate(1,Month.November,2003), new DDate(1,Month.May,2004),
                           0.497724380567),
            new SingleCase(ActualActual.Convention.ISMA,
                           new DDate(1,Month.November,2003), new DDate(1,Month.May,2004),
                           new DDate(1,Month.November,2003), new DDate(1,Month.May,2004),
                           0.500000000000),
            new SingleCase(ActualActual.Convention.AFB,
                           new DDate(1,Month.November,2003), new DDate(1,Month.May,2004),
                           0.497267759563),
            // short first calculation period (first period)
            new SingleCase(ActualActual.Convention.ISDA,
                           new DDate(1,Month.February,1999), new DDate(1,Month.July,1999),
                           0.410958904110),
            new SingleCase(ActualActual.Convention.ISMA,
                   new DDate(1,Month.February,1999), new DDate(1,Month.July,1999),
                   new DDate(1,Month.July,1998), new DDate(1,Month.July,1999),
                   0.410958904110),
            new SingleCase(ActualActual.Convention.AFB,
                   new DDate(1,Month.February,1999), new DDate(1,Month.July,1999),
                   0.410958904110),
            // short first calculation period (second period)
            new SingleCase(ActualActual.Convention.ISDA,
                   new DDate(1,Month.July,1999), new DDate(1,Month.July,2000),
                   1.001377348600),
            new SingleCase(ActualActual.Convention.ISMA,
                   new DDate(1,Month.July,1999), new DDate(1,Month.July,2000),
                   new DDate(1,Month.July,1999), new DDate(1,Month.July,2000),
                   1.000000000000),
            new SingleCase(ActualActual.Convention.AFB,
                   new DDate(1,Month.July,1999), new DDate(1,Month.July,2000),
                   1.000000000000),
            // long first calculation period (first period)
            new SingleCase(ActualActual.Convention.ISDA,
                   new DDate(15,Month.August,2002), new DDate(15,Month.July,2003),
                   0.915068493151),
            new SingleCase(ActualActual.Convention.ISMA,
                   new DDate(15,Month.August,2002), new DDate(15,Month.July,2003),
                   new DDate(15,Month.January,2003), new DDate(15,Month.July,2003),
                   0.915760869565),
            new SingleCase(ActualActual.Convention.AFB,
                   new DDate(15,Month.August,2002), new DDate(15,Month.July,2003),
                   0.915068493151),
            // long first calculation period (second period)
            /* Warning: the ISDA case is in disagreement with mktc1198.pdf */
            new SingleCase(ActualActual.Convention.ISDA,
                   new DDate(15,Month.July,2003), new DDate(15,Month.January,2004),
                   0.504004790778),
            new SingleCase(ActualActual.Convention.ISMA,
                   new DDate(15,Month.July,2003), new DDate(15,Month.January,2004),
                   new DDate(15,Month.July,2003), new DDate(15,Month.January,2004),
                   0.500000000000),
            new SingleCase(ActualActual.Convention.AFB,
                   new DDate(15,Month.July,2003), new DDate(15,Month.January,2004),
                   0.504109589041),
            // short final calculation period (penultimate period)
            new SingleCase(ActualActual.Convention.ISDA,
                   new DDate(30,Month.July,1999), new DDate(30,Month.January,2000),
                   0.503892506924),
            new SingleCase(ActualActual.Convention.ISMA,
                   new DDate(30,Month.July,1999), new DDate(30,Month.January,2000),
                   new DDate(30,Month.July,1999), new DDate(30,Month.January,2000),
                   0.500000000000),
            new SingleCase(ActualActual.Convention.AFB,
                   new DDate(30,Month.July,1999), new DDate(30,Month.January,2000),
                   0.504109589041),
            // short final calculation period (final period)
            new SingleCase(ActualActual.Convention.ISDA,
                   new DDate(30,Month.January,2000), new DDate(30,Month.June,2000),
                   0.415300546448),
            new SingleCase(ActualActual.Convention.ISMA,
                   new DDate(30,Month.January,2000), new DDate(30,Month.June,2000),
                   new DDate(30,Month.January,2000), new DDate(30,Month.July,2000),
                   0.417582417582),
            new SingleCase(ActualActual.Convention.AFB,
                   new DDate(30,Month.January,2000), new DDate(30,Month.June,2000),
                   0.41530054644)
             };

         int n = testCases.Length; /// sizeof(SingleCase);
         for (int i = 0; i < n; i++)
         {
            ActualActual dayCounter = new ActualActual(testCases[i]._convention);
            DDate d1 = testCases[i]._start;
            DDate d2 = testCases[i]._end;
            DDate rd1 = testCases[i]._refStart;
            DDate rd2 = testCases[i]._refEnd;
            double calculated = dayCounter.yearFraction(d1, d2, rd1, rd2);

            if (Math.Abs(calculated - testCases[i]._result) > 1.0e-10)
            {
               Assert.Fail(dayCounter.name() + "period: " + d1 + " to " + d2 +
                           "    calculated: " + calculated + "    expected:   " + testCases[i]._result); 
            }
         }
      }

      [TestMethod()]
      public void testSimple()
      {
         Period[] p = { new Period(3, TimeUnit.Months), new Period(6, TimeUnit.Months), new Period(1, TimeUnit.Years) };
         double[] expected = { 0.25, 0.5, 1.0 };
         int n = p.Length;

         // 4 years should be enough
         DDate first= new DDate(1,Month.January,2002), last = new DDate(31,Month.December,2005);
         DayCounter dayCounter = new SimpleDayCounter();

          for (DDate start = first; start <= last; start++) 
          {
              for (int i=0; i<n; i++) 
              {
                  DDate end = start + p[i];
                  double calculated = dayCounter.yearFraction(start,end,null ,null );
                  if (Math.Abs(calculated-expected[i]) > 1.0e-12) 
                  {
                      Assert.Fail ("from " + start + " to " + end +
                                   "Calculated: " + calculated +
                                   "Expected:   " + expected[i]);
                  }
              }
          }

      }
      [TestMethod()]
      public void testOne()
      {
          Period[] p = { new Period(3,TimeUnit.Months), new Period(6,TimeUnit.Months), new Period(1,TimeUnit.Years) };
          double[] expected = { 1.0, 1.0, 1.0 };
          int n = p.Length;

          // 1 years should be enough
          DDate first = new DDate(1,Month.January,2004), last= new DDate (31,Month.December,2004);
          DayCounter dayCounter = new OneDayCounter();

          for (DDate start = first; start <= last; start++) 
          {
              for (int i=0; i<n; i++) 
              {
                  DDate end = start + p[i];
                  double calculated = dayCounter.yearFraction(start,end,null,null);
                  if (Math.Abs(calculated-expected[i]) > 1.0e-12) 
                  {
                      Assert.Fail("from " + start + " to " + end +
                                  "Calculated: " + calculated +
                                  "Expected:   " + expected[i]);
                  }
              }
          }

      }

   }
}
