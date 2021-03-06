﻿/*
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

namespace QLNet
{
	public class DiscountingLoanLoanPricingEngine : LoanPricingEngine
	{
		private readonly Handle<YieldTermStructure> _discountCurve;

		public Handle<YieldTermStructure> discountCurve()
		{
			return _discountCurve;
		}

		public DiscountingLoanLoanPricingEngine(Handle<YieldTermStructure> discountCurve)
		{
			_discountCurve = discountCurve;
			_discountCurve.registerWith(update);
		}

		public override void calculate()
		{
			if (_discountCurve.IsEmpty)
			{
				throw new ArgumentException("no discounting term structure set");
			}

			results_.value = 0;
			results_.cash = 0;
			results_.errorEstimate = null;
			results_.legNPV.Clear();
			

			for (int i = 0; i < arguments_.legs.Count; ++i)
			{
				results_.legNPV[i] = arguments_.payer[i] * CashFlows.npv(arguments_.legs[i], _discountCurve);
				results_.value += results_.legNPV[i];
				results_.cash += arguments_.payer[i] * CashFlows.cash(arguments_.legs[i]);
			}
		}
	}
}
