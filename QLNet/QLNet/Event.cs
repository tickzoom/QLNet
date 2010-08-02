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
	/// <summary>
	/// This class acts as a base class for event implementations.
	/// </summary>
	public abstract class Event : DefaultObservable
	{
		/// <summary>
		/// Returns the date at which the event occurs
		/// </summary>
		/// <value></value>
		public abstract Date Date { get; }

		/// <summary>
		/// Returns true if an event has already occurred before a date
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public virtual bool hasOccurred(Date date)
		{
			return Date <= date;
		}

		/// <summary>
		/// Returns true if an event has already occurred before a date
		/// </summary>
		/// <param name="date"></param>
		/// <param name="includeToday"></param>
		/// <returns></returns>
		public virtual bool hasOccurred(Date date, bool includeToday)
		{
			if (includeToday) return Date < date;
			return Date <= date;
		}

		public virtual void accept(IAcyclicVisitor v)
		{
			if (v != null)
			{
				v.visit(this);
			}
			else
			{
				throw new ApplicationException("not an event visitor");
			}
		}
	}
}
