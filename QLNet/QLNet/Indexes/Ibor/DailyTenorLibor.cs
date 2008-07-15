/*
 Copyright (C) 2008 Toyin Akin (toyin_akin@hotmail.com)
  
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
using System.Linq;
using System.Text;

namespace QLNet {
    //! base class for O/N-S/N BBA LIBOR indexes but the EUR ones
    //    ! One day deposit LIBOR fixed by BBA.
    //
    //        See <http://www.bba.org.uk/bba/jsp/polopoly.jsp?d=225&a=1414>.
    //    
    public class DailyTenorLibor : IborIndex {
        // http://www.bba.org.uk/bba/jsp/polopoly.jsp?d=225&a=1412 :
        // no o/n or s/n fixings (as the case may be) will take place
        // when the principal centre of the currency concerned is
        // closed but London is open on the fixing day.
        public DailyTenorLibor(string familyName, int settlementDays, Currency currency, Calendar financialCenterCalendar, 
                               DayCounter dayCounter)
            : this(familyName, settlementDays, currency, financialCenterCalendar, dayCounter, new Handle<YieldTermStructure>()) {
        }

        public DailyTenorLibor(string familyName, int settlementDays, Currency currency, Calendar financialCenterCalendar, 
                               DayCounter dayCounter, Handle<YieldTermStructure> h)
            : base(familyName, new Period(1, TimeUnit.Days), settlementDays, currency, 
                   new JointCalendar(new UnitedKingdom(UnitedKingdom.Market.Exchange), financialCenterCalendar, JointCalendar.JointCalendarRule.JoinHolidays),
                   Utils.liborConvention(new Period(1, TimeUnit.Days)), Utils.liborEOM(new Period(1, TimeUnit.Days)), dayCounter, h) {
            if (!(currency != new EURCurrency()))
                throw new ApplicationException("for EUR Libor dedicated EurLibor constructor must be used");
        }
    }
}