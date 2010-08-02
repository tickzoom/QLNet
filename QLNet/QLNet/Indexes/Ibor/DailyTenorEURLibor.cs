using QLNet.Currencies;
using QLNet.Time.DayCounters;

namespace QLNet
{
	/// <summary>
	/// Base class for the one day deposit BBA EUR Libor indexes Euro O/N LIBOR fixed by BBA. 
	/// 
	/// It can be also used for T/N and S/N indexes, even if such indexes do not have BBA fixing.
	/// 
	/// See http://www.bba.org.uk/bba/jsp/polopoly.jsp?d=225&a=1414.
	/// </summary>
	/// <remarks>
	/// This is the rate fixed in London by BBA. Use Eonia if
	/// you're interested in the fixing by the ECB.
	/// </remarks>
	public class DailyTenorEURLibor : IborIndex
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="settlementDays"></param>
		/// <remarks>
		/// http://www.bba.org.uk/bba/jsp/polopoly.jsp?d=225&a=1412 :
		///		no o/n or s/n fixings (as the case may be) will take place
		///		when the principal centre of the currency concerned is
		///		closed but London is open on the fixing day.
		/// </remarks>
		public DailyTenorEURLibor(int settlementDays)
			: this(settlementDays, new Handle<YieldTermStructure>())
		{
		}

		public DailyTenorEURLibor()
			: base("EURLibor", new Period(1, TimeUnit.Days), 0, new EURCurrency(), new TARGET(),
			       Utils.eurliborConvention(new Period(1, TimeUnit.Days)), Utils.eurliborEOM(new Period(1, TimeUnit.Days)), new Actual360(), new Handle<YieldTermStructure>())
		{
		}

		public DailyTenorEURLibor(int settlementDays, Handle<YieldTermStructure> h)
			: base("EURLibor", new Period(1, TimeUnit.Days), settlementDays, new EURCurrency(), new TARGET(),
			       Utils.eurliborConvention(new Period(1, TimeUnit.Days)), Utils.eurliborEOM(new Period(1, TimeUnit.Days)), new Actual360(), h)
		{
		}
	}
}