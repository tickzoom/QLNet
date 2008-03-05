using System;
using System.Collections.Generic;
using System.Text;

namespace QLNet
{
   /// <summary>
   /// cashflow-analysis functions
   /// </summary>
   public class CashFlows
   {

      // constructors - private
      private CashFlows() {}
      private CashFlows(CashFlows c) {}

      public static CashFlow previousCashFlow(Leg leg)
      {
         return previousCashFlow(leg, new DDate());
      }


      public static CashFlow previousCashFlow(Leg leg, DDate refDate)
      {
         if (refDate == new DDate())
             refDate = Settings.Instance.evaluationDate();
    
         if (! leg[0].hasOccurred(refDate))
             return leg[leg.Count-1];
    
         CashFlow i = nextCashFlow(leg, refDate);

         DDate beforeLastPaymentDate = leg[leg.IndexOf(i)-1].date()-1;
         return nextCashFlow(leg, beforeLastPaymentDate);
      }

      public static CashFlow nextCashFlow(Leg leg)
      {
         return nextCashFlow(leg, new DDate());
      }


      public static CashFlow nextCashFlow(Leg leg, DDate refDate)
      {
         if (refDate == new DDate())
             refDate = Settings.Instance.evaluationDate();
 
         foreach ( CashFlow c in leg )
         {
            if ( c.hasOccurred (refDate) )
               return c;
         }

         return leg[leg.Count-1];
      }

      public static double previousCouponRate(Leg leg)
      {
         return previousCouponRate(leg, new DDate());
      }

      public static double previousCouponRate(Leg leg, DDate refDate)
      {
            CashFlow cf = previousCashFlow(leg, refDate);
            return couponRate(leg, cf);
      }

      public static double nextCouponRate(Leg leg)
      {
         return nextCouponRate(leg, new DDate());
      }
      
      public static double nextCouponRate(Leg leg, DDate refDate)
      {
            CashFlow cf = nextCashFlow(leg, refDate);
            return couponRate(leg, cf);
      }

      public static DDate startDate(Leg cashflows)
      {
         DDate d = DDate.maxDate();

         for (int i = 0; i < cashflows.Count; ++i)
         {
            Coupon c = (Coupon)cashflows[i];
            if (c != null)
               d = DDate.MIN(d , c.accrualStartDate());
         }
         if (d == DDate.maxDate())
            throw new Exception ("not enough information available");
         return d;
      }

      public static DDate maturityDate(Leg cashflows)
      {
         DDate d = DDate.minDate();
         for (int i = 0; i < cashflows.Count ; ++i)
            d = DDate.MIN(d , cashflows[i].date());
         if (d == DDate.maxDate())
            throw new Exception("no cashflows");
         return d;
      }

      /// <summary>
      /// NPV of the cash flows.
      /// The NPV is the sum of the cash flows, each discounted
      /// according to the given term structure.
      /// </summary>
      /// <param name="cashflows"></param>
      /// <param name="discountCurve"></param>
      /// <param name="settlementDate"></param>
      /// <param name="npvDate"></param>
      /// <returns></returns>
 
      public static double npv(Leg cashflows, YieldTermStructure discountCurve, DDate settlementDate, DDate npvDate)
      {
         return npv(cashflows, discountCurve, settlementDate, npvDate, 0);
      }
      public static double npv(Leg cashflows, YieldTermStructure discountCurve, DDate settlementDate)
      {
         return npv(cashflows, discountCurve, settlementDate, new DDate(), 0);
      }
      public static double npv(Leg cashflows, YieldTermStructure discountCurve)
      {
         return npv(cashflows, discountCurve, new DDate(), new DDate(), 0);
      }
      public static double npv(Leg cashflows, YieldTermStructure discountCurve, DDate settlementDate, DDate npvDate, int exDividendDays)
      {
         if (settlementDate == new DDate())
            settlementDate = discountCurve.referenceDate();

         double totalNPV = 0.0;
         for (int i = 0; i < cashflows.Count; ++i)
         {
            if (!cashflows[i].hasOccurred(settlementDate + exDividendDays))
               totalNPV += cashflows[i].amount() * discountCurve.discount(cashflows[i].date());
         }

         if (npvDate == new DDate())
            return totalNPV;
         else
            return totalNPV / discountCurve.discount(npvDate);
      }

      // utility functions

      public static double couponRate(Leg leg, CashFlow  cf)
      {
         if (cf == leg[leg.Count -1])
            return 0.0;

         DDate paymentDate = cf.date();
         bool firstCouponFound = false;
         double nominal = 0;
         double accrualPeriod = 0;
         DayCounter dc = null ;
         double result = 0.0;
         for (int i = leg.IndexOf(cf); i < leg.Count - 1 && leg[i].date() == paymentDate; ++i)
         {
            Coupon cp = (Coupon)(leg[i]);
             if (cp != null)
             {
                 if (firstCouponFound)
                 {
                     if (nominal != cp.nominal() || accrualPeriod != cp.accrualPeriod() || dc != cp.dayCounter()) 
                        throw new Exception ("cannot aggregate two different coupons on " + paymentDate);
                 }
                 else
                 {
                     firstCouponFound = true;
                     nominal = cp.nominal();
                     accrualPeriod = cp.accrualPeriod();
                     dc = cp.dayCounter();
                 }
                 result += cp.rate();
             }
         }
         if (!firstCouponFound) 
            throw new Exception ("next cashflow (" + paymentDate + ") is not a coupon");

         return result;
      }


   }
}
