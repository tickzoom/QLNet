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

    //! Saudi Arabian calendar
    /*! Holidays for the Tadawul financial market
        (data from <http://www.tadawul.com.sa>):
        <ul>
        <li>Thursdays</li>
        <li>Fridays</li>
        <li>National Day of Saudi Arabia, September 23rd</li>
        </ul>

        Other holidays for which no rule is given
        (data available for 2004-2005 only:)
        <ul>
        <li>Eid Al-Adha</li>
        <li>Eid Al-Fitr</li>
        </ul>

        \ingroup calendars
    */
    public class SaudiArabia : Calendar {
      private class TadawulImpl : Calendar.Impl {
          
            public override string name() { return "Tadawul"; }
           public override bool isWeekend(Weekday w){
               return w == Weekday.Thursday || w == Weekday.Friday;
    }

            public override bool isBusinessDay(DDate date) {
                     Weekday w = date.weekday();
        int d = date.dayOfMonth();
        Month m = date.month();
        int y = date.year();

        if (isWeekend(w)
            // National Day
            || (d == 23 && m == Month.September)
            // Eid Al-Adha
            || (d >= 1 && d <= 6 && m == Month.February && y==2004)
            || (d >= 21 && d <= 25 && m == Month.January && y==2005)
            // Eid Al-Fitr
            || (d >= 25 && d <= 29 && m == Month.November && y==2004)
            || (d >= 14 && d <= 18 && m == Month.November && y==2005)
            )
            return false;
        return true;
    }

        };  
          
      private static Calendar.Impl tadawulImpl = new SaudiArabia.TadawulImpl();
      public enum Market { Tadawul    //!< Tadawul financial market
        };
       public SaudiArabia(){
            new SaudiArabia(Market.Tadawul);
       }

       public SaudiArabia(Market market) {
        // all calendar instances share the same implementation instance
      
        switch (market) {
          case Market.Tadawul:
            _impl = tadawulImpl;
            break;
          default:
            throw new Exception("unknown market");
        }
    }
    };

}