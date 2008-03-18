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

    //! South-African calendar
    /*! Holidays:
        <ul>
        <li>Saturdays</li>
        <li>Sundays</li>
        <li>New Year's Day, January 1st (possibly moved to Monday)</li>
        <li>Good Friday</li>
        <li>Family Day, Easter Monday</li>
        <li>Human Rights Day, March 21st (possibly moved to Monday)</li>
        <li>Freedom Day, April 27th (possibly moved to Monday)</li>
        <li>Workers Day, May 1st (possibly moved to Monday)</li>
        <li>Youth Day, June 16th (possibly moved to Monday)</li>
        <li>National Women's Day, August 9th
        (possibly moved to Monday)</li>
        <li>Heritage Day, September 24th (possibly moved to Monday)</li>
        <li>Day of Reconciliation, December 16th
        (possibly moved to Monday)</li>
        <li>Christmas December 25th </li>
        <li>Day of Goodwill December 26th (possibly moved to Monday)</li>
        </ul>

        \ingroup calendars
    */
    public class SouthAfrica :  Calendar {
      private class Impl :  Calendar.WesternImpl {
            public override string name() { return "South Africa"; }
            public override bool isBusinessDay(DDate date) {
                Weekday w = date.weekday();
        int d = date.dayOfMonth(), dd = date.dayOfYear();
        Month m = date.month();
        int y = date.year();
        int em = easterMonday(y);
        if (isWeekend(w)
            // New Year's Day (possibly moved to Monday)
            || ((d == 1 || (d == 2 && w == Weekday.Monday)) && m == Month.January)
            // Good Friday
            || (dd == em-3)
            // Family Day
            || (dd == em)
            // Human Rights Day, March 21st (possibly moved to Monday)
            || ((d == 21 || (d == 22 && w == Weekday.Monday))
                && m == Month.March)
            // Freedom Day, April 27th (possibly moved to Monday)
            || ((d == 27 || (d == 28 && w == Weekday.Monday))
                && m == Month.April)
            // Election Day, April 14th 2004
            || (d == 14 && m == Month.April && y == 2004)
            // Workers Day, May 1st (possibly moved to Monday)
            || ((d == 1 || (d == 2 && w == Weekday.Monday))
                && m == Month.May)
            // Youth Day, June 16th (possibly moved to Monday)
            || ((d == 16 || (d == 17 && w == Weekday.Monday))
                && m == Month.June)
            // National Women's Day, August 9th (possibly moved to Monday)
            || ((d == 9 || (d == 10 && w == Weekday.Monday))
                && m == Month.August)
            // Heritage Day, September 24th (possibly moved to Monday)
            || ((d == 24 || (d == 25 && w == Weekday.Monday))
                && m == Month.September)
            // Day of Reconciliation, December 16th
            // (possibly moved to Monday)
            || ((d == 16 || (d == 17 && w == Weekday.Monday))
                && m == Month.December)
            // Christmas
            || (d == 25 && m == Month.December)
            // Day of Goodwill (possibly moved to Monday)
            || ((d == 26 || (d == 27 && w == Weekday.Monday))
                && m == Month.December)
            )
            return false;
        return true;
    }
        };
          
      private static Calendar.Impl impl = new SouthAfrica.Impl();
      
      public SouthAfrica() {
        // all calendar instances share the same implementation instance
        
        _impl = impl;
    }
    };

}

