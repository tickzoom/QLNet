/*
 Copyright (C) 2008 Alessandro Duci

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

    //! Chinese calendar
    /*! Holidays:
        <ul>
        <li>Saturdays</li>
        <li>Sundays</li>
        <li>New Year's day, January 1st (possibly followed by one or
            two more holidays)</li>
        <li>Labour Day, first week in May</li>
        <li>National Day, one week from October 1st</li>
        </ul>

        Other holidays for which no rule is given:
        <ul>
        <li>Chinese New Year (data available for 2004-2007 only)</li>
        </ul>

        Data from <http://www.sse.com.cn/sseportal/en_us/ps/home.shtml>

        \ingroup calendars
    */
    public class China : Calendar {
      private class SseImpl : Calendar.Impl {

            public override string name() { return "Shanghai stock exchange"; }
            public override bool isWeekend(Weekday w){
                return w == Weekday.Saturday || w == Weekday.Sunday;
        }
            public override bool isBusinessDay(DDate date) {
                   Weekday w = date.weekday();
        int d = date.dayOfMonth();
        Month m = date.month();
        int y = date.year();

        if (isWeekend(w)
            // New Year's Day
            || (d == 1 && m == Month.January)
            || (d == 3 && m ==Month.January && y == 2005)
            || ((d == 2 || d == 3) && m == Month.January && y == 2006)
            || (d <= 3 && m == Month.January && y == 2007)
            // Labor Day
            || (d >= 1 && d <= 7 && m == Month.May)
            // National Day
            || (d >= 1 && d <= 7 && m == Month.October)
            // Chinese New Year
            || (d >= 19 && d <= 28 && m == Month.January && y == 2004)
            || (d >=  7 && d <= 15 && m == Month.February && y == 2005)
            || (((d >= 26 && m == Month.January) || (d <= 3 && m == Month.February))
                && y == 2006)
            || (d >= 17 && d <= 25 && m == Month.February && y == 2007)
            )
            return false;
        return true;
    }
        };
    private static Calendar.Impl sseImpl= new China.SseImpl();
      public enum Market { SSE    //!< Shanghai stock exchange
        };
      public China(){
        new China(Market.SSE);
      }
            
            
      public China(Market m) {
        // all calendar instances share the same implementation instance
        
        switch (m) {
            case Market.SSE:
            _impl = sseImpl;
            break;
          default:
            throw new Exception("unknown market");
        }
    }
    };

}

