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
   public class OneDayCounter : DayCounter
   {
      private new class Impl : DayCounter.Impl
      {
         public override string name() { return "1/1"; }
         public override int dayCount(DDate d1,DDate d2) 
         {
                // the sign is all we need
                return (d2 >= d1 ? 1 : -1);
         }
         public override double yearFraction(DDate d1, DDate d2, DDate Start, DDate End)
         {
            return (double)dayCount(d1, d2);
         }
      };

      public OneDayCounter() 
         : base(new OneDayCounter.Impl()) {}
   }
}
