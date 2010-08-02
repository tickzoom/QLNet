/*
 Copyright (C) 2008 Siarhei Novik (snovik@gmail.com)
  
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

namespace QLNet
{
	/// <summary>
	/// Binary superfund payoff
	/// 
	/// Superfund sometimes also called "supershare", which can lead to ambiguity; within QuantLib
	/// the terms supershare and superfund are used consistently according to the definitions in
	/// Bloomberg OVX function's help pages.
	/// 
	/// This payoff is equivalent to being (1/lowerstrike) a) long (short) an AssetOrNothing
	/// Call (Put) at the lower strike and b) short (long) an AssetOrNothing
	/// Call (Put) at the higher strike
	/// </summary>
	public class SuperFundPayoff : StrikedTypePayoff
	{
		private readonly double _secondStrike;

		public SuperFundPayoff(double strike, double secondStrike)
			: base(Option.Type.Call, strike)
		{
			_secondStrike = secondStrike;

			if (!(strike > 0.0))
				throw new ApplicationException("strike (" + strike + ") must be positive");

			if (!(secondStrike > strike))
				throw new ApplicationException("second strike (" + secondStrike + ") must be higher than first strike (" + strike + ")");
		}

		public double secondStrike()
		{
			return _secondStrike;
		}

		public override string name()
		{
			return "SuperFund";
		}
		
		public override double value(double price)
		{
			return (price >= strike_ && price < _secondStrike) ? price / strike_ : 0.0;
		}
	}
}
