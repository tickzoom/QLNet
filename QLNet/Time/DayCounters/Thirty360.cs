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
   public class Thirty360 : DayCounter
   {
      public enum Convention 
      { 
         USA, 
         BondBasis,
         European, 
         EurobondBasis,
         Italian 
      };

      private class US_Impl : DayCounter.Impl
      {
         public override string name() { return "30/360 (Bond Basis)"; }
         public override int dayCount(DDate d1, DDate d2)
         {
            int dd1 = d1.dayOfMonth(), dd2 = d2.dayOfMonth();
            Month mm1 = d1.month(), mm2 = d2.month();
            int yy1 = d1.year(), yy2 = d2.year();

            if (dd2 == 31 && dd1 < 30) { dd2 = 1; mm2++; }

            return 360*(yy2-yy1) + 30*(mm2-mm1-1) +
                   Math.Max((int)(0),30-dd1) + Math.Min((int)(30),dd2);            
         }
         public override double yearFraction(DDate d1, DDate d2, DDate Start, DDate End)
         {
            return dayCount(d1, d2)/360.0;
         }
      };

      private class EU_Impl : DayCounter.Impl
      {
         public override string name() { return "30E/360 (Eurobond Basis)"; }
         public override int dayCount(DDate d1, DDate d2)
         {
            int dd1 = d1.dayOfMonth(), dd2 = d2.dayOfMonth();
            Month mm1 = d1.month(), mm2 = d2.month();
            int yy1 = d1.year(), yy2 = d2.year();

            return 360*(yy2-yy1) + 30*(mm2-mm1-1) +
                   Math.Max((int)(0),30-dd1) + Math.Min((int)(30),dd2);
         }
         public override double yearFraction(DDate d1, DDate d2, DDate Start, DDate End)
         {
            return dayCount(d1, d2) / 360.0;
         }
      };

      private class IT_Impl : DayCounter.Impl
      {
         public override string name() { return "30/360 (Italian)"; }
         public override int dayCount(DDate d1, DDate d2)
         {
            int dd1 = d1.dayOfMonth(), dd2 = d2.dayOfMonth();
            Month mm1 = d1.month(), mm2 = d2.month();
            int yy1 = d1.year(), yy2 = d2.year();

            if (mm1 == Month.Feb && dd1 > 27) dd1 = 30;
            if (mm2 == Month.Feb && dd2 > 27) dd2 = 30;

            return 360*(yy2-yy1) + 30*(mm2-mm1-1) +
                   Math.Max((int)(0),30-dd1) + Math.Min((int)(30),dd2);
         }
         public override double yearFraction(DDate d1, DDate d2, DDate Start, DDate End)
         {
            return dayCount(d1, d2) / 360.0;
         }
      };

      private static DayCounter.Impl implementation(Convention c)
      {
         switch (c)
         {
            case Convention.USA:
            case Convention.BondBasis:
               return new US_Impl();
            case Convention.European:
            case Convention.EurobondBasis:
               return new EU_Impl();
            case Convention.Italian:
               return new IT_Impl();
            default:
               throw new Exception("unknown 30/360 convention");
         }

      }

      public Thirty360()
         : this(Thirty360.Convention.BondBasis) { }

      public Thirty360(Convention c)
         : base(implementation(c)) { }

   }
}
