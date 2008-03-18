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

    //! Swiss calendar
    /*! Holidays:
        <ul>
        <li>Saturdays</li>
        <li>Sundays</li>
        <li>New Year's Day, January 1st</li>
        <li>Berchtoldstag, January 2nd</li>
        <li>Good Friday</li>
        <li>Easter Monday</li>
        <li>Ascension Day</li>
        <li>Whit Monday</li>
        <li>Labour Day, May 1st</li>
        <li>National Day, August 1st</li>
        <li>Christmas, December 25th</li>
        <li>St. Stephen's Day, December 26th</li>
        </ul>

        \ingroup calendars
    */
    public class Switzerland :  Calendar {
      private class Impl : Calendar.WesternImpl {
         
            public override string name() { return "Switzerland"; }
            public override bool isBusinessDay(DDate date) {
                 Weekday w = date.weekday();
        int d = date.dayOfMonth(), dd = date.dayOfYear();
        Month m = date.month();
        int y = date.year();
        int em = easterMonday(y);
        if (isWeekend(w)
            // New Year's Day
            || (d == 1  && m == Month.January)
            // Berchtoldstag
            || (d == 2  && m == Month.January)
            // Good Friday
            || (dd == em-3)
            // Easter Monday
            || (dd == em)
            // Ascension Day
            || (dd == em+38)
            // Whit Monday
            || (dd == em+49)
            // Labour Day
            || (d == 1  && m == Month.May)
            // National Day
            || (d == 1  && m == Month.August)
            // Christmas
            || (d == 25 && m == Month.December)
            // St. Stephen's Day
            || (d == 26 && m == Month.December))
            return false;
        return true;
    }
        };
      private static Calendar.Impl impl = new Switzerland.Impl();
        
      public Switzerland() {
        // all calendar instances share the same implementation instance
        _impl = impl;
    }

      
      };

}


