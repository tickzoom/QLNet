/*
 Copyright (C) 2008 Siarhei Novik (snovik@gmail.com)
  
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
using System.Linq;
using System.Text;

namespace QLNet {
    public class Brazil : Calendar {
        //! Brazilian calendars
        public enum Market { Settlement,            //!< generic settlement calendar
                             Exchange               //!< BOVESPA calendar
        }

        public Brazil() : this(Market.Settlement) { }
        public Brazil(Market market) {
        // all calendar instances on the same market share the same implementation instance
            switch (market) {
                case Market.Settlement:
                    calendar_ = SettlementImpl.Singleton;
                    break;
                case Market.Exchange:
                    calendar_  = ExchangeImpl.Singleton;
                    break;
                default:
                    throw new ApplicationException("unknown market");
            }
        }


        private class SettlementImpl : Calendar.WesternImpl {
            public static readonly SettlementImpl Singleton = new SettlementImpl();
            private SettlementImpl() { }

            public override string name() { return "Brazil"; }
            public override bool isBusinessDay(Date date) {
                DayOfWeek w = date.DayOfWeek;
                int d = date.Day;
                Month m = (Month)date.Month;
                int y = date.Year;
                int dd = date.DayOfYear;
                int em = easterMonday(y);

                if (isWeekend(w)
                    // New Year's Day
                    || (d == 1 && m == Month.January)
                    // Tiradentes Day
                    || (d == 21 && m == Month.April)
                    // Labor Day
                    || (d == 1 && m == Month.May)
                    // Independence Day
                    || (d == 7 && m == Month.September)
                    // Nossa Sra. Aparecida Day
                    || (d == 12 && m == Month.October)
                    // All Souls Day
                    || (d == 2 && m == Month.November)
                    // Republic Day
                    || (d == 15 && m == Month.November)
                    // Christmas
                    || (d == 25 && m == Month.December)
                    // Passion of Christ
                    || (dd == em-3)
                    // Carnival
                    || (dd == em-49 || dd == em-48)
                    // Corpus Christi
                    || (dd == em+59)
                    )
                    return false;
                return true;
            }
        }

        private class ExchangeImpl : Calendar.WesternImpl {
            public static readonly ExchangeImpl Singleton = new ExchangeImpl();
            private ExchangeImpl() { }

            public override string name() { return "BOVESPA"; }
            public override bool isBusinessDay(Date date) {
                DayOfWeek w = date.DayOfWeek;
                int d = date.Day;
                Month m = (Month)date.Month;
                int y = date.Year;
                int dd = date.DayOfYear;
                int em = easterMonday(y);

                if (isWeekend(w)
                    // New Year's Day
                    || (d == 1 && m == Month.January)
                    // Sao Paulo City Day
                    || (d == 25 && m == Month.January)
                    // Tiradentes Day
                    || (d == 21 && m == Month.April)
                    // Labor Day
                    || (d == 1 && m == Month.May)
                    // Revolution Day
                    || (d == 9 && m == Month.July)
                    // Independence Day
                    || (d == 7 && m == Month.September)
                    // Nossa Sra. Aparecida Day
                    || (d == 12 && m == Month.October)
                    // All Souls Day
                    || (d == 2 && m == Month.November)
                    // Republic Day
                    || (d == 15 && m == Month.November)
                    // Black Consciousness Day
                    || (d == 20 && m == Month.November && y >= 2007)
                    // Christmas
                    || (d == 25 && m == Month.December)
                    // Passion of Christ
                    || (dd == em-3)
                    // Carnival
                    || (dd == em-49 || dd == em-48)
                    // Corpus Christi
                    || (dd == em+59)
                    // last business day of the year
                    || (m == Month.December && (d == 31 || (d >= 29 && w == DayOfWeek.Friday)))
                    )
                    return false;
                return true;
            }
        }

    }
}
