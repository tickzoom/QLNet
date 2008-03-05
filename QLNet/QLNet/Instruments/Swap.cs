using System;
using System.Collections.Generic;
using System.Text;

namespace QLNet
{
   /// <summary>
   /// Interest rate swap
   /// The cash flows belonging to the first leg are paid;
   /// the ones belonging to the second leg are received.
   /// </summary>
   public class Swap : Instrument 
   {
      protected List<Leg> legs_;
      protected List<double> payer_;
      protected List<double> legNPV_;
      protected List<double> legBPS_;

      /// <summary>
      /// The cash flows belonging to the first leg are paid;
      /// the ones belonging to the second leg are received.
      /// </summary>
      /// <param name="firstLeg"></param>
      /// <param name="secondLeg"></param>
      public Swap(Leg firstLeg, Leg secondLeg)
      {
         legs_ = new List<Leg>();
         legs_.Add(firstLeg); 
         legs_.Add(secondLeg);
         payer_ = new List<double>(2);
         payer_.Add(-1); 
         payer_.Add(1);
         legNPV_ = new List<double>(2);
         legBPS_ = new List<double>(2);
         
         foreach (CashFlow c in legs_[0] ) 
            registerWith(c); 
         foreach (CashFlow c in legs_[1])
            registerWith(c);

      }

      /// <summary>
      /// Multi leg constructor. 
      /// </summary>
      /// <param name="legs"></param>
      /// <param name="payer"></param>
      public Swap(List<Leg> legs, List<bool> payer)
      {
         legs_ = legs;
         payer_ = new List<double>(legs.Count);
         legBPS_ = new List<double>(legs.Count);
         legNPV_ = new List<double>(legs.Count);
         if (payer.Count != legs_.Count) 
            throw new Exception("size mismatch between payer (" + payer.Count + ") and legs (" + legs_.Count + ")");
         for (int j = 0; j < legs_.Count-1; ++j)
         {
            if (payer[j]) payer_[j] = -1.0; else payer_[j] = 1.0;
            foreach (CashFlow c in legs_[j])
               registerWith(c);
         }
      }
 
      public override bool isExpired() 
      {
         DDate today = Settings.Instance.evaluationDate() ;
         for (int j=0; j<legs_.Count-1; ++j) 
         {
            foreach (CashFlow c in legs_[j])
               if (c.hasOccurred(today))
                  return false;
         }
         return true;
      }

      protected override void setupExpired() 
      {
         base.setupExpired();
         legBPS_ = new List<double>(legBPS_.Count);
         legNPV_ = new List<double>(legNPV_.Count);
      }

      public void setupArguments(ref PricingEngine.arguments args)
      {
         Swap.arguments arguments = args as Swap.arguments;
         if (arguments == null) throw new Exception("wrong argument type");

         arguments.legs = legs_;
         arguments.payer = payer_;
      }

      public override void fetchResults(PricingEngine.results r)
      {
            base.fetchResults(r);
    
            Swap.results results = r as Swap.results;
            if (results ==  null) throw new ArgumentException("wrong result type");
    
            if (results.legNPV.Count != 0)
            {
               if (results.legNPV.Count != legNPV_.Count) 
                  throw new Exception("wrong number of leg NPV returned");
                legNPV_ = results.legNPV;
            }
            else
            {
               legNPV_ = new List<double>(legNPV_.Count);
            }
    
            if (results.legBPS.Count != 0 )
            {
               if (results.legBPS.Count != legBPS_.Count) 
                  throw new Exception("wrong number of leg BPS returned");
                legBPS_ = results.legBPS;
            }
            else
            {
               legBPS_ = new List<double>(legBPS_.Count);
            }
        }

      public DDate startDate()
      {
         if (legs_.Count == 0) 
            throw new Exception("no legs given");

         DDate d = CashFlows.startDate(legs_[0]);
         for (int j = 1; j < legs_.Count -1; ++j)
            d =  DDate.MIN(d , CashFlows.startDate(legs_[j]));
         return d;
      }

      public DDate maturityDate()
      {
         if (legs_.Count == 0)
            throw new Exception("no legs given");
         DDate d = CashFlows.maturityDate(legs_[0]);
         for (int j = 1; j < legs_.Count -1 ; ++j)
            d = DDate.MIN(d , CashFlows.maturityDate(legs_[j]));
         return d;
      }

      public double legBPS(int j)
      {
         if (j >= legs_.Count) 
            throw new Exception("leg# " + j + " doesn't exist!");
         calculate();
         return legBPS_[j];
      }
      public double legNPV(int j)
      {
         if (j >= legs_.Count) 
            throw new Exception("leg# " + j + " doesn't exist!");
         calculate();
         return legNPV_[j];
      }

      public Leg leg(int j)
      {
         if (j >= legs_.Count) 
            throw new Exception("leg# " + j + " doesn't exist!");
         return legs_[j];
      }

      public class arguments : PricingEngine.arguments
      {
         public List<Leg> legs;
         public List<double> payer;
         public override void validate()
         {
            if (legs.Count != payer.Count) throw new Exception("number of legs and multipliers differ");
         }

      }

      public new class results : Instrument.results
      {
         public List<double> legNPV;
         public List<double> legBPS;
         public override void reset()
         {
            base.reset();
            legNPV.Clear();
            legBPS.Clear();
         }
      }

   }
}
