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
   public class SimpleQuote : Quote
   {
      private Nullable<double> value_;

      public SimpleQuote()
         : this(new Nullable<double>())
      {
      }

      public SimpleQuote(Nullable<double> value)
      {
         value_ = value;
      }

      public override double value()
      {
         if (!isValid())
            throw new Exception("invalid SimpleQuote");
         return (double)value_;
      }
      
      public override bool isValid()
      {
         return value_ != new Nullable<double>();
      }

      //@}
      //! \name Modifiers
      //@{
      //! returns the difference between the new value and the old value
      public double setValue()
      {
         return setValue(new Nullable<double>());
      }
      public double setValue(Nullable<double> value)
      {
         double diff = (double)(value - value_);
         if (diff != 0.0)
         {
            value_ = value;
            notifyObservers();
         }
         return diff;
      }
      public void reset()
      {
         setValue(new Nullable<double>());
      }
   }
}
