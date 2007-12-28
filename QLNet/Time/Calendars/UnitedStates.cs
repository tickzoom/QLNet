/*
 This file is part of QLNet Project http://trac2.assembla.com/QLNet
 
 QLNet is a porting of QuantLib, a free-software/open-source library
 for financial quantitative analysts and developers - http://quantlib.org/
 The license is available online at http://quantlib.org/license.shtml.
 
 This program is distributed in the hope that it will be useful, but WITHOUT
 ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 FOR A PARTICULAR PURPOSE.  See the license for more details.
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace QLNet
{
   public class UnitedStates : Calendar 
   {
      private class SettlementImpl : Calendar.WesternImpl 
       {
          
          public override string name() { return "US settlement"; }
          public override bool isBusinessDay(DDate date)
          {
             Weekday w = date.weekday();
             int d = date.dayOfMonth();
             Month m = date.month();
             if (isWeekend(w)
                // New Year's Day (possibly moved to Monday if on Sunday)
                 || ((d == 1 || (d == 2 && w == Weekday.Monday)) && m == Month.January)
                // (or to Friday if on Saturday)
                 || (d == 31 && w == Weekday.Friday && m == Month.December)
                // Martin Luther King's birthday (third Monday in January)
                 || ((d >= 15 && d <= 21) && w == Weekday.Monday && m == Month.January)
                // Washington's birthday (third Monday in February)
                 || ((d >= 15 && d <= 21) && w == Weekday.Monday && m == Month.February)
                // Memorial Day (last Monday in May)
                 || (d >= 25 && w == Weekday.Monday && m == Month.May)
                // Independence Day (Monday if Sunday or Friday if Saturday)
                 || ((d == 4 || (d == 5 && w == Weekday.Monday) ||
                      (d == 3 && w == Weekday.Friday)) && m == Month.July)
                // Labor Day (first Monday in September)
                 || (d <= 7 && w == Weekday.Monday && m == Month.September)
                // Columbus Day (second Monday in October)
                 || ((d >= 8 && d <= 14) && w == Weekday.Monday && m == Month.October)
                // Veteran's Day (Monday if Sunday or Friday if Saturday)
                 || ((d == 11 || (d == 12 && w == Weekday.Monday) ||
                      (d == 10 && w == Weekday.Friday)) && m == Month.November)
                // Thanksgiving Day (fourth Thursday in November)
                 || ((d >= 22 && d <= 28) && w == Weekday.Thursday && m == Month.November)
                // Christmas (Monday if Sunday or Friday if Saturday)
                 || ((d == 25 || (d == 26 && w == Weekday.Monday) ||
                      (d == 24 && w == Weekday.Friday)) && m == Month.December))
                return false;
             return true;

          }
        };
      private class NyseImpl : Calendar.WesternImpl 
      {
         public override string name() { return "New York stock exchange"; }
         public override bool isBusinessDay(DDate date)
        {
           Weekday w = date.weekday();
           int d = date.dayOfMonth(), dd = date.dayOfYear();
           Month m = date.month();
           int y = date.year();
           int em = easterMonday(y);
           if (isWeekend(w)
              // New Year's Day (possibly moved to Monday if on Sunday)
               || ((d == 1 || (d == 2 && w == Weekday.Monday)) && m == Month.January)
              // Washington's birthday (third Monday in February)
               || ((d >= 15 && d <= 21) && w == Weekday.Monday && m == Month.February)
              // Good Friday
               || (dd == em - 3)
              // Memorial Day (last Monday in May)
               || (d >= 25 && w == Weekday.Monday && m == Month.May)
              // Independence Day (Monday if Sunday or Friday if Saturday)
               || ((d == 4 || (d == 5 && w == Weekday.Monday) ||
                    (d == 3 && w == Weekday.Friday)) && m == Month.July)
              // Labor Day (first Monday in September)
               || (d <= 7 && w == Weekday.Monday && m == Month.September)
              // Thanksgiving Day (fourth Thursday in November)
               || ((d >= 22 && d <= 28) && w == Weekday.Thursday && m == Month.November)
              // Christmas (Monday if Sunday or Friday if Saturday)
               || ((d == 25 || (d == 26 && w == Weekday.Monday) ||
                    (d == 24 && w == Weekday.Friday)) && m == Month.December)
               ) return false;

           if (y >= 1998)
           {
              if (// Martin Luther King's birthday (third Monday in January)
                  ((d >= 15 && d <= 21) && w == Weekday.Monday && m == Month.January)
                 // Reagan's funeral
                  || (y == 2004 && m == Month.June && d == 11)
                 // September 11, 2001
                  || (y == 2001 && m == Month.September && (11 <= d && d <= 14))
                  ) return false;
           }
           else if (y <= 1980)
           {
              if (// Presidential election days
                  ((y % 4 == 0) && m == Month.November && d <= 7 && w == Weekday.Tuesday)
                 // 1977 Blackout
                  || (y == 1977 && m == Month.July && d == 14)
                 // Funeral of former President Lyndon B. Johnson.
                  || (y == 1973 && m == Month.January && d == 25)
                 // Funeral of former President Harry S. Truman
                  || (y == 1972 && m == Month.December && d == 28)
                 // National Day of Participation for the lunar exploration.
                  || (y == 1969 && m == Month.July && d == 21)
                 // Funeral of former President Eisenhower.
                  || (y == 1969 && m == Month.March && d == 31)
                 // Closed all day - heavy snow.
                  || (y == 1969 && m == Month.February && d == 10)
                 // Day after Independence Day.
                  || (y == 1968 && m == Month.July && d == 5)
                 // June 12-Dec. 31, 1968
                 // Four day week (closed on Wednesdays) - Paperwork Crisis
                  || (y == 1968 && dd >= 163 && w == Weekday.Wednesday)
                  ) return false;
           }
           else
           {
              if (// Nixon's funeral
                  (y == 1994 && m == Month.April && d == 27)
                  ) return false;
           }


           return true;

        }
      };
      private class GovernmentBondImpl : Calendar.WesternImpl 
      {
          
         public override string name() { return "US government bond market"; }
         public override bool isBusinessDay(DDate date)
         {
            Weekday w = date.weekday();
            int d = date.dayOfMonth(), dd = date.dayOfYear();
            Month m = date.month();
            int y = date.year();
            int em = easterMonday(y);
            if (isWeekend(w)
               // New Year's Day (possibly moved to Monday if on Sunday)
                || ((d == 1 || (d == 2 && w == Weekday.Monday)) && m == Month.January)
               // Martin Luther King's birthday (third Monday in January)
                || ((d >= 15 && d <= 21) && w == Weekday.Monday && m == Month.January)
               // Washington's birthday (third Monday in February)
                || ((d >= 15 && d <= 21) && w == Weekday.Monday && m == Month.February)
               // Good Friday
                || (dd == em - 3)
               // Memorial Day (last Monday in May)
                || (d >= 25 && w == Weekday.Monday && m == Month.May)
               // Independence Day (Monday if Sunday or Friday if Saturday)
                || ((d == 4 || (d == 5 && w == Weekday.Monday) ||
                     (d == 3 && w == Weekday.Friday)) && m == Month.July)
               // Labor Day (first Monday in September)
                || (d <= 7 && w == Weekday.Monday && m == Month.September)
               // Columbus Day (second Monday in October)
                || ((d >= 8 && d <= 14) && w == Weekday.Monday && m == Month.October)
               // Veteran's Day (Monday if Sunday or Friday if Saturday)
                || ((d == 11 || (d == 12 && w == Weekday.Monday) ||
                     (d == 10 && w == Weekday.Friday)) && m == Month.November)
               // Thanksgiving Day (fourth Thursday in November)
                || ((d >= 22 && d <= 28) && w == Weekday.Thursday && m == Month.November)
               // Christmas (Monday if Sunday or Friday if Saturday)
                || ((d == 25 || (d == 26 && w == Weekday.Monday) ||
                     (d == 24 && w == Weekday.Friday)) && m == Month.December))
               return false;
            return true;

         }
      };
      private class NercImpl : Calendar.WesternImpl 
      {
         public override string name() {return "North American Energy Reliability Council";}
         public override bool isBusinessDay(DDate date)
         {
            Weekday w = date.weekday();
            int d = date.dayOfMonth();
            Month m = date.month();
            if (isWeekend(w)
               // New Year's Day (possibly moved to Monday if on Sunday)
                || ((d == 1 || (d == 2 && w == Weekday.Monday)) && m == Month.January)
               // Memorial Day (last Monday in May)
                || (d >= 25 && w == Weekday.Monday && m == Month.May)
               // Independence Day (Monday if Sunday)
                || ((d == 4 || (d == 5 && w == Weekday.Monday)) && m == Month.July)
               // Labor Day (first Monday in September)
                || (d <= 7 && w == Weekday.Monday && m == Month.September)
               // Thanksgiving Day (fourth Thursday in November)
                || ((d >= 22 && d <= 28) && w == Weekday.Thursday && m == Month.November)
               // Christmas (Monday if Sunday)
                || ((d == 25 || (d == 26 && w == Weekday.Monday)) && m == Month.December))
               return false;
            return true;

         }
     };
  
      public enum Market 
      { 
         Settlement,     //!< generic settlement calendar
         NYSE,           //!< New York stock exchange calendar
         GovernmentBond, //!< government-bond calendar
         NERC            //!< off-peak days for NERC
      };

      private static Calendar.Impl settlementImpl = new UnitedStates.SettlementImpl();
      private static Calendar.Impl nyseImpl = new UnitedStates.NyseImpl();
      private static Calendar.Impl governmentImpl = new UnitedStates.GovernmentBondImpl();
      private static Calendar.Impl nercImpl = new UnitedStates.NercImpl();

      public UnitedStates()
      {
         new UnitedStates(Market.Settlement);  
      }

      public UnitedStates(Market market)
      {
        // all calendar instances on the same market share the same
        // implementation instance
        switch (market) {
           case Market.Settlement:
            _impl = settlementImpl;
            break;
         case Market.NYSE:
            _impl = nyseImpl;
            break;
         case Market.GovernmentBond:
            _impl = governmentImpl;
            break;
         case Market.NERC:
            _impl = nercImpl;
            break;
          default:
            throw new Exception("unknown market");
        }

      }
   }
}
