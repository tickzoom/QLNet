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

    //! German calendars
    /*! Public holidays:
        <ul>
        <li>Saturdays</li>
        <li>Sundays</li>
        <li>New Year's Day, January 1st</li>
        <li>Good Friday</li>
        <li>Easter Monday</li>
        <li>Ascension Thursday</li>
        <li>Whit Monday</li>
        <li>Corpus Christi</li>
        <li>Labour Day, May 1st</li>
        <li>National Day, October 3rd</li>
        <li>Christmas Eve, December 24th</li>
        <li>Christmas, December 25th</li>
        <li>Boxing Day, December 26th</li>
        <li>New Year's Eve, December 31st</li>
        </ul>

        Holidays for the Frankfurt Stock exchange
        (data from http://deutsche-boerse.com/):
        <ul>
        <li>Saturdays</li>
        <li>Sundays</li>
        <li>New Year's Day, January 1st</li>
        <li>Good Friday</li>
        <li>Easter Monday</li>
        <li>Labour Day, May 1st</li>
        <li>Christmas' Eve, December 24th</li>
        <li>Christmas, December 25th</li>
        <li>Christmas Holiday, December 26th</li>
        <li>New Year's Eve, December 31st</li>
        </ul>

        Holidays for the Xetra exchange
        (data from http://deutsche-boerse.com/):
        <ul>
        <li>Saturdays</li>
        <li>Sundays</li>
        <li>New Year's Day, January 1st</li>
        <li>Good Friday</li>
        <li>Easter Monday</li>
        <li>Labour Day, May 1st</li>
        <li>Christmas' Eve, December 24th</li>
        <li>Christmas, December 25th</li>
        <li>Christmas Holiday, December 26th</li>
        <li>New Year's Eve, December 31st</li>
        </ul>

        Holidays for the Eurex exchange
        (data from http://www.eurexchange.com/index.html):
        <ul>
        <li>Saturdays</li>
        <li>Sundays</li>
        <li>New Year's Day, January 1st</li>
        <li>Good Friday</li>
        <li>Easter Monday</li>
        <li>Labour Day, May 1st</li>
        <li>Christmas' Eve, December 24th</li>
        <li>Christmas, December 25th</li>
        <li>Christmas Holiday, December 26th</li>
        <li>New Year's Eve, December 31st</li>
        </ul>

        \ingroup calendars

        \test the correctness of the returned results is tested
              against a list of known holidays.
    */
    class Germany : Calendar {
      private class SettlementImpl : Calendar.WesternImpl {
          
            public override string name() { return "German settlement"; }
            public override bool isBusinessDay(DDate date) {
                Weekday w = date.weekday();
        int d = date.dayOfMonth(), dd = date.dayOfYear();
        Month m = date.month();
        int y = date.year();
        int em = easterMonday(y);
        if (isWeekend(w)
            // New Year's Day
            || (d == 1 && m == Month.January)
            // Good Friday
            || (dd == em-3)
            // Easter Monday
            || (dd == em)
            // Ascension Thursday
            || (dd == em+38)
            // Whit Monday
            || (dd == em+49)
            // Corpus Christi
            || (dd == em+59)
            // Labour Day
            || (d == 1 && m == Month.May)
            // National Day
            || (d == 3 && m == Month.October)
            // Christmas Eve
            || (d == 24 && m == Month.December)
            // Christmas
            || (d == 25 && m == Month.December)
            // Boxing Day
            || (d == 26 && m == Month.December)
            // New Year's Eve
            || (d == 31 && m == Month.December))
            return false;
        return true;
    }
        };
       private class FrankfurtStockExchangeImpl : Calendar.WesternImpl {
          
            public override string name() { return "Frankfurt stock exchange"; }
            public override bool isBusinessDay(DDate date) {
                 Weekday w = date.weekday();
        int d = date.dayOfMonth(), dd = date.dayOfYear();
        Month m = date.month();
        int y = date.year();
        int em = easterMonday(y);
        if (isWeekend(w)
            // New Year's Day
            || (d == 1 && m == Month.January)
            // Good Friday
            || (dd == em-3)
            // Easter Monday
            || (dd == em)
            // Labour Day
            || (d == 1 && m == Month.May)
            // Christmas' Eve
            || (d == 24 && m == Month.December)
            // Christmas
            || (d == 25 && m == Month.December)
            // Christmas Day
            || (d == 26 && m == Month.December)
            // New Year's Eve
            || (d == 31 && m == Month.December))
            return false;
        return true;
    }
        };
        private class XetraImpl : Calendar.WesternImpl {
          
            public override string name() { return "Xetra"; }
             public override bool isBusinessDay(DDate date) {
             Weekday w = date.weekday();
        int d = date.dayOfMonth(), dd = date.dayOfYear();
        Month m = date.month();
        int y = date.year();
        int em = easterMonday(y);
        if (isWeekend(w)
            // New Year's Day
            || (d == 1 && m == Month.January)
            // Good Friday
            || (dd == em-3)
            // Easter Monday
            || (dd == em)
            // Labour Day
            || (d == 1 && m == Month.May)
            // Christmas' Eve
            || (d == 24 && m == Month.December)
            // Christmas
            || (d == 25 && m == Month.December)
            // Christmas Day
            || (d == 26 && m == Month.December)
            // New Year's Eve
            || (d == 31 && m == Month.December))
            return false;
        return true;
    }
           
        };
        private class EurexImpl : Calendar.WesternImpl {
          
            public override string name() { return "Eurex"; }
            public override bool isBusinessDay(DDate date) {
                 Weekday w = date.weekday();
        int d = date.dayOfMonth(), dd = date.dayOfYear();
        Month m = date.month();
        int y = date.year();
        int em = easterMonday(y);
        if (isWeekend(w)
            // New Year's Day
            || (d == 1 && m == Month.January)
            // Good Friday
            || (dd == em-3)
            // Easter Monday
            || (dd == em)
            // Labour Day
            || (d == 1 && m == Month.May)
            // Christmas' Eve
            || (d == 24 && m == Month.December)
            // Christmas
            || (d == 25 && m == Month.December)
            // Christmas Day
            || (d == 26 && m == Month.December)
            // New Year's Eve
            || (d == 31 && m == Month.December))
            return false;
        return true;
    }
        };
      private static Calendar.Impl settlementImpl=new Germany.SettlementImpl();
        private static Calendar.Impl frankfurtStockExchangeImpl = new Germany.FrankfurtStockExchangeImpl();
        private static Calendar.Impl xetraImpl=new Germany.XetraImpl();
        private static Calendar.Impl eurexImpl=new Germany.EurexImpl();
        //! German calendars
      public enum Market { Settlement,             //!< generic settlement calendar
                      FrankfurtStockExchange, //!< Frankfurt stock-exchange
                      Xetra,                  //!< Xetra
                      Eurex                   //!< Eurex
        };
        public Germany(){
        new Germany(Market.FrankfurtStockExchange);
        }

 public Germany(Market market) {
        // all calendar instances on the same market share the same
        // implementation instance
        
        switch (market) {
            case Market.Settlement:
            _impl = settlementImpl;
            break;
        case Market.FrankfurtStockExchange:
            _impl = frankfurtStockExchangeImpl;
            break;
        case Market.Xetra:
            _impl = xetraImpl;
            break;
        case Market.Eurex:
            _impl = eurexImpl;
            break;
          default:
            throw new Exception("unknown market");
        }
    }

    };

}