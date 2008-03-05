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

namespace QLNet
{
   public class Calendar
   {
      //! abstract base class for calendar implementations
      protected abstract class Impl
      {
         //virtual ~Impl() {}
         public virtual string name() { throw new System.NotImplementedException(); }
         public virtual bool isBusinessDay(DDate d) { throw new System.NotImplementedException(); }
         public virtual bool isWeekend(Weekday w) { throw new System.NotImplementedException(); }
         public  List<DDate> addedHolidays = new List<DDate>()  , removedHolidays = new List<DDate> ();
      };
      protected Impl _impl;

      public bool empty()
      {
         return _impl == null;
      }
      public string name()
      {
         return _impl.name();
      }
      public bool isBusinessDay(DDate d)
      {
         if (_impl.addedHolidays.Contains(d) )
             return false;
         if (_impl.removedHolidays.Contains(d) )
             return true;
         return _impl.isBusinessDay(d);
      }
      public bool isEndOfMonth(DDate d)
      {
         return (d.month() != adjust(d + 1).month());
      }
      public DDate endOfMonth(DDate d)
      {
         return adjust(DDate.endOfMonth(d), BusinessDayConvention.Preceding);
      }
      public bool isHoliday(DDate d)
      {
         return !isBusinessDay(d);
      }
      public bool isWeekend(Weekday w)
      {
         return _impl.isWeekend(w);
      }


      public void addHoliday(DDate d)
      {
         // if d was a genuine holiday previously removed, revert the change
         _impl.removedHolidays.Remove(d);
         // if it's already a holiday, leave the calendar alone.
         // Otherwise, add it.
         if (_impl.isBusinessDay(d))
            _impl.addedHolidays.Add(d);
      }
      public void removeHoliday(DDate d)
      {
         // if d was an artificially-added holiday, revert the change
         _impl.addedHolidays.Remove(d);
         // if it's already a business day, leave the calendar alone.
         // Otherwise, add it.
         if (!_impl.isBusinessDay(d))
            _impl.removedHolidays.Add(d);
      }
      public DDate adjust(DDate d)
      {
         return adjust(d, BusinessDayConvention.Following);
      }
      public DDate adjust(DDate d, BusinessDayConvention c)
      {
         if (d == new DDate())
         {
            throw new Exception("null date");
         }

         if (c == BusinessDayConvention.Unadjusted)
            return d;

         DDate d1 = d;
         if (c == BusinessDayConvention.Following || c == BusinessDayConvention.ModifiedFollowing)
         {
            while (isHoliday(d1))
               d1++;
            if (c == BusinessDayConvention.ModifiedFollowing)
            {
               if (d1.month() != d.month())
               {
                  return adjust(d, BusinessDayConvention.Preceding);
               }
            }
         }
         else if (c == BusinessDayConvention.Preceding || c == BusinessDayConvention.ModifiedPreceding)
         {
            while (isHoliday(d1))
               d1--;
            if (c == BusinessDayConvention.ModifiedPreceding && d1.month() != d.month())
            {
               return adjust(d, BusinessDayConvention.Following);
            }
         }
         else
         {
            throw new Exception("unknown business-day convention");
         }
         return d1;
      }
      public DDate advance(DDate d, int n, TimeUnit unit)
      {
         return advance(d, n, unit, BusinessDayConvention.Following, false);
      }

      public DDate advance(DDate d, int n, TimeUnit unit, BusinessDayConvention c, bool endOfMonth)
      {
         if (d == new DDate())
         {
            throw new Exception("null date");
         }

         if (n == 0)
         {
            return adjust(d, c);
         }
         else if (unit == TimeUnit.Days)
         {
            DDate d1 = d;
            if (n > 0)
            {
               while (n > 0)
               {
                  d1++;
                  while (isHoliday(d1))
                     d1++;
                  n--;
               }
            }
            else
            {
               while (n < 0)
               {
                  d1--;
                  while (isHoliday(d1))
                     d1--;
                  n++;
               }
            }
            return d1;
         }
         else if (unit == TimeUnit.Weeks)
         {
            DDate d1 = d + new Period(n, unit);
            return adjust(d1, c);
         }
         else
         {
            DDate d1 = d + new Period(n, unit);

            // we are sure the unit is Months or Years
            if (endOfMonth && isEndOfMonth(d))
            {
               return DDate.endOfMonth(d1);
            }

            return adjust(d1, c);
         }
      }

      public int businessDaysBetween(DDate from, DDate to)
      {
         return businessDaysBetween(from, to, true, false);
      }

