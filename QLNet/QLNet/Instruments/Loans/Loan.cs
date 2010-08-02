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

namespace QLNet
{
	public class Loan : Instrument
	{
		protected InitializedList<List<CashFlow>> legs_;
		protected InitializedList<double> payer_;
		protected List<double> notionals_;
		protected InitializedList<double?> legNPV_;

		public enum Type
		{
			Deposit = -1,
			Loan = 1
		}
	
		public enum Amortising
		{
			Bullet = 1,
			Step = 2,
			French = 3
		}

		public Loan(int legs)
		{
			legs_ = new InitializedList<List<CashFlow>>(legs);
			payer_ = new InitializedList<double>(legs);
			notionals_ = new List<double>();
			legNPV_ = new InitializedList<double?>(legs);
		}

		public override bool isExpired()
		{
			Date today = Settings.evaluationDate();
			return !legs_.Any<List<CashFlow>>(leg => leg.Any<CashFlow>(cf => !cf.hasOccurred(today)));
		}

		protected override void setupExpired()
		{
			base.setupExpired();
			legNPV_ = new InitializedList<double?>(legNPV_.Count);
		}

		public override void setupArguments(IPricingEngineArguments args)
		{
			LoanPricingEngineArguments arguments = args as LoanPricingEngineArguments;
			if (arguments == null) throw new ArgumentException("wrong argument type");

			arguments.legs.Clear();
			arguments.payer.Clear();

			arguments.legs.AddRange(legs_);
			arguments.payer.AddRange(payer_);
		}

		public override void fetchResults(IPricingEngineResults r)
		{
			base.fetchResults(r);

			LoanPricingEngineResults results = r as LoanPricingEngineResults;
			if (results == null) throw new ArgumentException("wrong result type");

			if (results.legNPV.Count != 0)
			{
				if (results.legNPV.Count != legNPV_.Count)
				{
					throw new ArgumentException("wrong number of leg NPV returned");
				}

				legNPV_ = results.legNPV;
			}
			else
			{
				legNPV_ = new InitializedList<double?>(legNPV_.Count);
			}
		}
	}
}
