using QLNet.Currencies;
using QLNet.Time.DayCounters;

namespace QLNet
{
	/// <summary>
	/// Base class for the one day deposit BBA CHF LIBOR indexes.
	/// </summary>
	public class DailyTenorCHFLibor : DailyTenorLibor
	{
		public DailyTenorCHFLibor(int settlementDays, Handle<YieldTermStructure> h)
			: base("CHFLibor", settlementDays, new CHFCurrency(), new Switzerland(), new Actual360(), h)
		{
		}
	}
}