      public int businessDaysBetween(DDate from, DDate to, bool includeFirst, bool includeLast)
      {
         int wd = 0;
         if (from != to)
         {
            if (from < to)
            {
               // the last one is treated separately to avoid
               // incrementing Date::maxDate()
               for (DDate d = from; d < to; ++d) 
               {
                  if (isBusinessDay(d))
                     ++wd;
               }
               if (isBusinessDay(to))
                  ++wd;

            }
            else if (from > to)
            {
               for (DDate d = to; d < from; ++d) 
               {
                  if (isBusinessDay(d))
                     ++wd;
               }
               if (isBusinessDay(from))
                  ++wd;
            }

            if (isBusinessDay(from) && !includeFirst)
               wd--;
            if (isBusinessDay(to) && !includeLast)
               wd--;

            if (from > to)
               wd = -wd;
         }

         return wd;
      }

      //! partial calendar implementation
      /*! This class provides the means of determining the Easter
         Monday for a given year, as well as specifying Saturdays
         and Sundays as weekend days.
      */
      protected class WesternImpl : Impl
      {
         public override bool isWeekend(Weekday w)
         {
            return w == Weekday.Saturday || w == Weekday.Sunday;
         }

         //! expressed relative to first day of year
         protected static int easterMonday(int y)
         {
            int[] EasterMonday = {
                  98,  90, 103,  95, 114, 106,  91, 111, 102,   // 1901-1909
             87, 107,  99,  83, 103,  95, 115,  99,  91, 111,   // 1910-1919
             96,  87, 107,  92, 112, 103,  95, 108, 100,  91,   // 1920-1929
            111,  96,  88, 107,  92, 112, 104,  88, 108, 100,   // 1930-1939
             85, 104,  96, 116, 101,  92, 112,  97,  89, 108,   // 1940-1949
            100,  85, 105,  96, 109, 101,  93, 112,  97,  89,   // 1950-1959
            109,  93, 113, 105,  90, 109, 101,  86, 106,  97,   // 1960-1969
             89, 102,  94, 113, 105,  90, 110, 101,  86, 106,   // 1970-1979
             98, 110, 102,  94, 114,  98,  90, 110,  95,  86,   // 1980-1989
            106,  91, 111, 102,  94, 107,  99,  90, 103,  95,   // 1990-1999
            115, 106,  91, 111, 103,  87, 107,  99,  84, 103,   // 2000-2009
             95, 115, 100,  91, 111,  96,  88, 107,  92, 112,   // 2010-2019
            104,  95, 108, 100,  92, 111,  96,  88, 108,  92,   // 2020-2029
            112, 104,  89, 108, 100,  85, 105,  96, 116, 101,   // 2030-2039
             93, 112,  97,  89, 109, 100,  85, 105,  97, 109,   // 2040-2049
            101,  93, 113,  97,  89, 109,  94, 113, 105,  90,   // 2050-2059
            110, 101,  86, 106,  98,  89, 102,  94, 114, 105,   // 2060-2069
             90, 110, 102,  86, 106,  98, 111, 102,  94, 114,   // 2070-2079
             99,  90, 110,  95,  87, 106,  91, 111, 103,  94,   // 2080-2089
            107,  99,  91, 103,  95, 115, 107,  91, 111, 103,   // 2090-2099
             88, 108, 100,  85, 105,  96, 109, 101,  93, 112,   // 2100-2109
             97,  89, 109,  93, 113, 105,  90, 109, 101,  86,   // 2110-2119
            106,  97,  89, 102,  94, 113, 105,  90, 110, 101,   // 2120-2129
             86, 106,  98, 110, 102,  94, 114,  98,  90, 110,   // 2130-2139
             95,  86, 106,  91, 111, 102,  94, 107,  99,  90,   // 2140-2149
            103,  95, 115, 106,  91, 111, 103,  87, 107,  99,   // 2150-2159
             84, 103,  95, 115, 100,  91, 111,  96,  88, 107,   // 2160-2169
             92, 112, 104,  95, 108, 100,  92, 111,  96,  88,   // 2170-2179
            108,  92, 112, 104,  89, 108, 100,  85, 105,  96,   // 2180-2189
            116, 101,  93, 112,  97,  89, 109, 100,  85, 105    // 2190-2199
            };
            return EasterMonday[y - 1901];
         }
      }

      public static List<DDate> holidayList(Calendar calendar, DDate from, DDate to)
      {
         return holidayList(calendar, from, to, false);
      }
         
      public static List<DDate> holidayList(Calendar calendar,DDate from, DDate to, bool includeWeekEnds) 
      {

         if (to <= from)
         {
            throw new Exception ("'from' date (" + from + ") must be earlier than 'to' date (" + to + ")");
         }

         List<DDate> result = new List<DDate>();
        
         for (DDate d = from; d <= to; ++d) 
         {
            if (calendar.isHoliday(d)
                && (includeWeekEnds || !calendar.isWeekend(d.weekday())))
                result.Add(d);
       }
       return result;
    }

   }
}
