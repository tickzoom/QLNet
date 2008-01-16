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
   public class SimpleDayCounter : DayCounter
   {
      private new class Impl : DayCounter.Impl
      {
         public override string name() { return "Simple"; }
         public override int dayCount(DDate d1, DDate d2)
         {
            return fallback.dayCount(d1, d2);
         }
         public override double yearFraction(DDate d1, DDate d2, DDate Start, DDate End)
         {
            int dm1 = d1.dayOfMonth(),
                dm2 = d2.dayOfMonth();

            if (dm1 == dm2 ||
               // e.g., Aug 30 -> Feb 28 ?
               (dm1 > dm2 && DDate.isEndOfMonth(d2)) ||
               // e.g., Feb 28 -> Aug 30 ?
               (dm1 < dm2 && DDate.isEndOfMonth(d1))) 
            {

               return (d2.year()-d1.year()) +
                      ((int)(d2.month())-(int)(d1.month()))/12.0;

            } 
            else 
            {
               return fallback.yearFraction(d1,d2,Start,End);
            }
         }
      };

      private static DayCounter fallback = new Thirty360(); 

      public SimpleDayCounter()
         : base(new SimpleDayCounter.Impl()) { }
   }
}
