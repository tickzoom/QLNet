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
using QLNet.Patterns;

namespace QLNet
{
	public interface IQuote : IObservable
	{
		/// <summary>
		/// Returns true if the <see cref="Quote"/> holds a valid value, true by default
		/// </summary>
		/// <returns></returns>
		bool IsValid { get; }
	}

	/// <summary>
	/// Base class for market observables.
	/// </summary>
	public class Quote : DefaultObservable, IQuote
	{
		[Obsolete("Use Value method instead.")]
		public virtual double value()
		{
			return Value();
		}

		[Obsolete("Use IsValid property instead.")]
		public virtual bool isValid()
		{
			return true;
		}

		/// <summary>
		/// Returns true if the <see cref="Quote"/> holds a valid value, true by default
		/// </summary>
		/// <returns></returns>
		public bool IsValid { get { return true; } }

		/// <summary>
		/// Returns the current value, 0 by default
		/// </summary>
		/// <returns></returns>
		public virtual double Value()
		{
			return 0;
		}
	}
}