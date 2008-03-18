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

    //! Canadian calendar
    /*! Banking holidays:
        <ul>
        <li>Saturdays</li>
        <li>Sundays</li>
        <li>New Year's Day, January 1st (possibly moved to Monday)</li>
        <li>Family Day, third Monday of February (since 2008)</li>
        <li>Good Friday</li>
        <li>Easter Monday</li>
        <li>Victoria Day, The Monday on or preceding 24 May</li>
        <li>Canada Day, July 1st (possibly moved to Monday)</li>
        <li>Provincial Holiday, first Monday of August</li>
        <li>Labour Day, first Monday of September</li>
        <li>Thanksgiving Day, second Monday of October</li>
        <li>Remembrance Day, November 11th (possibly moved to Monday)</li>
        <li>Christmas, December 25th (possibly moved to Monday or Tuesday)</li>
        <li>Boxing Day, December 26th (possibly moved to Monday or
            Tuesday)</li>
        </ul>

        Holidays for the Toronto stock exchange (TSX):
        <ul>
        <li>Saturdays</li>
        <li>Sundays</li>
        <li>New Year's Day, January 1st (possibly moved to Monday)</li>
        <li>Family Day, third Monday of February (since 2008)</li>
        <li>Good Friday</li>
        <li>Easter Monday</li>
        <li>Victoria Day, The Monday on or preceding 24 May</li>
        <li>Canada Day, July 1st (possibly moved to Monday)</li>
        <li>Provincial Holiday, first Monday of August</li>
        <li>Labour Day, first Monday of September</li>
        <li>Thanksgiving Day, second Monday of October</li>
        <li>Christmas, December 25th (possibly moved to Monday or Tuesday)</li>
        <li>Boxing Day, December 26th (possibly moved to Monday or
            Tuesday)</li>
        </ul>

        \ingroup calendars
    */
    public class Canada :  Calendar {
      private class SettlementImpl : Calendar.WesternImpl {
            public override string name() { return "Canada"; }
            public override bool isBusinessDay(DDate date) {
                   Weekday w = date.weekday();
        int d = date.dayOfMonth(), dd = date.dayOfYear();
        Month m = date.month();
        int y = date.year();
        int em = easterMonday(y);
        if (isWeekend(w)
            // New Year's Day (possibly moved to Monday)
            || ((d == 1 || (d == 2 && w == Weekday.Monday)) && m == Month.January)
            // Family Day (third Monday in February, since 2008)
            || ((d >= 15 && d <= 21) && w == Weekday.Monday && m == Month.February
                && y >= 2008)
            // Good Friday
            || (dd == em-3)
            // Easter Monday
            || (dd == em)
            // The Monday on or preceding 24 May (Victoria Day)
            || (d > 17 && d <= 24 && w == Weekday.Monday && m == Month.May)
            // July 1st, possibly moved to Monday (Canada Day)
            || ((d == 1 || ((d == 2 || d == 3) && w == Weekday.Monday)) && m == Month.July)
            // first Monday of August (Provincial Holiday)
            || (d <= 7 && w == Weekday.Monday && m == Month.August)
            // first Monday of September (Labor Day)
            || (d <= 7 && w == Weekday.Monday && m == Month.September)
            // second Monday of October (Thanksgiving Day)
            || (d > 7 && d <= 14 && w == Weekday.Monday && m == Month.October)
            // November 11th (possibly moved to Monday)
            || ((d == 11 || ((d == 12 || d == 13) && w == Weekday.Monday))
                && m == Month.November)
            // Christmas (possibly moved to Monday or Tuesday)
            || ((d == 25 || (d == 27 && (w == Weekday.Monday || w == Weekday.Tuesday)))
                && m == Month.December)
            // Boxing Day (possibly moved to Monday or Tuesday)
            || ((d == 26 || (d == 28 && (w == Weekday.Monday || w == Weekday.Tuesday)))
                && m == Month.December)
            )
            return false;
        return true;
    }
        };
        private class TsxImpl :  Calendar.WesternImpl {
         
            public override string name() { return "TSX"; }
              public override bool isBusinessDay(DDate date) {
               Weekday w = date.weekday();
        int d = date.dayOfMonth(), dd = date.dayOfYear();
        Month m = date.month();
        int y = date.year();
        int em = easterMonday(y);
        if (isWeekend(w)
            // New Year's Day (possibly moved to Monday)
            || ((d == 1 || (d == 2 && w == Weekday.Monday)) && m == Month.January)
            // Family Day (third Monday in February, since 2008)
            || ((d >= 15 && d <= 21) && w == Weekday.Monday && m == Month.February
                && y >= 2008)
            // Good Friday
            || (dd == em-3)
            // Easter Monday
            || (dd == em)
            // The Monday on or preceding 24 May (Victoria Day)
            || (d > 17 && d <= 24 && w == Weekday.Monday && m == Month.May)
            // July 1st, possibly moved to Monday (Canada Day)
            || ((d == 1 || ((d == 2 || d == 3) && w == Weekday.Monday)) && m == Month.July)
            // first Monday of August (Provincial Holiday)
            || (d <= 7 && w == Weekday.Monday && m == Month.August)
            // first Monday of September (Labor Day)
            || (d <= 7 && w == Weekday.Monday && m == Month.September)
            // second Monday of October (Thanksgiving Day)
            || (d > 7 && d <= 14 && w == Weekday.Monday && m == Month.October)
            // Christmas (possibly moved to Monday or Tuesday)
            || ((d == 25 || (d == 27 && (w == Weekday.Monday || w == Weekday.Tuesday)))
                && m == Month.December)
            // Boxing Day (possibly moved to Monday or Tuesday)
            || ((d == 26 || (d == 28 && (w == Weekday.Monday || w == Weekday.Tuesday)))
                && m == Month.December)
            )
            return false;
        return true;
    }

          
        };
         
        private static Calendar.Impl settlementImpl = new Canada.SettlementImpl();
        private static Calendar.Impl tsxImpl = new Canada.TsxImpl();

      public enum Market { Settlement,       //!< generic settlement calendar
                      TSX               //!< Toronto stock exchange calendar
        };
        Canada(){
              new Canada(Market.Settlement);
        }
        public Canada(Market market) {
        // all calendar instances share the same implementation instance
        
        switch (market) {
            case Market.Settlement:
            _impl = settlementImpl;
            break;
        case Market.TSX:
            _impl = tsxImpl;
            break;
          default:
            throw new Exception("unknown market");
        }
    }

        
        };

}