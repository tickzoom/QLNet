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

namespace QLNet {
    public class TimeSeries<T> : Dictionary<Date, T> {
		// constructors
		public TimeSeries() : base() {}
        public TimeSeries(int size) : base(size) { }
    }

    public struct Duration {
        public enum Type { Simple, Macaulay, Modified };
    };

	public struct Position {
		public enum Type { Long, Short };
	};

    public enum InterestRateType { Fixed, Floating }
    //! Interest rate coumpounding rule
    public enum Compounding {
        Simple = 0,          //!< \f$ 1+rt \f$
        Compounded = 1,      //!< \f$ (1+r)^t \f$
        Continuous = 2,      //!< \f$ e^{rt} \f$
        SimpleThenCompounded //!< Simple up to the first period then Compounded
    };
	
	public enum Month { January   = 1,
                 February  = 2,
                 March     = 3,
                 April     = 4,
                 May       = 5,
                 June      = 6,
                 July      = 7,
                 August    = 8,
                 September = 9,
                 October   = 10,
                 November  = 11,
                 December  = 12,
                 Jan = 1,
                 Feb = 2,
                 Mar = 3,
                 Apr = 4,
                 Jun = 6,
                 Jul = 7,
                 Aug = 8,
                 Sep = 9,
                 Oct = 10,
                 Nov = 11,
                 Dec = 12
    };

	public enum BusinessDayConvention {
        // ISDA
        Following,          /*!< Choose the first business day after
                                 the given holiday. */
        ModifiedFollowing,  /*!< Choose the first business day after
                                 the given holiday unless it belongs
                                 to a different month, in which case
                                 choose the first business day before
                                 the holiday. */
        Preceding,          /*!< Choose the first business day before
                                 the given holiday. */
        // NON ISDA
        ModifiedPreceding,  /*!< Choose the first business day before
                                 the given holiday unless it belongs
                                 to a different month, in which case
                                 choose the first business day after
                                 the holiday. */
        Unadjusted          /*!< Do not adjust. */
    };

    //! Units used to describe time periods
    public enum TimeUnit {
        Days,
        Weeks,
        Months,
        Years };

    public enum Frequency {
        NoFrequency = -1,     //!< null frequency
        Once = 0,             //!< only once, e.g., a zero-coupon
        Annual = 1,           //!< once a year
        Semiannual = 2,       //!< twice a year
        EveryFourthMonth = 3, //!< every fourth month
        Quarterly = 4,        //!< every third month
        Bimonthly = 6,        //!< every second month
        Monthly = 12,         //!< once a month
        Biweekly = 26,        //!< every second week
        Weekly = 52,          //!< once a week
        Daily = 365           //!< once a day
    };

    // These conventions specify the rule used to generate dates in a Schedule.
    public struct DateGeneration {
        public enum Rule {
            Backward,      /*!< Backward from termination date to effective date. */
            Forward,       /*!< Forward from effective date to termination date. */
            Zero,          /*!< No intermediate dates between effective date and termination date. */
            ThirdWednesday /*!< All dates but effective date and termination date are taken to be on the third wednesday of their month*/
        };
    };
}