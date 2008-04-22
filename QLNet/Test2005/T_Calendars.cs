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
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QLNet;
using System.Diagnostics;

namespace TestSuite {
   [TestClass()]
   public class T_Calendars {
      [TestMethod()]
      public void testModifiedCalendars() {
         Calendar c1 = new TARGET();
         Calendar c2 = new UnitedStates(UnitedStates.Market.NYSE);
         Date d1 = new Date(1,Month.May,2004);      // holiday for both calendars
         Date d2 = new Date(26,Month.April,2004);   // business day

         Assert.IsTrue (c1.isHoliday(d1),"wrong assumption---correct the test");
         Assert.IsTrue (c1.isBusinessDay(d2), "wrong assumption---correct the test");

         Assert.IsTrue(c2.isHoliday(d1), "wrong assumption---correct the test");
         Assert.IsTrue(c2.isBusinessDay(d2), "wrong assumption---correct the test");

         // modify the TARGET calendar
         c1.removeHoliday(d1);
         c1.addHoliday(d2);

         // test
         Assert.IsFalse(c1.isHoliday(d1), d1 + " still a holiday for original TARGET instance");
         Assert.IsFalse(c1.isBusinessDay(d2), d2 + " still a business day for original TARGET instance");

         // any instance of TARGET should be modified...
         Calendar c3 = new TARGET();
         Assert.IsFalse(c3.isHoliday(d1),d1 + " still a holiday for generic TARGET instance");
         Assert.IsFalse(c3.isBusinessDay(d2),d2 + " still a business day for generic TARGET instance");

         // ...but not other calendars
         Assert.IsFalse(c2.isBusinessDay(d1),d1 + " business day for New York");
         Assert.IsFalse(c2.isHoliday(d2),d2 + " holiday for New York");

         // restore original holiday set---test the other way around
         c3.addHoliday(d1);
         c3.removeHoliday(d2);

         Assert.IsFalse(c1.isBusinessDay(d1),d1 + " still a business day");
         Assert.IsFalse(c1.isHoliday(d2),d2 + " still a holiday");
      }

      [TestMethod()]
      public void testUSSettlement() 
      {
         Debug.Print ("Testing US settlement holiday list...");
         List<Date>  expectedHol = new List<Date> ();

         expectedHol.Add(new Date(1, Month.January, 2004));
         expectedHol.Add(new Date(19, Month.January, 2004));
         expectedHol.Add(new Date(16, Month.February, 2004));
         expectedHol.Add(new Date(31, Month.May, 2004));
         expectedHol.Add(new Date(5, Month.July, 2004));
         expectedHol.Add(new Date(6, Month.September, 2004));
         expectedHol.Add(new Date(11, Month.October, 2004));
         expectedHol.Add(new Date(11, Month.November, 2004));
         expectedHol.Add(new Date(25, Month.November, 2004));
         expectedHol.Add(new Date(24, Month.December, 2004));

         expectedHol.Add(new Date(31, Month.December, 2004));
         expectedHol.Add(new Date(17, Month.January, 2005));
         expectedHol.Add(new Date(21, Month.February, 2005));
         expectedHol.Add(new Date(30, Month.May, 2005));
         expectedHol.Add(new Date(4, Month.July, 2005));
         expectedHol.Add(new Date(5, Month.September, 2005));
         expectedHol.Add(new Date(10, Month.October, 2005));
         expectedHol.Add(new Date(11, Month.November, 2005));
         expectedHol.Add(new Date(24, Month.November, 2005));
         expectedHol.Add(new Date(26, Month.December, 2005));

         Calendar c = new UnitedStates(UnitedStates.Market.Settlement);
         List<Date> hol = Calendar.holidayList(c, new Date(1, Month.January, 2004),
                                                  new Date(31, Month.December, 2005));

         for (int i=0; i< Math.Min(hol.Count, expectedHol.Count); i++) 
         {
            if (hol[i]!=expectedHol[i])
            Assert.Fail ("expected holiday was " + expectedHol[i] + " while calculated holiday is " + hol[i]);
         }
         
         if (hol.Count!= expectedHol.Count)
            Assert.Fail ("there were " + expectedHol.Count +
                         " expected holidays, while there are " + hol.Count +
                         " calculated holidays");
      }

      [TestMethod()]
      public void testUSGovernmentBondMarket() 
      {

         List<Date> expectedHol = new List<Date> ();
         expectedHol.Add(new Date(1,Month.January,2004));
         expectedHol.Add(new Date(19,Month.January,2004));
         expectedHol.Add(new Date(16,Month.February,2004));
         expectedHol.Add(new Date(9,Month.April,2004));
         expectedHol.Add(new Date(31,Month.May,2004));
         expectedHol.Add(new Date(5,Month.July,2004));
         expectedHol.Add(new Date(6,Month.September,2004));
         expectedHol.Add(new Date(11,Month.October,2004));
         expectedHol.Add(new Date(11,Month.November,2004));
         expectedHol.Add(new Date(25,Month.November,2004));
         expectedHol.Add(new Date(24,Month.December,2004));

         Calendar c = new UnitedStates(UnitedStates.Market.GovernmentBond);
         List<Date> hol = Calendar.holidayList(c, new Date(1,Month.January,2004),new Date(31,Month.December,2004));

         for (int i=0; i<Math.Min(hol.Count, expectedHol.Count); i++) 
         {
            if (hol[i]!=expectedHol[i])
            Assert.Fail("expected holiday was " + expectedHol[i] + " while calculated holiday is " + hol[i]);
         }
         if (hol.Count!=expectedHol.Count)
            Assert.Fail("there were " + expectedHol.Count +
                        " expected holidays, while there are " + hol.Count +
                        " calculated holidays");
      }

