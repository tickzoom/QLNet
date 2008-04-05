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

    // %Euribor index
    // Euribor rate fixed by the ECB.
    // This is the rate fixed by the ECB. Use EurLibor if you're interested in the London fixing by BBA.
    public class Euribor : IborIndex {
        public Euribor(Period tenor) : this(tenor, new Handle<YieldTermStructure>()) { }
        public Euribor(Period tenor, Handle<YieldTermStructure> h) :
            base("Euribor", tenor, 2, // settlementDays
                 new EURCurrency(), new TARGET(),
                 euriborConvention(tenor), euriborEOM(tenor),
                 new Actual360(), h) { }

        private static BusinessDayConvention euriborConvention(Period p) {
            switch (p.units()) {
                case TimeUnit.Days:
                case TimeUnit.Weeks:
                    return BusinessDayConvention.Following;
                case TimeUnit.Months:
                case TimeUnit.Years:
                    return BusinessDayConvention.ModifiedFollowing;
                default:
                    throw Error.UnknownTimeUnit(p.units());
            }
        }

        private static bool euriborEOM(Period p) {
            switch (p.units()) {
                case TimeUnit.Days:
                case TimeUnit.Weeks:
                    return false;
                case TimeUnit.Months:
                case TimeUnit.Years:
                    return true;
                default:
                    throw Error.UnknownTimeUnit(p.units());
            }
        }
    }

    // 3-months %Euribor index
    public class Euribor3M : Euribor {
        public Euribor3M() : this(new Handle<YieldTermStructure>()) { }
        public Euribor3M(Handle<YieldTermStructure> h)
            : base(new Period(3, TimeUnit.Months), h) {}
    }
    // 6-months %Euribor index
    public class Euribor6M : Euribor {
        public Euribor6M() : this(new Handle<YieldTermStructure>()) { }
        public Euribor6M(Handle<YieldTermStructure> h)
            : base(new Period(6, TimeUnit.Months), h) { }
    }
}
