/*
 This file is part of QLNet Project http://trac2.assembla.com/QLNet
 
 QLNet is a porting of QuantLib, a free-software/open-source library
 for financial quantitative analysts and developers - http://quantlib.org/
 The license is available online at http://quantlib.org/license.shtml.
 
 This program is distributed in the hope that it will be useful, but WITHOUT
 ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 FOR A PARTICULAR PURPOSE.  See the license for more details.
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace QLNet
{
   public class DayCounter
   {
      //! abstract base class for day counter implementations
      protected class Impl 
      {
         ~Impl() {}
         public virtual string name() { throw new System.NotImplementedException(); }
         //! to be overloaded by more complex day counters
         public virtual int dayCount(DDate d1,DDate d2) 
         {
            return (d2-d1);
         }
         public virtual double yearFraction(DDate d1, DDate d2, DDate refPeriodStart, DDate refPeriodEnd) 
         { throw new System.NotImplementedException(); }
      };
      protected Impl _impl;

      /// <summary>
      /// This constructor can be invoked by derived classes which
      /// define a given implementation.
      /// </summary>
      /// <param name="impl"></param>
      protected DayCounter(Impl impl) 
      {
         _impl = impl ;
      }

      /// <summary>
      /// The default constructor returns a day counter with a null
      /// implementation, which is therefore unusable except as a
      /// placeholder.
      /// </summary>
      public DayCounter() { }

      /// <summary>
      /// DayCounter interface
      /// </summary>
      /// <returns>
      /// Returns whether or not the day counter is initialized
      /// </returns>
      public bool empty() 
      {
         return _impl == null;
      }
      /// <summary>
      /// Returns the name of the day counter.
      /// This method is used for output and comparison between
      /// day counters. It is <b>not</b> meant to be used for writing
      /// switch-on-type code.
      /// </summary>
      /// <returns></returns>
      public string name() 
      {
        if ( _impl == null ) 
          throw new Exception ("no implementation provided");
        return _impl.name();
      }
      /// <summary>
      /// Returns the number of days between two dates.
      /// </summary>
      /// <param name="d1">StartDate</param>
      /// <param name="d2">EndDate</param>
      /// <returns></returns>
      public int dayCount(DDate d1,DDate d2) 
      {
         if (_impl == null)
            throw new Exception("no implementation provided");
        return _impl.dayCount(d1,d2);
      }
      public double yearFraction(DDate d1, DDate d2)
      {
         return yearFraction(d1, d2, new DDate(), new DDate());
      }

      /// <summary>
      /// Returns the period between two dates as a fraction of year.
      /// </summary>
      /// <param name="d1"></param>
      /// <param name="d2"></param>
      /// <param name="refPeriodStart"></param>
      /// <param name="refPeriodEnd"></param>
      /// <returns></returns>
      public double yearFraction(DDate d1,DDate d2,DDate refPeriodStart,DDate refPeriodEnd)
      {
         if (_impl == null)
            throw new Exception("no implementation provided");
         return _impl.yearFraction(d1, d2, refPeriodStart, refPeriodEnd);
      }
      /// <summary>
      /// Returns <tt>true</tt> iff the two day counters belong to the same
      /// derived class.
      /// </summary>
      /// <param name="d1"></param>
      /// <param name="d2"></param>
      /// <returns></returns>
      public static bool operator==(DayCounter d1, DayCounter d2) 
      {
        return (d1.empty() && d2.empty())
            || (!d1.empty() && !d2.empty() && d1.name() == d2.name());
      }
      /// <summary>
      /// Returns <tt>true</tt> iff the two day counters not belong to the same
      /// derived class.
      /// </summary>
      /// <param name="d1"></param>
      /// <param name="d2"></param>
      /// <returns></returns>
      public static bool operator!=(DayCounter d1, DayCounter d2) 
      {
        return !(d1 == d2);
      }

   }
}
