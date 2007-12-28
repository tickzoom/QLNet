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
   /// <summary>
   /// Actual/Actual day count
   /// The day count can be calculated according to:
   /// - The ISDA convention, also known as "Actual/Actual (Historical)",
   ///   "Actual/Actual", "Act/Act", and according to ISDA also "Actual/365",
   ///   "Act/365", and "A/365";
   /// - The ISMA and US Treasury convention, also known as "Actual/Actual (Bond)"
   /// - The AFB convention, also known as "Actual/Actual (Euro)".
   /// </summary>
   public class ActualActual : DayCounter
   {
      public enum Convention 
      { ISMA, 
        Bond,
        ISDA, 
        Historical,
        Actual365,
        AFB,
        Euro 
      };

      private class ISMA_Impl : DayCounter.Impl
      {
         public override string name() { return "Actual/Actual (ISMA)"; }
         public override double yearFraction(DDate d1, DDate d2, DDate d3, DDate d4)
         {
            if (d1 == d2)
               return 0.0;

            if (d1 > d2)
               return -yearFraction(d2,d1,d3,d4);

            // when the reference period is not specified, try taking
            // it equal to (d1,d2)
            DDate refPeriodStart = (d3 != new DDate() ? d3 : d1);
            DDate refPeriodEnd = (d4 != new DDate() ? d4 : d2);

            if (refPeriodEnd <=  refPeriodStart || refPeriodEnd <= d1 )
               throw new Exception ( "invalid reference period: " +
                                     "date 1: " + d1 +
                                     ", date 2: " + d2 +
                                     ", reference period start: " + refPeriodStart +
                                     ", reference period end: " + refPeriodEnd);

            // estimate roughly the length in months of a period
            int months = (int)(0.5+12*(double)(refPeriodEnd-refPeriodStart)/365);

            // for short periods...
            if (months == 0) 
            {
               // ...take the reference period as 1 year from d1
               refPeriodStart = d1;
               refPeriodEnd = d1 + TimeUnit.Years;
               months = 12;
            }

            double period = (double)months/12.0;

            if (d2 <= refPeriodEnd) 
            {
               // here refPeriodEnd is a future (notional?) payment date
               if (d1 >= refPeriodStart) 
               {
                  // here refPeriodStart is the last (maybe notional)
                  // payment date.
                  // refPeriodStart <= d1 <= d2 <= refPeriodEnd
                  // [maybe the equality should be enforced, since
                  // refPeriodStart < d1 <= d2 < refPeriodEnd
                  // could give wrong results] ???
                  return period* (dayCount(d1,d2)) / dayCount(refPeriodStart,refPeriodEnd);
               } 
               else 
               {
                  // here refPeriodStart is the next (maybe notional)
                  // payment date and refPeriodEnd is the second next
                  // (maybe notional) payment date.
                  // d1 < refPeriodStart < refPeriodEnd
                  // AND d2 <= refPeriodEnd
                  // this case is long first coupon

                  // the last notional payment date
                  DDate previousRef = refPeriodStart - new Period(months,TimeUnit.Months);
                  if (d2 > refPeriodStart)
                    return yearFraction(d1, refPeriodStart, previousRef,refPeriodStart) +
                           yearFraction(refPeriodStart, d2, refPeriodStart,refPeriodEnd);
                  else
                    return yearFraction(d1,d2,previousRef,refPeriodStart);
               }
            } 
            else 
            {
               // here refPeriodEnd is the last (notional?) payment date
               // d1 < refPeriodEnd < d2 AND refPeriodStart < refPeriodEnd
               if (refPeriodStart>d1) 
                  throw new Exception ("invalid dates: d1 < refPeriodStart < refPeriodEnd < d2");
               // now it is: refPeriodStart <= d1 < refPeriodEnd < d2
               // the part from d1 to refPeriodEnd
               double sum = yearFraction(d1, refPeriodEnd,refPeriodStart, refPeriodEnd);

               // the part from refPeriodEnd to d2
               // count how many regular periods are in [refPeriodEnd, d2],
               // then add the remaining time
               int i=0;
               DDate newRefStart, newRefEnd;
               do 
               {
                  newRefStart = refPeriodEnd + new Period ( (months * i) ,TimeUnit.Months);
                  newRefEnd = refPeriodEnd + new Period( (months * (i + 1)) , TimeUnit.Months);
                  if (d2 < newRefEnd) 
                  {
                     break;
                  } 
                  else 
                  {
                     sum += period;
                     i++;
                  } 
               } 
               while (true);
               sum += yearFraction(newRefStart,d2,newRefStart,newRefEnd);
               return sum;
            }
         }
      };
      private class ISDA_Impl : DayCounter.Impl
      {
         public override string name() { return "Actual/Actual (ISDA)"; }
         public override double yearFraction(DDate d1, DDate d2, DDate Start, DDate End)
         {
           if (d1 == d2)
               return 0;

           if (d1 > d2)
               return -yearFraction(d2,d1,new DDate(),new DDate());

           int y1 = d1.year(), y2 = d2.year();
           double dib1 = (DDate.isLeap(y1) ? 366.0 : 365.0),
                dib2 = (DDate.isLeap(y2) ? 366.0 : 365.0);

           double sum = y2 - y1 - 1;
           // FLOATING_POINT_EXCEPTION
           sum += dayCount(d1, new DDate(1,Month.January,y1+1))/dib1;
           sum += dayCount(new DDate(1,Month.January,y2),d2)/dib2;
           return sum;
         }
      };
      private class AFB_Impl : DayCounter.Impl
      {
         public override string name() { return "Actual/Actual (AFB)"; }
         public override double yearFraction(DDate d1, DDate d2, DDate Start, DDate End)
         {
            if (d1 == d2)
               return 0.0;

            if (d1 > d2)
               return -yearFraction(d2,d1,new DDate(),new DDate());

            DDate newD2=d2, temp=d2;
            double sum = 0.0;
            while (temp > d1) 
            {
               temp = newD2 - TimeUnit.Years;
               if (temp.dayOfMonth()==28 && (int)temp.month() == 2
                   && DDate.isLeap(temp.year())) 
               {
                  temp += 1;
               }
               if (temp>=d1) 
               {
                  sum += 1.0;
                  newD2 = temp;
               }
            }

            double den = 365.0;

            if (DDate.isLeap(newD2.year())) 
            {
               temp = new DDate(29, Month.February, newD2.year());
               if (newD2>temp && d1<=temp)
                  den += 1.0;
            } 
            else if (DDate.isLeap(d1.year())) 
            {
               temp = new DDate(29, Month.February, d1.year());
               if (newD2>temp && d1<=temp)
                  den += 1.0;
            }

            return sum+dayCount(d1, newD2)/den;         }
      };

      private static DayCounter.Impl implementation(Convention c)
      {
         switch (c)
         {
            case Convention.ISMA:
            case Convention.Bond:
               return new ISMA_Impl();
            case Convention.ISDA:
            case Convention.Historical:
            case Convention.Actual365:
               return new ISDA_Impl();
            case Convention.AFB:
            case Convention.Euro:
               return new AFB_Impl();
            default:
               throw new Exception("unknown act/act convention");
         }

      }

      public ActualActual()
         : this(ActualActual.Convention.ISDA) {}


      public ActualActual(Convention c)
         : base(implementation(c)) {}


   }
}
