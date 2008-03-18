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

    //! %Indonesian calendars
    /*! Holidays for the Jakarta stock exchange
        (data from <http://www.jsx.co.id/>):
        <ul>
        <li>Saturdays</li>
        <li>Sundays</li>
        <li>Good Friday</li>
        <li>New Year's Day, January 1st</li>
        <li>Ascension of Jesus Christ</li>
        <li>Independence Day, August 17th</li>
        <li>Christmas, December 25th</li>
        </ul>

        Other holidays for which no rule is given
        (data available for 2005-2007 only:)
        <ul>
        <li>Idul Adha</li>
        <li>Ied Adha</li>
        <li>Imlek</li>
        <li>Moslem's New Year Day</li>
        <li>Nyepi (Saka's New Year)</li>
        <li>Birthday of Prophet Muhammad SAW</li>
        <li>Waisak</li>
        <li>Ascension of Prophet Muhammad SAW</li>
        <li>Idul Fitri</li>
        <li>Ied Fitri</li>
        <li>Other national leaves</li>
        </ul>
        \ingroup calendars
    */
    public class Indonesia : Calendar {
      private class BejImpl : Calendar.WesternImpl {
        
            public override string name() { return "Jakarta stock exchange"; }
            public override bool isBusinessDay(DDate date) {
                    Weekday w = date.weekday();
        int d = date.dayOfMonth();
        Month m = date.month();
        int y = date.year();
        int dd = date.dayOfYear();
        int em = easterMonday(y);

        if (isWeekend(w)
            // New Year's Day
            || (d == 1 && m == Month.January)
            // Good Friday
            || (dd == em-3)
            // Ascension Thursday
            || (dd == em+38)
            // Independence Day
            || (d == 17 && m == Month.August)
            // Christmas
            || (d == 25 && m == Month.December)
            )
            return false;
        if (y == 2005) {
            if (// Idul Adha
                (d == 21 && m == Month.January)
                // Imlek
                || (d == 9 && m == Month.February)
                // Moslem's New Year Day
                || (d == 10 && m == Month.February)
                // Nyepi
                || (d == 11 && m == Month.March)
                // Birthday of Prophet Muhammad SAW
                || (d == 22 && m == Month.April)
                // Waisak
                || (d == 24 && m == Month.May)
                // Ascension of Prophet Muhammad SAW
                || (d == 2 && m == Month.September)
                // Idul Fitri
                || ((d == 3 || d == 4) && m == Month.November)
                // National leaves
                || ((d == 2 || d == 7 || d == 8) && m == Month.November)
                || (d == 26 && m == Month.December)
                )
                return false;
        }
        if (y == 2006) {
            if (// Idul Adha
                (d == 10 && m == Month.January)
                // Moslem's New Year Day
                || (d == 31 && m == Month.January)
                // Nyepi
                || (d == 30 && m == Month.March)
                // Birthday of Prophet Muhammad SAW
                || (d == 10 && m == Month.April)
                // Ascension of Prophet Muhammad SAW
                || (d == 21 && m == Month.August)
                // Idul Fitri
                || ((d == 24 || d == 25) && m == Month.October)
                // National leaves
                || ((d == 23 || d == 26 || d == 27) && m == Month.October)
                )
                return false;
        }
        if (y == 2007) {
            if (// Nyepi
                (d == 19 && m == Month.March)
                // Waisak
                || (d == 1 && m == Month.June)
                // Ied Adha
                || (d == 20 && m == Month.December)
                // National leaves
                || (d == 18 && m == Month.May)
                || ((d == 12 || d == 15 || d == 16) && m == Month.October)
                || ((d == 21 || d == 24) && m == Month.October)
                )
                return false;
        }
        return true;
    }

        };
           private static Calendar.Impl bejImpl = new Indonesia.BejImpl();
      public enum Market { BEJ,  //!< Jakarta stock exchange
                      JSX   //!< Jakarta stock exchange
        };
        public Indonesia()
        {
            new Indonesia(Market.BEJ);
        }
        public Indonesia(Market market) {
        // all calendar instances share the same implementation instance
       
        switch (market) {
            case Market.BEJ:
            case Market.JSX:
            _impl = bejImpl;
            break;
          default:
            throw new Exception("unknown market");
        }
    }
    };

}
