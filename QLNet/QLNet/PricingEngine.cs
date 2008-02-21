using System;
using System.Collections.Generic;
using System.Text;

namespace QLNet
{
   public abstract class PricingEngine : Observable
   {
      ~PricingEngine() {}
      public abstract arguments getArguments();
      public abstract results getResults();
      public abstract void reset();
      public virtual void calculate()
      {
         throw new Exception("The method or operation is not implemented.");
      }

      public abstract class arguments 
      {
        ~ arguments() {}
         public abstract void validate();
      }

      public abstract class results
      {
         ~results() { }
         public abstract void reset();
      }

   }

   public class GenericEngine<ArgumentsType, ResultsType> : PricingEngine, IObserver
      where ArgumentsType : PricingEngine.arguments
      where ResultsType : PricingEngine.results

   {
      protected ArgumentsType arguments_;
      protected ResultsType results_;

      public override PricingEngine.arguments getArguments()
      {
         return arguments_;
      }
      public override PricingEngine.results getResults()
      {
         return results_;
      }
      public override void reset()
      {
         results_.reset();
      }
      public void update()
      {
         notifyObservers();
      }

      #region IObserver Membri di

      private List<IObservable> _observables = new List<IObservable>();

      public void registerWith(IObservable o)
      {
         if (o != null)
         {
            _observables.Add(o);
            o.registerObserver(this);
         }
      }

      public void unregisterWith(IObservable o)
      {
         if (o != null)
         {
            foreach (IObservable i in _observables)
            {
               if (i == o)
               {
                  i.unregisterObserver(this);
                  _observables.Remove(i);
                  return;
               }
            }
         }
      }

      #endregion
   }
}
