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
    
    //! United Kingdom calendars
    /*! Public holidays (data from http://www.dti.gov.uk/er/bankhol.htm):
        <ul>
        <li>Saturdays</li>
        <li>Sundays</li>
        <li>New Year's Day, January 1st (possibly moved to Monday)</li>
        <li>Good Friday</li>
        <li>Easter Monday</li>
        <li>Early May Bank Holiday, first Monday of May</li>
        <li>Spring Bank Holiday, last Monday of May</li>
        <li>Summer Bank Holiday, last Monday of August</li>
        <li>Christmas Day, December 25th (possibly moved to Monday or
            Tuesday)</li>
        <li>Boxing Day, December 26th (possibly moved to Monday or
            Tuesday)</li>
        </ul>

        Holidays for the stock exchange:
        <ul>
        <li>Saturdays</li>
        <li>Sundays</li>
        <li>New Year's Day, January 1st (possibly moved to Monday)</li>
        <li>Good Friday</li>
        <li>Easter Monday</li>
        <li>Early May Bank Holiday, first Monday of May</li>
        <li>Spring Bank Holiday, last Monday of May</li>
        <li>Summer Bank Holiday, last Monday of August</li>
        <li>Christmas Day, December 25th (possibly moved to Monday or
            Tuesday)</li>
        <li>Boxing Day, December 26th (possibly moved to Monday or
            Tuesday)</li>
        </ul>

        Holidays for the metals exchange:
        <ul>
        <li>Saturdays</li>
        <li>Sundays</li>
        <li>New Year's Day, January 1st (possibly moved to Monday)</li>
        <li>Good Friday</li>
        <li>Easter Monday</li>
        <li>Early May Bank Holiday, first Monday of May</li>
        <li>Spring Bank Holiday, last Monday of May</li>
        <li>Summer Bank Holiday, last Monday of August</li>
        <li>Christmas Day, December 25th (possibly moved to Monday or
            Tuesday)</li>
        <li>Boxing Day, December 26th (possibly moved to Monday or
            Tuesday)</li>
        </ul>

        \ingroup calendars

        \todo add LIFFE

        \test the correctness of the returned results is tested
              against a list of known holidays.
    */
    public class UnitedKingdom : Calendar 
    {
      private class SettlementImpl : Calendar.WesternImpl 
      {
          public override string name() { return "UK settlement"; }
          public override bool isBusinessDay(DDate date) {
            Weekday w = date.weekday();
            int d = date.dayOfMonth(), dd = date.dayOfYear();
            Month m = date.month();
            int y = date.year();
            int em = easterMonday(y);
            if (isWeekend(w)
                // New Year's Day (possibly moved to Monday)
                || ((d == 1 || ((d == 2 || d == 3) && w == Weekday.Monday)) &&
                    m == Month.January)
                // Good Friday
                || (dd == em-3)
                // Easter Monday
                || (dd == em)
                // first Monday of May (Early May Bank Holiday)
                || (d <= 7 && w == Weekday.Monday && m == Month.May)
                // last Monday of May (Spring Bank Holiday)
                || (d >= 25 && w == Weekday.Monday && m == Month.May && y != 2002)
                // last Monday of August (Summer Bank Holiday)
                || (d >= 25 && w == Weekday.Monday && m == Month.August)
                // Christmas (possibly moved to Monday or Tuesday)
                || ((d == 25 || (d == 27 && (w == Weekday.Monday || w == Weekday.Tuesday)))
                    && m == Month.December)
                // Boxing Day (possibly moved to Monday or Tuesday)
                || ((d == 26 || (d == 28 && (w == Weekday.Monday || w == Weekday.Tuesday)))
                    && m == Month.December)
                // June 3rd, 2002 only (Golden Jubilee Bank Holiday)
                // June 4rd, 2002 only (special Spring Bank Holiday)
                || ((d == 3 || d == 4) && m == Month.June && y == 2002)
                // December 31st, 1999 only
                || (d == 31 && m == Month.December && y == 1999))
                return false;
            return true;
          }
        };
        private class ExchangeImpl : Calendar.WesternImpl {

            public override string name() { return "London stock exchange"; }
            public override bool isBusinessDay(DDate date) {
        Weekday w = date.weekday();
        int d = date.dayOfMonth(), dd = date.dayOfYear();
        Month m = date.month();
        int y = date.year();
        int em = easterMonday(y);
        if (isWeekend(w)
            // New Year's Day (possibly moved to Monday)
            || ((d == 1 || ((d == 2 || d == 3) && w == Weekday.Monday)) &&
                m == Month.January)
            // Good Friday
            || (dd == em-3)
            // Easter Monday
            || (dd == em)
            // first Monday of May (Early May Bank Holiday)
            || (d <= 7 && w == Weekday.Monday && m == Month.May)
            // last Monday of May (Spring Bank Holiday)
            || (d >= 25 && w == Weekday.Monday && m == Month.May && y != 2002)
            // last Monday of August (Summer Bank Holiday)
            || (d >= 25 && w == Weekday.Monday && m == Month.August)
            // Christmas (possibly moved to Monday or Tuesday)
            || ((d == 25 || (d == 27 && (w == Weekday.Monday || w == Weekday.Tuesday)))
                && m == Month.December)
            // Boxing Day (possibly moved to Monday or Tuesday)
            || ((d == 26 || (d == 28 && (w == Weekday.Monday || w == Weekday.Tuesday)))
                && m == Month.December)
            // June 3rd, 2002 only (Golden Jubilee Bank Holiday)
            // June 4rd, 2002 only (special Spring Bank Holiday)
            || ((d == 3 || d == 4) && m == Month.June && y == 2002)
            // December 31st, 1999 only
            || (d == 31 && m == Month.December && y == 1999))
            return false;
        return true;
    }

        };
        private class MetalsImpl : Calendar.WesternImpl {
            public override string name() { return "London metals exchange"; }
            public override bool isBusinessDay(DDate date) {
                  Weekday w = date.weekday();
                  int d = date.dayOfMonth(), dd = date.dayOfYear();
        Month m = date.month();
        int y = date.year();
        int em = easterMonday(y);
        if (isWeekend(w)
            // New Year's Day (possibly moved to Monday)
            || ((d == 1 || ((d == 2 || d == 3) && w == Weekday.Monday)) &&
                m == Month.January)
            // Good Friday
            || (dd == em-3)
            // Easter Monday
            || (dd == em)
            // first Monday of May (Early May Bank Holiday)
            || (d <= 7 && w == Weekday.Monday && m == Month.May)
            // last Monday of May (Spring Bank Holiday)
            || (d >= 25 && w == Weekday.Monday && m == Month.May && y != 2002)
            // last Monday of August (Summer Bank Holiday)
            || (d >= 25 && w == Weekday.Monday && m == Month.August)
            // Christmas (possibly moved to Monday or Tuesday)
            || ((d == 25 || (d == 27 && (w == Weekday.Monday || w == Weekday.Tuesday)))
                && m == Month.December)
            // Boxing Day (possibly moved to Monday or Tuesday)
            || ((d == 26 || (d == 28 && (w == Weekday.Monday || w == Weekday.Tuesday)))
                && m == Month.December)
            // June 3rd, 2002 only (Golden Jubilee Bank Holiday)
            // June 4rd, 2002 only (special Spring Bank Holiday)
            || ((d == 3 || d == 4) && m == Month.June && y == 2002)
            // December 31st, 1999 only
            || (d == 31 && m == Month.December && y == 1999))
            return false;
        return true;
    }

        };
      

        private static Calendar.Impl settlementImpl = new UnitedKingdom.SettlementImpl();
        private static Calendar.Impl exchangeImpl = new UnitedKingdom.ExchangeImpl();
        private static Calendar.Impl metalsImpl = new UnitedKingdom.MetalsImpl();
      
      
        //! UK calendars
        public enum Market { 
            Settlement,     //!< generic settlement calendar
            Exchange,       //!< London stock-exchange calendar
            Metals          //!< London metals-exchange calendar
        };
        
        public UnitedKingdom()
        {
            new UnitedKingdom(Market.Settlement);  
        }

        public UnitedKingdom(Market market) {
         // all calendar instances on the same market share the same
        // implementation instance
        switch (market) {
            case Market.Settlement:
            _impl = settlementImpl;
            break;
        case Market.Exchange:
            _impl = exchangeImpl;
            break;
        case Market.Metals:
            _impl = metalsImpl;
            break;
          default:
            throw new Exception("unknown market");
        }
    }
    };

}