      [TestMethod()]
      public void testUSNewYorkStockExchange() 
      {

         List<Date> expectedHol = new List<Date> ();
         expectedHol.Add(new Date(1, Month.January, 2004));
         expectedHol.Add(new Date(19, Month.January, 2004));
         expectedHol.Add(new Date(16, Month.February, 2004));
         expectedHol.Add(new Date(9, Month.April, 2004));
         expectedHol.Add(new Date(31, Month.May, 2004));
         expectedHol.Add(new Date(11, Month.June, 2004));
         expectedHol.Add(new Date(5, Month.July, 2004));
         expectedHol.Add(new Date(6, Month.September, 2004));
         expectedHol.Add(new Date(25, Month.November, 2004));
         expectedHol.Add(new Date(24, Month.December, 2004));

         expectedHol.Add(new Date(17, Month.January, 2005));
         expectedHol.Add(new Date(21, Month.February, 2005));
         expectedHol.Add(new Date(25, Month.March, 2005));
         expectedHol.Add(new Date(30, Month.May, 2005));
         expectedHol.Add(new Date(4, Month.July, 2005));
         expectedHol.Add(new Date(5, Month.September, 2005));
         expectedHol.Add(new Date(24, Month.November, 2005));
         expectedHol.Add(new Date(26, Month.December, 2005));

         expectedHol.Add(new Date(2, Month.January, 2006));
         expectedHol.Add(new Date(16, Month.January, 2006));
         expectedHol.Add(new Date(20, Month.February, 2006));
         expectedHol.Add(new Date(14, Month.April, 2006));
         expectedHol.Add(new Date(29, Month.May, 2006));
         expectedHol.Add(new Date(4, Month.July, 2006));
         expectedHol.Add(new Date(4, Month.September, 2006));
         expectedHol.Add(new Date(23, Month.November, 2006));
         expectedHol.Add(new Date(25, Month.December, 2006));

         Calendar c = new UnitedStates(UnitedStates.Market.NYSE);
         List<Date> hol = Calendar.holidayList(c, new Date(1,Month.January,2004),new Date(31,Month.December,2006));

         int i;
         for (i = 0; i < Math.Min(hol.Count, expectedHol.Count); i++) 
         {
            if (hol[i]!=expectedHol[i])
            Assert.Fail ("expected holiday was " + expectedHol[i] + " while calculated holiday is " + hol[i]);
         }
         if (hol.Count != expectedHol.Count)
            Assert.Fail("there were " + expectedHol.Count +
                        " expected holidays, while there are " + hol.Count +
                        " calculated holidays");

         List<Date> histClose = new List<Date>();
         histClose.Add(new Date(11, Month.June, 2004));     // Reagan's funeral
         histClose.Add(new Date(14, Month.September, 2001));// September 11, 2001
         histClose.Add(new Date(13, Month.September, 2001));// September 11, 2001
         histClose.Add(new Date(12, Month.September, 2001));// September 11, 2001
         histClose.Add(new Date(11, Month.September, 2001));// September 11, 2001
         histClose.Add(new Date(14, Month.July, 1977));     // 1977 Blackout
         histClose.Add(new Date(25, Month.January, 1973));  // Johnson's funeral.
         histClose.Add(new Date(28, Month.December, 1972)); // Truman's funeral
         histClose.Add(new Date(21, Month.July, 1969));     // Lunar exploration nat. day
         histClose.Add(new Date(31, Month.March, 1969));    // Eisenhower's funeral
         histClose.Add(new Date(10, Month.February, 1969)); // heavy snow
         histClose.Add(new Date(5, Month.July, 1968));      // Day after Independence Day
         // June 12-Dec. 31, 1968
         // Four day week (closed on Wednesdays) - Paperwork Crisis
         histClose.Add(new Date(12, Month.Jun, 1968));
         histClose.Add(new Date(19, Month.Jun, 1968));
         histClose.Add(new Date(26, Month.Jun, 1968));
         histClose.Add(new Date(3, Month.Jul, 1968));
         histClose.Add(new Date(10, Month.Jul, 1968));
         histClose.Add(new Date(17, Month.Jul, 1968));
         histClose.Add(new Date(20, Month.Nov, 1968));
         histClose.Add(new Date(27, Month.Nov, 1968));
         histClose.Add(new Date(4, Month.Dec, 1968));
         histClose.Add(new Date(11, Month.Dec, 1968));
         histClose.Add(new Date(18, Month.Dec, 1968));
         // Presidential election days
         histClose.Add(new Date(4, Month.Nov, 1980));
         histClose.Add(new Date(2, Month.Nov, 1976));
         histClose.Add(new Date(7, Month.Nov, 1972));
         histClose.Add(new Date(5, Month.Nov, 1968));
         histClose.Add(new Date(3, Month.Nov, 1964));
    
         for (i=0; i<histClose.Count; i++) 
         {
            if (!c.isHoliday(histClose[i]))
            Assert.Fail(histClose[i] + " should be holiday (historical close)");
         }
         
      }

   }
}
