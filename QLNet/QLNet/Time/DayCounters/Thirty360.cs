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

namespace QLNet.Time.DayCounters
{
	/// <summary>
	/// 30/360 day count convention
	/// 
	/// The 30/360 day count can be calculated according to US, European, or Italian conventions.
	/// </summary>
	public class Thirty360 : DayCounter
	{
		public enum Thirty360Convention
		{
			/// <summary>
			/// If the starting date is the 31st of a  month, it becomes equal to the 30th of the same month.
			/// If the ending date is the 31st of a month and the starting date is earlier than the 30th of a month, 
			/// the ending date  becomes equal to the 1st of the next month, otherwise the ending date becomes equal to the 30th of the same month.
			/// 
			/// Also known as "30/360", "360/360", or "Bond Basis"
			/// </summary>
			USA,

			/// <summary>
			/// If the starting date is the 31st of a  month, it becomes equal to the 30th of the same month.
			/// If the ending date is the 31st of a month and the starting date is earlier than the 30th of a month, 
			/// the ending date  becomes equal to the 1st of the next month, otherwise the ending date becomes equal to the 30th of the same month.
			/// </summary>
			BondBasis,

			/// <summary>
			/// Starting dates or ending dates that occur on the 31st of a month become equal to the 30th of the same month.
			/// Also known as "30E/360", or "Eurobond Basis"
			/// </summary>
			European,

			/// <summary>
			/// Starting dates or ending dates that occur on the 31st of a month become equal to the 30th of the same month.
			/// </summary>
			EurobondBasis,

			/// <summary>
			/// Starting dates or ending dates that occur on February and are grater than 27 become equal to 30 for computational sake.
			/// </summary>
			Italian
		}

		public Thirty360()
			: base(Thirty360USImpl.Singleton)
		{
		}

		public Thirty360(Thirty360Convention c)
			: base(GetDayCounterFromConvention(c))
		{
		}

		private static DayCounter GetDayCounterFromConvention(Thirty360Convention c)
		{
			switch (c)
			{
				case Thirty360Convention.USA:
				case Thirty360Convention.BondBasis:
					return Thirty360USImpl.Singleton;

				case Thirty360Convention.European:
				case Thirty360Convention.EurobondBasis:
					return Thirty360EUImpl.Singleton;

				case Thirty360Convention.Italian:
					return Thirty360ITImpl.Singleton;

				default:
					throw new ArgumentException("Unknown 30/360 convention: " + c);
			}
		}

		internal class Thirty360USImpl : DayCounter
		{
			public static readonly Thirty360USImpl Singleton = new Thirty360USImpl();

			private Thirty360USImpl()
			{
			}

			public override string name()
			{
				return "30/360 (Bond Basis)";
			}

			public override int dayCount(Date d1, Date d2)
			{
				int dd1 = d1.Day, dd2 = d2.Day;
				int mm1 = d1.Month, mm2 = d2.Month;
				int yy1 = d1.Year, yy2 = d2.Year;

				if (dd2 == 31 && dd1 < 30)
				{
					dd2 = 1; mm2++;
				}

				return 360 * (yy2 - yy1) + 30 * (mm2 - mm1 - 1) + Math.Max(0, 30 - dd1) + Math.Min(30, dd2);
			}

			public override double yearFraction(Date d1, Date d2, Date d3, Date d4) { return dayCount(d1, d2) / 360.0; }
		}

		private class Thirty360EUImpl : DayCounter
		{
			public static readonly Thirty360EUImpl Singleton = new Thirty360EUImpl();

			private Thirty360EUImpl()
			{
			}

			public override string name()
			{
				return "30E/360 (Eurobond Basis)";
			}

			public override int dayCount(Date d1, Date d2)
			{
				int dd1 = d1.Day, dd2 = d2.Day;
				int mm1 = d1.Month, mm2 = d2.Month;
				int yy1 = d1.Year, yy2 = d2.Year;

				return 360 * (yy2 - yy1) + 30 * (mm2 - mm1 - 1) + Math.Max(0, 30 - dd1) + Math.Min(30, dd2);
			}

			public override double yearFraction(Date d1, Date d2, Date d3, Date d4)
			{
				return dayCount(d1, d2) / 360.0;
			}
		}

		private class Thirty360ITImpl : DayCounter
		{
			public static readonly Thirty360ITImpl Singleton = new Thirty360ITImpl();

			private Thirty360ITImpl()
			{
			}

			public override string name()
			{
				return "30/360 (Italian)";
			}

			public override int dayCount(Date d1, Date d2)
			{
				int dd1 = d1.Day, dd2 = d2.Day;
				int mm1 = d1.Month, mm2 = d2.Month;
				int yy1 = d1.Year, yy2 = d2.Year;

				if (mm1 == 2 && dd1 > 27) dd1 = 30;
				if (mm2 == 2 && dd2 > 27) dd2 = 30;

				return 360 * (yy2 - yy1) + 30 * (mm2 - mm1 - 1) + Math.Max(0, 30 - dd1) + Math.Min(30, dd2);
			}

			public override double yearFraction(Date d1, Date d2, Date d3, Date d4)
			{
				return dayCount(d1, d2) / 360.0;
			}
		}
	}
}