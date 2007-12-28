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
   public class TARGET : Calendar 
   {
      private static Calendar.Impl impl ; //= new TARGET.Impl() ;

      private new class Impl : WesternImpl
      {
         public override string name() { return "TARGET"; }
         public override bool isBusinessDay(DDate date)
         {
            Weekday w = date.weekday();
            int d = date.dayOfMonth(), dd = date.dayOfYear();
            Month m = date.month();
            int y = date.year();
            int em = easterMonday(y);
            if (isWeekend(w)
               // New Year's Day
                || (d == 1 && m == Month.January)
               // Good Friday
                || (dd == em - 3 && y >= 2000)
               // Easter Monday
                || (dd == em && y >= 2000)
               // Labour Day
                || (d == 1 && m == Month.May && y >= 2000)
               // Christmas
                || (d == 25 && m == Month.December)
               // Day of Goodwill
                || (d == 26 && m == Month.December && y >= 2000)
               // December 31st, 1998, 1999, and 2001 only
                || (d == 31 && m == Month.December &&
                    (y == 1998 || y == 1999 || y == 2001)))
               return false;
            return true;
         }
      }

      public TARGET()
      {
        // all calendar instances share the same implementation instance
        if ( impl == null  ) impl = new TARGET.Impl();
        _impl = impl;

      }
   }
}
