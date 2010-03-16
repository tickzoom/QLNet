/*
 Copyright (C) 2008, 2009 , 2010  Andrea Maggiulli (a.maggiulli@gmail.com)
  
 This file is part of QLNet Project http://www.qlnet.org

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
using System.Linq;
using System.Text;

namespace QLNet
{
   public class Loan : Instrument
   {
      public enum Type { Receiver = -1, Payer = 1 };
      protected InitializedList<List<CashFlow>> legs_;
      protected InitializedList<double> payer_;

      public Loan(int legs)
      {
          legs_ = new InitializedList<List<CashFlow>>(legs);
          payer_ = new InitializedList<double>(legs);
      }

   }


   public class FixedLoan : Loan
   {
      private Type type_;
      private double nominal_;
      private Schedule fixedSchedule_;
      private double fixedRate_;
      private DayCounter fixedDayCount_;
      private Schedule principalSchedule_;
      private BusinessDayConvention paymentConvention_;

      public FixedLoan(Type type, double nominal,
                       Schedule fixedSchedule, double fixedRate, DayCounter fixedDayCount,
                       Schedule principalSchedule, BusinessDayConvention? paymentConvention) :
         base(2) 
      {

         type_ = type;
         nominal_ = nominal;
         fixedSchedule_ = fixedSchedule;
         fixedRate_ = fixedRate;
         fixedDayCount_ = fixedDayCount;
         principalSchedule_ = principalSchedule;

         if (paymentConvention.HasValue)
             paymentConvention_ = paymentConvention.Value;
         else
            paymentConvention_ = fixedSchedule_.businessDayConvention();

         List<CashFlow> fixedLeg = new FixedRateLeg(fixedSchedule, fixedDayCount)
                                     .withCouponRates(fixedRate)
                                     .withPaymentAdjustment(paymentConvention_)
                                     .withNotionals(nominal);

         List<CashFlow> principalLeg = new PricipalLeg(principalSchedule)
                                     .withNotionals(nominal)
                                     .withPaymentAdjustment(paymentConvention_);

         legs_[0] = fixedLeg;
         legs_[1] = principalLeg;
         if (type_ == Type.Payer) {
             payer_[0] = -1;
             payer_[1] = +1;
         } else {
             payer_[0] = +1;
             payer_[1] = -1;
         }
      }
   }

}
