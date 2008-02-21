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
   /// Predetermined cash flow
   /// This cash flow pays a predetermined amount at a given date.
   /// </summary>
   public class SimpleCashFlow : CashFlow
   {
      private double amount_;
      private DDate date_;

      public SimpleCashFlow(double amount, DDate date)
      {
         amount_ = amount;
         date_ = date;
      }

      /// <summary>
      /// Event interface
      /// </summary>
      /// <returns></returns>
      public override DDate date()
      {
         return date_;
      }

      /// <summary>
      /// CashFlow interface
      /// </summary>
      /// <returns></returns>
      public override double amount()
      {
         return amount_;
      }

      public override void accept(ref AcyclicVisitor v)
      {
         //Visitor<SimpleCashFlow> v1 = v as Visitor<SimpleCashFlow>;
         //if (v1 != 0)
         //   v1.visit(this);
         //else
         //   CashFlow.accept(ref v);
      }

   }
}
