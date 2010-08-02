/*
 Copyright (C) 2008 Siarhei Novik (snovik@gmail.com)
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

namespace QLNet
{
	/// <summary>
	/// Bootstrap error
	/// </summary>
	public class BootstrapError : ISolver1d
	{
		private readonly PiecewiseYieldCurve _curve;
		private readonly RateHelper _helper;
		private readonly int _segment;

		public BootstrapError(PiecewiseYieldCurve curve, RateHelper helper, int segment)
		{
			_curve = curve;
			_helper = helper;
			_segment = segment;
		}

		public override double value(double guess)
		{
			_curve.updateGuess(_curve.data(), guess, _segment);
			_curve.interpolation_.update();
			return _helper.quoteError();
		}
	}
}
