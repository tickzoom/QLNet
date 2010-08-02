using QLNet.Currencies;
using QLNet.Time;

namespace QLNet
{
	public class OvernightIndex : IborIndex
	{
		public OvernightIndex(string familyName, int settlementDays, Currency currency, Calendar fixingCalendar, DayCounter dayCounter, Handle<YieldTermStructure> h)
			: base(familyName, new Period(1, TimeUnit.Days), settlementDays, currency, fixingCalendar, BusinessDayConvention.Following, false, dayCounter, h)
		{
		}

		/// <summary>
		/// Returns a copy of itself linked to a different forwarding curve
		/// </summary>
		/// <param name="h"></param>
		/// <returns></returns>
		public override IborIndex clone(Handle<YieldTermStructure> h)
		{
			return new OvernightIndex(familyName(), fixingDays(), currency(), fixingCalendar(), dayCounter(), h);
		}
	}
}