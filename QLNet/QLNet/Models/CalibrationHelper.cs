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
using System.Collections.Generic;
using QLNet.Patterns;

namespace QLNet
{
	/// <summary>
	/// Liquid market instrument used during calibration
	/// </summary>
	public abstract class CalibrationHelper : DefaultObservable, IObserver
	{
		protected double marketValue_;
		protected Handle<Quote> volatility_;
		protected Handle<YieldTermStructure> termStructure_;
		protected IPricingEngine engine_;
		private readonly bool calibrateVolatility_;

		protected CalibrationHelper(Handle<Quote> volatility, Handle<YieldTermStructure> termStructure)
			: this(volatility, termStructure, false)
		{	
		}

		protected CalibrationHelper(Handle<Quote> volatility, Handle<YieldTermStructure> termStructure, bool calibrateVolatility)
		{
			if (volatility == null)
			{
				throw new ArgumentNullException("volatility");
			}

			if (termStructure == null)
			{
				throw new ArgumentNullException("termStructure");
			}
	
			volatility_ = volatility;
			termStructure_ = termStructure;
			calibrateVolatility_ = calibrateVolatility;

			volatility_.registerWith(update);
			termStructure_.registerWith(update);
		}

		public double marketValue()
		{
			return marketValue_;
		}

		/// <summary>
		/// Returns the price of the instrument according to the model
		/// </summary>
		/// <returns></returns>
		public abstract double modelValue();

		/// <summary>
		/// Returns the error resulting from the model valuation
		/// </summary>
		/// <returns></returns>
		public virtual double calibrationError()
		{
			if (calibrateVolatility_)
			{
				double lowerPrice = blackPrice(0.001);
				double upperPrice = blackPrice(10);
				double modelPrice = modelValue();

				double implied;
				if (modelPrice <= lowerPrice)
					implied = 0.001;
				else
					if (modelPrice >= upperPrice)
						implied = 10.0;
					else
						implied = this.impliedVolatility(modelPrice, 1e-12, 5000, 0.001, 10);

				return implied - volatility_.link.value();
			}
			
			return Math.Abs(marketValue() - modelValue()) / marketValue();
		}

		public abstract void addTimesTo(List<double> times);

		public double impliedVolatility(double targetValue, double accuracy, int maxEvaluations, double minVol, double maxVol)
		{
			CalibrationImpliedVolatilityHelper f = new CalibrationImpliedVolatilityHelper(this, targetValue);
			Brent solver = new Brent();
			solver.setMaxEvaluations(maxEvaluations);
			return solver.solve(f, accuracy, volatility_.link.value(), minVol, maxVol);
		}

		public abstract double blackPrice(double volatility);

		public void setPricingEngine(IPricingEngine engine)
		{
			engine_ = engine;
		}

		public void update()
		{
			marketValue_ = blackPrice(volatility_.link.value());
			notifyObservers();
		}

		private class CalibrationImpliedVolatilityHelper : ISolver1d
		{
			private readonly CalibrationHelper _helper;
			private readonly double _value;

			public CalibrationImpliedVolatilityHelper(CalibrationHelper helper, double value)
			{
				_helper = helper;
				_value = value;
			}

			public override double value(double x)
			{
				return _value - _helper.blackPrice(x);
			}
		}
	}
}
