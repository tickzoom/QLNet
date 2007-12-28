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
   public class Business252 : DayCounter
   {
      private new class Impl : DayCounter.Impl
      {
         private Calendar _calendar;
         public Impl(Calendar c) { _calendar = c; }
         public override string name() { return "Business/252(" + _calendar.name() + ")"; }
         public override int dayCount(DDate d1,DDate d2)
         {
            return _calendar.businessDaysBetween(d1, d2);
         }

         public override double yearFraction(DDate d1, DDate d2, DDate Start, DDate End)
         {
            return dayCount(d1, d2) / 252.0;
         }
      };

      public Business252(Calendar c)
         : base(new Business252.Impl(c)) {}
   }
}
