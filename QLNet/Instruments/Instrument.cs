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
   /// Abstract instrument class
   /// This class is purely abstract and defines the interface of concrete
   /// instruments which will be derived from this one.
   /// Observability of class instances is checked.
   /// </summary>
   public class Instrument : LazyObject
   {
      public class results : PricingEngine.results
      {
         public Nullable<double> value;
         public Nullable<double> errorEstimate;
         public Dictionary<string, object> additionalResults;

         public override void reset()
         {
            value = errorEstimate = null;
            additionalResults.Clear();
         }

      }

      protected Nullable<double> NPV_, errorEstimate_;
      protected Dictionary<string,object> additionalResults_;
      protected PricingEngine engine_;

      public Instrument()
      {
         NPV_ = null;
         errorEstimate_ = null;
      }

      /// <summary>
      /// Set the pricing engine to be used.
      /// <remarks>
      /// Calling this method will have no effects in
      /// case the <b>performCalculation</b> method
      /// was overridden in a derived class.
      /// </remarks>
      /// </summary>
      /// <param name="e"></param>
      public void setPricingEngine(PricingEngine e) 
      {
        if (engine_ != null)
            unregisterWith(engine_);
        engine_ = e;
        if (engine_ !=null)
            registerWith(engine_);
        // trigger (lazy) recalculation and notify observers
        update();
   
      }
     
      /// <summary>
      /// When a derived argument structure is defined for an
      /// instrument, this method should be overridden to fill
      /// it. This is mandatory in case a pricing engine is used.
      /// </summary>
      /// <param name="a"></param>
      public virtual void setupArguments(PricingEngine.arguments a) 
      {
         throw new Exception("Instrument.setupArguments() not implemented");
      }

      /// <summary>
      /// Returns whether the instrument is still tradable.
      /// </summary>
      /// <returns></returns>
      public virtual bool isExpired()
      {
         throw new Exception("Instrument.isExpired not implemented");
      }

      /// <summary>
      /// Calculations
      /// </summary>
      protected override void calculate() 
      {
        if (isExpired()) 
        {
            setupExpired();
            _calculated = true;
        } 
        else 
        {
            base.calculate();
        }
      }

      /// <summary>
      /// This method must leave the instrument in a consistent
      /// state when the expiration condition is met.
      /// </summary>
      protected virtual void setupExpired()
      {
         NPV_ = errorEstimate_ = 0.0;
         additionalResults_.Clear();
      }

      /// <summary>
      /// In case a pricing engine is <b>not</b> used, this
      /// method must be overridden to perform the actual
      /// calculations and set any needed results. In case
      /// a pricing engine is used, the default implementation
      /// can be used.
      /// </summary>
      protected override void performCalculations()
      {
         if ( engine_== null) throw new Exception ("null pricing engine");
         engine_.reset();
         setupArguments(engine_.getArguments());
         engine_.getArguments().validate();
         engine_.calculate();
         fetchResults(engine_.getResults());
      }

      public virtual void fetchResults(PricingEngine.results r) 
      {
         Instrument.results results = (Instrument.results) r ; //dynamic_cast<const Instrument::results*>(r);
         if (results == null) throw new Exception("no results returned from pricing engine");
         NPV_ = results.value;
         errorEstimate_ = results.errorEstimate;
         additionalResults_ = results.additionalResults;
      }

      /// <summary>
      /// Returns the net present value of the instrument.
      /// </summary>
      /// <returns></returns>
      public double NPV() 
      {
        calculate();
        if ( ! NPV_.HasValue ) throw new Exception("NPV not provided");
        return NPV_.Value;
      }

      /// <summary>
      /// Returns the error estimate on the NPV when available.
      /// </summary>
      /// <returns></returns>
      public double errorEstimate() 
      {
        calculate();
        if (!errorEstimate_.HasValue) throw new Exception("Error estimate not provided");
        return errorEstimate_.Value;
      }
      /// <summary>
      /// Returns all additional result returned by the pricing engine.
      /// </summary>
      /// <returns></returns>
      public Dictionary<string, object> additionalResults()
      {
         return additionalResults_;
      }
   }
}
