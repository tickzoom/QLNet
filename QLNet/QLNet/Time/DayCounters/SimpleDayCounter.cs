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

namespace QLNet.Time.DayCounters
{
	/// <summary>
	/// Simple day counter for reproducing theoretical calculations.
	/// 
	/// This day counter tries to ensure that whole-month distances are returned 
	/// as a simple fraction, i.e., 1 year = 1.0, 6 months = 0.5, 3 months = 0.25 and so forth.
	/// this day counter should be used together with <see cref="NullCalendar"/>, 
	/// which ensures that dates at whole-month distances share the same day of month. 
	/// It is <b>not</b> guaranteed to work with any other calendar.
	/// </summary>
	public class SimpleDayCounter : DayCounter
	{
		public SimpleDayCounter()
			: base(SimpleDayCounterImpl.Singleton)
		{
		}

		private class SimpleDayCounterImpl : DayCounter
		{
			public static readonly SimpleDayCounterImpl Singleton = new SimpleDayCounterImpl();

			private SimpleDayCounterImpl()
			{
			}

			public override string name()
			{
				return "Simple";
			}

			public override int dayCount(Date d1, Date d2)
			{
				return Thirty360.Thirty360USImpl.Singleton.dayCount(d1, d2);
			}

			public override double yearFraction(Date d1, Date d2, Date d3, Date d4)
			{
				int dm1 = d1.Day;
				int dm2 = d2.Day;

				if (dm1 == dm2 ||
					// e.g., Aug 30 -> Feb 28 ?
					(dm1 > dm2 && Date.isEndOfMonth(d2)) ||
					// e.g., Feb 28 -> Aug 30 ?
					(dm1 < dm2 && Date.isEndOfMonth(d1)))
				{
					return (d2.Year - d1.Year) + (d2.Month - d1.Month) / 12.0;
				}
				
				return Thirty360.Thirty360USImpl.Singleton.yearFraction(d1, d2, d3, d4);
			}
		}
	}
}