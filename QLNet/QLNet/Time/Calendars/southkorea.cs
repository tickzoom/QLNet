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

    //! South Korean calendars
    /*! Holidays for the Korea exchange
        (data from <http://www.krx.co.kr>):
        <ul>
        <li>Saturdays</li>
        <li>Sundays</li>
        <li>New Year's Day, January 1st</li>
        <li>Independence Day, March 1st</li>
        <li>Arbour Day, April 5th</li>
        <li>Labor Day, May 1st</li>
        <li>Children's Day, May 5th</li>
        <li>Memorial Day, June 6th</li>
        <li>Constitution Day, July 17th</li>
        <li>Liberation Day, August 15th</li>
        <li>National Fondation Day, October 3th</li>
        <li>Christmas Day, December 25th</li>
        </ul>

        Other holidays for which no rule is given
        (data available for 2004-2007 only:)
        <ul>
        <li>Lunar New Year</li>
        <li>Election Day 2004</li>
        <li>Buddha's birthday</li>
        <li>Harvest Moon Day</li>
        </ul>

        \ingroup calendars
    */
    public class SouthKorea : Calendar {
      private class KrxImpl : Calendar.Impl {
          
            public override string name() { return "Korea exchange"; }
            public override bool isWeekend(Weekday w)
            {
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
            // Independence Day
            || (d == 1 && m == Month.March)
            // Arbour Day
            || (d == 5 && m == Month.April)
            // Labor Day
            || (d == 1 && m == Month.May)
            // Children's Day
            || (d == 5 && m == Month.May)
            // Memorial Day
            || (d == 6 && m == Month.June)
            // Constitution Day
            || (d == 17 && m == Month.July)
            // Liberation Day
            || (d == 15 && m == Month.August)
            // National Foundation Day
            || (d == 3 && m == Month.October)
            // Christmas Day
            || (d == 25 && m == Month.December)

            // Lunar New Year 2004
            || ((d == 21 || d==22 || d==23 || d==24 || d==26 )
                && m == Month.January && y==2004)
            || ((d == 8 || d==9 || d==10) && m == Month.February && y==2005)
            || ((d==29 || d==30 || d==31 ) && m == Month.January && y==2006)
            || (d==19 && m == Month.February && y==2007)
            // Election Day 2004
            || (d == 15 && m == Month.April && y==2004)
            // Buddha's birthday
            || (d == 26 && m == Month.May && y==2004)
            || (d == 15 && m == Month.May && y==2005)
            || (d == 5 && m == Month.May && y==2006)
            || (d == 24 && m == Month.May && y==2007)
            // Harvest Moon Day
            || ((d == 27 || d == 28 || d == 29) && m == Month.September && y==2004)
            || ((d == 17 || d == 18 || d == 19) && m == Month.September && y==2005)
            || ((d == 5 || d == 6 || d == 7) && m == Month.October && y==2006)
            || ((d == 24 || d == 25 || d == 26) && m == Month.September && y==2007)
            )
            return false;
        return true;
    }
        };
      public enum Market { KRX    //!< Korea exchange
        };
         
     private static Calendar.Impl krxImpl = new SouthKorea.KrxImpl();
        
        SouthKorea(){
            new SouthKorea(Market.KRX);
        }

        SouthKorea(Market market) {
        // all calendar instances share the same implementation instance
      
        switch (market) {
          case Market.KRX:
            _impl = krxImpl;
            break;
          default:
            throw new Exception("unknown market");
        }
    }

    
    };

}



  
