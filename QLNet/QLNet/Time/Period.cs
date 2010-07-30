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
	public struct Period
	{
		private const string UnknownFrequency = "unknown frequency";
		private const string UnknownTimeUnit = "unknown time unit";
		private const string IncompatibleTimeUnit = "incompatible time unit";
		private const string UndecidableComparison = "undecidable comparison";

		private int _length;
		private TimeUnit _timeUnit;

		public Period(int n, TimeUnit u)
		{
			_length = n; _timeUnit = u;
		}

		public Period(Frequency f)
		{
			switch (f)
			{
				case Frequency.NoFrequency:
					_timeUnit = TimeUnit.Days;
					_length = 0;
					break;

				case Frequency.Once:
					_timeUnit = TimeUnit.Years;
					_length = 0;
					break;

				case Frequency.Annual:
					_timeUnit = TimeUnit.Years;
					_length = 1;
					break;

				case Frequency.Semiannual:
				case Frequency.EveryFourthMonth:
				case Frequency.Quarterly:
				case Frequency.Bimonthly:
				case Frequency.Monthly:
					_timeUnit = TimeUnit.Months;
					_length = 12 / (int)f;
					break;

				case Frequency.EveryFourthWeek:
				case Frequency.Biweekly:
				case Frequency.Weekly:
					_timeUnit = TimeUnit.Weeks;
					_length = 52 / (int)f;
					break;

				case Frequency.Daily:
					_timeUnit = TimeUnit.Days;
					_length = 1;
					break;

				default:
					throw new ApplicationException(UnknownFrequency);
			}
		}

		/// <summary>
		/// Create from a string like "1M", "2Y"...
		/// </summary>
		/// <param name="period"></param>
		public Period(string period)
		{
			if (string.IsNullOrEmpty(period))
			{
				throw new ArgumentNullException("period");
			}

			TimeUnit timeUnit = TimeUnit.Unknow;
			int index;

			if ((index = period.IndexOf('d')) > 0 || (index = period.IndexOf('D')) > 0)
			{
				timeUnit = TimeUnit.Days;
			}
			else if ((index = period.IndexOf('w')) > 0 || (index = period.IndexOf('W')) > 0)
			{
				timeUnit = TimeUnit.Weeks;
			}
			else if ((index = period.IndexOf('m')) > 0 || (index = period.IndexOf('M')) > 0)
			{
				timeUnit = TimeUnit.Months;
			}
			else if ((index = period.IndexOf('y')) > 0 || (index = period.IndexOf('Y')) > 0)
			{
				timeUnit = TimeUnit.Years;
			}

			if (timeUnit == TimeUnit.Unknow)
			{
				throw new InvalidOperationException(UnknownTimeUnit);
			}

			int periodLength = Convert.ToInt32(period.Substring(0, index).Trim());

			_timeUnit = timeUnit;
			_length = periodLength;
		}

		[Obsolete("Use Length property instead.")]
		public int length()
		{
			return _length;
		}

		[Obsolete("Use Length property instead.")]
		public TimeUnit units()
		{
			return _timeUnit;
		}

		[Obsolete("Use Frequency property instead.")]
		public Frequency frequency()
		{
			return Frequency;
		}

		public int Length
		{
			get { return _length; }
		}

		public TimeUnit TimeUnit
		{
			get { return _timeUnit; }
		}

		public Frequency Frequency
		{
			get
			{
				int length = Math.Abs(_length);

				if (length == 0)
				{
					if (_timeUnit == TimeUnit.Years) return Frequency.Once;
					return Frequency.NoFrequency;
				}

				switch (_timeUnit)
				{
					case TimeUnit.Years:
						return (length == 1) ? Frequency.Annual : Frequency.OtherFrequency;

					case TimeUnit.Months:
						return (12 % length == 0 && length <= 12) ? (Frequency)(12 / length) : Frequency.OtherFrequency;

					case TimeUnit.Weeks:
						if (length == 1) return Frequency.Weekly;
						if (length == 2) return Frequency.Biweekly;
						if (length == 4) return Frequency.EveryFourthWeek;
						return Frequency.OtherFrequency;

					case TimeUnit.Days:
						return (length == 1) ? Frequency.Daily : Frequency.OtherFrequency;

					default:
						throw new ArgumentException(UnknownTimeUnit);
				}
			}
		}

		public void Normalize()
		{
			if (_length != 0)
				switch (_timeUnit)
				{
					case TimeUnit.Days:
						if ((_length % 7) == 0)
						{
							_length /= 7;
							_timeUnit = TimeUnit.Weeks;
						}
						break;

					case TimeUnit.Months:
						if ((_length % 12) == 0)
						{
							_length /= 12;
							_timeUnit = TimeUnit.Years;
						}
						break;

					case TimeUnit.Weeks:
					case TimeUnit.Years:
						break;

					default:
						throw new ArgumentException(UnknownTimeUnit);
				}
		}

		public override string ToString()
		{
			return "TimeUnit: " + _timeUnit + ", length: " + _length;
		}

		public string ToShortString()
		{
			string result = string.Empty;
			int n = Length;
			int m = 0;
			switch (TimeUnit)
			{
				case TimeUnit.Days:
					if (n >= 7)
					{
						m = n / 7;
						result += m + "W";
						n = n % 7;
					}

					return (n != 0 || m == 0) ? result + n + "D" : result;

				case TimeUnit.Weeks:
					return result + n + "W";

				case TimeUnit.Months:
					if (n >= 12)
					{
						m = n / 12;
						result += n / 12 + "Y";
						n = n % 12;
					}
					return (n != 0 || m == 0) ? result + n + "M" : result;

				case TimeUnit.Years:
					return result + n + "Y";

				default:
					throw new ApplicationException("unknown time unit (" + TimeUnit + ")");
			}
		}

		public bool Equals(Period other)
		{
			return other._length == _length && Equals(other._timeUnit, _timeUnit);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (obj.GetType() != typeof(Period)) return false;
			return Equals((Period)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (_length * 397) ^ _timeUnit.GetHashCode();
			}
		}

		public static bool operator ==(Period p1, Period p2)
		{
			return Equals(p1, p2);
		}

		public static bool operator !=(Period p1, Period p2)
		{
			return !Equals(p1, p2);
		}

		public static Period operator -(Period p)
		{
			return new Period(-1 * p.Length, p.TimeUnit);
		}

		public static Period operator -(Period p1, Period p2)
		{
			return p1 + (-1 * p2);
		}

		public static Period operator *(int n, Period p)
		{
			return new Period(n * p.Length, p.TimeUnit);
		}

		public static Period operator *(Period p, int n)
		{
			return new Period(n * p.Length, p.TimeUnit);
		}

		public static bool operator <=(Period p1, Period p2)
		{
			return p1 < p2 || p1 == p2;
		}

		public static bool operator >=(Period p1, Period p2)
		{
			return p2 >= p1;
		}

		public static bool operator >(Period p1, Period p2)
		{
			return p2 < p1;
		}

		public static Period operator +(Period p1, Period p2)
		{
			int newLength = p1.Length;
			TimeUnit newTimeUnit = p1.TimeUnit;

			if (newLength == 0)
			{
				newLength = p2.Length;
				newTimeUnit = p2.TimeUnit;
			}
			else if (newTimeUnit == p2.TimeUnit)
			{
				// no conversion needed
				newLength += p2.Length;
			}
			else
			{
				switch (newTimeUnit)
				{
					case TimeUnit.Years:
						switch (p2.TimeUnit)
						{
							case TimeUnit.Months:
								newTimeUnit = TimeUnit.Months;
								newLength = newLength * 12 + p2.Length;
								break;

							case TimeUnit.Weeks:
							case TimeUnit.Days:
								if (p1.Length != 0)
									throw new InvalidOperationException(IncompatibleTimeUnit);
								break;

							default:
								throw new ApplicationException(UnknownTimeUnit);
						}
						break;

					case TimeUnit.Months:
						switch (p2.TimeUnit)
						{
							case TimeUnit.Years:
								newLength += p2.Length * 12;
								break;

							case TimeUnit.Weeks:
							case TimeUnit.Days:
								throw new InvalidOperationException(IncompatibleTimeUnit);

							default:
								throw new ApplicationException(UnknownTimeUnit);
						}
						break;

					case TimeUnit.Weeks:
						switch (p2.TimeUnit)
						{
							case TimeUnit.Days:
								newTimeUnit = TimeUnit.Days;
								newLength = newLength * 7 + p2.Length;
								break;

							case TimeUnit.Years:
							case TimeUnit.Months:
								throw new InvalidOperationException(IncompatibleTimeUnit);

							default:
								throw new ApplicationException(UnknownTimeUnit);
						}
						break;

					case TimeUnit.Days:
						switch (p2.TimeUnit)
						{
							case TimeUnit.Weeks:
								newLength += p2.Length * 7;
								break;

							case TimeUnit.Years:
							case TimeUnit.Months:
								throw new InvalidOperationException(IncompatibleTimeUnit);

							default:
								throw new ApplicationException(UnknownTimeUnit);
						}
						break;

					default:
						throw new ApplicationException(UnknownTimeUnit);
				}
			}

			return new Period(newLength, newTimeUnit);
		}

		public static bool operator <(Period p1, Period p2)
		{
			// special cases
			if (p1.Length == 0) return (p2.Length > 0);
			if (p2.Length == 0) return (p1.Length < 0);

			// exact comparisons
			if (p1.TimeUnit == p2.TimeUnit) return p1.Length < p2.Length;
			if (p1.TimeUnit == TimeUnit.Months && p2.TimeUnit == TimeUnit.Years) return p1.Length < 12 * p2.Length;
			if (p1.TimeUnit == TimeUnit.Years && p2.TimeUnit == TimeUnit.Months) return 12 * p1.Length < p2.Length;
			if (p1.TimeUnit == TimeUnit.Days && p2.TimeUnit == TimeUnit.Weeks) return p1.Length < 7 * p2.Length;
			if (p1.TimeUnit == TimeUnit.Weeks && p2.TimeUnit == TimeUnit.Days) return 7 * p1.Length < p2.Length;

			// inexact comparisons (handled by converting to days and using limits)
			int period1MinDays = p1.GetMinDays();
			int period1MaxDays = p1.GetMaxDays();
			int period2MinDays = p2.GetMinDays();
			int period2MaxDays = p2.GetMaxDays();

			if (period1MaxDays < period2MinDays)
				return true;

			if (period1MinDays > period2MaxDays)
				return false;

			throw new ApplicationException(UndecidableComparison);
		}

		/// <summary>
		/// Converts Period to days.
		/// </summary>
		/// <returns></returns>
		private int GetMinDays()
		{
			switch (TimeUnit)
			{
				case TimeUnit.Years:
					return Length * 365;
				case TimeUnit.Months:
					return Length * 28;
				case TimeUnit.Weeks:
					return Length * 7;
				case TimeUnit.Days:
					return Length;
				default:
					throw new ApplicationException(UnknownTimeUnit);
			}
		}

		/// <summary>
		/// Converts Period to days.
		/// </summary>
		/// <returns></returns>
		private int GetMaxDays()
		{
			switch (TimeUnit)
			{
				case TimeUnit.Years:
					return Length * 366;
				case TimeUnit.Months:
					return Length * 31;
				case TimeUnit.Weeks:
					return Length * 7;
				case TimeUnit.Days:
					return Length;
				default:
					throw new ApplicationException(UnknownTimeUnit);
			}
		}
	}
}
