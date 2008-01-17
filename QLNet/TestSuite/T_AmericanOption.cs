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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QLNet;

namespace TestSuite
{

   public struct AmericanOptionData
   {
      public Option.Type type;
      public double strike;
      public double s; // spot
      public double q; // dividend
      public double r; // risk-free rate
      public double t; // time to maturity
      public double v; // volatility
      public double result; // expected result
      public AmericanOptionData(Option.Type type_,
                         double strike_,
                         double s_,
                         double q_,
                         double r_,
                         double t_,
                         double v_,
                         double result_)
      {
         type = type_;
         strike= strike_;
         s = s_;
         q = q_;
         r = r_;
         t = t_;
         v = v_;
         result = result_;
      }
   }

   [TestClass()]
   class T_AmericanOption
   {
  		public static YieldTermStructure flatRate(DDate today, Quote forward, DayCounter dc)
		{
			//return new YieldTermStructure(new FlatForward(today, new Handle<Quote>(forward), dc));
         return null;
      }

		public static YieldTermStructure flatRate(DDate today, double forward, DayCounter dc)
		{
			//return flatRate(today, boost.shared_ptr<Quote>(new SimpleQuote(forward)), dc);
         return null;
      }

      [TestMethod()]
      public void testBaroneAdesiWhaleyValues()
      {
         AmericanOptionData[] values = {
            new AmericanOptionData(Option.Type.Call, 100.00,  90.00, 0.10, 0.10, 0.10, 0.15,  0.0206) ,
            new AmericanOptionData(Option.Type.Call, 100.00, 100.00, 0.10, 0.10, 0.10, 0.15,  1.8771) ,
            new AmericanOptionData(Option.Type.Call, 100.00, 110.00, 0.10, 0.10, 0.10, 0.15, 10.0089) ,
            new AmericanOptionData(Option.Type.Call, 100.00,  90.00, 0.10, 0.10, 0.10, 0.25,  0.3159) ,
            new AmericanOptionData(Option.Type.Call, 100.00, 100.00, 0.10, 0.10, 0.10, 0.25,  3.1280) ,
            new AmericanOptionData(Option.Type.Call, 100.00, 110.00, 0.10, 0.10, 0.10, 0.25, 10.3919) ,
            new AmericanOptionData(Option.Type.Call, 100.00,  90.00, 0.10, 0.10, 0.10, 0.35,  0.9495) ,
            new AmericanOptionData(Option.Type.Call, 100.00, 100.00, 0.10, 0.10, 0.10, 0.35,  4.3777) ,
            new AmericanOptionData(Option.Type.Call, 100.00, 110.00, 0.10, 0.10, 0.10, 0.35, 11.1679) ,
            new AmericanOptionData(Option.Type.Call, 100.00,  90.00, 0.10, 0.10, 0.50, 0.15,  0.8208) ,
            new AmericanOptionData(Option.Type.Call, 100.00, 100.00, 0.10, 0.10, 0.50, 0.15,  4.0842) ,
            new AmericanOptionData(Option.Type.Call, 100.00, 110.00, 0.10, 0.10, 0.50, 0.15, 10.8087) ,
            new AmericanOptionData(Option.Type.Call, 100.00,  90.00, 0.10, 0.10, 0.50, 0.25,  2.7437) ,
            new AmericanOptionData(Option.Type.Call, 100.00, 100.00, 0.10, 0.10, 0.50, 0.25,  6.8015) ,
            new AmericanOptionData(Option.Type.Call, 100.00, 110.00, 0.10, 0.10, 0.50, 0.25, 13.0170) ,
            new AmericanOptionData(Option.Type.Call, 100.00,  90.00, 0.10, 0.10, 0.50, 0.35,  5.0063) ,
            new AmericanOptionData(Option.Type.Call, 100.00, 100.00, 0.10, 0.10, 0.50, 0.35,  9.5106) ,
            new AmericanOptionData(Option.Type.Call, 100.00, 110.00, 0.10, 0.10, 0.50, 0.35, 15.5689) ,
            new AmericanOptionData(Option.Type.Put,  100.00,  90.00, 0.10, 0.10, 0.10, 0.15, 10.0000) ,
            new AmericanOptionData(Option.Type.Put,  100.00, 100.00, 0.10, 0.10, 0.10, 0.15,  1.8770) ,
            new AmericanOptionData(Option.Type.Put,  100.00, 110.00, 0.10, 0.10, 0.10, 0.15,  0.0410) ,
            new AmericanOptionData(Option.Type.Put,  100.00,  90.00, 0.10, 0.10, 0.10, 0.25, 10.2533) ,
            new AmericanOptionData(Option.Type.Put,  100.00, 100.00, 0.10, 0.10, 0.10, 0.25,  3.1277) ,
            new AmericanOptionData(Option.Type.Put,  100.00, 110.00, 0.10, 0.10, 0.10, 0.25,  0.4562) ,
            new AmericanOptionData(Option.Type.Put,  100.00,  90.00, 0.10, 0.10, 0.10, 0.35, 10.8787) ,
            new AmericanOptionData(Option.Type.Put,  100.00, 100.00, 0.10, 0.10, 0.10, 0.35,  4.3777) ,
            new AmericanOptionData(Option.Type.Put,  100.00, 110.00, 0.10, 0.10, 0.10, 0.35,  1.2402) ,
            new AmericanOptionData(Option.Type.Put,  100.00,  90.00, 0.10, 0.10, 0.50, 0.15, 10.5595) ,
            new AmericanOptionData(Option.Type.Put,  100.00, 100.00, 0.10, 0.10, 0.50, 0.15,  4.0842) ,
            new AmericanOptionData(Option.Type.Put,  100.00, 110.00, 0.10, 0.10, 0.50, 0.15,  1.0822) ,
            new AmericanOptionData(Option.Type.Put,  100.00,  90.00, 0.10, 0.10, 0.50, 0.25, 12.4419) ,
            new AmericanOptionData(Option.Type.Put,  100.00, 100.00, 0.10, 0.10, 0.50, 0.25,  6.8014) ,
            new AmericanOptionData(Option.Type.Put,  100.00, 110.00, 0.10, 0.10, 0.50, 0.25,  3.3226) ,
            new AmericanOptionData(Option.Type.Put,  100.00,  90.00, 0.10, 0.10, 0.50, 0.35, 14.6945) ,
            new AmericanOptionData(Option.Type.Put,  100.00, 100.00, 0.10, 0.10, 0.50, 0.35,  9.5104) ,
            new AmericanOptionData(Option.Type.Put,  100.00, 110.00, 0.10, 0.10, 0.50, 0.35,  5.8823)};

          DDate today = DDate.todaysDate();
          DayCounter dc = new Actual360();
          SimpleQuote spot = new SimpleQuote(0.0);
          SimpleQuote qRate = new SimpleQuote(0.0);
          YieldTermStructure qTS = flatRate(today, qRate, dc);



      }
   }
}
