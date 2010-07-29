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

namespace QLNet.Time
{
	/// <summary>
	/// This class provides methods for determining the length of a time period according to given market convention,
	/// both as a number of days and as a year fraction.
	/// </summary>
	public class DayCounter
	{
		private DayCounter _dayCounter;

		public DayCounter dayCounter
		{
			get { return _dayCounter; }
			protected set { _dayCounter = value; }
		}

		/// <summary>
		/// The default constructor returns a day counter with a null implementation, 
		/// which is therefore unusable except as a placeholder.
		/// </summary>
		public DayCounter()
		{
		}

		protected DayCounter(DayCounter dayCounter)
		{
			_dayCounter = dayCounter;
		}

		public bool IsEmpty
		{
			get { return _dayCounter == null; }
		}

		[Obsolete("Use IsEmpty property instead.")]
		public bool empty()
		{
			return _dayCounter == null;
		}
		
		[Obsolete("Use Name property instead.")]
		public virtual string name()
		{
			if (empty()) return "No implementation provided";

			return _dayCounter.name();
		}

		public virtual string Name
		{
			get
			{
				if (IsEmpty)
				{
					return "No implementation provided";
				}

				return _dayCounter.Name;
			}
		}

		public virtual int dayCount(Date d1, Date d2)
		{
			if (IsEmpty)
			{
				throw Error.MissingImplementation();
			}

			return _dayCounter.dayCount(d1, d2);
		}

		public double yearFraction(Date d1, Date d2)
		{
			return yearFraction(d1, d2, d1, d2);
		}

		public virtual double yearFraction(Date d1, Date d2, Date refPeriodStart, Date refPeriodEnd)
		{
			if (IsEmpty)
			{
				throw Error.MissingImplementation();
			}

			return _dayCounter.yearFraction(d1, d2, refPeriodStart, refPeriodEnd);
		}

		public override string ToString()
		{
			return name();
		}

		public bool Equals(DayCounter other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.name(), name());
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(DayCounter)) return false;
			return Equals((DayCounter)obj);
		}

		public override int GetHashCode()
		{
			return (_dayCounter != null && _dayCounter.name() != null ? _dayCounter.name().GetHashCode() : 0);
		}

		public static bool operator ==(DayCounter d1, DayCounter d2)
		{
			return Equals(d1, d2);
		}

		public static bool operator !=(DayCounter d1, DayCounter d2)
		{
			return !Equals(d1, d2);
		}
	}
}