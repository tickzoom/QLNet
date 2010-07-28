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

using System.Collections.Generic;

namespace QLNet
{
	/// <summary>
	/// Interface for all value methods
	/// </summary>
	public interface IValue
	{
		double value(double v);
	}

	public struct Const
	{
		public const double QL_Epsilon = 2.2204460492503131e-016;

		public const double M_SQRT_2 = 0.7071067811865475244008443621048490392848359376887;
		public const double M_1_SQRTPI = 0.564189583547756286948;

		public const double M_LN2 = 0.693147180559945309417;
		public const double M_PI = 3.141592653589793238462643383280;
		public const double M_PI_2 = 1.57079632679489661923;
	}

	public class TimeSeries<T> : Dictionary<Date, T>
	{
		public TimeSeries() : base() { }
		public TimeSeries(int size) : base(size) { }
	}

	public struct Duration
	{
		public enum Type
		{
			Simple,
			Macaulay,
			Modified
		}
	}

	public struct Position
	{
		public enum Type
		{
			Long,
			Short
		}
	}

	public enum InterestRateType
	{
		Fixed,
		Floating
	}

	/// <summary>
	/// Interest rate coumpounding rule
	/// </summary>
	public enum Compounding
	{
		/// <summary>
		/// 1+rt
		/// </summary>
		Simple = 0,

		/// <summary>
		/// (1+r)^t
		/// </summary>
		Compounded = 1,

		/// <summary>
		/// e^(rt)
		/// </summary>
		Continuous = 2,

		/// <summary>
		/// Simple up to the first period then Compounded
		/// </summary>
		SimpleThenCompounded
	}

	public enum Month
	{
		January = 1,
		February = 2,
		March = 3,
		April = 4,
		May = 5,
		June = 6,
		July = 7,
		August = 8,
		September = 9,
		October = 10,
		November = 11,
		December = 12,

		Jan = 1,
		Feb = 2,
		Mar = 3,
		Apr = 4,
		Jun = 6,
		Jul = 7,
		Aug = 8,
		Sep = 9,
		Oct = 10,
		Nov = 11,
		Dec = 12
	}

	public enum BusinessDayConvention
	{
		/// <summary>
		/// Choose the first business day after the given holiday.
		/// </summary>
		Following,

		/// <summary>
		/// Choose the first business day after
		/// the given holiday unless it belongs
		/// to a different month, in which case
		/// choose the first business day before
		/// the holiday.
		/// </summary>
		ModifiedFollowing,

		/// <summary>
		/// Choose the first business day before the given holiday.
		/// </summary>
		Preceding,

		/// <summary>
		/// Choose the first business day before
		/// the given holiday unless it belongs
		/// to a different month, in which case
		/// choose the first business day after
		/// the holiday.
		/// </summary>
		ModifiedPreceding,

		/// <summary>
		/// Do not adjust.
		/// </summary>
		Unadjusted
	}

	/// <summary>
	/// Units used to describe time periods.
	/// </summary>
	public enum TimeUnit
	{
		Days,
		Weeks,
		Months,
		Years
	}

	public enum Frequency
	{
		/// <summary>
		/// Null frequency
		/// </summary>
		NoFrequency = -1,

		/// <summary>
		/// Only once, e.g., a zero-coupon
		/// </summary>
		Once = 0,

		/// <summary>
		/// Once a year.
		/// </summary>
		Annual = 1,

		/// <summary>
		/// Twice a year.
		/// </summary>
		Semiannual = 2,

		/// <summary>
		/// Every fourth month.
		/// </summary>
		EveryFourthMonth = 3,

		/// <summary>
		/// Every third month.
		/// </summary>
		Quarterly = 4,

		/// <summary>
		/// Every second month.
		/// </summary>
		Bimonthly = 6,

		/// <summary>
		/// Once a month.
		/// </summary>
		Monthly = 12,

		/// <summary>
		/// Every fourth week.
		/// </summary>
		EveryFourthWeek = 13,

		/// <summary>
		/// Every second week.
		/// </summary>
		Biweekly = 26,

		/// <summary>
		/// Once a week.
		/// </summary>
		Weekly = 52,

		/// <summary>
		/// Once a day.
		/// </summary>
		Daily = 365,

		/// <summary>
		/// Some other unknown frequency.
		/// </summary>
		OtherFrequency = 999
	}

	/// <summary>
	/// These conventions specify the rule used to generate dates in a Schedule.
	/// </summary>
	public struct DateGeneration
	{
		public enum Rule
		{
			/// <summary>
			/// Backward from termination date to effective date.
			/// </summary>
			Backward,

			/// <summary>
			/// Forward from effective date to termination date.
			/// </summary>
			Forward,

			/// <summary>
			/// No intermediate dates between effective date and termination date.
			/// </summary>
			Zero,

			/// <summary>
			/// All dates but effective date and termination date are taken to be on the third wednesday of their month.
			/// </summary>
			ThirdWednesday,

			/// <summary>
			/// All dates but the effective date are taken to be the twentieth of their
			/// month (used for CDS schedules in emerging markets). The termination
			/// date is also modified.
			/// </summary>
			Twentieth,

			/// <summary>
			/// All dates but the effective date are taken to be the twentieth of an IMM
			/// month (used for CDS schedules). The termination date is also modified.
			/// </summary>
			TwentiethIMM,

			/// <summary>
			/// Same as TwentiethIMM with unrestricted date ends and log/short stub 
			/// coupon period (old CDS convention).
			/// </summary>
			OldCDS,

			/// <summary>
			/// Credit derivatives standard rule since 'Big Bang' changes in 2009.
			/// </summary>
			CDS,
		}
	}

	public enum CapFloorType
	{
		Cap,
		Floor,
		Collar
	}
}