/*
 Copyright (C) 2008 Alessandro Duci
 Copyright (C) 2008 Andrea Maggiulli
 Copyright (C) 2008 Siarhei Novik (snovik@gmail.com)

 This file is part of QLNet Project http://www.qlnet.org

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

namespace QLNet {
    //! Turkish calendar
    /*! Holidays:
        <ul>
        <li>Saturdays</li>
        <li>Sundays</li>
        <li>New Year's Day, January 1st</li>
        <li>National Holidays (April 23rd, May 19th, August 30th,
            October 29th</li>
        <li>Local Holidays (Kurban, Ramadan; 2004 to 2009 only) </li>
        </ul>

        \ingroup calendars
    */
    public class Turkey :  Calendar {
        public Turkey() : base(Impl.Singleton) { }

        class Impl : Calendar {
            public static readonly Impl Singleton = new Impl();
            private Impl() { }

            public override string name() { return "Turkey"; }
            public override bool isWeekend(DayOfWeek w) {
                return w == DayOfWeek.Saturday || w == DayOfWeek.Sunday;
            }

            public override bool isBusinessDay(Date date) {
                DayOfWeek w = date.DayOfWeek;
                int d = date.Day, dd = date.DayOfYear;
                Month m = (Month)date.Month;
                int y = date.Year;

                if (isWeekend(w)
                    // New Year's Day
                    || (d == 1 && m == Month.January)
                    // 23 nisan / National Holiday
                    || (d == 23 && m == Month.April)
                    // 19 may/ National Holiday
                    || (d == 19 && m == Month.May)
                    // 30 aug/ National Holiday
                    || (d == 30 && m == Month.August)
                    ///29 ekim  National Holiday
                    || (d == 29 && m == Month.October))
                    return false;

                // Local Holidays
                if (y == 2004) {
                    // kurban
                    if ((m == Month.February && d <= 4)
                    // ramazan
                        || (m == Month.November && d >= 14 && d <= 16))
                        return false;
                } else if (y == 2005) {
                    // kurban
                    if ((m == Month.January && d >= 19 && d <= 21)
                    // ramazan
                        || (m == Month.November && d >= 2 && d <= 5))
                        return false;
                } else if (y == 2006) {
                    // kurban
                    if ((m == Month.January && d >= 9 && d <= 13)
                    // ramazan
                        || (m == Month.October && d >= 23 && d <= 25)
                    // kurban
                        || (m == Month.December && d >= 30))
                        return false;
                } else if (y == 2007) {
                    // kurban
                    if ((m == Month.January && d <= 4)
                    // ramazan
                        || (m == Month.October && d >= 11 && d <= 14)
                    // kurban
                        || (m == Month.December && d >= 19 && d <= 23))
                        return false;
                } else if (y == 2008) {
                    // ramazan
                    if ((m == Month.September && d >= 29)
                        || (m == Month.October && d <= 2)
                        // kurban
                        || (m == Month.December && d >= 7 && d <= 11))
                        return false;
                }
                return true;
            }
        };
    }
}




