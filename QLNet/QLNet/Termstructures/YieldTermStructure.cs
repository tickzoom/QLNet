/*
 Copyright (C) 2008 Andrea Maggiulli
  
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
   /// <summary>
   /// Interest-rate term structure
   /// This abstract class defines the interface of concrete
   /// rate structures which will be derived from this one.
   /// Rates are assumed to be annual continuous compounding.
   /// </summary>
   public class YieldTermStructure : TermStructure
   {
      /// <summary>
      /// Constructors
      /// See the TermStructure documentation for issues regarding
      /// constructors.
      /// </summary>
      public YieldTermStructure() : this(new DayCounter())
      {
      }

      /// <summary>
      /// Default constructor
      /// <remarks>
      /// term structures initialized by means of this
      /// constructor must manage their own reference date
      /// by overriding the referenceDate() method.
      /// </remarks> 
      /// </summary>
      /// <param name="dc"></param>
      public YieldTermStructure(DayCounter dc)  : base(dc)
      {
      }

      /// <summary>
      /// initialize with a fixed reference date
      /// </summary>
      /// <param name="referenceDate"></param>
      /// <param name="cal"></param>
      public YieldTermStructure(DDate referenceDate, Calendar cal) : this(referenceDate, cal, new DayCounter())
      {
      }
      public YieldTermStructure(DDate referenceDate) : this(referenceDate, new Calendar(), new DayCounter())
      {
      }
      public YieldTermStructure(DDate referenceDate, Calendar cal, DayCounter dc) : base(referenceDate, cal, dc)
      {
      }
      /// <summary>
      /// calculate the reference date based on the global evaluation date
      /// </summary>
      /// <param name="settlementDays"></param>
      /// <param name="cal"></param>
      public YieldTermStructure(int settlementDays, Calendar cal) : this(settlementDays, cal, new DayCounter())
      {
      }
      public YieldTermStructure(int settlementDays, Calendar cal, DayCounter dc) : base(settlementDays, cal, dc)
      {
      }

      public InterestRate zeroRate(DDate d, DayCounter dayCounter, Compounding comp, Frequency freq)
      {
         return zeroRate(d, dayCounter, comp, freq, false);
      }
      public InterestRate zeroRate(DDate d, DayCounter dayCounter, Compounding comp)
      {
         return zeroRate(d, dayCounter, comp, Frequency.Annual, false);
      }
      public InterestRate zeroRate(DDate d, DayCounter dayCounter, Compounding comp, Frequency freq, bool extrapolate)
      {
         if (d == referenceDate())
         {
            double t = 0.0001;
            //double compound = 1.0 / discount(t, extrapolate);
            //return InterestRate.impliedRate(compound, t, dayCounter, comp, freq);
         }
         double c= 1.0 / discount(d, extrapolate);
         return InterestRate.impliedRate(c, referenceDate(), d, dayCounter, comp, freq);
      }

      /// <summary>
      /// Discount factors
      /// These methods return the discount factor for a given date
      /// or time.  In the former case, the time is calculated as a
      /// fraction of year from the reference date.
      /// </summary>
      /// <param name="d"></param>
      /// <param name="extrapolate"></param>
      /// <returns></returns>
      public double discount(DDate d)
      {
         return discount(d, false);
      }

      double discount(DDate d,bool extrapolate)
      {
           return 0;
     }

   }
}
