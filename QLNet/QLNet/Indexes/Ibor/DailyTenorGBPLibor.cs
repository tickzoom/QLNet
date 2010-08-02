using QLNet.Currencies;
using QLNet.Time.DayCounters;

namespace QLNet
{
	/// <summary>
	/// Base class for the one day deposit BBA GBP LIBOR indexes
	/// </summary>
	public class DailyTenorGBPLibor : DailyTenorLibor
	{
		public DailyTenorGBPLibor(int settlementDays, Handle<YieldTermStructure> h)
			: base("GBPLibor", settlementDays, new GBPCurrency(), new UnitedKingdom(UnitedKingdom.Market.Exchange), new Actual365Fixed(), h)
		{
		}
	}
}