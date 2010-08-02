/*
 Copyright (C) 2008 Toyin Akin (toyin_akin@hotmail.com)
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
using QLNet.Currencies;
using QLNet.Time.DayCounters;

namespace QLNet
{
	/// <summary>
	/// Base class for all BBA EUR Libor indexes but the O/N Euro LIBOR fixed by BBA.
	///
	/// See http://www.bba.org.uk/bba/jsp/polopoly.jsp?d=225&a=1414.
	/// </summary>
	/// <remarks>
	/// This is the rate fixed in London by BBA. Use Euribor if
	/// you're interested in the fixing by the ECB.
	/// </remarks>
	public class EURLibor : IborIndex
	{
		private readonly Calendar _target;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tenor"></param>
		/// <remarks>
		/// http://www.bba.org.uk/bba/jsp/polopoly.jsp?d=225&a=1412 :
		///		JoinBusinessDays is the fixing calendar for
		///		all indexes but o/n
		/// </remarks>
		public EURLibor(Period tenor)
			: this(tenor, new Handle<YieldTermStructure>())
		{
		}

		public EURLibor(Period tenor, Handle<YieldTermStructure> h)
			: base("EURLibor", tenor, 2, new EURCurrency(), new JointCalendar(new UnitedKingdom(UnitedKingdom.Market.Exchange), new TARGET(), JointCalendar.JointCalendarRule.JoinHolidays), Utils.eurliborConvention(tenor), Utils.eurliborEOM(tenor), new Actual360(), h)
		{
			if (this.tenor().TimeUnit == TimeUnit.Days)
			{
				throw new ApplicationException("for daily tenors (" + this.tenor() + ") dedicated DailyTenor constructor must be used");
			}

			_target = new TARGET();
		}

		public override Date valueDate(Date fixingDate)
		{
			if (!(isValidFixingDate(fixingDate)))
				throw new ApplicationException("Fixing date " + fixingDate + " is not valid");

			// http://www.bba.org.uk/bba/jsp/polopoly.jsp?d=225&a=1412 :
			// In the case of EUR the Value Date shall be two TARGET
			// business days after the Fixing Date.
			return _target.advance(fixingDate, fixingDays_, TimeUnit.Days);
		}

		public override Date maturityDate(Date valueDate)
		{
			// http://www.bba.org.uk/bba/jsp/polopoly.jsp?d=225&a=1412 :
			// In the case of EUR only, maturity dates will be based on days in
			// which the Target system is open.
			return _target.advance(valueDate, tenor_, convention_, EndOfMonth);
		}
	}
}
