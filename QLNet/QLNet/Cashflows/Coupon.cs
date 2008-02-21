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
   /// Coupon accruing over a fixed period
   /// This class implements part of the CashFlow interface but it is
   /// still abstract and provides derived classes with methods for
   /// accrual period calculations.
   /// </summary>
   public class Coupon : CashFlow
   {
      protected double nominal_;
      protected DDate paymentDate_;
      protected DDate accrualStartDate_;
      protected DDate accrualEndDate_;
      protected DDate refPeriodStart_;
      protected DDate refPeriodEnd_;

      public Coupon(double nominal, DDate paymentDate, DDate accrualStartDate, DDate accrualEndDate, DDate refPeriodStart)
         : this(nominal, paymentDate, accrualStartDate, accrualEndDate, refPeriodStart, new DDate()) {}
      
      public Coupon(double nominal, DDate paymentDate, DDate accrualStartDate, DDate accrualEndDate)
         : this(nominal, paymentDate, accrualStartDate, accrualEndDate, new DDate(), new DDate()) {}
      
      public Coupon(double nominal, DDate paymentDate, DDate accrualStartDate, DDate accrualEndDate, DDate refPeriodStart, DDate refPeriodEnd)
      {
         nominal_ = nominal;
         paymentDate_ = paymentDate;
         accrualStartDate_ = accrualStartDate;
         accrualEndDate_ = accrualEndDate;
         refPeriodStart_ = refPeriodStart;
         refPeriodEnd_ = refPeriodEnd;
         if (refPeriodStart_ == new DDate())
            refPeriodStart_ = accrualStartDate_;
         if (refPeriodEnd_ == new DDate())
            refPeriodEnd_ = accrualEndDate_;
      }

      // inline definitions

      public override DDate date()
      {
         return paymentDate_;
      }

      public double nominal()
      {
         return nominal_;
      }

      /// <summary>
      /// Start of the accrual period
      /// </summary>
      /// <returns></returns>
      public DDate accrualStartDate()
      {
         return accrualStartDate_;
      }
      
      /// <summary>
      /// End of the accrual period
      /// </summary>
      /// <returns></returns>
      public DDate accrualEndDate()
      {
         return accrualEndDate_;
      }

      /// <summary>
      /// Start date of the reference period
      /// </summary>
      /// <returns></returns>
      public DDate referencePeriodStart()
      {
         return refPeriodStart_;
      }

      /// <summary>
      /// End date of the reference period
      /// </summary>
      /// <returns></returns>
      public DDate referencePeriodEnd()
      {
         return refPeriodEnd_;
      }

      /// <summary>
      /// Accrual period as fraction of year
      /// </summary>
      /// <returns></returns>
      public double accrualPeriod()
      {
         return dayCounter().yearFraction(accrualStartDate_, accrualEndDate_, refPeriodStart_, refPeriodEnd_);
      }

      /// <summary>
      /// accrual period in days
      /// </summary>
      /// <returns></returns>
      public int accrualDays()
      {
         return dayCounter().dayCount(accrualStartDate_, accrualEndDate_);
      }

      //! accrued rate
      public virtual double rate() 
      { throw new Exception("The method or operation is not implemented."); }
      //! day counter for accrual calculation
      public virtual DayCounter dayCounter() 
      { throw new Exception("The method or operation is not implemented."); }
      //! accrued amount at the given date
      public virtual double accruedAmount(DDate NamelessParameter) 
      { throw new Exception("The method or operation is not implemented."); }

      /// <summary>
      /// Visitability
      /// </summary>
      /// <param name="v"></param>
      public override void accept(ref AcyclicVisitor v)
      {
         //Visitor<Coupon> v1 = v as Visitor<Coupon>;
         //if (v1 != 0)
         //   v1.visit(this);
         //else
         //   CashFlow.accept(ref v);
      }

   }
}
