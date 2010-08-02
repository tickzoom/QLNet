using System;
using QLNet.Currencies;
using QLNet.Time;

namespace QLNet
{
	/// <summary>
	/// Base class for O/N-S/N BBA LIBOR indexes but the EUR ones.
	/// 
	/// One day deposit LIBOR fixed by BBA.
	/// </summary>
	/// <remarks>
	/// See <http://www.bba.org.uk/bba/jsp/polopoly.jsp?d=225&a=1414>.
	/// </remarks>
	public class DailyTenorLibor : IborIndex
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="familyName"></param>
		/// <param name="settlementDays"></param>
		/// <param name="currency"></param>
		/// <param name="financialCenterCalendar"></param>
		/// <param name="dayCounter"></param>
		/// <remarks>
		/// http://www.bba.org.uk/bba/jsp/polopoly.jsp?d=225&a=1412 :
		/// no o/n or s/n fixings (as the case may be) will take place
		/// when the principal centre of the currency concerned is
		/// closed but London is open on the fixing day.
		/// </remarks>
		public DailyTenorLibor(string familyName, int settlementDays, Currency currency, Calendar financialCenterCalendar, DayCounter dayCounter)
			: this(familyName, settlementDays, currency, financialCenterCalendar, dayCounter, new Handle<YieldTermStructure>())
		{
		}

		public DailyTenorLibor(string familyName, int settlementDays, Currency currency, Calendar financialCenterCalendar, DayCounter dayCounter, Handle<YieldTermStructure> h)
			: base(familyName, new Period(1, TimeUnit.Days), settlementDays, currency, new JointCalendar(new UnitedKingdom(UnitedKingdom.Market.Exchange), financialCenterCalendar, JointCalendar.JointCalendarRule.JoinHolidays), Utils.liborConvention(new Period(1, TimeUnit.Days)), Utils.liborEOM(new Period(1, TimeUnit.Days)), dayCounter, h)
		{
			if (!(currency != new EURCurrency()))
			{
				throw new ApplicationException("for EUR Libor dedicated EurLibor constructor must be used");
			}
		}
	}
}