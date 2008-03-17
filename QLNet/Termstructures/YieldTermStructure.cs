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

      protected virtual double discountImpl(double P) {throw new Exception ("discountImpl not implemented");}

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
            double compound = 1.0 / discount(t, extrapolate);
            return InterestRate.impliedRate(compound, t, dayCounter, comp, freq);
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
      public double discount(DDate d, bool extrapolate)
      {
         return discount(timeFromReference(d), extrapolate);
      }
      public double discount(DDate d)
      {
         return discount(timeFromReference(d),false);
      }

      /// <summary>
      /// The same day-counting rule used by the term structure
      /// should be used for calculating the passed time t.
      /// </summary>
      /// <param name="t"></param>
      /// <returns></returns>
      public double discount(double t)
      {
         return discount(t, false);
      }
      public double discount(double t, bool extrapolate)
      {
         checkRange(t, extrapolate);
         return discountImpl(t);
      }

        /// <summary>
        /// These methods returns the implied forward interest rate
        /// between two dates or times.  In the former case, times are
        /// calculated as fractions of year from the reference date.
        /// The resulting interest rate has the required day-counting
        /// rule.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="p"></param>
        /// <param name="dayCounter"></param>
        /// <param name="comp"></param>
        /// <param name="freq"></param>
        /// <returns></returns>
        public InterestRate forwardRate(DDate d, Period p, DayCounter dayCounter, Compounding comp, Frequency freq)
        {
           return forwardRate(d, p, dayCounter, comp, freq, false);
        }
        public InterestRate forwardRate(DDate d, Period p, DayCounter dayCounter, Compounding comp)
        {
           return forwardRate(d, p, dayCounter, comp, Frequency.Annual, false);
        }
        public InterestRate forwardRate(DDate d, Period p, DayCounter dayCounter, Compounding comp, Frequency freq, bool extrapolate)
        {
            return forwardRate(d, d+p, dayCounter, comp, freq, extrapolate);
        }
      /// <summary>
      /// The resulting interest rate has the required day-counting rule.
      /// <remarks>
      /// dates are not adjusted for holidays
      /// </remarks> 
      /// </summary>
      /// <param name="d1"></param>
      /// <param name="d2"></param>
      /// <param name="dayCounter"></param>
      /// <param name="comp"></param>
      /// <param name="freq"></param>
      /// <returns></returns>
        public InterestRate forwardRate(DDate d1, DDate d2, DayCounter dayCounter, Compounding comp, Frequency freq)
        {
           return forwardRate(d1, d2, dayCounter, comp, freq, false);
        }
        public InterestRate forwardRate(DDate d1, DDate d2, DayCounter dayCounter, Compounding comp)
        {
           return forwardRate(d1, d2, dayCounter, comp, Frequency.Annual, false);
        }
        public InterestRate forwardRate(DDate d1, DDate d2, DayCounter dayCounter, Compounding comp, Frequency freq, bool extrapolate)
        {
           double compound;

            if (d1 == d2)
            {
                double t1= timeFromReference(d1);
                double t2 = t1 + 0.0001;
                compound = discount(t1, extrapolate)/discount(t2, extrapolate);
                return InterestRate.impliedRate(compound, t2-t1, dayCounter, comp, freq);
            }
           if ( d1 >= d2 )
              throw new Exception(d1 + " later than " + d2);

            compound = discount(d1, extrapolate)/discount(d2, extrapolate);
            return InterestRate.impliedRate(compound, d1, d2, dayCounter, comp, freq);
        }

      /// <summary>
      /// The resulting interest rate has the same day-counting rule
      /// used by the term structure. The same rule should be used
      /// for the calculating the passed times t1 and t2.
      /// </summary>
      /// <param name="detail"></param>
      /// <returns></returns>
        public InterestRate forwardRate(double t1, double t2, Compounding comp, Frequency freq)
        {
           return forwardRate(t1, t2, comp, freq, false);
        }
        public InterestRate forwardRate(double t1, double  t2, Compounding comp)
        {
           return forwardRate(t1, t2, comp, Frequency.Annual, false);
        }
        public InterestRate forwardRate(double t1, double t2, Compounding comp, Frequency freq, bool extrapolate)
        {
            if (t2 == t1)
            t2 =t1+0.0001;
            
           if ( t2 <= t1 )
              throw new Exception( "t2 (" + t2 + ") < t1 (" + t2 + ")");

            double compound = discount(t1, extrapolate)/discount(t2, extrapolate);
            return InterestRate.impliedRate(compound, t2-t1, dayCounter(), comp, freq);
        }

        // par rates
        //
        // These methods returns the implied par rate for a given
        // sequence of payments at the given dates or times.  In the
        // former case, times are calculated as fractions of year
        // from the reference date.
        //
        // though somewhat related to a swap rate, this
        // method is not to be used for the fair rate of a
        // real swap, since it does not take into account
        // all the market conventions' details. The correct
        // way to evaluate such rate is to instantiate a
        // SimpleSwap with the correct conventions, pass it
        // the term structure and call the swap's fairRate()
        // method.
        //        
        public double parRate(int tenor, DDate startDate, Frequency freq)
        {
           return parRate(tenor, startDate, freq, false);
        }
        public double parRate(int tenor, DDate startDate)
        {
           return parRate(tenor, startDate, Frequency.Annual, false);
        }
        public double parRate(int tenor, DDate startDate, Frequency freq, bool extrapolate)
        {
           List<DDate> dates = new List<DDate>();
           dates.Add(startDate); 
           dates.Capacity = (tenor + 1);
           for (int i = 1; i <= tenor; ++i)
              dates.Add(startDate + new Period(i ,TimeUnit.Years));
           return parRate(dates, freq, extrapolate);
        }

      /// <summary>
      /// the first date in the vector must equal the start date;
      /// the following dates must equal the payment dates.
      /// </summary>
      /// <param name="dates"></param>
      /// <param name="freq"></param>
      /// <returns></returns>
        public double parRate(List<DDate> dates, Frequency freq)
        {
           return parRate(dates, freq, false);
        }
        public double parRate(List<DDate> dates)
        {
           return parRate(dates, Frequency.Annual, false);
        }
        public double parRate(List<DDate> dates, Frequency freq, bool extrapolate)
        {
           List<double> times = new List<double>((dates.Count));
           for (int i = 0; i < dates.Count; i++)
              times[i] = timeFromReference(dates[i]);
           return parRate(times, freq, extrapolate);
        }

      /// <summary>
        /// the first time in the vector must equal the start time;
        /// the following times must equal the payment times.
      /// </summary>
      /// <param name="times"></param>
      /// <param name="freq"></param>
      /// <returns></returns>
        public double parRate(List<double> times, Frequency freq)
        {
           return parRate(times, freq, false);
        }
        public double parRate(List<double> times)
        {
           return parRate(times, Frequency.Annual, false);
        }
        public double parRate(List<double> times, Frequency freq, bool extrapolate)
        {
           if ( times.Count < 2 )
              throw new Exception ("at least two times are required");

            checkRange(times[times.Count-1] , extrapolate);
            double sum = 0.0;
            for (int i =0; i<times.Count; i++)
                sum += discountImpl(times[i]);
            double result  = discountImpl(times[0])-discountImpl(times[times.Count-1]);
            result *= (double)freq/sum;
            return result;
        }

   }
}
