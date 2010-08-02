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

using QLNet.Patterns;

namespace QLNet
{
	/// <summary>
	/// Base inflation-coupon pricer.
	/// 
	/// The main reason we can't use FloatingRateCouponPricer as the
	/// base is that it takes a FloatingRateCoupon which takes an
	/// InterestRateIndex and we need an inflation index (these are
	/// lagged).
	/// 
	/// The basic inflation-specific thing that the pricer has to do
	/// is deal with different lags in the index and the option
	/// e.g. the option could look 3 months back and the index 2.
	/// 
	/// We add the requirement that pricers do inverseCap/Floor-lets.
	/// These are cap/floor-lets as usually defined, i.e. pay out if
	/// underlying is above/below a strike.  The non-inverse (usual)
	/// versions are from a coupon point of view (a capped coupon has
	/// a maximum at the strike).
	/// 
	/// We add the inverse prices so that conventional caps can be
	/// priced simply.
	/// </summary>
	public class InflationCouponPricer : DefaultObservable, IObserver
	{
		public Handle<YieldTermStructure> RateCurve { get; protected set; }
		public Date PaymentDate { get; protected set; }

		public virtual double swapletPrice()
		{
			return 0;
		}

		public virtual double swapletRate()
		{
			return 0;
		}

		public virtual double capletPrice(double effectiveCap)
		{
			return 0;
		}

		public virtual double capletRate(double effectiveCap)
		{
			return 0;
		}

		public virtual double floorletPrice(double effectiveFloor)
		{
			return 0;
		}

		public virtual double floorletRate(double effectiveFloor)
		{
			return 0;
		}

		public virtual void initialize(InflationCoupon i)
		{
		}

		public void update()
		{
			notifyObservers();
		}
	}
}
